Imports System.Data.Odbc
Imports System.Text
Public Class FrmLogin
    Public userGroup As String = ""

    'Private Function ceklogin() As Boolean
    '    sqlSelect = "select user_name,user_password,user_id, user_group from m_user where user_name = '" & TxtUsername.Text & _
    '                    "' and user_org_cd='" & cmbOrg.SelectedValue & "'"
    '    ds = New DataSet()
    '    loadConnection.Open()

    '    'dataAdapter = New OdbcDataAdapter(sqlSelect, loadConnection)
    '    dataAdapter.Fill(ds)
    '    loadConnection.Close()

    '    If ds.Tables(0).Rows.Count = 0 Then
    '        Return False
    '    Else
    '        strUsername = (Me.TxtUsername.Text).ToUpper 'CStr(ds.Tables(0).Rows(0).Item(0))
    '        strPassword = CStr(ds.Tables(0).Rows(0).Item(1))
    '        TxtPassword.Text = TxtPassword.Text
    '        CurrentUID = CStr(ds.Tables(0).Rows(0).Item(2))
    '        userGroup = ds.Tables(0).Rows(0)("user_group").ToString

    '        sqlSelect = "select MSP_ORG_CD,MSP_ORG_NAME FROM M_SETUP_parameter "
    '        ds = New DataSet()
    '        loadConnection.Open()
    '        'dataAdapter = New OdbcDataAdapter(sqlSelect, loadConnection)
    '        dataAdapter.Fill(ds)
    '        loadConnection.Close()
    '        If Me.TxtPassword.Text <> strPassword Then
    '            Return False
    '        Else
    '            'MsgBox(TxtPassword.Text + " " & strPassword)
    '            Return True
    '        End If
    '    End If
    'End Function

    Private Sub FrmLogin_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim list As New List(Of String)
        list.Add("Buccheri")
        list.Add("Milano")

        cmbOrg.Properties.DataSource = list

    End Sub

    Private Sub TxtUsername_TextChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtUsername.TextChanged
        TxtUsername.SelectionStart = Len(TxtUsername.Text)
    End Sub

    Private Sub TxtPassword_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TxtPassword.KeyPress
        If e.KeyChar = Chr(13) And Not Me.TxtPassword.Text.Trim = "" Then
            Me.OK.Focus()
        End If
    End Sub

    Private Sub Cancel_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub

    Private Sub OK_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        If cmbOrg.Text = "" Then
            MsgBox("Organisasi tidak boleh kosong")
            cmbOrg.Focus()
            Exit Sub
        End If

        If Me.TxtUsername.Text = "" Then
            MsgBox("Nama Tidak Boleh Kosong")
            Me.TxtUsername.Focus()
        Else
            If cmbOrg.Text = "Buccheri" Then
                constring = "Server=192.168.0.5; Port=3307; Database=db_buccheri; Uid=root; Pwd=password; Allow User Variables=True"
            Else
                constring = "Server=192.168.0.3; Database=milanoHeadOffice; User=sa; Password=110485;"
            End If

            setConn(cmbOrg.Text)

            If ceklogin(cmbOrg.Text, TxtUsername.Text, TxtPassword.Text) = False Then
                MsgBox("Nama Atau Password salah")
                Me.TxtUsername.Clear()
                Me.TxtPassword.Clear()
                Me.TxtUsername.Focus()
            Else
                Me.Hide()
                frmBarcodeProd.ShowDialog()
            End If
        End If
    End Sub

    Private Sub FrmLogin_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'If MsgBox("Apakah anda yakin ingin menutup halaman ini??", MsgBoxStyle.YesNo) = MsgBoxResult.No Then e.Cancel = True
    End Sub
End Class