Imports System.Data.Common
Imports System.Data.Odbc
Imports System.Data.OleDb
Imports System.Data.SqlClient

Public Class cLASERDB
    'Base Object for creation of database connections
    'Connections can be setup three ways
    'First = Pass the relevant connection information into the init event
    'Second = Set the properties of the object directly
    'Third = Set the Conn variable equal to another connection object
    Private m_UserID As String
    Private m_Password As String
    Private m_Server As String
    Private m_Database As String
    Private m_ConnectString As String
    Private m_DSN As String
    Private m_Conn As DbConnection
    Private m_ConnType As ConnectionType
    Private m_Provider As ServerType
    Private emessage As String
    Public dberror As String
    Private enumber As Long
    Private ConnectionStringBuilder As cConnectionString
    Private m_CommandTimeout As Integer = 90

    Public ReadOnly Property CommandTimeout() As Integer
        Get
            Return m_CommandTimeout
        End Get
    End Property


    Public ReadOnly Property ConnType() As ConnectionType
        Get
            Return m_ConnType
        End Get
    End Property

    Public ReadOnly Property SrvrType() As ServerType
        Get
            Return m_Provider
        End Get
    End Property

    Public Enum ConnectionType
        OLEDB
        ODBC
        SQL
    End Enum

    Public Enum ServerType
        Oracle
        SQLServer
        SybaseServer
    End Enum

    Public Function InitDB(ByVal SQLServerConnectionString As String, HostName As String) As Boolean
        Dim TF As Boolean = False
        If Not String.IsNullOrEmpty(SQLServerConnectionString) Then
            Dim connBuilder As New SqlConnectionStringBuilder(SQLServerConnectionString)
            connBuilder.WorkstationID = HostName

            m_Server = connBuilder.DataSource
            m_Database = connBuilder.InitialCatalog
            m_CommandTimeout = connBuilder.ConnectTimeout
            m_Provider = ServerType.SQLServer
            m_ConnType = ConnectionType.SQL

            If Not String.IsNullOrEmpty(connBuilder.UserID) Then
                m_UserID = connBuilder.UserID
            End If
            If Not String.IsNullOrEmpty(connBuilder.Password) Then
                m_Password = connBuilder.Password
            End If

            TF = DoInit(connBuilder.ConnectionString)
        End If
        Return TF
    End Function

    Public Function InitDB(ByVal DataServer As String, ByVal byValDatabase As String, ByVal UserName As String, ByVal Pwd As String, ByVal connType As ConnectionType, Optional ByVal DBServerType As ServerType = ServerType.SybaseServer, Optional ByVal CmdTimeout As Integer = 90) As Boolean
        m_Server = DataServer
        m_Database = byValDatabase
        m_UserID = UserName
        m_Password = Pwd
        m_Provider = DBServerType
        m_CommandTimeout = CmdTimeout
        Return DoInit(connType)
    End Function

    Public Function InitDB(ByVal DSN As String, ByVal CmdTimeout As Integer) As Boolean
        m_DSN = DSN
        m_CommandTimeout = CmdTimeout
        Return DoInit(ConnectionType.ODBC)
    End Function

    Public Function InitDB(ByVal DSN As String, ByVal UserName As String, ByVal Pwd As String, Optional ByVal CmdTimeout As Integer = 90) As Boolean
        m_DSN = DSN + ";UID=" + UserName + ";PWD=" + Pwd
        m_CommandTimeout = CmdTimeout
        Return DoInit(ConnectionType.ODBC)
    End Function

    Public Function InitDB(ByVal SQLDataServer As String, ByVal SQLDatabase As String, ByVal CmdTimeout As Integer) As Boolean
        m_Server = SQLDataServer
        m_Database = SQLDatabase
        m_CommandTimeout = CmdTimeout
        Return DoInit(ConnectionType.SQL)
    End Function

    Private Function DoInit(ConnectionString As String) As Boolean
        m_Conn = New SqlConnection()
        Return InitCall(ConnectionString)
    End Function

    Private Function DoInit(ByVal connType As ConnectionType) As Boolean
        Dim TF As Boolean = False
        m_ConnType = connType
        Select Case m_ConnType
            Case ConnectionType.ODBC
                m_Conn = New OdbcConnection()
                TF = InitCallODBC()
            Case ConnectionType.OLEDB
                m_Conn = New OleDbConnection()
                TF = InitCall()
            Case ConnectionType.SQL
                m_Conn = New SqlConnection()
                TF = InitCall()
        End Select
        Return TF
    End Function

    Private Function InitCall(ConnectionString As String) As Boolean
        'Function to initialize and test the connection
        'Verify the connection is currently closed
        Close()
        m_Conn.ConnectionString = ConnectionString
        m_ConnectString = m_Conn.ConnectionString

        'Test the open connection
        If Not Open() Then
            Return False
        End If

        'Close the connection
        If Not Close() Then
            Return False
        End If

        Return True
    End Function

    Private Function InitCall() As Boolean
        'Function to initialize and test the connection
        'Verify the connection is currently closed
        Close()
        ConnectionStringBuilder = New cConnectionString(Me)

        'Set all local property values
        m_Conn.ConnectionString = ConnectionStringBuilder.Build(m_ConnType)
        m_ConnectString = m_Conn.ConnectionString

        'Test the open connection
        If Not Open() Then
            Return False
        End If

        'Close the connection
        If Not Close() Then
            Return False
        End If

        Return True
    End Function

    Private Function InitCallODBC() As Boolean
        'Function to initialize and test the connection
        'Verify the connection is currently closed
        Close()

        'Set all local property values
        m_Conn.ConnectionString = m_DSN
        m_ConnectString = m_DSN

        'Test the open connection
        If Not Open() Then
            Return False
        End If

        'Close the connection
        If Not Close() Then
            Return False
        End If

        Return True
    End Function

    Public Function Open() As Boolean
        'Function to open the connection
        Try
            m_Conn.Open()
        Catch
            dberror = Err.Description
            emessage = Err.Description
            enumber = Err.Number
            Return False
        End Try

        Return True

    End Function

    Public Function Close() As Boolean
        'Function to close the connection
        Try
            m_Conn.Close()
        Catch
            'eMessage = Err.Description
            'eNumber = Err.Number
            Return False
        End Try

        Return True

    End Function

#Region "Property Statements"
    Property UserID() As String
        Get
            UserID = m_UserID
        End Get
        Set(ByVal Value As String)
            m_UserID = Value
        End Set
    End Property

    Property Password() As String
        Get
            Password = m_Password
        End Get
        Set(ByVal Value As String)
            m_Password = Value
        End Set
    End Property

    Property Server() As String
        Get
            Server = m_Server
        End Get
        Set(ByVal Value As String)
            m_Server = Value
        End Set
    End Property

    Property Database() As String
        Get
            Database = m_Database
        End Get
        Set(ByVal Value As String)
            m_Database = Value
        End Set
    End Property

    Property ConnectString() As String
        Get
            ConnectString = m_ConnectString
        End Get
        Set(ByVal Value As String)
            m_ConnectString = Value
            m_Conn.ConnectionString = ConnectString
        End Set
    End Property

    Property Conn() As DbConnection
        Get
            Conn = m_Conn
        End Get
        Set(ByVal Value As DbConnection)
            m_Conn = Value
        End Set
    End Property
#End Region

    Private Class cConnectionString

        Private mConnectionString As String
        Private mConnType As cLASERDB.ConnectionType
        Private mSQLCA As cLASERDB

        Public Sub New(ByVal SQLCA As cLASERDB)
            mSQLCA = SQLCA

        End Sub

        Public Function Build(ByVal ConnType As cLASERDB.ConnectionType) As String
            mConnType = ConnType
            Select Case mConnType
                Case cLASERDB.ConnectionType.OLEDB
                    BuildOLEDBConnectionString()
                Case cLASERDB.ConnectionType.SQL
                    BuildSQLConnectionString()
            End Select
            Return mConnectionString
        End Function

        Private Sub BuildOLEDBConnectionString()
            Select Case mSQLCA.m_Provider
                Case ServerType.Oracle
                    mConnectionString = "Provider=OraOLEDB.Oracle;Data Source=" + mSQLCA.Server _
                        + ";User Id=" + mSQLCA.UserID _
                        + ";Password= " + mSQLCA.Password + ";"
                Case ServerType.SQLServer
                    mConnectionString = "PROVIDER=SQLOLEDB;DATA SOURCE=" + mSQLCA.Server _
                        + ";Initial Catalog=" + mSQLCA.Database _
                        + ";Password=" + mSQLCA.Password _
                        + ";User ID=" + mSQLCA.UserID + ";"
                Case ServerType.SybaseServer
                    mConnectionString = "Provider=Sybase.ASEOLEDBProvider.2;Initial Catalog=" + mSQLCA.Database _
                        + ";Password=" + mSQLCA.Password _
                        + ";User ID=" + mSQLCA.UserID + ";Data Source=" + mSQLCA.Server _
                        + ";Server Name=" + Right(mSQLCA.Server, 5) + ";Network Protocol=Winsock;" _
                        + "Server Port Address= 4100" _
                        + ";Optimize Prepare=Partial;Select Method=Direct;" _
                        + "Raise Error Behavior=MS Compatible;Print Statement Behavior=MS Compatible;" _
                        + "Extended ErrorInfo=FALSE;Stored Proc Row Count=Last Statement Only;" _
                        + "Row Cache Size=50;Enable Quoted Identifiers=0;" _
                        + "Packet Size=1;Default Length For Long Data=1024"
            End Select

        End Sub

        Private Sub BuildSQLConnectionString()
            If mSQLCA.UserID Is Nothing AndAlso mSQLCA.Password Is Nothing Then
                mConnectionString = "packet size=4096;Integrated Security=True;data source=" + mSQLCA.Server _
                                    + ";persist security info=False;initial catalog=" + mSQLCA.Database
            Else
                mConnectionString = "Trusted_Connection=No;packet size=4096;user id=" + mSQLCA.UserID _
                                    + ";password=" + mSQLCA.Password + ";data source=" + mSQLCA.Server _
                                    + ";persist security info=False;initial catalog=" + mSQLCA.Database
            End If
        End Sub
    End Class

End Class