using Microsoft.AspNetCore.Mvc;
using Smadot.Utilities.GestionTokens;
using Smadot.Web.Models;

namespace Smadot.Web.Controllers.Camara
{
    public class CamaraController : Controller
    {
        private readonly IUserResolver _userResolver;

        public CamaraController(IUserResolver userResolver)
        {
            _userResolver = userResolver;
        }

        [HttpGet]
        public ActionResult ScreenShot()
        {
            var result = new JsonResultDto();

            if (_userResolver.GetUser().IdUser > 0)
            {
                result.Result = new object();
            }
            else
            {
                return RedirectToAction("Index", "Autenticacion");
            }
            return Json(result);
        }

        [HttpGet]
        public ActionResult ZoomUp()
        {
            var result = new JsonResultDto();

            if (_userResolver.GetUser().IdUser > 0)
            {
                result.Result = new object();
            }
            else
            {
                return RedirectToAction("Index", "Autenticacion");
            }
            return Json(result);
        }

        [HttpGet]
        public ActionResult ZoomDown()
        {
            var result = new JsonResultDto();

            if (_userResolver.GetUser().IdUser > 0)
            {
                result.Result = new object();
            }
            else
            {
                return RedirectToAction("Index", "Autenticacion");
            }
            return Json(result);
        }
    }
}
