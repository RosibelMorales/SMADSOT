using Microsoft.AspNetCore.Mvc;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.CargaMasiva.Response;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Utilities.Modelos;

namespace Smadot.DirectorioCentrosVerificacion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DirectorioCentrosVerificacionController : Controller
    {
        private IDirectorioCentrosVerificacionNegocio _negocio;

        public DirectorioCentrosVerificacionController(IDirectorioCentrosVerificacionNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DirectorioCentrosVerificacionListRequest request)
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

        [HttpGet("EditCVV/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var resultado = await _negocio.Edit(id);
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
        [HttpGet, Route("ConfiguradorCitaList")]
        public async Task<IActionResult> FechasCitas([FromQuery] ConfiguradorCitaRequestList request)
        {
            try
            {
                var resultado = await _negocio.GetFechasByIdCVV(request);
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
        [HttpGet, Route("GetConfiguradorByCVV")]
        public async Task<IActionResult> GetConfiguradorByCVV([FromQuery] long id)
        {
            try
            {
                var resultado = await _negocio.GetConfiguradorByCVV(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("ConfiguradorCita")]
        public async Task<IActionResult> ConfiguradorCita([FromBody] List<ConfiguradorCitaDiasRequest> request)
        {
            try
            {
                var resultado = await _negocio.RegistroConfiguradorCita(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    
        [HttpPost, Route("EditFecha")]
        public async Task<IActionResult> EditFechaCita([FromBody] ConfiguradorCitaEditRequest request)
        {
            try
            {
                var resultado = await _negocio.EditFecha(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("RegistroCVV")]
        public async Task<IActionResult> RegistrpCVV([FromBody] NuevoVerificentroResponse request)
        {
            try
            {
                var resultado = await _negocio.RegistroCVV(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("GuardarCarga")]
        public async Task<IActionResult> GuardarCarga([FromBody]DataCarga request)
        {
            try
            {
                var resultado = await _negocio.GuardarCarga(request);
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
