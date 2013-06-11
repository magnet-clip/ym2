Imports AdfinXAnalyticsFunctions
Imports System.ComponentModel
Imports DbManager.Bonds
Imports DbManager
Imports Settings
Imports Uitls
Imports ReutersData

Namespace Tools.Elements
    Public Class SyntheticZcb
        Inherits Bond
        Public Sub New(ByVal parent As Group, ByVal metaData As BondMetadata)
            MyBase.New(parent, metaData)
        End Sub

        Public Overrides ReadOnly Property Coupon(ByVal dt As Date) As Object
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class BondCurve
        Inherits Group
        Implements ICurve

        Private Const ZcbPmtStructure As String = _
           "ACC:A5 IC:L1 CLDR:RUS_FI SETTLE:0WD CFADJ:NO DMC:FOLLOWING EMC:LASTDAY FRQ:ZERO " &
           "PX:CLEAN REFDATE:MATURITY YM:DISCA5 ISSUE:01JAN2013"

        Private ReadOnly _bondModule As AdxBondModule = New AdxBondModule
        Private ReadOnly _curveModule As AdxYieldCurveModule = New AdxYieldCurveModule

        Public Class BondCurveSnapshot
            ''' <summary>
            ''' Technical class to represent bond curve structure
            ''' </summary>
            ''' <remarks></remarks>
            Public Class BondCurveElement
                Implements IComparable(Of BondCurveElement)
                Private ReadOnly _ric As String
                Private ReadOnly _descr As String
                Private ReadOnly _yield As Double
                Private ReadOnly _duration As Double
                Private ReadOnly _price As Double
                Private ReadOnly _quote As String
                Private ReadOnly _yieldDate As Date

                Public Sub New(ByVal ric As String, ByVal descr As String, ByVal [yield] As Double, ByVal duration As Double, ByVal price As Double, ByVal quote As String, ByVal yieldDate As Date)
                    _ric = ric
                    _descr = descr
                    _yield = yield
                    _duration = duration
                    _price = price
                    _quote = quote
                    _yieldDate = yieldDate
                End Sub

                Public ReadOnly Property RIC() As String
                    Get
                        Return _ric
                    End Get
                End Property

                Public ReadOnly Property Descr() As String
                    Get
                        Return _descr
                    End Get
                End Property

                Public ReadOnly Property Yield() As String
                    Get
                        Return String.Format("{0:P2}", _yield)
                    End Get
                End Property

                Public ReadOnly Property Duration() As String
                    Get
                        Return String.Format("{0:F2}", _duration)
                    End Get
                End Property

                Public ReadOnly Property Price() As String
                    Get
                        Return String.Format("{0:F4}", _price)
                    End Get
                End Property

                Public ReadOnly Property Quote() As String
                    Get
                        Return _quote
                    End Get
                End Property

                <DisplayName("Yield date")>
                Public ReadOnly Property YieldDate() As String
                    Get
                        Return String.Format("{0:dd/MM/yyyy}", _yieldDate)
                    End Get
                End Property

                Public Function CompareTo(ByVal other As BondCurveElement) As Integer Implements IComparable(Of BondCurveElement).CompareTo
                    Return _duration.CompareTo(other._duration)
                End Function
            End Class

            Private ReadOnly _ansamble As Ansamble

            Private ReadOnly _spreads As New Dictionary(Of IOrdinate, List(Of BondSpreadCurveItem))

            Public ReadOnly Property Spreads() As Dictionary(Of IOrdinate, List(Of BondSpreadCurveItem))
                Get
                    Return _spreads
                End Get
            End Property

            Private ReadOnly _current As List(Of CurveItem)
            Public ReadOnly Property Current() As List(Of CurveItem)
                Get
                    Return _current
                End Get
            End Property

            Private ReadOnly _disabledElements As New List(Of BondCurveElement)
            Public ReadOnly Property DisabledElements() As List(Of BondCurveElement)
                Get
                    Return _disabledElements
                End Get
            End Property

            Private ReadOnly _enabledElements As New List(Of BondCurveElement)
            Public ReadOnly Property EnabledElements() As List(Of BondCurveElement)
                Get
                    Return _enabledElements
                End Get
            End Property

            Public Sub New(ByVal bonds As List(Of Bond), ByVal items As List(Of CurveItem), ByVal ansamble As Ansamble)
                _ansamble = ansamble
                For Each bond In bonds
                    Dim mainQuote = bond.QuotesAndYields.Main
                    If mainQuote Is Nothing Then Continue For
                    If bond.Enabled Then
                        _enabledElements.Add(New BondCurveElement(bond.MetaData.RIC, bond.Label, mainQuote.Yield, mainQuote.Duration, mainQuote.Price, bond.QuotesAndYields.MaxPriorityField, mainQuote.YieldAtDate))
                    Else
                        _disabledElements.Add(New BondCurveElement(bond.MetaData.RIC, bond.Label, mainQuote.Yield, mainQuote.Duration, mainQuote.Price, bond.QuotesAndYields.MaxPriorityField, mainQuote.YieldAtDate))
                    End If
                Next
                _enabledElements.Sort()
                _disabledElements.Sort()
                _current = New List(Of CurveItem)(items)
                '+ bond.UserDefinedSpread()
                For Each ord In From q In Ordinate.Spreads Where _ansamble.Benchmarks.HasOrd(q)
                    Dim tmp = ord
                    _spreads(tmp) = New List(Of BondSpreadCurveItem)(
                        From bond In bonds
                        Let mainQuote = bond.QuotesAndYields.Main
                        Where mainQuote IsNot Nothing
                        Let vle = tmp.GetValue(mainQuote)
                        Where vle.HasValue
                        Select New BondSpreadCurveItem(mainQuote.Duration, tmp.GetValue(mainQuote), bond.MetaData.RIC, bond.MetaData.ShortName)
                    )
                    _spreads(tmp).Sort()
                Next ord
            End Sub
        End Class

        Private _curveDate As Date = Today
        Public Property CurveDate() As Date Implements ICurve.CurveDate
            Get
                Return _curveDate
            End Get
            Set(ByVal value As Date)
                If _curveDate <> value Then
                    _curveDate = value
                    Subscribe()
                End If
            End Set
        End Property

        Private ReadOnly _histFields As FieldContainer

        ' Last curve snapshot
        Private ReadOnly _lastCurve As New Dictionary(Of IOrdinate, List(Of CurveItem))
        Private _lastSyntCurve As List(Of SyntheticZcb)

        Private _formula As String
        Public ReadOnly Property Formula() As String
            Get
                Return _formula
            End Get
        End Property

        Public ReadOnly Property IsSynthetic() As Boolean
            Get
                Return _bootstrapped OrElse _estModel IsNot Nothing
            End Get
        End Property

        Private Function GetSyntBond(dur As Double, yield As Double) As SyntheticZcb
            Dim mat = _curveDate.AddDays(dur * 365)
            Dim bond = New SyntheticZcb(Me, New BondMetadata(String.Format("ZCB {0:N2}", dur), mat, 0, ZcbPmtStructure, "RM:YTM", Name))
            Dim settleDate = _bondModule.BdSettle(_curveDate, ZcbPmtStructure)
            Dim priceObject As Array = _bondModule.AdBondPrice(settleDate, yield, mat, 0, 0, ZcbPmtStructure, "RM:YTM", "", "RES:BDPRICE")
            AddHandler bond.CustomPrice, Sub(bnd, prc) HandleNewQuote(bnd, BondFields.XmlName(bond.Fields.Custom), prc, _curveDate, False)
            bond.SetCustomPrice(100 * priceObject.GetValue(1))
            Return bond
        End Function

        Private _bootstrapped As Boolean

        Public ReadOnly Property CanBootstrap() As Boolean Implements ICurve.CanBootstrap
            Get
                Return True
            End Get
        End Property

        Public Property Bootstrapped() As Boolean Implements ICurve.Bootstrapped
            Get
                Return _bootstrapped
            End Get
            Set(ByVal value As Boolean)
                _bootstrapped = value
                Recalculate()
            End Set
        End Property

        Private _estModel As EstimationModel
        Public Property EstModel() As EstimationModel
            Get
                Return _estModel
            End Get
            Set(ByVal value As EstimationModel)
                _estModel = value
                Recalculate()
            End Set
        End Property

        Public Sub New(ByVal ansamble As Ansamble, ByVal src As Source)
            MyBase.new(ansamble)

            Nm = src.Name
            PortfolioID = src.ID
            BondFields = src.Fields.Realtime.AsContainer()
            Color = src.Color
            _histFields = src.Fields.History.AsContainer()

            YieldMode = SettingsManager.Instance.YieldCalcMode
            AddRics(src.GetDefaultRics())
        End Sub

        Public Overrides Sub Subscribe()
            Dim rics As List(Of String) = (From elem In AllElements Select elem.MetaData.RIC).ToList()
            If rics.Count = 0 Then Return
            If _curveDate = Today Then
                QuoteLoader.AddItems(rics, BondFields.AllNames)
            Else
                QuoteLoader.CancelAll()
                Dim historyBlock As New HistoryBlock
                AddHandler historyBlock.History, AddressOf OnHistory
                historyBlock.Load(rics, _histFields.AllNames, _curveDate.AddDays(-10), _curveDate)
            End If
        End Sub

        Private Sub OnHistory(ByVal obj As HistoryBlock.DataCube)
            If obj Is Nothing Then
                CurveDate = Today
            Else
                ' doing some cleanup
                For Each elem In AllElements
                    elem.QuotesAndYields.Clear()
                Next
                ' parsing historical data
                For Each ric In obj.Rics
                    ParseHistory(ric, obj.RicData2(ric))
                Next
            End If
        End Sub

        Private Sub ParseHistory(ByVal ric As String, ByVal rawData As Dictionary(Of String, Dictionary(Of Date, String)))
            If rawData Is Nothing Then
                Logger.Error("No data on bond {0}", ric)
                Return
            End If
            Dim bonds = (From elem In AllElements Where elem.MetaData.RIC = ric)
            If Not bonds.Any Then
                Logger.Warn("Instrument {0} does not belong to serie {1}", ric, Name)
                Return
            End If
            Dim bond = bonds.First()

            Dim fieldsDescription As FieldsDescription = _histFields.Fields
            If rawData.ContainsKey(fieldsDescription.Last) Then
                ParseHistoricalItem(rawData, fieldsDescription.Last, bond)
            End If
            If rawData.ContainsKey(fieldsDescription.Bid) Or rawData.ContainsKey(fieldsDescription.Ask) Then
                Dim bidData = ParseHistoricalItem(rawData, fieldsDescription.Bid, bond)
                Dim askData = ParseHistoricalItem(rawData, fieldsDescription.Ask, bond)
                If bidData IsNot Nothing AndAlso askData IsNot Nothing AndAlso bidData.Item1 = askData.Item1 Then
                    Dim mid = (bidData.Item2 + askData.Item2) / 2
                    HandleNewQuote(bond, _histFields.XmlName(_histFields.Fields.Mid), mid, bidData.Item1)
                ElseIf (bidData IsNot Nothing Or askData IsNot Nothing) And Not SettingsManager.Instance.MidIfBoth Then
                    If bidData IsNot Nothing Then
                        HandleNewQuote(bond, _histFields.XmlName(_histFields.Fields.Mid), bidData.Item2, bidData.Item1)
                    Else
                        HandleNewQuote(bond, _histFields.XmlName(_histFields.Fields.Mid), askData.Item2, askData.Item1)
                    End If
                End If
            End If
        End Sub

        Private Function ParseHistoricalItem(ByVal rawData As Dictionary(Of String, Dictionary(Of Date, String)), ByVal field As String, ByVal bond As Bond) As Tuple(Of Date, Double)
            Dim datVal = rawData(field)
            Dim dates = (From key In datVal.Keys Where IsNumeric(datVal(key))).ToList()
            If dates.Any Then
                Dim maxdate = dates.Max
                HandleNewQuote(bond, _histFields.XmlName(field), datVal(maxdate), maxdate)
                Return Tuple.Create(maxdate, CDbl(rawData(field)(maxdate)))
            End If
            Return Nothing
        End Function

        Public Overrides Sub RecalculateTotal()
            For Each bnd In AllElements
                For Each q In bnd.QuotesAndYields
                    HandleNewQuote(bnd, q, bnd.QuotesAndYields(q).Price, bnd.QuotesAndYields(q).YieldAtDate, False)
                Next
            Next
            Recalculate()
        End Sub

        Public Overrides Sub Recalculate(ByVal ord As IOrdinate)
            ' yield can't be a source for benchmark
            If ord = Yield Then Throw New InvalidOperationException()
            If IsSynthetic Then
                UpdateSyntSpreads(_lastSyntCurve, ord)
                NotifyUpdatedSpread(ExtractPoints(_lastSyntCurve, ord), ord)
            Else
                _lastCurve(ord) = UpdateSpreads(ord)
                NotifyUpdatedSpread(_lastCurve(ord), ord)
            End If

        End Sub

        Private Function UpdateSpreads(ByVal ord As IOrdinate) As List(Of CurveItem)
            SetSpread(ord)
            Dim res = New List(Of CurveItem)(
                        From item In AllElements
                        From quoteName In item.QuotesAndYields
                        Let q = item.QuotesAndYields(quoteName)
                        Let theY = ord.GetValue(q)
                        Where theY.HasValue AndAlso item.QuotesAndYields.Main IsNot Nothing AndAlso quoteName = item.QuotesAndYields.Main.QuoteName
                        Select New PointCurveItem(q.Duration, theY, Me))
            res.Sort()
            Return res
        End Function

        Public Overrides Sub Recalculate()
            _lastCurve(Yield) = UpdateCurveShape()
            If IsSynthetic Then
                _lastSyntCurve = (From item In _lastCurve(Yield) Select GetSyntBond(item.TheX, item.TheY)).ToList()
                For Each ord In Spreads
                    UpdateSyntSpreads(_lastSyntCurve, ord)
                Next
                NotifyUpdated(ExtractPoints(_lastSyntCurve, Ansamble.YSource))
            Else
                _lastSyntCurve = Nothing
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
            End If
        End Sub

        Private Function ExtractPoints(ByVal crv As List(Of SyntheticZcb), ByVal ord As OrdinateBase) As List(Of CurveItem)
            Return (From item In crv
                    From quoteName In item.QuotesAndYields
                    Let m = item.QuotesAndYields(quoteName)
                    Let vl = ord.GetValue(m)
                    Where vl.HasValue
                    Select New PointCurveItem(m.Duration, ord.GetValue(m), Me)).Cast(Of CurveItem).ToList
        End Function

        Private Sub UpdateSyntSpreads(ByVal crv As List(Of SyntheticZcb), ByVal ord As OrdinateBase)
            If Ansamble.Benchmarks.Keys.Contains(ord) AndAlso Ansamble.Benchmarks(ord) <> Me Then
                For Each qy In From item In crv From quoteName In item.QuotesAndYields Select item.QuotesAndYields(quoteName)
                    ord.SetValue(qy, Ansamble.Benchmarks(ord))
                Next
            Else
                For Each qy In From item In crv From quoteName In item.QuotesAndYields Select item.QuotesAndYields(quoteName)
                    ord.ClearValue(qy)
                Next
            End If
        End Sub

        Private Function UpdateCurveShape() As List(Of CurveItem)
            Dim result As New List(Of CurveItem)
            If _bootstrapped Then
                Try
                    Dim data = (From elem In Elements
                            Where elem.MetaData.IssueDate <= _curveDate And
                                  elem.MetaData.Maturity > _curveDate And
                                  elem.QuotesAndYields.Any()).ToList()

                    Dim params(0 To data.Count() - 1, 5) As Object
                    For i = 0 To data.Count - 1
                        Dim meta = data(i).MetaData
                        Dim main As BondPointDescription = data(i).QuotesAndYields.Main
                        params(i, 0) = "B"
                        params(i, 1) = _curveDate
                        params(i, 2) = meta.Maturity
                        params(i, 3) = meta.GetCouponByDate(_curveDate)

                        ' incorporating spread
                        If data(i).UserDefinedSpread(Yield) > 0 Then
                            Dim settleDate = _bondModule.BdSettle(_curveDate, meta.PaymentStructure)
                            Dim priceObject As Array = _bondModule.AdBondPrice(settleDate, main.Yield + data(i).UserDefinedSpread(Yield),
                                                                              meta.Maturity, params(i, 3), 0, meta.PaymentStructure,
                                                                              meta.RateStructure, "", "RES:BDPRICE")
                            params(i, 4) = priceObject.GetValue(1)
                        Else
                            params(i, 4) = main.Price / 100.0
                        End If

                        params(i, 5) = meta.PaymentStructure
                    Next

                    Dim termStructure As Array = _curveModule.AdTermStructure(params, "RM:YC ZCTYPE:RATE IM:CUBX ND:DIS", Nothing)
                    For i = termStructure.GetLowerBound(0) To termStructure.GetUpperBound(0)
                        Dim matDate = Utils.FromExcelSerialDate(termStructure.GetValue(i, 1))
                        Dim dur = (matDate - _curveDate).TotalDays / 365.0
                        Dim yld = termStructure.GetValue(i, 2)
                        If dur > 0 And yld > 0 Then result.Add(New PointCurveItem(dur, yld, Me))
                    Next
                Catch ex As Exception
                    Logger.ErrorException("Failed to bootstrap", ex)
                    Logger.Error("Exception = {0}", ex.ToString())
                    Return result
                End Try
            Else
                For Each bnd In Elements
                    Dim x As Double, y As Double
                    Dim description = bnd.QuotesAndYields.Main
                    If description Is Nothing Then Continue For

                    Select Case Ansamble.XSource
                        Case XSource.Duration
                            x = description.Duration
                        Case XSource.Maturity
                            x = (bnd.MetaData.Maturity.Value - Date.Today).Days / 365
                    End Select

                    y = description.Yield
                    If x > 0 And y > 0 Then result.Add(New BondCurveItem(x, y, bnd, description.BackColor, description.Yld.ToWhat, description.MarkerStyle, bnd.Label))
                Next
            End If
            result.Sort()

            If _estModel IsNot Nothing Then
                Dim est As New Estimator(_estModel)
                Dim tmp = New List(Of CurveItem)(result)
                Dim list As List(Of XY) = (From item In tmp Select New XY(item.TheX, item.TheY)).ToList()
                Dim apprXY = est.Approximate(list)
                result = (From item In apprXY Select New PointCurveItem(item.X, item.Y, Me)).Cast(Of CurveItem).ToList()
                _formula = est.GetFormula()
            Else
                _formula = "N/A"
            End If

            Return result
        End Function

        Public Sub Bootstrap() Implements ICurve.Bootstrap
            Bootstrapped = Not Bootstrapped
        End Sub

        Public Function GetSnapshot() As BondCurveSnapshot
            If Not _lastCurve.ContainsKey(Yield) Then Return Nothing
            Return New BondCurveSnapshot(AllElements, _lastCurve(Yield), Ansamble)
        End Function

        Public Sub SetFitMode(ByVal mode As String)
            Dim model = EstimationModel.FromName(mode)
            EstModel = If(model Is Nothing OrElse (EstModel IsNot Nothing AndAlso EstModel = model), Nothing, model)
        End Sub

        Public Sub ClearSpread(ByVal ySource As OrdinateBase) Implements ICurve.ClearSpread
            For Each qy In From item In AllElements From quoteName In item.QuotesAndYields Select item.QuotesAndYields(quoteName)
                ySource.ClearValue(qy)
            Next
        End Sub

        Public Sub SetSpread(ByVal ySource As OrdinateBase) Implements ICurve.SetSpread
            If Ansamble.Benchmarks.Keys.Contains(ySource) AndAlso Ansamble.Benchmarks(ySource) <> Me Then
                For Each qy In From item In AllElements From quoteName In item.QuotesAndYields Select item.QuotesAndYields(quoteName)
                    ySource.SetValue(qy, Ansamble.Benchmarks(ySource))
                Next
            Else
                For Each qy In From item In AllElements From quoteName In item.QuotesAndYields Select item.QuotesAndYields(quoteName)
                    ySource.ClearValue(qy)
                Next
            End If
        End Sub

        Public Function RateArray() As Array Implements ICurve.RateArray
            If Not _lastCurve.ContainsKey(Yield) Then Return Nothing
            Dim list = (From elem In _lastCurve(Yield) Select New XY(elem.TheX, elem.TheY)).ToList()
            list.Sort()
            Dim len = list.Count - 1
            Dim res(0 To len, 1) As Object
            For i = 0 To len
                res(i, 0) = DateTime.Today.AddDays(TimeSpan.FromDays(list(i).X * 365).TotalDays)
                res(i, 1) = list(i).Y
            Next
            Return res
        End Function
    End Class
End Namespace