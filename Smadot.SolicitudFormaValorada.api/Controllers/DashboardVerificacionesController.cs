using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Entities.ConsultaSock.Request;
using Smadot.Models.Entities.EstadisticasUsoFormaValorada.Request;
using Smadot.Models.Entities.FoliosRegresadosSPF.Request;
using Smadot.Models.Entities.FoliosUsadosEnVentanilla.Request;
using Smadot.Models.Entities.FoliosVendidosCentrosVerificacion.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Models.Entities.StockCertificado.Request;
using Smadot.Models.Entities.StockCertificado.Response;
using Smadot.Models.Entities.StockMinimo.Request;
using Smadot.Models.Entities.StockMinimo.Response;
using Smadot.SolicitudFormaValorada.model.Negocio;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DashboardVerificacionesController : Controller
    {
        private readonly IDashboardNegocio _negocio;

        public DashboardVerificacionesController(IDashboardNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet, Route("ConsultaEstadisticaUsoFormaValorada")]
        public async Task<IActionResult> ConsultaEstadisticaUsoFormaValorada([FromQuery] EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                var resultado = await _negocio.EstadisticasDashboard(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
        [HttpGet, Route("ConsultaEstadisticasDashboard")]
        public async Task<IActionResult> ConsultaEstadisticasDashboard([FromQuery] EstadisticaUsoFormaValoradaGridRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaEstadisticasDashboard(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
        [HttpGet, Route("GetConteoCertificados")]
        public async Task<IActionResult> GetConteoCertificados([FromQuery] EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                var resultado = await _negocio.GetConteoFolios(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("ConsultaGraficaBarras")]
        public async Task<IActionResult> ConsultaGraficaBarras([FromQuery] EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                var resultado = await _negocio.EstadisticasDashboardGraphBar(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
        [HttpGet, Route("ConsultaGraficaPie")]
        public async Task<IActionResult> ConsultaGraficaPie([FromQuery] EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                var resultado = await _negocio.EstadisticasDashboardGraphPie(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
        [HttpGet, Route("ConsultaGraficaGauge")]
        public async Task<IActionResult> ConsultaGraficaGauge([FromQuery] EstadisticaUsoFormaValoradaRequest request)
        {
            try
            {
                var resultado = await _negocio.EstadisticasDashboardGraphGauge(request);
                if (resultado.Status == ResponseStatus.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }


    }
}
