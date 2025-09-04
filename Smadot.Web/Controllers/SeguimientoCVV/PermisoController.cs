using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Permiso.Request;
using Smadot.Models.Entities.Permiso.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Handler.Autorizacion;
using Smadot.Web.Helper.Operaciones.SeguimientoCVV;
using Smadot.Web.Models;
using Smadot.Web.Utilities;

namespace Smadot.Web.Controllers.SeguimientoCVV
{
    [Authorize]
	public class PermisoController : Controller
	{
        private readonly IProxyWebAPI _proxyWebAPI;
        private readonly IUserResolver _userResolver;
        public PermisoController(IProxyWebAPI proxyWebAPI, IUserResolver userResolver)
        {
            _proxyWebAPI = proxyWebAPI;
            _userResolver = userResolver;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userResolver.HasPermission(MenuPermisos.PermisoMenuPadre))
                return new UnauthorizedResult();
            var response = await PermisoHelper.GetRoles(_proxyWebAPI);
            ViewBag.Roles = (response.Result as List<PermisoTreeRolesRequest> ?? new List<PermisoTreeRolesRequest>()).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PermisosTree(int id)
        {
            var res = new JsonResultDto();
            var response = await PermisoHelper.GetPermisosTreeByRol(_proxyWebAPI, id);
            var tree = response.Result as List<PermisoTreeResponse> ?? new List<PermisoTreeResponse>();
            if (tree.Count == 0)
            {
                res.Error = true;
                res.ErrorDescription = "No sé encontró información.";
            }
            else
            {
                res.Error = false;
                res.Result = await this.RenderViewToStringAsync("_PermisosTree", tree);
            }
            return Json(res);
        }

        [HttpPost]
        public async Task<ActionResult> SavePermisos(PermisoTreeRequest req)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var save = await PermisoHelper.SavePermisosByRol(_proxyWebAPI, req);
                result.Result = save.Result;
                result.IsSuccessFully = save.IsSuccessFully;
                result.Message = save.Message;
            }
            catch (Exception e)
            {
                result.IsSuccessFully = false;
                result.Message = e.Message;
            }
            return new JsonResult(result);
        }
    }
}
