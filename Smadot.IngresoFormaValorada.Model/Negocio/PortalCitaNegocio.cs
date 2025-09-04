using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.Reporting.Interfaces;
using QRCoder;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.PortalCitas.Request;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.EnvioCorreoElectronico;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;
using static Smadot.Models.Entities.Reposicion.Response.ReposicionResponseData;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.PortalCita.Response;
using iTextSharp.text.pdf.parser;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using Newtonsoft.Json;
using Smadot.Utilities.GestionTokens;
using Smadot.Models.Entities.Cita.Request;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Response;
using Smadot.Utilities;
using Smadsot.Historico.Models.DataBase;
using Namespace;
using Smadot.Models.GenericProcess;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;
using Smadot.Utilities.ServicioMultas;
using System.Net.NetworkInformation;
using ZXing;
using SixLabors.ImageSharp;
using System.Web;

namespace Smadot.IngresoFormaValorada.Model.Negocio
{
    public class PortalCitaNegocio : IPortalCitaNegocio
    {
        private SmadotDbContext _context;
        private SmadsotHistoricoDbContext _contextHistorico;
        private readonly IPdfBuider _pdfBuilder;
        private readonly BlobStorage _blobStorage;
        private readonly IConfiguration _configuration;
        private readonly string _urlSite;
        private readonly bool _sinRestricciones;
        private readonly bool _excepcionRegla;
        private readonly IUserResolver _userResolver;
        private readonly SmadsotGenericInserts _genericInserts;
        private readonly IConsultaVehiculoServicio _consultaVehiculoServicio;
        public PortalCitaNegocio(SmadotDbContext context, IPdfBuider pdfBuilder, IConfiguration configuration, IUserResolver userResolver, SmadsotHistoricoDbContext contextHistorico, SmadsotGenericInserts genericInserts, IConsultaVehiculoServicio consultaVehiculoServicio)
        {
            _context = context;
            _pdfBuilder = pdfBuilder;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _configuration = configuration;
            _urlSite = configuration["SiteUrl"];
            _userResolver = userResolver;
            var rol = _userResolver.GetUser()?.RoleNames?.FirstOrDefault() ?? string.Empty;
            _sinRestricciones = rol.Contains("1. Administrador de citas") || rol.Contains("ADMIN SMADSOT");
            _excepcionRegla = rol.Contains("Usuario Verificador");
            _contextHistorico = contextHistorico;
            _genericInserts = genericInserts;
            _consultaVehiculoServicio = consultaVehiculoServicio;
        }

        public async Task<ResponseGeneric<PortalCitasResponse>> GetPortalCitasByIdCvv(long id)
        {
            try
            {
                var result = new PortalCitasResponse();
                #region Reglas anteriores
                // ------------------------------------------------------- ---------------------------------------------------
                // TODO: Cambiar reglas para las Citas y como se obtienen los espacios disponibles
                // var config = await _context.Verificentros
                //     .Where(x => x.Id == id)
                //     .Select(x => new
                //     {
                //         x.Id,
                //         ConfiguradorCita = x.ConfiguradorCita,
                //         DiaNoLaborables = x.DiaNoLaborables
                //     })
                //     .FirstOrDefaultAsync();

                // var disableDays = new List<int>();
                // var disableDates = new List<string>();
                // result.HorasResponses = new();
                // if (config != null)
                // {
                //     var confCita = config.ConfiguradorCita.ToList();

                //     if (confCita.Count > 0)
                //     {
                //         var disabledConfCita = confCita
                //             .Where(x => (x.Capacidad == 0 || x.InvertavaloMin == 0))
                //             .ToList();

                //         disableDays.AddRange(disabledConfCita.Select(x => GetDayOfWeekNumber(x.Dia)));
                //         result.Dias = confCita.Select(x => new PortalCitasDiasResponse
                //         {
                //             Dia = x.Dia,
                //             HoraInicio = x.HoraInicio,
                //             HoraFin = x.HoraFin,
                //         }).ToList();
                //     }

                //     if (config.DiaNoLaborables != null && config.DiaNoLaborables.Count() > 0)
                //     {
                //         disableDates.AddRange(config.DiaNoLaborables.Select(item => item.Fecha.ToString("yyyy/MM/dd")));
                //     }

                //     var today = DateTime.Now;
                //     var toMaxDate = today.AddDays(14);
                //     var maxDate = new DateTime(toMaxDate.Year, toMaxDate.Month, toMaxDate.Day, 23, 59, 59);
                //     var fechaInicioHoy = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                //     var fechaFinHoy = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                //     var fechasUso = await _context.vIntervalosCitas
                //                 .Where(x => x.IdCVV == config.Id && x.Fecha >= today && x.Fecha <= maxDate)
                //                 .ToListAsync();
                //     // var confCitaHoy = confCita.FirstOrDefault(x => x.Dia.Equals(GetDayOfWeekName(today)) && (x.Capacidad > 0 && x.InvertavaloMin > 0));

                //     for (DateTime i = today; i <= toMaxDate; i = i.AddDays(1))
                //     {
                //         var dia = GetDayOfWeekString(i.DayOfWeek);
                //         var configCitaHoy = confCita.FirstOrDefault(x => x.Dia.Equals(dia) && (x.Capacidad > 0 && x.InvertavaloMin > 0));

                //         if (configCitaHoy == null)
                //         {
                //             disableDates.Add(i.ToString("yyyy/MM/dd"));
                //         }
                //         else
                //         {
                //             var inicioDia = new DateTime(i.Year, i.Month, i.Day, 0, 0, 0);
                //             var finDia = new DateTime(i.Year, i.Month, i.Day, 23, 59, 59);

                //             var horariosEnUso = fechasUso
                //                 .Where(x => x.IdCVV == config.Id && x.Fecha >= inicioDia && x.Fecha <= finDia && x.TotalIntervalo >= configCitaHoy.Capacidad)
                //                 .Select(x => x.Fecha.ToString("HH:mm")).ToList();

                //             var horaInicio = configCitaHoy?.HoraInicio ?? new TimeSpan(0, 0, 0);
                //             var horaFin = configCitaHoy?.HoraFin ?? new TimeSpan(0, 0, 0);
                //             var intervaloMinutos = configCitaHoy?.InvertavaloMin ?? 0;

                //             var listaHoras = GetTimeSlots(horaInicio, horaFin, intervaloMinutos, i == today);
                //             var listaResultado = listaHoras.Except(horariosEnUso).ToList();

                //             if (i.Date == today.Date && today.TimeOfDay > horaFin)
                //             {
                //                 disableDates.Add(i.ToString("yyyy/MM/dd"));
                //                 // else if (horariosEnUso.Count() >= (configCitaHoy?.Capacidad ?? 0))
                //                 // {
                //                 //     disableDates.Add(i.ToString("yyyy/MM/dd"));
                //                 // }
                //                 // else if (listaResultado.Count() == 0)
                //                 // {
                //                 //     disableDates.Add(i.ToString("yyyy/MM/dd"));
                //                 // }
                //             }
                //             else if (!listaResultado.Any())
                //             {
                //                 disableDates.Add(i.ToString("yyyy/MM/dd"));
                //             }
                //             else
                //             {
                //                 result.HorasResponses.Add(new() { Fecha = i.ToString("dd/MM/yyyy"), HorasDisponible = listaResultado });
                //             }

                //             // else if (listaResultado.Count() == 0)
                //             // {
                //             //     disableDates.Add(i.ToString("yyyy/MM/dd"));
                //             // }
                //         }
                //     }

                //     result.DisableDays = disableDays;
                //     result.DisableDates = disableDates;
                // }
                #endregion


                result.Dias = new List<PortalCitasDiasResponse>();//Ya no aplica
                result.DisableDays = new List<int>();//Ya no aplica
                result.DisableDates = new List<string>();
                result.HorasResponses = new List<PortalCitasHorasResponse>();
                var today = DateTime.Now;
                var toMaxDate = today.AddDays(14);
                var maxDate = new DateTime(toMaxDate.Year, toMaxDate.Month, toMaxDate.Day, 23, 59, 59);
                var config = await _context.vConfiguradorCita.Where(x => x.Fecha >= today && x.Fecha <= maxDate && x.IdCVV == id).ToListAsync();
                if (!config.Any())
                    throw new ValidationException("No se encontraron horarios.");

                // Generar la lista de fechas entre today y toMaxDate
                var todasLasFechas = new List<DateTime>();
                var fechaActual = today.Date;
                while (fechaActual <= toMaxDate)
                {
                    todasLasFechas.Add(fechaActual);
                    fechaActual = fechaActual.AddDays(1);
                }

                // Identificar las fechas que no están en vConfiguradorCita
                var fechasFaltantes = todasLasFechas.Where(fecha => !config.Any(c => c.Fecha.Date == fecha.Date)).ToList();

                // Agregar las fechas faltantes a la lista DisableDates
                foreach (var fechaFaltante in fechasFaltantes)
                {
                    result.DisableDates.Add(fechaFaltante.ToString("yyyy/MM/dd"));
                }
                var fechasUso = await _context.vIntervalosCitas.Where(x => x.IdCVV == id && x.Fecha >= today && x.Fecha <= maxDate).ToListAsync();
                foreach (var c in config)
                {
                    var esHoy = c.Fecha.Date == today.Date;

                    if ((!c.Habilitado || (esHoy ? today.TimeOfDay > c.HoraFin : false)) && !_sinRestricciones)
                    {
                        result.DisableDates.Add(c.Fecha.ToString("yyyy/MM/dd"));
                        continue;
                    }
                    var listaHoras = GetTimeSlots(c.HoraInicio, c.HoraFin, c.IntervaloMin, c.Fecha.Date == today.Date);
                    var inicioDia = new DateTime(c.Fecha.Year, c.Fecha.Month, c.Fecha.Day, 0, 0, 0);
                    var finDia = new DateTime(c.Fecha.Year, c.Fecha.Month, c.Fecha.Day, 23, 59, 59);
                    var horariosEnUso = fechasUso.Where(x => x.IdCVV == id && x.Fecha >= inicioDia && x.Fecha <= finDia && x.TotalIntervalo >= c.Capacidad).Select(x => x.Fecha.ToString("HH:mm")).ToList();
                    var listaResultado = new List<string>();
                    if (!_sinRestricciones)
                    {
                        listaResultado = listaHoras.Except(horariosEnUso).ToList();
                    }
                    else
                    {
                        listaResultado = listaHoras;

                    }
                    if (!listaResultado.Any() && !_sinRestricciones)
                        result.DisableDates.Add(c.Fecha.ToString("yyyy/MM/dd"));
                    else
                        result.HorasResponses.Add(new() { Fecha = c.Fecha.ToString("dd/MM/yyyy"), HorasDisponible = listaResultado });

                }


                return new ResponseGeneric<PortalCitasResponse>(result);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<PortalCitasResponse>(ex.Message) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<PortalCitasResponse>(ex) { mensaje = "No se han podido cargar los horarios para el cvv seleccionado." };
            }
        }
        public async Task<ResponseGeneric<ResponseGrid<HistorialCitasResponse>>> Consulta(CitaGridRequest request)
        {
            try
            {
                var user = _userResolver.GetUser();
                var query = _context.vHistorialCita
                            .Where(x => x.IdVerificentro == user.IdVerificentro);

                var query2 = _contextHistorico.CitaVerificacionHistoricos
                                .Where(x => x.IdVerificentro == user.IdVerificentro);

                if (request.Fecha1.HasValue && request.Fecha2.HasValue)
                {
                    query = query.Where(x => x.Fecha >= request.Fecha1 && x.Fecha <= request.Fecha2.Value.AddDays(1));
                    query2 = query2.Where(x => x.Fecha >= request.Fecha1 && x.Fecha <= request.Fecha2.Value.AddDays(1));
                }

                if (request.Atendida.HasValue)
                {
                    if (request.Atendida.Value)
                    {
                        query = query.Where(x => x.IdRecepcionDocumentos != null);
                        query2 = query2.Where(x => x.IdRecepcionDocumentos != null);
                    }
                    else
                    {
                        query = query.Where(x => x.IdRecepcionDocumentos == null);
                        query2 = query2.Where(x => x.IdRecepcionDocumentos == null);
                    }
                }


                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    var searchTerms = request.Busqueda.ToLower();
                    query = query.Where(x =>
                                x.Marca.ToLower().Contains(searchTerms)
                                || x.CitaSerie.ToLower().Contains(searchTerms)
                                || x.Folio.ToLower().Contains(searchTerms)
                                || x.Placa.ToLower().Contains(searchTerms)
                                || x.Marca.ToLower().Contains(searchTerms));

                    query2 = query2.Where(x =>
                                x.Marca.ToLower().Contains(searchTerms)
                                || x.CitaSerie.ToLower().Contains(searchTerms)
                                || x.Folio.ToLower().Contains(searchTerms)
                                || x.Placa.ToLower().Contains(searchTerms)
                                || x.Marca.ToLower().Contains(searchTerms));
                }

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    query = query.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                var total = await query.CountAsync();
                var total2 = 0;
                try
                {
                    total2 = await query2.CountAsync();
                }
                catch (Exception)
                {


                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    query = query.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }


                var data = await query.ToListAsync();
                var fechaHistorico = DateTime.Now.Date.AddDays(-15);
                var data2 = new List<CitaVerificacionHistorico>();
                if (total == 0 || (request.Fecha2 != null && request.Fecha2.Value > fechaHistorico) || data.Count < (request.Registros ?? 0))
                {
                    try
                    {
                        //total2 += await query2.CountAsync();

                        if (!string.IsNullOrEmpty(request.Busqueda))
                        {
                            var searchTerms = request.Busqueda.ToLower();
                            query2 = query2.Where(x =>
                                        x.Marca.ToLower().Contains(searchTerms)
                                        || x.CitaSerie.ToLower().Contains(searchTerms)
                                        || x.Folio.ToLower().Contains(searchTerms)
                                        || x.Placa.ToLower().Contains(searchTerms)
                                        || x.Marca.ToLower().Contains(searchTerms));
                        }

                        if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                        {
                            query2 = query2.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                        }
                        var totalrecientes = total;
                        total2 = await query2.CountAsync();


                        if (request.Pagina > 0 && request.Registros != null && (request.Registros ?? 0) > 0)
                        {
                            var paginasTotalRecientes = totalrecientes / request.Registros;

                            var paginaActual = request.Pagina.Value - 1;
                            var paginaHistorico = paginaActual - paginasTotalRecientes;
                            var diferenciaPrimerPagina = totalrecientes % request.Registros;
                            if (paginaHistorico >= 0)
                            {
                                int skipCount = paginaHistorico == 0 ? 0 : (paginaHistorico * (request.Registros ?? 0) ?? 0);
                                query2 = query2.Skip(skipCount + (diferenciaPrimerPagina ?? 0)).Take(request.Registros.Value);

                                data2 = await query2.ToListAsync();
                            }
                            else if (diferenciaPrimerPagina > 0)
                            {
                                query2 = query2.Take(diferenciaPrimerPagina ?? 0);
                            }
                        }
                        else
                            data2 = await query2.ToListAsync();
                    }
                    catch (Exception ex)
                    {
                        // Manejar la excepción apropiadamente
                    }
                }

                total += total2;

                var result = JsonConvert.SerializeObject(data);
                var list = JsonConvert.DeserializeObject<List<HistorialCitasResponse>>(result) ?? new();
                result = JsonConvert.SerializeObject(data2);
                list.AddRange(JsonConvert.DeserializeObject<List<HistorialCitasResponse>>(result) ?? new());

                return new ResponseGeneric<ResponseGrid<HistorialCitasResponse>>(new ResponseGrid<HistorialCitasResponse> { Data = list ?? new(), RecordsFiltered = total, RecordsTotal = total });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<HistorialCitasResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<PortalCitasRegistroResponse>> RegistroCita(PortalCitaRequest request)
        {
            try
            {
                //var confirmacionAutomatica = false;
                var resultSerie = new ResponseViewModel(true);
                var ressResponse = new PortalCitasRegistroResponse();
                var transactionOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted, // Puedes ajustar el nivel de aislamiento según tus necesidades.
                    Timeout = TimeSpan.FromMinutes(5)
                };

                //if (request.Poblano == true)
                //{
                //    var consulta = await _consultaVehiculoServicio.Consulta(request.Serie, request.Placa);

                //    if (consulta.codigo == 0)
                //    {
                //        resultSerie.IsSuccessFully = false;
                //        ressResponse.ErrorMessage = consulta.desc;
                //        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                //    }

                //    if (consulta.codigo == 1 && consulta.vehs[0].cuentaInfr > 0)
                //    {
                //        resultSerie.IsSuccessFully = false;
                //        //result.Message = consulta.VchMensajeFotomulta;
                //        ressResponse.ErrorMessage = $"El auto tiene {consulta.vehs[0].cuentaInfr} multa(s) pendiente(s) de pago derivadas del “Monitoreo Remoto” y/o de “Contaminación Ostensible”, no es posible solicitar una cita. Para mayor información visite: https://infraccionespuebla.monitorambiental.mx ";
                //        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                //    }
                //}

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var fechaCita = request.Fecha;
                    var diaText = request.Fecha.ToString("dddd", new CultureInfo("es-MX"));
                    var dia = char.ToUpper(diaText[0]) + diaText.Substring(1);

                    if (request.IdSubMarca == -1)
                    {
                        var verificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == request.IdCVV && x.Activo);
                        if (string.IsNullOrEmpty(request.SubMarcaNueva) || string.IsNullOrEmpty(request.Marca))
                        {
                            ressResponse.ErrorMessage = $"Falta información para completar la solicitud de registro en la tabla maestra.";
                            return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                        }
                        var alerta = new Alertum
                        {
                            TableName = DictAlertas.Cita,
                            Data = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                PreserveReferencesHandling = PreserveReferencesHandling.None,
                                NullValueHandling = NullValueHandling.Ignore
                            }),
                            TableId = 0,
                            IdVerificentro = verificentro.Id,
                            IdUser = null,
                            IdEstatusInicial = 0,
                            MovimientoInicial = string.Format(MovimientosDicts.DictAlertaCita[CitaEstatus.SoloNotificacion], request.Placa, request.Serie, request.SubMarcaNueva, request.Marca, verificentro.Nombre, request.NombreGeneraCita, request.Correo),
                            Fecha = DateTime.Now,
                            IdEstatusFinal = null,
                            MovimientoFinal = null,
                            Leido = false,
                            Procesada = false
                        };

                        _context.Alerta.Add(alerta);
                        await _context.SaveChangesAsync();
                        transaction.Complete();
                        ressResponse.ErrorMessage = "No se encontraron los datos del vehiculo en los catalogos del sistema, se realizará una solicitud para actualizarlos pronto. Para más información comunicarse con Secretaría de Medio Ambiente, Desarrollo Sustentable y Ordenamiento Territorial  (222)273 68 00 EXT(1127)";
                        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                    }
                    // if (request.Fecha == null)
                    // {
                    //     ressResponse.ErrorMessage = $"Debe ingresar una fecha para el registro.";
                    //     return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    // }

                    var today = DateTime.Now.Date;
                    var otherDay = today.AddDays(1);
                    if (!_sinRestricciones)
                    {
                        var comprobarCupo = _context.vNumeroCitasHorarios.FirstOrDefault(x => x.IdCVV == request.IdCVV && x.Fecha == fechaCita);
                        var horarioConfigurador = _context.ConfiguradorCita.FirstOrDefault(x => x.IdCVV == request.IdCVV && x.Fecha == fechaCita.Date && x.Habilitado == true);

                        if (comprobarCupo != null && comprobarCupo.NumeroCitas >= comprobarCupo.Capacidad)
                        {
                            ressResponse.ErrorMessage = $"Ya no hay más cupos disponibles en este horario.";
                            return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                        }
                        if(horarioConfigurador == null)
                        {
                            ressResponse.ErrorMessage = $"No se han registrado citas en ese horario.";
                            return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                        }
                        //var citaCorreo = _context.CitaVerificacions.FirstOrDefault(x => x.Correo == request.Correo && (x.Cancelada == null || x.Cancelada == false) && x.Fecha >= today && x.Fecha < otherDay);
                        //if (citaCorreo != null && !_excepcionRegla)
                        //{
                        //    ressResponse.ErrorMessage = $"Ya se ha registrado una cita con el mismo correo para el mismo día. Por favor, utilice un correo diferente, folio: {citaCorreo.Folio}.";
                        //    return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                        //}

                    }
                    //else
                    //{
                    //    confirmacionAutomatica = true;
                    //}
                    var ahora = DateTime.Now;
                    var OtraCita = _context.CitaVerificacions.FirstOrDefault(x => x.Serie == request.Serie && (x.Cancelada == null || x.Cancelada == false) && x.Fecha >= ahora);
                    if (OtraCita != null)
                    {
                        ressResponse.ErrorMessage = $"El vehículo ya cuenta con una cita registrada, folio: {OtraCita.Folio}.";
                        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    }

                    OtraCita = _context.CitaVerificacions.FirstOrDefault(x => x.Serie == request.Serie && (x.Cancelada == null || x.Cancelada == false) && x.Fecha >= today && x.Fecha < otherDay);
                    if (OtraCita != null)
                    {
                        ressResponse.ErrorMessage = $"El vehículo ya cuenta con una cita registrada, folio: {OtraCita.Folio}.";
                        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    }

                    #region Reglas anteriores
                    //var citaProxima = _context.CitaVerificacions.FirstOrDefault(x => x.Placa == request.Placa && x.Serie == request.Serie && (x.Cancelada == null || x.Cancelada == false) && x.Fecha >= DateTime.Now);
                    //if (citaProxima != null)
                    //{
                    //    ressResponse.ErrorMessage = $"El vehículo ya cuenta con una cita registrada, folio: {citaProxima.Folio}.";
                    //    return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    //}

                    //var comprobarCitaPrevia = _context.CitaVerificacions.Any(x => x.Placa == request.Placa && (x.Fecha >= DateTime.Now.AddHours(-12) && x.Fecha <= DateTime.Now) && (x.Cancelada == null || x.Cancelada == false));

                    //if (comprobarCitaPrevia)
                    //{
                    //    ressResponse.ErrorMessage = $"Debe esperar 12 horas antes de registrar una nueva cita.";
                    //    return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    //}
                    #endregion
                    #region Se retiraron reglas para comprobar la tmaestra desde la cita
                    //Obtenemos el registro de la submarca del vehiculo para ver si hay algún registro en la t maestra
                    // var submarca = _context.CatSubMarcaVehiculos.FirstOrDefault(x => x.Id == request.IdSubMarca);
                    // if (submarca == null)
                    // {
                    //     ressResponse.ErrorMessage = $"Debe ingresar una fecha para el registro.";
                    //     return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                    // }
                    // IdCatMarcaVehiculo hace referencia al Id del registro de la t maestra para ubicar marca vehiculo y año, según lo que selecciono el usuario
                    //var parametrosTMaestra = _context.vTablaMaestras.Where(x => x.Id == request.IdSubMarca && request.Anio >= x.ANO_DESDE && request.Anio <= x.ANO_HASTA &&
                    //    (request.IdTipoCombustible == Combustible.Diesel ? (x.CERO_DSL == 1) :
                    //    (request.IdTipoCombustible == Combustible.Gasolina ? x.CERO_GASOL == 1 :
                    //    (request.IdTipoCombustible == Combustible.GasNat ? x.CERO_GASNC == 1 :
                    //    request.IdTipoCombustible == Combustible.GasLp ? x.CERO_GASLP == 1 : false))));
                    //if (!parametrosTMaestra.Any())
                    //{
                    //    var verificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == request.IdCVV && x.Activo);
                    //    var datosVehiculo = _context.vCatSubMarcaVehiculos.FirstOrDefault(x => x.Id == request.IdSubMarca);
                    //    if (verificentro != null)
                    //    {
                    //        var alerta = new Alertum
                    //        {
                    //            TableName = DictAlertas.Cita,
                    //            TableId = 0,
                    //            IdVerificentro = verificentro.Id,
                    //            Data = null,
                    //            IdUser = null,
                    //            IdEstatusInicial = 0,
                    //            MovimientoInicial = string.Format(MovimientosDicts.DictAlertaCita[CitaEstatus.SoloNotificacion], request.Placa, request.Serie, datosVehiculo?.Nombre, datosVehiculo?.Marca, verificentro.Nombre, request.NombreGeneraCita, request.Correo),
                    //            Fecha = DateTime.Now,
                    //            IdEstatusFinal = null,
                    //            MovimientoFinal = null,
                    //            Leido = false
                    //        };

                    //        _context.Alerta.Add(alerta);
                    //        await _context.SaveChangesAsync();
                    //        transaction.Complete();
                    //    }
                    //    ressResponse.ErrorMessage = "No se encontraron los datos del vehiculo en los catalogos del sistema, se realizará una solicitud para actualizarlos pronto. Para más información comunicarse con Secretaría de Medio Ambiente, Desarrollo Sustentable y Ordenamiento Territorial  (222)273 68 00 EXT(1127)";
                    //    return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    //}
                    #endregion

                    var idMarca = _context.vCatMarcaVehiculoNombres.FirstOrDefault(x => x.Nombre.Equals(request.Marca))?.Id ?? 0;

                    vCatSubMarcaVehiculo? datosMarca = _context.vCatSubMarcaVehiculos.FirstOrDefault(x => x.Id == request.IdSubMarca);
                    var registro = new CitaVerificacion
                    {
                        Nombre = request.Nombre,
                        NombreGeneraCita = request.NombreGeneraCita,
                        Correo = request.Correo,
                        Fecha = request.Fecha,
                        IdCVV = request.IdCVV,
                        Placa = request.Placa,
                        IdTipoCombustible = request.IdTipoCombustible,
                        Anio = request.Anio,
                        Acepto = request.Acepto,
                        Serie = request.Serie.Trim(),
                        Poblano = request.Poblano,
                        Estado = request.Poblano ? "PUEBLA" : request.ClaveEstado,
                        IdCatMarcaVehiculo = datosMarca == null ? idMarca : datosMarca.IdCatMarcaVehiculo,
                        IdSubMarcaVehiculo = 0,
                        ColorVehiculo = request.ColorVehiculo,
                        Folio = GenerarFolio(10),
                        UrlComprobante = string.Empty,
                        FechaCreacion = DateTime.Now
                        //Cancelada = !confirmacionAutomatica
                    };
                    if (request.EsPErsonaMoral)
                    {
                        registro.RazonSocial = request.Nombre;
                        registro.Nombre = null;
                    }
                    _context.CitaVerificacions.Add(registro);

                    var Guardado = _context.SaveChanges() > 0;



                    var result = Guardado;
                    var verif = _context.Verificentros.FirstOrDefault(x => x.Id == registro.IdCVV);
                    if (result)
                    {
                        if (verif != null)
                        {
                            var dataReport = new DataReport();
                            var data = new ComprobanteCitaResponse();
                            data.NombrePersona = registro.NombreGeneraCita;
                            data.Fecha = registro.Fecha;
                            data.NombreCentroVerificacion = verif.Nombre;
                            data.DireccionCentroVerificacion = verif.Direccion;
                            data.Folio = registro.Folio;
                            data.UrlWeb = _urlSite;

                            var getdoc = await _pdfBuilder.GetComprobanteCita(data);
                            var doc = getdoc.Response.DocumentoPDF;
                            var nomb = getdoc.Response.NombreDocumento;

                            dataReport.NombreDocumento = nomb;
                            dataReport.DocumentoPDF = doc;

                            var pdf = dataReport;

                            if (pdf.DocumentoPDF.Length > 0 && pdf.DocumentoPDF != null)
                            {
                                var stringB64 = Convert.ToBase64String(pdf.DocumentoPDF);

                                var url = _blobStorage.UploadFileAsync(new byte[0], "PortalCita/" + registro.Folio + "/" + pdf.NombreDocumento, stringB64).Result;
                                if (!string.IsNullOrEmpty(url))
                                {
                                    registro.UrlComprobante = url;
                                    _context.SaveChanges();
                                }
                            }

                            EmailMessage emailMessage = new EmailMessage();
                            EnvioCorreoSMTP envioCorreo = new EnvioCorreoSMTP();

                            string imageLogo = "";
                            imageLogo = !string.IsNullOrEmpty(request.Logo) ? request.Logo : "";

                            //var currentD = Directory.GetCurrentDirectory();
                            //var DirectorP = Directory.GetParent(currentD);
                            //string imageLogo = "";
                            //if (DirectorP != null)
                            //{
                            //    var directories = Directory.GetDirectories(DirectorP.FullName);
                            //    var directW = directories.FirstOrDefault(x => x.ToLower().Contains("web"));
                            //    var path = Path.Combine(directW, "wwwroot", "assets", "media", "logos", "smadot_logo_simple2.png");
                            //    if (File.Exists(path))
                            //    {
                            //        byte[] imageArray = System.IO.File.ReadAllBytes(path);
                            //        imageLogo = Convert.ToBase64String(imageArray);
                            //    }
                            //}

                            emailMessage.Subject = "Cita en Línea";

                            var options = new ZXing.Common.EncodingOptions
                            {
                                PureBarcode = true,
                                Width = 300,  // ajusta según tus necesidades
                                Height = 300, // ajusta según tus necesidades
                            };
                            // Crear un objeto BarcodeWriter
                            var barcodeWriter = new ZXing.ImageSharp.BarcodeWriter<SixLabors.ImageSharp.PixelFormats.Argb32>
                            {
                                // Establecer el formato del código de barras (por ejemplo, CODE_128)
                                Format = BarcodeFormat.QR_CODE,
                                Options = options

                            };
                            Image barcodeBitmap = barcodeWriter.Write(string.Format("{0}PortalCitas/ConsultaCita?folio={1}", _urlSite, data.Folio));
                            string ImageAsBase64 = ImageToBase64String(barcodeBitmap);
                            var urlQR = _blobStorage.UploadFileAsync(new byte[0], "PortalCita/" + registro.Folio + "/QR/" + "QR_" + registro.Folio  + ".png", ImageAsBase64).Result;


                            var urlConfirmarCita = string.Format("{0}PortalCitas/ConfirmarCita?folio={1}", _urlSite, data.Folio);

                            var contentMail = "<p style=\"\r\n    font-size: 15px;\r\n\">Estimado(a) <b style=\"\r\n    font-size: 16px;\r\n\">" + data.NombrePersona + ": </b></p>\r\n<p style=\"\r\n    font-size: 15px;\r\n\">Ha registrado una cita para el día <b>" + data.Fecha.ToString("D", CultureInfo.GetCultureInfo("es-ES")) + "</b>, a las <b>" + data.Fecha.ToString("t", CultureInfo.GetCultureInfo("es-ES")) + " hrs.</b></p>\r\n<p style=\"\r\n    font-size: 15px;\r\n\">Por lo cual le reiteramos debe presentarse en el <b>" + data.NombreCentroVerificacion.ToUpper() + "</b> en la fecha y hora antes mencionadas.</p>\r\n<hr>\r\n<p style=\"\r\n    font-size: 15px;\r\n\">Le recordamos que el <b>Folio</b> de su Cita es: </p>\r\n\r\n<p style=\"\r\n    font-size: 30px;\r\n    text-align-last: center;\r\n\"><b>" + data.Folio + "</b></p><div style=\"text-align-last:center\"><img alt=\"QR Code\" style=\"width:200px; height:200px\" src=\"" + urlQR + "\" /></div><hr><br>\r\n<p style=\"font-size: 15px\"><b>Notas: </b></p>\r\n<ul style=\"\r\n    font-size: 15px;\r\n\">\r\n    <li>Deberá llegar a su cita al menos 05 minutos antes</li>\r\n    <li>Presentarse con original y copia de todos los requisitos</li>\r\n    <li>Se recomienda que el vehículo llegue con la temperatura normal de operación para realizar la prueba</li>\r\n    <li>No es recomendable prender y apagar el vehículo mientras se encuentra en espera para pasar a la prueba</li>\r\n    <li>Se recomienda que los vehículos nuevos realicen la prueba con al menos 300 km recorridos</li>\r\n</ul><br><br>\r\n<div class=\"col-md-4 col-5\" style=\"text-align: center;\">\r\n<a href=\"" + urlConfirmarCita + "\">\r\n <button class=\"button\">Confirmar Cita</button>\r\n</a><br><br><br><br>";


                            String bodyCorreo = envioCorreo.BodyEmail(emailMessage.Subject, contentMail, imageLogo, null);

                            var destinatario = new List<string>
                            {
                                registro.Correo
                            };

                            //var sent = envioCorreo.Send(
                            //    destinatario,
                            //    emailMessage.Subject,
                            //    bodyCorreo,
                            //    _configuration["Correo:email"], _configuration["Correo:usuario"],
                            //    _configuration["Correo:contrasena"], _configuration["Correo:smtp"],
                            //    null, null, _configuration["Correo:puerto"], true
                            //    );

                            //SE COMENTA PORQUE LAS CREDENCIALES DE PROD NO FUNCIONAN
                            //if (!sent)
                            //{
                            //    ressResponse.ErrorMessage = $"El email no se logró enviar. Por favor inténtelo más tarde.";
                            //    return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                            //}


                            //if (sedm == "Correcto")
                            //{
                            //    _context.CircularVerificentros.Add(circVerificentro);
                            //    _context.SaveChanges();
                            //}
                        }


                    }

                    transaction.Complete();
                    //return new ResponseGeneric<bool>(result);
                    ressResponse.ErrorMessage = "";
                    ressResponse.FolioResult = registro.Folio;
                    return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                }
            }
            catch (Exception ex)
            {
                //return new ResponseGeneric<bool>(ex);
                return new ResponseGeneric<PortalCitasRegistroResponse>(ex);
            }
        }
        public async Task<ResponseGeneric<PortalCitasComprobanteResponse>> GetCitaData(long idCita)
        {
            try
            {
                var result = new PortalCitasComprobanteResponse();
                var cita = await _context.vHistorialCita.Select(x => new PortalCitasComprobanteResponse
                {
                    Id = x.IdCita,
                    NombrePersona = x.NombrePersona ?? x.RazonSocial,
                    Fecha = x.Fecha,
                    NombreCentroVerificacion = x.NombreCentro,
                    DireccionCentroVerificacion = x.Direccion,
                    Folio = x.Folio,
                    Cancelada = x.Cancelada ?? false,
                    UrlComprobante = x.UrlComprobante,
                    Placa = x.Placa,
                    Serie = x.Serie,
                    Anio = x.Anio,
                    Marca = x.Marca,
                    Modelo = x.Modelo,
                    Poblano = x.Poblano,
                    Estado = x.Estado
                }).FirstOrDefaultAsync(x => x.Id == idCita);

                if (cita == null)
                {
                    cita = new PortalCitasComprobanteResponse();
                }
                return new ResponseGeneric<PortalCitasComprobanteResponse>(cita);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<PortalCitasComprobanteResponse>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<PortalCitasComprobanteResponse>(ex) { mensaje = "Ocurrió un error al obtener la información" };
            }
        }
        public async Task<ResponseGeneric<PortalCitasComprobanteResponse>> GetCitaData(string folio)
        {
            try
            {
                var currentDate = DateTime.Now.Date;
                var fechaInicio = DateTime.Now.Date.AddDays(-1);
                var fechaFinal = currentDate.AddDays(16);
                var result = new PortalCitasComprobanteResponse();
                var citas = await _context.vHistorialCita.Select(x => new PortalCitasComprobanteResponse
                {
                    Id = x.IdCita,
                    NombrePersona = x.NombrePersona ?? x.RazonSocial,
                    Fecha = x.Fecha,
                    NombreCentroVerificacion = x.NombreCentro,
                    DireccionCentroVerificacion = x.Direccion,
                    Folio = x.Folio,
                    Cancelada = x.Cancelada ?? false,
                    UrlComprobante = x.UrlComprobante,
                    Placa = x.Placa,
                    Serie = x.Serie,
                    Anio = x.Anio,
                    Marca = x.Marca,
                    Modelo = x.Modelo,
                    Poblano = x.Poblano,
                    Estado = x.Estado
                }).Where(x => (x.Folio.Equals(folio) || x.Serie.Equals(folio)) && x.Fecha >= fechaInicio && x.Fecha <= fechaFinal).ToListAsync();

                var cita = new PortalCitasComprobanteResponse();
                if (citas.Count > 0)
                {
                    cita = citas[citas.Count - 1];
                }
                // if (cita == null)
                // {
                //     cita = new PortalCitasComprobanteResponse();

                // }
                return new ResponseGeneric<PortalCitasComprobanteResponse>(cita);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<PortalCitasComprobanteResponse>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<PortalCitasComprobanteResponse>(ex) { mensaje = "Ocurrió un error al obtener la información" };
            }
        }

        public async Task<ResponseGeneric<bool>> CancelarCita(PortalCitaCancelarRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var cita = _context.CitaVerificacions.FirstOrDefault(x => x.Id == request.Id && x.Folio == request.Folio) ?? throw new ValidationException("No se encontro información de la cita.");

                    cita.Cancelada = true;
                    cita.FechaCancelacion = DateTime.Now;
                    cita.IdCatMotivoCancelacionCita = CatMotivoCancelacionCita.CanceladoPorElUsuario;
                    // cita.IdUserCancelo = _userResolver.GetUser().IdUser;


                    if (request.ErrorTablaMaestra ?? false)
                    {
                        var verificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == cita.IdCVV && x.Activo);
                        var datosVehiculo = _context.vCatSubMarcaVehiculos.FirstOrDefault(x => x.Id == cita.IdSubMarcaVehiculo && x.IdCatMarcaVehiculo == cita.IdCatMarcaVehiculo);
                        if (verificentro != null)
                        {
                            var alerta = new Alertum
                            {
                                TableName = DictAlertas.Cita,
                                TableId = 0,
                                IdVerificentro = cita.IdCVV,
                                Data = JsonConvert.SerializeObject(cita, new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                                    NullValueHandling = NullValueHandling.Ignore
                                }),
                                IdUser = null,
                                IdEstatusInicial = 0,
                                MovimientoInicial = string.Format(MovimientosDicts.DictAlertaCita[CitaEstatus.SoloNotificacion], cita.Placa, cita.Serie, datosVehiculo?.Nombre, datosVehiculo?.Marca, verificentro.Nombre, cita.NombreGeneraCita, cita.Correo),
                                Fecha = DateTime.Now,
                                IdEstatusFinal = null,
                                MovimientoFinal = null,
                                Leido = false,
                                Procesada = false
                            };
                            _context.Alerta.Add(alerta);
                        }
                    }
                    var eliminado = await _context.SaveChangesAsync() > 0;

                    var result = eliminado;
                    transaction.Complete();
                    return new ResponseGeneric<bool>(result);

                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> ReiniciarCita(ReiniciarCitaRequest request)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var cita = _context.CitaVerificacions.FirstOrDefault(x => x.Id == request.IdCita) ?? throw new ValidationException("No sé encontró una cita.");
                    // _context.CitaVerificacions.Remove(cita);
                    var idUser = _userResolver.GetUser().IdUser;
                    cita.FechaReinicio = DateTime.Now;
                    cita.IdUserReinicio = idUser;

                    var docs = _context.DocumentosCita.FirstOrDefault(x => x.IdCitaVerificacion == request.IdCita);
                    if (docs != null)
                        _context.DocumentosCita.Remove(docs);

                    var ver = _context.Verificacions.Include(v => v.ParametrosTablaMaestraVerificacion).Include(p => p.ResultadosVerificacion).Include(x => x.LimiteVerificacionParametro)
                    .FirstOrDefault(x => x.IdCitaVerificacion == request.IdCita);
                    //var idVer = ver?.Id ?? 0;
                    if (ver?.ResultadosVerificacion?.EstatusPrueba > EstatusVerificacion.VerificacionFinalizada)
                    {
                        return new ResponseGeneric<long>($"El certificado ya fue impreso. En caso de existir algún error, se tendrá que cancelar y realizar de nuevo el proceso de verificación.", true) { Status = ResponseStatus.Failed, mensaje = $"El certificado ya fue impreso. En caso de existir algún error, se tendrá que cancelar y realizar de nuevo el proceso de verificación." };
                        //throw new ValidationException("El certificado ya fue impreso. En caso de existir algún error, se tendrá que cancelar y realizar de nuevo el proceso de verificación.");
                    }
                    var folios = _context.FoliosFormaValoradaVerificentros.Where(x => x.IdVerificacion == request.IdVerificacion);
                    foreach (var folio in folios)
                    {
                        if (folio.Impreso)
                        {
                            var count = _context.FoliosFormaValoradaVerificentros.Where(x => x.Cancelado && x.IdCatTipoTramite != null).Count() + 1;
                            folio.FechaCancelacion = DateTime.Now;
                            folio.IdCatMotivoCancelacion = 3;
                            folio.OtroMotivo = $"Cancelado por reinicio de verificación realizada por el usuario. {folio.IdVerificacion}";
                            folio.Cancelado = true;
                            folio.IdVerificacion = null;
                            folio.ClaveTramiteCancelado = $"{TipoTramite.Dict[TipoTramite.CANCELADO]}";
                            folio.IdUserCancelo = idUser;
                            folio.ConsecutivoTramiteCancelado = count;
                        }
                        else
                        {
                            _context.FoliosFormaValoradaVerificentros.Remove(folio);
                        }
                    }
                    await _context.SaveChangesAsync();
                    if (ver?.ParametrosTablaMaestraVerificacion != null)
                        _context.ParametrosTablaMaestraVerificacions.Remove(ver.ParametrosTablaMaestraVerificacion);
                    if (ver?.LimiteVerificacionParametro != null)
                    {
                        _context.LimiteVerificacionParametros.Remove(ver.LimiteVerificacionParametro);
                    }
                    if (ver?.ResultadosVerificacion != null)
                    {
                        _context.ResultadosVerificacions.Remove(ver.ResultadosVerificacion);
                        if (ver.ResultadosVerificacion.EstatusPrueba >= EstatusVerificacion.TerminaPruebaVisual)
                        {
                            var verificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Id == cita.IdCVV);// Obtenermos los datos del verificentro
                            var eliminarPruebaServidor = await EliminarPruebaServidor(verificentro?.ApiEndPoint ?? "", verificentro?.ApiKey ?? "", verificentro?.Nombre ?? "", ver.Id); // Intentamos eliminar la prueba del servidor
                            if (eliminarPruebaServidor.Status == ResponseStatus.Failed)
                            {
                                throw new ValidationException(eliminarPruebaServidor.mensaje);

                            }
                        }

                    }
                    if (ver != null)
                        _context.Verificacions.Remove(ver);

                    var result = await _context.SaveChangesAsync() > 0;

                    scope.Complete();
                    return result ? new ResponseGeneric<long>(1) : new ResponseGeneric<long>();
                }
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<long>(ex.Message);
            }
            catch (Exception ex)
            {
                
                return new ResponseGeneric<long>(new Exception("Ocurrió un error al reiniciar la información para su recaptura.")) { mensaje = "Ocurrió un error al reiniciar la información para su recaptura." };
            }
        }

        public async Task<ResponseGeneric<Response>> ConfirmarCita(ConfirmarCita request)
        {
            try
            {
                var respuesta = new Response();
                var actualizado = false;
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var cita = _context.CitaVerificacions.FirstOrDefault(x => x.Folio == request.Folio) ?? throw new ValidationException("No se encontro información de la cita.");

                    DateTime fechaExpiracion = cita.FechaCreacion.Value.AddDays(1).ToUniversalTime();
                    DateTime fechaActual = DateTime.UtcNow;


                    if (fechaActual >= fechaExpiracion)
                    {
                        respuesta.Status = ResponseStatus.Failed;
                        respuesta.respuesta = "Lo sentimos, la cita esta  fuera de tiempo ya no posible Confirmarla, es necesario agendar una nueva cita";
                        return new ResponseGeneric<Response>(respuesta);
                    }

                    cita.Cancelada = false;
                    _context.CitaVerificacions.Update(cita);
                    actualizado = await _context.SaveChangesAsync() > 0;
                    transaction.Complete();

                    respuesta.Status = ResponseStatus.Success;
                    respuesta.respuesta = "¡La cita fue confirmada con Éxito!";
                    return new ResponseGeneric<Response>(respuesta);

                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<Response>(ex);
            }
        }

        #region private methods

        private List<string> GetTimeSlots(TimeSpan startTime, TimeSpan endTime, int intervalMinutes, bool today)
        {
            var timeSlots = new List<string>();
            var currentTime = startTime;

            while (currentTime < endTime)
            {
                if (!today)
                {
                    timeSlots.Add(currentTime.ToString(@"hh\:mm"));
                }
                else if (currentTime >= DateTime.Now.TimeOfDay)
                {
                    timeSlots.Add(currentTime.ToString(@"hh\:mm"));

                }
                currentTime = currentTime.Add(TimeSpan.FromMinutes(intervalMinutes));
            }

            return timeSlots;
        }
        private string GenerarFolio(int length)
        {
            Random random = new Random();

            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(characters, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private async Task<ResponseGeneric<string>> EliminarPruebaServidor(string ApiEndPoint, string ApiKey, string verificentro, long IdPrueba)
        {
            try
            {
                var serviceVerificentro = new ServicioEventosVerificentro(ApiEndPoint, ApiKey);
                var texto = $"La solicitud al servicio del {verificentro} excedió el tiempo de espera.";
                var requestService = new EliminarPruebaRequest { IdPrueba = IdPrueba };

                var responseService = await serviceVerificentro.DeleteAsync<object, EliminarPruebaRequest>("RecepcionEventos/EliminarPrueba", verificentro, requestService, texto);
                if (responseService.Status != ResponseStatus.Success)
                {
                    return new ResponseGeneric<string>() { mensaje = responseService.mensaje, CurrentException = responseService.CurrentException, Status = ResponseStatus.Failed };
                }
                return new ResponseGeneric<string>("", true);

            }
            catch (Exception)
            {

                return new ResponseGeneric<string>("Ocurrió un error al contactar el servicio", true);
            }
        }

        //private async Task<ResponseGeneric<string>> GenerarImpresionCita(bool Guardado, long IdCVV)
        //{
        //    var registro = new CitaVerificacion();
        //    var verif = _context.Verificentros.FirstOrDefault(x => x.Id == registro.IdCVV);
        //    var response = new ResponseGeneric<string>();

        //    if (Guardado)
        //    {
        //        if (verif != null)
        //        {
        //            var dataReport = new DataReport();
        //            var data = new ComprobanteCitaResponse
        //            {
        //                NombrePersona = registro.NombreGeneraCita,
        //                Fecha = registro.Fecha,
        //                NombreCentroVerificacion = verif.Nombre,
        //                DireccionCentroVerificacion = verif.Direccion,
        //                Folio = registro.Folio,
        //                UrlWeb = _urlSite
        //            };


        //            var getdoc = await _pdfBuilder.GetComprobanteCita(data);
        //            var doc = getdoc.Response.DocumentoPDF;
        //            var nomb = getdoc.Response.NombreDocumento;

        //            dataReport.NombreDocumento = nomb;
        //            dataReport.DocumentoPDF = doc;

        //            var pdf = dataReport;

        //            if (pdf.DocumentoPDF.Length > 0 && pdf.DocumentoPDF != null)
        //            {
        //                byte[] pdfBytes = pdf.DocumentoPDF;

        //                var stringB64 = Convert.ToBase64String(pdfBytes);

        //                var url = $"data:application/pdf;base64,{stringB64}";
        //                if (!string.IsNullOrEmpty(url))
        //                {
        //                    registro.UrlComprobante = url;
        //                    await _context.SaveChangesAsync();
        //                }


        //            }
        //        }

        //    }
        //}

        static string ImageToBase64String(SixLabors.ImageSharp.Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.SaveAsPng(memoryStream); // SaveAsPng se utiliza directamente en ImageSharp
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
        #endregion
    }


    public interface IPortalCitaNegocio
    {
        public Task<ResponseGeneric<PortalCitasResponse>> GetPortalCitasByIdCvv(long id);
        public Task<ResponseGeneric<PortalCitasRegistroResponse>> RegistroCita(PortalCitaRequest request);
        public Task<ResponseGeneric<PortalCitasComprobanteResponse>> GetCitaData(long IdCita);
        public Task<ResponseGeneric<PortalCitasComprobanteResponse>> GetCitaData(string folio);
        public Task<ResponseGeneric<ResponseGrid<HistorialCitasResponse>>> Consulta(CitaGridRequest request);
        public Task<ResponseGeneric<bool>> CancelarCita(PortalCitaCancelarRequest request);
        public Task<ResponseGeneric<long>> ReiniciarCita(ReiniciarCitaRequest request);
        public Task<ResponseGeneric<Response>> ConfirmarCita(ConfirmarCita request);
    }
}
