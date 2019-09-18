Imports System.Data.Odbc
Imports Microsoft.Win32
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.Text

Module Connection_String
    Public gIcon As System.Drawing.Icon
    Public usercode As String
    Public employee_name As String

    Public strUsername As String = ""
    'Public constring As String = "DSN=FactorySystem;"
    Public constring As String
    'Public loadConnection As New OdbcConnection(constring)
    Public loadConnection As Object
    'Public dataAdapter As OdbcDataAdapter
    Public dataAdapter As Object
    Public sqlSelect As String
    Public ds As New DataSet()
    Public ds1 As New DataSet()
    Public ds2 As New DataSet()
    Public ds3 As New DataSet()
    Public ds4 As New DataSet()
    Public ds5 As New DataSet()
    Public ds6 As New DataSet()
    Public dt As New DataTable
    'Public transaction As OdbcTransaction
    Public transaction As Object
    'Public command As New OdbcCommand
    Public command As New Object
    Public strPassword As String = ""
    Public CurrentUID As String = ""
    Public OrgCode As String = ""
    Public OrgName As String = ""
    Public levelNumber As String = ""

    Public Sub setConn(orgName As String)
        If orgName = "Milano" Then
            loadConnection = New SqlConnection(constring)
            command = New SqlCommand
            dataAdapter = New SqlDataAdapter
            transaction = CType(transaction, SqlTransaction)
        Else
            loadConnection = New MySqlConnection(constring)
            command = New MySqlCommand
            dataAdapter = New MySqlDataAdapter
            transaction = CType(transaction, MySqlTransaction)
        End If
    End Sub

    Public Function ceklogin(orgCode As String, userName As String, userPass As String) As Boolean
        loadConnection.Open()
        If orgCode = "Buccheri" Then
            command.commandtext = "select * from z_user where userid='" & userName & "' and ref_userid='" & _
                                Convert.ToBase64String(Encoding.ASCII.GetBytes(userPass)) & "'"
        Else
            command.commandtext = "select * from employee_mst where emp_name='" & userName & "' and emp_pword='" & _
                                Convert.ToBase64String(Encoding.ASCII.GetBytes(userPass)) & "'"
        End If

        command.connection = loadConnection
        command.commandtimeout = 10000
        ds = New DataSet()
        dataAdapter.selectcommand = command
        dataAdapter.Fill(ds)
        loadConnection.Close()

        If ds.Tables(0).Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Sub getData(str As String)
        Try
            loadConnection.open()
            command.connection = loadConnection
            command.commandtext = str
            command.commandtimeout = 10000
            dataAdapter.selectcommand = command
            dataAdapter.fill(dt)
            loadConnection.close()
        Catch ex As Exception
            MsgBox(ex.Message)
            loadConnection.close()
        End Try
    End Sub

    Public Sub getDetail(str As String, dg As DataGridView, prodCode As String)
        Try
            dg.Rows.Clear()

            loadConnection.open()
            command.connection = loadConnection
            command.commandText = str
            command.commandtimeout = 10000
            dataAdapter.selectcommand = command
            Dim dt1 As New DataTable
            dataAdapter.fill(dt1)
            loadConnection.close()

            If dt1.Rows.Count > 0 Then
                For a = 0 To dt1.Rows.Count - 1
                    dg.Rows.Add()
                    dg(1, a).Value = dt1.Rows(a).Item(0).ToString                           'kode_barang
                    dg(2, a).Value = dt1.Rows(a).Item(1).ToString                           'nama_barang
                    dg(3, a).Value = dt1.Rows(a).Item(2).ToString                           'kode_barcode
                    dg(4, a).Value = CInt(dt1.Rows(a).Item(3).ToString)                     'qty
                    dg(5, a).Value = dt1.Rows(a).Item(4).ToString                           'brand
                    dg(6, a).Value = dt1.Rows(a).Item(5).ToString                           'style
                    dg(7, a).Value = dt1.Rows(a).Item(6).ToString                           'color
                    dg(8, a).Value = dt1.Rows(a).Item(7).ToString                           'size
                    dg(9, a).Value = dt1.Rows(a).Item(8).ToString                           'size
                    dg(10, a).Value = Microsoft.VisualBasic.Right(Now.Year.ToString(), 1)   'th1
                    dg(11, a).Value = "Made In Indonesia"                                   'made
                    dg(12, a).Value = dt1.Rows(a).Item(9).ToString & prodCode               'prodCode
                Next
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            loadConnection.close()
        End Try
    End Sub
End Module
