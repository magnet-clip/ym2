﻿Imports AdfinXAnalyticsFunctions
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports ReutersData
Imports Uitls
Imports NLog

Namespace Tools.Elements
    Public Class RubIRS
        Inherits SwapCurve
        Implements IAswBenchmark

        Protected ReadOnly Descrs As New List(Of SwapPointDescription)
        Protected Overridable Property BaseInstrumentPrice As Double
        Protected ReadOnly DateModule = Eikon.Sdk.CreateAdxDateModule()

        '' LOGGER
        Private Shared ReadOnly Logger As Logger = Logging.GetLogger(GetType(RubIRS))

        '' SWAP STRUCTURES
        Private Shared ReadOnly SwapStructure =
            "LBOTH CLDR:RUS ARND:NO CFADJ:YES CRND:NO DMC:MODIFIED EMC:SAMEDAY IC:S1 " +
            "PDELAY:0 REFDATE:MATURITY RP:1 XD:NO LPAID LTYPE:FIXED CCM:A5P FRQ:Y " +
            "LRECEIVED LTYPE:FLOAT CCM:MMAA FRQ:Q"

        Protected Overridable ReadOnly Property Struct() As String
            Get
                Return SwapStructure
            End Get
        End Property

        Private Shared ReadOnly SwapFloatLeg =
            "CLDR:RUS ARND:NO CCM:MMAA CFADJ:YES CRND:NO DMC:MODIFIED EMC:SAMEDAY IC:S1 " +
            "PDELAY:0 REFDATE:MATURITY RP:1 RT:BULLET XD:NO FRQ:Q "

        Protected Overridable Property InstrumentName As String = "RUBAM3MO"
        Protected Overridable Property BaseInstrument As String = "MOSPRIME3MD="
        Protected Overridable Property AllowedTenors() As String() = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"}
        Protected Overridable Property Brokers() As String() = {"GFI", "TRDL", "ICAP", ""}

        Private Shared ReadOnly PossibleQuotes() As String = {"BID", "ASK", "MID"}

        Private _bootstrapped As Boolean

        '' LOADERS
        Private WithEvents _quoteLoader As New LiveQuotes

        '' DATA LOADING PARAMETERS
        Private _theGroupDate As Date = Date.Today
        Private _broker As String = ""
        Private _quote As String = "MID"
        Private ReadOnly _lastCurve As New Dictionary(Of IOrdinate, List(Of PointOfCurve))
        Private ReadOnly _yieldCurveModule As AdxYieldCurveModule = Eikon.Sdk.CreateAdxYieldCurveModule()
        Private Const InstrumentType As String = "S"

        Public Overrides ReadOnly Property IsSynthetic() As Boolean
            Get
                Return _bootstrapped
            End Get
        End Property

        Sub New(ByVal ansamble As Ansamble)
            MyBase.new(ansamble)
        End Sub

        Public Overrides Sub Subscribe()
            Logger.Debug("Subscirbe({0})", Identity)

            Clear()
            GetRICs(GetBroker()).ForEach(Sub(ric As String) Descrs.Add(New SwapPointDescription(ric)))

            If GroupDate() = Date.Today Then
                StartRealTime()
            Else
                LoadHistory()
            End If
        End Sub

        ''' <summary>
        ''' Parsing swap name to retrieve term
        ''' </summary>
        ''' <param name="ric">swap ric</param>
        ''' <returns>numeric value - swap tenor</returns>
        ''' <remarks></remarks>
        Protected Overridable Function GetDuration(ByVal ric As String) As Double
            Dim match = Regex.Match(ric, String.Format("{0}(?<year>[0-9]+?)Y.*", InstrumentName))
            Dim capture = match.Groups("year").Value
            Return CInt(capture)
        End Function

        ''' <summary>
        ''' Return full list of rics to be loaded
        ''' </summary>
        ''' <param name="broker">broker name</param>
        ''' <returns>a list of string</returns>
        ''' <remarks></remarks>
        Protected Overridable Function GetRICs(ByVal broker As String) As List(Of String)
            Return AllowedTenors.Select(Function(item) String.Format("{0}{1}Y={2}", InstrumentName, item, broker)).ToList()
        End Function

        '' START LOADING HISTORICAL DATA
        Protected Sub LoadHistory()
            Logger.Debug("LoadHistory")
            Dim rics = GetRICs(_broker)
            rics.ForEach(Sub(ric) DoLoadRIC(ric, "DATE, BID, ASK", _theGroupDate))
            DoLoadRIC(BaseInstrument, "DATE, CLOSE", _theGroupDate)
        End Sub

        Protected Sub DoLoadRic(ByVal ric As String, ByVal fields As String, ByVal aDate As Date)
            Logger.Debug("DoLoadRIC({0})", ric)
            If ric = "" Then Return

            Dim hst As History = New History()
            AddHandler hst.HistoricalData, AddressOf OnHistoricalData
            hst.StartTask(ric, fields, aDate.AddDays(-3), aDate)
            If hst.Finished Then Return
        End Sub

        '' START LOADING REALTIME DATA
        Protected Sub StartRealTime()
            _quoteLoader.AddItems(GetRICs(_broker), {"275", "393"}.ToList())
            If BaseInstrument <> "" Then
                _quoteLoader.AddItems({BaseInstrument}.ToList(), {"BID", "ASK"}.ToList())
            End If
        End Sub

        '' HISTORICAL DATA ARRIVED
        Protected Sub OnHistoricalData(ByVal ric As String, ByVal data As Dictionary(Of Date, HistoricalItem), ByVal rawData As Dictionary(Of DateTime, RawHistoricalItem))
            Logger.Debug("OnHistoricalData({0})", ric)
            If data IsNot Nothing Then
                Dim lastDate = data.Keys.Max
                Dim elem = data(lastDate)
                Dim aYield As Double
                Select Case _quote
                    Case "BID" : aYield = elem.Bid
                    Case "ASK" : aYield = elem.Ask
                    Case "MID" : aYield = If(elem.Bid > 0 And elem.Ask > 0, (elem.Bid + elem.Ask) / 2, If(elem.Bid > 0, elem.Bid, elem.Ask))
                End Select
                Try
                    If ric <> BaseInstrument Then
                        Dim item = (From descr In Descrs Where descr.RIC = ric).First
                        With item
                            .Yield(lastDate) = aYield / 100
                            .Duration = GetDuration(ric)
                            .YieldAtDate = lastDate
                        End With
                    Else
                        BaseInstrumentPrice = elem.Value
                    End If
                    Recalculate()
                Catch ex As Exception
                    Logger.WarnException("Failed to parse instrument " + ric, ex)
                    Logger.Warn("Exception = {0}", ex.ToString())
                End Try
            Else
                Logger.Warn("No data!")
            End If
        End Sub

        Public Overrides Sub Cleanup()
            Clear()
            NotifyCleanup()
        End Sub

        Private Sub Clear()
            Descrs.Clear()
            _quoteLoader.CancelItems(GetRICs(_broker))
            If BaseInstrument <> "" Then _quoteLoader.CancelItem(BaseInstrument)
        End Sub

        Public Overrides Sub Recalculate(ByVal ord As IOrdinate)
            If ord = Yield Then Throw New InvalidOperationException()
            _lastCurve(ord) = UpdateSpreads(ord)
            NotifyUpdatedSpread(_lastCurve(ord), ord)
        End Sub

        Public Overrides Sub Recalculate()
            _lastCurve(Yield) = UpdatePoints()
            For Each ord In Spreads
                _lastCurve(ord) = UpdateSpreads(ord)
            Next

            If Ansamble.YSource = Yield Then
                NotifyUpdated(_lastCurve(Yield))
            ElseIf Ansamble.YSource.Belongs(AswSpread, OaSpread, ZSpread, PointSpread) Then
                If _lastCurve.ContainsKey(Ansamble.YSource) Then NotifyUpdated(_lastCurve(Ansamble.YSource))
            Else
                Logger.Warn("Unknown spread type {0}", Ansamble.YSource)
            End If
        End Sub

        Public Overrides Sub RecalculateTotal()
            Recalculate()
        End Sub

        Public Overrides Sub Disable(ByVal ric As String)
        End Sub

        Public Overrides Sub Disable(ByVal rics As List(Of String))
        End Sub

        Public Overrides Sub Enable(ByVal rics As List(Of String))
        End Sub

        Public Overrides ReadOnly Property DisabledElements() As List(Of Bond)
            Get
                Return New List(Of Bond)()
            End Get
        End Property


        Private Function UpdateSpreads(ByVal ordinate As IOrdinate) As List(Of PointOfCurve)
            SetSpread(ordinate)
            Dim res = New List(Of PointOfCurve)(From item In Descrs
                                             Let theY = ordinate.GetValue(item)
                                             Where theY.HasValue
                                             Select New JustPoint(item.Duration, theY, Me))
            res.Sort()
            Return res
        End Function

        Private Function UpdatePoints() As List(Of PointOfCurve)
            Dim result As New List(Of PointOfCurve)
            If _bootstrapped Then
                Try
                    Dim data = (From elem In Descrs Where elem.Yield.HasValue AndAlso elem.Yield > 0).ToList()

                    Dim params(0 To data.Count() - 1, 5) As Object
                    For i = 0 To data.Count - 1
                        params(i, 0) = InstrumentType
                        params(i, 1) = GroupDate
                        params(i, 2) = GroupDate.AddDays(data(i).Duration * 365.0)
                        params(i, 3) = BaseInstrumentPrice / 100
                        params(i, 4) = data(i).Yield
                        params(i, 5) = Struct
                    Next
                    Dim termStructure As Array = _yieldCurveModule.AdTermStructure(params, "RM:YC ZCTYPE:RATE IM:CUBX ND:DIS", Nothing)
                    For i = termStructure.GetLowerBound(0) To termStructure.GetUpperBound(0)
                        Dim dur = (Utils.FromExcelSerialDate(termStructure.GetValue(i, 1)) - GroupDate).TotalDays / 365.0
                        Dim yld = termStructure.GetValue(i, 2)
                        If dur > 0 And yld > 0 Then result.Add(New JustPoint(dur, yld, Me))
                    Next
                Catch ex As Exception
                    Logger.ErrorException("Failed to bootstrap", ex)
                    Logger.Error("Exception = {0}", ex.ToString())
                    Return result
                End Try
            Else
                result.AddRange((From item In Descrs
                                   Let x = item.Duration, y = item.Yield
                                   Where x > 0 And y > 0
                                   Select New PointOfSwapCurve(x, y, Me, item.RIC)).
                                   Cast(Of PointOfCurve))
            End If
            result.Sort()
            Return result
        End Function

        '' REALTIME DATA ARRIVED
        Private Sub OnRealTimeData(ByVal data As Dictionary(Of String, Dictionary(Of String, Double))) Handles _quoteLoader.NewData
            Logger.Debug("OnRealTimeData")
            For Each rfv As KeyValuePair(Of String, Dictionary(Of String, Double)) In data
                Dim ric = rfv.Key
                Dim fv = rfv.Value

                If GetRICs(_broker).Contains(ric) Then
                    Logger.Trace("Got RIC {0}", ric)
                    ' define yield curve elem
                    Dim duration = GetDuration(ric)
                    If fv.Keys.Contains("393") Or fv.Keys.Contains("275") Then
                        Try
                            Dim elem = (From descr In Descrs Where descr.RIC = ric).First
                            elem.YieldAtDate = GroupDate
                            If _quote = "BID" Or _quote = "ASK" Then
                                Dim yld As Double
                                yld = CDbl(fv(IIf(_quote = "BID", "393", "275")))
                                If yld > 0 Then
                                    elem.Yield(Today) = yld / 100
                                    elem.Duration = duration
                                    Recalculate()
                                End If
                            Else
                                Dim bidYield = CDbl(fv("393")) / 100
                                Dim askYield = CDbl(fv("275")) / 100
                                Dim found = True
                                If bidYield > 0 And askYield > 0 Then
                                    elem.Yield(Today) = (bidYield + askYield) / 2
                                ElseIf bidYield > 0 Then
                                    elem.Yield(Today) = bidYield
                                ElseIf askYield > 0 Then
                                    elem.Yield(Today) = askYield
                                Else
                                    found = False
                                End If
                                If found Then
                                    elem.Duration = duration
                                    Recalculate()
                                End If
                            End If
                        Catch ex As Exception
                            Logger.WarnException("Failed to parse realtime data", ex)
                            Logger.Warn("Exception = {0}", ex.ToString())
                        End Try
                    End If
                ElseIf BaseInstrument = ric Then
                    Logger.Trace("Got base instrument {0}", BaseInstrument)
                    If fv.Keys.Contains("BID") Or fv.Keys.Contains("ASK") Then
                        Try
                            Dim found = False
                            If _quote = "BID" Or _quote = "ASK" Then
                                Dim yld As Double
                                yld = CDbl(fv(_quote))
                                If yld > 0 Then
                                    BaseInstrumentPrice = yld
                                    found = True
                                End If
                            Else
                                found = True
                                Dim bidYield = CDbl(fv("BID"))
                                Dim askYield = CDbl(fv("ASK"))
                                If bidYield > 0 And askYield > 0 Then
                                    BaseInstrumentPrice = (bidYield + askYield) / 2
                                ElseIf bidYield > 0 Then
                                    BaseInstrumentPrice = bidYield
                                ElseIf askYield > 0 Then
                                    BaseInstrumentPrice = askYield
                                Else
                                    found = False
                                End If
                            End If
                            If found Then Recalculate()
                        Catch ex As Exception
                            Logger.WarnException("Failed to parse realtime base data", ex)
                            Logger.Warn("Exception = {0}", ex.ToString())
                        End Try
                    End If
                End If
            Next
        End Sub

        Public Overrides ReadOnly Property Snapshot() As ISnapshot
            Get
                Return New CurveSnapshot(_lastCurve, Ansamble)
            End Get
        End Property

        Public Overrides ReadOnly Property Formula() As String
            Get
                Return "N/A"
            End Get
        End Property

        Public Overrides ReadOnly Property CanBootstrap() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Property Bootstrapped() As Boolean
            Get
                Return _bootstrapped
            End Get
            Set(ByVal value As Boolean)
                _bootstrapped = value
                Recalculate()
            End Set
        End Property

        Public Overrides Sub Bootstrap()
            Bootstrapped = Not Bootstrapped
        End Sub

        Public Overrides Sub ClearSpread(ByVal ySource As OrdinateBase)
            For Each item In Descrs
                ySource.ClearValue(item)
            Next
        End Sub

        Public Overrides Sub SetSpread(ByVal ySource As OrdinateBase)
            If Ansamble.Benchmarks.Keys.Contains(ySource) AndAlso Ansamble.Benchmarks(ySource) <> Me Then
                For Each item In Descrs
                    ySource.SetValue(item, Ansamble.Benchmarks(ySource))
                Next
            Else
                For Each item In Descrs
                    ySource.ClearValue(item)
                Next
            End If
        End Sub

        Public Overrides Function RateArray() As Array
            If _lastCurve.ContainsKey(Yield) Then
                Dim list = (From elem In _lastCurve(Yield) Select New XY(elem.TheX, elem.TheY)).ToList()
                list.Sort()
                Dim len = list.Count - 1
                Dim res(0 To len, 1) As Object
                For i = 0 To len
                    res(i, 0) = DateTime.Today.AddDays(TimeSpan.FromDays(list(i).X * 365).TotalDays)
                    res(i, 1) = list(i).Y
                Next
                Return res
            Else
                Return Nothing
            End If
        End Function

        '' OVERRIDEN METHODS
        Public Overrides Function GetBrokers() As String()
            Return Brokers
        End Function

        Public Overrides Sub SetBroker(ByVal b As String)
            _broker = b
            Subscribe()
        End Sub

        Public Overrides Function GetBroker() As String
            Return _broker
        End Function

        Public Overrides Function GetQuotes() As String()
            Return PossibleQuotes
        End Function

        Public Overrides Sub SetQuote(ByVal b As String)
            _quote = b
            Subscribe()
        End Sub

        Public Overrides Function GetQuote() As String
            Return _quote
        End Function

        Public Overrides Property GroupDate() As Date
            Get
                Return _theGroupDate
            End Get
            Set(ByVal value As Date)
                _theGroupDate = value
                Subscribe()
            End Set
        End Property

        Public Overrides ReadOnly Property Name As String
            Get
                Dim dt = GroupDate()
                Dim dateStr = IIf(dt <> DateTime.Today, String.Format("{0:dd/MM/yy}", dt), "Today")

                Dim broker = GetBroker()
                If broker.Trim().Length = 0 Then
                    Return String.Format("{0} ({1}, {2})", Me.GetType().Name, GetQuote, dateStr)
                Else
                    Return String.Format("{0} ({1}, {2} by {3})", Me.GetType().Name, GetQuote, dateStr, broker)
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property OuterColor() As Color
            Get
                Return Color.Firebrick
            End Get
        End Property

        Public Overrides ReadOnly Property InnerColor() As Color
            Get
                Return Color.NavajoWhite
            End Get
        End Property

        Public Overridable Function BenchmarkEnabled() As Boolean Implements IAswBenchmark.CanBeBenchmark
            Return True
        End Function

        Public Overridable ReadOnly Property FloatLegStructure() As String Implements IAswBenchmark.FloatLegStructure
            Get
                Return SwapFloatLeg
            End Get
        End Property

        Public Overridable ReadOnly Property FloatingPointValue() As Double Implements IAswBenchmark.FloatingPointValue
            Get
                Return 0
            End Get
        End Property
    End Class

    Public NotInheritable Class RubCCS
        Inherits RubIRS

        Public Sub New(ByVal ansamble As Ansamble)
            MyBase.New(ansamble)
        End Sub

        Protected Overrides Property InstrumentName() As String = "RUUSAM3L"
        Protected Overrides Property AllowedTenors() As String() = {"1", "2", "3", "4", "5", "6", "7", "10", "15", "20"}
        Protected Overrides Property BaseInstrument As String = ""

        Protected Overrides Property BaseInstrumentPrice As Double
            Set(ByVal value As Double)
            End Set
            Get
                Return 0
            End Get
        End Property

        Public Overrides ReadOnly Property OuterColor() As Color
            Get
                Return Color.MidnightBlue
            End Get
        End Property

        Public Overrides ReadOnly Property InnerColor() As Color
            Get
                Return Color.LightSteelBlue
            End Get
        End Property
    End Class

    Public Class RubNDF
        Inherits RubIRS


        Public Sub New(ByVal ansamble As Ansamble)
            MyBase.New(ansamble)
        End Sub
        Protected Overrides Property InstrumentName() As String = "RUB"
        Protected Overrides Property AllowedTenors() As String() = {"1W", "2W", "1M", "2M", "3M", "6M", "9M", "1Y", "18M", "2Y", "3Y", "4Y", "5Y"}
        Protected Overrides Property Brokers() As String() = {"GFI", "TRDL", "ICAP", "R", ""}
        Protected Overrides Property BaseInstrument As String = ""

        Public Overrides ReadOnly Property OuterColor() As Color
            Get
                Return Color.OrangeRed
            End Get
        End Property

        Public Overrides ReadOnly Property InnerColor() As Color
            Get
                Return Color.Orange
            End Get
        End Property

        Public Overrides ReadOnly Property CanBootstrap As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Function GetDuration(ByVal ric As String) As Double
            Dim match = Regex.Match(ric, String.Format("{0}(?<term>[0-9]+?[DWMY])ID=.*", InstrumentName))
            Dim term = match.Groups("term").Value
            Dim dt As Date = GroupDate()
            Dim aDate As Array = DateModule.DfAddPeriod("RUS", dt, term, "")
            Return DateModule.DfCountYears(dt, Utils.FromExcelSerialDate(aDate.GetValue(1, 1)), "")
        End Function

        Public Overrides Function BenchmarkEnabled() As Boolean
            Return False
        End Function

        Protected Overrides Function GetRICs(ByVal broker As String) As List(Of String)
            Return AllowedTenors.Select(Function(item) String.Format("{0}{1}ID={2}", InstrumentName, item, broker)).ToList()
        End Function
    End Class

    Public Class UsdIRS
        Inherits RubIRS

        Public Sub New(ByVal ansamble As Ansamble)
            MyBase.New(ansamble)
        End Sub
        Protected Overrides Property InstrumentName() As String = "USDAM3L"
        Protected Overrides Property AllowedTenors() As String() = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "20", "25", "30"}
        Protected Overrides Property Brokers() As String() = {"TRDL", "ICAP", ""}
        Protected Overrides Property BaseInstrument As String = "USD3MFSR="


        Public Overrides ReadOnly Property OuterColor() As Color
            Get
                Return Color.DarkKhaki
            End Get
        End Property

        Public Overrides ReadOnly Property InnerColor() As Color
            Get
                Return Color.Khaki
            End Get
        End Property

        Protected Overrides ReadOnly Property Struct() As String
            Get
                Return "LBOTH CLDR:USA ARND:NO CFADJ:YES CRND:NO DMC:MODIFIED EMC:SAMEDAY IC:S1 PDELAY:0 REFDATE:MATURITY RP:1 XD:NO LPAID LTYPE:FIXED CCM:BB00 FRQ:S LRECEIVED LTYPE:FLOAT CCM:MMA0 FRQ:Q"
            End Get
        End Property

        Protected Overrides Function GetDuration(ByVal ric As String) As Double
            Dim match = Regex.Match(ric, String.Format("{0}(?<term>[0-9]+?Y)=.*", InstrumentName))
            Dim term = match.Groups("term").Value
            Dim dt As Date = GroupDate
            Dim aDate As Array = DateModule.DfAddPeriod("USA", dt, term, "")
            Return DateModule.DfCountYears(dt, Utils.FromExcelSerialDate(aDate.GetValue(1, 1)), "")
        End Function

        Public Overrides Function BenchmarkEnabled() As Boolean
            Return True
        End Function

        Protected Overrides Function GetRICs(ByVal broker As String) As List(Of String)
            Return AllowedTenors.Select(Function(item) String.Format("{0}{1}Y={2}", InstrumentName, item, broker)).ToList()
        End Function

        Public Overrides ReadOnly Property FloatLegStructure() As String
            Get
                Return "CLDR:USA  ARND:NO CCM:MMA0 CFADJ:YES CRND:NO DMC:MODIFIED EMC:SAMEDAY IC:S1 PDELAY:0  REFDATE:MATURITY RP:1 XD:NO FRQ:Q"
            End Get
        End Property
    End Class

    Public NotInheritable Class EurIRS
        Inherits UsdIRS

        Public Sub New(ByVal ansamble As Ansamble)
            MyBase.New(ansamble)
        End Sub
        Protected Overrides Property InstrumentName() As String = "EURAB3E"
        Protected Overrides Property AllowedTenors() As String() = {"1Y", "18M", "2Y", "3Y", "4Y", "5Y", "6Y", "7Y", "8Y", "9Y", "10Y", "11Y", "12Y", "15Y", "20Y", "25Y", "30Y", "40Y", "50Y"}
        Protected Overrides Property Brokers() As String() = {""}
        Protected Overrides Property BaseInstrument As String = "EUR3MFSR="


        Protected Overrides Function GetDuration(ByVal ric As String) As Double
            Dim match = Regex.Match(ric, String.Format("{0}(?<term>[0-9]+[MY]?)=.*", InstrumentName))
            Dim term = match.Groups("term").Value
            Dim dt As Date = GroupDate
            Dim aDate As Array = DateModule.DfAddPeriod("USA", dt, term, "")
            Return DateModule.DfCountYears(dt, Utils.FromExcelSerialDate(aDate.GetValue(1, 1)), "")
        End Function

        Protected Overrides Function GetRICs(ByVal broker As String) As List(Of String)
            Return AllowedTenors.Select(Function(item) String.Format("{0}{1}={2}", InstrumentName, item, broker)).ToList()
        End Function

        Public Overrides ReadOnly Property OuterColor() As Color
            Get
                Return Color.Teal
            End Get
        End Property

        Public Overrides ReadOnly Property InnerColor() As Color
            Get
                Return Color.DarkBlue
            End Get
        End Property

        Protected Overrides ReadOnly Property Struct() As String
            Get
                Return "LBOTH CLDR:EMU ARND:NO CFADJ:YES CRND:NO DMC:MODIFIED EMC:SAMEDAY IC:S1 PDELAY:0 REFDATE:MATURITY RP:1 XD:NO LPAID LTYPE:FIXED CCM:BB00 FRQ:Y LRECEIVED LTYPE:FLOAT CCM:MMA0 FRQ:Q"
            End Get
        End Property

        Public Overrides ReadOnly Property FloatLegStructure() As String
            Get
                Return "CLDR:EMU ARND:NO CCM:MMA0 CFADJ:YES CRND:NO DMC:MODIFIED EMC:SAMEDAY IC:S1 PDELAY:0  REFDATE:MATURITY RP:1 XD:NO FRQ:Q"
            End Get
        End Property
    End Class

    Public Class UahNDF
        Inherits RubNDF

        Public Sub New(ByVal ansamble As Ansamble)
            MyBase.New(ansamble)
        End Sub
        Protected Overrides Property InstrumentName() As String = "UAH"
        Protected Overrides Property AllowedTenors() As String() = {"1W", "2W", "1M", "2M", "3M", "6M", "9M", "1Y"}
        Protected Overrides Property Brokers() As String() = {""}
        Protected Overrides Property BaseInstrument As String = ""

        Public Overrides ReadOnly Property OuterColor() As Color
            Get
                Return Color.MediumVioletRed
            End Get
        End Property

        Public Overrides ReadOnly Property InnerColor() As Color
            Get
                Return Color.DodgerBlue
            End Get
        End Property

        Protected Overrides Function GetDuration(ByVal ric As String) As Double
            Dim match = Regex.Match(ric, String.Format("{0}(?<term>[0-9]+?[DWMY])ID=.*", InstrumentName))
            Dim term = match.Groups("term").Value
            Dim dt As Date = GroupDate()
            Dim aDate As Array = DateModule.DfAddPeriod("RUS", dt, term, "")
            Return DateModule.DfCountYears(dt, Utils.FromExcelSerialDate(aDate.GetValue(1, 1)), "")
        End Function

        Protected Overrides Function GetRICs(ByVal broker As String) As List(Of String)
            Return AllowedTenors.Select(Function(item) String.Format("{0}{1}ID={2}", InstrumentName, item, broker)).ToList()
        End Function
    End Class
End Namespace