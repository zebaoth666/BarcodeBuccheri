#Region "#Usings"
Imports System
Imports System.Collections.Generic
Imports System.Drawing.Printing
Imports System.Windows.Forms
Imports DevExpress.XtraPrinting.BarCode
Imports DevExpress.XtraReports.UI
' ...
#End Region ' #Usings

Public Class frmBarcodeProd
    Dim test1 As New XtraReport1
    Dim r1 As Integer = 0, r2 As Integer = 0
    Dim aktif As Boolean = False
    Dim prodCode As String = ""

    Private Sub genRndNum()
        r1 = Microsoft.VisualBasic.Right(CInt(Math.Ceiling(Rnd() * 10)), 1)
        r2 = Microsoft.VisualBasic.Right(CInt(Math.Ceiling(Rnd() * 10)), 1)
    End Sub

    Public Sub genProdCode(tgl As String)
        Dim dd As String = tgl.Substring(0, 2)
        Dim MM As String = tgl.Substring(3, 2)
        Dim yy As String = tgl.Substring(8, 2)
        Dim comb1 As String = Microsoft.VisualBasic.Right(CInt(yy) + CInt(dd), 1)
        Dim comb2 As String = Microsoft.VisualBasic.Left(CInt(yy) + CInt(dd), 1)

        prodCode = "-" & yy & comb1 & dd & comb2 & MM
    End Sub

    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        End
    End Sub

    Private Sub btnCari_Click(sender As Object, e As EventArgs) Handles btnCari.Click
        frmBarcodeSearch.ShowDialog()
    End Sub

    Private Sub frmBarcodeProd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim str As String
        If FrmLogin.cmbOrg.Text = "Buccheri" Then
            str = "select a.*, @row:=@row+1 as baris from" & _
                    " (select pono NO_PO, date_format(podate,'%d-%m-%Y') TANGGAL_PO, suppliercompany NAMA_SUPPLIER, userid PEMBUAT " & _
                    " from t_po, m_supplier where status='0' and t_po.supplierid=m_supplier.supplierid order by pono desc) a, " & _
                    " (select @row:=0) t;"
        Else
            str = "select poh_po_no NO_PO, convert(varchar(10),poh_po_dt,103) TANGGAL_PO, ven_vendor_name NAMA_SUPPLIER, emp_name PEMBUAT, row_number() over (order by poh_po_dt desc, poh_po_no desc) " & _
                " from purchase_order_hdr, vendor_mst, employee_mst " & _
                " where poh_vendor_cd=ven_vendor_cd and emp_cd=poh_maker_id and (poh_status='A' or poh_status='O') " & _
                " and poh_flag='FP' order by poh_po_dt desc, poh_po_no desc"
        End If

        getData(str)
    End Sub

    Private Sub txtPO_TextChanged(sender As Object, e As EventArgs) Handles txtPO.TextChanged
        If String.IsNullOrEmpty(txtPO.Text) = False Then
            Dim str As String
            If FrmLogin.cmbOrg.Text = "Buccheri" Then
                str = "select A.itemid, A.descr, A.barcode, A.qty, A.brand, A.style_desc, A.color, A.size, " & _
                    " concat(B.depan, '-', B.belakang) as range_size, A.tax_flag from " & _
                    " (select brand, style_desc, size, color,  itemid, descr, itemid barcode, qty, gender, tax_flag from t_podetail, vw_itemdescription" & _
                        " where itemid=item_code and pono='" & txtPO.Text & "') A join " & _
                    " (select genderDesc, substring_index(sizedef,',',1) as depan, substring_index(sizedef,',',-1) as belakang from m_gender) B" & _
                    " where A.gender=B.genderDesc;"
            Else
                str = "select pod_itm_cd, itemname, itemCode as barcode_code, pod_qty, " & _
                        " brand, Style, color, size, SizeRange, case (itm_tax_category_cd) when '001' then '1' else '0' end as tax" & _
                        " from purchase_order_dtl, ViewSizeRange, item_mst " & _
                        " where itemCode=pod_itm_cd and pod_po_no='" & txtPO.Text & "' and itemCode=itm_cd"
            End If

            getDetail(str, dgDetail, prodCode)
        End If
    End Sub

    Private Sub btnCetak_Click_1(sender As Object, e As EventArgs) Handles btnCetak.Click
        If dgDetail.Rows.Count > 0 Then
            If dgDetail.IsCurrentCellInEditMode Then dgDetail.EndEdit()
            For a = 0 To dgDetail.Rows.Count - 1
                If dgDetail(0, a).Value = True Then
                    Dim index As DataRow
                    For b = 1 To CInt(dgDetail(4, a).Value)
                        With test1
                            index = .dtSPK.NewRow
                            index(0) = txtPO.Text
                            index(1) = dgDetail(5, a).Value
                            index(2) = dgDetail(6, a).Value
                            index(3) = dgDetail(7, a).Value
                            index(4) = dgDetail(1, a).Value
                            index(5) = dgDetail(8, a).Value
                            index(6) = 1
                            index(7) = dgDetail(9, a).Value
                            index(8) = dgDetail(12, a).Value
                            index(9) = dgDetail(10, a).Value
                            .dtSPK.Rows.Add(index)
                        End With
                    Next
                End If
            Next

            test1.CreateDocument()
            test1.ShowPreviewDialog()
        End If
    End Sub

    Private Sub dgDetail_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgDetail.CellEndEdit
        If String.IsNullOrEmpty(dgDetail.CurrentRow.Cells(4).Value) Then
            dgDetail.CurrentRow.Cells(4).Value = "1"
        End If
    End Sub

    Private Sub dgDetail_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgDetail.CurrentCellDirtyStateChanged
        If dgDetail.Rows.Count > 0 Then
            If dgDetail.CurrentRow.Cells(0).Value = True Then
                dgDetail.CurrentRow.Cells(4).ReadOnly = False
            Else
                dgDetail.CurrentRow.Cells(4).ReadOnly = True
            End If
        End If
    End Sub

    Private Sub dgDetail_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles dgDetail.EditingControlShowing
        If e.Control.GetType.ToString() = "System.Windows.Forms.DataGridViewTextBoxEditingControl" Then
            Dim c As DataGridViewTextBoxEditingControl = CType(e.Control, DataGridViewTextBoxEditingControl)
            RemoveHandler c.KeyPress, AddressOf dgDetail_KeyPress
            AddHandler c.KeyPress, AddressOf dgDetail_KeyPress
        End If
    End Sub

    Private Sub dgDetail_KeyPress(sender As Object, e As KeyPressEventArgs) Handles dgDetail.KeyPress
        If dgDetail.Rows.Count > 0 Then
            If dgDetail.CurrentCell.ColumnIndex = "4" Then
                Select Case e.KeyChar
                    Case "0"c To "9"c ' digits allowed
                        TextBox1.Text = TextBox1.Text & e.KeyChar
                        'Case "."c           ' . allowed as long as there's not one already present
                        '    If TextBox1.Text.Contains(".") Then
                        '        e.Handled = True
                        '    Else
                        '        TextBox1.Text = TextBox1.Text & e.KeyChar
                        '    End If
                    Case ChrW(Keys.Back) ' backspace allowed for deleting (Delete key automatically overrides)
                        If TextBox1.TextLength > 0 Then
                            TextBox1.Text = TextBox1.Text.Substring(0, TextBox1.TextLength - 1)
                        Else
                            TextBox1.Text = ""
                        End If
                    Case Else ' everything else ....
                        e.Handled = True ' .... and it's just like you never pressed a key at all
                End Select
            End If
        End If
    End Sub
End Class