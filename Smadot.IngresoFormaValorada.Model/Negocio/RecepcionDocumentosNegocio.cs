using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Utilities.Modelos;
using static Smadot.Models.Entities.RecepcionDocumentos.Response.RecepcionDocumentosResponseData;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Smadot.Utilities.GestionTokens;
using static Smadot.Models.Entities.RecepcionDocumentos.Request.RecepcionDocumentosRequestData;
using System.Transactions;
using Smadot.Models.Entities.PortalCitas.Response;
using Smadot.Utilities.Modelos.Documentos;
using Smadot.Utilities.BlobStorage;
using Microsoft.Extensions.Configuration;
using Smadot.Utilities.EnvioCorreoElectronico;
using QRCoder;
using System.Globalization;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Verificacion.Response;
using Smadot.Utilities;
using Smadot.Models.Entities.Linea.Response;
using Smadot.Models.Entities.RecepcionDocumentos.Request;
using NPOI.POIFS.Crypt.Dsig;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using Smadot.Models.GenericProcess;
using System.ComponentModel.DataAnnotations;
using Namespace;

namespace Smadot.IngresoFormaValorada.Model.Negocio
{
    public class RecepcionDocumentosNegocio : IRecepcionDocumentosNegocio
    {
        private readonly SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly IPdfBuider _pdfBuilder;
        private readonly BlobStorage _blobStorage;
        private readonly IConfiguration _configuration;
        private readonly string _urlSite;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;

        public RecepcionDocumentosNegocio(SmadotDbContext context, IUserResolver userResolver, IPdfBuider pdfBuilder, IConfiguration configuration, SmadsotGenericInserts smadsotGenericInserts)
        {
            _context = context;
            _userResolver = userResolver;
            _pdfBuilder = pdfBuilder;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _configuration = configuration;
            _urlSite = configuration["SiteUrl"];
            _smadsotGenericInserts = smadsotGenericInserts;
        }

        public async Task<ResponseGeneric<ResponseGrid<RecepcionDocumentosGridResponse>>> Consulta(RequestList request)
        {
            try
            {
                var idVerificentro = _userResolver.GetUser().IdVerificentro;
                var today = DateTime.Now.Date;
                var otherDay = today.AddDays(1);
                var busquedaLower = request.Busqueda?.ToLower() ?? "";

                var query = _context.vCitaVerificacions
                    .Where(x => x.Fecha >= today && x.Fecha < otherDay && x.IdCVV == idVerificentro).AsQueryable();

                var total = await query.CountAsync();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    query = query.Where(x =>
                        x.Folio.ToLower().Contains(busquedaLower) ||
                        x.NombrePropietario.ToLower().Contains(busquedaLower) ||
                        x.RazonSocial.ToLower().Contains(busquedaLower) ||
                        x.Verificentro.ToLower().Contains(busquedaLower) ||
                        x.Placa.ToLower().Contains(busquedaLower) ||
                        x.Marca.ToLower().Contains(busquedaLower) ||
                        x.Modelo.ToLower().Contains(busquedaLower) ||
                        x.Serie.ToLower().Contains(busquedaLower));
                }

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    query = query.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                var filtered = await query.CountAsync();

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    query = query
                        .Skip((request.Pagina.Value - 1) * request.Registros.Value)
                        .Take(request.Registros.Value);
                }

                var result = new ResponseGrid<RecepcionDocumentosGridResponse>
                {
                    RecordsTotal = total,
                    RecordsFiltered = filtered,
                    Data = JsonConvert.DeserializeObject<List<RecepcionDocumentosGridResponse>>(
                        JsonConvert.SerializeObject(await query.ToListAsync())) ?? new()
                };

                return new ResponseGeneric<ResponseGrid<RecepcionDocumentosGridResponse>>(result);

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<RecepcionDocumentosGridResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<RecepcionDocumentosGridResponse>> GetById(long Id)
        {
            try
            {
                var result = new RecepcionDocumentosGridResponse();
                if (Id > 0)
                {
                    var today = DateTime.Now.Date;
                    var otherDay = today.AddDays(1);
                    //var cita = _context.vCitaVerificacions.FirstOrDefault(x => x.IdCita == Id && x.Fecha.Date == today.Date);
                    var cita = _context.vCitaVerificacions.FirstOrDefault(x => x.IdCita == Id && x.Fecha >= today && x.Fecha < otherDay && x.IdCVV == _userResolver.GetUser().IdVerificentro);
                    if (cita != null)
                    {
                        string r = JsonConvert.SerializeObject(cita);
                        result = JsonConvert.DeserializeObject<RecepcionDocumentosGridResponse>(r);
                        if (cita.IdRecepcionDocumento != null)
                        {
                            var dataDocumentos = _context.vDocumentosCita.FirstOrDefault(x => x.Id == cita.IdRecepcionDocumento);
                            r = JsonConvert.SerializeObject(dataDocumentos);
                            result.DataDocumentos = JsonConvert.DeserializeObject<DocumentosCitaResponse>(r);
                        }
                    }
                }
                //else
                //{
                //    throw new Exception("No sé encontró información.");
                //}
                result.ListaMarcas = new List<vCatMarcaVehiculoNombre?>();
                result.ListaMarcasDisel = new List<vDieselVehiculo?>();
                result.ListaMarcas = await _context.vCatMarcaVehiculoNombres.ToListAsync();
                result.ListaMarcasDisel = await _context.vDieselVehiculos.ToListAsync();
                return new ResponseGeneric<RecepcionDocumentosGridResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<RecepcionDocumentosGridResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<string>> Documentos(RecepcionDocumentosRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var user = _userResolver.GetUser();                                    
                    var citaDB = await _context.CitaVerificacions.Include(x => x.DocumentosCitum).FirstOrDefaultAsync(x => x.Id == request.IdCitaVerificacion);
                    if (citaDB == null)
                        return new ResponseGeneric<string>($"No se encontró información de la cita.", true) { Status = ResponseStatus.Failed, mensaje = $"No se encontró información de la cita." };
                    var docCita = new DocumentosCitum();
                    var existeNumeroReferencia = await _context.Verificacions.AnyAsync(x => x.NumeroReferencia == request.NumeroReferencia && request.NumeroReferencia != 0.ToString());
                    if(existeNumeroReferencia)
                        return new ResponseGeneric<string>($"Ya existe ese número de referencia. Favor de verificarlo.", false) { Status = ResponseStatus.Failed, mensaje = $"Ya existe ese número de referencia. Favor de verificarlo." };
                    if (citaDB.DocumentosCitum != null)
                    {
                        return new ResponseGeneric<string>($"Ya se le realizó recepción de documentos a este vehículo.", false) { Status = ResponseStatus.Failed, mensaje = $"Ya se le realizó recepción de documentos a este vehículo." };
                        // docCita = citaDB.DocumentosCitum;
                    }
                    if (citaDB.IdCVV != _userResolver.GetUser().IdVerificentro)
                    {
                        return new ResponseGeneric<string>($"El centro de verificación en el que el usuario inicio sesión es diferente al centro dónde se genero la cita.", true) { Status = ResponseStatus.Failed, mensaje = $"El centro de verificación en el que el usuario inicio sesión es diferente al centro dónde se genero la cita." };

                    }
                    if (citaDB.IdSubMarcaVehiculo != request.Modelo && request.Modelo != null)
                    {
                        citaDB.IdSubMarcaVehiculo = request.Modelo.Value;
                    }
                    if (!string.IsNullOrEmpty(request.ColorVehiculo))
                    {
                        citaDB.ColorVehiculo = request.ColorVehiculo;
                    }
                    if (!string.IsNullOrEmpty(request.Estado))
                    {
                        citaDB.Estado = request.Estado;
                        citaDB.Poblano = request.Estado.Equals("PUEBLA");
                    }
                    else if (string.IsNullOrEmpty(request.Estado))
                    {
                        return new ResponseGeneric<string>($"Debe seleccionar el estado de procedencia del vehículo.", true) { Status = ResponseStatus.Failed, mensaje = $"Debe seleccionar el estado de procedencia del vehículo.." };
                    }
                    if (request.IdMotivoVerificacion == MotivoVerificacionDict.VERIFICACIONVOLUNTARIA && request.Estado.Equals("PUEBLA"))
                    {
                        return new ResponseGeneric<string>($"Sí la verificación es voluntaria no puede pertenecer al estado de PUEBLA.", true) { Status = ResponseStatus.Failed, mensaje = $"Sí la verificación es voluntaria no puede pertenecer al estado de PUEBLA." };

                    }
                    if ((request.IdMotivoVerificacion == MotivoVerificacionDict.HologramaDobleceroRefrendo && request.FechaFacturacion == null) || (request.IdMotivoVerificacion == MotivoVerificacionDict.HologramaDoblecero && request.FechaFacturacion == null) || (request.IdMotivoVerificacion == MotivoVerificacionDict.RECHAZOHOLOGRAMADOBLECERO && request.FechaFacturacion == null))
                    {
                        return new ResponseGeneric<string>($"Debe ingresar una fecha de facturación.", true) { Status = ResponseStatus.Failed, mensaje = $"Debe ingresar una fecha de facturación." };
                    }
                    if (!string.IsNullOrEmpty(request.Placa))
                    {
                        citaDB.Placa = request.Placa;
                    }
                    else
                    {
                        return new ResponseGeneric<string>($"Ingrese la placa.", true) { Status = ResponseStatus.Failed, mensaje = $"Ingrese la placa." };
                    }
                    if (!string.IsNullOrEmpty(request.Serie))
                    {
                        citaDB.Serie = request.Serie.Trim();
                    }
                    else
                    {
                        return new ResponseGeneric<string>($"Ingrese el número de serie.", true) { Status = ResponseStatus.Failed, mensaje = $"Ingrese el número de serie." };
                    }
                    if (!string.IsNullOrEmpty(request.NombrePersona))
                    {
                        citaDB.Nombre = !request.PersonaMoral ? request.NombrePersona.ToUpper() : null;
                        citaDB.RazonSocial = request.PersonaMoral ? request.NombrePersona.ToUpper() : null;
                    }
                    else
                    {
                        return new ResponseGeneric<string>($"Ingrese el número de nombre o razón social del propietario del vehículo.", true) { Status = ResponseStatus.Failed, mensaje = $"Ingrese el número de nombre o razón social del propietario del vehículo." };
                    }
                    if (!string.IsNullOrEmpty(request.NombreGeneraCita))
                    {
                        citaDB.NombreGeneraCita = request.NombreGeneraCita;
                    }
                    else
                    {
                        return new ResponseGeneric<string>($"Ingrese el número de nombre de la persona que verifica.", true) { Status = ResponseStatus.Failed, mensaje = $"Ingrese el número de nombre de la persona que verifica." };
                    }
                    if (request.IdTipoCombustible != null)
                    {
                        citaDB.IdTipoCombustible = request.IdTipoCombustible;
                    }
                    else
                    {
                        return new ResponseGeneric<string>($"Seleccione el combustible.", true) { Status = ResponseStatus.Failed, mensaje = $"Seleccione el combustible." };
                    }
                    if (request.Anio > 1800 && request.Anio != null)
                    {
                        citaDB.Anio = request.Anio.Value;
                    }
                    else
                    {
                        return new ResponseGeneric<string>($"El Año Modelo del Vehículo ingresado no es válido.", true) { Status = ResponseStatus.Failed, mensaje = $"El Año Modelo del Vehículo ingresado no es válido." };
                    }

                    var parametrosVerificacion = await _context.vTablaMaestras.FirstOrDefaultAsync(tm => tm.Id == request.IdTablaMaestra);
                    if (parametrosVerificacion == null)
                    {
                        return new ResponseGeneric<string>($"No se encontró información del vehículo en la tabla maestra.", false) { Status = ResponseStatus.Failed, mensaje = $"No se encontró información del vehículo en la tabla maestra." };

                    }
                    var doblecero = false;
                    if (citaDB.Anio >= DateTime.Now.AddYears(-2).Year && citaDB.Anio <= DateTime.Now.AddYears(1).Year && parametrosVerificacion.DOBLECERO > 0 && (request.IdMotivoVerificacion == MotivoVerificacionDict.NuevoSinVerificacionAnterior || request.IdMotivoVerificacion == MotivoVerificacionDict.HologramaDoblecero || request.IdMotivoVerificacion == MotivoVerificacionDict.HologramaDobleceroRefrendo || request.IdMotivoVerificacion == MotivoVerificacionDict.RECHAZOHOLOGRAMADOBLECERO))
                    {
                        if (request.FechaFacturacion == null)
                            return new ResponseGeneric<string>($"Se debe establecer la fecha de facturación para un tipo doblecero.", false) { Status = ResponseStatus.Failed, mensaje = $"Se debe establecer la fecha de facturación para un tipo doblecero." };
                        else if (request.FechaFacturacion.Value.Year < DateTime.Now.AddYears(-2).Year && request.FechaFacturacion.Value.Year > DateTime.Now.AddYears(1).Year)
                            return new ResponseGeneric<string>($"El año de la factura no es válida para un tipo doblecero.", false) { Status = ResponseStatus.Failed, mensaje = $"El año de la factura no es válida para un tipo doblecero." };
                        else
                            docCita.FechaFacturacion = request.FechaFacturacion;
                        doblecero = true;
                    }
                    else
                    {
                        docCita.FechaFacturacion = null;

                    }


                    citaDB.IdCatMarcaVehiculo = parametrosVerificacion.IdCatMarcaVehiculo;
                    citaDB.IdSubMarcaVehiculo = parametrosVerificacion.IdCatSubmarcaVehiculo;
                    docCita.IdCatTipoServicio = request.IdCatTipoServicio;
                    docCita.IdCitaVerificacion = citaDB.Id;
                    docCita.FolioTarjetaCirculacion = request.FolioTarjetaCirculacion;
                    docCita.CambioPlacas = request.CambioPlacas;
                    docCita.FechaRecepcion = DateTime.Now;
                    docCita.IdUserRegistro = user.IdUser;

                    if (citaDB.DocumentosCitum == null)
                    {
                        docCita.URLIdentificacion = string.Empty;
                        docCita.URLFactura = string.Empty;
                        docCita.URLCartaFactura = string.Empty;
                        docCita.URLValidacionCertificado = string.Empty;
                        await _context.DocumentosCita.AddAsync(docCita);
                    }

                    // var guardado = await _context.SaveChangesAsync() > 0;




                    if (request.FileIdentificacion != null)
                    {
                        var file = request.FileIdentificacion;
                        if (file != null)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + citaDB.Id + "/Identificacion/" + file.Nombre, file.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                docCita.URLIdentificacion = url;
                            }
                        }
                    }
                    if (request.FileFactura != null)
                    {
                        var file = request.FileFactura;
                        if (file != null)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + citaDB.Id + "/Factura/" + file.Nombre, file.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                docCita.URLFactura = url;
                            }
                        }
                    }
                    //if (request.FileCartaFactura.Count() > 0 && request.FileCartaFactura != null)
                    //{
                    //    var file = request.FileCartaFactura.FirstOrDefault();
                    //    if (file != null)
                    //    {
                    //        var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + citaDB.Id + "/" + file.Nombre, file.Base64);
                    //        if (!string.IsNullOrEmpty(url))
                    //        {
                    //            docCita.URLCartaFactura = url;
                    //        }
                    //    }
                    //}
                    if (request.FileValidacionCertificado != null)
                    {
                        var file = request.FileValidacionCertificado;
                        if (file != null)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + citaDB.Id + "/CertificadoAnterior/" + file.Nombre, file.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                docCita.URLValidacionCertificado = url;
                            }
                        }
                    }
                    if (request.FileMulta != null)
                    {
                        var file = request.FileMulta;
                        if (file != null)
                        {
                            var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + citaDB.Id + "/Multa/" + file.Nombre, file.Base64);
                            if (!string.IsNullOrEmpty(url))
                            {
                                docCita.URLCartaFactura = url;
                            }
                        }
                    }

                    if (docCita.CambioPlacas)
                    {
                        if (request.FileBajaPlacas != null)
                        {
                            var file = request.FileBajaPlacas;
                            if (file != null)
                            {
                                var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + citaDB.Id + "/BajaPlaca/" + file.Nombre, file.Base64);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    docCita.URLBajaPlacas = url;
                                }
                            }
                        }
                        if (request.FileAltaPlacas != null)
                        {
                            var file = request.FileAltaPlacas;
                            if (file != null)
                            {
                                var url = await _blobStorage.UploadFileAsync(new byte[0], "RecepcionDocumentos/" + citaDB.Id + "/AltaPlaca" + file.Nombre, file.Base64);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    docCita.URLAltaPlacas = url;
                                }
                            }
                        }
                    }
                    var guardado = await _context.SaveChangesAsync() > 0;
                    if (!guardado)
                        return new ResponseGeneric<string>($"Ocurrió un error al guardar la información.", false) { Status = ResponseStatus.Failed, mensaje = $"Ocurrió un error al guardar la información." };
                    //var fechaActual = DateTime.Now.ToString("d");

                    var lineasQ = new List<LineaPendientes>();
                    lineasQ = await _smadsotGenericInserts.PendientesLineas(user.IdVerificentro ?? 0, parametrosVerificacion.PROTOCOLO);
                    var datosLinea = lineasQ.FirstOrDefault();
                    if (datosLinea == null)
                    {
                        return new ResponseGeneric<string>($"No se encontró una línea para poder atender el vehículo. Verifique que los equipos estén activos, calibrados correctamente y las cámaras de las líneas estén funcionando.", false)
                        {
                            Status = ResponseStatus.Failed,
                            mensaje = $"No se encontró una línea para poder atender el vehículo. Verifique que los equipos estén activos, calibrados correctamente y las cámaras de las líneas estén funcionando."
                        };

                    }
                    var lineaSeleccionada = datosLinea?.Linea;
                    var datoslineaSeleccionada = datosLinea;
                    var combustible = Combustible.DictCombustible[citaDB.IdTipoCombustible];
                    Verificacion? verificacion = await _context.Verificacions.Include(x => x.IdLineaNavigation).FirstOrDefaultAsync(x => x.IdCitaVerificacion == request.IdCitaVerificacion);
                    // var motivoVerificacion = 2;
                    // if (docCita.URLBajaPlacas != null)
                    //     motivoVerificacion = 3; // ALTA Y BAJA DE PLACAS 
                    // if (doblecero)
                    //     motivoVerificacion = 6; // HOLOGRAMA DOBLE CERO 
                    var fechaVigencia = DateTime.Now.AddDays(30).Date;
                    if (verificacion == null)
                    {
                        if (doblecero && request.IdMotivoVerificacion == MotivoVerificacionDict.NuevoSinVerificacionAnterior)
                            request.IdMotivoVerificacion = MotivoVerificacionDict.HologramaDoblecero;
                        var fechaVerificacion = DateTime.Now;
                        // Obtemenos el semestre para guardarlo
                        var semestre = fechaVerificacion.Month > 6 ? 2 : 1;
                        var anio = fechaVerificacion.Year;
                        if (request.IdMotivoVerificacion == MotivoVerificacionDict.ExencionPeriodo || request.IdMotivoVerificacion == MotivoVerificacionDict.EXENCIONFUERADEPERIODOPRIMERINTENTO)
                        {
                            semestre = semestre == 2 ? 1 : 2;
                            if (semestre == 2)
                            {
                                anio -= 1;
                            }
                        }


                        verificacion = new Verificacion
                        {
                            IdLinea = lineaSeleccionada.Id,
                            Anio = citaDB.Anio,
                            Serie = citaDB.Serie.Trim(),
                            Fecha = fechaVerificacion,
                            Modelo = parametrosVerificacion.SubMarca,
                            Marca = parametrosVerificacion.Marca,
                            Vigencia = fechaVigencia,
                            Placa = citaDB.Placa.Trim(),
                            TarjetaCirculacion = request.FolioTarjetaCirculacion,
                            Orden = datosLinea.UltimoEnCola + 1,
                            IdCicloVerificacion = (long)user.IdCicloVerificacion,
                            IdCitaVerificacion = citaDB.Id,
                            IngresoManual = true,
                            IdTipoCombustible = citaDB.IdTipoCombustible,
                            NumeroReferencia = request.NumeroReferencia,
                            IdEquipoVerificacion = datoslineaSeleccionada?.IdEquipo == 0 ? null : datoslineaSeleccionada?.IdEquipo,
                            IdCatSubDiesel = null,
                            Combustible = combustible,
                            IdMotivoVerificacion = request.IdMotivoVerificacion,
                            FolioCertificado = request.FolioCertificadoAnterior ?? string.Empty,
                            Semestre = semestre,
                            AnioVerificacion = anio,
                        };
                        await _context.Verificacions.AddAsync(verificacion);
                        verificacion.ParametrosTablaMaestraVerificacion = new ParametrosTablaMaestraVerificacion
                        {
                            IdCatMarcaVehiculo = parametrosVerificacion.IdCatMarcaVehiculo,
                            IdCatSubmarcaVehiculo = parametrosVerificacion.IdCatSubmarcaVehiculo,
                            Motor_DSL = parametrosVerificacion.Motor_DSL,
                            COMB_ORIG = parametrosVerificacion.COMB_ORIG,
                            CARROCERIA = parametrosVerificacion.CARROCERIA,
                            ALIM_COMB = parametrosVerificacion.ALIM_COMB,
                            CILINDROS = parametrosVerificacion.CILINDROS,
                            CILINDRADA = parametrosVerificacion.CILINDRADA,
                            PBV = parametrosVerificacion.PBV,
                            PBV_EQUIV = parametrosVerificacion.PBV_EQUIV,
                            PBV_ASM = parametrosVerificacion.PBV_ASM,
                            CONV_CATAL = parametrosVerificacion.CONV_CATAL,
                            OBD = parametrosVerificacion.OBD,
                            C_ABS = parametrosVerificacion.C_ABS,
                            T_TRACC = parametrosVerificacion.T_TRACC,
                            C_TRACC = parametrosVerificacion.C_TRACC,
                            T_PRUEBA = parametrosVerificacion.T_PRUEBA,
                            PROTOCOLO = parametrosVerificacion.PROTOCOLO,
                            POTMAX_RPM = parametrosVerificacion.POTMAX_RPM,
                            ANO_DESDE = parametrosVerificacion.ANO_DESDE,
                            ANO_HASTA = parametrosVerificacion.ANO_HASTA,
                            O2_MAX = parametrosVerificacion.O2_MAX,
                            LAMDA_MAX = parametrosVerificacion.LAMDA_MAX,
                            POT_5024 = parametrosVerificacion.POT_5024,
                            POT_2540 = parametrosVerificacion.POT_2540,
                            DOBLECERO = parametrosVerificacion.DOBLECERO,
                            CERO_GASOL = parametrosVerificacion.CERO_GASOL,
                            CERO_GASLP = parametrosVerificacion.CERO_GASLP,
                            CERO_GASNC = parametrosVerificacion.CERO_GASNC,
                            CERO_DSL = parametrosVerificacion.CERO_DSL,
                            REF_00 = parametrosVerificacion.REF_00
                        };
                    }

                    await _context.SaveChangesAsync();
                    transaction.Complete();
                    return new ResponseGeneric<string>("", true) { mensaje = $"La documentación del vehículo ha sido guardada exitosamente. La línea asignada para su verificación es la <b>{lineaSeleccionada.Nombre}</b>" };
                }
            }
            catch (Exception ex)
            {
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionRecepcionDocumentos);


                return new ResponseGeneric<string>("Ocurrió un error al guardar la información");
            }

        }

        public async Task<ResponseGeneric<PortalCitasRegistroResponse>> Reagendar(ReagendarCitaRequest request)
        {
            try
            {
                var ressResponse = new PortalCitasRegistroResponse();
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var citaDB = _context.CitaVerificacions.FirstOrDefault(x => x.Id == request.IdCita && x.Folio == request.Folio);
                    if (citaDB == null)
                    {
                        ressResponse.ErrorMessage = $"Ocurrió un error al registrar la información de la cita.";
                        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                    }

                    if (request.Fecha == null)
                    {
                        ressResponse.ErrorMessage = $"Debe ingresar una fecha para el registro.";
                        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                    }
                    var comprobarCupo = _context.vNumeroCitasHorarios.FirstOrDefault(x => x.IdCVV == request.IdCVV && x.Fecha == request.Fecha);
                    if (comprobarCupo != null && comprobarCupo.NumeroCitas >= comprobarCupo.Capacidad)
                    {
                        ressResponse.ErrorMessage = $"Ya no hay más cupos disponibles en este horario.";
                        return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    }


                    #region Reglas Anteriores
                    //var diaText = request.Fecha.ToString("dddd", new CultureInfo("es-MX"));
                    //var dia = char.ToUpper(diaText[0]) + diaText.Substring(1);
                    // ------------------------------------------------------- ---------------------------------------------------
                    // TODO: Cambiar reglas para las Citas y como se obtienen los espacios disponibles
                    // var config = _context.ConfiguradorCita.FirstOrDefault(x => x.IdCVV == request.IdCVV && x.Dia.Equals(dia));
                    //var comprobarCitaExiste = _context.CitaVerificacions.Where(x => x.IdCVV == request.IdCVV && x.Fecha == request.Fecha && (x.Cancelada == false || x.Cancelada == null));
                    // TODO: Cambiar reglas para las Citas y como se obtienen los espacios disponibles
                    // if (comprobarCitaExiste.Count() >= config.Capacidad)
                    // {
                    //     ressResponse.ErrorMessage = $"Ya no hay más cupos disponibles en este horario.";
                    //     return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, false);
                    // }
                    // ------------------------------------------------------- ---------------------------------------------------

                    // var comprobarCitaPrevia = _context.CitaVerificacions.Any(x => x.Placa == citaDB.Placa && (x.Fecha >= DateTime.Now.AddHours(-12) && x.Fecha <= DateTime.Now) && x.Cancelada == true);

                    // if (comprobarCitaPrevia)
                    // {
                    //     ressResponse.ErrorMessage = $"El vehículo ya cuenta con una cita registrada.";
                    //     return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                    // }

                    //04-07-2023 Omar ua no se cancela, ahora se modifica
                    //var registro = new CitaVerificacion
                    //{
                    //    Nombre = citaDB.Nombre,
                    //    NombreGeneraCita = citaDB.NombreGeneraCita,
                    //    IdTipoCombustible = citaDB.IdTipoCombustible,
                    //    RazonSocial = citaDB.RazonSocial,
                    //    Correo = citaDB.Correo,
                    //    Fecha = request.Fecha,
                    //    IdCVV = request.IdCVV,
                    //    Placa = citaDB.Placa,
                    //    IdCatMarcaVehiculo = citaDB.IdCatMarcaVehiculo,
                    //    IdSubMarcaVehiculo = citaDB.IdSubMarcaVehiculo,
                    //    Anio = citaDB.Anio,
                    //    Acepto = true,
                    //    Serie = citaDB.Serie,
                    //    ColorVehiculo = string.Empty,
                    //    Folio = GenerarFolio(10),
                    //    UrlComprobante = string.Empty,
                    //    Estado = citaDB.Estado,
                    //    Poblano = citaDB.Poblano,
                    //    FechaCreacion = DateTime.Now
                    //};

                    //_context.CitaVerificacions.Add(registro);
                    //citaDB.Cancelada = true;
                    #endregion

                    citaDB.IdCVV = request.IdCVV;
                    citaDB.Fecha = request.Fecha;
                    var Guardado = await _context.SaveChangesAsync() > 0;



                    var result = Guardado;
                    var verif = _context.Verificentros.FirstOrDefault(x => x.Id == citaDB.IdCVV);
                    if (result && verif != null)
                    {
                        var dataReport = new DataReport();
                        var data = new ComprobanteCitaResponse
                        {
                            NombrePersona = citaDB.NombreGeneraCita,
                            Fecha = citaDB.Fecha,
                            NombreCentroVerificacion = verif.Nombre,
                            DireccionCentroVerificacion = verif.Direccion,
                            Folio = citaDB.Folio,
                            UrlWeb = _urlSite
                        };

                        var getdoc = await _pdfBuilder.GetComprobanteCita(data);
                        var doc = getdoc.Response.DocumentoPDF;
                        var nomb = getdoc.Response.NombreDocumento;

                        dataReport.NombreDocumento = nomb;
                        dataReport.DocumentoPDF = doc;

                        var pdf = dataReport;

                        if (pdf.DocumentoPDF.Length > 0 && pdf.DocumentoPDF != null)
                        {
                            var stringB64 = Convert.ToBase64String(pdf.DocumentoPDF);

                            var url = _blobStorage.UploadFileAsync(new byte[0], "PortalCita/" + citaDB.Folio + "/" + pdf.NombreDocumento, stringB64).Result;
                            if (!string.IsNullOrEmpty(url))
                            {
                                citaDB.UrlComprobante = url;
                                await _context.SaveChangesAsync();
                            }
                        }

                        EmailMessage emailMessage = new EmailMessage();
                        EnvioCorreoSMTP envioCorreo = new EnvioCorreoSMTP();

                        string imageLogo = "";
                        imageLogo = !string.IsNullOrEmpty(request.Logo) ? request.Logo : "";
                        emailMessage.Subject = "Cita en Línea";
                        QRCodeData qrCodeData;
                        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                        {
                            qrCodeData = qrGenerator.CreateQrCode(string.Format("{0}PortalCitas/ConsultaCita?folio={1}", _urlSite, data.Folio), QRCodeGenerator.ECCLevel.Q);
                        }
                        // Image Format
                        var imgType = Base64QRCode.ImageType.Png;

                        var qrCode = new Base64QRCode(qrCodeData);
                        //Base64 Format
                        string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White, true, imgType);
                        var ImageAsBase64 = qrCodeImageAsBase64;
                        var imgTypeLow = imgType.ToString().ToLower();
                        var contentMail = "<p style=\"\r\n    font-size: 15px;\r\n\">Estimado(a) <b style=\"\r\n    font-size: 16px;\r\n\">" + data.NombrePersona + ": </b></p>\r\n<p style=\"\r\n    font-size: 15px;\r\n\">Ha registrado una cita para el día <b>" + data.Fecha.ToString("D", CultureInfo.GetCultureInfo("es-ES")) + "</b>, a las <b>" + data.Fecha.ToString("t", CultureInfo.GetCultureInfo("es-ES")) + " hrs.</b></p>\r\n<p style=\"\r\n    font-size: 15px;\r\n\">Por lo cual le reiteramos debe presentarse en el <b>" + data.NombreCentroVerificacion.ToUpper() + "</b> en la fecha y hora antes mencionadas.</p>\r\n<hr>\r\n<p style=\"\r\n    font-size: 15px;\r\n\">Le recordamos que el <b>Folio</b> de su Cita es: </p>\r\n\r\n<p style=\"\r\n    font-size: 30px;\r\n    text-align-last: center;\r\n\"><b>" + data.Folio + "</b></p><div style=\"text-align-last:center\"><img alt=\"QR Code\" style=\"width:200px; height:200px\" src=\"data:image/" + imgTypeLow + ";base64," + ImageAsBase64 + "\" /></div><hr><br>\r\n<p style=\"font-size: 15px\"><b>Notas: </b></p>\r\n<ul style=\"\r\n    font-size: 15px;\r\n\">\r\n    <li>Deberá llegar a su cita al menos 05 minutos antes</li>\r\n    <li>Presentarse con original y copia de todos los requisitos</li>\r\n    <li>Se recomienda que el vehículo llegue con la temperatura normal de operación para realizar la prueba</li>\r\n    <li>No es recomendable prender y apagar el vehículo mientras se encuentra en espera para pasar a la prueba</li>\r\n    <li>Se recomienda que los vehículos nuevos realicen la prueba con al menos 300 km recorridos</li>\r\n</ul><br><br<br><br>\r\n<div class=\"col-md-4 col-5\" style=\"text-align: center;\">\r\n<a href=\"https://localhost:7065/PortalCitas/ConfirmarCita?folio=" + data.Folio + "\">\r\n    <button class=\"button\">Confimar Cita</button>\r\n</a>";

                        String bodyCorreo = envioCorreo.BodyEmail(emailMessage.Subject, contentMail, imageLogo, null);

                        var destinatario = new List<string>
                            {
                                citaDB.Correo
                            };

                        var sedm = envioCorreo.Send(
                            destinatario,
                            emailMessage.Subject,
                            bodyCorreo,
                            _configuration["Correo:email"], _configuration["Correo:usuario"],
                            _configuration["Correo:contrasena"], _configuration["Correo:smtp"],
                            null, null, _configuration["Correo:puerto"], true
                            );
                    }

                    transaction.Complete();
                    //return new ResponseGeneric<bool>(result);
                    ressResponse.ErrorMessage = "";
                    ressResponse.FolioResult = citaDB.Folio;
                    return new ResponseGeneric<PortalCitasRegistroResponse>(ressResponse, true);
                }
            }
            catch (Exception ex)
            {
                //return new ResponseGeneric<bool>(ex);
                return new ResponseGeneric<PortalCitasRegistroResponse>(ex) { mensaje = "Ocurrió un error al procesar la reagendación de la cita." };
            }
        }

        public async Task<ResponseGeneric<List<CatalogoTablaMaestraResponse>>> ConsultaTablaMaestra(CatalogoTablaMaestraRequest request)
        {
            try
            {
                var cita = await _context.CitaVerificacions.FirstOrDefaultAsync(x => x.Id == request.IdCita);
                if (cita == null)
                {
                    return new ResponseGeneric<List<CatalogoTablaMaestraResponse>>(new List<CatalogoTablaMaestraResponse>());
                }

                var catalogo = _context.vTablaMaestras.Where(x => (
                    cita.IdTipoCombustible == Combustible.Diesel ? (x.CERO_DSL == 1) :
                    (cita.IdTipoCombustible == Combustible.Gasolina ? x.CERO_GASOL == 1 :
                    (cita.IdTipoCombustible == Combustible.GasNat ? x.CERO_GASNC == 1 :
                    cita.IdTipoCombustible == Combustible.GasLp ? x.CERO_GASLP == 1 : false))
                )
                ).AsQueryable();

                var tot = catalogo.Count();

                catalogo = catalogo.Where(x => x.IdCatMarcaVehiculo == request.IdMarca && x.IdCatSubmarcaVehiculo == request.IdSubmarca);

                // catalogo = catalogo.Where(x => x.IdCatSubmarcaVehiculo == cita.IdSubMarcaVehiculo);

                if (cita.Anio > 0)
                {
                    catalogo = catalogo.Where(x => cita.Anio >= x.ANO_DESDE && cita.Anio <= x.ANO_HASTA);
                }

                if (request.Cilindros > 0 && request.Cilindros != null)
                {
                    catalogo = catalogo.Where(x => x.CILINDROS == request.Cilindros);
                }


                var resut = await catalogo.Select(x => new CatalogoTablaMaestraResponse
                {
                    CILINDROS = x.CILINDROS,
                    CILINDRADA = (request.Cilindros > 0 && request.Cilindros != null) ? x.CILINDRADA : 0

                }).ToListAsync();

                return new ResponseGeneric<List<CatalogoTablaMaestraResponse>>(resut);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<CatalogoTablaMaestraResponse>>(ex);
            }
        }
        private async Task<ResponseGeneric<PlacasResponse>> CapturarPlacas(string ApiEndPoint, string ApiKey, string clave)
        {
            var serviceVerificentro = new ServicioEventosVerificentro(ApiEndPoint, ApiKey);

            var responseService = await serviceVerificentro.GetAsync<PlacasResponse>($"RecepcionEventos/CapturaPlacas/{clave}", clave, $"No se pudo obtener imagen de las cámaras del {clave}.");
            if (responseService.Status != ResponseStatus.Success)
            {
                await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(responseService), DictTipoLog.ExcepcionEventoEntrada);
            }
            return responseService;
        }
        public async Task<ResponseGeneric<List<DieselAutocompleteResponse>>> Autocomplete(GenericSelect2AutocompleRequest request)
        {
            try
            {
                var verificaciones = _context.vDieselVehiculos.Where(x => x.Marca.ToLower().Contains(request.Term.ToLower())).AsQueryable();

                var tot = verificaciones.Count();
                verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                var result = await verificaciones.Select(x => new DieselAutocompleteResponse
                {
                    Id = x.IdCatMarcaVehiculo,
                    Text = x.Marca
                }).Distinct().ToListAsync();
                return new ResponseGeneric<List<DieselAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<DieselAutocompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<SubDieselResponse>>> ConsultaSubDiesel(SubDieselRequest request)
        {
            try
            {
                var catalogo = _context.vDieselVehiculos.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);

                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x => x.Id.ToString().Contains(request.Busqueda.ToLower()) || x.Nombre.ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.IdCatMarcaVehiculo > 0 && request.IdCatMarcaVehiculo != null)
                {
                    catalogo = catalogo.Where(x => x.IdCatMarcaVehiculo == request.IdCatMarcaVehiculo);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }
                var resut = await catalogo.Select(x => new SubDieselResponse
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Total = tot
                }).ToListAsync();

                return new ResponseGeneric<List<SubDieselResponse>>(resut);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<SubDieselResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<LineaPendientes>>> CambiarLinea(long idVerificacion)
        {
            try
            {
                var user = _userResolver.GetUser();
                var verificacion = await _context.Verificacions.Include(x => x.ParametrosTablaMaestraVerificacion).FirstOrDefaultAsync(x => x.Id == idVerificacion);
                var protocolo = verificacion?.ParametrosTablaMaestraVerificacion?.PROTOCOLO ?? 0;
                var lineasQ = new List<LineaPendientes>();

                lineasQ = await _smadsotGenericInserts.PendientesLineas(user.IdVerificentro ?? 0, protocolo);


                lineasQ = lineasQ.Where(x => x.Linea.Id != verificacion?.IdLinea).OrderBy(x => x.Pendientes).ToList();

                return new ResponseGeneric<List<LineaPendientes>>(lineasQ);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<LineaPendientes>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> CambiarLinea(RecepcionDocumentosCambiarLineaRequest request)
        {
            try
            {
                var verificacion = new Verificacion();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (request.IdVerificacion > 0)
                    {
                        verificacion = _context.Verificacions.Include(x => x.ResultadosVerificacion).FirstOrDefault(x => x.Id == request.IdVerificacion);
                        if (verificacion == null)
                            throw new ValidationException("No sé encontró la verificación seleccionada.");
                        var citaVerificacion = await _context.CitaVerificacions.FirstOrDefaultAsync(x => x.Id == verificacion.IdCitaVerificacion);

                        if (citaVerificacion == null)
                            throw new ValidationException("No sé encontró información de la Cita.");

                        var verificentro = await _context.vVerificentros.FirstOrDefaultAsync(x => x.Id == citaVerificacion.IdCVV);
                        if (verificentro == null)
                            throw new ValidationException("No sé encontró información del Centro de Verificación.");
                        var linea = await _context.Lineas.FirstOrDefaultAsync(x => x.Id == request.IdLinea);

                        if (linea == null)
                            throw new ValidationException("No sé encontró información de la Línea.");
                        verificacion.IdLinea = request.IdLinea;
                        if (verificacion.ResultadosVerificacion != null && verificacion.ResultadosVerificacion.EstatusPrueba >= EstatusVerificacion.TerminaPruebaVisual)
                        {

                            var responseService = await CambiarLineaSWProveedor(verificentro.ApiEndPoint, verificentro.ApiKey, verificentro.Nombre, verificacion.Id, linea.Clave);
                            if (responseService.Status != ResponseStatus.Success)
                            {
                                throw new ValidationException(responseService.mensaje);
                            }
                        }
                    }
                    else
                    {
                        throw new ValidationException("Ocurrió un error al obtener la información de la verificación.");

                    }

                    var result = await _context.SaveChangesAsync() > 0;
                    scope.Complete();
                    return result ? new ResponseGeneric<long>(verificacion.Id) : new ResponseGeneric<long>();
                }
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "Ocurrió un error intentar cambiar la línea" };
            }
        }

        public async Task<ResponseGeneric<List<Verificacion>>> ConsultaVeririfacion(string numeroSerie)
        {
            try
            {
                var verificacion = await _context.Verificacions.Where(x => x.Serie == numeroSerie).ToListAsync();
                
                var estatusVerificacion = _context.ResultadosVerificacions.Where(x => x.Id == verificacion[verificacion.Count - 1 ].Id && x.EstatusPrueba == 11);

                var impreso = _context.FoliosFormaValoradaVerificentros.Where(x => x.Folio == Convert.ToInt64(verificacion[verificacion.Count - 1].FolioCertificado) && x.Impreso == true);

                if (verificacion == null || estatusVerificacion == null || impreso == null)
                {
                    return new ResponseGeneric<List<Verificacion>>(new List<Verificacion>());
                }
                

                return new ResponseGeneric<List<Verificacion>>(verificacion);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<Verificacion>>(ex);
            }
        }
        #region Private Methods
        private async Task<ResponseGeneric<string>> CambiarLineaSWProveedor(string ApiEndPoint, string ApiKey, string verificentro, long IdPrueba, string Linea)
        {
            try
            {
                var serviceVerificentro = new ServicioEventosVerificentro(ApiEndPoint, ApiKey);
                var texto = $"La solicitud al servicio del {verificentro} no fue satifactoria.";
                var requestService = new CambiarLineaRequest { IdPrueba = IdPrueba, Linea = Linea };

                var responseService = await serviceVerificentro.PutAsync<object, CambiarLineaRequest>("RecepcionEventos/CambiarLinea", requestService, verificentro, texto);
                if (responseService.Status != ResponseStatus.Success)
                {
                    await _smadsotGenericInserts.SaveLog(JsonConvert.SerializeObject(responseService), DictTipoLog.ExcepcionEventoEntrada);

                    return new ResponseGeneric<string>() { mensaje = responseService.mensaje, CurrentException = responseService.CurrentException, Status = ResponseStatus.Failed };
                }
                return new ResponseGeneric<string>("", true);

            }
            catch (Exception)
            {

                return new ResponseGeneric<string>("Ocurrió un error al contactar el servicio", true);
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

    public interface IRecepcionDocumentosNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<RecepcionDocumentosGridResponse>>> Consulta(RequestList request);
        public Task<ResponseGeneric<RecepcionDocumentosGridResponse>> GetById(long Id);
        public Task<ResponseGeneric<string>> Documentos(RecepcionDocumentosRequest request);
        public Task<ResponseGeneric<PortalCitasRegistroResponse>> Reagendar(ReagendarCitaRequest request);
        public Task<ResponseGeneric<List<CatalogoTablaMaestraResponse>>> ConsultaTablaMaestra(CatalogoTablaMaestraRequest request);
        public Task<ResponseGeneric<List<DieselAutocompleteResponse>>> Autocomplete(GenericSelect2AutocompleRequest request);
        public Task<ResponseGeneric<List<SubDieselResponse>>> ConsultaSubDiesel(SubDieselRequest request);
        public Task<ResponseGeneric<List<LineaPendientes>>> CambiarLinea(long idVerificacion);
        public Task<ResponseGeneric<long>> CambiarLinea(RecepcionDocumentosCambiarLineaRequest request);
        public Task<ResponseGeneric<List<Verificacion>>> ConsultaVeririfacion(string numeroSerie);
    }
}
