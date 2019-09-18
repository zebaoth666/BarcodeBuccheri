#Region "#Usings"
Imports System
Imports System.Collections.Generic
Imports System.Drawing.Printing
Imports System.Windows.Forms
Imports DevExpress.XtraPrinting.BarCode
Imports DevExpress.XtraReports.UI
' ...
#End Region ' #Usings

Public Class frmBarcodeSearch
    Dim batasAtas As Integer
    Dim aktif As Boolean = False
    Dim dt1 As DataTable = dt.Clone

    Private Sub fillGrid(x1 As Integer, x2 As Integer, dt0 As DataTable)
        Dim x3 As Integer
        If x2 > dt0.Rows.Count - 1 Then
            x3 = dt0.Rows.Count - 1
        Else
            x3 = x2
        End If

        dgList.Rows.Clear()
        Dim index As Int16 = 0
        For a = x1 To x3
            dgList.Rows.Add()
            index = dgList.Rows.Count - 1
            dgList(0, index).Value = dt0.Rows(a).Item(0).ToString
            dgList(1, index).Value = dt0.Rows(a).Item(1).ToString
            dgList(2, index).Value = dt0.Rows(a).Item(2).ToString
            dgList(3, index).Value = dt0.Rows(a).Item(3).ToString
            dgList(4, index).Value = dt0.Rows(a).Item(4).ToString
        Next

        dgList.CurrentCell = dgList.Item(0, 0)
    End Sub

    Private Sub hal(obj As DataTable)
        'If obj.Rows.Count > 100 Then
        '    'input jml halaman
        '    batasAtas = Math.Floor(obj.Rows.Count / 100)
        '    lblMax.Text = batasAtas
        '    For a = 1 To batasAtas
        '        'cmbHalaman.Items.Add(a)
        '    Next

        '    'set halaman 1
        '    'cmbHalaman.SelectedIndex = 0

        '    'panggil semua data di halaman awal
        '    fillGrid(1, 100, obj)
        'Else
        '    'input jml halaman
        '    batasAtas = Math.Ceiling(obj.Rows.Count / 100)
        '    lblMax.Text = batasAtas
        '    For a = 1 To batasAtas
        '        'cmbHalaman.Items.Add(a)
        '    Next

        '    'set halaman 1
        '    'cmbHalaman.SelectedIndex = 0

        '    'fillGrid((cmbHalaman.SelectedIndex + 1) * 100, ((cmbHalaman.SelectedIndex + 1) * 100) + 100)

        '    fillGrid(1, obj.Rows.Count, obj)
        'End If

        batasAtas = Math.Ceiling(obj.Rows.Count / 100)
        txtPage.Text = batasAtas
        lblMax.Text = batasAtas
        txtPage.Text = "1"
    End Sub

    Private Sub frmBarcodeSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.F1 Then
            btnAmbil.PerformClick()
        End If
    End Sub

    Private Sub frmBarcodeSearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If dt.Rows.Count > 0 Then
            hal(dt)
            aktif = True
        End If

        Me.KeyPreview = True
    End Sub

    Private Sub dgList_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgList.CellDoubleClick
        If dgList.Rows.Count > 0 Then
            frmBarcodeProd.genProdCode(dgList.CurrentRow.Cells(1).Value)
            frmBarcodeProd.txtPO.Text = dgList.CurrentRow.Cells(0).Value
            Me.Dispose()
        End If
    End Sub

    Private Sub btnTutup_Click(sender As Object, e As EventArgs) Handles btnTutup.Click
        Me.Dispose()
    End Sub

    Private Sub btnAmbil_Click(sender As Object, e As EventArgs) Handles btnAmbil.Click
        If dgList.Rows.Count > 0 Then
            frmBarcodeProd.genProdCode(dgList.CurrentRow.Cells(1).Value)
            frmBarcodeProd.txtPO.Text = dgList.CurrentRow.Cells(0).Value
            Me.Dispose()
        End If
    End Sub

    Private Sub txtPO_TextChanged(sender As Object, e As EventArgs) Handles txtPO.TextChanged

    End Sub

    Private Sub ttxPage_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPage.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub ttxPage_Leave(sender As Object, e As EventArgs) Handles txtPage.Leave
        If String.IsNullOrEmpty(txtPage.Text) Then
            txtPage.Text = "1"
        End If
    End Sub

    Private Sub ttxPage_TextChanged(sender As Object, e As EventArgs) Handles txtPage.TextChanged
        If String.IsNullOrEmpty(txtPO.Text) Then
            fillGrid((CInt(txtPage.Text) - 1) * 100, (CInt(txtPage.Text) * 100) - 1, dt)
        Else
            fillGrid((CInt(txtPage.Text) - 1) * 100, (CInt(txtPage.Text) * 100) - 1, dt1)
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If CInt(txtPage.Text) < CInt(lblMax.Text) Then
            txtPage.Text = CInt(txtPage.Text) + 1
        End If
    End Sub

    Private Sub btnLast_Click(sender As Object, e As EventArgs) Handles btnLast.Click
        txtPage.Text = lblMax.Text
    End Sub

    Private Sub btnFirst_Click(sender As Object, e As EventArgs) Handles btnFirst.Click
        txtPage.Text = "1"
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As EventArgs) Handles btnPrev.Click
        If txtPage.Text > "1" Then
            txtPage.Text = CInt(txtPage.Text) - 1
        End If
    End Sub

    Private Sub btnCari_Click(sender As Object, e As EventArgs) Handles btnCari.Click
        If String.IsNullOrEmpty(txtPO.Text) = False Then
            dt1.Clear()
            Dim dr() As DataRow = dt.Select("NO_PO like '%" & Trim(txtPO.Text) & "%'")
            Dim row As DataRow

            For Each row In dr
                dt1.ImportRow(row)
            Next

            dgList.Rows.Clear()
            hal(dt1)
            fillGrid((CInt(txtPage.Text) - 1) * 100, (CInt(txtPage.Text) * 100) - 1, dt1)
        Else
            hal(dt)
        End If
    End Sub
End Class