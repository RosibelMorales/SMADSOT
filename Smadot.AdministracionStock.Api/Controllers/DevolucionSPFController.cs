using Microsoft.AspNetCore.Mvc;
using Smadot.AdministracionStock.Negocio.Negocio;
using Smadot.Models.Entities.DevolucionSPF.Request;
using Smadot.Utilities.Modelos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Smadot.AdministracionStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevolucionSPFController : ControllerBase
    {

        private IDevolucionSPFNegocio _negocio;

        public DevolucionSPFController(IDevolucionSPFNegocio negocio)
        {
            _negocio = negocio;
        }
       
        // GET api/<DevolucionSPFController>/5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RequestList request)
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

        // POST api/<DevolucionSPFController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DevolucionSPFRequest request)
        {
            try
            {
                var resultado = await _negocio.Guardar(request);
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

        // PUT api/<DevolucionSPFController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DevolucionSPFController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet, Route("Autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string prefix)
        {
            try
            {
                var resultado = await _negocio.Autocomplete(prefix);
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

        [HttpGet, Route("AutocompleteResponsable")]
        public async Task<IActionResult> AutocompleteResponsable([FromQuery] string prefix)
        {
            try
            {
                var resultado = await _negocio.AutocompleteResponsable(prefix);
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
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var resultado = await _negocio.ConsultaById(id);
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

		[HttpGet, Route("GetInventarioByAlmacen")]
		public async Task<IActionResult> GetInventarioByAlmacen([FromQuery] long id)
		{
			try
			{
				var resultado = await _negocio.GetInventarioByAlmacen(id);
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

		[HttpGet, Route("GetNumeroDevolucion")]
		public async Task<IActionResult> GetNumeroDevolucion()
		{
			try
			{
				var resultado = await _negocio.GetNumeroDevolucion();
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

        [HttpGet, Route("GetDoc")]
        public async Task<IActionResult> GetDoc([FromQuery] RequestList request)
        {
            try
            {
                var resultado = await _negocio.GetDevolucionSPFDocumento(request);
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
