using DevExpress.Web;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraRichEdit;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.Web.Internal;
using DevExpress.Web.Office;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page {

    string[] RICH_EDIT_TYPES = { ".doc", ".docx", ".rtf", ".html" };
    string[] SPREAD_SHEET_TYPES = { ".xls", ".xlsx", ".txt", ".csv" };
    const int THUMBNAILS_SIZE = 200;

    protected void ASPxFileManager1_Init(object sender, EventArgs e) {
        ASPxFileManager fileManager = sender as ASPxFileManager;
        fileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailHeight = THUMBNAILS_SIZE;
        fileManager.SettingsFileList.ThumbnailsViewSettings.ThumbnailWidth = THUMBNAILS_SIZE;
    }

    protected void ASPxFileManager1_CustomThumbnail(object source, FileManagerThumbnailCreateEventArgs e) {
        FileManagerItem item = e.Item;
        if (Path.GetExtension(item.Name) != String.Empty) {
            string url = GenerateThumbnail(e.Item);
            e.ThumbnailImage.Url = Page.ResolveUrl(url);
        }
    }

    public static string ConvertVirtPathToUrl(string virtPath) {
        return virtPath.Replace("~\\", "").Replace('\\', '/');
    }

    private string GetFileName(string virtPath) {
        if (HttpContext.Current != null)
            return HttpContext.Current.Server.MapPath(virtPath);
        return UrlUtils.ResolvePhysicalPath(virtPath);
    }

    IPrintable CreatePrintableComponent(string extention, Stream contentStream) {
        IPrintable component;
        if (RICH_EDIT_TYPES.Contains(extention)) {
            RichEditDocumentServer docServer = new RichEditDocumentServer();
            switch (extention) {
                case ".docx": docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Doc); break;
                case ".doc": docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Doc); break;
                case ".rtf": docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Rtf); break;
                case ".html": docServer.LoadDocument(contentStream, DevExpress.XtraRichEdit.DocumentFormat.Html); break;
            }
            component = docServer;
        }
        else if (SPREAD_SHEET_TYPES.Contains(extention)) {
            IWorkbook workbook = new Workbook();
            workbook.AddService(typeof(IChartControllerFactoryService), new ChartControllerFactoryService());
            workbook.AddService(typeof(IChartImageService), new ChartImageService());
            switch (extention) {
                case ".xls": workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Xls); break;
                case ".xlsx": workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Xlsx); break;
                case ".txt": workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Text); break;
                case ".csv": workbook.LoadDocument(contentStream, DevExpress.Spreadsheet.DocumentFormat.Csv); break;
            }
            component = workbook;
        }
        else
            return null;
        return component;
    }

    string GenerateThumbnail(FileManagerItem item) {
        string virtPath = Path.Combine("~", item.FullName);
        string fullname;
        fullname = GetFileName(virtPath);
        string extention = Path.GetExtension(item.RelativeName);
        Stream contentStream;
        using (contentStream = File.OpenRead(fullname)) {
            IPrintable component = CreatePrintableComponent(extention, contentStream);
            MemoryStream stream = InitializeThumbnail(component);
            string thumbPath = @"~\Imgs";
            string fullThumbname = GetFileName(thumbPath);
            fullThumbname = Path.Combine(fullThumbname, item.Name + ".png");
            if (!File.Exists(fullThumbname))
                GenerateThumbnailInternal(stream, fullThumbname, THUMBNAILS_SIZE, THUMBNAILS_SIZE);
            return Path.Combine("~\\Imgs\\", item.Name + ".png");
        }
    }

    MemoryStream InitializeThumbnail(IPrintable component) {
        PrintableComponentLinkBase pcl = new PrintableComponentLinkBase(new PrintingSystemBase());
        pcl.Component = component;
        pcl.CreateDocument();
        ImageExportOptions imgOptions = new ImageExportOptions();
        imgOptions.ExportMode = ImageExportMode.SingleFilePageByPage;
        imgOptions.Format = System.Drawing.Imaging.ImageFormat.Png;
        imgOptions.PageRange = "1";
        imgOptions.PageBorderWidth = 0;
        MemoryStream stream = new MemoryStream();
        pcl.ExportToImage(stream, imgOptions);
        stream.Position = 0;
        return stream;
    }

    void GenerateThumbnailInternal(Stream file, string thumbnailPath, int width, int height) {
         using(System.Drawing.Image original = System.Drawing.Image.FromStream(file)) {
            using(System.Drawing.Bitmap thumbnail = ChangeImageSize(original, width, height)) {
                thumbnail.Save(thumbnailPath);
            }
        }
    }

    Bitmap ChangeImageSize(System.Drawing.Image original, int width, int height) {
        float scaleThumbnail = (float)height / (float)width;

        RectangleF srcRect = new RectangleF(0.0F, 0.0F, original.Width, original.Width * scaleThumbnail);
        RectangleF dstRect = new RectangleF(0.0F, 0.0F, width, height);
        Bitmap thumbnail = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(thumbnail);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.DrawImage(original, dstRect, srcRect, GraphicsUnit.Pixel);
        return thumbnail;
    }

}


