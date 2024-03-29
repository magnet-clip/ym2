Imports DbManager
Imports DbManager.Bonds
Imports NLog
Imports Settings

Namespace Tools.Elements
    Public Class Bond
        Inherits Identifyable

        Public Event Changed As Action
        Public Event CustomPrice As Action(Of Bond, Double)

        Public TodayVolume As Double

        Public Class QyContainer
            Implements IEnumerable(Of String)
            Private ReadOnly _parent As Bond
            Private ReadOnly _quotesAndYields As New Dictionary(Of String, BondPointDescription)
            Protected Shared ReadOnly Logger As Logger = Logging.GetLogger(GetType(QyContainer))

            Sub New(ByVal parent As Bond)
                _parent = parent
            End Sub

            Default Public Property Val(ByVal key As String) As BondPointDescription
                Get
                    Return If(_quotesAndYields.ContainsKey(key), _quotesAndYields(key), Nothing)
                End Get
                Set(ByVal value As BondPointDescription)
                    _quotesAndYields(key) = value
                End Set
            End Property

            Public ReadOnly Property MaxPriorityField() As String
                Get
                    If _parent.UserSelectedQuote <> "" Then Return _parent.UserSelectedQuote
                    Dim forbiddenFields = SettingsManager.Instance.ForbiddenFields.Split(",")
                    Dim existingFields = (From item In _quotesAndYields.Keys
                            Where _quotesAndYields(item).Price > 0 AndAlso Not forbiddenFields.Contains(item)).ToList()
                    Dim allowedFields = SettingsManager.Instance.FieldsPriority.Split(",")
                    If Not allowedFields.Any OrElse Not existingFields.Any Then Return ""

                    Dim i As Integer
                    For i = 0 To allowedFields.Count() - 1
                        If existingFields.Contains(allowedFields(i)) Then Return allowedFields(i)
                    Next
                    Return ""
                End Get
            End Property

            Public Function Has(ByVal key As String) As Boolean
                Return _quotesAndYields.ContainsKey(key)
            End Function

            Public Function IEnumerable_GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
                Return _quotesAndYields.Keys.GetEnumerator()
            End Function

            Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Return _quotesAndYields.Keys.GetEnumerator()
            End Function

            Public ReadOnly Property Main() As BondPointDescription
                Get
                    Dim priorityField = MaxPriorityField
                    If priorityField <> "" AndAlso Not _quotesAndYields.ContainsKey(priorityField) Then
                        Logger.Warn("MaxPriorityField {0} not found in bond {1}!", priorityField, _parent.MetaData.Ric)
                    End If
                    Return If(priorityField <> "" AndAlso _quotesAndYields.ContainsKey(priorityField), _quotesAndYields(priorityField), Nothing)
                End Get
            End Property

            Public Sub Clear()
                _quotesAndYields.Clear()
            End Sub
        End Class

        Public Overridable ReadOnly Property Coupon(ByVal dt As Date) As Double
            Get
                Return BondsData.Instance.GetBondPayments(MetaData.RIC).GetCoupon(dt)
            End Get
        End Property

        Private ReadOnly _quotesAndYields As New QyContainer(Me)
        Public ReadOnly Property QuotesAndYields As QyContainer
            Get
                Return _quotesAndYields
            End Get
        End Property

        Private ReadOnly _metaData As BondMetadata
        Public ReadOnly Property MetaData As BondMetadata
            Get
                Return _metaData
            End Get
        End Property

        Private ReadOnly _parent As Group
        Public ReadOnly Property Parent As Group
            Get
                Return _parent
            End Get
        End Property

        Private _enabled As Boolean = True
        Public Property Enabled() As Boolean
            Get
                Return _enabled
            End Get
            Set(ByVal value As Boolean)
                _enabled = value
                RaiseEvent Changed()
            End Set
        End Property

        Private _userSelectedQuote As String
        Public Property UserSelectedQuote As String
            Get
                Return _userSelectedQuote
            End Get
            Set(ByVal value As String)
                _userSelectedQuote = value
                RaiseEvent Changed()
            End Set
        End Property

        Private _userDefinedSpread As Double
        Public Property UserDefinedSpread(Optional ByVal ord As IOrdinate = Nothing) As Double
            Get
                Return If(ord = Yield, _userDefinedSpread / 10000, _userDefinedSpread)
            End Get
            Set(ByVal value As Double)
                _userDefinedSpread = value
                RaiseEvent Changed()
            End Set
        End Property

        Private _labelMode As LabelMode? = Nothing
        Public Property LabelMode(Optional ByVal force As Boolean = False) As LabelMode?
            Get
                Return _labelMode
            End Get
            Set(ByVal value As LabelMode?)
                If Not force Then
                    If Not value.HasValue And _labelMode.HasValue Then
                        _labelMode = Nothing
                    ElseIf value.HasValue And Not _labelMode.HasValue Then
                        _labelMode = value
                    ElseIf value <> _labelMode Then
                        _labelMode = value
                    Else
                        _labelMode = Nothing
                    End If
                Else
                    _labelMode = value
                End If

                RaiseEvent Changed()
            End Set
        End Property

        Sub New(ByVal parent As Group, ByVal metaData As BondMetadata)
            _parent = parent
            _metaData = metaData
        End Sub

        Public ReadOnly Property Label() As String
            Get
                If Not LabelMode.HasValue Then Return ""
                Dim lab As String = ""
                Select Case LabelMode.Value
                    Case Elements.LabelMode.IssuerAndSeries : lab = MetaData.Label1
                    Case Elements.LabelMode.IssuerCpnMat : lab = MetaData.Label2
                    Case Elements.LabelMode.Description : lab = MetaData.Label3
                    Case Elements.LabelMode.SeriesOnly : lab = MetaData.Label4
                End Select
                Label = lab
            End Get
        End Property

        Public ReadOnly Property Fields() As FieldsDescription
            Get
                Return Parent.BondFields.Fields
            End Get
        End Property

        Private _indvidualBondMode As Boolean = False

        Public ReadOnly Property IndvidualBondMode() As Boolean
            Get
                Return _indvidualBondMode
            End Get
        End Property

        Private _yieldMode As String = SettingsManager.Instance.YieldCalcMode
        Public Property YieldMode() As String
            Get
                Return If(_indvidualBondMode, _yieldMode, SettingsManager.Instance.YieldCalcMode)
            End Get
            Set(ByVal value As String)
                _yieldMode = value
                If value = "Default" Or value = "" Then
                    _indvidualBondMode = False
                Else
                    _indvidualBondMode = True
                End If
                RaiseEvent Changed()
            End Set
        End Property

        Public Sub Disable()
            Enabled = False
            RaiseEvent Changed()
        End Sub

        Public Sub Enable()
            Enabled = True
            RaiseEvent Changed()
        End Sub

        Public Sub Annihilate()
            Parent.RemoveBond(Me)
            RaiseEvent Changed()
        End Sub

        Sub SetCustomPrice(ByVal price As Double)
            If price > 0 Then
                _userSelectedQuote = Fields.Custom
                RaiseEvent CustomPrice(Me, price)
            End If
        End Sub
    End Class
End NameSpace