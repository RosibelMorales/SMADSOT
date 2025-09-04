using Microsoft.AspNetCore.Mvc;
using Smadot.DirectorioCentrosVerificacion.Model.Negocio;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.Modelos;

namespace Smadot.DirectorioCentrosVerificacion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AcreditacionTecnicoVerificadorController : Controller
    {
        private IAcreditacionTecnicoVerificadorNegocio _negocio;

        public AcreditacionTecnicoVerificadorController(IAcreditacionTecnicoVerificadorNegocio negocio)
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

        [HttpGet, Route("GetEvidenciaBySolicitud")]
        public async Task<IActionResult> GetEvidenciaBySolicitud(string solicitud)
        {
            try
            {
                var resultado = await _negocio.GetEvidenciaBySolicitud(solicitud);
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
        public async Task<IActionResult> Post(AcreditacionTecnicoSolicitudRequest request)
        {
            try
            {
                var resultado = await _negocio.RegistroSolicitud(request);
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

        [HttpPost, Route("RegistroEvidencia")]
        public async Task<IActionResult> RegistroEvidencia(AcreditacionTecnicoEvidenciaRequest request)
        {
            try
            {
                var resultado = await _negocio.RegistroEvidencia(request);
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

        [HttpPost, Route("AutorizarAcreditacion")]
        public async Task<IActionResult> AutorizarAcreditacion(AutorizarAcreditacionRequest request)
        {
            try
            {
                var resultado = await _negocio.AutorizarAcreditacion(request);
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

        [HttpGet("Autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] EmpleadoAutocompletRequest request)
        {
            try
            {
                var resultado = await _negocio.Autocomplete(request);
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

        //[HttpGet("{id}/{verificacion}")]
        //public async Task<IActionResult> Get(long id, bool verificacion)
        //{
        //    try
        //    {
        //        var resultado = await _negocio.GetById(id, verificacion);
        //        if (resultado.Status == ResponseStatus.Success)
        //        {
        //            return Ok(resultado);
        //        }
        //        else
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseGeneric<string>(ex.Message));
        //    }
        //}

        //[HttpGet("Autocomplete")]
        //public async Task<IActionResult> Autocomplete([FromQuery] RefrendoAutocompletRequest request)
        //{
        //    try
        //    {
        //        var resultado = await _negocio.Autocomplete(request);
        //        if (resultado.Status == ResponseStatus.Success)
        //        {
        //            return Ok(resultado);
        //        }
        //        else
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseGeneric<string>(ex.Message));
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> Post(RefrendoRequest request)
        //{
        //    try
        //    {
        //        var resultado = await _negocio.Registro(request);
        //        if (resultado.Status == ResponseStatus.Success)
        //        {
        //            return Ok(resultado);
        //        }
        //        else
        //        {
        //            return NoContent();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResponseGeneric<string>(ex.Message));
        //    }
        //}
    }
}
