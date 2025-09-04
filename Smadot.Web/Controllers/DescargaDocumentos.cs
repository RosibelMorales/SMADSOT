using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.Modelos;
using Smadot.Web.Helper.Operaciones.Tramite;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Models.Entities.ConstanciaUltimaVerificacion.Response;
using Smadot.Models.DataBase;

namespace Smadot.Web.Controllers
{
    public class DescargaDocumentos : Controller
    {
        private readonly BlobStorage _blobStorage;
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IPdfBuider _pdfBuider;

        public DescargaDocumentos(IConfiguration configuration, IProxyWebAPI proxyWebAPI, IPdfBuider pdfBuider)
        {
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _proxyWebAPI = proxyWebAPI;
            _pdfBuider = pdfBuider;
        }
        [HttpGet]
        public async Task<IActionResult> Index(long id, string url = "")
        {
            var result = new ResponseViewModel(true);
            try
            {
                //var urlSplit = url.Split("/");
                //var nombreArchivo = urlSplit[urlSplit.Length - 1];
                //url = _blobStorage._url + "/ConstanciaUltimaVerificacion/" + id + "/Certificado.pdf";
                //var archivo = await _blobStorage.DownloadFileAsync(url, true);
                //if (!archivo.IsSuccessFully)
                //{
                //    result.IsSuccessFully = false;
                //    throw new Exception("No sé encontró el documento.");
                //}
                //result.Result = new
                //{
                //    FileName = nombreArchivo,
                //    Base64 = archivo.Result,
                //    //Ext = nombreArchivo.Split('.')[1]
                //};
                //ViewBag.Base64 = archivo.Result;
                string stringHtml = string.Empty;
                var dataReport = new DataReport();
                var vm = new ConstanciaUltimaVerificacionResponse();

                var response = await ConstanciaUltimaVerificacionHelper.Consulta(_proxyWebAPI, id, false);
                var res = response.Result as ConstanciaUltimaVerificacionGridResponse ?? new ConstanciaUltimaVerificacionGridResponse();
                vm = JsonConvert.DeserializeObject<ConstanciaUltimaVerificacionResponse>(JsonConvert.SerializeObject(response.Result));
                vm.UrlRoot = new Uri($"{Request.Scheme}://{Request.Host}").ToString();
                vm.NombreC = res.NombreRazonSocial;
                vm.NombreEncargado = res.DirectorGestionCalidadAire;
                vm.Telefono = res.Telefono;
                vm.Direccion = res.Direccion;
                vm.Folio = res.Folio;
                var getdoc = await _pdfBuider.GetConstanciaUltimaVerificacion(vm);

                var doc = getdoc.Response.DocumentoPDF;
                var nomb = getdoc.Response.NombreDocumento;

                dataReport.NombreDocumento = nomb;
                dataReport.DocumentoPDF = doc;

                var pdf = dataReport;

                // var stream = new MemoryStream(pdf.DocumentoPDF);
                // var contentType = @"application/pdf";
                // var fileName = pdf.NombreDocumento;

                // var doc1 = File(stream, contentType, fileName);

                ViewBag.Base64 = Convert.ToBase64String(doc);
            }
            catch (Exception ex)
            {
                if (result.IsSuccessFully)
                    result.Message = "Error al descargar el documento.";
                else
                    result.Message = ex.Message;
                result.IsSuccessFully = false;
                ViewBag.ErrorMessage = result.Message;
            }
            return View();
        }
    }
}
