using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Response;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.SolicitudFormaValorada.Request;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.Entities.AcreditacionTecnicoVerificador.Request;
using System.ComponentModel.DataAnnotations;
using Smadot.Models.Dicts;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
    public class AcreditacionTecnicoVerificadorNegocio : IAcreditacionTecnicoVerificadorNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        public AcreditacionTecnicoVerificadorNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
        }

        public async Task<ResponseGeneric<ResponseGrid<AcreditacionTecnicoVerificadorGridResponse>>> Consulta(SolicitudFormaValoradaListRequest request)
        {
            try
            {
                //var solicitudes = _context.AceditacionTecnicoSolicituds.AsQueryable();
                var user = _userResolver.GetUser();

                var solicitudes = _context.vAcreditacionTecnicoSolicituds.Where(x => x.IdVerificentro == user.IdVerificentro);

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    solicitudes = solicitudes.Where(x => x.NumeroSolicitud.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var tot = solicitudes.Count();
                var filtered = string.IsNullOrEmpty(request.Busqueda) ? tot : tot;

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    solicitudes = solicitudes.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    solicitudes = solicitudes.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                DateTime now = DateTime.Now;
                var result = new ResponseGrid<AcreditacionTecnicoVerificadorGridResponse>
                {
                    RecordsTotal = tot,
                    RecordsFiltered = filtered,
                    Data = JsonConvert.DeserializeObject<List<AcreditacionTecnicoVerificadorGridResponse>>(JsonConvert.SerializeObject(await solicitudes.ToListAsync()))
                };
                return new ResponseGeneric<ResponseGrid<AcreditacionTecnicoVerificadorGridResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<AcreditacionTecnicoVerificadorGridResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<string>> RegistroSolicitud(AcreditacionTecnicoSolicitudRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (request.Id > 0)
                    {
                        var actecsol = _context.AceditacionTecnicoSolicituds.FirstOrDefault(x => x.Id == request.Id);

                        if (actecsol == null)
                            return new ResponseGeneric<string>($"No se encontró información.", true);
                        //throw new ValidationException("No se encontró información.");

                        if (string.IsNullOrEmpty(request.NumeroSolicitud))
                            return new ResponseGeneric<string>($"El número de solicitud no puede ir vacío.", true);
                        //throw new ValidationException("El número de solicitud no puede ir vacío.");

                        var numerosolicitud =
                            _context.AceditacionTecnicoSolicituds.Any(x => x.NumeroSolicitud == request.NumeroSolicitud && x.Id != request.Id);

                        if (numerosolicitud)
                            return new ResponseGeneric<string>($"El número de solicitud ya ha sido registrado.", true);
                        //throw new ValidationException("El número de solicitud ya ha sido registrado.");

                        actecsol.NumeroSolicitud = request.NumeroSolicitud;

                        var acredTecnicoSolicitudGuardado = _context.SaveChanges() > 0;

                        if (request.Empleados == null || request.Empleados.Count() == 0)
                            return new ResponseGeneric<string>($"Debe seleccionar al menos un empleado.", true);
                        //throw new ValidationException("Debe seleccionar al menos un empleado.");

                        var previousEmpl = false;
                        if (actecsol.IdUserPuestoVerificentros.Count() > 0)
                        {
                            foreach (var item in actecsol.IdUserPuestoVerificentros)
                            {
                                actecsol.IdUserPuestoVerificentros.Remove(item);
                            }

                        }
                        previousEmpl = _context.SaveChanges() is 7;
                        foreach (var emp in request.Empleados)
                        {
                            var userempl = _context.UserPuestoVerificentros.FirstOrDefault(x => x.Id == emp);
                            if (userempl == null)
                            {
                                return new ResponseGeneric<string>($"No se encontro el personal.", true);
                                //throw new ValidationException("No se encontro el personal.");
                            }
                            actecsol.IdUserPuestoVerificentros.Add(userempl);
                        }

                        var empleadosGuardados = _context.SaveChanges() is 7;

                        var result = acredTecnicoSolicitudGuardado && empleadosGuardados && previousEmpl;

                        transaction.Complete();
                        //return new ResponseGeneric<bool>(result);
                        return new ResponseGeneric<string>("", true);

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(request.NumeroSolicitud))
                            return new ResponseGeneric<string>($"El número de solicitud no puede ir vacío.", true);
                        //throw new ValidationException("El número de solicitud no puede ir vacío.");

                        var numerosolicitud = _context.AceditacionTecnicoSolicituds.Any(x => x.NumeroSolicitud == request.NumeroSolicitud);

                        if (numerosolicitud)
                            return new ResponseGeneric<string>($"El número de solicitud ya ha sido registrado.", true);
                        //throw new ValidationException("El número de solicitud ya ha sido registrado.");

                        var reg = JsonConvert.DeserializeObject<List<long>>(request.EmpleadosString);
                        if (reg.Count() > 0 && reg != null)
                        {
                            request.Empleados = reg;
                        }

                        var acredTecnicoSolicitud = new AceditacionTecnicoSolicitud
                        {
                            IdVerificentro = _userResolver.GetUser().IdVerificentro,
                            NumeroSolicitud = request.NumeroSolicitud,
                            IdCatEstatusAcreditacion = EstatusAcreditacion.SolicitarAutorización,
                        };

                        _context.AceditacionTecnicoSolicituds.Add(acredTecnicoSolicitud);

                        var acredTecnicoSolicitudGuardado = _context.SaveChanges() > 0;

                        if (acredTecnicoSolicitud.Id == 0)
                            return new ResponseGeneric<string>($"No se pudo registrar la información.", true);
                        //throw new ValidationException("No se pudo registrar la información.");

                        if (request.Empleados == null || request.Empleados.Count() == 0)
                            return new ResponseGeneric<string>($"Debe seleccionar al menos un empleado.", true);
                        //throw new ValidationException("Debe seleccionar al menos un empleado.");

                        foreach (var emp in request.Empleados)
                        {
                            var userempl = _context.UserPuestoVerificentros.FirstOrDefault(x => x.Id == emp);
                            if (userempl == null)
                            {
                                return new ResponseGeneric<string>($"No se encontro el personal.", true);
                                //throw new ValidationException("No se encontro el personal."); 
                            }
                            acredTecnicoSolicitud.IdUserPuestoVerificentros.Add(userempl);
                        }

                        var empleadosGuardados = _context.SaveChanges() is 7;

                        var result = acredTecnicoSolicitudGuardado && empleadosGuardados;

                        transaction.Complete();
                        //return new ResponseGeneric<bool>(result);
                        return new ResponseGeneric<string>("", true);
                    }
                }
            }
            catch (Exception ex)
            {
                //return new ResponseGeneric<bool>(ex);
                return new ResponseGeneric<string>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> RegistroEvidencia(AcreditacionTecnicoEvidenciaRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (request.Id > 0)
                    {
                        var actecevid = _context.AcreditacionTecnicoEvidencia.Include(x => x.AcreditacionTecnicoEvidenciaPuestoVerificentros).FirstOrDefault(x => x.Id == request.Id);

                        if (actecevid == null)
                            throw new ValidationException("No se encontró información.");

                        if (string.IsNullOrEmpty(request.NumeroSolicitud))
                            throw new ValidationException("Debe ingresar el número de solicitud.");

                        if (string.IsNullOrEmpty(request.TipoTramite))
                            throw new ValidationException("Debe ingresar el tipo de tramite.");

                        if (string.IsNullOrEmpty(request.NumeroAcreditacion))
                            throw new ValidationException("Debe ingresar el número de acreditación.");

                        if (string.IsNullOrEmpty(request.NumeroReferencia))
                            throw new ValidationException("Debe ingresar el número de referencia.");

                        var hoy = DateTime.Now;

                        actecevid.TipoTramite = request.TipoTramite;
                        actecevid.NumeroAcreditacion = request.NumeroAcreditacion;
                        actecevid.NumeroReferencia = request.NumeroReferencia;
                        actecevid.FechaAcreditacion = request.FechaAcreditacion;
                        actecevid.FechaAmpliacion = request.FechaAmpliacion;
                        actecevid.FechaEmision = request.FechaEmision;
                        actecevid.FechaModificacion = hoy;

                        var acredTecnicoEvidenciaGuardado = _context.SaveChanges() > 0;

                        //if (request.UrlAcreditacion != null)
                        //{
                        //    var file = request.UrlAcreditacion;
                        //    var url = await _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoEvidencia/" + actecevid.Id + "/" + file.Nombre, file.Base64);
                        //    actecevid.UrlAcreditacion = url;
                        //}
                        if (request.UrlAcreditacion != null && request.UrlAcreditacion.Count() > 0)
                        {
                            //var file = request.UrlAprobacion;
                            //var url = await _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoSolicitud/" + acreditacion.Id + "/" + file.Nombre, file.Base64);
                            //acreditacion.UrlAprobacion = url;
                            foreach (var file in request.UrlAcreditacion)
                            {
                                var url = _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoEvidencia/" + actecevid.Id + "/" + file.Nombre, file.Base64).Result;
                                if (!string.IsNullOrEmpty(url))
                                {
                                    actecevid.UrlAcreditacion = url; break;
                                }
                            }
                        }
                        _context.SaveChanges();

                        //var previousEmpl = false;
                        //if (actecevid.AcreditacionTecnicoEvidenciaPuestoVerificentros.Count() > 0)
                        //{
                        //    foreach (var item in actecevid.AcreditacionTecnicoEvidenciaPuestoVerificentros)
                        //    {
                        //        actecevid.AcreditacionTecnicoEvidenciaPuestoVerificentros.Remove(item);
                        //    }

                        //}
                        //previousEmpl = _context.SaveChanges() is 7;

                        var empleadosEvidenciaGuardados = false;
                        if (request.EvidenciaEmpleados.Count() > 0 && request.EvidenciaEmpleados != null)
                        {
                            var List = new List<AcreditacionTecnicoEvidenciaPuestoVerificentro>();
                            foreach (var item in request.EvidenciaEmpleados)
                            {
                                var empleado = _context.AcreditacionTecnicoEvidenciaPuestoVerificentros
                                    .FirstOrDefault(x => x.IdUserPuestoVerificentro == item.IdUserPuesto
                                        && x.IdAcreditacionTecnicoEvidencia == actecevid.Id);

                                if (empleado != null)
                                {
                                    empleado.IdTipoAcreditacion = item.TipoAcreditacion.Value;
                                    empleado.IdNormaAcreditacion = item.NormaAcreditacion == 0 ? null : item.NormaAcreditacion;
                                    empleado.Eliminado = item.Eliminado;
                                }
                                else
                                {
                                    var newItem = new AcreditacionTecnicoEvidenciaPuestoVerificentro
                                    {
                                        IdAcreditacionTecnicoEvidencia = actecevid.Id,
                                        IdUserPuestoVerificentro = item.IdUserPuesto,
                                        IdTipoAcreditacion = item.TipoAcreditacion.Value,
                                        IdNormaAcreditacion = item.NormaAcreditacion,
                                        Eliminado = item.Eliminado
                                    };
                                    List.Add(newItem);
                                }
                                //var newItem = new AcreditacionTecnicoEvidenciaPuestoVerificentro
                                //{
                                //    IdAcreditacionTecnicoEvidencia = actecevid.Id,
                                //    IdUserPuestoVerificentro = item.IdUserPuesto,
                                //    IdTipoAcreditacion = item.TipoAcreditacion.Value,
                                //    IdNormaAcreditacion = item.NormaAcreditacion,
                                //    Eliminado = item.Eliminado
                                //};
                                //List.Add(newItem);
                            }
                            if (List.Count() > 0)
                            {
                                _context.AcreditacionTecnicoEvidenciaPuestoVerificentros.AddRange(List);
                            }

                        }

                        empleadosEvidenciaGuardados = _context.SaveChanges() > 0;

                        var result = acredTecnicoEvidenciaGuardado && empleadosEvidenciaGuardados;

                        transaction.Complete();
                        return new ResponseGeneric<bool>(result);


                    }
                    else
                    {
                        if (string.IsNullOrEmpty(request.NumeroSolicitud))
                            throw new ValidationException("Debe ingresar el número de solicitud.");

                        if (string.IsNullOrEmpty(request.TipoTramite))
                            throw new ValidationException("Debe ingresar el tipo de tramite.");

                        if (string.IsNullOrEmpty(request.NumeroAcreditacion))
                            throw new ValidationException("Debe ingresar el número de acreditación.");

                        if (string.IsNullOrEmpty(request.NumeroReferencia))
                            throw new ValidationException("Debe ingresar el número de referencia.");

                        var hoy = DateTime.Now;
                        var acredTecnicoEvidencia = new AcreditacionTecnicoEvidencium
                        {
                            NumeroSolicitud = request.NumeroSolicitud,
                            TipoTramite = request.TipoTramite,
                            NumeroAcreditacion = request.NumeroAcreditacion,
                            NumeroReferencia = request.NumeroReferencia,
                            FechaAcreditacion = request.FechaAcreditacion,
                            FechaAmpliacion = request.FechaAmpliacion,
                            FechaEmision = request.FechaEmision,
                            FechaModificacion = hoy,
                            UrlAcreditacion = null,
                            IdCatEstatusAcreditacion = EstatusAcreditacion.SolicitarAutorización
                        };

                        _context.AcreditacionTecnicoEvidencia.Add(acredTecnicoEvidencia);

                        var acredTecnicoEvidenciaGuardado = _context.SaveChanges() > 0;

                        if (acredTecnicoEvidencia.Id == 0)
                            throw new ValidationException("No se pudo registrar la información.");

                        //if (request.UrlAcreditacion != null)
                        //{
                        //    var file = request.UrlAcreditacion;
                        //    var url = await _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoEvidencia/" + acredTecnicoEvidencia.Id + "/" + file.Nombre, file.Base64);
                        //    acredTecnicoEvidencia.UrlAcreditacion = url;
                        //}
                        if (request.UrlAcreditacion != null && request.UrlAcreditacion.Count() > 0)
                        {
                            //var file = request.UrlAprobacion;
                            //var url = await _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoSolicitud/" + acreditacion.Id + "/" + file.Nombre, file.Base64);
                            //acreditacion.UrlAprobacion = url;
                            foreach (var file in request.UrlAcreditacion)
                            {
                                var url = _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoEvidencia/" + acredTecnicoEvidencia.Id + "/" + file.Nombre, file.Base64).Result;
                                if (!string.IsNullOrEmpty(url))
                                {
                                    acredTecnicoEvidencia.UrlAcreditacion = url; break;
                                }
                            }
                        }

                        _context.SaveChanges();

                        var empleadosEvidenciaGuardados = false;
                        if (request.EvidenciaEmpleados.Count() > 0 && request.EvidenciaEmpleados != null)
                        {
                            var List = new List<AcreditacionTecnicoEvidenciaPuestoVerificentro>();
                            foreach (var item in request.EvidenciaEmpleados)
                            {
                                var newItem = new AcreditacionTecnicoEvidenciaPuestoVerificentro
                                {
                                    IdAcreditacionTecnicoEvidencia = acredTecnicoEvidencia.Id,
                                    IdUserPuestoVerificentro = item.IdUserPuesto,
                                    IdTipoAcreditacion = item.TipoAcreditacion.Value,
                                    IdNormaAcreditacion = item.NormaAcreditacion,
                                    Eliminado = item.Eliminado
                                };
                                List.Add(newItem);
                            }
                            _context.AcreditacionTecnicoEvidenciaPuestoVerificentros.AddRange(List);
                        }

                        empleadosEvidenciaGuardados = _context.SaveChanges() > 0;

                        var result = acredTecnicoEvidenciaGuardado && empleadosEvidenciaGuardados;

                        transaction.Complete();
                        return new ResponseGeneric<bool>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> AutorizarAcreditacion(AutorizarAcreditacionRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var acreditacion = _context.AceditacionTecnicoSolicituds.FirstOrDefault(x => x.Id == request.Id);

                    if (acreditacion == null)
                        throw new ValidationException("No se encontró información.");

                    if (request.UrlAprobacion != null && request.UrlAprobacion.Count() > 0)
                    {
                        //var file = request.UrlAprobacion;
                        //var url = await _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoSolicitud/" + acreditacion.Id + "/" + file.Nombre, file.Base64);
                        //acreditacion.UrlAprobacion = url;
                        foreach (var file in request.UrlAprobacion)
                        {
                            var url = _blobStorage.UploadFileAsync(new byte[0], "AcreditacionTecnicoSolicitud/" + acreditacion.Id + "/" + file.Nombre, file.Base64).Result;
                            if (!string.IsNullOrEmpty(url))
                            {
                                acreditacion.UrlAprobacion = url; break;
                            }
                        }
                    }

                    if (request.Estatus)
                    {
                        acreditacion.IdCatEstatusAcreditacion = EstatusAcreditacion.ApruebaDVRF;
                    }
                    else
                    {
                        acreditacion.IdCatEstatusAcreditacion = EstatusAcreditacion.RechazadoDVRF;
                    }

                    var fileGuardado = _context.SaveChanges() > 0;

                    var result = fileGuardado;

                    transaction.Complete();

                    return new ResponseGeneric<bool>(result);

                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<AcreditacionTecnicoVerificadorGridResponse>> GetById(long Id)
        {
            try
            {
                var result = new AcreditacionTecnicoVerificadorGridResponse();
                if (Id > 0)
                {
                    var acreditacion = _context.AceditacionTecnicoSolicituds.Include(x => x.IdCatEstatusAcreditacionNavigation)
                        .Include(x => x.IdUserPuestoVerificentros).ThenInclude(x => x.IdUserNavigation).FirstOrDefault(x => x.Id == Id);
                    //var acreditacion = _context.AceditacionTecnicoSolicituds.Include(x => x.IdCatEstatusAcreditacionNavigation)
                    //    .FirstOrDefault(x => x.Id == Id);
                    //var acreditacion = _context.AceditacionTecnicoSolicituds.FirstOrDefault(x => x.Id == Id);
                    //if (acreditacion != null)
                    //{
                    //    string r = JsonConvert.SerializeObject(acreditacion);
                    //    result = JsonConvert.DeserializeObject<AcreditacionTecnicoVerificadorGridResponse>(r);
                    //}
                    //var result = new AcreditacionTecnicoVerificadorGridResponse
                    //{
                    result.Id = acreditacion.Id;
                    result.IdCatEstatusAcreditacion = acreditacion.IdCatEstatusAcreditacion;
                    result.CatEstatus = acreditacion.IdCatEstatusAcreditacionNavigation.Nombre;
                    //result.IdCatEstatusAcreditacionNavigation = acreditacion.IdCatEstatusAcreditacionNavigation;
                    result.ListIdUserPuestoVerificentros = acreditacion.IdUserPuestoVerificentros.Select(x => new EmpleadoDataResponse
                    {
                        Id = x.Id,
                        Nombre = x.IdUserNavigation.Nombre
                    }).ToList();
                    result.NumeroSolicitud = acreditacion.NumeroSolicitud;
                    result.UrlAprobacion = acreditacion.UrlAprobacion;
                    //};
                }
                else
                {
                    throw new Exception("No sé encontró información.");
                }
                return new ResponseGeneric<AcreditacionTecnicoVerificadorGridResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<AcreditacionTecnicoVerificadorGridResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<AcreditacionTecnicoEvidenciaResponse>> GetEvidenciaBySolicitud(string solicitud)
        {
            try
            {
                var result = new AcreditacionTecnicoVerificadorGridResponse();
                var resp = new AcreditacionTecnicoEvidenciaResponse();
                if (!string.IsNullOrEmpty(solicitud))
                {
                    var acreditacion = _context.AceditacionTecnicoSolicituds.Include(x => x.IdCatEstatusAcreditacionNavigation)
                        .Include(x => x.IdUserPuestoVerificentros).ThenInclude(x => x.IdUserNavigation).FirstOrDefault(x => x.NumeroSolicitud == solicitud);

                    if (acreditacion == null)
                        throw new Exception("No sé encontró información.");

                    var evidencia = _context.AcreditacionTecnicoEvidencia
                        .Include(x => x.AcreditacionTecnicoEvidenciaPuestoVerificentros).ThenInclude(x => x.IdUserPuestoVerificentroNavigation)
                        .ThenInclude(x => x.IdUserNavigation).FirstOrDefault(x => x.NumeroSolicitud == acreditacion.NumeroSolicitud);
                    //var acreditacion = _context.AceditacionTecnicoSolicituds.Include(x => x.IdCatEstatusAcreditacionNavigation)
                    //    .FirstOrDefault(x => x.Id == Id);
                    //var acreditacion = _context.AceditacionTecnicoSolicituds.FirstOrDefault(x = > x.Id == Id);
                    //if (acreditacion != null)
                    //{
                    //    string r = JsonConvert.SerializeObject(acreditacion);
                    //    result = JsonConvert.DeserializeObject<AcreditacionTecnicoVerificadorGridResponse>(r);
                    //}
                    //var result = new AcreditacionTecnicoVerificadorGridResponse
                    //{
                    resp.EstatusAcreditacionSolicitud = acreditacion.IdCatEstatusAcreditacion;
                    if (evidencia == null)
                    {
                        var hoy = DateTime.Now;
                        resp.NumeroSolicitud = acreditacion.NumeroSolicitud;
                        resp.FechaAcreditacion = hoy;
                        resp.FechaAmpliacion = null;
                        resp.FechaEmision = hoy;
                        resp.ListIdUserPuestoVerificentros = new List<EvidenciaEmpleados>();
                        foreach (var item in acreditacion.IdUserPuestoVerificentros)
                        {
                            resp.ListIdUserPuestoVerificentros.Add(new EvidenciaEmpleados
                            {
                                Eliminado = false,
                                IdUserPuesto = item.Id,
                                Nombre = item.IdUserNavigation == null ? "" : item.IdUserNavigation.Nombre,
                                NormaAcreditacion = null,
                                TipoAcreditacion = null
                            });
                        }
                    }
                    else
                    {
                        resp.Id = evidencia.Id;
                        resp.NumeroSolicitud = evidencia.NumeroSolicitud;
                        resp.TipoTramite = evidencia.TipoTramite;
                        resp.NumeroAcreditacion = evidencia.NumeroAcreditacion;
                        resp.NumeroReferencia = evidencia.NumeroReferencia;
                        resp.FechaAcreditacion = evidencia.FechaAcreditacion;
                        resp.FechaAmpliacion = evidencia.FechaAmpliacion;
                        resp.FechaEmision = evidencia.FechaEmision;
                        resp.UrlAcreditacion = evidencia.UrlAcreditacion;
                        resp.IdCatEstatusAcreditacion = evidencia.IdCatEstatusAcreditacion;
                        //resp.ListIdUserPuestoVerificentros = evidencia.AcreditacionTecnicoEvidenciaPuestoVerificentros
                        resp.ListIdUserPuestoVerificentros = evidencia.AcreditacionTecnicoEvidenciaPuestoVerificentros.Select(x => new EvidenciaEmpleados
                        {
                            Eliminado = x.Eliminado,
                            IdUserPuesto = x.IdUserPuestoVerificentro,
                            Nombre = x.IdUserPuestoVerificentroNavigation == null ? "" : x.IdUserPuestoVerificentroNavigation.IdUserNavigation == null ? "" : x.IdUserPuestoVerificentroNavigation.IdUserNavigation.Nombre,
                            NormaAcreditacion = x.IdNormaAcreditacion,
                            TipoAcreditacion = x.IdTipoAcreditacion
                        }).ToList();
                    }
                    //result.Id = acreditacion.Id;
                    //result.IdCatEstatusAcreditacion = acreditacion.IdCatEstatusAcreditacion;
                    //result.CatEstatus = acreditacion.IdCatEstatusAcreditacionNavigation.Nombre;
                    //result.IdCatEstatusAcreditacionNavigation = acreditacion.IdCatEstatusAcreditacionNavigation;
                    //result.ListIdUserPuestoVerificentros = acreditacion.IdUserPuestoVerificentros.Select(x => new EmpleadoDataResponse
                    //{
                    //    Id = x.Id,
                    //    Nombre = x.IdUserNavigation.Nombre
                    //}).ToList();
                    //result.NumeroSolicitud = acreditacion.NumeroSolicitud;
                    //result.UrlAprobacion = acreditacion.UrlAprobacion;
                    //};
                }
                else
                {
                    throw new Exception("No sé encontró información.");
                }
                return new ResponseGeneric<AcreditacionTecnicoEvidenciaResponse>(resp);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<AcreditacionTecnicoEvidenciaResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<List<EmpleadoAutocompleteResponse>>> Autocomplete(EmpleadoAutocompletRequest request)
        {
            try
            {
                var user = _userResolver.GetUser();
                var verificaciones = _context.vPersonalAutorizacions.Where(x => (x.Nombre != null && x.Nombre.ToLower().Contains(request.Term.ToLower())) || x.NumeroTrabajador.ToLower().Contains(request.Term.ToLower()) && x.IdVerificentro == user.IdVerificentro).AsQueryable();
                var tot = verificaciones.Count();
                verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                var result = await verificaciones.Select(x => new EmpleadoAutocompleteResponse
                {
                    Id = x.IdPuestoVerificentro,
                    Text = x.Nombre + " / " + x.NumeroTrabajador
                }).ToListAsync();
                return new ResponseGeneric<List<EmpleadoAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<EmpleadoAutocompleteResponse>>(ex);
            }
        }

        #region comment
        //public async Task<ResponseGeneric<RefrendoGridResponse>> GetById(long Id, bool verificacion)
        //{
        //    try
        //    {
        //        var result = new RefrendoGridResponse();
        //        if (Id > 0)
        //        {
        //            //Se modifica los refrendos para pertenecer a un Exento (reunion 13/01/2023)
        //            var refrendo = _context.vRefrendos.FirstOrDefault(x => x.Id == Id);
        //            if (refrendo != null)
        //            {
        //                string r = JsonConvert.SerializeObject(refrendo);
        //                result = JsonConvert.DeserializeObject<RefrendoGridResponse>(r);
        //                result.Exentos = await _context.vExentos.Where(x => x.Placa == refrendo.Placa || x.Serie == refrendo.Serie).ToListAsync();
        //            }
        //            //if (verificacion)
        //            //{
        //            //    var verif = _context.vVerificacions.FirstOrDefault(x => x.Id == Id);
        //            //    result.Exentos = await _context.vVerificacions.Where(x => x.Placa == verif.Placa || x.Serie == verif.Serie).ToListAsync();
        //            //}
        //            //else
        //            //{
        //            //    var refrendo = _context.vRefrendos.FirstOrDefault(x => x.Id == Id);
        //            //    if (refrendo != null)
        //            //    {
        //            //        string r = JsonConvert.SerializeObject(refrendo);
        //            //        result = JsonConvert.DeserializeObject<RefrendoGridResponse>(r);
        //            //        result.Exentos = await _context.vVerificacions.Where(x => x.Id == refrendo.IdExento).ToListAsync();
        //            //    }
        //            //}
        //        }
        //        else
        //        {
        //            throw new Exception("No sé encontró un refrendo.");
        //        }
        //        return new ResponseGeneric<RefrendoGridResponse>(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseGeneric<RefrendoGridResponse>(ex);
        //    }
        //}

        //public async Task<ResponseGeneric<long>> Registro(RefrendoRequest request)
        //{
        //    try
        //    {
        //        var refrendo = new Refrendo();
        //        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            if (request.Id > 0)
        //            {
        //                refrendo = _context.Refrendos.FirstOrDefault(x => x.Id == request.Id);
        //                if (refrendo == null)
        //                    throw new Exception("No sé encontró un refrendo.");
        //            }
        //            else
        //            {
        //                refrendo = new Refrendo
        //                {
        //                    IdExento = request.IdExento,
        //                    NumeroReferencia = request.NumeroReferencia,
        //                    UrlDoc1 = request.UrlDoc1,
        //                    UrlDoc2 = request.UrlDoc2,
        //                    UrlDoc3 = request.UrlDoc3,
        //                    FechaCartaFactura = request.FechaCartaFactura,
        //                    VigenciaHoloAnterior = request.VigenciaHoloAnterior,
        //                    Placa = request.Placa,
        //                    Propietario = request.Propietario,
        //                    FechaRegistro = DateTime.Now,
        //                    IdUserRegistro = _userResolver.GetUser().IdUser
        //                };
        //                _context.Refrendos.Add(refrendo);
        //            }
        //            var result = await _context.SaveChangesAsync() > 0;

        //            var i = 0;
        //            if (request.Files != null && request.Files.Count > 0)
        //            {
        //                foreach (var f in request.Files)
        //                {
        //                    var url = await _blobStorage.UploadFileAsync(new byte[0], "Refrendo/" + refrendo.Id + "/" + f.Nombre, f.Base64);
        //                    if (!string.IsNullOrEmpty(url))
        //                    {
        //                        switch (i)
        //                        {
        //                            case 0:
        //                                refrendo.UrlDoc1 = url; break;
        //                            case 1:
        //                                refrendo.UrlDoc2 = url; break;
        //                            case 2:
        //                                refrendo.UrlDoc3 = url; break;
        //                        }
        //                    }
        //                    i++;
        //                }
        //                result = await _context.SaveChangesAsync() > 0;
        //            }

        //            scope.Complete();
        //            return result ? new ResponseGeneric<long>(refrendo.Id) : new ResponseGeneric<long>();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseGeneric<long>(ex);
        //    }
        //}

        //public async Task<ResponseGeneric<List<RefrendoAutocompleteResponse>>> Autocomplete(RefrendoAutocompletRequest request)
        //{
        //    try
        //    {
        //        var verificaciones = _context.vVerificacions.Where(x => x.Placa.ToLower().Contains(request.Term.ToLower()) || x.Serie.ToLower().Contains(request.Term.ToLower())).GroupBy(x => x.Placa).AsQueryable();
        //        var tot = verificaciones.Count();
        //        verificaciones = verificaciones.Skip(request.Start).Take(request.End);
        //        var result = await verificaciones.Select(x => new RefrendoAutocompleteResponse
        //        {
        //            Id = x.FirstOrDefault().Id,
        //            Text = x.FirstOrDefault().Placa + " / " + x.FirstOrDefault().Serie
        //        }).ToListAsync();
        //        return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseGeneric<List<RefrendoAutocompleteResponse>>(ex);
        //    }
        //}
        #endregion
    }

    public interface IAcreditacionTecnicoVerificadorNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<AcreditacionTecnicoVerificadorGridResponse>>> Consulta(SolicitudFormaValoradaListRequest request);
        //public Task<ResponseGeneric<RefrendoGridResponse>> GetById(long Id, bool verificacion);
        //public Task<ResponseGeneric<long>> Registro(RefrendoRequest request);
        //public Task<ResponseGeneric<List<RefrendoAutocompleteResponse>>> Autocomplete(RefrendoAutocompletRequest request);
        public Task<ResponseGeneric<string>> RegistroSolicitud(AcreditacionTecnicoSolicitudRequest request);
        public Task<ResponseGeneric<bool>> RegistroEvidencia(AcreditacionTecnicoEvidenciaRequest request);
        public Task<ResponseGeneric<bool>> AutorizarAcreditacion(AutorizarAcreditacionRequest request);
        public Task<ResponseGeneric<AcreditacionTecnicoVerificadorGridResponse>> GetById(long Id);
        public Task<ResponseGeneric<AcreditacionTecnicoEvidenciaResponse>> GetEvidenciaBySolicitud(string solicitud);
        public Task<ResponseGeneric<List<EmpleadoAutocompleteResponse>>> Autocomplete(EmpleadoAutocompletRequest request);
    }
}
