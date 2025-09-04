using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smadot.SolicitudFormaValorada.Model.Negocio;
using Smadot.Utilities.Modelos;
using static Smadot.Models.Entities.FoliosCancelados.Request.FoliosCanceladosRequestData;
using static Smadot.Models.Entities.Reposicion.Request.ReposicionRequestData;

namespace Smadot.SolicitudFormaValorada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ReposicionController : ControllerBase
    {
        private IReposicionNegocio _negocio;

        public ReposicionController(IReposicionNegocio negocio)
        {
            _negocio = negocio;
        }

        [HttpGet]
        public async Task<IActionResult> Consulta([FromQuery] RequestList request)
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

        [HttpPost]
        public async Task<IActionResult> Post(ReposicionApiRequest request)
        {
            try
            {
                var resultado = await _negocio.GuardarReposicion(request);
                return Ok(resultado);
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

        [HttpGet, Route("PlacaSerieAutocomplete")]
        public async Task<IActionResult> Get([FromQuery] string request)
        {
            try
            {
                var resultado = await _negocio.ConsultaAutocomplete(request);
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

        //[HttpGet, Route("ListDataVerificacion")]
        //public async Task<IActionResult> ConsultaListDataVerificacion([FromQuery] string request)
        //{
        //    try
        //    {
        //        var resultado = await _negocio.GetListDataVerificacion(request);
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

        [HttpGet, Route("DataVerificacion")]
        public async Task<IActionResult> GetDataVerificacion([FromQuery] long request)
        {
            try
            {
                var resultado = await _negocio.GetDataVerificacion(request);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var resultado = await _negocio.Eliminar(id);
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
