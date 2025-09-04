using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.StockCertificado.Response;
using Smadot.Models.Entities.StockCertificado.Request;
using Smadot.Models.Entities.StockMinimo.Response;
using Smadot.Web.Utilities;
using Smadot.Utilities.Modelos;
using Smadot.Web.Helper.Operaciones.Catalogo;
using Smadot.Web.Helper.Operaciones.SolicitudFormaValorada;
using Smadot.Models.Entities.Catalogos.Request;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Utilities.ProxyWebAPI.Interfaces;
using Smadot.Web.Models.StockMinimoModel;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.GestionTokens;
using Smadot.Web.Models.StockCertificado;

namespace Smadot.Web.Controllers.SolicitudFormaValorada
{
    public class StockCertificadoController : Controller
    {
        private readonly IProxyWebAPI _proxyWebAPI;

        public string _mensajeErrorCatch = "Ocurrió un error al realizar la operación";

        public StockCertificadoController(IProxyWebAPI proxyWebAPI)
        {
            _proxyWebAPI = proxyWebAPI;
        }

    }
}
