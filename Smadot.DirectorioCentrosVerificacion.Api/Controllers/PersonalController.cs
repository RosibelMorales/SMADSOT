using Microsoft.AspNetCore.Mvc;
using Smadot.DirectorioCentrosVerificacion.Model.Negocio;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.Personal.Request;
using Smadot.Utilities.Modelos;

namespace Smadot.DirectorioCentrosVerificacion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonalController : ControllerBase
    {
        private IPersonalNegocio _negocio;
        private readonly ILogger<PersonalController> _logger;

        public PersonalController(IPersonalNegocio negocio, ILogger<PersonalController> logger)
        {
            _negocio = negocio;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PersonalListRequest request)
        {
            try
            {
                var resultado = await _negocio.Consulta(request);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PersonalRequest request)
        {
            try
            {
                if (request.IdUsuario > 0)
                {
                    var resultado = await _negocio.Editar(request);
                    _logger.LogInformation("Regresa: {resultado}", resultado);
                    return Ok(resultado);
                }
                else
                {
                    var resultado = await _negocio.Ingreso(request);
                    _logger.LogInformation("Regresa: {resultado}", resultado);
                    return Ok(resultado);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Regresa: {ex}", ex);

                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("ValidarUsuario")]
        public async Task<IActionResult> ValidarUsuario([FromQuery] string curp)
        {
            try
            {

                var resultado = await _negocio.ValidarUsuario(curp);
                _logger.LogInformation("Regresa: {resultado}", resultado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Regresa: {ex}", ex);

                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> ValidarUsuario([FromRoute] long id)
        {
            try
            {
                var resultado = await _negocio.GetById(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("Baja")]
        public async Task<IActionResult> Baja([FromBody] BajaPersonalRequest request)
        {
            try
            {
                var resultado = await _negocio.Baja(request);
                _logger.LogInformation("Regresa: {resultado}", resultado);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Regresa: {ex}", ex);

                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("GetPersonalById")]
        public async Task<IActionResult> GetPersonalById([FromQuery] long id)
        {
            try
            {
                var resultado = await _negocio.GetById(id,true);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPost, Route("Movimiento")]
        public async Task<IActionResult> Movimiento([FromBody] PersonalRequest request)
        {
            try
            {
                var resultado = await _negocio.Movimiento(request);
                _logger.LogInformation("Regresa: {resultado}", resultado);
                return Ok(resultado);

            }
            catch (Exception ex)
            {

                _logger.LogInformation("Regresa: {ex}", ex);

                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("GetByIdPuestoVerificentro")]
        public async Task<IActionResult> GetByIdPuestoVerificentro([FromQuery] long id, bool edit)
        {
            try
            {
                var resultado = await _negocio.GetByIdPuestoVerificentro(id,edit);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpGet, Route("GetPuestos")]
        public async Task<IActionResult> GetPuestos([FromQuery] long id)
        {
            try
            {
                var resultado = await _negocio.GetPuestos(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }

        [HttpPut, Route("EstatusPuesto")]
        public async Task<IActionResult> ModificarEstatusPuesto([FromBody] EstatusPuestoPersonalRequest request)
        {
            try
            {
                var resultado = await _negocio.ModificarEstatusPuesto(request);
                _logger.LogInformation("Regresa: {resultado}", resultado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Regresa: {ex}", ex);
                return BadRequest(new ResponseGeneric<string>(ex.Message));
            }
        }
    }
}
