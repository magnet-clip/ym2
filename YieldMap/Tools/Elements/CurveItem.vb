Imports System.ComponentModel

Namespace Tools.Elements
    Public MustInherit Class CurveItem
        Implements IComparable(Of CurveItem)
        Protected ReadOnly _x As Double
        Protected ReadOnly _y As Double

        Public Overridable ReadOnly Property X() As String
            Get
                Return String.Format("{0:F2}", _x)
            End Get
        End Property

        Public Overridable ReadOnly Property Y() As String
            Get
                Return String.Format("{0:P2}", _y)
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property TheX() As Double
            Get
                Return _x
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property TheY() As Double
            Get
                Return _y
            End Get
        End Property

        Public Function CompareTo(ByVal other As CurveItem) As Integer Implements IComparable(Of CurveItem).CompareTo
            Return _x.CompareTo(other._x)
        End Function

        Public Sub New(ByVal x As Double, ByVal y As Double)
            _x = x
            _y = y
        End Sub
    End Class

    Public Class BondCurveItem
        Inherits CurveItem
        Private ReadOnly _bond As Bond
        Private ReadOnly _backColor As String
        Private ReadOnly _label As String
        Private ReadOnly _toWhat As YieldToWhat
        Private ReadOnly _markerStyle As String

        <Browsable(False)>
        Public ReadOnly Property BackColor() As String
            Get
                Return _backColor
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property ToWhat() As YieldToWhat
            Get
                Return _toWhat
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property MarkerStyle() As String
            Get
                Return _markerStyle
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property Bond() As Bond
            Get
                Return _bond
            End Get
        End Property

        Public ReadOnly Property Ric() As String
            Get
                Return _bond.MetaData.RIC
            End Get
        End Property

        Public ReadOnly Property Label As String
            Get
                Return _label
            End Get
        End Property

        Public Sub New(ByVal x As Double, ByVal y As Double, ByVal bond As Bond, ByVal backColor As String, ByVal toWhat As YieldToWhat, ByVal markerStyle As String, ByVal label As String)
            MyBase.new(x, y)
            _bond = bond
            _backColor = backColor
            _toWhat = toWhat
            _markerStyle = markerStyle
            _label = label
        End Sub
    End Class

    Public Class SwapCurveItem
        Inherits CurveItem

        Private ReadOnly _curve As SwapCurve
        Private ReadOnly _ric As String

        Public Sub New(ByVal x As Double, ByVal y As Double, ByVal curve As SwapCurve, ByVal ric As String)
            MyBase.New(x, y)
            _curve = curve
            _ric = ric
        End Sub

        <Browsable(False)>
        Public ReadOnly Property Curve As SwapCurve
            Get
                Return _curve
            End Get
        End Property

        Public ReadOnly Property RIC As String
            Get
                Return _ric
            End Get
        End Property
    End Class

    Public Class PointCurveItem
        Inherits CurveItem
        Private ReadOnly _curve As ICurve

        <Browsable(False)>
        Public ReadOnly Property Curve() As ICurve
            Get
                Return _curve
            End Get
        End Property

        Public Sub New(ByVal x As Double, ByVal y As Double, ByVal curve As ICurve)
            MyBase.New(x, y)
            _curve = curve
        End Sub
    End Class

    Public Class BondSpreadCurveItem
        Inherits CurveItem
        Private ReadOnly _ric As String
        Private ReadOnly _name As String

        Public ReadOnly Property RIC() As String
            Get
                Return _ric
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property

        Public Sub New(ByVal x As Double, ByVal y As Double, ByVal ric As String, ByVal name As String)
            MyBase.New(x, y)
            _ric = ric
            _name = name
        End Sub
        Public Overrides ReadOnly Property X() As String
            Get
                Return String.Format("{0:F2}", _x)
            End Get
        End Property

        Public Overrides ReadOnly Property Y() As String
            Get
                Return String.Format("{0:N2}", _y)
            End Get
        End Property
    End Class
End Namespace