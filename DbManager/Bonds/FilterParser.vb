﻿Imports System.Text.RegularExpressions

Namespace Bonds
    Public Class FilterParser
        Private Const LogicalOpPriority = 1
        Private Const BinaryOpPriority = 2
        Private Const BracketsPriority = 10

        Private ReadOnly _opStack As New LinkedList(Of Operation)

        Private Shared ReadOnly VarName As Regex = New Regex("^\s*?\$(?<varname>[a-zA-Z0-9_0]+?)")
        Private Shared ReadOnly LogOp As Regex = New Regex("^\s*?(?<lop>AND|OR)")
        Private Shared ReadOnly BinOp As Regex = New Regex("^\s*?(?<bop>\<=|\>=|=|\<\>|\<|\>|like)")
        Private Shared ReadOnly NumValue As Regex = New Regex("^\s*?(?<num>[0-9]+|[0-9]+.[0-9]+?)")
        Private Shared ReadOnly StrValue As Regex = New Regex("^\s*?""(?<str>[^""]*)""")
        Private Shared ReadOnly DatValue As Regex = New Regex("^\s*?#(?<dd>[0-9]{1,2})/(?<mm>[0-9]{1,2})/(?<yy>[0-9]{2}|[0-9]{4})#")

        Private Enum ParserState
            Expr
            Term
            Bop
            Lop
            Name
            Value
        End Enum

        Private _state As ParserState = ParserState.Expr
        Private _filterString As String

        Public Interface IGrammarElement
        End Interface

        Public MustInherit Class Operation
            Implements IGrammarElement

            Private ReadOnly _priority As Integer

            Sub New(ByVal priority As Integer)
                _priority = priority
            End Sub

            Public ReadOnly Property Priority As Integer
                Get
                    Return _priority
                End Get
            End Property
        End Class

        Public Class Lop
            Inherits Operation

            Public Enum LogicalOperation
                OpAnd
                OpOr
            End Enum

            Private ReadOnly _logicalOperation As LogicalOperation

            Public Overrides Function ToString() As String
                Return If(_logicalOperation = LogicalOperation.OpAnd, "And", "Or")
            End Function

            Sub New(ByVal logicalOperation As String, ByVal priority As Integer)
                MyBase.new(priority)
                If logicalOperation.ToUpper() = "AND" Then
                    _logicalOperation = Lop.LogicalOperation.OpAnd
                ElseIf logicalOperation.ToUpper() = "OR" Then
                    _logicalOperation = Lop.LogicalOperation.OpOr
                Else
                    Throw New ConditionLexicalException(String.Format("Invalid logical operation {0}", logicalOperation))
                End If
            End Sub

            Public ReadOnly Property LogOperation As LogicalOperation
                Get
                    Return _logicalOperation
                End Get
            End Property
        End Class

        Public Class Bop
            Inherits Operation

            Public Enum BinaryOperation
                OpEquals
                OpGreater
                OpLess
                OpGreatorOrEquals
                OpLessOrEquals
                OpNotEqual
                OpLike
            End Enum

            Private ReadOnly _binaryOperation As BinaryOperation

            Public Overrides Function ToString() As String
                Dim res As String = ""
                Select Case _binaryOperation
                    Case BinaryOperation.OpEquals : res = "="
                    Case BinaryOperation.OpGreater : res = ">"
                    Case BinaryOperation.OpLess : res = "<"
                    Case BinaryOperation.OpGreatorOrEquals : res = ">="
                    Case BinaryOperation.OpLessOrEquals : res = "<="
                    Case BinaryOperation.OpNotEqual : res = "<>"
                    Case BinaryOperation.OpLike : res = "like"
                End Select
                Return res
            End Function

            Sub New(ByVal binaryOperation As String, ByVal priority As Integer)
                MyBase.new(priority)
                Select Case binaryOperation
                    Case "=" : _binaryOperation = Bop.BinaryOperation.OpEquals
                    Case "<>" : _binaryOperation = Bop.BinaryOperation.OpNotEqual
                    Case ">" : _binaryOperation = Bop.BinaryOperation.OpGreater
                    Case "<" : _binaryOperation = Bop.BinaryOperation.OpLess
                    Case ">=" : _binaryOperation = Bop.BinaryOperation.OpGreatorOrEquals
                    Case "<=" : _binaryOperation = Bop.BinaryOperation.OpLessOrEquals
                    Case "like" : _binaryOperation = Bop.BinaryOperation.OpLike
                    Case Else : Throw New ConditionLexicalException(String.Format("Invalid binary operation {0}", binaryOperation))
                End Select
            End Sub

            Public ReadOnly Property BinOperation As BinaryOperation
                Get
                    Return _binaryOperation
                End Get
            End Property
        End Class

        Public Class Var
            Implements IGrammarElement
            Private ReadOnly _name As String

            Public Overrides Function ToString() As String
                Return _name
            End Function

            Sub New(ByVal name As String)
                _name = name
            End Sub

            Public ReadOnly Property Name As String
                Get
                    Return _name
                End Get
            End Property
        End Class

        Public Class Val(Of T)
            Implements IGrammarElement
            Private ReadOnly _value As T

            Sub New(ByVal value As T)
                _value = value
            End Sub

            Public Overrides Function ToString() As String
                Return _value.ToString()
            End Function

            Public ReadOnly Property Value As T
                Get
                    Return _value
                End Get
            End Property
        End Class

        Public ReadOnly Property FilterString As String
            Get
                Return _filterString
            End Get
        End Property

        Public Function SetFilter(ByVal fltStr As String) As LinkedList(Of IGrammarElement)
            fltStr = fltStr.Trim()
            _filterString = fltStr
            Dim res As LinkedList(Of IGrammarElement)
            Try
                _opStack.Clear()
                _state = ParserState.Expr

                Dim ind As Integer
                res = ParseFilterString(fltStr, ind, 0)
                If ind < fltStr.Length() Then Throw New ConditionSyntaxException("Parsing not finished", ind)
            Catch ex As ConditionSyntaxException
                If ex.FilterStr = "" Then ex.FilterStr = fltStr
                Throw
            End Try
            Return res
        End Function

        Friend Function GetStack(ByVal grammar As LinkedList(Of IGrammarElement)) As List(Of String)
            Dim list = New List(Of String)
            If grammar.Any Then
                Dim lnk As LinkedListNode(Of IGrammarElement) = grammar.First
                Do
                    list.Add(String.Format("[{0}] ", lnk.Value.ToString()))
                    lnk = lnk.Next
                Loop While lnk IsNot Nothing
            End If
            Return list
        End Function

        '=========================================================================================
        '
        '   GRAMMAR LOOKS LIKE THIS
        '   --------------------------------------------------------------------------------------
        '   <EXPR>          ::= <BR_EXPR>|<TERM>|<TERM_CHAIN>
        '   <BR_EXPR>       ::= (<EXPR>)
        '   <LOP>           ::= AND|OR
        '   <TERM_CHAIN>    ::= <TERM> <LOP> <TERM> | <TERM> <LOP> <EXPR>
        '   <TERM>          ::= <NAME> <OP> <VALUE>
        '   <OP>            ::= =|>|<|>=|<=
        '   <NAME>          ::= ${<LETTERS>}
        '   <VALUE>         ::= <STRVAL>|<DATVAL>|<NUMVAL>
        '
        '=========================================================================================
        Private Function ParseFilterString(ByVal fltStr As String, ByRef endIndex As Integer, ByVal bracketsLevel As Integer) As LinkedList(Of IGrammarElement)
            Dim i As Integer = 0
            Dim res As New LinkedList(Of IGrammarElement)

            While i < fltStr.Length
                ' SKIP EMPTY SPACES
                While fltStr(i) = " "
                    i = i + 1
                End While

                Dim match As Match
                Select Case _state
                    Case ParserState.Expr
                        'Console.WriteLine("--> EXPR")
                        If fltStr(i) = "(" Then
                            ' BR_EXPR
                            Dim ind As Integer
                            Dim elems As LinkedList(Of IGrammarElement)
                            Try
                                elems = ParseFilterString(fltStr.Substring(i + 1), ind, bracketsLevel + 1)
                            Catch ex As ConditionSyntaxException
                                If bracketsLevel > 0 Then
                                    ex.ErrorPos = ex.ErrorPos + i + 1
                                    Throw
                                End If
                            End Try

                            If elems Is Nothing OrElse Not elems.Any Then
                                Throw New ConditionSyntaxException("Invalid expression in brackets", i)
                            End If

                            Dim elem = elems.First
                            Do
                                res.AddLast(elem.Value)
                                elem = elem.Next
                            Loop Until elem Is Nothing

                            i = i + ind + 1
                        ElseIf fltStr(i) = ")" Then
                            ' END OF CURRENT BR_EXPR
                            endIndex = i + 1
                            Return res
                        ElseIf fltStr(i) = "$" Then
                            ' NAME
                            _state = ParserState.Term
                        Else
                            Throw New ConditionSyntaxException("Unexpected symbol, brackets or variable name required", i)
                        End If

                    Case ParserState.Term
                        'Console.WriteLine("--> TERM")
                        If fltStr(i) = "$" Then
                            _state = ParserState.Name
                        ElseIf fltStr(i) = ")" Then
                            ' END OF CURRENT BR_EXPR
                            endIndex = i + 1
                            Return res
                        Else
                            _state = ParserState.Lop
                        End If

                    Case ParserState.Name
                        'Console.WriteLine("--> NAME")
                        If fltStr(i) <> "$" Then
                            Throw New ConditionSyntaxException("Unexpected symbol, variable name required", i)
                        End If

                        ' Reading variable name
                        match = VarName.Match(fltStr.Substring(i))
                        If match.Success Then
                            Dim variableName = match.Groups("varname").Captures(0).Value
                            Dim node As New Var(variableName)
                            i = i + match.Length + 1
                            res.AddLast(node)
                        Else
                            Throw New ConditionSyntaxException("Unexpected sequence, variable name required", i)
                        End If
                        _state = ParserState.Bop

                    Case ParserState.Bop
                        'Console.WriteLine("--> BOP")
                        ' Reading binary operation
                        match = BinOp.Match(fltStr.Substring(i).ToLower())
                        Dim opNode As Bop
                        If match.Success Then
                            Dim opName = match.Groups("bop").Captures(0).Value
                            opNode = New Bop(opName, BinaryOpPriority + bracketsLevel * BracketsPriority)
                            i = i + match.Length + 1
                            PushToOpStack(res, opNode)
                        Else
                            Throw New ConditionSyntaxException("Unexpected sequence, binary operation (>/</=/<>/>=/<=/like) required", i)
                        End If
                        _state = ParserState.Value

                    Case ParserState.Value
                        'Console.WriteLine("--> VALUE")
                        Dim valNode As IGrammarElement
                        ' Reading value
                        If fltStr(i) = """" Then          ' string value
                            match = StrValue.Match(fltStr.Substring(i))
                            If match.Success Then
                                Dim str = match.Groups("str").Captures(0).Value
                                valNode = New Val(Of String)(str)
                                i = i + match.Length + 1
                            Else
                                Throw New ConditionSyntaxException("Unexpected sequence, string expression required", i)
                            End If
                        ElseIf IsNumeric(fltStr(i)) Then ' number
                            match = NumValue.Match(fltStr.Substring(i))
                            If match.Success Then
                                Dim num = match.Groups("num").Captures(0).Value
                                If Not IsNumeric(num) Then Throw New ConditionSyntaxException("Invalid number", i)
                                valNode = New Val(Of Double)(num)
                                i = i + match.Length + 1
                            Else
                                Throw New ConditionSyntaxException("Unexpected sequence, string expression required", i)
                            End If
                        ElseIf fltStr(i) = "#" Then       ' date
                            match = DatValue.Match(fltStr.Substring(i))
                            If match.Success Then
                                Dim dd = match.Groups("dd").Captures(0).Value
                                Dim mm = match.Groups("mm").Captures(0).Value
                                Dim yy = match.Groups("yy").Captures(0).Value
                                Dim dt As New Date(yy, mm, dd)
                                valNode = New Val(Of Date)(dt)
                                i = i + match.Length + 1
                            Else
                                Throw New ConditionSyntaxException("Unexpected sequence, date expression required", i)
                            End If
                        Else
                            Throw New ConditionSyntaxException("Unexpected symbol, string, date or number required", i)
                        End If
                        res.AddLast(valNode)

                        _state = ParserState.Term

                    Case ParserState.Lop
                        'Console.WriteLine("--> LOP")
                        match = LogOp.Match(fltStr.Substring(i).ToUpper())
                        Dim opNode As Lop
                        If match.Success Then
                            Dim num = match.Groups("lop").Captures(0).Value
                            opNode = New Lop(num, LogicalOpPriority + bracketsLevel * BracketsPriority)
                            PushToOpStack(res, opNode)
                            i = i + match.Length + 1
                        Else
                            Throw New ConditionSyntaxException("Unexpected sequence, logical expression required", i)
                        End If
                        _state = ParserState.Expr
                End Select
            End While
            FlushOpStack(res, bracketsLevel * BracketsPriority)
            endIndex = i
            Return res
        End Function

        Private Sub FlushOpStack(ByRef res As LinkedList(Of IGrammarElement), ByVal priority As Integer)
            While _opStack.Any() AndAlso _opStack.First().Value.Priority > priority
                res.AddLast(_opStack.First.Value)
                _opStack.RemoveFirst()
            End While
        End Sub

        Private Sub PushToOpStack(ByRef res As LinkedList(Of IGrammarElement), ByVal opNode As Operation)
            If Not TypeOf opNode Is Lop And Not TypeOf opNode Is Bop Then Throw New ConditionLexicalException(String.Format("Invalid operation {0}", opNode))
            FlushOpStack(res, opNode.Priority)
            _opStack.AddFirst(opNode)
        End Sub
    End Class

    Public Class ConditionSyntaxException
        Inherits Exception

        Private _errorPos As Integer
        Private _filterStr As String

        Public Sub New(ByVal message As String, ByVal errorPos As Integer, ByVal filterStr As String)
            MyBase.New(message)
            _errorPos = errorPos
            _filterStr = filterStr
        End Sub

        Public Property FilterStr() As String
            Get
                Return _filterStr
            End Get
            Friend Set(ByVal value As String)
                _filterStr = value
            End Set
        End Property

        Public Sub New(ByVal message As String, ByVal errorPos As Integer)
            MyBase.New(message)
            _errorPos = errorPos
        End Sub

        Public Overrides Function ToString() As String
            If _filterStr = "" Then
                Return String.Format("At position {0}: {1}", ErrorPos, Message)
            Else
                Return String.Format("At position {0}: {1} {2}{3} {2}{4," + CStr(ErrorPos) + "} {2}", ErrorPos, Message, Environment.NewLine, _filterStr, "^")
            End If
        End Function

        Public Property ErrorPos As Integer
            Get
                Return _errorPos
            End Get
            Friend Set(ByVal value As Integer)
                _errorPos = value
            End Set
        End Property
    End Class

    Public Class ConditionLexicalException
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
End Namespace