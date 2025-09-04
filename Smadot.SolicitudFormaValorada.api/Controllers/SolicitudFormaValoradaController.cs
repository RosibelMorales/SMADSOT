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
using Smadot.Utilities.Modelos;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SolicitudFormaValoradaController : Controller
    {
        private ISolicitudFormaValoradaNegocio _negocio;

        public SolicitudFormaValoradaController(ISolicitudFormaValoradaNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SolicitudFormaValoradaListRequest request)
        {
            try
            {
                var resultado = await _negocio.Consulta(request);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var resultado = await _negocio.GetById(id);
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

        [HttpPost]
        public async Task<IActionResult> Post(SolicitudFormaValoradaRequest request)
        {
            try
            {
                var resultado = await _negocio.Registro(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        #region FOLIOS
        [HttpGet, Route("FoliosUsadosVentanilla")]
        public async Task<IActionResult> Get([FromQuery] FoliosUsadosEnVentanillaRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaFoliosUsadosVentanilla(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("FoliosUsadosVentanilla/{id}")]
        public async Task<IActionResult> GetDetalleFoliosVentanilla(long id)
        {
            try
            {
                var resultado = await _negocio.DetalleFoliosVentanilla(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("FoliosRegresadosSPF")]
        public async Task<IActionResult> Get([FromQuery] FoliosRegresadosSPFRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaFoliosRegresadosSPF(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("FoliosRegresadosSPF/{id}")]
        public async Task<IActionResult> GetDetalleFolioRegresadoSPF(long id)
        {
            try
            {
                var resultado = await _negocio.DetalleFolioRegresadoSPF(id);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("FoliosVendidosCentrosVerificacion")]
        public async Task<IActionResult> Get([FromQuery] FoliosVendidosCentrosVerificacionRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaFoliosVendidosCentrosVerificacion(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("FoliosVendidosCentrosVerificacion/{id}")]
        public async Task<IActionResult> GetFoliosVendidosCentrosVerificacion(long id)
        {
            try
            {
                var resultado = await _negocio.DetalleFoliosVendidosCentrosVerificacion(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        #endregion



        [HttpGet, Route("ConsultaStockDVRF")]
        public async Task<IActionResult> Get([FromQuery] ConsultaStockDVRFRequest request)
        {
            try
            {
                var resultado = await _negocio.InformeConsultaStockDVRF(request);
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

        [HttpGet("ConsultaStockDVRF/{IdAlmacen}")]
        public async Task<IActionResult> GetCatalogo(long IdAlmacen)
        {
            try
            {
                var resultado = await _negocio.GetByIdDVRF(IdAlmacen);
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

        [HttpGet("StockMinimo")]
        public async Task<IActionResult> GetStockMinimo([FromQuery] StockMinimoRequest request)
        {
            try
            {
                var resultado = await _negocio.ConsultaStockMinimo(request);
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
        [HttpPost, Route("StockMinimo")]
        public async Task<IActionResult> Post([FromBody] List<StockMinimoResponse> request)
        {
            try
            {
                var resultado = await _negocio.RegistroStockMinimo(request);
                return Ok(resultado);
                // if (resultado.Status == ResponseStatus.Success)
                // {
                // }
                // else
                // {
                //     return NoContent();
                // }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }



    }
}
