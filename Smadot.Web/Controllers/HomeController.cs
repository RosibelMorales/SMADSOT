using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smadot.FrontEnd.Models;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Web.Handler.Autorizacion;
using System.Diagnostics;
using AuthorizeAttribute = Smadot.Web.Handler.Autorizacion.AuthorizeAttribute;

namespace Smadot.FrontEnd.Controllers
{

    public class HomeController : Controller
    {
        #region Propiedades

        private readonly IUserResolver _userResolver;
        private readonly IConfiguration _configuration;

        private readonly BlobStorage _blobStorage;
        #endregion

        #region Constructor

        public HomeController(IUserResolver userResolver, IConfiguration configuration)
        {
            _userResolver = userResolver;

            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _configuration = configuration;
        }
        #endregion

        [Route("Inicio")]
        [Authorize]

        public async Task<IActionResult> Index(bool updateContrasenia = false)
        {
            var usuarioActual = _userResolver.GetUser();
            ViewBag.UpdateContrasenia = updateContrasenia;
            return View(usuarioActual);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/FilesStorage")]
        [Authorize]
        public async Task<IActionResult> GetImagen(string url)
        {
            try
            {
                return await _blobStorage.DownloadFileStream(url);
            }
            catch (Exception)
            {

                return NotFound("No se encontró el archivo.");
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetImageQR(string ImageAsBase64)
        {
            try
            {
                ImageAsBase64 = ImageAsBase64.Replace(".png", "");
                // Decodificar el base64 en un array de bytes
                byte[] imageBytes = Convert.FromBase64String(ImageAsBase64);

                // Crear un MemoryStream con los bytes de la imagen
                MemoryStream memoryStream = new MemoryStream(imageBytes);

                    // Devolver la imagen como un FileStreamResult
                return new FileStreamResult(memoryStream, "image/png");

            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción y devolver una respuesta de error
                return Content($"Ocurrio un error al generar el código QR: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(ErrorViewModel vm)
        {
            return View(vm);
        }
    }
}