using Microsoft.AspNetCore.Mvc;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.DashboardLineas.Response;
using Smadot.Models.Entities.SeguimientoCVV.Request;
using Smadot.Models.Entities.Verificacion.Request;
using Smadot.Models.Entities.Verificacion.Response;
using Smadot.Venta.Model.Negocio;
using Smadot.Utilities.CustomAttributes;
using Smadot.Utilities.Modelos;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Smadot.Models.GenericProcess;
using Namespace;
using Smadot.Models.Dicts.ProveedorDicts;

namespace Smadot.Venta.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificacionController : ControllerBase
    {

        private IVerificacionNegocio _negocio;
        private readonly string token;
        private readonly Smadsot.Api.VentaApi ventaApi;
        private readonly List<long> exludedCvvs = new() { 27, 34, 41 };

        public VerificacionController(IVerificacionNegocio negocio, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _negocio = negocio;
            token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Add("Smadot-API-Key", configuration["SmadotAPISettings:APIKey"]);
            ventaApi = new(configuration["VentaApiUrl"], httpClient);
        }
        // GET: api/<VerificacionController>
        [HttpGet("InfoLineas")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _negocio.ObtnerInformacionLineas();
                if (response.Status == ResponseStatus.Success)
                {
                    return Ok(response);
                }
                return BadRequest(response);

            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpGet("BuscarVin")]
        public async Task<IActionResult> GetVins([FromQuery] VerificacionCitaRequest request)
        {
            try
            {
                var response = await _negocio.GetvVerificacionCitas(request);

                return Ok(response);


            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }


        [HttpPost("SavePlacas")]
        [ApiKey]
        public async Task<IActionResult> Post([FromBody] VerificacionRequest request)
        {
            try
            {
                //using (var webClient = new WebClient())
                //{
                //request.ImgPlacaDelantera = webClient.DownloadData("https://cherry-brightspot.s3.amazonaws.com/f7/6a/d2dc8c124361883a407bc7bc0e4d/cambio-placas-1.jpg-1%20(1).jpg");
                var processImage = await _negocio.SavePlacas(request);
                if (processImage.Status == ResponseStatus.Success)
                {
                    return Ok(processImage);
                }

                return UnprocessableEntity(processImage);
                //}
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [HttpPost("PruebaVisual/Diesel")]
        // [ApiKey]
        [ProducesResponseType(typeof(ResponseGeneric<ConsultaVerificacionResponse<object>?>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseGeneric<ConsultaVerificacionResponse<object>?>), (int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> Post([FromBody] PruebaVisualRequest<ResultadosDiesel> request, [FromHeader] bool sinRedireccion)
        {
            try
            {
                if (exludedCvvs.Contains(request.IdVerificentro) && !sinRedireccion)
                {
                    var requestApi = new Smadsot.Api.ResultadosDieselPruebaVisualRequest
                    {
                        IdVerificacion = request.IdVerificacion,
                        IdVerificentro = request.IdVerificentro,
                        NumeroEscapes = request.NumeroEscapes,
                        FotoEvidencia = request.FotoEvidencia,
                        Resultados = new()
                        {
                            TipoCombustible = request.Resultados.TipoCombustible,
                            Resultados = new()
                            {
                                GobernadorBuenEstado = request.Resultados.Resultados.GobernadorBuenEstado,
                                SistemaEscape = request.Resultados.Resultados.SistemaEscape
                            },
                        }
                    };
                    var callApi = await ventaApi.DieselAsync(true, requestApi);
                    var resApi = new ResponseGeneric<ConsultaVerificacionResponse<object>>()
                    {
                        Status = callApi.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                        CurrentException = callApi.CurrentException,
                        mensaje = callApi.Mensaje,
                        respuesta = callApi.Respuesta,
                        AccesoTotalVerificentros = callApi.AccesoTotalVerificentros,
                        Response = new()
                        {
                            IdEstatusPrueba = callApi.Response.IdEstatusPrueba,
                            Resultados = callApi.Response.Resultados,
                        },
                    };

                    if (resApi.Status == ResponseStatus.Success)
                    {
                        return Ok(resApi);
                    }

                    return UnprocessableEntity(resApi);
                }

                var processImage = await _negocio.PruebaVisualDiesel(request);

                if (processImage.Status == ResponseStatus.Success)
                {
                    return Ok(processImage);
                }

                return UnprocessableEntity(processImage);
            }
            catch (Smadsot.Api.ApiException<Smadsot.Api.ObjectConsultaVerificacionResponseResponseGeneric> e)
            {

                var resApi = new ResponseGeneric<ConsultaVerificacionResponse<object>>()
                {
                    Status = e.Result.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                    CurrentException = e.Result.CurrentException,
                    mensaje = e.Result.Mensaje,
                    respuesta = e.Result.Respuesta,
                    AccesoTotalVerificentros = e.Result.AccesoTotalVerificentros,
                    Response = new()
                    {
                        IdEstatusPrueba = e.Result.Response.IdEstatusPrueba,
                        Resultados = e.Result.Response.Resultados,
                    },
                };

                return UnprocessableEntity(resApi);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [HttpPost("PruebaVisual/Gasolina")]
        [ApiKey]
        [ProducesResponseType(typeof(ResponseGeneric<ConsultaVerificacionResponse<object>?>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseGeneric<ConsultaVerificacionResponse<object>?>), (int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> Post([FromBody] PruebaVisualRequest<ResultadosGasolina> request, [FromHeader] bool sinRedireccion)
        {
            try
            {
                if (exludedCvvs.Contains(request.IdVerificentro) && !sinRedireccion)
                {
                    var requestApi = new Smadsot.Api.ResultadosGasolinaPruebaVisualRequest
                    {
                        IdVerificacion = request.IdVerificacion,
                        IdVerificentro = request.IdVerificentro,
                        NumeroEscapes = request.NumeroEscapes,
                        FotoEvidencia = request.FotoEvidencia,
                        Resultados = new()
                        {
                            TipoCombustible = request.Resultados.TipoCombustible,
                            Resultados = new()
                            {
                                SistemaEscape = request.Resultados.Resultados.SistemaEscape,
                                PortafiltroAire = request.Resultados.Resultados.PortafiltroAire,
                                TaponDispositivoAceite = request.Resultados.Resultados.TaponDispositivoAceite,
                                TaponCombustible = request.Resultados.Resultados.TaponCombustible,
                                Bayoneta = request.Resultados.Resultados.Bayoneta,
                                NeumaticosBuenEstato = request.Resultados.Resultados.NeumaticosBuenEstato,
                                ComponenteControlEmisiones = request.Resultados.Resultados.ComponenteControlEmisiones,
                                FugaFluidos = request.Resultados.Resultados.FugaFluidos,
                            },
                        }
                    };
                    var callApi = await ventaApi.GasolinaAsync(true, requestApi);
                    var resApi = new ResponseGeneric<ConsultaVerificacionResponse<object>>()
                    {
                        Status = callApi.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                        CurrentException = callApi.CurrentException,
                        mensaje = callApi.Mensaje,
                        respuesta = callApi.Respuesta,
                        AccesoTotalVerificentros = callApi.AccesoTotalVerificentros,
                        Response = new()
                        {
                            IdEstatusPrueba = callApi.Response.IdEstatusPrueba,
                            Resultados = callApi.Response.Resultados,
                        },
                    };
                    if (resApi.Status == ResponseStatus.Success)
                    {
                        return Ok(resApi);
                    }

                    return UnprocessableEntity(resApi);
                }
                var processImage = await _negocio.PruebaVisualGasolina(request);
                if (processImage.Status == ResponseStatus.Success)
                {
                    return Ok(processImage);
                }

                return UnprocessableEntity(processImage);
            }
            catch (Smadsot.Api.ApiException<Smadsot.Api.ObjectConsultaVerificacionResponseResponseGeneric> e)
            {
                var resApi = new ResponseGeneric<ConsultaVerificacionResponse<object>>()
                {
                    Status = e.Result.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                    CurrentException = e.Result.CurrentException,
                    mensaje = e.Result.Mensaje,
                    respuesta = e.Result.Respuesta,
                    AccesoTotalVerificentros = e.Result.AccesoTotalVerificentros,
                    Response = new()
                    {
                        IdEstatusPrueba = e.Result.Response.IdEstatusPrueba,
                        Resultados = e.Result.Response.Resultados,
                    },
                };

                return UnprocessableEntity(resApi);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        [HttpPost("ConsultaPruebaVisual")]
        [ApiKey]
        public IActionResult Post([FromBody] ConsultaVerificacionRequest request)
        {
            try
            {

                var processImage = _negocio.ConsultaPruebaVisual(request);
                if (processImage.Status == ResponseStatus.Success)
                {
                    return Ok(processImage);
                }

                return UnprocessableEntity(processImage);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }
        // [HttpPost("ConsultaVerificacion")]
        // [ApiKey]
        // public async Task<IActionResult> ConsultaPrueba([FromBody] ConsultaVerificacionRequest request)
        // {
        //     try
        //     {
        //         //var response = new ResponseGeneric<ConsultaVerificacionResponse<object>>();
        //         switch (request.IdTipoPrueba)
        //         {
        //             case EstatusVerificacion.TerminaPruebaVisual:
        //                 var response4 = _negocio.ConsultaPruebaVisual(request);
        //                 if (response4.Status == ResponseStatus.Success)
        //                 {
        //                     return Ok(response4);
        //                 }

        //                 return UnprocessableEntity(response4);
        //                 break;
        //             case EstatusVerificacion.EnPruebaOBD:
        //                 var response = _negocio.ConsultaVerificacionOBD(request);
        //                 if (response.Status == ResponseStatus.Success)
        //                 {
        //                     return Ok(response);
        //                 }

        //                 return UnprocessableEntity(response);
        //                 break;
        //             case EstatusVerificacion.EnPruebaEstaticaDinamica:
        //                 var response2 = _negocio.ConsultaVerificacionEmisiones(request);
        //                 if (response2.Status == ResponseStatus.Success)
        //                 {
        //                     return Ok(response2);
        //                 }

        //                 return UnprocessableEntity(response2);
        //                 break;
        //             case EstatusVerificacion.EnPruebaOpacidad:
        //                 var response3 = _negocio.ConsultaVerificacionOpacidad(request);
        //                 if (response3.Status == ResponseStatus.Success)
        //                 {
        //                     return Ok(response3);
        //                 }

        //                 return UnprocessableEntity(response3);
        //                 break;
        //         }
        //         return NotFound();
        //     }
        //     catch (Exception e)
        //     {

        //         return BadRequest(new ResponseGeneric<string>(e.Message));
        //     }

        // }
        [HttpPost("AbortarPrueba")]
        [ApiKey]
        [ProducesResponseType(typeof(ResponseGeneric<ConsultaVerificacionResponse<long>?>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ResponseGeneric<ConsultaVerificacionResponse<long>?>), (int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> AbortarPrueba([FromBody] ConsultaVerificacionRequest request, [FromHeader] bool sinRedireccion)
        {
            try
            {
                if (exludedCvvs.Contains(request.IdVerificentro) && !sinRedireccion)
                {
                    var requestApi = new Smadsot.Api.ConsultaVerificacionRequest
                    {
                        IdTipoPrueba = request.IdTipoPrueba,
                        IdVerificacion = request.IdVerificacion,
                        IdUserTecnico = request.IdUserTecnico,
                        IdVerificentro = request.IdVerificentro,
                        ClaveRechazo = request.ClaveRechazo,
                    };
                    var callApi = await ventaApi.AbortarPruebaAsync(true, requestApi);
                    var resApi = new ResponseGeneric<ConsultaVerificacionResponse<long>>()
                    {
                        Status = callApi.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                        CurrentException = callApi.CurrentException,
                        mensaje = callApi.Mensaje,
                        respuesta = callApi.Respuesta,
                        AccesoTotalVerificentros = callApi.AccesoTotalVerificentros,
                        Response = new()
                        {
                            IdEstatusPrueba = callApi.Response.IdEstatusPrueba,
                            Resultados = callApi.Response.Resultados,
                        },
                    };
                    if (resApi.Status == ResponseStatus.Success)
                    {
                        return Ok(resApi);
                    }

                    return UnprocessableEntity(resApi);
                }

                var reponse = await _negocio.AbortarPrueba(request);
                if (reponse.Status == ResponseStatus.Success)
                {
                    return Ok(reponse);
                }

                return UnprocessableEntity(reponse);
            }
            catch (Smadsot.Api.ApiException<Smadsot.Api.Int64ConsultaVerificacionResponseResponseGeneric> e)
            {
                var resApi = new ResponseGeneric<ConsultaVerificacionResponse<long>>()
                {
                    Status = e.Result.Status == Smadsot.Api.ResponseStatus._0 ? ResponseStatus.Failed : ResponseStatus.Success,
                    CurrentException = e.Result.CurrentException,
                    mensaje = e.Result.Mensaje,
                    respuesta = e.Result.Respuesta,
                    AccesoTotalVerificentros = e.Result.AccesoTotalVerificentros,
                    Response = new()
                    {
                        IdEstatusPrueba = e.Result.Response.IdEstatusPrueba,
                        Resultados = e.Result.Response.Resultados,
                    },
                };

                return UnprocessableEntity(resApi);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }

        [HttpGet("Expediente/{id}")]
        public async Task<IActionResult> Expediente(long id)
        {
            try
            {

                var reponse = await _negocio.GetExpediente(id);
                if (reponse.Status == ResponseStatus.Success)
                {
                    return Ok(reponse);
                }

                return UnprocessableEntity(reponse);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }

        [Authorize]
        [HttpGet("ExpedienteTest")]
        public async Task<IActionResult> Expediente()
        {
            try
            {
                var claims = User.Claims.ToList();
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                // Obtener el valor del claim 'exp' que representa el tiempo de expiración en segundos
                var expirationTimeInSeconds = jsonToken?.ValidTo;

                if (expirationTimeInSeconds.HasValue)
                {
                    // expirationTime representa la fecha y hora en la que el token expirará
                    var expirationTime = jsonToken.ValidTo;

                    // Comparar con la hora actual
                    if (expirationTime > DateTime.Now)
                    {
                        Console.WriteLine("El token aún no ha expirado.");
                    }
                    else
                    {
                        Console.WriteLine("El token ha expirado.");
                    }
                }
                else
                {
                    Console.WriteLine("No se pudo obtener el tiempo de expiración del token.");
                }
                var reponse = await _negocio.GetExpediente(1);
                if (reponse.Status == ResponseStatus.Success)
                {
                    return Ok(reponse);
                }

                return UnprocessableEntity(reponse);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }

        }

        [HttpGet("PruebaFinalizada/{id}")]
        public async Task<IActionResult> PruebaFinalizada(long id)
        {
            try
            {
                var reponse = await _negocio.PruebaFinalizada(id);
                if (reponse.Status == ResponseStatus.Success)
                {
                    return Ok(reponse);
                }

                return UnprocessableEntity(reponse);
            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }
        }
        [HttpGet("InsertPruebaTest/{id}")]
        public async Task<IActionResult> InsertPruebaTest(long id)
        {
            try
            {
                _negocio.InsertarPruebaTest(id);

                return Ok();

            }
            catch (Exception e)
            {

                return BadRequest(new ResponseGeneric<string>(e.Message));
            }
        }

        [HttpPut("VolverImprimir")]
        public async Task<IActionResult> VolverImprimir([FromBody] long id)
        {
            try
            {
                var resultado = await _negocio.VolverImprimir(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<bool>(ex));
            }
        }
        [HttpGet("ListaMotivosAbortar")]
        public async Task<IActionResult> GetListaMotivosAbortar()
        {
            try
            {
                var resultado = CausaRechazo.DictPruebaAbortadaApp;
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseGeneric<bool>(ex));
            }
        }

        // PUT api/<VerificacionController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<VerificacionController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
