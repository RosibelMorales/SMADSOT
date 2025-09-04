using iTextSharp.text;
using iTextSharp.text.pdf;
using Smadot.Utilities.Reporting.Entities;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using System.Globalization;
using System.Runtime.CompilerServices;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;

namespace Smadot.Utilities.Reporting.Implementacion
{
    public class PdfBuider : IPdfBuider
    {
        public PdfBuider()
        {
        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoSolicitudFormaValorada(List<SolicitudFormaValoradaDocument> solicitud)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Solicitud Forma Valorada");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));
                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Solicitud Forma Valorada", head) { Alignment = Element.ALIGN_CENTER });
                var tblInfo = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f, };

                tblInfo.AddCell(new PdfPCell(new Paragraph("Fecha de Solicitud", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(solicitud.FirstOrDefault().FechaSolicitudFV.ToString("dd/MM/yyyy"), pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Almacén ", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(solicitud.FirstOrDefault().Almacen, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });

                doc.Add(tblInfo);

                var tblSolicitudServicio = new PdfPTable(3) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Tipo de Certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Cantidad", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                // tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio Inicial", BodyTable))
                // {
                //     HorizontalAlignment = Element.ALIGN_CENTER,
                //     Border = 1,
                //     BorderWidthLeft = 0.5f,
                //     BorderWidthBottom = 0,
                //     FixedHeight = 1f,
                //     MinimumHeight = 20f,
                //     PaddingTop = 0f,
                //     UseBorderPadding = false,
                //     BorderColor = colorborder
                // });
                // tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio Final", BodyTable))
                // {
                //     HorizontalAlignment = Element.ALIGN_CENTER,
                //     Border = 1,
                //     BorderWidthLeft = 0.5f,
                //     BorderWidthBottom = 0,
                //     FixedHeight = 1f,
                //     MinimumHeight = 20f,
                //     PaddingTop = 0f,
                //     UseBorderPadding = false,
                //     BorderColor = colorborder
                // });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Clave del Certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                int i = 0;
                foreach (var s in solicitud)
                {
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.TipoCertificadoSC, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.CantidadSC.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    // tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.FolioInicialSC.ToString(), BodyTableMin))
                    // {
                    //     HorizontalAlignment = Element.ALIGN_LEFT,
                    //     Border = 1,
                    //     BorderWidthLeft = 0.5f,
                    //     BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                    //     FixedHeight = 1f,
                    //     MinimumHeight = 20f,
                    //     PaddingTop = 3f,
                    //     PaddingLeft = 1.5f,
                    //     UseBorderPadding = false,
                    //     BorderColor = colorborder
                    // });
                    // tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.FolioFinalSC.ToString(), BodyTableMin))
                    // {
                    //     HorizontalAlignment = Element.ALIGN_LEFT,
                    //     Border = 1,
                    //     BorderWidthLeft = 0.5f,
                    //     BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                    //     FixedHeight = 1f,
                    //     MinimumHeight = 20f,
                    //     PaddingTop = 3f,
                    //     PaddingLeft = 1.5f,
                    //     UseBorderPadding = false,
                    //     BorderColor = colorborder
                    // });
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.ClaveCertificadoSC, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                        BorderWidthRight = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    i++;
                }

                doc.Add(tblSolicitudServicio);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoCertificadoReposicion(CertificadoReposicionResponse data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Certificado Reposición");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Certificado Reposición", head) { Alignment = Element.ALIGN_CENTER });

                var tblSolicitudServicio = new PdfPTable(6) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Placa", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Serie", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio del Certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Vigencia", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Resultados prueba", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Fecha.ToString("dd-MM-yyyy"), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Placa.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Serie.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.FolioCertificado, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Vigencia.ToString("dd-MM-yyyy"), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.ResultadosPrueba.ToString(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                ////ROW 2
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Tipo Certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Semestre", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Marca", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Modelo", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Combustible", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("# tarjeta circulación", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.TipoCertificado, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Semestre, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Marca, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Modelo, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Combustible, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.TarjetaCirculacion, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                doc.Add(tblSolicitudServicio);

                var tblSolicitudServicio2 = new PdfPTable(4) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Tipo de Certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Año", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Semestre", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Folio asignado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(data.TipoCertificadoFV, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(data.Anio.ToString(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(data.Semestre, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(data.FolioCertificado, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                doc.Add(tblSolicitudServicio2);

                var tblInfo = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 1f };
                tblInfo.AddCell(new PdfPCell(new Paragraph("Número de referencia", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(data.NumeroReferencia, BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblInfo.AddCell(new PdfPCell(new Paragraph("", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblInfo.AddCell(new PdfPCell(new Paragraph("", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                doc.Add(tblInfo);

                //var tblInfo = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f };

                //tblInfo.AddCell(new PdfPCell(new Paragraph("Fecha de Solicitud", BodyTable))
                //{
                //    Border = 0,
                //    BorderWidthTop = 1,
                //    BorderWidthBottom = 1,
                //    BorderWidthLeft = 0.5f,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 3f,
                //    UseBorderPadding = true,
                //    BorderColor = noborder
                //});
                //tblInfo.AddCell(new PdfPCell(new Paragraph(solicitud.FirstOrDefault().FechaSolicitudFV.ToString("dd/MM/yyyy"), pTable))
                //{
                //    Border = 0,
                //    BorderWidthTop = 1,
                //    BorderWidthBottom = 1,
                //    BorderWidthRight = 1,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 3f,
                //    UseBorderPadding = true,
                //    BorderColor = noborder
                //});
                //tblInfo.AddCell(new PdfPCell(new Paragraph("Almacén ", BodyTable))
                //{
                //    Border = 0,
                //    BorderWidthTop = 1,
                //    BorderWidthBottom = 1,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 3f,
                //    UseBorderPadding = true,
                //    BorderColor = noborder
                //});
                //tblInfo.AddCell(new PdfPCell(new Paragraph(solicitud.FirstOrDefault().Almacen, pTable))
                //{
                //    Border = 0,
                //    BorderWidthTop = 1,
                //    BorderWidthBottom = 1,
                //    BorderWidthRight = 1,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 3f,
                //    UseBorderPadding = true,
                //    BorderColor = noborder
                //});

                //doc.Add(tblInfo);

                //var tblSolicitudServicio = new PdfPTable(5) { WidthPercentage = 100f, SpacingBefore = 10f };
                //tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Tipo de Certificado", BodyTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    Border = 1,
                //    BorderWidthLeft = 0.5f,
                //    BorderWidthBottom = 0,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder
                //});
                //tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Cantidad", BodyTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    Border = 1,
                //    BorderWidthLeft = 0.5f,
                //    BorderWidthBottom = 0,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder
                //});
                //tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio Inicial", BodyTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    Border = 1,
                //    BorderWidthLeft = 0.5f,
                //    BorderWidthBottom = 0,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder
                //});
                //tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio Final", BodyTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    Border = 1,
                //    BorderWidthLeft = 0.5f,
                //    BorderWidthBottom = 0,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder
                //});
                //tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Clave del Certificado", BodyTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    Border = 1,
                //    BorderWidthLeft = 0.5f,
                //    BorderWidthBottom = 0,
                //    BorderWidthRight = 0.5f,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder
                //});
                //int i = 0;
                //foreach (var s in solicitud)
                //{
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.TipoCertificadoSC, BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.CantidadSC.ToString(), BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.FolioInicialSC.ToString(), BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.FolioFinalSC.ToString(), BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.ClaveCertificadoSC, BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        BorderWidthRight = 0.5f,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    i++;
                //}

                //doc.Add(tblSolicitudServicio);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoVentaCertificados(VentaCertificadoDocument ventaCertificados)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Venta de Formas Valoradas a Verificentros");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Venta de Formas Valoradas a Verificentros", head) { Alignment = Element.ALIGN_CENTER });
                var tblInfo = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f };

                tblInfo.AddCell(new PdfPCell(new Paragraph("Fecha de venta", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(ventaCertificados.FechaVenta.ToString("dd/MM/yyyy"), pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Almacén", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(ventaCertificados.NombreAlmacen, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Número de referencia", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(ventaCertificados.NumeroReferencia, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                // tblInfo.AddCell(new PdfPCell(new Paragraph("Referencia bancaria", BodyTable))
                // {
                //     Border = 0,
                //     BorderWidthTop = 1,
                //     BorderWidthBottom = 1,
                //     FixedHeight = 1f,
                //     MinimumHeight = 20f,
                //     PaddingTop = 3f,
                //     UseBorderPadding = true,
                //     BorderColor = noborder
                // });
                // tblInfo.AddCell(new PdfPCell(new Paragraph(ventaCertificados.ReferenciaBancaria, pTable))
                // {
                //     Border = 0,
                //     BorderWidthTop = 1,
                //     BorderWidthBottom = 1,
                //     BorderWidthRight = 1,
                //     FixedHeight = 1f,
                //     MinimumHeight = 20f,
                //     PaddingTop = 3f,
                //     UseBorderPadding = true,
                //     BorderColor = noborder
                // });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Clave", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(ventaCertificados.Clave?.ToString(), pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("RFC", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(ventaCertificados.Rfc, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Razón social", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(ventaCertificados.NombreVerificentro, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(string.Empty, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });

                doc.Add(tblInfo);

                var tblSolicitudServicio = new PdfPTable(5) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Holograma clave", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Cantidad", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio Inicial", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio Final", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Importe Total", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                foreach (var t in ventaCertificados.certificadosTotalData)
                {
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(t.NombreHolograma, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(t.Cantidad.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(t.FolioInicial, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(t.FolioFinal, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(t.ImporteTotal.ToString("C", new CultureInfo("es-MX")), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = 0.5f,
                        BorderWidthRight = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                }

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("TOTAL", BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(ventaCertificados.TotalCantidadHologramas?.ToString(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(ventaCertificados.SumaImporteTotal?.ToString("C"), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                doc.Add(tblSolicitudServicio);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetRefrendoCertificado(RefrendoCertificadoResponse refrendo)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Certificado");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));
                Font fontDocument = new Font(fuenteB, 26f, Font.BOLD, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Certificado", head) { Alignment = Element.ALIGN_CENTER });
                var tblInfo = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f };

                tblInfo.AddCell(new PdfPCell(new Paragraph("Número de Referencia", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Fecha de Carta Factura", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Vigencia Holograma Anterior", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph("Propietario", BodyTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthLeft = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(refrendo.NumeroReferencia, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(refrendo.FechaCartaFactura.ToString("dd/MM/yyyy"), pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(refrendo.VigenciaHoloAnterior.ToString("dd/MM/yyyy"), pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });
                tblInfo.AddCell(new PdfPCell(new Paragraph(refrendo.Propietario, pTable))
                {
                    Border = 0,
                    BorderWidthTop = 1,
                    BorderWidthBottom = 1,
                    BorderWidthRight = 1,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    UseBorderPadding = true,
                    BorderColor = noborder
                });

                doc.Add(tblInfo);

                var tblSolicitudServicio = new PdfPTable(6) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Marca", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Submarca", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Modelo", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Serie", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Combustible", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("# Tarjeta Circulacion", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                int i = 0;


                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(refrendo.Marca, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(refrendo.Submarca, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(refrendo.Modelo.ToString(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(refrendo.Serie.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(refrendo.Combustible, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(refrendo.NumTarjetaCirculacion, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                doc.Add(tblSolicitudServicio);

                //Se comentan los documentos del Refrendo
                //var urls = new List<string>();
                //if (!string.IsNullOrEmpty(refrendo.UrlDoc1))
                //    urls.Add(refrendo.UrlDoc1);
                //if (!string.IsNullOrEmpty(refrendo.UrlDoc2))
                //    urls.Add(refrendo.UrlDoc2);
                //if (!string.IsNullOrEmpty(refrendo.UrlDoc3))
                //    urls.Add(refrendo.UrlDoc3);
                //i = 1;
                //foreach(var u in urls)
                //{
                //    var urlSplit = u.Split("/");
                //    var nombreArchivo = urlSplit[urlSplit.Length - 1];
                //    var archivo = new StorageManager().ObtenerRuta(u);
                //    if (!string.IsNullOrEmpty(archivo))
                //    {
                //        doc.NewPage();

                //        tblInfo = new PdfPTable(1) { };

                //        for (int j = 0; j < 7; j++)
                //        {
                //            tblInfo.AddCell(new PdfPCell(new Paragraph("", fontDocument))
                //            {
                //                Border = 0,
                //                BorderColor = noborder,
                //                HorizontalAlignment = Element.ALIGN_CENTER,
                //                FixedHeight = 50f
                //            });
                //        }

                //        tblInfo.AddCell(new PdfPCell(new Paragraph("Documento " + i, fontDocument))
                //        {
                //            Border = 0,
                //            BorderColor = noborder,
                //            HorizontalAlignment = Element.ALIGN_CENTER,
                //        });
                //        doc.Add(tblInfo);

                //        doc.NewPage();
                //        var ext = Path.GetExtension(archivo);
                //        if (ext == ".pdf")
                //        {
                //            // we create a reader for the document
                //            PdfReader reader = new PdfReader(archivo);
                //            PdfContentByte cb = writer.DirectContent;
                //            for (int pageNumber = 1; pageNumber < reader.NumberOfPages + 1; pageNumber++)
                //            {
                //                doc.SetPageSize(reader.GetPageSizeWithRotation(1));
                //                doc.NewPage();

                //                //Insert to Destination on the first page
                //                if (pageNumber == 1)
                //                {
                //                    Chunk fileRef = new Chunk(" ");
                //                    fileRef.SetLocalDestination(archivo);
                //                    doc.Add(fileRef);
                //                }

                //                PdfImportedPage page = writer.GetImportedPage(reader, pageNumber);
                //                int rotation = reader.GetPageRotation(pageNumber);
                //                if (rotation == 90 || rotation == 270)
                //                {
                //                    cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(pageNumber).Height);
                //                }
                //                else
                //                {
                //                    cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                //                }
                //            }
                //        }
                //        else if (ext == ".png" || ext == ".jpg")
                //        {
                //            var logo_mexico = Image.GetInstance(archivo);
                //            logo_mexico.WidthPercentage = 25f;
                //            doc.Add(logo_mexico);
                //        }
                //    }
                //    i++;
                //}

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }

        }


        public async Task<ResponseGeneric<DataReport>> GetConstanciaUltimaVerificacion(ConstanciaUltimaVerificacionResponse refrendo)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            //refrendo.verificaciones = new List<vVerificacionCertificado>();
            //refrendo.verificaciones.Add(new vVerificacionCertificado
            //{
            //    FolioCertificado = "2023 / 0001",
            //    Marca = "Chevrolet",
            //    Modelo = 2023,
            //    Serie = "123456",
            //    Placa = "123-456",
            //    Semestre = 2,
            //    Anio = 2023,
            //    Vigencia = DateTime.Parse("12-08-2025"),
            //    ResultadosPrueba = "B",
            //    Fecha = DateTime.Parse("10-05-2025"),
            //    Folio = "123",
            //    Direccion = "Boulevard Ferrocarriles, Colonia San Juan Tejaluca, Atlixco, Puebla",
            //    NombreC = "Gerardo",
            //    Telefono = "55 0123 4567",
            //    NombreEncargado = "MARCO PEREZ CHECO",
            //});         
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();
                doc.SetMargins(80f, 80f, 90f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper(direccion: refrendo.Direccion, telefono: refrendo.Telefono);
                writer.PageEvent = pe;

                doc.AddTitle("Constancia de Última Verificación");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 11f, Font.NORMAL, new BaseColor(0, 0, 0));

                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 10f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));
                Font fontDocument = new Font(fuenteB, 26f, Font.BOLD, new BaseColor(0, 0, 0));
                Font fontDocumentTitle = new Font(fuenteB, 20f, Font.BOLD, new BaseColor(0, 0, 0));
                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 9f, Font.NORMAL, new BaseColor(3, 0, 0));
                Font BodyTableMin2 = new Font(fuenteTableH, 10.2f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);
                //var asd = Directory.GetDirectories();
                iTextSharp.text.Image imageBackground = iTextSharp.text.Image.GetInstance(refrendo.UrlRoot + "/images/pdf/fondo-smadsot.png");
                imageBackground.Alignment = iTextSharp.text.Image.UNDERLYING;
                imageBackground.SetAbsolutePosition(0, 0);
                doc.Add(imageBackground);

                iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(refrendo.UrlRoot + "/images/pdf/logo-puebla.png");
                //image1.ScalePercent(50f);
                image1.ScaleAbsoluteWidth(500);
                image1.ScaleAbsoluteHeight(100);
                image1.SetAbsolutePosition(0, 750);
                doc.Add(image1);

                CultureInfo myCI = new CultureInfo("es-MX", false);
                DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
                var fechaActual = DateTime.Now;
                var dia = fechaActual.Day;
                var mes = fechaActual.Month;
                string nombreMes = myCI.DateTimeFormat.GetMonthName(mes);

                var anio = fechaActual.Year;
                //doc.Add(new Paragraph("Secretaría de Medio Ambiente, Desarrollo\rSustentable y Ordenamiento Territorial", fontDocumentTitle) { Alignment = Element.ALIGN_LEFT });

                doc.Add(new Paragraph("DIRECCIÓN DE GESTIÓN DE CALIDAD DEL AIRE", BodyTable) { Alignment = Element.ALIGN_RIGHT });
                //doc.Add(new Paragraph("Certificado", head) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph("Folio: SMADSOT DGCA/ DVRF / " + refrendo.Folio + "" +
                    "\r\n" + "A " + dia + " del mes de " + nombreMes + " del " + anio, pTable)
                { Alignment = Element.ALIGN_RIGHT });

                doc.Add(new Paragraph("\rCONSTANCIA DE ÚLTIMA VERIFICACIÓN VEHICULAR", head) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph("\rC. " + refrendo.Nombre + "\r\nP R E S E N T E", pBold) { Alignment = Element.ALIGN_LEFT });
                doc.Add(new Paragraph("\rQue con fundamento en lo dispuesto por el artículo 48 apartado A fracción XLII " +
                    "de la Ley de Ingresos del Estado de Puebla para el Ejercicio Fiscal 2023; 14 fracción " +
                    "XI, 19 fracción XXII del Reglamento Interior de la Secretaría de Medio Ambiente, " +
                    "Desarrollo Sustentable y Ordenamiento Territorial; se informa que, después de una minuciosa búsqueda en los archivos de este Departamento:", parrafo)
                { Alignment = Element.ALIGN_JUSTIFIED });
                //doc.Add(new Paragraph("", parrafo)
                //{ Alignment = Element.ALIGN_LEFT });

                doc.Add(new Paragraph("\r\n------------------------------------------ SE HACE CONSTAR LO SIGUIENTE: -----------------------------------", titulo) { Alignment = Element.ALIGN_CENTER });

                var tblInfo = new PdfPTable(new float[] { 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f };

                doc.Add(tblInfo);
                var tblSolicitudServicio1 = new PdfPTable(2)
                {
                    WidthPercentage = 30,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    TotalWidth = 200.0F,
                    LockedWidth = true,
                    SpacingBefore = 10F,
                };

                var tituloTable = new PdfPTable(1) { };

                var tblSolicitudServicio2 = new PdfPTable(2)
                {
                    WidthPercentage = 30,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    TotalWidth = 200.0F,
                    LockedWidth = true,
                    SpacingBefore = 10F,
                };

                var tablaPrincipal = new PdfPTable(2)
                {
                    SpacingBefore = 0F,
                    HorizontalAlignment = Element.ALIGN_TOP,
                    WidthPercentage = 100,
                    SpacingAfter = 0F,
                };
                tablaPrincipal.DefaultCell.Border = 0;

                //refrendo.verificaciones[0]
                tblSolicitudServicio1.AddCell(new PdfPCell(new Phrase("DATOS DEL VEHICULO", HeaderTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Rowspan = 3,
                    Colspan = 2,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Marca:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(refrendo.Marca, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Submarca:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(refrendo.Placa.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Modelo:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(refrendo.Modelo, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Serie:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(refrendo.Serie.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Placas:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(refrendo.Placa.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });


                tblSolicitudServicio2.AddCell(new PdfPCell(new Phrase("REGISTRO DE ÚLTIMA VERIFICACIÓN VEHICULAR", HeaderTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Rowspan = 3,
                    Colspan = 2,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Semestre:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(refrendo.Semestre, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Año:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(refrendo.Anio.ToString(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Vigencia:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(refrendo.Vigencia.ToString("dd/MM/yyyy"), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Holograma:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(refrendo.TipoCertificado, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph("Fecha de consulta:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                });
                tblSolicitudServicio2.AddCell(new PdfPCell(new Paragraph(refrendo.Fecha.ToString("dd/MM/yyyy"), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 3f,
                    PaddingLeft = 1.5f,
                });


                tablaPrincipal.AddCell(tblSolicitudServicio1);
                tablaPrincipal.AddCell(tblSolicitudServicio2);
                doc.Add(tablaPrincipal);
                //doc.Add(tblSolicitudServicio1);
                //doc.Add(tblSolicitudServicio2);

                doc.Add(new Paragraph("Esta constancia no hace las veces de certificado ni holograma, ni regulariza de " +
                    "modo alguno al vehículo en materia de verificación vehicular, así como, tampoco " +
                    "acredita de manera tácita la estancia legal u otro. Asimismo, la presente es únicamente de carácter informativo.\r\n", parrafo)
                { Alignment = Element.ALIGN_JUSTIFIED });
                //doc.Add(new Paragraph("", parrafo) { Alignment = Element.ALIGN_LEFT });
                doc.Add(new Paragraph(" ", parrafo) { Alignment = Element.ALIGN_LEFT });
                doc.Add(new Paragraph("Cabe señalar que la presente constancia es generada a petición de la parte " +
                    "interesada, y se extiende para los efectos que al interesado le convengan.", parrafo)
                { Alignment = Element.ALIGN_LEFT });


                //doc.Add(new Paragraph("\rFIRMA DE VALIDEZ", pBold) { Alignment = Element.ALIGN_CENTER });
                //doc.Add(new Paragraph("\r\r" + refrendo.NombreEncargado +"", pBold) { Alignment = Element.ALIGN_CENTER });
                //doc.Add(new Paragraph("DIRECTOR DE GESTIÓN DE CALIDAD DEL AIRE", pBold) { Alignment = Element.ALIGN_CENTER });
                var qr = new BarcodeQRCode(string.Format("{0}DescargaDocumentos/Index/{1}", refrendo.UrlRoot, refrendo.Id), 100, 100, null);
                var imgqr = qr.GetImage();
                var tbQR = new PdfPTable(new float[] { 80f, 20f }) { WidthPercentage = 100f };
                tbQR.AddCell(new PdfPCell(new Paragraph("\rFIRMA DE VALIDEZ", pBold)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = 0 });
                PdfPCell cell2 = new PdfPCell(imgqr);
                cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                cell2.Border = 0;
                cell2.Rowspan = 3;
                cell2.BorderColor = colorborder;
                tbQR.AddCell(cell2);
                tbQR.AddCell(new PdfPCell(new Paragraph(refrendo.NombreEncargado, pBold)) { MinimumHeight = 55, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Border = 0 });
                tbQR.AddCell(new PdfPCell(new Paragraph("DIRECTOR DE GESTIÓN DE CALIDAD DEL AIRE", pBold)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_BOTTOM, Border = 0 });
                doc.Add(tbQR);
                doc.Add(new Paragraph(" ", pBold) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph("La modificación, alteración, manipulación, uso indebido, tachaduras, " +
                    "enmendaduras de este documento, o la entrega de datos falsos para su obtención, " +
                    "puede constituir un delito en términos de lo previsto en el artículo 250 y 253 del Código Penal del Estado Libre y Soberano de Puebla.", BodyTableMin2)
                { Alignment = Element.ALIGN_JUSTIFIED });
                //doc.Add(new Paragraph(".\r\n", BodyTableMin) { Alignment = Element.ALIGN_LEFT });

                doc.Add(new Paragraph(refrendo.ClaveTramite, BodyTableMin) { Alignment = Element.ALIGN_RIGHT });

                //iTextSharp.text.Image image2 = iTextSharp.text.Image.GetInstance(refrendo.UrlRoot + "/images/pdf/logo-porpuebla.png");
                ////image1.ScalePercent(50f);
                //var tablaPie = new PdfPTable(2)
                //{
                //    SpacingBefore = 0F,
                //    HorizontalAlignment = Element.ALIGN_TOP,
                //    WidthPercentage = 100,
                //    SpacingAfter = 0F,
                //    TotalWidth = 500f,
                //    LockedWidth = true,

                //};
                //tablaPie.DefaultCell.Border = 0;


                //image2.ScaleAbsoluteWidth(450);
                //image2.ScaleAbsoluteHeight(100);
                ////image2.SetAbsolutePosition(0, 0);
                //tablaPie.AddCell(new PdfPCell(new Paragraph(refrendo.Direccion + "\r\nTel. " + refrendo.Telefono, pTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_RIGHT,
                //    Border = 0,
                //});
                //PdfContentByte cb = writer.DirectContent;
                //tablaPie.AddCell(image2);
                //tablaPie.WriteSelectedRows(1, 2, 0, 500,cb);
                //doc.Add(new Phrase(" "));
                //doc.Add(tablaPie);
                //doc.Add(image2);
                //Se comentan los documentos del Refrendo
                //var urls = new List<string>();
                //if (!string.IsNullOrEmpty(refrendo.UrlDoc1))
                //    urls.Add(refrendo.UrlDoc1);
                //if (!string.IsNullOrEmpty(refrendo.UrlDoc2))
                //    urls.Add(refrendo.UrlDoc2);
                //if (!string.IsNullOrEmpty(refrendo.UrlDoc3))
                //    urls.Add(refrendo.UrlDoc3);
                //i = 1;
                //foreach(var u in urls)
                //{
                //    var urlSplit = u.Split("/");
                //    var nombreArchivo = urlSplit[urlSplit.Length - 1];
                //    var archivo = new StorageManager().ObtenerRuta(u);
                //    if (!string.IsNullOrEmpty(archivo))
                //    {
                //        doc.NewPage();

                //        tblInfo = new PdfPTable(1) { };

                //        for (int j = 0; j < 7; j++)
                //        {
                //            tblInfo.AddCell(new PdfPCell(new Paragraph("", fontDocument))
                //            {
                //                Border = 0,
                //                BorderColor = noborder,
                //                HorizontalAlignment = Element.ALIGN_CENTER,
                //                FixedHeight = 50f
                //            });
                //        }

                //        tblInfo.AddCell(new PdfPCell(new Paragraph("Documento " + i, fontDocument))
                //        {
                //            Border = 0,
                //            BorderColor = noborder,
                //            HorizontalAlignment = Element.ALIGN_CENTER,
                //        });
                //        doc.Add(tblInfo);

                //        doc.NewPage();
                //        var ext = Path.GetExtension(archivo);
                //        if (ext == ".pdf")
                //        {
                //            // we create a reader for the document
                //            PdfReader reader = new PdfReader(archivo);
                //            PdfContentByte cb = writer.DirectContent;
                //            for (int pageNumber = 1; pageNumber < reader.NumberOfPages + 1; pageNumber++)
                //            {
                //                doc.SetPageSize(reader.GetPageSizeWithRotation(1));
                //                doc.NewPage();

                //                //Insert to Destination on the first page
                //                if (pageNumber == 1)
                //                {
                //                    Chunk fileRef = new Chunk(" ");
                //                    fileRef.SetLocalDestination(archivo);
                //                    doc.Add(fileRef);
                //                }

                //                PdfImportedPage page = writer.GetImportedPage(reader, pageNumber);
                //                int rotation = reader.GetPageRotation(pageNumber);
                //                if (rotation == 90 || rotation == 270)
                //                {
                //                    cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(pageNumber).Height);
                //                }
                //                else
                //                {
                //                    cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                //                }
                //            }
                //        }
                //        else if (ext == ".png" || ext == ".jpg")
                //        {
                //            var logo_mexico = Image.GetInstance(archivo);
                //            logo_mexico.WidthPercentage = 25f;
                //            doc.Add(logo_mexico);
                //        }
                //    }
                //    i++;
                //}

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }

        }


        public async Task<ResponseGeneric<DataReport>> GetDocumentoCertificadoAdministrativa(CertificadoAdministrativaResponse data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Certificado Administrativa");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Certificado Administrativa", head) { Alignment = Element.ALIGN_CENTER });

                var tblSolicitudServicio = new PdfPTable(2) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Placa", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Serie", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Placa.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.Serie.ToUpper(), BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Número de Referencia", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Folio asignado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.NumeroReferencia, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.FolioAsignado, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Motivo del Trámite", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.MotivoTramite, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(data.FechaRegistro, BodyTableMin))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                doc.Add(tblSolicitudServicio);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }


        public async Task<ResponseGeneric<DataReport>> GetDocumentoFoliosUsadosEnVentanilla(List<FoliosUsadosEnVentanillaDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Folios usados en ventanilla");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Folios usados en ventanilla", head) { Alignment = Element.ALIGN_CENTER });

                var tablaFoliosUsadosEnVentanilla = new PdfPTable(7) { WidthPercentage = 100f, SpacingBefore = 10f };

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Folio de tramite", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Tipo de tramite", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Datos del vehículo", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Folio de certificado o constancia", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Razón", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Referencia bancaria", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                int i = 0;
                foreach (var item in data)
                {
                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.FolioTramite, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.TipoTramite, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.DatosVehiculo, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.FolioCertificado, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.Razon, BodyTableMin))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.ReferenciaBancaria, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.Fecha.ToString("dd/MM/yyyy"), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    i++;
                }

                doc.Add(tablaFoliosUsadosEnVentanilla);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoFoliosRegresadosSPF(List<FoliosRegresadosSPFDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Folios regresados a SPF");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Folios regresados a SPF", head) { Alignment = Element.ALIGN_CENTER });

                var tablaFoliosUsadosEnVentanilla = new PdfPTable(7) { WidthPercentage = 100f, SpacingBefore = 10f };

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Tipo de certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Clave de certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Folio inicial", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Folio final", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Responsable de la entrega", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Persona que validó", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                int i = 0;
                foreach (var item in data)
                {
                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.TipoCertificado, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.ClaveCertificado, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.FolioInicial, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.FolioFinal, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.ResponsableEntrega, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.PersonaValido, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.Fecha.ToString("dd/MM/yyyy"), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        BorderWidthRight = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    i++;
                }

                doc.Add(tablaFoliosUsadosEnVentanilla);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoFoliosVendidosCentroVerificacion(List<FoliosVendidosCentroVerificacionDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Folios vendidos a centros de verificación");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Folios vendidos a centros de verificación", head) { Alignment = Element.ALIGN_CENTER });

                var tablaFoliosUsadosEnVentanilla = new PdfPTable(7) { WidthPercentage = 100f, SpacingBefore = 10f };

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Folio de venta", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Folios de FV", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Clave de venta", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Folios en stock", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Referencia bancaria", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Monto cada venta", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                int i = 0;
                foreach (var item in data)
                {
                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.FolioVenta?.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.FolioFV?.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.ClaveVenta, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.FoliosStock?.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.ReferenciaBancaria, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.MontoCadaVenta?.ToString("C"), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaFoliosUsadosEnVentanilla.AddCell(new PdfPCell(new Paragraph(item.Fecha.ToString("dd/MM/yyyy"), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        BorderWidthRight = 0.5f,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    i++;
                }

                doc.Add(tablaFoliosUsadosEnVentanilla);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoPersonal(PersonalVistaPreviaResponse vm)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Personal");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Personal", head) { Alignment = Element.ALIGN_CENTER });

                var tblSolicitudServicio = new PdfPTable(3) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Nombre", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Genero", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Fecha de Nacimiento", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthRight = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(vm.Nombre ?? "", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph((string.IsNullOrEmpty(vm.Genero) ? "" : vm.GeneroText), BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(vm.FechaNacimiento.ToString("dd/MM/yyyy") ?? "", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("Correo", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("CURP", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph("RFC", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthRight = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(vm.CorreoUsuario ?? "", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(vm.Curp ?? "", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    BorderWidthRight = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(!string.IsNullOrEmpty(vm.Rfc) ? vm.Rfc.ToUpper() : "", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0.5f,
                    BorderWidthRight = 0.5f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                //int i = 0;
                //foreach (var s in solicitud)
                //{
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.TipoCertificadoSC, BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.CantidadSC.ToString(), BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.FolioInicialSC.ToString(), BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.FolioFinalSC.ToString(), BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    tblSolicitudServicio.AddCell(new PdfPCell(new Paragraph(s.ClaveCertificadoSC, BodyTableMin))
                //    {
                //        HorizontalAlignment = Element.ALIGN_LEFT,
                //        Border = 1,
                //        BorderWidthLeft = 0.5f,
                //        BorderWidthBottom = i == solicitud.Count() - 1 ? 0.5f : 0,
                //        BorderWidthRight = 0.5f,
                //        FixedHeight = 1f,
                //        MinimumHeight = 20f,
                //        PaddingTop = 3f,
                //        PaddingLeft = 1.5f,
                //        UseBorderPadding = false,
                //        BorderColor = colorborder
                //    });
                //    i++;
                //}

                doc.Add(tblSolicitudServicio);


                var tblPuestos = new PdfPTable(5) { WidthPercentage = 100f, SpacingBefore = 10f };
                tblPuestos.AddCell(new PdfPCell(new Paragraph("# Trabajador", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblPuestos.AddCell(new PdfPCell(new Paragraph("Puesto", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblPuestos.AddCell(new PdfPCell(new Paragraph("Fecha Cap. Inicio", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblPuestos.AddCell(new PdfPCell(new Paragraph("Fecha Cap. Fin", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tblPuestos.AddCell(new PdfPCell(new Paragraph("Fecha Incorporación", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                if (vm.PuestosResult.Count() > 0)
                {
                    foreach (var item in vm.PuestosResult)
                    {
                        tblPuestos.AddCell(new PdfPCell(new Paragraph(item.NumeroTrabajador, BodyTable))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Border = 1,
                            BorderWidthLeft = 0.5f,
                            BorderWidthBottom = 0,
                            FixedHeight = 1f,
                            MinimumHeight = 20f,
                            PaddingTop = 0f,
                            UseBorderPadding = false,
                            BorderColor = colorborder
                        });
                        tblPuestos.AddCell(new PdfPCell(new Paragraph(item.Puesto, BodyTable))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Border = 1,
                            BorderWidthLeft = 0.5f,
                            BorderWidthBottom = 0,
                            FixedHeight = 1f,
                            MinimumHeight = 20f,
                            PaddingTop = 0f,
                            UseBorderPadding = false,
                            BorderColor = colorborder
                        });
                        tblPuestos.AddCell(new PdfPCell(new Paragraph(item.FechaCapacitacionInicio.ToString("dd/MM/yyyy"), BodyTable))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Border = 1,
                            BorderWidthLeft = 0.5f,
                            BorderWidthBottom = 0,
                            FixedHeight = 1f,
                            MinimumHeight = 20f,
                            PaddingTop = 0f,
                            UseBorderPadding = false,
                            BorderColor = colorborder
                        });
                        tblPuestos.AddCell(new PdfPCell(new Paragraph(item.FechaCapacitacionFinal.ToString("dd/MM/yyyy"), BodyTable))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Border = 1,
                            BorderWidthLeft = 0.5f,
                            BorderWidthBottom = 0,
                            FixedHeight = 1f,
                            MinimumHeight = 20f,
                            PaddingTop = 0f,
                            UseBorderPadding = false,
                            BorderColor = colorborder
                        });
                        tblPuestos.AddCell(new PdfPCell(new Paragraph(item.FechaIncorporacion.ToString("dd/MM/yyyy"), BodyTable))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Border = 1,
                            BorderWidthLeft = 0.5f,
                            BorderWidthBottom = 0,
                            FixedHeight = 1f,
                            MinimumHeight = 20f,
                            PaddingTop = 0f,
                            UseBorderPadding = false,
                            BorderColor = colorborder
                        });
                    }
                }

                doc.Add(tblPuestos);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetComprobanteCita(ComprobanteCitaResponse data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                var ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Comprobante");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 16f, Font.BOLD, new BaseColor(0, 0, 0));
                Font subHead = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pNoBold = new Font(fuenteB, 12f, Font.NORMAL, new BaseColor(0, 0, 0));

                Font pNoBoldNotas = new Font(fuenteB, 9f, Font.NORMAL, new BaseColor(0, 0, 0));

                Font pBoldUnder = new Font(fuenteB, 12f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Confirmación de Cita", head) { Alignment = Element.ALIGN_LEFT });
                doc.Add(new Paragraph("") { Alignment = Element.ALIGN_LEFT });
                doc.Add(new Paragraph("Información Importante:", subHead) { Alignment = Element.ALIGN_LEFT });

                //var content1 = new Paragraph("Confirmación de Cita", head);

                var Paragraph1 = new Paragraph() { Alignment = Element.ALIGN_LEFT };

                var content1 = new Phrase("Estimado(a) ", pNoBold);
                var content2 = new Phrase(string.Format("{0}: ", data.NombrePersona), pBold);
                var content3 = new Phrase("su cita para el trámite ", pNoBold);
                var content4 = new Phrase("Cita para realizar la Verificación Vehicular ", pBold);
                var content5 = new Phrase("ha quedado agendada para el ", pNoBold);
                var content6 = new Phrase(string.Format("{0}, ", data.Fecha.ToString("D", CultureInfo.GetCultureInfo("es-ES"))), pBold);
                var content7 = new Phrase("a las ", pNoBold);
                var content8 = new Phrase(string.Format("{0}, ", data.Fecha.ToString("t", CultureInfo.GetCultureInfo("es-ES"))), pBold);
                var content9 = new Phrase("horas.", pNoBold);

                //Paragraph1.Add(content1);
                //Paragraph1.Add(content2);
                //Paragraph1.Add(content3);
                //Paragraph1.Add(content4);
                //Paragraph1.Add(content5);
                //Paragraph1.Add(content6);
                //Paragraph1.Add(content7);
                //Paragraph1.Add(content8);
                //Paragraph1.Add(content9);
                doc.Add(content1);
                doc.Add(content2);
                doc.Add(content3);
                doc.Add(content4);
                doc.Add(content5);
                doc.Add(content6);
                doc.Add(content7);
                doc.Add(content8);
                doc.Add(content9);

                doc.Add(Paragraph1);


                var Paragraph2 = new Paragraph() { Alignment = Element.ALIGN_LEFT };

                var content10 = new Phrase("Por lo que le reiteramos debe presentarse en el Centro de atención de ", pNoBold);
                var content11 = new Phrase(string.Format("{0} ", data.NombreCentroVerificacion), pBold);
                var content12 = new Phrase("ubicado en ", pNoBold);
                var content13 = new Phrase(string.Format("{0} ", data.DireccionCentroVerificacion), pBold);
                var content14 = new Phrase("en la fecha y hora antes mencionadas.", pNoBold);

                //Paragraph2.Add(content10);
                //Paragraph2.Add(content11);
                //Paragraph2.Add(content12);
                //Paragraph2.Add(content13);
                //Paragraph2.Add(content14);
                doc.Add(content10);
                doc.Add(content11);
                doc.Add(content12);
                doc.Add(content13);
                doc.Add(content14);

                doc.Add(Paragraph2);


                var Paragraph3 = new Paragraph("") { Alignment = Element.ALIGN_LEFT };

                var content15 = new Phrase("Le recordamos que el ", pNoBold);
                var content16 = new Phrase("Folio ", pBold);
                var content17 = new Phrase("de su Cita es:", pNoBold);

                doc.Add(content15);
                doc.Add(content16);
                doc.Add(content17);

                doc.Add(Paragraph3);
                doc.Add(new Paragraph("") { Alignment = Element.ALIGN_LEFT, SpacingAfter = 30f });
                //var tblInfo = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100f, SpacingAfter = 10f, SpacingBefore = 10f };
                var tblInfo = new PdfPTable(1);
                PdfPCell cell1 = new PdfPCell(new Paragraph(data.Folio, subHead));
                cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                cell1.MinimumHeight = 30;
                cell1.BorderWidthTop = 1;
                cell1.BorderWidthBottom = 0;
                cell1.BorderWidthLeft = 0;
                cell1.BorderWidthRight = 0;
                cell1.BorderColor = colorborder;
                tblInfo.AddCell(cell1);

                //var qr = new BarcodeQRCode(data.Folio, 100, 100, null);
                var qr = new BarcodeQRCode(string.Format("{0}PortalCitas/ConsultaCita?folio={1}", data.UrlWeb, data.Folio), 100, 100, null);
                var imgqr = qr.GetImage();

                PdfPCell cell2 = new PdfPCell(imgqr);
                cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                cell2.MinimumHeight = 130;
                cell2.BorderWidthTop = 0;
                cell2.BorderWidthBottom = 1;
                cell2.BorderWidthLeft = 0;
                cell2.BorderWidthRight = 0;
                cell2.BorderColor = colorborder;
                tblInfo.AddCell(cell2);
                //{
                //    Border = 0,
                //    BorderWidthTop = 1,
                //    BorderWidthBottom = 0,
                //    BorderWidthLeft = 0,
                //    BorderWidthRight = 0,
                //    FixedHeight = 1f,
                //    MinimumHeight = 40f,
                //    PaddingTop = 5f,
                //    PaddingBottom = 5f,
                //    PaddingLeft = 25f,
                //    PaddingRight = 25f,
                //    UseBorderPadding = true,
                //    BorderColor = colorborder
                //}
                //tblInfo.AddCell(new PdfPCell(new Paragraph(data.Folio, subHead))
                //{
                //    Border = 0,
                //    BorderWidthTop = 1,
                //    BorderWidthBottom = 0,
                //    BorderWidthLeft = 0,
                //    BorderWidthRight = 0,
                //    FixedHeight = 1f,
                //    MinimumHeight = 40f,
                //    PaddingTop = 5f,
                //    PaddingBottom = 5f,
                //    PaddingLeft = 25f,
                //    PaddingRight = 25f,
                //    UseBorderPadding = true,
                //    BorderColor = colorborder
                //});




                //tblInfo.AddCell(new PdfPCell(imgqr)
                //{
                //    Border = 0,
                //    BorderWidthTop = 0,
                //    BorderWidthBottom = 1,
                //    BorderWidthLeft = 0,
                //    BorderWidthRight = 0,
                //    FixedHeight = 1f,
                //    MinimumHeight = 50f,
                //    PaddingTop = 3f,
                //    UseBorderPadding = true,
                //    BorderColor = colorborder
                //});

                doc.Add(tblInfo);

                doc.Add(new Paragraph("Notas:", subHead) { Alignment = Element.ALIGN_LEFT });

                List list = new List();
                //list.Numbered = true;
                list.SetListSymbol("•");
                list.IndentationLeft = 5;
                var content18 = new Paragraph("   Deberá llegar a su cita al menos 05 minutos antes.", pNoBoldNotas);
                var content19 = new Paragraph("   Presentarse con original y copia de todos los requisitos.", pNoBoldNotas);
                var content20 = new Paragraph("   Se recomienda que el vehículo llegue con la temperatura normal de operación para realizar la prueba.", pNoBoldNotas);
                var content21 = new Paragraph("   No es recomendable prender y apagar el vehículo mientras se encuentra en espera para pasar a la prueba.", pNoBoldNotas);
                var content22 = new Paragraph("   Se recomienda que Vehículos nuevos realicen su prueba de verificación con al menos 300 km recorridos.", pNoBoldNotas);

                //list.Items.Add(content18);
                list.Add(new ListItem(content18));
                list.Add(new ListItem(content19));
                list.Add(new ListItem(content20));
                list.Add(new ListItem(content21));
                list.Add(new ListItem(content22));
                //list.Items.Add(content18);
                //list.Items.Add(content19);
                //list.Items.Add(content20);
                //list.Items.Add(content21);
                //list.Items.Add(content22);
                //doc.Add(new Paragraph(
                //    "1. - Deberá llegar a su cita al menos 05 minutos antes. - Presentarse con original y copia de todos los requisitos. -" +
                //    " Se recomienda que el vehículo llegue con la temperatura normal de operación para realizar la prueba. - " +
                //    "No es recomendable prender y apagar el vehículo mientras se encuentra en espera para pasar a la prueba. - " +
                //    "Se recomienda que Vehículos nuevos realicen su prueba de verificación con al menos 300 km recorridos.",pNoBold) 
                //{ Alignment = Element.ALIGN_LEFT });
                doc.Add(list);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetDevolucionSPF(List<DevolucionSPFDocument> devolucion)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();

            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();
                doc.SetMargins(30f, 30f, 90f, 60f);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper(direccion: devolucion.Count > 0 ? devolucion[0].Direccion : null, telefono: devolucion.Count > 0 ? devolucion[0]?.Telefono : null);
                writer.PageEvent = pe;

                doc.AddTitle("Devolución a SPF");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 11f, Font.NORMAL, new BaseColor(0, 0, 0));

                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 10f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));
                Font fontDocument = new Font(fuenteB, 26f, Font.BOLD, new BaseColor(0, 0, 0));
                Font fontDocumentTitle = new Font(fuenteB, 20f, Font.BOLD, new BaseColor(0, 0, 0));
                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);
                //var asd = Directory.GetDirectories();
                //iTextSharp.text.Image imageBackground = iTextSharp.text.Image.GetInstance(devolucion[0].UrlRoot + "fondo-smadsot.png");
                //imageBackground.Alignment = iTextSharp.text.Image.UNDERLYING;
                //imageBackground.SetAbsolutePosition(0, 0);
                //doc.Add(imageBackground);

                //iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(devolucion[0].UrlRoot + "logo-puebla.png");
                ////image1.ScalePercent(50f);
                //image1.ScaleAbsoluteWidth(500);
                //image1.ScaleAbsoluteHeight(100);
                //image1.SetAbsolutePosition(0, 750);
                //doc.Add(image1);

                CultureInfo myCI = new CultureInfo("es-MX", false);
                DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
                var fechaActual = DateTime.Now;
                var dia = fechaActual.Day;
                var mes = fechaActual.Month;
                string nombreMes = myCI.DateTimeFormat.GetMonthName(mes);

                var anio = fechaActual.Year;

                doc.Add(new Paragraph("DIRECCIÓN DE GESTIÓN DE CALIDAD DEL AIRE", BodyTable) { Alignment = Element.ALIGN_RIGHT });
                //doc.Add(new Paragraph("Folio: SMADSOT DGCA/ DVRF / " + devolucion[0].FolioCertificado + "" +
                //    "\r\n" + devolucion[0].Direccion + " a " + dia + " del mes de " + nombreMes + " del " + anio, pTable)
                //{ Alignment = Element.ALIGN_RIGHT });

                var tblInfo = new PdfPTable(new float[] { 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f };

                doc.Add(tblInfo);
                var tblSolicitudServicio1 = new PdfPTable(8)
                {
                    SpacingBefore = 30F,
                };

                tblSolicitudServicio1.AddCell(new PdfPCell(new Phrase("DEVOLUCIÓN A SPF", HeaderTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Rowspan = 4,
                    Colspan = 9,
                    BorderColor = colorborder
                });
                //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("#", BodyTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    FixedHeight = 1f,
                //    MinimumHeight = 25f,
                //    PaddingTop = 0f,
                //    BorderColor = colorborder
                //});
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Numero de Devolución:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 25f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Fecha de Registro:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Fecha de Entrega:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Usuario que Aprobo:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Responsable de Entrega:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Recibio SPF:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Número de Solicitud:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Cantidad: ", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });

                int i = 1;
                foreach (var item in devolucion)
                {
                    //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(i.ToString(), BodyTableMin))
                    //{
                    //    HorizontalAlignment = Element.ALIGN_CENTER,

                    //    FixedHeight = 1f,
                    //    MinimumHeight = 20f,
                    //    PaddingTop = 3f,
                    //    PaddingLeft = 1.5f,
                    //    BorderColor = colorborder
                    //});
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.NumeroDevolucion.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.FechaRegistro.ToString("dd/MM/yyyy"), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.FechaEntrega.ToString("dd/MM/yyyy"), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.UsuarioAprobo, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.ResponsableEntrega, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.RecibioSPF, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.NumeroSolicitud.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,

                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.Cantidad.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,

                        BorderColor = colorborder
                    });
                    i++;
                }

                doc.Add(tblSolicitudServicio1);

                //iTextSharp.text.Image image2 = iTextSharp.text.Image.GetInstance(devolucion[0].UrlRoot + "logo-porpuebla.png");
                //var tablaPie = new PdfPTable(2)
                //{
                //    SpacingBefore = 0F,
                //    SpacingAfter = 0F,
                //    HorizontalAlignment = Element.ALIGN_RIGHT,
                //    WidthPercentage = 100,
                //    TotalWidth = 500f,
                //    LockedWidth = true,

                //};
                //tablaPie.DefaultCell.Border = 0;
                //tablaPie.AddCell(new PdfPCell(new Paragraph(devolucion[0].Direccion + "\r\nTel. " + devolucion[0].Telefono, pTable))
                //{
                //    HorizontalAlignment = Element.ALIGN_RIGHT,
                //    Border = 0,
                //});
                ////tablaPie.AddCell(image2);

                //var _celly = new PdfPCell(new Paragraph(writer.PageNumber.ToString()));//For page no.
                //_celly.HorizontalAlignment = Element.ALIGN_LEFT;
                //_celly.Border = 0;
                //tablaPie.AddCell(_celly);
                //float[] widths1 = new float[] { 1f, 1f };
                //tablaPie.SetWidths(widths1);
                //tablaPie.WriteSelectedRows(0, -1, doc.LeftMargin, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }

        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoVentaCVV(List<VentaCVVDocument> venta)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();

            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();
                doc.SetMargins(30f, 30f, 90f, 60f);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Devolución a SPF");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 11f, Font.NORMAL, new BaseColor(0, 0, 0));

                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 10f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));
                Font fontDocument = new Font(fuenteB, 26f, Font.BOLD, new BaseColor(0, 0, 0));
                Font fontDocumentTitle = new Font(fuenteB, 20f, Font.BOLD, new BaseColor(0, 0, 0));
                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);
                //var asd = Directory.GetDirectories();
                iTextSharp.text.Image imageBackground = iTextSharp.text.Image.GetInstance(venta[0].UrlRoot + "fondo-smadsot.png");
                imageBackground.Alignment = iTextSharp.text.Image.UNDERLYING;
                imageBackground.SetAbsolutePosition(0, 0);
                doc.Add(imageBackground);

                iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(venta[0].UrlRoot + "logo-puebla.png");
                //image1.ScalePercent(50f);
                image1.ScaleAbsoluteWidth(500);
                image1.ScaleAbsoluteHeight(100);
                image1.SetAbsolutePosition(0, 750);
                doc.Add(image1);

                CultureInfo myCI = new CultureInfo("es-MX", false);
                DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
                var fechaActual = DateTime.Now;
                var dia = fechaActual.Day;
                var mes = fechaActual.Month;
                string nombreMes = myCI.DateTimeFormat.GetMonthName(mes);

                var anio = fechaActual.Year;

                doc.Add(new Paragraph("DIRECCIÓN DE GESTIÓN DE CALIDAD DEL AIRE", BodyTable) { Alignment = Element.ALIGN_RIGHT });
                doc.Add(new Paragraph("Folio: SMADSOT DGCA/ DVRF / " + venta[0].FolioCertificado + "" +
                    "\r\n" + venta[0].Direccion + " a " + dia + " del mes de " + nombreMes + " del " + anio, pTable)
                { Alignment = Element.ALIGN_RIGHT });

                var tblInfo = new PdfPTable(new float[] { 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f };

                doc.Add(tblInfo);
                var tblSolicitudServicio1 = new PdfPTable(6)
                {
                    SpacingBefore = 30F,
                };

                tblSolicitudServicio1.AddCell(new PdfPCell(new Phrase("VENTA DE FORMA VALORADA A CVV", HeaderTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Rowspan = 4,
                    Colspan = 6,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("#", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 1f,
                    MinimumHeight = 25f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Numero de Venta:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 25f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Verificentro:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Usuario Registró:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Cantidad de Hologramas:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });
                tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Fecha de Venta:", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    BorderColor = colorborder
                });


                int i = 1;
                foreach (var item in venta)
                {
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(i.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.NumeroVenta.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.Verificentro, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.UserRegistro, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.CantidadHologramas.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });
                    tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.FechaVenta.ToString("dd/MM/yyyy"), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,

                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 3f,
                        PaddingLeft = 1.5f,
                        BorderColor = colorborder
                    });

                    i++;
                }

                doc.Add(tblSolicitudServicio1);

                iTextSharp.text.Image image2 = iTextSharp.text.Image.GetInstance(venta[0].UrlRoot + "logo-porpuebla.png");
                var tablaPie = new PdfPTable(2)
                {
                    SpacingBefore = 0F,
                    SpacingAfter = 0F,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    WidthPercentage = 100,
                    TotalWidth = 500f,
                    LockedWidth = true,

                };
                tablaPie.DefaultCell.Border = 0;
                tablaPie.AddCell(new PdfPCell(new Paragraph(venta[0].Direccion + "\r\nTel. " + venta[0].Telefono, pTable))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = 0,
                });
                //tablaPie.AddCell(image2);

                var _celly = new PdfPCell(new Paragraph(writer.PageNumber.ToString()));//For page no.
                _celly.HorizontalAlignment = Element.ALIGN_LEFT;
                _celly.Border = 0;
                tablaPie.AddCell(_celly);
                float[] widths1 = new float[] { 1f, 1f };
                tablaPie.SetWidths(widths1);
                tablaPie.WriteSelectedRows(0, -1, doc.LeftMargin, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }

        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoConsultaStockDVRF(List<ConsultaStockDVRFDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper();
                writer.PageEvent = pe;

                doc.AddTitle("Consulta Stock DVRF");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Consulta Stock DVRF", head) { Alignment = Element.ALIGN_CENTER });

                var tablaConsultaStockDVRF = new PdfPTable(6) { WidthPercentage = 100f, SpacingBefore = 10f };

                tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph("Tipo de Certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph("Cantidad", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph("Caja", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph("Folio inicial", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph("Folio final", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph("Clave del Certificado", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = 1,
                    BorderWidthLeft = 0.5f,
                    BorderWidthBottom = 0,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                int i = 0;
                foreach (var item in data)
                {
                    tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph(item.TipoCertificado, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph(item.Cantidad.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph(item.Caja.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph(item.FolioInicial.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph(item.FolioFinal.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tablaConsultaStockDVRF.AddCell(new PdfPCell(new Paragraph(item.ClaveCertificado, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = 1,
                        BorderWidthLeft = 0.5f,
                        BorderWidthBottom = i == data.Count() - 1 ? 0.5f : 0,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    i++;
                }

                doc.Add(tablaConsultaStockDVRF);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetImpresion(ImpresionPDFResponse data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper(headerFooter: false);
                writer.PageEvent = pe;

                //doc.AddTitle("Folios usados en ventanilla");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.BOLD, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));
                Font BodyTableMinBold = new Font(fuenteTableH, 7f, Font.BOLD, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                Font SemestreFont = new Font(fuenteT, 12f, Font.BOLD, new BaseColor(128, 128, 0));
                Font ErrorTableFont = new Font(fuenteT, 7f, Font.BOLD, new BaseColor(255, 0, 0));
                Font VersionFont = new Font(fuenteT, 7f, Font.NORMAL, new BaseColor(128, 128, 0));

                var tabla = new PdfPTable(new float[] { 19f, 19f, 19f, 19f, 3.8f, 3.8f, 3.8f, 12.5f }) { WidthPercentage = 100f, SpacingBefore = 0f, SpacingAfter = 0f };

                //tabla.AddCell(new PdfPCell(new Paragraph(data.Holograma, titulo))
                //{
                //    Border = 0,
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder,
                //    Colspan = 8
                //});
                //tabla.AddCell(new PdfPCell(new Paragraph(data.Semestre.ToUpper(), SemestreFont))
                //{
                //    Border = 0,
                //    HorizontalAlignment = Element.ALIGN_CENTER,
                //    FixedHeight = 1f,
                //    MinimumHeight = 20f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    PaddingLeft = 25f,
                //    BorderColor = colorborder,
                //    Colspan = 7
                //});
                tabla.AddCell(new PdfPCell(new Paragraph(data.Folio.ToString("00000000"), BodyTableMinBold))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });


                tabla.AddCell(new PdfPCell(new Paragraph("Placa:", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph("Marca", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph("Submarca", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph("Año modelo", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph(data.Folio.ToString(), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 4
                });

                tabla.AddCell(new PdfPCell(new Paragraph(data.Placa.ToUpper(), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph(data.Marca, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph(data.Submarca, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph(data.Anio, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 4
                });

                tabla.AddCell(new PdfPCell(new Paragraph(data.Vigencia, ErrorTableFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });

                tabla.AddCell(new PdfPCell(new Paragraph(string.IsNullOrEmpty(data.LeyendaCNA) ? "" : data.LeyendaCNA.ToUpper(), ErrorTableFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });

                tabla.AddCell(new PdfPCell(new Paragraph($"CENTRO CVV: {data.Centro}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph($"LINEA: {data.Linea}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph($"EQUIPO: {data.Equipo}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph($"Nombre o Razón Social: {data.Nombre}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 4
                });

                tabla.AddCell(new PdfPCell(new Paragraph($"FOLIO ANTERIOR: {data.FolioAnterior.ToString()}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });

                doc.Add(tabla);

                var tablaPadre = new PdfPTable(new float[] { 45f, 41f, 14f }) { WidthPercentage = 100f, SpacingBefore = 0f, SpacingAfter = 0f };
                tablaPadre.DefaultCell.Border = 0;
                tablaPadre.DefaultCell.PaddingLeft = 0f;
                tablaPadre.DefaultCell.PaddingRight = 0f;
                tablaPadre.DefaultCell.PaddingBottom = 0f;
                tabla = new PdfPTable(new float[] { 43f, 41f, 16f }) { WidthPercentage = 100f, SpacingBefore = 0f, SpacingAfter = 0f };
                tabla.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph("Hora de Inicio", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph("Hora Fin", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Fecha, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.HoraInicio, ErrorTableFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.HoraFin, ErrorTableFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                //tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                //{
                //    Border = 0,
                //    HorizontalAlignment = Element.ALIGN_LEFT,
                //    FixedHeight = 1f,
                //    MinimumHeight = 10f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder,
                //    Colspan = 3
                //});
                //tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                //{
                //    Border = 0,
                //    HorizontalAlignment = Element.ALIGN_LEFT,
                //    FixedHeight = 1f,
                //    MinimumHeight = 10f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder,
                //    Colspan = 3
                //});
                //tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                //{
                //    Border = 0,
                //    HorizontalAlignment = Element.ALIGN_LEFT,
                //    FixedHeight = 1f,
                //    MinimumHeight = 10f,
                //    PaddingTop = 0f,
                //    UseBorderPadding = false,
                //    BorderColor = colorborder,
                //    Colspan = 3
                //});
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3,
                    Rowspan = 4
                });
                tabla.AddCell(new PdfPCell(new Paragraph("Combustible:", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Combustible, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3,
                    Rowspan = 4
                });
                tabla.AddCell(new PdfPCell(new Paragraph($"Técnico de Captura: {data.TecnicoCapturaNumero}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3,
                    Rowspan = 4
                });
                tabla.AddCell(new PdfPCell(new Paragraph($"Técnico de Prueba: {data.TecnicoPruebaNumero}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });

                tablaPadre.AddCell(tabla);

                tabla = new PdfPTable(4) { WidthPercentage = 100f, SpacingAfter = 0f };
                int i = 0;
                foreach (var item in data.Emisiones ?? new List<ImpresionPDFEmisionResponse>())
                {
                    tabla.AddCell(new PdfPCell(new Paragraph(item.Nombre, i == 0 ? BodyTableMinBold : BodyTableMin))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        FixedHeight = 1f,
                        MinimumHeight = 10f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                    });
                    tabla.AddCell(new PdfPCell(new Paragraph(item.PrimeraColumna, i == 0 ? BodyTableMinBold : BodyTableMin))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        FixedHeight = 1f,
                        MinimumHeight = 10f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                    });
                    tabla.AddCell(new PdfPCell(new Paragraph(item.SegundaColumna, i == 0 ? BodyTableMinBold : BodyTableMin))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        FixedHeight = 1f,
                        MinimumHeight = 10f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                    });
                    tabla.AddCell(new PdfPCell(new Paragraph(item.TerceraColumna, i == 0 ? BodyTableMinBold : BodyTableMin))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        FixedHeight = 1f,
                        MinimumHeight = 10f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                    });
                    i++;
                }
                tablaPadre.AddCell(tabla);

                tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingAfter = 0f };
                if (!string.IsNullOrEmpty(data.UrlExpediente))
                {
                    var qr = new BarcodeQRCode(data.UrlExpediente, 100, 100, null);
                    var imgqr = qr.GetImage();
                    tabla.AddCell(new PdfPCell(imgqr)
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                        Rowspan = 6
                    });
                }
                else
                {
                    tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                        Rowspan = 6
                    });
                }
                tablaPadre.AddCell(tabla);
                doc.Add(tablaPadre);

                tabla = new PdfPTable(3) { WidthPercentage = 100f, SpacingBefore = 10f };
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.AprobadoPor, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph("VERSIÓN SMRN: 1.0", VersionFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                doc.Add(tabla);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetImpresionCertificado(ImpresionPDFResponse data, int certificado = 1)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.LETTER, 36, 36, 30, 0);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper(headerFooter: false);
                // writer.PageEvent = pe;

                // Agregar el evento de página sin paginado
                writer.PageEvent = null;

                //doc.AddTitle("Folios usados en ventanilla");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.BOLD, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));
                Font BodyTableMinBold = new Font(fuenteTableH, 7f, Font.BOLD, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                Font SemestreFont = new Font(fuenteT, 12f, Font.BOLD, new BaseColor(128, 128, 0));
                Font ErrorTableFont = new Font(fuenteT, 7f, Font.BOLD, new BaseColor(255, 0, 0));
                Font VersionFont = new Font(fuenteT, 7f, Font.NORMAL, new BaseColor(128, 128, 0));

                doc.CertificadoVerificacion(certificado, BodyTableMin, colorborder, BodyTableMinBold, data, ErrorTableFont, VersionFont);

                var tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingBefore = 0f, SpacingAfter = 0f };
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 70f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                doc.Add(tabla);

                doc.CertificadoVerificacion(certificado, BodyTableMin, colorborder, BodyTableMinBold, data, ErrorTableFont, VersionFont, true);
                // Verificamos si hay más de una página generada
                //if (writer.PageNumber > 1)
                //{
                //    // Eliminamos la última página
                //    writer.PageCount--;
                //}

                // Cerramos el documento
                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataReport>> GetDocumentoTablaMaestra(ConsultaTablaMaestraPDFUtilitiesResponse obj)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4, 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper(direccion: obj.DirecciónVerificentro, telefono: obj.TelefonoVerificentro);
                writer.PageEvent = pe;

                doc.AddTitle("Consulta Tabla Maestra");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                doc.Add(new Paragraph("Consulta Tabla Maestra", head) { Alignment = Element.ALIGN_CENTER });

                var tb = new PdfPTable(6) { WidthPercentage = 100f, SpacingBefore = 10f };

                tb.AddCell(new PdfPCell(new Paragraph("Marca", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Submarca", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Protocolo", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Desde", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Hasta", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Cilindros", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Cilindrada", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Combustible", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Doble Cero", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                int i = 0;
                foreach (var item in obj.Rows)
                {
                    tb.AddCell(new PdfPCell(new Paragraph(item.Marca, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.SubMarca, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.ProtocoloStr, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.ANO_DESDE.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.ANO_HASTA.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.CILINDROS.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.CILINDRADA.ToString(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.CombustibleStr, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.AplicaDobleCero, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    i++;
                }
                doc.Add(tb);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }
        public async Task<ResponseGeneric<DataReport>> GetDocumentoHistorialCitas(HistorialCitasUtilitiesResponse obj)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.pdf";
            var respuesta = new DataReport();
            try
            {
                Document doc = new Document(PageSize.A4.Rotate(), 36, 36, 50, 36);
                MemoryStream ms = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                var pe = new PageEventHelper(direccion: "", telefono: "");
                writer.PageEvent = pe;

                doc.AddTitle("Historial de Citas");
                doc.Open();

                BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
                BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
                Font parrafo = new Font(fuenteP, 10f, Font.NORMAL, new BaseColor(0, 0, 0));
                Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
                Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

                BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBold = new Font(fuenteB, 12f, Font.BOLD, new BaseColor(0, 0, 0));
                Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));

                BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

                Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

                Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

                Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

                BaseColor backTableH = new BaseColor(192, 192, 192);

                BaseColor colorborder = new BaseColor(211, 211, 211);

                BaseColor noborder = new BaseColor(255, 255, 255);

                if (!string.IsNullOrEmpty(obj.Fecha1) && !string.IsNullOrEmpty(obj.Fecha2))
                {
                    doc.Add(new Paragraph($"Historial de Citas {obj.Fecha1} - {obj.Fecha2}", head) { Alignment = Element.ALIGN_CENTER });
                }
                else
                {
                    doc.Add(new Paragraph("Historial de Citas", head) { Alignment = Element.ALIGN_CENTER });
                }

                var tb = new PdfPTable(8) { WidthPercentage = 100f, SpacingBefore = 10f };

                tb.AddCell(new PdfPCell(new Paragraph("Folio Cita", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });



                tb.AddCell(new PdfPCell(new Paragraph("Placa", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("VIN", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Marca", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });
                tb.AddCell(new PdfPCell(new Paragraph("Submarca", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Centro", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });

                tb.AddCell(new PdfPCell(new Paragraph("Atendida", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });


                tb.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTable))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.2f,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder
                });


                int i = 0;
                foreach (var item in obj.Rows)
                {


                    tb.AddCell(new PdfPCell(new Paragraph(item.Folio, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    // tb.AddCell(new PdfPCell(new Paragraph(item.FolioAsignado.HasValue ? item.FolioAsignado.Value.ToString("00000000") : "", BodyTableMin))
                    // {
                    //     HorizontalAlignment = Element.ALIGN_CENTER,
                    //     VerticalAlignment = Element.ALIGN_MIDDLE,
                    //     FixedHeight = 1f,
                    //     MinimumHeight = 20f,
                    //     Padding = 0f,
                    //     BorderWidth = 0.2f,
                    //     UseBorderPadding = false,
                    //     BorderColor = colorborder
                    // });

                    tb.AddCell(new PdfPCell(new Paragraph(item.Placa.ToUpper(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    tb.AddCell(new PdfPCell(new Paragraph(item.Serie.ToUpper(), BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.Marca, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.Modelo, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.NombreCentro, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });

                    tb.AddCell(new PdfPCell(new Paragraph(item.Progreso, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    tb.AddCell(new PdfPCell(new Paragraph(item.FechaStr, BodyTableMin))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        Padding = 0f,
                        BorderWidth = 0.2f,
                        UseBorderPadding = false,
                        BorderColor = colorborder
                    });
                    // tb.AddCell(new PdfPCell(new Paragraph(item.IngresoManualStr, BodyTableMin))
                    // {
                    //     HorizontalAlignment = Element.ALIGN_CENTER,
                    //     VerticalAlignment = Element.ALIGN_MIDDLE,
                    //     FixedHeight = 1f,
                    //     MinimumHeight = 20f,
                    //     Padding = 0f,
                    //     BorderWidth = 0.2f,
                    //     UseBorderPadding = false,
                    //     BorderColor = colorborder
                    // });


                    i++;
                }
                doc.Add(tb);

                doc.Close();

                resp = ms.ToArray();

                return new ResponseGeneric<DataReport>(new DataReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoPDF = resp
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<DataReport>(ex);
            }
        }

        // public async Task<ResponseGeneric<DataReport>> GetDocumentoDashboardGrid(List<EstadisticasDashboardGridDocumentResponse> data)
        // {
        //     byte[] resp = null;
        //     string archivoPDF = $"{Guid.NewGuid()}.pdf";
        //     var respuesta = new DataReport();

        //     try
        //     {
        //         Document doc = new Document(PageSize.A4, 5, 5, 50, 36);
        //         MemoryStream ms = new MemoryStream();
        //         doc.SetMargins(0f, 0f, 90f, 60f);
        //         PdfWriter writer = PdfWriter.GetInstance(doc, ms);

        //         var pe = new PageEventHelper();
        //         writer.PageEvent = pe;

        //         doc.AddTitle("Verificaciones");
        //         doc.Open();

        //         BaseFont fuenteT = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
        //         Font titulo = new Font(fuenteT, 10f, Font.ITALIC, new BaseColor(0, 0, 0));
        //         BaseFont fuenteP = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
        //         Font parrafo = new Font(fuenteP, 11f, Font.NORMAL, new BaseColor(0, 0, 0));

        //         Font parrafoItalic = new Font(fuenteP, 9f, Font.ITALIC, new BaseColor(0, 0, 0));
        //         Font parrafolist = new Font(fuenteP, 1f, Font.NORMAL, new BaseColor(0, 0, 0));

        //         BaseFont fuenteB = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);

        //         Font head = new Font(fuenteB, 14f, Font.BOLD, new BaseColor(0, 0, 0));
        //         Font pBold = new Font(fuenteB, 10f, Font.BOLD, new BaseColor(0, 0, 0));
        //         Font pBoldUnd = new Font(fuenteB, 10f, Font.BOLD | Font.UNDERLINE, new BaseColor(0, 0, 0));
        //         Font fontDocument = new Font(fuenteB, 26f, Font.BOLD, new BaseColor(0, 0, 0));
        //         Font fontDocumentTitle = new Font(fuenteB, 20f, Font.BOLD, new BaseColor(0, 0, 0));
        //         BaseFont fuenteTableH = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, true);

        //         Font HeaderTable = new Font(fuenteTableH, 10f, Font.BOLD, new BaseColor(3, 0, 0));

        //         Font BodyTable = new Font(fuenteTableH, 9f, Font.BOLD, new BaseColor(3, 0, 0));

        //         Font BodyTableMin = new Font(fuenteTableH, 7f, Font.NORMAL, new BaseColor(3, 0, 0));

        //         Font pTable = new Font(fuenteTableH, 8f, Font.NORMAL, new BaseColor(3, 0, 0));

        //         BaseColor backTableH = new BaseColor(192, 192, 192);

        //         BaseColor colorborder = new BaseColor(211, 211, 211);

        //         BaseColor noborder = new BaseColor(255, 255, 255);
        //         //var asd = Directory.GetDirectories();
        //         //iTextSharp.text.Image imageBackground = iTextSharp.text.Image.GetInstance(data[0].UrlRoot + "fondo-smadsot.png");
        //         //imageBackground.Alignment = iTextSharp.text.Image.UNDERLYING;
        //         //imageBackground.SetAbsolutePosition(0, 0);
        //         //doc.Add(imageBackground);

        //         //iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(data[0].UrlRoot + "logo-puebla.png");
        //         ////image1.ScalePercent(50f);
        //         //image1.ScaleAbsoluteWidth(500);
        //         //image1.ScaleAbsoluteHeight(100);
        //         //image1.SetAbsolutePosition(0, 750);
        //         //doc.Add(image1);

        //         CultureInfo myCI = new CultureInfo("es-MX", false);
        //         DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
        //         var fechaActual = DateTime.Now;
        //         var dia = fechaActual.Day;
        //         var mes = fechaActual.Month;
        //         string nombreMes = myCI.DateTimeFormat.GetMonthName(mes);

        //         //var anio = fechaActual.Year;

        //         //doc.Add(new Paragraph("DIRECCIÓN DE GESTIÓN DE CALIDAD DEL AIRE", BodyTable) { Alignment = Element.ALIGN_RIGHT });
        //         //doc.Add(new Paragraph("Folio: SMADSOT DGCA/ DVRF / " + venta[0].FolioCertificado + "" +
        //         //    "\r\n" + venta[0].Direccion + " a " + dia + " del mes de " + nombreMes + " del " + anio, pTable)
        //         //{ Alignment = Element.ALIGN_RIGHT });

        //         var tblInfo = new PdfPTable(new float[] { 25f }) { WidthPercentage = 100f, SpacingAfter = 1f, SpacingBefore = 10f };

        //         doc.Add(tblInfo);
        //         var tblSolicitudServicio1 = new PdfPTable(11)
        //         {
        //             SpacingBefore = 30F,
        //         };

        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Phrase("VERIFICACIONES", HeaderTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_CENTER,
        //             Rowspan = 4,
        //             Colspan = 13,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Combustible", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_CENTER,
        //             FixedHeight = 1f,
        //             MinimumHeight = 25f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Pruebas:", BodyTable))
        //         //{
        //         //    HorizontalAlignment = Element.ALIGN_LEFT,
        //         //    FixedHeight = 1f,
        //         //    MinimumHeight = 25f,
        //         //    PaddingTop = 0f,
        //         //    BorderColor = colorborder
        //         //});
        //         //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Prueba Emisiones:", BodyTable))
        //         //{
        //         //    HorizontalAlignment = Element.ALIGN_LEFT,
        //         //    FixedHeight = 1f,
        //         //    MinimumHeight = 25f,
        //         //    PaddingTop = 0f,
        //         //    BorderColor = colorborder
        //         //});
        //         //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Prueba Obd:", BodyTable))
        //         //{
        //         //    HorizontalAlignment = Element.ALIGN_LEFT,
        //         //    FixedHeight = 1f,
        //         //    MinimumHeight = 25f,
        //         //    PaddingTop = 0f,
        //         //    BorderColor = colorborder
        //         //});
        //         //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Prueba Opacidad:", BodyTable))
        //         //{
        //         //    HorizontalAlignment = Element.ALIGN_LEFT,
        //         //    FixedHeight = 1f,
        //         //    MinimumHeight = 25f,
        //         //    PaddingTop = 0f,
        //         //    BorderColor = colorborder
        //         //});
        //         //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Sin Multa:", BodyTable))
        //         //{
        //         //    HorizontalAlignment = Element.ALIGN_LEFT,
        //         //    FixedHeight = 1f,
        //         //    MinimumHeight = 20f,
        //         //    PaddingTop = 0f,
        //         //    BorderColor = colorborder
        //         //});
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Marca:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Causa Rechazo:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Aprobado:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Tipo Servicio:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Tipo Certificado:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Verificación activa:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Duración prueba:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("No. Intentos:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Cambio Placas:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });
        //         tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph("Cilindros:", BodyTable))
        //         {
        //             HorizontalAlignment = Element.ALIGN_LEFT,
        //             FixedHeight = 1f,
        //             MinimumHeight = 20f,
        //             PaddingTop = 0f,
        //             BorderColor = colorborder
        //         });


        //         int i = 1;
        //         foreach (var item in data)
        //         {
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.Combustible, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_CENTER,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             //var pruebaEmisionesString = (item.PruebaEmisiones == true) ? "Sí" : "No";
        //             //var pruebaObdString = (item.PruebaObd == true) ? "Sí" : "No";
        //             //var pruebaOpacidadString = (item.PruebaOpacidad == true) ? "Sí" : "No";
        //             //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(string.Format("Prueba Emisiones: {0} ,Prueba Obd: {1} ,Prueba Opacidad: {2}", pruebaEmisionesString, pruebaObdString, pruebaOpacidadString), BodyTableMin))
        //             //{
        //             //    HorizontalAlignment = Element.ALIGN_LEFT,

        //             //    FixedHeight = 1f,
        //             //    MinimumHeight = 20f,
        //             //    PaddingTop = 3f,
        //             //    PaddingLeft = 1.5f,
        //             //    BorderColor = colorborder
        //             //});
        //             //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(pruebaEmisionesString, BodyTableMin))
        //             //{
        //             //    HorizontalAlignment = Element.ALIGN_LEFT,

        //             //    FixedHeight = 1f,
        //             //    MinimumHeight = 20f,
        //             //    PaddingTop = 3f,
        //             //    PaddingLeft = 1.5f,
        //             //    BorderColor = colorborder
        //             //});
        //             //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(pruebaObdString, BodyTableMin))
        //             //{
        //             //    HorizontalAlignment = Element.ALIGN_LEFT,

        //             //    FixedHeight = 1f,
        //             //    MinimumHeight = 20f,
        //             //    PaddingTop = 3f,
        //             //    PaddingLeft = 1.5f,
        //             //    BorderColor = colorborder
        //             //});
        //             //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(pruebaOpacidadString, BodyTableMin))
        //             //{
        //             //    HorizontalAlignment = Element.ALIGN_LEFT,

        //             //    FixedHeight = 1f,
        //             //    MinimumHeight = 20f,
        //             //    PaddingTop = 3f,
        //             //    PaddingLeft = 1.5f,
        //             //    BorderColor = colorborder
        //             //});
        //             //var sinMultaString = (item.SinMulta == true) ? "Sí" : "No";
        //             //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(sinMultaString, BodyTableMin))
        //             //{
        //             //    HorizontalAlignment = Element.ALIGN_LEFT,

        //             //    FixedHeight = 1f,
        //             //    MinimumHeight = 20f,
        //             //    PaddingTop = 3f,
        //             //    PaddingLeft = 1.5f,
        //             //    BorderColor = colorborder
        //             //});
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.Marca, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.CausaRechazoString.ToString(), BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             var aprobadoString = (item.Aprobado == true) ? "Sí" : "No";
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(aprobadoString, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.Nombre, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.TipoCertificadoString, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             var verificacionActivaString = (item.VerificacionActiva == true) ? "Sí" : "No";
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(verificacionActivaString, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.IntervaloMinutos, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.NoIntentos.ToString(), BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             var cambioPlacasString = (item.CambioPlacas == true) ? "Sí" : "No";
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(cambioPlacasString, BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });
        //             tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.CILINDROS.ToString(), BodyTableMin))
        //             {
        //                 HorizontalAlignment = Element.ALIGN_LEFT,

        //                 FixedHeight = 1f,
        //                 MinimumHeight = 20f,
        //                 PaddingTop = 3f,
        //                 PaddingLeft = 1.5f,
        //                 BorderColor = colorborder
        //             });

        //             //tblSolicitudServicio1.AddCell(new PdfPCell(new Paragraph(item.FechaVenta.ToString("dd/MM/yyyy"), BodyTableMin))
        //             //{
        //             //    HorizontalAlignment = Element.ALIGN_LEFT,

        //             //    FixedHeight = 1f,
        //             //    MinimumHeight = 20f,
        //             //    PaddingTop = 3f,
        //             //    PaddingLeft = 1.5f,
        //             //    BorderColor = colorborder
        //             //});

        //             //i++;
        //         }

        //         doc.Add(tblSolicitudServicio1);

        //         //iTextSharp.text.Image image2 = iTextSharp.text.Image.GetInstance(data[0].UrlRoot + "logo-porpuebla.png");
        //         var tablaPie = new PdfPTable(2)
        //         {
        //             SpacingBefore = 0F,
        //             SpacingAfter = 0F,
        //             HorizontalAlignment = Element.ALIGN_RIGHT,
        //             WidthPercentage = 100,
        //             TotalWidth = 500f,
        //             LockedWidth = true,

        //         };
        //         tablaPie.DefaultCell.Border = 0;
        //         //tablaPie.AddCell(new PdfPCell(new Paragraph(venta[0].Direccion + "\r\nTel. " + venta[0].Telefono, pTable))
        //         //{
        //         //    HorizontalAlignment = Element.ALIGN_RIGHT,
        //         //    Border = 0,
        //         //});
        //         //tablaPie.AddCell(image2);

        //         var _celly = new PdfPCell(new Paragraph(writer.PageNumber.ToString()));//For page no.
        //         _celly.HorizontalAlignment = Element.ALIGN_LEFT;
        //         _celly.Border = 0;
        //         tablaPie.AddCell(_celly);
        //         float[] widths1 = new float[] { 1f, 1f };
        //         tablaPie.SetWidths(widths1);
        //         tablaPie.WriteSelectedRows(0, -1, doc.LeftMargin, writer.PageSize.GetBottom(doc.BottomMargin), writer.DirectContent);

        //         doc.Close();

        //         resp = ms.ToArray();

        //         return new ResponseGeneric<DataReport>(new DataReport
        //         {
        //             NombreDocumento = archivoPDF,
        //             DocumentoPDF = resp
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return new ResponseGeneric<DataReport>(ex);
        //     }

        // }

    }

    public static class PdfBuidelExtension
    {
        public static void CertificadoVerificacion(this Document doc,
            int certificado,
            Font BodyTableMin,
            BaseColor colorborder,
            Font BodyTableMinBold,
            ImpresionPDFResponse data,
            Font ErrorTableFont,
            Font VersionFont,
            bool footer = false)
        {
            var FolioAnterior = !string.IsNullOrEmpty(data.FolioAnterior) ? $"{data.FolioAnterior}" : "-";

            var tipoCertificadoVerificacion = new List<int> { 1, 2, 3, 4 };

            var tabla = new PdfPTable(new float[] { 19f, 19f, 19f, 18f, 10f, 3.8f, 3.8f, 12.5f }) { WidthPercentage = 100f, SpacingBefore = 0f, SpacingAfter = 0f };

            if (certificado == TipoCertificadoUtil.Exentos /*|| certificado == TipoCertificadoUtil.ConstanciasNoAprobado*/)
            {
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = footer && certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 55f : footer ? 15f : certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 30f : 5f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });
            }
            if (tipoCertificadoVerificacion.Contains(certificado) || certificado == TipoCertificadoUtil.ConstanciasNoAprobado)
            {
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = !footer && certificado == TipoCertificadoUtil.ConstanciasNoAprobado? 55f:footer && certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 50f:!footer ? 18f : 15f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });
            }

            tabla.AddCell(new PdfPCell(new Paragraph(data.Semestre, BodyTableMinBold))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });
            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });

            //tabla.AddCell(new PdfPCell(new Paragraph(data.Folio.ToString("00000000"), BodyTableMinBold))
            //{
            //    Border = 0,
            //    HorizontalAlignment = Element.ALIGN_RIGHT,
            //    FixedHeight = 1f,
            //    MinimumHeight = 20f,
            //    PaddingTop = 0f,
            //    UseBorderPadding = false,
            //    BorderColor = colorborder,
            //    Colspan = 8
            //});

            var isVisiblePlaca = (certificado == TipoCertificadoUtil.ConstanciasNoAprobado || tipoCertificadoVerificacion.Contains(certificado));

            if (isVisiblePlaca)
            {
                tabla.AddCell(new PdfPCell(new Paragraph("Placa:", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
            }

            tabla.AddCell(new PdfPCell(new Paragraph("Marca", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });
            var submarca_lg = false;//(data.Submarca.Length > data.NumSerie.Length);
            tabla.AddCell(new PdfPCell(new Paragraph("Submarca", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                // Colspan = (submarca_lg) ? 2 : 1
            });

            tabla.AddCell(new PdfPCell(new Paragraph("VIN", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph("Año modelo", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            var colspanRowVehicle = (4 - ((isVisiblePlaca ? 1 : 0) + (submarca_lg ? 1 : 0)));

            if (certificado != TipoCertificadoUtil.ConstanciasNoAprobado)
            {
                tabla.AddCell(new PdfPCell(new Paragraph(data.Folio.ToString("00000000"), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = colspanRowVehicle
                });
            }
            else
            {
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = colspanRowVehicle
                });
            }


            if (isVisiblePlaca)
            {
                tabla.AddCell(new PdfPCell(new Paragraph(data.Placa.ToUpper(), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
            }

            tabla.AddCell(new PdfPCell(new Paragraph(data.Marca, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.Submarca, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                // Colspan = (submarca_lg) ? 2 : 1
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.NumSerie, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.Anio, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });
            if (certificado == TipoCertificadoUtil.ConstanciasNoAprobado)
            {
                tabla.AddCell(new PdfPCell(new Paragraph(data.Folio.ToString("00000000"), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 30f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = colspanRowVehicle
                });
            }
            else
            {

                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = colspanRowVehicle
                });
            }

            // Begin Row -> Leyenda CNA

            if (!string.IsNullOrEmpty(data.LeyendaCNA))
            {

                // tabla.AddCell(new PdfPCell(new Paragraph(data.Vigencia, ErrorTableFont))
                // {
                //     Border = 0,
                //     HorizontalAlignment = Element.ALIGN_RIGHT,
                //     FixedHeight = 1f,
                //     MinimumHeight = 20f,
                //     PaddingTop = 0f,
                //     UseBorderPadding = false,
                //     BorderColor = colorborder,
                //     Colspan = 8
                // });

                tabla.AddCell(new PdfPCell(new Paragraph(string.IsNullOrEmpty(data.LeyendaCNA) ? "" : data.LeyendaCNA.ToUpper(), ErrorTableFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });
            }

            // End Row -> Leyenda CNA

            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });

            // BEGIN :: ROW

            if (!tipoCertificadoVerificacion.Contains(certificado))
            {

                tabla.AddCell(new PdfPCell(new Paragraph(certificado == TipoCertificadoUtil.Exentos ? $"CENTRO: {data.Centro}" : $"CENTRO CVV: {data.Centro}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    Colspan = certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 2 : 3,
                    BorderColor = colorborder,
                });

                if (certificado == TipoCertificadoUtil.ConstanciasNoAprobado)
                {
                    tabla.AddCell(new PdfPCell(new Paragraph(certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? (data.IdCatTipoTramite != 1 ? "LINEA: " : "") + $"{data.Linea}" : string.Empty, BodyTableMin))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        FixedHeight = 1f,
                        MinimumHeight = 20f,
                        PaddingTop = 0f,
                        UseBorderPadding = false,
                        BorderColor = colorborder,
                    });
                }
            }
            else
            {

                tabla.AddCell(new PdfPCell(new Paragraph(tipoCertificadoVerificacion.Contains(certificado) ? (data.IdCatTipoTramite != 1 ? "LINEA: " : "") + $"{data.Linea}" : string.Empty, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

                tabla.AddCell(new PdfPCell(new Paragraph(tipoCertificadoVerificacion.Contains(certificado) ? (data.IdCatTipoTramite != 1 ? "EQUIPO: " : "") + $"{data.Equipo}" : string.Empty, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });

            }

            tabla.AddCell(new PdfPCell(new Paragraph($"Nombre o Razón Social: {data.Nombre}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 20f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 2
            });

            if ((tipoCertificadoVerificacion.Contains(certificado) || certificado == TipoCertificadoUtil.ConstanciasNoAprobado))
            {
                var estado = !string.IsNullOrEmpty(data.Entidad) && data.Entidad.Trim().ToUpper() != "PUEBLA" ? $"Estado: {data.Entidad}" : "";
                tabla.AddCell(new PdfPCell(new Paragraph(estado, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 30f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = tipoCertificadoVerificacion.Contains(certificado) ? 4 : 3
                });
            }
            else
            {
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 30f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = tipoCertificadoVerificacion.Contains(certificado) ? 4 : 3
                });
            }

            // END :: ROW

            tabla.AddCell(new PdfPCell(new Paragraph($"FOLIO ANTERIOR: {FolioAnterior}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });
            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 1f : 20f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });

            doc.Add(tabla);

            var tablaPadre = new PdfPTable(new float[] { 16f, 42f, 20f, 26f }) { WidthPercentage = 100f, SpacingBefore = 0f, SpacingAfter = 0f };
            tablaPadre.DefaultCell.Border = 0;
            tablaPadre.DefaultCell.PaddingLeft = 0f;
            tablaPadre.DefaultCell.PaddingRight = 0f;
            tablaPadre.DefaultCell.PaddingBottom = 0f;
            tabla = new PdfPTable(new float[] { 17f }) { WidthPercentage = 100f, SpacingBefore = 0f, SpacingAfter = 0f };
            tabla.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });
            tabla.AddCell(new PdfPCell(new Paragraph(data.Fecha, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });
            if (tipoCertificadoVerificacion.Contains(certificado) || certificado == TipoCertificadoUtil.ConstanciasNoAprobado)
            {
                tabla.AddCell(new PdfPCell(new Paragraph(data.IdCatTipoTramite != 1 ? "Hora de Inicio" : "", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.HoraInicio, ErrorTableFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph((data.IdCatTipoTramite != 1 ? "Hora Fin" : ""), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.HoraFin, ErrorTableFont))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
            }
            //tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            //{
            //    Border = 0,
            //    HorizontalAlignment = Element.ALIGN_LEFT,
            //    FixedHeight = 1f,
            //    MinimumHeight = 10f,
            //    PaddingTop = 0f,
            //    UseBorderPadding = false,
            //    BorderColor = colorborder,
            //    Colspan = 3,
            //    Rowspan = 4
            //});
            if (!(tipoCertificadoVerificacion.Contains(certificado) || certificado == TipoCertificadoUtil.Exentos))
            {
                tabla.AddCell(new PdfPCell(new Paragraph("Combustible:", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Combustible, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });
            }
            //tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            //{
            //    Border = 0,
            //    HorizontalAlignment = Element.ALIGN_LEFT,
            //    FixedHeight = 1f,
            //    MinimumHeight = 10f,
            //    PaddingTop = 0f,
            //    UseBorderPadding = false,
            //    BorderColor = colorborder,
            //    Colspan = 3,
            //    Rowspan = 4
            //});
            if (tipoCertificadoVerificacion.Contains(certificado) || certificado == TipoCertificadoUtil.ConstanciasNoAprobado)
            {
                tabla.AddCell(new PdfPCell(new Paragraph($"Técnico de Captura: {data.TecnicoCapturaNumero}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3,
                    Rowspan = 4
                });
                tabla.AddCell(new PdfPCell(new Paragraph((data.IdCatTipoTramite != 1 ? "Técnico de Prueba: " : "") + $"{data.TecnicoPruebaNumero}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });
            }

            tablaPadre.AddCell(tabla);

            tabla = new PdfPTable(4) { WidthPercentage = 100f, SpacingAfter = 0f };
            int i = 0;
            if (!string.IsNullOrEmpty(data.AprobadoPor))
            {

                tabla.AddCell(new PdfPCell(new Paragraph(data.AprobadoPor.ToUpper(), BodyTableMinBold))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 4
                });
            }
            foreach (var item in data.Emisiones ?? new List<ImpresionPDFEmisionResponse>())
            {
                tabla.AddCell(new PdfPCell(new Paragraph(item.Nombre, i == 0 ? BodyTableMinBold : BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(item.PrimeraColumna, i == 0 ? BodyTableMinBold : BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(item.SegundaColumna, i == 0 ? BodyTableMinBold : BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                tabla.AddCell(new PdfPCell(new Paragraph(item.TerceraColumna, i == 0 ? BodyTableMinBold : BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
                i++;
            }
            tablaPadre.AddCell(tabla);

            tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingAfter = 0f };
            if (!string.IsNullOrEmpty(data.UrlExpediente))
            {
                var qr = new BarcodeQRCode(data.UrlExpediente, 85, 85, null);
                var imgqr = qr.GetImage();
                tabla.AddCell(new PdfPCell(imgqr)
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
            }
            else
            {
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
            }
            tablaPadre.AddCell(tabla);


            if (tipoCertificadoVerificacion.Contains(certificado))
            {
                tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingAfter = 0f };
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 55f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Centro, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    FixedHeight = 1f,
                    MinimumHeight = 12f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6,
                    //PaddingLeft = 8f
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
                tabla.AddCell(new PdfPCell(new Paragraph($"{data.Placa.ToUpper()}                         { data.VigenciaFecha.ToString("dd/MM/yyy")}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    FixedHeight = 1f,
                    MinimumHeight = 30f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                    FixedHeight = 0f,
                    MinimumHeight = 0f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
                tabla.AddCell(new PdfPCell(new Paragraph($"{data.Combustible}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 4
                });
                tablaPadre.AddCell(tabla);
            }
            else if (certificado == TipoCertificadoUtil.Exentos)
            {
                tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingAfter = 0f };
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 45f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Vigencia, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6,
                    PaddingLeft = 8f
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 4f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Placa.ToUpper(), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6,
                    PaddingLeft = 8f
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 8f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Combustible, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 15f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6,
                    PaddingLeft = 8f
                });
                tablaPadre.AddCell(tabla);
            }
            else if (certificado == TipoCertificadoUtil.Testificacion)
            {
                tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingBefore = 70f };
                tabla.AddCell(new PdfPCell(new Paragraph(data.Placa.ToUpper(), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6,
                    PaddingLeft = 50f
                });
                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 25f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6,
                    PaddingLeft = 50f
                });
                tabla.AddCell(new PdfPCell(new Paragraph(data.Entidad, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Rowspan = 6,
                    PaddingLeft = 50f
                });
                tablaPadre.AddCell(tabla);
            }
            else
            {
                tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingAfter = 0f };
                tablaPadre.AddCell(tabla);
            }

            doc.Add(tablaPadre);

            tabla = new PdfPTable(3) { WidthPercentage = 100f, SpacingBefore = 10f };
            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                FixedHeight = 1f,
                MinimumHeight = certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 1f : certificado == TipoCertificadoUtil.Exentos ? 10f : 15f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 3
            });

            tabla.AddCell(new PdfPCell(new Paragraph($"VERSIÓN SMADSOT: {data.Version}", VersionFont))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = footer ? 10f : certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 20f : tipoCertificadoVerificacion.Contains(certificado) ? 60f : 40f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });
            tabla.AddCell(new PdfPCell(new Paragraph(string.Empty, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = footer ? 10f : certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 20f : tipoCertificadoVerificacion.Contains(certificado) ? 60f : 40f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });
            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                FixedHeight = 1f,
                MinimumHeight = footer ? 10f : certificado == TipoCertificadoUtil.ConstanciasNoAprobado ? 20f : tipoCertificadoVerificacion.Contains(certificado) ? 60f : 40f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });


            // if (certificado == TipoCertificadoUtil.Exentos)
            // {
            //     tabla = new PdfPTable(1) { WidthPercentage = 100f, SpacingAfter = 0f };
            //     tabla.AddCell(new PdfPCell(new Paragraph("", VersionFont))
            //     {
            //         Border = 0,
            //         HorizontalAlignment = Element.ALIGN_RIGHT,
            //         FixedHeight = 1f,
            //         MinimumHeight = 20f,
            //         PaddingTop = 0f,
            //         UseBorderPadding = false,
            //         BorderColor = colorborder,
            //         Colspan = 4
            //     });
            // }
            doc.Add(tabla);
        }
        public static void Certificado(this PdfPTable tabla, int certificado, Font BodyTableMin, BaseColor colorborder, Font BodyTableMinBold, ImpresionPDFResponse data, Font ErrorTableFont)
        {
            var FolioAnterior = !string.IsNullOrEmpty(data.FolioAnterior) ? $"{data.FolioAnterior}" : "-";

            if (certificado == TipoCertificadoUtil.ConstanciasNoAprobado)
            {
                tabla.AddCell(new PdfPCell(new Paragraph("Placa:", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
            }

            tabla.AddCell(new PdfPCell(new Paragraph("Marca", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph("Submarca", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph("Año modelo", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            //data.Folio.ToString("00000000")
            tabla.AddCell(new PdfPCell(new Paragraph((new List<int> { 5, 7 }.Contains(certificado)) ? "Combustible" : "", BodyTableMinBold))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = (certificado == TipoCertificadoUtil.ConstanciasNoAprobado) ? 4 : 5
            });


            if (certificado == TipoCertificadoUtil.ConstanciasNoAprobado)
            {
                tabla.AddCell(new PdfPCell(new Paragraph(data.Placa.ToUpper(), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                });
            }

            tabla.AddCell(new PdfPCell(new Paragraph(data.Marca, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.Submarca, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.Anio, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph((new List<int> { 5, 7 }.Contains(certificado)) ? data.Combustible : "", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = (certificado == TipoCertificadoUtil.ConstanciasNoAprobado) ? 4 : 5
            });

            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMinBold))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8,
                Rowspan = 1
            });

            tabla.AddCell(new PdfPCell(new Paragraph(string.IsNullOrEmpty(data.LeyendaCNA) ? "" : data.LeyendaCNA.ToUpper(), ErrorTableFont))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });

            if (new List<int> { 5, 6, 7 }.Contains(certificado))
            {
                tabla.AddCell(new PdfPCell(new Paragraph($"CENTRO CVV: {data.Centro}", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    FixedHeight = 1f,
                    MinimumHeight = 10f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });
            }

            tabla.AddCell(new PdfPCell(new Paragraph($"NOMBRE O RAZON SOCIAL: {data.Nombre}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });

            tabla.AddCell(new PdfPCell(new Paragraph($"LINEA: {data.Linea}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 2
            });

            tabla.AddCell(new PdfPCell(new Paragraph($"TECNICO DE CAPTURA: {data.TecnicoCapturaNumero}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 6
            });

            tabla.AddCell(new PdfPCell(new Paragraph($"EQUIPO: {data.Equipo}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 2
            });

            tabla.AddCell(new PdfPCell(new Paragraph($"TECNICO DE PRUEBA: {data.TecnicoPruebaNumero}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 6
            });

            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });
            tabla.AddCell(new PdfPCell(new Paragraph($"FOLIO ANTERIOR: {FolioAnterior}", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8
            });

            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8,
                Rowspan = 4
            });

            tabla.AddCell(new PdfPCell(new Paragraph("Fecha", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph("Hora de Inicio", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph("Hora Fin", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 6
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.Fecha, BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.HoraInicio, ErrorTableFont))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
            });

            tabla.AddCell(new PdfPCell(new Paragraph(data.HoraFin, ErrorTableFont))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_LEFT,
                FixedHeight = 1f,
                MinimumHeight = 10f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 6,
            });

            tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                FixedHeight = 1f,
                MinimumHeight = 30f,
                PaddingTop = 0f,
                UseBorderPadding = false,
                BorderColor = colorborder,
                Colspan = 8,
                Rowspan = 8
            });

            if (certificado != 5)
            {

                tabla.AddCell(new PdfPCell(new Paragraph("", BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 5
                });

                tabla.AddCell(new PdfPCell(new Paragraph((certificado == TipoCertificadoUtil.Testificacion) ? data.Placa.ToUpper() : (certificado == TipoCertificadoUtil.Exentos) ? data.Vigencia : data.Centro, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 3
                });

                tabla.AddCell(new PdfPCell(new Paragraph((certificado == TipoCertificadoUtil.Testificacion) ? "Entidad" : data.Placa.ToUpper(), BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });

                tabla.AddCell(new PdfPCell(new Paragraph((certificado == TipoCertificadoUtil.Testificacion) ? "" : data.Combustible, BodyTableMin))
                {
                    Border = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    FixedHeight = 1f,
                    MinimumHeight = 20f,
                    PaddingTop = 0f,
                    UseBorderPadding = false,
                    BorderColor = colorborder,
                    Colspan = 8
                });

            }
        }
    }
}