using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using System.Linq.Dynamic.Core;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Models.Entities.SolicitudFormaValorada.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Transactions;
using System.ComponentModel.DataAnnotations;
using Smadot.Models.Entities.Generic.Response;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Smadot.Utilities.Reporting.Interfaces;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.BlobStorage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Azure.Core;
using Smadot.Models.Entities.CargaMasiva.Response;
using Smadot.Models.GenericProcess;
using Smadot.Models.Dicts.ProveedorDicts;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
    public class DirectorioCentrosVerificacionNegocio : IDirectorioCentrosVerificacionNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly IExcelBuilder _excelBuilder;
        private readonly IConfiguration _configuration;
        private readonly BlobStorage _blobStorage;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        public DirectorioCentrosVerificacionNegocio(SmadotDbContext context, IUserResolver userResolver, IExcelBuilder excelBuilder, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _excelBuilder = excelBuilder;
            _configuration = configuration;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task<ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>> Consulta(DirectorioCentrosVerificacionListRequest request)
        {
            try
            {

                var solicitudes = _context.vVerificentros.AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {

                    solicitudes = solicitudes.Where(x => x.Nombre.ToLower().Contains(request.Busqueda.ToLower()) || x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Clave.ToLower().Contains(request.Busqueda.ToLower()) || x.Rfc.ToLower().Contains(request.Busqueda.ToLower()) || x.Telefono.ToLower().Contains(request.Busqueda.ToLower()) || x.Correo.ToLower().Contains(request.Busqueda.ToLower()) || x.RepresentanteLegal.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var aux = "https://www.google.com/maps?q=";
                var tot = solicitudes.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    solicitudes = solicitudes.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    solicitudes = solicitudes.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                DateTime now = DateTime.Now;
                var result = await solicitudes.Select(x => new DirectorioCentrosVerificacionResponse
                {
                    Id = (int)x.Id,
                    Nombre = x.NombreCorto,
                    Activo = x.Activo,
                    Clave = x.Clave,
                    Direccion = x.Direccion,
                    Rfc = x.Rfc,
                    Telefono = x.Telefono,
                    Correo = x.Correo,
                    GerenteTecnico = x.GerenteTecnico,
                    RepresentanteLegal = x.RepresentanteLegal,
                    Longitud = (float?)x.Longitud,
                    Latitud = (float?)x.Latitud,
                    Link = aux,
                    Total = tot
                }).ToListAsync();
                return new ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>> GetById(long Id)
        {
            try
            {
                var result = new List<DirectorioCentrosVerificacionResponse>();
                if (Id > 0)
                {
                    var solicitud = _context.vVerificentros.Where(x => x.Id == Id);
                    if (solicitud != null)
                    {
                        string r = JsonConvert.SerializeObject(solicitud);
                        result = JsonConvert.DeserializeObject<List<DirectorioCentrosVerificacionResponse>>(r);
                    }
                }

                return new ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<NuevoVerificentroResponse>> Edit(long Id)
        {
            try
            {
                var result = new NuevoVerificentroResponse();
                if (Id > 0)
                {
                    var solicitud = await _context.Verificentros.FirstOrDefaultAsync(x => x.Id == Id);
                    if (solicitud != null)
                    {
                        string r = JsonConvert.SerializeObject(solicitud);
                        result = JsonConvert.DeserializeObject<NuevoVerificentroResponse>(r);
                    }
                }

                return new ResponseGeneric<NuevoVerificentroResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<NuevoVerificentroResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<ConfiguradorCitaResponse?>> GetConfiguradorByCVV(long id)
        {
            try
            {

                var calendario = await _context.ConfiguradorCita.FirstOrDefaultAsync(x => x.Id == id);
                if (calendario == null)
                    throw new ValidationException("No sé encontró el registro de la fecha.");
                var json = JsonConvert.SerializeObject(calendario);
                var result = JsonConvert.DeserializeObject<ConfiguradorCitaResponse>(json);
                return new ResponseGeneric<ConfiguradorCitaResponse>(result);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<ConfiguradorCitaResponse?>(null, true) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ConfiguradorCitaResponse?>(ex);
            }
        }
        public async Task<ResponseGeneric<ResponseGrid<ConfiguradorCitaResponse>>> GetFechasByIdCVV(ConfiguradorCitaRequestList request)
        {
            try
            {
                var fecha = DateTime.Now.Date;
                var tot = await _context.vConfiguradorCita.CountAsync(x => x.IdCVV == request.IdCVV && x.Fecha >= fecha);
                var calendario = _context.vConfiguradorCita.Where(x => x.IdCVV == request.IdCVV && x.Fecha >= fecha && (string.IsNullOrEmpty(request.Busqueda)
                || x.Fecha.ToString("dd/MM/yyyy").Contains(request.Busqueda.ToLower())
                || x.Capacidad.ToString().Contains(request.Busqueda.ToLower())
                || x.IntervaloMin.ToString().Contains(request.Busqueda.ToLower())));

                var filtered = await calendario.CountAsync();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    calendario = calendario.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    calendario = calendario.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                var json = JsonSerializer.Serialize(await calendario.ToListAsync());
                var result = JsonSerializer.Deserialize<List<ConfiguradorCitaResponse>>(json);
                return new ResponseGeneric<ResponseGrid<ConfiguradorCitaResponse>>(new ResponseGrid<ConfiguradorCitaResponse>
                {
                    Data = result,
                    Draw = "Draw",
                    RecordsFiltered = filtered,
                    RecordsTotal = tot
                });
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<ResponseGrid<ConfiguradorCitaResponse>>(null, true) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<ConfiguradorCitaResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> RegistroConfiguradorCita(List<ConfiguradorCitaDiasRequest> request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var jsonData = JsonConvert.SerializeObject(request);

                    var listRecord = JsonConvert.DeserializeObject<List<ConfiguradorCitum>>(jsonData);
                    if (!(listRecord?.Any() ?? false))
                    {
                        return new ResponseGeneric<bool>(false) { mensaje = "No se encuentra ninguna fecha para registrar en la solicitud." };
                    }
                    var currentDate = DateTime.Now.Date;
                    var fechasList = request.Select(x => x.Fecha);
                    var fechasExists = await _context.ConfiguradorCita.AnyAsync(x => x.Fecha >= currentDate && fechasList.Contains(x.Fecha) && x.IdCVV == request.FirstOrDefault().IdCVV);
                    if (fechasExists)
                    {
                        return new ResponseGeneric<bool>(!fechasExists) { mensaje = "No se pudieron registrar las fechas y horarios. Una o más fechas ya han sido registradas." };
                    }
                    _context.ConfiguradorCita.AddRange(listRecord);
                    var result = await _context.SaveChangesAsync() > 0;
                    transaction.Complete();
                    return new ResponseGeneric<bool>(result) { mensaje = "Se cargaron los horarios correctamente." };
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }
        public async Task<ResponseGeneric<EditFechaResponse>> EditFecha(ConfiguradorCitaEditRequest request)
        {
            try
            {
                var user = _userResolver.GetUser();
                bool cambios = false;
                var fecha = await _context.ConfiguradorCita.Include(x => x.IdCVVNavigation).FirstOrDefaultAsync(x => x.Id == request.Data.Id);
                if (fecha == null)
                {
                    return new ResponseGeneric<EditFechaResponse>(new EditFechaResponse { Error = true }) { mensaje = "No sé encontró registro de la fecha." };
                }
                if (fecha.IdCVVNavigation == null)
                {
                    return new ResponseGeneric<EditFechaResponse>(new EditFechaResponse { Error = true }) { mensaje = "No sé encontró información del verificentro del registro que s intenta editar." };
                }
                var estadoActual = fecha.Habilitado;
                var fechaRequest = request.Data;
                bool hasDifferences = fecha.Capacidad != fechaRequest.Capacidad ||
                    fecha.Fecha != fechaRequest.Fecha ||
                    fecha.Habilitado != fechaRequest.Habilitado ||
                    fecha.HoraInicio != fechaRequest.HoraInicio ||
                    fecha.HoraFin != fechaRequest.HoraFin ||
                    fecha.IdCVV != fechaRequest.IdCVV ||
                    fecha.IntervaloMin != fechaRequest.IntervaloMin;
                if (!hasDifferences)
                {
                    return new ResponseGeneric<EditFechaResponse>(new EditFechaResponse { Error = true }) { mensaje = "No se detectaron diferencias para guardar la información." };
                }

                // Modificar Intervalo
                var cambiarIntervalo = fechaRequest.IntervaloMin != fecha.IntervaloMin;
                var cambiarCapacidad = fechaRequest.Capacidad < fecha.Capacidad;
                var diferenciaCapacidad = fechaRequest.Capacidad;
                var fechaCitasInicio = fecha.Fecha.Date.Add(fechaRequest.HoraInicio);
                var fechaCitasFin = fecha.Fecha.Date.Add(fechaRequest.HoraFin);
                var reducirHorario = fecha.HoraInicio < fechaRequest.HoraInicio || fecha.HoraFin > fechaRequest.HoraFin;
                var existenCitas = await _context.vNumeroCitasHorarios.AnyAsync(x => x.IdCVV == fecha.IdCVV && (x.Fecha < fechaCitasInicio || x.Fecha > fechaCitasFin || cambiarIntervalo || !fechaRequest.Habilitado || x.NumeroCitas > fechaRequest.Capacidad) && x.FechaDia == fecha.Fecha.Date);
                if (existenCitas && !request.NoValidar)
                {
                    string message = "<ul class=\"text-start\">";
                    if (cambiarIntervalo)
                    {
                        message += "<li> El cambio de intervalo de tiempo entre horarios implica cancelar todas las citas.</li>";
                    }
                    if (cambiarCapacidad)
                    {
                        message += "<li> Disminuir la capacidad de citas implica cancelar las citas que estén fuera del rango.</li>";
                    }
                    if (!cambiarIntervalo && !cambiarCapacidad && existenCitas)
                    {
                        message += "<li> A cortar el horario de citas implica cancelar las citas que estén fuera del horario.</li>";
                    }
                    message += "</ul>";
                    return new ResponseGeneric<EditFechaResponse>(new EditFechaResponse { Error = true, Modificar = true })
                    {
                        mensaje = $"<h1>Advertencia</h1><p class=\"mb-2\" style=\"text-align: justify;\">Los cambios que se intentan realizar implican la cancelación de citas, la fecha quedará bloqueada momentaneamente para evitar generar más citas mientras se realizan los cambios.</p>{message}" +
                        "<p class=\"mt-2\">¿Aún así desea continuar?</p>"
                    };
                }
                bool requiereProcesamiento = false;
                if (existenCitas)
                {
                    requiereProcesamiento = true;
                    fecha.Habilitado = false;
                    await _context.SaveChangesAsync();
                    ProcesarCancelaciones(fechaRequest,
                                            fecha.Id,
                                            cambiarIntervalo,
                                            cambiarCapacidad,
                                            estadoActual,
                                            fechaCitasInicio,
                                            fechaCitasFin,
                                            user.IdUser,
                                            fecha.IdCVVNavigation.Nombre, reducirHorario);
                }
                else
                {
                    fecha.IntervaloMin = fechaRequest.IntervaloMin;
                    fecha.Habilitado = fechaRequest.Habilitado;
                    fecha.HoraInicio = fechaRequest.HoraInicio;
                    fecha.HoraFin = fechaRequest.HoraFin;
                    fecha.Capacidad = fechaRequest.Capacidad;
                    fecha.Habilitado = fechaRequest.Habilitado;
                    cambios = await _context.SaveChangesAsync() > 0;
                }
                var mensaje = !requiereProcesamiento ? "Se realizaron los cambios de horario para las citas." : "Los cambios necesitan un tiempo antes de efectuarse. Se enviará una notificación en la plataforma en el momento que se procese la información.";
                return new ResponseGeneric<EditFechaResponse>(new EditFechaResponse { Error = !cambios && !request.NoValidar }) { mensaje = mensaje };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<EditFechaResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> RegistroCVV(NuevoVerificentroResponse request)
        {
            try
            {
                var cvv = new Verificentro();
                if (request.Id > 0)
                {
                    cvv = _context.Verificentros.FirstOrDefault(x => x.Id == request.Id);
                    if (cvv != null)
                    {
                        cvv.Nombre = request.Nombre;
                        cvv.Clave = request.Clave;
                        cvv.Direccion = request.Direccion;
                        cvv.Rfc = request.Rfc;
                        cvv.Telefono = request.Telefono;
                        cvv.Correo = request.Correo;
                        cvv.GerenteTecnico = request.GerenteTecnico;
                        cvv.RepresentanteLegal = request.RepresentanteLegal;
                        cvv.Longitud = request.Longitud;
                        cvv.Latitud = request.Latitud;
                        cvv.DirectorGestionCalidadAire = request.DirectorGestionCalidadAire;
                        cvv.Municipio = request.Municipio;
                    }
                    _context.Verificentros.Update(cvv);
                    _context.SaveChanges();
                }
                else
                {
                    cvv = new Verificentro
                    {
                        Nombre = request.Nombre,
                        Activo = request.Activo,
                        Clave = request.Clave,
                        Direccion = request.Direccion,
                        Rfc = request.Rfc,
                        Telefono = request.Telefono,
                        Correo = request.Correo,
                        GerenteTecnico = request.GerenteTecnico,
                        RepresentanteLegal = request.RepresentanteLegal,
                        Longitud = request.Longitud,
                        Latitud = request.Latitud,
                        DirectorGestionCalidadAire = request.DirectorGestionCalidadAire,
                        ApiEndPoint = request.ApiEndPoint,
                        ApiKey = request.ApiKey,
                        Municipio = request.Municipio
                    };
                    _context.Verificentros.Add(cvv);
                    await _context.SaveChangesAsync();
                }
                return new ResponseGeneric<bool>(true);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(false);
            }
        }

        public async Task<ResponseGeneric<bool>> GuardarCarga(DataCarga dataCarga)
        {
            try
            {
                var cita = new CitaVerificacion();
                var foliosFormaValorada = new FoliosFormaValoradaVerificentro();
                var verificacion = new Verificacion();
                var resultadosVerificacion = new ResultadosVerificacion();
                var parametrostablaMaestraVerificacion = new ParametrosTablaMaestraVerificacion();
                var ciclosVerificacion = await _context.CicloVerificacions.ToListAsync();
                var IdUser = _userResolver.GetUser().IdUser;
                var verificaciones = dataCarga.Lista;
                var lineas = await _context.Lineas.Where(x => x.IdVerificentro == verificaciones.First().IdCVV).ToListAsync();
                var submarcas = await _context.CatSubMarcaVehiculos.Where(x => x.IdCatMarcaVehiculo == verificaciones.First().IdCatMarcaVehiculo).ToListAsync();
                var user = _userResolver.GetUser();
                var tablaCarga = new CargaMasiva()
                {
                    FechaRegistro = DateTime.Now,
                    NombreArchivo = dataCarga.FileName,
                    NumeroRegistros = dataCarga.Lista.Count,
                    IdUserRegistro = user.IdUser,
                    // TODO: Agregar cabio base64 y descomentar la siguiente línea y la coma de arriba.
                    Base64String = dataCarga.FileBase64
                };




                using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(5), TransactionScopeAsyncFlowOption.Enabled))
                {
                    var validacionArchivo = _context.CargaMasivas.Any(x => x.NombreArchivo.Equals(dataCarga.FileName) || x.Base64String.Equals(dataCarga.FileBase64)); //quitar null al NombreArchivo para evitar errores y en el Base64String también hacerlo not null
                    if (validacionArchivo)
                        throw new ValidationException("Ya se ha cargado un archivo con el mismo nombre o contenido. Para evitar duplicados deben ser diferentes.");
                    _context.CargaMasivas.Add(tablaCarga);
                    await _context.SaveChangesAsync();
                    foreach (var request in verificaciones)
                    {
                        var idLinea = lineas.FirstOrDefault(x => x.Clave.Equals(request.ClaveLinea))?.Id;
                        idLinea ??= lineas.First().Id; // SI no entra la linea con esa clave, asigna la primera que encuentra

                        var idSubmarca = submarcas.FirstOrDefault(x => x.Clave.Equals(request.IdSubMarcaVehiculo))?.Id;
                        idSubmarca ??= submarcas.First().Id;

                        if (request != null)
                        {
                            cita = new CitaVerificacion
                            {
                                Nombre = request.Nombre,
                                RazonSocial = request.RazonSocial,
                                Correo = request.Correo,
                                Fecha = request.Fecha,
                                IdCVV = request.IdCVV,
                                Placa = request.Placa,
                                IdCatMarcaVehiculo = request.IdCatMarcaVehiculo,
                                IdSubMarcaVehiculo = request.IdSubMarcaVehiculo,
                                Anio = request.Anio,
                                Acepto = true,
                                Serie = request.Serie,
                                ColorVehiculo = request.ColorVehiculo,
                                UrlComprobante = string.Empty,
                                Folio = GenerarFolio(10),
                                IdTipoCombustible = request.IdTipoCombustible,
                                NombreGeneraCita = request.Nombre,
                                Poblano = (request.Estado ?? "").Equals("PUEBLA"),
                                Estado = request.Estado,
                                IdUserReinicio = IdUser,
                                FechaCreacion = DateTime.Now

                            };
                            _context.CitaVerificacions.Add(cita);
                            await _context.SaveChangesAsync();

                            //Se obtiene ciclo de la verificacion para insertar 
                            var ciclo = _context.CicloVerificacions.FirstOrDefault(x => x.Activo && x.FechaInicio <= request.FechaVerificacion && x.FechaFin >= request.FechaVerificacion);

                            verificacion = new()
                            {
                                Fecha = request.FechaEmisionRef,
                                Placa = request.Placa,
                                Serie = request.Serie,
                                FolioCertificado = request.FolioAnterior ?? "0",
                                Vigencia = request.Vigencia,
                                Marca = request.Marca,
                                Modelo = request.Modelo,
                                Anio = request.Anio,
                                Combustible = request.Combustible,
                                TarjetaCirculacion = request.TarjetaCirculacion,
                                IdLinea = idLinea.Value,
                                IdCicloVerificacion = ciclo?.Id ?? 0,
                                IngresoManual = false,
                                IdTipoCombustible = request.IdTipoCombustible,
                                NumeroReferencia = request.NumeroReferencia,
                                Semestre = request.Semestre,
                                AnioVerificacion = request.AnioVerificacion,
                                IdTipoCertificado = request.IdTipoCertificado,
                                IdUserTecnico = IdUser,
                                IdMotivoVerificacion = request.IdMotivoVerificacion,
                                IdCitaVerificacion = cita.Id,
                                NoIntento = 0,
                                Orden = 0,
                                FechaVerificacion = request.FechaVerificacion
                            };

                            _context.Verificacions.Add(verificacion);
                            await _context.SaveChangesAsync();

                            resultadosVerificacion = new()
                            {
                                Id = verificacion.Id,
                                InicioPruebas = request.FechaVerificacion,
                                FinalizacionPruebas = request.FechaVerificacion,
                                EstatusPrueba = EstatusVerificacion.FolioImpreso,
                                RESULTADO = Resultados.DictCertificadoEquivalencia[request.IdTipoCertificado],
                                C_RECHAZO = request.IdTipoCertificado == TipoCertificado.ConstanciasNoAprobado ? CausaRechazo.Emisiones : CausaRechazo.NoAplica,
                                C_RECHAZO_OBD = OBD.SinCodigoError,
                                PruebaObd = false,
                                PruebaEmisiones = false,
                                PruebaOpacidad = false,
                                RESULTADO_PROVEEDOR = Resultados.DictCertificadoEquivalencia[request.IdTipoCertificado],
                                FugasSistemaEscape = false,
                                PortafiltroAire = false,
                                TaponDispositivoAceite = false,
                                FiltroAire = false,
                                TaponCombustible = false,
                                Bayoneta = false,
                                FugaAceiteMotor = false,
                                FugaAceiteTransmision = false,
                                FugaLiquidoRefrigerante = false,
                                DibujoNeumaticos = false,
                                DesperfectosNeumaticos = false,
                                DimensionesNeumaticoIncorrectas = false,
                                ControlEmisionDesconectados = false,
                                ControlEmisionAlterados = false,
                                PlacasCorrespondientes = false,
                                GobernadorBuenEstado = false,
                                NumeroEscapes = 0,
                                Etapa = "NA",
                                SPS_Humo = 0,
                                SPS_5024 = 0,
                                SPS_2540 = 0,
                                HC = 0,
                                CO = 0,
                                CO2 = 0,
                                O2 = 0,
                                NO = 0,
                                LAMDA = 0,
                                FCNOX = 0,
                                FCDIL = 0,
                                RPM = 0,
                                KPH = 0,
                                VEL_LIN = 0,
                                VEL_ANG = 0,
                                BHP = 0,
                                PAR_TOR = 0,
                                FUERZA = 0,
                                POT_FRENO = 0,
                                TEMP = 0,
                                PRESION = 0,
                                HUMREL = 0,
                                OBD_TIPO_SDB = "NA",
                                OBD_MIL = 0,
                                OBD_CATAL = 0,
                                OBD_CILIN = 0,
                                OBD_COMBU = 0,
                                OBD_INTEG = 0,
                                OBD_OXIGE = 0,
                                LAMDA_5024 = 0,
                                TEMP_5024 = 0,
                                HR_5024 = 0,
                                PSI_5024 = 0,
                                FCNOX_5024 = 0,
                                FCDIL_5024 = 0,
                                RPM_5024 = 0,
                                KPH_5024 = 0,
                                THP_5024 = 0,
                                VOLTS_5024 = 0,
                                HC_5024 = 0,
                                CO_5024 = 0,
                                CO2_5024 = 0,
                                COCO2_5024 = 0,
                                O2_5024 = 0,
                                NO_5024 = 0,
                                LAMDA_2540 = 0,
                                TEMP_2540 = 0,
                                HR_2540 = 0,
                                PSI_2540 = 0,
                                FCNOX_2540 = 0,
                                FCDIL_2540 = 0,
                                RPM_2540 = 0,
                                KPH_2540 = 0,
                                THP_2540 = 0,
                                VOLTS_2540 = 0,
                                HC_2540 = 0,
                                CO_2540 = 0,
                                CO2_2540 = 0,
                                COCO2_2540 = 0,
                                O2_2540 = 0,
                                NO_2540 = 0,
                                OPACIDADP = 0,
                                OPACIDADK = 0,
                                TEMP_MOT = 0,
                                VEL_GOB = 0,
                                POTMIN_RPM = 0,
                                POTMAX_RPM = 0,
                                TEMP_GAS = 0,
                                TEMP_CAM = 0,
                            };

                            _context.ResultadosVerificacions.Add(resultadosVerificacion);
                            await _context.SaveChangesAsync();

                            parametrostablaMaestraVerificacion = new()
                            {
                                IdVerificacion = verificacion.Id,
                                IdCatMarcaVehiculo = 0,
                                IdCatSubmarcaVehiculo = 0,
                                Motor_DSL = 0,
                                COMB_ORIG = 0,
                                CARROCERIA = 0,
                                ALIM_COMB = 0,
                                CILINDROS = 0,
                                CILINDRADA = 0,
                                PBV = 0,
                                PBV_EQUIV = 0,
                                PBV_ASM = 0,
                                CONV_CATAL = 0,
                                OBD = 0,
                                C_ABS = 0,
                                T_TRACC = 0,
                                C_TRACC = 0,
                                T_PRUEBA = 0,
                                PROTOCOLO = 0,
                                POTMAX_RPM = 0,
                                ANO_DESDE = 0,
                                ANO_HASTA = 0,
                                O2_MAX = 0,
                                LAMDA_MAX = 0,
                                POT_5024 = 0,
                                POT_2540 = 0,
                                DOBLECERO = 0,
                                CERO_GASOL = 0,
                                CERO_GASLP = 0,
                                CERO_GASNC = 0,
                                CERO_DSL = 0,
                                REF_00 = 0
                            };

                            _context.ParametrosTablaMaestraVerificacions.Add(parametrostablaMaestraVerificacion);
                            await _context.SaveChangesAsync();

                            //Se manda llamar al método para crear el folio 
                            await _smadsotGenericInserts.ValidateFolio(request.IdTipoCertificado, request.IdCVV, TipoTramite.CV, IdUser, request.Estado, verificacion.Id, null, null, null, false, request.FolioCertificado, request.ImporteActual, request.FechaVerificacion);


                        }
                    }

                    scope.Complete();
                }

                return new ResponseGeneric<bool>(true);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<bool>(false) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(false) { mensaje = "Ocurrió un error al procesar la información." };
            }
        }

        #region Private Methods

        private async void ProcesarCancelaciones(vConfiguradorCitum UpdateConfigurador, long IdConfiguradorCita, bool cambiarIntervalo,
            bool cambiarCapacidad, bool estadoActual, DateTime fechaIncio, DateTime fechaFin, long IdUser, string cvvName, bool reducirHorario)
        {
            var fechaSinCambios = new ConfiguradorCitum();
            var options = new DbContextOptionsBuilder<SmadotDbContext>()
                    .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                    .Options;
            try
            {
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted, // Puedes ajustar el nivel de aislamiento según tus necesidades.
                    Timeout = TimeSpan.FromMinutes(5)
                };
                using (var context = new SmadotDbContext(options))
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var listCorreos = new List<PersonaCita>();
                    var culture = new CultureInfo("es-MX");
                    var currentDate = DateTime.Now;
                    var fecha = context.ConfiguradorCita.FirstOrDefault(x => x.Id == IdConfiguradorCita);
                    if (fecha == null)
                    {
                        throw new ValidationException("Ocurrió un error obtener el registro de la fecha a editar.");
                    }
                    fechaSinCambios = new ConfiguradorCitum
                    {
                        IntervaloMin = fecha.IntervaloMin,
                        Habilitado = fecha.Habilitado,
                        HoraInicio = fecha.HoraInicio,
                        HoraFin = fecha.HoraFin,
                        Capacidad = fecha.Capacidad,
                    };
                    var diferenciaCapacidad = fecha.Capacidad - UpdateConfigurador.Capacidad;
                    var configuradorFecha = fecha.Fecha.ToString("d", culture);
                    var fechaCambio = currentDate.ToString("g", culture);
                    fecha.IntervaloMin = UpdateConfigurador.IntervaloMin;
                    fecha.Habilitado = UpdateConfigurador.Habilitado;
                    fecha.HoraInicio = UpdateConfigurador.HoraInicio;
                    fecha.HoraFin = UpdateConfigurador.HoraFin;
                    fecha.Capacidad = UpdateConfigurador.Capacidad;
                    if (UpdateConfigurador.Habilitado && !cambiarIntervalo && cambiarCapacidad)
                    {
                        var fechasCapacidad = await context.vNumeroCitasHorarios.Where(x => x.IdCVV == fecha.IdCVV && x.FechaDia == UpdateConfigurador.Fecha && x.Capacidad > UpdateConfigurador.Capacidad).ToListAsync();
                        foreach (var fechaHorario in fechasCapacidad)
                        {
                            var fechasReducirCapacidad = context.CitaVerificacions.Where(x => x.IdCVV == fecha.IdCVV && x.Fecha == fechaHorario.Fecha && (x.Cancelada == null || x.Cancelada == false)).OrderBy(x => x.FechaCreacion).Take(diferenciaCapacidad);
                            await fechasReducirCapacidad.ForEachAsync(async (x) =>
                            {
                                x.Cancelada = true;
                                x.FechaCancelacion = DateTime.Now;
                                x.IdUserCancelo = IdUser;
                                x.IdCatMotivoCancelacionCita = CatMotivoCancelacionCita.ReduccionDeCapacidad;
                            });
                            var correosCitasCanceladas = await fechasReducirCapacidad.Select(x => new PersonaCita { Correo = x.Correo, Fecha = x.Fecha, Nombre = x.NombreGeneraCita }).ToListAsync();
                            listCorreos.AddRange(correosCitasCanceladas);
                        }
                    }
                    if (reducirHorario || cambiarIntervalo || !UpdateConfigurador.Habilitado)
                    {
                        var inicioDia = fechaIncio.Date;
                        var finDia = new DateTime(inicioDia.Year, inicioDia.Month, inicioDia.Day, 23, 59, 59);
                        var listCitas = context.CitaVerificacions.Where(x => x.IdCVV == fecha.IdCVV
                        && x.Fecha > inicioDia && x.Fecha < finDia && (x.Fecha > fechaFin || x.Fecha < fechaIncio || cambiarIntervalo || !UpdateConfigurador.Habilitado)
                        && (x.Cancelada == null || x.Cancelada == false));
                        await listCitas.ForEachAsync((x) =>
                        {
                            x.Cancelada = true;
                            x.FechaCancelacion = DateTime.Now;
                            x.IdUserCancelo = IdUser;
                            x.IdCatMotivoCancelacionCita = reducirHorario ? CatMotivoCancelacionCita.ReduccionDeHorario : cambiarIntervalo ? CatMotivoCancelacionCita.CambioDeIntervaloEntreCitas : CatMotivoCancelacionCita.DiaDeshabilitado;
                        });
                        var correosCitasCanceladas = await listCitas.Select(x => new PersonaCita
                        {
                            Correo = x.Correo,
                            Fecha = x.Fecha,
                            Nombre = x.NombreGeneraCita
                        }).ToListAsync();
                        listCorreos.AddRange(correosCitasCanceladas);
                    }
                    var urlBlob = await CrearReporte(listCorreos);
                    if (urlBlob.Status != ResponseStatus.Success)
                    {
                        throw new ValidationException(urlBlob.mensaje);
                    }
                    var urlArr = urlBlob.Response.Split("/");
                    urlArr = urlArr.Skip(4).ToArray();
                    var url = "";
                    foreach (var item in urlArr)
                    {
                        url += "/" + item;
                    }
                    Alertum alerta = new()
                    {
                        Data = null,
                        FechaModificacion = null,
                        Fecha = currentDate,
                        IdEstatusInicial = 0,
                        IdEstatusFinal = 0,
                        IdVerificentro = fecha.IdCVV,
                        Leido = false,
                        MovimientoInicial = $"Los ajustes de horarios de la fecha {configuradorFecha} se realizaron correctamente el" +
                        $" {fechaCambio} para el {cvvName}. Puede descargar los datos de las personas con citas canceladas <a " +
                        $"href=\"/FilesStorage?url={url}\" target=\"_blank\" id=\"link-file\">aquí</a>.",
                        IdUser = IdUser,
                        TableId = fecha.Id,
                        MovimientoFinal = null,
                        TableName = DictAlertas.ConfiguradorCita,
                        Procesada = false
                    };
                    context.Alerta.Add(alerta);
                    var accionExitosa = await context.SaveChangesAsync() > 0;
                    transaction.Complete();
                }
            }
            catch (ValidationException ex)
            {
                using (var contextSecundario = new SmadotDbContext(options))
                {
                    Alertum alerta = new()
                    {
                        Data = null,
                        FechaModificacion = null,
                        Fecha = DateTime.Now,
                        IdEstatusInicial = 0,
                        IdEstatusFinal = 0,
                        Leido = false,
                        IdVerificentro = UpdateConfigurador.IdCVV,
                        MovimientoInicial = $"Ocurrió un error al generar el archivo excel los cambios serán revertidos para el día {UpdateConfigurador.Fecha:dd/MM/yyyy}" +
                        $" del {cvvName}. El registro no se verá afectado.",
                        IdUser = IdUser,
                        TableId = UpdateConfigurador.Id,
                        MovimientoFinal = null,
                        TableName = DictAlertas.ConfiguradorCita,
                        Procesada = false
                    };
                    contextSecundario.Alerta.Add(alerta);
                    var fecha = await contextSecundario.ConfiguradorCita.FirstOrDefaultAsync(x => x.Id == IdConfiguradorCita);
                    if (fecha != null)
                    {
                        fecha.Habilitado = estadoActual;
                    }
                    JObject data = new JObject();
                    data["Exception"] = JsonConvert.SerializeObject(ex.Message);
                    data["Tipo"] = "ExcepcionGenerarExcelCitas";
                    contextSecundario.Errors.Add(new()
                    {
                        Created = DateTime.Now,
                        Values = JsonConvert.SerializeObject(data)
                    });
                    await contextSecundario.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                using (var contextSecundario = new SmadotDbContext(options))
                {
                    Alertum alerta = new()
                    {
                        Data = JsonConvert.SerializeObject(ex),
                        FechaModificacion = null,
                        Fecha = DateTime.Now,
                        IdVerificentro = UpdateConfigurador.IdCVV,
                        IdEstatusInicial = 0,
                        IdEstatusFinal = 0,
                        Leido = false,
                        MovimientoInicial = $"Ocurrió un error al intentar guardar los cambios para el día {UpdateConfigurador.Fecha:dd/MM/yyyy} del {cvvName}. El registro no fue afectado.",
                        IdUser = IdUser,
                        TableId = UpdateConfigurador.Id,
                        MovimientoFinal = null,
                        TableName = DictAlertas.ConfiguradorCita,
                        Procesada = false

                    };
                    contextSecundario.Alerta.Add(alerta);
                    var fecha = await contextSecundario.ConfiguradorCita.FirstOrDefaultAsync(x => x.Id == IdConfiguradorCita);
                    if (fecha != null)
                    {
                        fecha.Habilitado = estadoActual;
                    }
                    await contextSecundario.SaveChangesAsync();
                }
            }
        }
        private async Task<ResponseGeneric<string>> CrearReporte(List<PersonaCita> list)
        {

            try
            {
                list = list.OrderByDescending(x => x.Fecha).ToList();
                var listDoc = list.Select(x => new Smadot.Utilities.Modelos.Documentos.PersonaCita { Correo = x.Correo, Nombre = x.Nombre, Fecha = x.Fecha }).ToList();
                var reporteExcel = await _excelBuilder.ExcelDocumentCitasCanceladas(listDoc);
                if (reporteExcel.Status == ResponseStatus.Failed)
                {
                    return new ResponseGeneric<string>("Hubo un error.")
                    { mensaje = "El archivo excel no pudo ser generado." + reporteExcel.CurrentException };

                }
                var fecha = list.FirstOrDefault().Fecha;

                var url = await _blobStorage.UploadFileAsync(reporteExcel.Response.DocumentoExcel, $"CitaCanceladas/{fecha:ddMMyyyy}" +
                    $"/CitasCanceladas_{fecha:dd_MM_yyyy}_" +
                    $"{DateTime.Now:ddMMyyyyHmmss}.xlsx");
                if (string.IsNullOrEmpty(url))
                {
                    return new ResponseGeneric<string>("Hubo un error.")
                    { mensaje = "El archivo excel no ser cargado en el almacenamiento en la nube." };
                }

                return new ResponseGeneric<string>() { Response = url };
            }
            catch (Exception ex)
            {
                //return new ResponseGeneric<string>(ex) { mensaje = "El archivo excel no pudo ser generado." };
                return new ResponseGeneric<string>(ex) { mensaje = $"El archivo excel no pudo ser generado. {ex.Message} {ex.StackTrace}" };
            }
        }

        private string GenerarFolio(int length)
        {
            Random random = new Random();

            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(characters, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion
    }

}

public interface IDirectorioCentrosVerificacionNegocio
{
    public Task<ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>> Consulta(DirectorioCentrosVerificacionListRequest request);
    public Task<ResponseGeneric<List<DirectorioCentrosVerificacionResponse>>> GetById(long Id);

    public Task<ResponseGeneric<NuevoVerificentroResponse>> Edit(long Id);

    public Task<ResponseGeneric<bool>> RegistroConfiguradorCita(List<ConfiguradorCitaDiasRequest> request);
    public Task<ResponseGeneric<EditFechaResponse>> EditFecha(ConfiguradorCitaEditRequest request);
    public Task<ResponseGeneric<ResponseGrid<ConfiguradorCitaResponse>>> GetFechasByIdCVV(ConfiguradorCitaRequestList request);

    public Task<ResponseGeneric<ConfiguradorCitaResponse>> GetConfiguradorByCVV(long id);

    public Task<ResponseGeneric<bool>> RegistroCVV(NuevoVerificentroResponse request);

    public Task<ResponseGeneric<bool>> GuardarCarga(DataCarga verificaciones);
}