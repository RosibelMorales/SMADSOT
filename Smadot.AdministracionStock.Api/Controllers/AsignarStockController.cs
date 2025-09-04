using Microsoft.AspNetCore.Mvc;
using Smadot.AdministracionStock.Negocio.Negocio;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.AsignacionStock.Request;
using Smadot.Utilities.Modelos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Smadot.AdministracionStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignarStockController : ControllerBase
    {
        private IAsignacionStock _negocio;

        public AsignarStockController(IAsignacionStock negocio)
        {
            _negocio = negocio;
        }

        // GET api/<AsignarStockController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var resultado = await _negocio.Consulta(id);
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
        // GET api/<AsignarStockController>/5
        [HttpGet()]
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

        // POST api/<AsignarStockController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistroStockventanillaRequest request)
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

       
    }
}
