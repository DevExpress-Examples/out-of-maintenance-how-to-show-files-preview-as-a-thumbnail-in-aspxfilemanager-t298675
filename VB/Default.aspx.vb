Imports DevExpress.Web
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrintingLinks
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.XtraRichEdit
Imports DevExpress.Spreadsheet
Imports DevExpress.XtraSpreadsheet.Services
Imports DevExpress.XtraSpreadsheet.Services.Implementation
Imports DevExpress.Web.Internal
Imports DevExpress.Web.Office
Imports System.Web.UI.HtmlControls

Partial Public Class _Default
	Inherits System.Web.UI.Page

	Private RICH_EDIT_TYPES() As String = { ".doc", ".docx", ".rtf", ".html" }
	Private SPREAD_SHEET_TYPES() As String = { ".xls", ".xlsx", ".txt", ".csv" }
	Private Const THUMBNAILS_SIZE As Integer = 200

	Protected Sub ASPxFileManager1_Init(ByVal sender As Object, ByVal e As EventArgs)
		Dim fileManager As ASPxFileManager = TryCast(sender, ASPxFileManager)
		fileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailHeight = THUMBNAILS_SIZE
		fileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailWidth = THUMBNAILS_SIZE
	End Sub

	Protected Sub ASPxFileManager1_CustomThumbnail(ByVal source As Object, ByVal e As FileManagerThumbnailCreateEventArgs)
		Dim item As FileManagerItem = e.Item
		If Path.GetExtension(item.Name) <> String.Empty Then
			Dim url As String = GenerateThumbnail(e.Item)
			e.ThumbnailImage.Url = Page.ResolveUrl(url)
		End If
	End Sub

	Public Shared Function ConvertVirtPathToUrl(ByVal virtPath As String) As String
		Return virtPath.Replace("~\", "").Replace("\"c, "/"c)
	End Function

	Private Function GetFileName(ByVal virtPath As String) As String
		If HttpContext.Current IsNot Nothing Then
			Return HttpContext.Current.Server.MapPath(virtPath)
		End If
		Return UrlUtils.ResolvePhysicalPath(virtPath)
	End Function

	Private Function CreatePrintableComponent(ByVal extention As String, ByVal contentStream As Stream) As IBasePrintable
		Dim component As IBasePrintable
		If RICH_EDIT_TYPES.Contains(extention) Then
			Dim docServer As New RichEditDocumentServer()
			Select Case extention
				Case ".docx"
					docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Doc)
				Case ".doc"
					docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Doc)
				Case ".rtf"
					docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Rtf)
				Case ".html"
					docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Html)
			End Select
			component = docServer
		ElseIf SPREAD_SHEET_TYPES.Contains(extention) Then
			Dim workbook As IWorkbook = New Workbook()
			workbook.AddService(GetType(IChartControllerFactoryService), New ChartControllerFactoryService())
			workbook.AddService(GetType(IChartImageService), New ChartImageService())
			Select Case extention
				Case ".xls"
					workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Xls)
				Case ".xlsx"
					workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Xlsx)
				Case ".txt"
					workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Text)
				Case ".csv"
					workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Csv)
			End Select
			component = workbook
		Else
			Return Nothing
		End If
		Return component
	End Function

	Private Function GenerateThumbnail(ByVal item As FileManagerItem) As String
		Dim virtPath As String = Path.Combine("~", item.FullName)
		Dim fullname As String
		fullname = GetFileName(virtPath)
		Dim extention As String = Path.GetExtension(item.RelativeName)
		Dim contentStream As Stream
		contentStream = File.OpenRead(fullname)
		Using contentStream
			Dim component As IBasePrintable = CreatePrintableComponent(extention, contentStream)
			Dim stream As MemoryStream = InitializeThumbnail(component)
			Dim thumbPath As String = "~\Imgs"
			Dim fullThumbname As String = GetFileName(thumbPath)
			fullThumbname = Path.Combine(fullThumbname, item.Name & ".png")
			If Not File.Exists(fullThumbname) Then
				GenerateThumbnailInternal(stream, fullThumbname, THUMBNAILS_SIZE, THUMBNAILS_SIZE)
			End If
			Return Path.Combine("~\Imgs\", item.Name & ".png")
		End Using
	End Function

	Private Function InitializeThumbnail(ByVal component As IBasePrintable) As MemoryStream
		Dim pcl As New PrintableComponentLinkBase(New PrintingSystemBase())
		pcl.Component = component
		pcl.CreateDocument()
		Dim imgOptions As New ImageExportOptions()
		imgOptions.ExportMode = ImageExportMode.SingleFilePageByPage
		imgOptions.Format = System.Drawing.Imaging.ImageFormat.Png
		imgOptions.PageRange = "1"
		imgOptions.PageBorderWidth = 0
		Dim stream As New MemoryStream()
		pcl.ExportToImage(stream, imgOptions)
		stream.Position = 0
		Return stream
	End Function

	Private Sub GenerateThumbnailInternal(ByVal file As Stream, ByVal thumbnailPath As String, ByVal width As Integer, ByVal height As Integer)
		 Using original As System.Drawing.Image = System.Drawing.Image.FromStream(file)
			Using thumbnail As System.Drawing.Bitmap = ChangeImageSize(original, width, height)
				thumbnail.Save(thumbnailPath)
			End Using
		 End Using
	End Sub

	Private Function ChangeImageSize(ByVal original As System.Drawing.Image, ByVal width As Integer, ByVal height As Integer) As Bitmap
		Dim scaleThumbnail As Single = CSng(height) / CSng(width)

		Dim srcRect As New RectangleF(0.0F, 0.0F, original.Width, original.Width * scaleThumbnail)
		Dim dstRect As New RectangleF(0.0F, 0.0F, width, height)
		Dim thumbnail As New Bitmap(width, height)
		Dim g As Graphics = Graphics.FromImage(thumbnail)
		g.InterpolationMode = InterpolationMode.HighQualityBicubic
		g.DrawImage(original, dstRect, srcRect, GraphicsUnit.Pixel)
		Return thumbnail
	End Function

End Class


