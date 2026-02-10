Imports System.Data
Imports System.Data.Common
Imports System.Data.Odbc
Imports System.Data.OleDb
Imports System.Data.SqlClient

''' <summary>
''' The cLASERBaseTable class allows you to manipulate data 
''' through stored procedures and SQL strings
''' </summary>
''' <remarks></remarks>
Public Class cLASERBaseTable

#Region " Instance Variables and Properties "

    'Base table/ procedure object
    'Should be inherited by all business objects based on tables or database queries.
    Private m_TableName As String 'Tablename to use to populate data - not required
    Protected mSQLCA As cLASERDB
    Private eNumber As Integer
    Private mConnType As cLASERDB.ConnectionType
    Private mCMD As DbCommand
    Private mDR As DbDataReader
    Private mDA As DbDataAdapter
    Private mParms As DBParms
    Private mDT As DataTable
    Private mRetVal As Integer = 0
    Private mRowCount As Integer = 0
    Private m_tran As DbTransaction
    Private m_isTran As Boolean = False
    Private m_leaveOpen As Boolean = False
    Private m_TransactionError As Boolean = False
    Private m_OutPutValues As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()

    ReadOnly Property RowCount() As Integer
        Get
            Return mRowCount
        End Get
    End Property

    ReadOnly Property TransactionError() As Boolean
        Get
            Return m_TransactionError
        End Get
    End Property

    ReadOnly Property ReturnValule() As Integer
        Get
            Return mRetVal
        End Get
    End Property

    ReadOnly Property GetParmCount() As Integer
        Get
            Return mParms.ParmCount
        End Get
    End Property

    ReadOnly Property OutPutValues() As Dictionary(Of String, Object)
        Get
            Return m_OutPutValues
        End Get
    End Property

    ''' <summary>
    ''' GetDataTable returns a datatable of the last dataset returned
    ''' </summary>
    ''' <value></value>
    ''' <returns>datatable</returns>
    ''' <remarks>Read Only</remarks>
    ReadOnly Property GetDataTable() As DataTable
        Get
            Return mDT
        End Get
    End Property

    ''' <summary>
    ''' Returns the text of any errors the cLASERBaseTable encounters.
    ''' </summary>
    ''' <value></value>
    ''' <returns>String</returns>
    ''' <remarks>Read Only</remarks>
    ReadOnly Property dberror() As String
        Get
            Return mSQLCA.dberror
        End Get
    End Property

    ''' <summary>
    ''' Provides a reference to the SQLCA member of cLASERBaseTable
    ''' </summary>
    ''' <value></value>
    ''' <returns>cLASERDB</returns>
    ''' <remarks></remarks>
    Property SQLCA() As cLASERDB
        Get
            Return mSQLCA
        End Get
        Set(ByVal Value As cLASERDB)
            mSQLCA = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns the name of current table
    ''' </summary>
    ''' <value></value>
    ''' <returns>String</returns>
    ''' <remarks>'Tablename to use to populate data - not required</remarks>
    Property TableName() As String
        Get
            Return m_TableName
        End Get
        Set(ByVal Value As String)
            m_TableName = Value
        End Set
    End Property

#End Region

#Region " Constructor "

    ''' <summary>
    ''' The constructor Sub for cLASERBaseTable
    ''' </summary>
    ''' <param name="Conn">This is a cLASERDB connection object</param>
    ''' <param name="sTable">This populates the table name and is optional</param>
    ''' <remarks>sTable is not required</remarks>
    Public Sub New(ByRef Conn As cLASERDB, Optional ByVal sTable As String = "")
        'If table name parameter is passed in then the Field hashtable will be
        'initialized using all of the fields in the table
        mSQLCA = Conn
        mConnType = mSQLCA.ConnType
        mParms = New DBParms
        If Not sTable = "" Then
            m_TableName = sTable
        End If
    End Sub

#End Region

    ''' <summary>
    ''' A private function used to set mCMD to the proper DB connection type
    ''' </summary>
    ''' <param name="sSQL">SQL to be executed</param>
    ''' <remarks></remarks>
    Private Sub setCMD(ByVal sSQL As String)
        Select Case mConnType
            Case cLASERDB.ConnectionType.ODBC
                If m_isTran Then
                    mCMD = New OdbcCommand(sSQL, mSQLCA.Conn, m_tran)
                Else
                    mCMD = New OdbcCommand(sSQL, mSQLCA.Conn)
                End If
            Case cLASERDB.ConnectionType.OLEDB
                If m_isTran Then
                    mCMD = New OleDbCommand(sSQL, mSQLCA.Conn, m_tran)
                Else
                    mCMD = New OleDbCommand(sSQL, mSQLCA.Conn)
                End If
            Case cLASERDB.ConnectionType.SQL
                If m_isTran Then
                    mCMD = New SqlCommand(sSQL, mSQLCA.Conn, m_tran)
                Else
                    mCMD = New SqlCommand(sSQL, mSQLCA.Conn)
                End If
        End Select
        mCMD.CommandTimeout = mSQLCA.CommandTimeout
    End Sub

    ''' <summary>
    ''' Closes an open connection to the database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Close() As Boolean
        If Not mDR Is Nothing Then
            If Not mDR.IsClosed Then
                mDR.Close()
            End If
        End If
        Return mSQLCA.Close
    End Function

    ''' <summary>
    ''' Opens an connection to the database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Open() As Boolean
        Dim TF As Boolean = False
        If mSQLCA.Conn.State = ConnectionState.Open Then
            TF = True
        Else
            TF = mSQLCA.Open
        End If
        Return TF
    End Function

#Region " Handles Stored Procedure Parameters "

    ''' <summary>
    ''' Adds a stored procedure parameter to the class for for execution of a stored procedure
    ''' </summary>
    ''' <param name="parm">This excepts any object, ie. int, string, date, etc...</param>
    ''' <param name="parmName">Paramter name, ie. @parm</param>
    ''' <remarks>Parameters should be added in the order they are expected</remarks>
    Public Sub Add_SP_Parm(ByVal parm As Object, ByVal parmName As String, Optional ByVal IsOutputParm As Boolean = False)
        mParms.AddParm(parm, parmName, IsOutputParm)
    End Sub

    ''' <summary>
    ''' Private sub to conver parms based on connection type
    ''' </summary>
    ''' <param name="parm">Parameter Object</param>
    ''' <param name="parmName">Parameter Name</param>
    ''' <remarks></remarks>
    Private Sub convertParm(ByVal parm As Object, ByVal parmName As String, ByVal IsOutputParm As Boolean)
        Select Case mConnType
            Case cLASERDB.ConnectionType.ODBC
                ODBCconvertParm(parm, parmName)
            Case cLASERDB.ConnectionType.OLEDB
                OLEDBconvertParm(parm, parmName)
            Case cLASERDB.ConnectionType.SQL
                SQLconvertParm(parm, parmName, IsOutputParm)
        End Select
    End Sub

    ''' <summary>
    ''' Private sub converts .Net ojbect into compatilbe OleDb type
    ''' </summary>
    ''' <param name="parm">Parameter Object</param>
    ''' <param name="parmName">Parameter Name</param>
    ''' <remarks></remarks>
    Private Sub OLEDBconvertParm(ByVal parm As Object, ByVal parmName As String)
        Dim lParm As OleDbParameter
        Select Case parm.GetType.ToString
            Case "System.String"
                lParm = New OleDbParameter(parmName, OleDbType.Char)
                lParm.Value = CType(parm, String)
                mCMD.Parameters.Add(lParm)
            Case "System.Int32"
                lParm = New OleDbParameter(parmName, OleDbType.Integer)
                lParm.Value = CType(parm, Integer)
                mCMD.Parameters.Add(lParm)
            Case "System.Int64"
                lParm = New OleDbParameter(parmName, OleDbType.BigInt)
                lParm.Value = CType(parm, Long)
                mCMD.Parameters.Add(lParm)
            Case "System.Decimal"
                lParm = New OleDbParameter(parmName, OleDbType.Numeric)
                lParm.Value = CType(parm, Decimal)
                mCMD.Parameters.Add(lParm)
            Case "System.Double"
                lParm = New OleDbParameter(parmName, OleDbType.Double)
                lParm.Value = CType(parm, Double)
                mCMD.Parameters.Add(lParm)
            Case "System.DateTime"
                lParm = New OleDbParameter(parmName, OleDbType.Date)
                lParm.Value = CType(parm, Date)
                mCMD.Parameters.Add(lParm)
        End Select
    End Sub

    ''' <summary>
    ''' Private sub converts .Net ojbect into compatilbe ODBC type
    ''' </summary>
    ''' <param name="parm">Parameter Object</param>
    ''' <param name="parmName">Parameter Name</param>
    ''' <remarks></remarks>
    Private Sub ODBCconvertParm(ByVal parm As Object, ByVal parmName As String)
        Dim lParm As OdbcParameter
        Select Case parm.GetType.ToString
            Case "System.String"
                lParm = New OdbcParameter(parmName, OdbcType.Char)
                lParm.Value = CType(parm, String)
                mCMD.Parameters.Add(lParm)
            Case "System.Int32"
                lParm = New OdbcParameter(parmName, OdbcType.Int)
                lParm.Value = CType(parm, Integer)
                mCMD.Parameters.Add(lParm)
            Case "System.Int64"
                lParm = New OdbcParameter(parmName, OdbcType.BigInt)
                lParm.Value = CType(parm, Long)
                mCMD.Parameters.Add(lParm)
            Case "System.Decimal"
                lParm = New OdbcParameter(parmName, OdbcType.Numeric)
                lParm.Value = CType(parm, Decimal)
                mCMD.Parameters.Add(lParm)
            Case "System.Double"
                lParm = New OdbcParameter(parmName, OdbcType.Double)
                lParm.Value = CType(parm, Double)
                mCMD.Parameters.Add(lParm)
            Case "System.DateTime"
                lParm = New OdbcParameter(parmName, OdbcType.Date)
                lParm.Value = CType(parm, Date)
                mCMD.Parameters.Add(lParm)
        End Select
    End Sub

    ''' <summary>
    ''' Private sub converts .Net ojbect into compatilbe SQL Server type
    ''' </summary>
    ''' <param name="parm">Parameter Object</param>
    ''' <param name="parmName">Parameter Name</param>
    ''' <remarks></remarks>
    Private Sub SQLconvertParm(ByVal parm As Object, ByVal parmName As String, ByVal IsOutputParm As Boolean)
        Dim lParm As SqlParameter
        If GetType(System.DBNull).ToString = parm.GetType.ToString Then
            lParm = New SqlParameter(parmName, CType(parm, System.DBNull))
            SetSQLParmDirection(lParm, IsOutputParm)
            mCMD.Parameters.Add(lParm)
        Else
            Select Case parm.GetType.ToString
                Case "System.String"
                    lParm = IIf(IsOutputParm, New SqlParameter(parmName, SqlDbType.VarChar, 8000), New SqlParameter(parmName, SqlDbType.Char))
                    lParm.Value = CType(parm, String)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Int32"
                    lParm = New SqlParameter(parmName, SqlDbType.Int)
                    lParm.Value = CType(parm, Integer)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Int64"
                    lParm = New SqlParameter(parmName, SqlDbType.BigInt)
                    lParm.Value = CType(parm, Long)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Decimal"
                    lParm = New SqlParameter(parmName, SqlDbType.Float)
                    lParm.Value = CType(parm, Decimal)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Double"
                    lParm = New SqlParameter(parmName, SqlDbType.Decimal)
                    lParm.Value = CType(parm, Double)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.DateTime"
                    lParm = New SqlParameter(parmName, SqlDbType.DateTime)
                    lParm.Value = CType(parm, Date)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Boolean"
                    lParm = New SqlParameter(parmName, SqlDbType.Bit)
                    lParm.Value = CType(parm, Boolean)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Guid"
                    lParm = New SqlParameter(parmName, SqlDbType.UniqueIdentifier)
                    lParm.Value = CType(parm, Guid)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Byte"
                    lParm = New SqlParameter(parmName, SqlDbType.TinyInt)
                    lParm.Value = CType(parm, Byte)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Int16"
                    lParm = New SqlParameter(parmName, SqlDbType.SmallInt)
                    lParm.Value = CType(parm, Int16)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
                Case "System.Single"
                    lParm = New SqlParameter(parmName, SqlDbType.Real)
                    lParm.Value = CType(parm, Single)
                    SetSQLParmDirection(lParm, IsOutputParm)
                    mCMD.Parameters.Add(lParm)
            End Select
        End If
    End Sub

    Private Sub SetSQLParmDirection(ByRef sqlParm As SqlParameter, ByVal IsOutputParm As Boolean)
        If IsOutputParm Then
            sqlParm.Direction = ParameterDirection.Output
        End If
    End Sub

    ''' <summary>
    ''' Sub Class to cLaserBaseTable that stores SQL Parms
    ''' </summary>
    ''' <remarks></remarks>
    Private Class DBParms
        Protected mParms As ArrayList
        Protected mParmNames As ArrayList
        Protected mParmDirectionIsOutput As ArrayList
        Protected mParmCount As Integer

        Public ReadOnly Property ParmCount() As Integer
            Get
                Return mParmCount
            End Get
        End Property

        Public ReadOnly Property Parms() As ArrayList
            Get
                Return mParms
            End Get
        End Property

        Public ReadOnly Property ParmNames() As ArrayList
            Get
                Return mParmNames
            End Get
        End Property

        Public ReadOnly Property ParmDirectionIsOutput() As ArrayList
            Get
                Return mParmDirectionIsOutput
            End Get
        End Property

        Public Sub New()
            mParms = New ArrayList
            mParmNames = New ArrayList
            mParmDirectionIsOutput = New ArrayList
            mParmCount = 0
        End Sub

        Public Sub AddParm(ByVal parm As Object, ByVal parmName As String, ByVal IsOutputParm As Boolean)
            mParms.Add(parm)
            mParmNames.Add(parmName)
            mParmDirectionIsOutput.Add(IsOutputParm)
            mParmCount += 1
        End Sub

        Public Sub Clear()
            mParms.Clear()
            mParmNames.Clear()
            mParmDirectionIsOutput.Clear()
            mParmCount = 0
        End Sub
    End Class

#End Region

    Private Sub cleardberror()
        mSQLCA.dberror = String.Empty
    End Sub

#Region " Populate Data Table, mDt, With SQL Strings/Stored Procs "

    'Brian Dyer TV Guide 2005

    Public Function SqlSpPopDt(ByVal sStoredProc As String, Optional ByVal FailOnNoResults As Boolean = True) As Boolean
        Dim TF As Boolean = False
        m_TransactionError = False

        cleardberror()
        mDT = New DataTable
        setCMD(sStoredProc)
        mCMD.CommandType = Data.CommandType.StoredProcedure


        For i As Integer = 1 To mParms.ParmCount
            convertParm(mParms.Parms(i - 1), CType(mParms.ParmNames(i - 1), String), CType(mParms.ParmDirectionIsOutput(i - 1), Boolean))
        Next

        Try
            Open()
            mDR = mCMD.ExecuteReader
            mDT = ConvertDRtoDataTable()
            If Not (mDT.Rows.Count = 0 AndAlso FailOnNoResults) Then
                TF = True
            End If
        Catch ex As Exception
            m_TransactionError = True
            mSQLCA.dberror = ex.Message
            If m_isTran Then
                m_tran.Rollback()
            End If
            m_isTran = False
        Finally
            If Not mDR Is Nothing Then
                If Not mDR.IsClosed Then
                    mDR.Close()
                End If
            End If
            If Not (m_isTran Or m_leaveOpen) Then
                Close()
            End If
            mParms.Clear()
        End Try
        Return TF
    End Function

    Public Function SqlStringPopDt(ByVal SQL As String, Optional ByVal FailOnNoResults As Boolean = True) As Boolean
        Dim TF As Boolean = False
        m_TransactionError = False

        cleardberror()
        mDT = New DataTable
        setCMD(SQL)
        Try
            Open()
            mDR = mCMD.ExecuteReader
            mDT = ConvertDRtoDataTable()
            If Not (mDT.Rows.Count = 0 AndAlso FailOnNoResults) Then
                TF = True
            End If
        Catch ex As Exception
            m_TransactionError = True
            mSQLCA.dberror = ex.Message
            If m_isTran Then
                m_tran.Rollback()
            End If
            m_isTran = False
        Finally
            If Not mDR Is Nothing Then
                If Not mDR.IsClosed Then
                    mDR.Close()
                End If
            End If
            If Not (m_isTran Or m_leaveOpen) Then
                Close()
            End If
            mParms.Clear()
        End Try
        Return TF
    End Function

    Private Function ConvertDRtoDataTable() As DataTable
        Dim dt As New DataTable
        cleardberror()

        If Not mDR Is Nothing Then
            If Not mDR.IsClosed Then
                Try
                    dt.Load(mDR)
                    For Each col As DataColumn In dt.Columns
                        If col.ReadOnly Then
                            col.ReadOnly = False
                        End If
                        'KSH 9-19-12 If type is datetime then set datetime to unspecified in order to
                        'keep diff timezones from offsetting dates
                        If col.DataType.FullName.ToString() = "System.DateTime" Then
                            col.DateTimeMode = DataSetDateTime.Unspecified
                        End If

                    Next
                Catch ex As Exception
                    mSQLCA.dberror = ex.Message
                    dt = New DataTable
                End Try
            End If
        End If

        Return dt
    End Function

#End Region

#Region " Non-Query Calls "


    Public Function NonQuerrySql(ByVal sSQL As String) As Boolean
        Dim TF As Boolean = False
        m_TransactionError = False

        cleardberror()
        setCMD(sSQL)

        Try
            Open()
            If mCMD.GetType.ToString = "System.Data.SqlClient.SqlCommand" Then
                If Not m_leaveOpen Then
                    CType(mCMD, SqlCommand).Parameters.Add(New SqlParameter("RETVAL", SqlDbType.Int)).Direction = ParameterDirection.ReturnValue
                End If

                mRowCount = CType(mCMD, SqlCommand).ExecuteNonQuery()

                If Not m_leaveOpen Then
                    mRetVal = CType(mCMD.Parameters("RETVAL").Value, Integer)
                End If

                'Dim tmCMD As SqlCommand = mCMD
                'tmCMD.Parameters.Add(New SqlParameter("RETVAL", SqlDbType.Int)).Direction = ParameterDirection.ReturnValue
                'mRowCount = tmCMD.ExecuteNonQuery()
                'mRetVal = CType(tmCMD.Parameters("RETVAL").Value, Integer)
            Else
                mRowCount = mCMD.ExecuteNonQuery()
            End If

            TF = True
        Catch ex As Exception
            m_TransactionError = True
            mSQLCA.dberror = ex.Message
            If m_isTran Then
                m_tran.Rollback()
            End If
            m_isTran = False
        Finally
            If Not (m_isTran Or m_leaveOpen) Then
                Close()
            End If
        End Try
        Return TF
    End Function

    Public Function NonQuerrySqlSp(ByVal sStoredProc As String) As Boolean
        Dim TF As Boolean = False
        m_TransactionError = False
        Dim OutPutParameters = New List(Of String)()

        cleardberror()
        setCMD(sStoredProc)
        mCMD.CommandType = Data.CommandType.StoredProcedure

        For i As Integer = 1 To mParms.ParmCount
            convertParm(mParms.Parms(i - 1), CType(mParms.ParmNames(i - 1), String), CType(mParms.ParmDirectionIsOutput(i - 1), Boolean))
            If mCMD.GetType.ToString = "System.Data.SqlClient.SqlCommand" AndAlso CType(mParms.ParmDirectionIsOutput(i - 1), Boolean) Then
                OutPutParameters.Add(CType(mParms.ParmNames(i - 1), String))
            End If
        Next

        Try
            Open()
            m_OutPutValues = New Dictionary(Of String, Object)
            If mCMD.GetType.ToString = "System.Data.SqlClient.SqlCommand" Then
                If Not m_leaveOpen Then
                    CType(mCMD, SqlCommand).Parameters.Add(New SqlParameter("RETVAL", SqlDbType.Int)).Direction = ParameterDirection.ReturnValue
                End If

                mRowCount = CType(mCMD, SqlCommand).ExecuteNonQuery()

                For Each OutPutParameter As String In OutPutParameters
                    m_OutPutValues.Add(OutPutParameter, mCMD.Parameters(OutPutParameter).Value)
                Next

                If Not m_leaveOpen Then
                    mRetVal = CType(mCMD.Parameters("RETVAL").Value, Integer)
                End If

                'Dim tmCMD As SqlCommand = mCMD
                'tmCMD.Parameters.Add(New SqlParameter("RETVAL", SqlDbType.Int)).Direction = ParameterDirection.ReturnValue
                'mRowCount = tmCMD.ExecuteNonQuery()
                'mRetVal = CType(tmCMD.Parameters("RETVAL").Value, Integer)
            Else
                mRowCount = mCMD.ExecuteNonQuery()
            End If

            TF = True
        Catch ex As Exception
            m_TransactionError = True
            mSQLCA.dberror = ex.Message
            If m_isTran Then
                m_tran.Rollback()
            End If
            m_isTran = False
        Finally
            If Not (m_isTran Or m_leaveOpen) Then
                Close()
            End If
            mParms.Clear()
        End Try
        Return TF
    End Function

    Public Function NonQuerrySqlSp(ByVal sStoredProc As String, ByRef Value As String) As Boolean
        Dim TF As Boolean = False
        m_TransactionError = False
        cleardberror()
        setCMD(sStoredProc)
        mCMD.CommandType = Data.CommandType.StoredProcedure

        For i As Integer = 1 To mParms.ParmCount
            convertParm(mParms.Parms(i - 1), CType(mParms.ParmNames(i - 1), String), CType(mParms.ParmDirectionIsOutput(i - 1), Boolean))
        Next

        Try
            Open()
            mDR = mCMD.ExecuteReader
            If mDR.HasRows Then
                mDR.Read()
                Value = mDR(0)
                TF = True
            End If
        Catch ex As Exception
            m_TransactionError = True
            mSQLCA.dberror = ex.Message
            If m_isTran Then
                m_tran.Rollback()
            End If
            m_isTran = False
        Finally
            If Not mDR Is Nothing Then
                If Not mDR.IsClosed Then
                    mDR.Close()
                End If
            End If
            If Not (m_isTran Or m_leaveOpen) Then
                Close()
            End If
            mParms.Clear()
        End Try
        Return TF
    End Function

#End Region

#Region " Transaction Methods "

    Public Sub BeginTransaction()
        Me.Open()
        m_tran = mSQLCA.Conn.BeginTransaction
        m_isTran = True
    End Sub

    Public Sub Commit()
        m_isTran = False
        m_tran.Commit()

        If Not m_leaveOpen Then
            Close()
        End If

    End Sub

    Public Sub Rollback()
        If m_isTran Then
            m_isTran = False
            m_tran.Rollback()
        End If

        If Not m_leaveOpen Then
            Close()
        End If
    End Sub

#End Region

#Region " Connection Status Methods "
    Public Sub BeginContinuosConnection()
        Me.Open()
        m_leaveOpen = True
    End Sub

    Public Sub EndContinuosConnection()
        Me.Close()
        m_leaveOpen = False
    End Sub
#End Region

End Class