﻿Imports System.ComponentModel
Imports System.Reflection

Namespace Bonds
    Public Class PaymentException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub
    End Class

    Public Class BondPayments
        Private ReadOnly _issueDate As Date
        Private ReadOnly _matDate As Date?
        Private ReadOnly _payments As New LinkedList(Of Tuple(Of Date, Double))

        Sub New(ByVal issueDate As Date, ByVal matDate As Date?)
            _issueDate = issueDate
            _matDate = matDate
        End Sub

        Public Sub AddPayment(ByVal dt As Date, ByVal cpn As Double)
            _payments.AddLast(Tuple.Create(dt, cpn))
        End Sub

        Public Function GetCoupon(ByVal dt As Date) As Double
            If dt < _issueDate Then
                Throw New PaymentException(String.Format("Requested date {0:dd/MMM/yy} is less then issue date {1:dd/MMM/yy}", dt, _issueDate))
            ElseIf dt > _matDate Then
                Throw New PaymentException(String.Format("Requested date {0:dd/MMM/yy} is greater then maturity date {1:dd/MMM/yy}", dt, _matDate))
            End If
            Dim item = _payments.First
            While item.Next IsNot Nothing And item.Value.Item1 < dt
                item = item.Next
            End While
            Return item.Value.Item2 / 100
        End Function
    End Class

    Public Class Hideable
        Inherits Attribute
    End Class

    Public Class BondMetadata
        Private ReadOnly _ric As String
        Private ReadOnly _shortName As String
        Private ReadOnly _label As String

        Private _maturity As Date? ' this item is not ReadOnly 'cause in case ZCB's the only way to obatin necessary yield and duration is to vary maturity
        Private ReadOnly _coupon As Double

        Private ReadOnly _paymentStructure As String
        Private ReadOnly _rateStructure As String
        Private ReadOnly _issueDate As Date

        Private ReadOnly _issuerName As String
        Private ReadOnly _borrowerName As String
        Private ReadOnly _currency As String
        Private ReadOnly _putable As Boolean
        Private ReadOnly _callable As Boolean
        Private ReadOnly _floater As Boolean
        Private ReadOnly _lastIssueRating As RatingDescr
        Private ReadOnly _lastIssuerRating As RatingDescr
        Private ReadOnly _lastRating As RatingDescr

        Private ReadOnly _seniorityType As String
        Private ReadOnly _instrumentType As String

        Private ReadOnly _label1 As String
        Private ReadOnly _label2 As String
        Private ReadOnly _label3 As String
        Private ReadOnly _label4 As String

        Private ReadOnly _industry As String
        Private ReadOnly _subIndustry As String

        Private ReadOnly _issuerCountry As String
        Private ReadOnly _borrowerCountry As String



        Sub New(ByVal ric As String, ByVal maturity As Date?, ByVal coupon As Double, ByVal paymentStructure As String, ByVal rateStructure As String,
                ByVal issuerName As String, ByVal shortName As String, issueDate As Date)
            _ric = ric
            _maturity = maturity
            _coupon = coupon
            _paymentStructure = paymentStructure
            _rateStructure = rateStructure
            _shortName = shortName

            _issueDate = issueDate
            _issuerCountry = issuerCountry
            _borrowerCountry = borrowerCountry
            _label1 = shortName
            _label2 = shortName
            _label3 = shortName
            _label4 = shortName

            _issuerName = issuerName
            _putable = False
            _callable = False
            _floater = False
        End Sub

        Sub New(ByVal ric As String, ByVal shortName As String, ByVal label As String, ByVal maturity As Date?, ByVal coupon As Double, ByVal paymentStructure As String, ByVal rateStructure As String, ByVal issueDate As Date, ByVal label1 As String, ByVal label2 As String, ByVal label3 As String, ByVal label4 As String,
                ByVal issuerName As String, ByVal borrowerName As String, ByVal currency As String, ByVal putable As Boolean, ByVal callable As Boolean, ByVal floater As Boolean, ByVal lastIssueRating As RatingDescr, ByVal lastIssuerRating As RatingDescr, ByVal lastRating As RatingDescr, ByVal seniorityType As String,
                ByVal industry As String, ByVal subIndustry As String, instrumentType As String, ByVal issuerCountry As String, ByVal borrowerCountry As String)

            _ric = ric
            _shortName = shortName
            _label = label
            _maturity = maturity
            _coupon = coupon
            _paymentStructure = paymentStructure
            _rateStructure = rateStructure
            _issueDate = issueDate
            _label1 = label1
            _label2 = label2
            _label3 = label3
            _label4 = label4

            _issuerName = issuerName
            _borrowerName = borrowerName
            _currency = currency
            _putable = putable
            _callable = callable
            _floater = floater
            _lastIssueRating = lastIssueRating
            _lastIssuerRating = lastIssuerRating
            _lastRating = lastRating
            _seniorityType = seniorityType
            _industry = industry
            _subIndustry = subIndustry
            _instrumentType = instrumentType
            _issuerCountry = issuerCountry
            _borrowerCountry = borrowerCountry
        End Sub

        <Hideable()>
        <Sortable()>
        <Filterable()>
        Public ReadOnly Property Industry() As String
            Get
                Return _industry
            End Get
        End Property

        <Hideable()>
        <Sortable()>
        <Filterable()>
        Public ReadOnly Property SubIndustry() As String
            Get
                Return _subIndustry
            End Get
        End Property

        <Hideable()>
        <DisplayName("Ric")>
        <Filterable()>
        Public ReadOnly Property Ric As String
            Get
                Return _ric
            End Get
        End Property

        <Hideable()>
        <DisplayName("Name")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property ShortName As String
            Get
                Return _shortName
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property Label As String
            Get
                Return _label
            End Get
        End Property

        <DisplayName("Maturity date")>
        <Filterable()>
        <Sortable()>
        <Hideable()>
        Public Property Maturity As Date?
            Get
                Return _maturity
            End Get
            Set(ByVal value As Date?)
                _maturity = value
            End Set
        End Property

        <DisplayName("Current coupon")>
        <Filterable()>
        <Sortable()>
        <Hideable()>
        Public ReadOnly Property Coupon As Double
            Get
                Return _coupon
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property PaymentStructure As String
            Get
                Return _paymentStructure
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property RateStructure As String
            Get
                Return _rateStructure
            End Get
        End Property

        <DisplayName("Issue date")>
        <Filterable()>
        <Hideable()>
        <Sortable()>
        Public ReadOnly Property IssueDate As Date
            Get
                Return _issueDate
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property Label1 As String
            Get
                Return _label1
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property Label2 As String
            Get
                Return _label2
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property Label3 As String
            Get
                Return _label3
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property Label4 As String
            Get
                Return _label4
            End Get
        End Property

        <Hideable()>
        <DisplayName("Issuer Name")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property IssuerName As String
            Get
                Return _issuerName
            End Get
        End Property

        <Hideable()>
        <DisplayName("Borrower Name")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property BorrowerName As String
            Get
                Return _borrowerName
            End Get
        End Property

        <Hideable()>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property Currency As String
            Get
                Return _currency
            End Get
        End Property

        <Hideable()>
        <Filterable()>
        Public ReadOnly Property Putable As Boolean
            Get
                Return _putable
            End Get
        End Property

        <Hideable()>
        <Filterable()>
        Public ReadOnly Property Callable As Boolean
            Get
                Return _callable
            End Get
        End Property

        <Hideable()>
        <Filterable()>
        Public ReadOnly Property Floater As Boolean
            Get
                Return _floater
            End Get
        End Property

        <Hideable()>
        <DisplayName("Last issue rating")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property LastIssueRating As RatingDescr
            Get
                Return _lastIssueRating
            End Get
        End Property

        <Hideable()>
        <DisplayName("Last issuer rating")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property LastIssuerRating As RatingDescr
            Get
                Return _lastIssuerRating
            End Get
        End Property

        <Hideable()>
        <DisplayName("Last rating")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property LastRating As RatingDescr
            Get
                Return _lastRating
            End Get
        End Property

        <Hideable()>
        <DisplayName("Type of seniority")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property SeniorityType As String
            Get
                Return _seniorityType
            End Get
        End Property

        <Hideable()>
        <DisplayName("Type of instrument")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property InstrumentType As String
            Get
                Return _instrumentType
            End Get
        End Property


        <Hideable()>
        <DisplayName("Issuer Country")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property IssuerCountry() As String
            Get
                Return _issuerCountry
            End Get
        End Property

        <Hideable()>
        <DisplayName("Borrower Country")>
        <Filterable()>
        <Sortable()>
        Public ReadOnly Property BorrowerCountry() As String
            Get
                Return _borrowerCountry
            End Get
        End Property

        Public Function GetCouponByDate(ByVal dt As Date) As Double
            Return BondsData.Instance.GetBondPayments(_ric).GetCoupon(dt)
        End Function

        Public Shared Function GetHideableFields() As List(Of String)
            Return (From field In GetType(BondMetadata).GetProperties(BindingFlags.Instance Or BindingFlags.Public)
                      Where field.GetCustomAttributes(GetType(Hideable), False).Any
                      Select field.Name).ToList()
        End Function
    End Class
End Namespace