using iTextSharp.text.html;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Smadot.Utilities.Reporting.Entities
{
    public class PageEventHelper : PdfPageEventHelper
    {

        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;

        //string Logo = GlobalSettingsOrch.PDFApLogo;
        iTextSharp.text.Image Logo = iTextSharp.text.Image.GetInstance("https://storageverificentros.blob.core.windows.net/documentos/SMADSOT/logo-puebla.png");
        string Fondo = "#795176";
        string Direccion = "";
        string Telefono = "";
        bool HeaderFooter = true;
        iTextSharp.text.Image FondoImage = iTextSharp.text.Image.GetInstance("https://storageverificentros.blob.core.windows.net/documentos/SMADSOT/fondo-smadsot.png");
        iTextSharp.text.Image FooterLogo = iTextSharp.text.Image.GetInstance("https://storageverificentros.blob.core.windows.net/documentos/SMADSOT/logo-porpuebla.png");
        public const string AlertInit = ">>>>>>>>>>>";
        public const string AlertEnd = "<<<<<<<<<<<";
        public PageEventHelper(string logo = "", string fondo = "", string direccion = "", string telefono = "", string fondoImage = "", string footerLogo = "", bool headerFooter = true)
        {
            if (!string.IsNullOrWhiteSpace(logo))
            {
                Logo = iTextSharp.text.Image.GetInstance(logo);
            }
            if (!string.IsNullOrWhiteSpace(fondo))
            {
                Fondo = fondo;
            }
            if (!string.IsNullOrWhiteSpace(direccion))
            {
                Direccion = direccion;
            }
            if (!string.IsNullOrWhiteSpace(telefono))
            {
                Telefono = telefono;
            }
            if (!string.IsNullOrWhiteSpace(fondoImage))
            {
                FondoImage = iTextSharp.text.Image.GetInstance(fondoImage);
            }
            if (!string.IsNullOrWhiteSpace(footerLogo))
            {
                FooterLogo = iTextSharp.text.Image.GetInstance(footerLogo);
            }
            HeaderFooter = headerFooter;
        }

        #region Properties
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _HeaderLeft;
        public string HeaderLeft
        {
            get { return _HeaderLeft; }
            set { _HeaderLeft = value; }
        }
        private string _HeaderRight;
        public string HeaderRight
        {
            get { return _HeaderRight; }
            set { _HeaderRight = value; }
        }
        private Font _HeaderFont;
        public Font HeaderFont
        {
            get { return _HeaderFont; }
            set { _HeaderFont = value; }
        }
        private Font _FooterFont;
        public Font FooterFont
        {
            get { return _FooterFont; }
            set { _FooterFont = value; }
        }
        #endregion
        // we override the onOpenDocument method
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                if(HeaderFooter)
                    document.SetMargins(36, 36, 0, 70);
                document.NewPage();
                template = cb.CreateTemplate(document.PageSize.Width, 50);
            }
            catch (DocumentException de)
            {
                string msg = AlertInit + " PDF " + AlertEnd;
                // Get stack trace for the exception with source file information
                var st = new StackTrace(de, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Rectangle pageSize = document.PageSize;
            if (!string.IsNullOrEmpty(Title))
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 15);
                cb.SetRGBColorFill(50, 50, 200);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetTop(40));
                cb.ShowText(Title);
                cb.EndText();
            }
            if (HeaderLeft + HeaderRight != string.Empty)
            {
                PdfPTable HeaderTable = new PdfPTable(2);
                HeaderTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                HeaderTable.TotalWidth = pageSize.Width - 80;
                HeaderTable.SetWidthPercentage(new float[] { 45, 45 }, pageSize);

                PdfPCell HeaderLeftCell = new PdfPCell(new Phrase(8, HeaderLeft, HeaderFont));
                HeaderLeftCell.Padding = 5;
                HeaderLeftCell.PaddingBottom = 8;
                HeaderLeftCell.BorderWidthRight = 0;
                HeaderTable.AddCell(HeaderLeftCell);
                PdfPCell HeaderRightCell = new PdfPCell(new Phrase(8, HeaderRight, HeaderFont));
                HeaderRightCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                HeaderRightCell.Padding = 5;
                HeaderRightCell.PaddingBottom = 8;
                HeaderRightCell.BorderWidthLeft = 0;
                HeaderTable.AddCell(HeaderRightCell);
                cb.SetRGBColorFill(0, 0, 0);
                HeaderTable.WriteSelectedRows(0, -1, pageSize.GetLeft(40), pageSize.GetTop(50), cb);
            }

            if (HeaderFooter)
            {
                PdfPTable tbHeader = new PdfPTable(1);
                tbHeader.TotalWidth = document.PageSize.Width;
                tbHeader.DefaultCell.Border = 0;
                //tbHeader.AddCell(new Paragraph("\n"));

                //if (!string.IsNullOrWhiteSpace(Logo))
                //{
                iTextSharp.text.Image addLogo = Logo;
                iTextSharp.text.BaseColor addFondo = WebColors.GetRGBColor(Fondo);
                addLogo.ScaleAbsolute(document.PageSize.Width, 100f);
                //addLogo.ScalePercent(11);
                //addLogo.ScaleAbsoluteHeight(200);
                PdfPCell _cell = new PdfPCell(addLogo)
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingTop = 0
                };
                _cell.BackgroundColor = addFondo;
                tbHeader.AddCell(_cell);
                //}

                tbHeader.LockedWidth = true;
                var h = tbHeader.TotalHeight;
                document.Add(tbHeader);
                //document.SetMargins(30, 30, document.Top + 20, 30);
                //tbHeader.WriteSelectedRows(0, -1, 0, document.PageSize.Top + 20, writer.DirectContent);

                iTextSharp.text.Image imageBackground = FondoImage;
                imageBackground.Alignment = iTextSharp.text.Image.UNDERLYING;
                imageBackground.SetAbsolutePosition(0, 0);
                document.Add(imageBackground);

                PdfPTable tbfooter = new PdfPTable(2)
                {
                    SpacingBefore = 0F,
                    HorizontalAlignment = Element.ALIGN_TOP,
                    WidthPercentage = 100,
                    SpacingAfter = 0F,
                    TotalWidth = 500f,
                    LockedWidth = true
                };
                tbfooter.TotalWidth = document.PageSize.Width;
                tbfooter.DefaultCell.Border = 0;
                //tbfooter.AddCell(new Paragraph("\n"));

                //if (!string.IsNullOrWhiteSpace(Logo))
                //{
                iTextSharp.text.Image addFlogo = FooterLogo;
                //iTextSharp.text.BaseColor addFondo = WebColors.GetRGBColor(Fondo);
                addFlogo.ScaleAbsolute((document.PageSize.Width / 2), 100f);
                tbfooter.AddCell(new PdfPCell(new Paragraph(Direccion + (!string.IsNullOrEmpty(Direccion) ? "\r\nTel. " : "") + Telefono, new Font(BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, true), 8f, Font.NORMAL, new BaseColor(3, 0, 0))))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = 0,
                });
                //addFlogo.ScalePercent(11);
                PdfPCell _fcell = new PdfPCell(addFlogo)
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingTop = 0
                };
                //_fcell.BackgroundColor = addFondo;
                tbfooter.AddCell(_fcell);
                //}

                var fh = tbfooter.TotalHeight;
                //document.Add(tbfooter);
                //document.SetMargins(30, 30, document.Top + 20, 30);
                tbfooter.WriteSelectedRows(0, -1, 0, document.Bottom + 11, writer.DirectContent);
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            

            int pageN = writer.PageNumber;
            String text = "Página " + pageN;
            float len = bf.GetWidthPoint(text, 8);
            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();

            //cb.BeginText();
            //cb.SetFontAndSize(bf, 8);
            //cb.SetTextMatrix(pageSize.GetRight(85), pageSize.GetBottom(30));
            //cb.ShowText("Versión 5.0");
            //cb.EndText();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }
    }
}
