using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System.Linq;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.ConsultaTablaMaestra.Response.ConsultaTablaMaestraResponseData;
using Microsoft.EntityFrameworkCore;
using static Smadot.Models.Entities.ConsultaTablaMaestra.Request.ConsultaTablaMaestraRequestData;
using Smadot.Models.Entities.Generic.Response;
using Newtonsoft.Json;
using Smadot.Models.Dicts.ProveedorDicts;
using Smadot.Models.Entities.ConsultaTablaMaestra.Request;
using Smadot.Models.Entities.ConsultaTablaMaestra.Response;
using System.ComponentModel.DataAnnotations;
using Smadot.Models.Entities.Catalogos.Request;
using Newtonsoft.Json.Linq;
using Namespace;
using Smadot.Models.GenericProcess;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
    public class ConsultaTablaMaestraNegocio : IConsultaTablaMaestraNegocio
    {
        private SmadotDbContext _context;
        private readonly SmadsotGenericInserts _smadsotGenericInserts;
        private readonly IConfiguration _configuration;

        public ConsultaTablaMaestraNegocio(SmadotDbContext context, SmadsotGenericInserts smadsotGenericInserts, IConfiguration configuration)
        {
            _context = context;
            _smadsotGenericInserts = smadsotGenericInserts;
            _configuration = configuration;
        }

        public async Task<ResponseGeneric<ResponseGrid<ConsultaTablaMaestraResponseGrid>>> Consulta(ConsultaTablaMaestraRequestList request)
        {
            try
            {
                ///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                ///REVISAR OPCIONES


                var catalogo = _context.vTablaMaestras.AsQueryable();
                //SE OBTIENE EL TOTAL DE REGISTROS PARA INFORMACION EN EL GRID
                var tot = catalogo.Count();
                var filtered = catalogo.Count();
                // if (request.IdCicloVerificacion != null && request.IdCicloVerificacion.Value > 0)
                // {
                //     catalogo = catalogo.Where(x => x.IdCicloVerificacion == request.IdCicloVerificacion.Value);
                // }

                if (request.IdSubMarca.HasValue)
                {
                    vCatSubMarcaVehiculo datosMarca = _context.vCatSubMarcaVehiculos.FirstOrDefault(x => x.Id == request.IdSubMarca) ?? new vCatSubMarcaVehiculo();

                    catalogo = catalogo.Where(x => x.IdCatMarcaVehiculo == datosMarca.IdCatMarcaVehiculo && x.IdCatSubmarcaVehiculo == datosMarca.Clave && ((request.IdTipoCombustible == Combustible.Diesel ? (x.CERO_DSL == 1) :
                        (request.IdTipoCombustible == Combustible.Gasolina ? x.CERO_GASOL == 1 :
                        (request.IdTipoCombustible == Combustible.GasNat ? x.CERO_GASNC == 1 :
                        request.IdTipoCombustible == Combustible.GasLp ? x.CERO_GASLP == 1 : false))) || request.ValidarCombustible == false));
                    filtered = catalogo.Count();
                }

                // if (!string.IsNullOrEmpty(request.Marca))
                // {
                //     catalogo = catalogo.Where(x => x.Marca == request.Marca);
                //     filtered = catalogo.Count();
                // }

                // if (!string.IsNullOrEmpty(request.SubMarca))
                // {
                //     catalogo = catalogo.Where(x => x.SubMarca == request.SubMarca);
                //     filtered = catalogo.Count();
                // }

                // if (!string.IsNullOrEmpty(request.Protocolo))
                // {
                //     int valres;
                //     var parseModelo = int.TryParse(request.Protocolo, out valres);
                //     if (parseModelo)
                //     {
                //         catalogo = catalogo.Where(x => x.PROTOCOLO == valres);
                //         filtered = catalogo.Count();
                //     }
                // }
                // if (!string.IsNullOrEmpty(request.Modelo))
                // {

                //     var anios = request.Modelo.Split('-');
                //     catalogo = catalogo.Where(x => x.ANO_DESDE.ToString().Equals(anios[0].Trim()) && x.ANO_HASTA.ToString().Equals(anios[1].Trim()));
                //     filtered = catalogo.Count();
                // }

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                    filtered = catalogo.Count();
                }
                //if (request.Activo.HasValue)
                //{
                //    catalogo = catalogo.Where(x => x.Activo == request.Activo);
                //}



                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                    x.Marca.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.SubMarca.ToLower().Contains(request.Busqueda.ToLower()) ||
                                                    x.PROTOCOLO.ToString() == request.Busqueda);
                    filtered = catalogo.Count();
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                    filtered = catalogo.Count();

                }

                string r2 = JsonConvert.SerializeObject(catalogo);
                var dataDB = JsonConvert.DeserializeObject<List<ConsultaTablaMaestraResponseGrid>>(r2);

                return new ResponseGeneric<ResponseGrid<ConsultaTablaMaestraResponseGrid>>(new ResponseGrid<ConsultaTablaMaestraResponseGrid>()
                {
                    Data = dataDB ?? new(),
                    RecordsTotal = tot,
                    RecordsFiltered = filtered,

                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<ConsultaTablaMaestraResponseGrid>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<ConsultaTablaMaestraCicloVerificacionResponse>>> ConsultaCicloVerificacion(RequestList request)
        {
            try
            {
                ///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
                ///REVISAR OPCIONES
                var catalogo = _context.CicloVerificacions.AsQueryable();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    catalogo = catalogo.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }
                //if (request.Activo.HasValue)
                //{
                //    catalogo = catalogo.Where(x => x.Activo == request.Activo);
                //}

                //SE OBTIENE EL TOTAL DE REGISTROS PARA INFORMACION EN EL GRID
                var tot = catalogo.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                {
                    catalogo = catalogo.Where(x =>
                                                    x.Nombre.ToLower().Contains(request.Busqueda.ToLower()));
                }
                if (request.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new ConsultaTablaMaestraCicloVerificacionResponse
                {
                    Id = x.Id,
                    FechaInicio = x.FechaInicio,
                    FechaFin = x.FechaFin,
                    Nombre = x.Nombre,
                    Activo = x.Activo,
                    ImporteFv = x.ImporteFv,
                    Total = tot,
                }).ToListAsync();

                return new ResponseGeneric<List<ConsultaTablaMaestraCicloVerificacionResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ConsultaTablaMaestraCicloVerificacionResponse>>(ex);
            }
        }
        public async Task<ResponseGeneric<bool>> ActualizarTablaMaestra(TablaMaestraDbfRequest request)
        {
            try
            {
                var result = false;
                if (request.TablaMaestra.Count > 0)
                {
                    // var ultimoRegistro = await _context.TablaMaestras.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
                    // if (ultimoRegistro != null)
                    //     request.TablaMaestra = request.TablaMaestra.Where(x => x.Id > ultimoRegistro.Id).ToList();
                    ProcessData(request.TablaMaestra);
                    result = true;
                }
                else if (request.CatTipoDiesel.Count > 0)
                {
                    //var ultimoRegistro = await _context.CatTipoDiesels.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
                    //if (ultimoRegistro != null)
                    //    request.CatTipoDiesel = request.CatTipoDiesel.Where(x => x.Id > ultimoRegistro.Id).ToList();
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        var existentes = await _context.CatTipoDiesels.Select(x => x.Id).ToListAsync();
                        _context.CatTipoDiesels.AddRange(request.CatTipoDiesel.Where(x => !existentes.Contains(x.Id)));
                        result = await _context.SaveChangesAsync() > 0;
                        await transaction.CommitAsync();
                    }
                }
                else if (request.CatSubdieselVehiculo.Count > 0)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        var rows = request.CatSubdieselVehiculo.Select(x => new CatSubdieselVehiculo
                        {
                            Clave = x.Clave,
                            Id = x.Id,
                            IdCatMarcaVehiculo = x.IdCatMarcaVehiculo,
                            Nombre = x.Nombre,
                            GOB_FAB = x.GOB_FAB,
                            POTMAX_RPM = x.POTMAX_RPM,
                            RAL_FAB = x.RAL_FAB
                        }).ToList();
                        var ultimoRegistro = await _context.CatSubdieselVehiculos.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
                        if (ultimoRegistro != null)
                        {
                            var index = rows.FindIndex(x => x.IdCatMarcaVehiculo == ultimoRegistro.IdCatMarcaVehiculo && x.Clave == ultimoRegistro.Clave);
                            if (index > -1)
                                rows.RemoveRange(0, index);
                        }
                        _context.CatSubdieselVehiculos.AddRange(rows);
                        result = await _context.SaveChangesAsync() > 0;
                        await transaction.CommitAsync();
                    }
                }
                else if (request.CatMarcaVehiculo.Count > 0)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        var existentes = await _context.CatMarcaVehiculos.Select(x => x.Id).ToListAsync();
                        _context.CatMarcaVehiculos.AddRange(request.CatMarcaVehiculo.Where(x => !existentes.Contains(x.Id)));
                        result = await _context.SaveChangesAsync() > 0;
                        await transaction.CommitAsync();
                    }
                }
                else if (request.CatSubMarcaVehiculo.Count > 0)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        var rows = request.CatSubMarcaVehiculo.Select(x => new CatSubMarcaVehiculo
                        {
                            Alias = x.Alias,
                            Clave = x.Clave,
                            Id = x.Id,
                            IdCatMarcaVehiculo = x.IdCatMarcaVehiculo,
                            Nombre = x.Nombre
                        }).ToList();
                        // var ultimoRegistro = await _context.CatSubMarcaVehiculos.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
                        // if (ultimoRegistro != null)
                        // {
                        //     var index = rows.FindIndex(x => x.IdCatMarcaVehiculo == ultimoRegistro.IdCatMarcaVehiculo && x.Clave == ultimoRegistro.Clave && ultimoRegistro.Nombre.Contains(x.Nombre) && ultimoRegistro.Alias.Contains(x.Alias));
                        //     if (index > -1)
                        //         rows.RemoveRange(0, index);
                        // }
                        _context.CatSubMarcaVehiculos.RemoveRange(_context.CatSubMarcaVehiculos);
                        await _context.SaveChangesAsync();
                        _context.CatSubMarcaVehiculos.AddRange(rows);
                        result = await _context.SaveChangesAsync() > 0;
                        await transaction.CommitAsync();
                    }
                }
                var message = result ? "La Tabla Maestra fue actualizada existosamente." : "No hay ningún registro nuevo en el archivo cargado";
                return new ResponseGeneric<bool>(result) { mensaje = message };
            }
            catch (DbUpdateException ex)
            {
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionActualizarTablaMaestra);
                if (request.CatSubdieselVehiculo.Count > 0)
                    return new ResponseGeneric<bool>(false) { mensaje = "El DIESEL es obligatorio, verifique los registros y cargue el archivo de DIESEL primero." };
                //if (ex.Entries.Count>0 && ex.Entries[0].Entity.GetType()== typeof(CatSubMarcaVehiculo))
                else if (request.CatSubMarcaVehiculo.Count > 0)
                    return new ResponseGeneric<bool>(false) { mensaje = "La MARCA es obligatoria, verifique los registros y cargue el archivo de MARCAS primero." };
                else
                    return new ResponseGeneric<bool>(false) { mensaje = "Ocurrió un error al registrar la información." };

            }
            catch (Exception ex)
            {
                await _smadsotGenericInserts.SaveLog(ex, DictTipoLog.ExcepcionActualizarTablaMaestra);
                return new ResponseGeneric<bool>(ex);
            }
        }


        public async Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteMarca(string prefix)
        {
            try
            {
                var verificaciones = _context.vTablaMaestras.Where(x => x.Marca.ToLower().Contains(prefix.ToLower())).GroupBy(x => x.Marca).AsQueryable();
                //var tot = verificaciones.Count();
                //verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                verificaciones = verificaciones.Take(10);
                var result = await verificaciones.Select(x => new TablaMaestraAutocompleteResponse
                {
                    Id = x.FirstOrDefault().Marca,
                    Text = x.FirstOrDefault().Marca
                }).ToListAsync();
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteSubmarca(string prefix)
        {
            try
            {
                var verificaciones = _context.vTablaMaestras.Where(x => x.SubMarca.ToLower().Contains(prefix.ToLower())).GroupBy(x => x.SubMarca).AsQueryable();
                //var tot = verificaciones.Count();
                //verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                verificaciones = verificaciones.Take(10);
                var result = await verificaciones.Select(x => new TablaMaestraAutocompleteResponse
                {
                    Id = x.FirstOrDefault().SubMarca,
                    Text = x.FirstOrDefault().SubMarca
                }).ToListAsync();
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<ResponseGrid<Catalogo>>> ConsultaSubMarca(SubMarcaVehiculoRequest request)
        {
            try
            {


                if (string.IsNullOrEmpty(request.Marca))
                {
                    return new ResponseGeneric<ResponseGrid<Catalogo>>(new ResponseGrid<Catalogo>());
                }
                // if (request.Anio != null)
                // {
                //     catalogo = catalogo.Where(x => request.Anio >= x.ANO_DESDE && request.Anio <= x.ANO_HASTA);
                // }

                // request.Marca = request?.Marca?.Replace(" ", "").ToLower();
                request.Busqueda = (request.Busqueda ?? "").ToLower();
                var catalogo = _context.vCatSubMarcaVehiculos.Where(x => x.Marca.Contains(request.Marca) && (x.Clave.ToString().Contains(request.Busqueda ?? "") ||
                x.Nombre.ToLower().Contains(request.Busqueda ?? "") || x.Nombre.ToLower().Replace("_", "").Contains(request.Busqueda ?? "")
                || x.Nombre.ToLower().Replace(" ", "").Contains(request.Busqueda ?? "") || x.Nombre.ToLower().Replace("-", "").Contains(request.Busqueda ?? "")
                || x.Nombre.ToLower().Replace(" ", "").Contains(request.Busqueda ?? "")
                || x.Nombre.ToLower().Replace("a", "").Replace("e", "").Replace("i", "").Replace("o", "").Replace("u", "")
                    .Contains((request.Busqueda ?? "").ToLower().Replace("a", "").Replace("e", "").Replace("i", "").Replace("o", "").Replace("u", "") ?? ""))).AsQueryable();
                var tot = catalogo.Count();
                if (request?.Pagina > 0 && request.Registros > 0)
                {
                    catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }

                var result = await catalogo.Select(x => new Catalogo
                {
                    Nombre = $"{x.Marca} {x.Nombre}",
                    //Value = request.SubmarcaClave ? x.IdCatSubmarcaVehiculo.ToString() : x.Id.ToString()
                    Id = x.Id
                }).ToListAsync();

                return new ResponseGeneric<ResponseGrid<Catalogo>>(new ResponseGrid<Catalogo>
                {
                    RecordsTotal = tot,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<Catalogo>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteProtocolo(string prefix)
        {
            try
            {
                var verificaciones = _context.TablaMaestras.Where(x => x.PROTOCOLO.ToString().Contains(prefix.ToLower())).GroupBy(x => x.PROTOCOLO).AsQueryable();
                //var tot = verificaciones.Count();
                //verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                verificaciones = verificaciones.Take(10);
                var result = await verificaciones.Select(x => new TablaMaestraAutocompleteResponse
                {
                    Id = x.FirstOrDefault().PROTOCOLO.ToString(),
                    Text = x.FirstOrDefault().PROTOCOLO.ToString()
                }).ToListAsync();
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteModelo(string prefix)
        {
            try
            {
                var anios = prefix.Split('-');
                var verificaciones = new List<vTablaMaestra>().AsQueryable();
                if (anios.Length < 2 && anios.Length > 0)
                {
                    verificaciones = _context.vTablaMaestras.Where(x => x.ANO_DESDE.ToString().Contains(anios[0].Trim()));
                }
                if (anios.Length > 1)
                {
                    verificaciones = _context.vTablaMaestras.Where(x => x.ANO_DESDE.ToString().Contains(anios[0].Trim()) && x.ANO_HASTA.ToString().Contains(anios[1].Trim()));
                }
                var verificacionesGroup = verificaciones.Where(x => string.Format("{0} - {1}", x.ANO_DESDE, x.ANO_HASTA).Contains(prefix)).GroupBy(x => new { x.ANO_DESDE, x.ANO_HASTA });
                //var tot = verificaciones.Count();
                //verificaciones = verificaciones.Skip(request.Start).Take(request.End);
                verificacionesGroup = verificacionesGroup.Take(10);
                var result = await verificacionesGroup.Select(x => new TablaMaestraAutocompleteResponse
                {
                    Id = $"{x.FirstOrDefault().ANO_DESDE} - {x.FirstOrDefault().ANO_HASTA}",
                    Text = $"{x.FirstOrDefault().ANO_DESDE} - {x.FirstOrDefault().ANO_HASTA}"
                }).ToListAsync();
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<TablaMaestraAutocompleteResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Registro(TablaMaestraRequest request)
        {
            try
            {

                var tabla = new TablaMaestra();
                var submarca = await _context.vCatSubMarcaVehiculos.FirstOrDefaultAsync(x => x.Id == request.IdRegistroSubMarca);
                if (submarca == null)
                {
                    throw new ValidationException("No se encontró registro de la submarca");
                }
                request.IdCatSubmarcaVehiculo = submarca.Clave;
                request.IdCatMarcaVehiculo = submarca.IdCatMarcaVehiculo;
                var existeRegistro = await _context.TablaMaestras.FirstOrDefaultAsync(x =>
                    x.Id != request.Id &&
                    x.IdCatMarcaVehiculo == request.IdCatMarcaVehiculo &&
                    x.IdCatSubmarcaVehiculo == request.IdCatSubmarcaVehiculo &&
                    x.Motor_DSL == request.Motor_DSL &&
                    x.COMB_ORIG == request.COMB_ORIG &&
                    x.CARROCERIA == request.CARROCERIA &&
                    x.ALIM_COMB == request.ALIM_COMB &&
                    x.CILINDROS == request.CILINDROS &&
                    x.CILINDRADA == request.CILINDRADA &&
                    x.PBV == request.PBV &&
                    x.PBV_EQUIV == request.PBV_EQUIV &&
                    x.PBV_ASM == request.PBV_ASM &&
                    x.CONV_CATAL == request.CONV_CATAL &&
                    x.OBD == request.OBD &&
                    x.C_ABS == request.C_ABS &&
                    x.T_TRACC == request.T_TRACC &&
                    x.C_TRACC == request.C_TRACC &&
                    x.T_PRUEBA == request.T_PRUEBA &&
                    x.PROTOCOLO == request.PROTOCOLO &&
                    x.POTMAX_RPM == request.POTMAX_RPM &&
                    x.ANO_DESDE == request.ANO_DESDE &&
                    x.ANO_HASTA == request.ANO_HASTA &&
                    x.O2_MAX == request.O2_MAX &&
                    x.LAMDA_MAX == request.LAMDA_MAX &&
                    x.POT_5024 == request.POT_5024 &&
                    x.POT_2540 == request.POT_2540 &&
                    x.DOBLECERO == request.DOBLECERO &&
                    x.CERO_GASOL == request.CERO_GASOL &&
                    x.CERO_GASLP == request.CERO_GASLP &&
                    x.CERO_GASNC == request.CERO_GASNC &&
                    x.CERO_DSL == request.CERO_DSL &&
                    (x.REF_00 > 0) == (request.REF_00 > 0)
                );
                if (existeRegistro != null)
                {
                    throw new ValidationException($"Ya hay un registro con la misma información. Es el {existeRegistro.Id}");
                }
                if (request.Id > 0)
                {
                    tabla = _context.TablaMaestras.FirstOrDefault(x => x.Id == request.Id);
                    if (tabla != null)
                    {
                        tabla.Id = request.Id;
                        tabla.IdCatMarcaVehiculo = request.IdCatMarcaVehiculo.Value;
                        tabla.IdCatSubmarcaVehiculo = request.IdCatSubmarcaVehiculo.Value;
                        tabla.Motor_DSL = request.Motor_DSL;
                        tabla.COMB_ORIG = request.COMB_ORIG;
                        tabla.CARROCERIA = request.CARROCERIA;
                        tabla.ALIM_COMB = request.ALIM_COMB;
                        tabla.CILINDROS = request.CILINDROS;
                        tabla.CILINDRADA = request.CILINDRADA;
                        tabla.PBV = request.PBV;
                        tabla.PBV_EQUIV = request.PBV_EQUIV;
                        tabla.PBV_ASM = request.PBV_ASM;
                        tabla.CONV_CATAL = request.CONV_CATAL;
                        tabla.OBD = request.OBD;
                        tabla.C_ABS = request.C_ABS;
                        tabla.T_TRACC = request.T_TRACC;
                        tabla.C_TRACC = request.C_TRACC;
                        tabla.T_PRUEBA = request.T_PRUEBA;
                        tabla.PROTOCOLO = request.PROTOCOLO;
                        tabla.POTMAX_RPM = request.POTMAX_RPM;
                        tabla.ANO_DESDE = request.ANO_DESDE;
                        tabla.ANO_HASTA = request.ANO_HASTA;
                        tabla.O2_MAX = request.O2_MAX;
                        tabla.LAMDA_MAX = request.LAMDA_MAX;
                        tabla.POT_5024 = request.POT_5024;
                        tabla.POT_2540 = request.POT_2540;
                        tabla.DOBLECERO = request.DOBLECERO;
                        tabla.CERO_GASOL = request.CERO_GASOL;
                        tabla.CERO_GASLP = request.CERO_GASLP;
                        tabla.CERO_GASNC = request.CERO_GASNC;
                        tabla.CERO_DSL = request.CERO_DSL;
                        tabla.REF_00 = request.REF_00 > 0 ? 2 : 0;
                    }
                }
                else
                {
                    tabla = _context.TablaMaestras.FirstOrDefault();
                    tabla = new TablaMaestra
                    {
                        Id = await GenerateRandomId(),
                        IdCatMarcaVehiculo = request.IdCatMarcaVehiculo.Value,
                        IdCatSubmarcaVehiculo = request.IdCatSubmarcaVehiculo.Value,
                        Motor_DSL = request.Motor_DSL,
                        COMB_ORIG = request.COMB_ORIG,
                        CARROCERIA = request.CARROCERIA,
                        ALIM_COMB = request.ALIM_COMB,
                        CILINDROS = request.CILINDROS,
                        CILINDRADA = request.CILINDRADA,
                        PBV = request.PBV,
                        PBV_EQUIV = request.PBV_EQUIV,
                        PBV_ASM = request.PBV_ASM,
                        CONV_CATAL = request.CONV_CATAL,
                        OBD = request.OBD,
                        C_ABS = request.C_ABS,
                        T_TRACC = request.T_TRACC,
                        C_TRACC = request.C_TRACC,
                        T_PRUEBA = request.T_PRUEBA,
                        PROTOCOLO = request.PROTOCOLO,
                        POTMAX_RPM = request.POTMAX_RPM,
                        ANO_DESDE = request.ANO_DESDE,
                        ANO_HASTA = request.ANO_HASTA,
                        O2_MAX = request.O2_MAX,
                        LAMDA_MAX = request.LAMDA_MAX,
                        POT_5024 = request.POT_5024,
                        POT_2540 = request.POT_2540,
                        DOBLECERO = request.DOBLECERO,
                        CERO_GASOL = request.CERO_GASOL,
                        CERO_GASLP = request.CERO_GASLP,
                        CERO_GASNC = request.CERO_GASNC,
                        CERO_DSL = request.CERO_DSL,
                        REF_00 = request.REF_00 > 0 ? 2 : 0
                    };

                    _context.TablaMaestras.Add(tabla);
                }
                var result = await _context.SaveChangesAsync() > 0;

                return new ResponseGeneric<bool>(result);

            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<bool>(ex, ex.Message);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(false) { mensaje = "Ocurrió un error al guardar la información del registro de la tabla maestra." };
            }
        }

        public async Task<ResponseGeneric<RegistroTablaMaestraResponse>> GetById(long id)
        {
            try
            {
                var tabla = await _context.vTablaMaestras.FirstOrDefaultAsync(x => x.Id == id);
                if (tabla is null)
                    tabla = new vTablaMaestra();
                var marcas = await _context.CatMarcaVehiculos.AsQueryable().Select(x => new Catalogo
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                }).ToListAsync();
                var protocolos = await _context.CatTipoProtocolos.AsQueryable().Select(x => new Catalogo
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                }).ToListAsync();
                var jsonTablaMaestra = JsonConvert.SerializeObject(tabla);
                var result = new RegistroTablaMaestraResponse
                {
                    TablaMaestra = JsonConvert.DeserializeObject<TablaMaestraResponse>(jsonTablaMaestra) ?? new TablaMaestraResponse(),
                    Protocolos = protocolos,
                    Marcas = marcas
                };
                return new ResponseGeneric<RegistroTablaMaestraResponse>(result);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<RegistroTablaMaestraResponse>(ex, ex.Message);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<RegistroTablaMaestraResponse>(ex);
            }
        }

        public async Task SaveLog(Exception ex, string type)
        {
            try
            {
                JObject exceptioJson = new()
                {
                    ["StackTrace"] = ex.StackTrace,
                    ["Source"] = ex.Source,
                    ["Message"] = ex.Message,
                    ["InnerException"] = ex.InnerException?.Message ?? "",
                    ["InnerExceptionData"] = JObject.FromObject(ex.InnerException?.Data ?? new object()),
                    ["InnerExceptionDataSource"] = ex.InnerException?.Source ?? "",
                };
                JObject data = new()
                {
                    ["Exception"] = exceptioJson,
                    ["Type"] = type
                };
                _context.Errors.Add(new()
                {
                    Created = DateTime.Now,
                    Values = JsonConvert.SerializeObject(data)
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        #region private Methods

        private async Task<long> GenerateRandomId()
        {
            Random random = new Random();
            int maxAttempts = 40; // Establece un límite máximo de intentos

            for (int i = 0; i < maxAttempts; i++)
            {
                int numeroAleatorio = random.Next(100000, 200001);

                var existeRegistro = await _context.TablaMaestras.AnyAsync(x => x.Id == numeroAleatorio);
                if (!existeRegistro)
                {
                    return numeroAleatorio;
                }
            }

            throw new ValidationException("No se pudo generar un ID. Vuelva a intentarlo.");
        }
        private void ProcessData(List<TablaMaestra> data)
        {

            using (SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // data = data.OrderBy(x => x.Id).ToList();
                        // // Obtener los IDs existentes en la base de datos
                        // List<long> existingIds = GetExistingAndNotExistingIds(ids, connection, transaction);
                        // HashSet<long> existingIdsSet = new HashSet<long>(existingIds);
                        // // Separar los datos en dos listas
                        // List<TablaMaestra> itemsToUpdate = new();
                        // List<TablaMaestra> itemsToInsert = new();
                        // foreach (var id in existingIdsSet)
                        // {
                        //     // Verificar si el ID está presente en data
                        //     var item = data.FirstOrDefault(d => d.Id == id);
                        //     if (item != null)
                        //     {
                        //         itemsToUpdate.Add(item);
                        //     }
                        // }
                        // HashSet<long> notExistingIdsSet = new HashSet<long>();
                        // int i = 0;
                        // foreach (var id in ids)
                        // {
                        //     Console.WriteLine($"Verificando ID: {id}");
                        //     if (!existingIdsSet.Contains(id))
                        //     {
                        //         // Depuración: Imprimir el ID actual para verificar

                        //         // Buscar el item en data que coincida con el ID
                        //         var item = data.FirstOrDefault(d => d.Id == id);

                        //         // Depuración: Imprimir el item encontrado (debería ser null si no existe)
                        //         Console.WriteLine($"Item encontrado: {item}");

                        //         if (item != null)
                        //         {
                        //             itemsToInsert.Add(item);
                        //         }
                        //     }
                        //     i++;
                        //     Console.WriteLine($"Contador: {i }");

                        // }


                        // Actualizar los elementos existentes
                        // UpdateItems(itemsToUpdate, connection, transaction);

                        // Insertar los elementos nuevos
                        InsertItems(data, connection, transaction);

                        // Confirmar la transacción
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Deshacer la transacción en caso de error
                        transaction.Rollback();
                        throw new Exception("Error processing data", ex);
                    }
                }
            }
        }

        private List<long> GetExistingAndNotExistingIds(List<long> ids, SqlConnection connection, SqlTransaction transaction)
        {
            // Step 1: Create a temporary table for the input IDs
            string createTempTableQuery = @"
            CREATE TABLE #TempIds (Id BIGINT);";
            using (SqlCommand createCmd = new SqlCommand(createTempTableQuery, connection, transaction))
            {
                createCmd.ExecuteNonQuery();
            }

            // Step 2: Bulk insert IDs into the temporary table
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.DestinationTableName = "#TempIds";
                bulkCopy.WriteToServer(CreateDataTable(ids));
            }

            List<long> existingIds = new List<long>();
            string selectExistingIdsQuery = @"
            SELECT Id 
            FROM TablaMaestra WITH (READCOMMITTEDLOCK)
            WHERE Id IN (SELECT Id FROM #TempIds) ORDER BY Id;";

            using (SqlCommand selectExistingIdsCmd = new SqlCommand(selectExistingIdsQuery, connection, transaction))
            {
                using (SqlDataReader reader = selectExistingIdsCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingIds.Add(reader.GetInt64(0));
                    }
                }
            }
            // Clean up temporary table
            string dropTempTableQuery = @"DROP TABLE #TempIds;";
            using (SqlCommand dropCmd = new SqlCommand(dropTempTableQuery, connection, transaction))
            {
                dropCmd.ExecuteNonQuery();
            }

            return existingIds;
        }



        private static void UpdateItems(List<TablaMaestra> items, SqlConnection connection, SqlTransaction transaction)
        {
            string updateQuery = @"
            UPDATE TablaMaestra
            SET
                IdCatMarcaVehiculo = @IdCatMarcaVehiculo,
                IdCatSubmarcaVehiculo = @IdCatSubmarcaVehiculo,
                Motor_DSL = @Motor_DSL,
                COMB_ORIG = @COMB_ORIG,
                CARROCERIA = @CARROCERIA,
                ALIM_COMB = @ALIM_COMB,
                CILINDROS = @CILINDROS,
                CILINDRADA = @CILINDRADA,
                PBV = @PBV,
                PBV_EQUIV = @PBV_EQUIV,
                PBV_ASM = @PBV_ASM,
                CONV_CATAL = @CONV_CATAL,
                OBD = @OBD,
                C_ABS = @C_ABS,
                T_TRACC = @T_TRACC,
                C_TRACC = @C_TRACC,
                T_PRUEBA = @T_PRUEBA,
                PROTOCOLO = @PROTOCOLO,
                POTMAX_RPM = @POTMAX_RPM,
                ANO_DESDE = @ANO_DESDE,
                ANO_HASTA = @ANO_HASTA,
                O2_MAX = @O2_MAX,
                LAMDA_MAX = @LAMDA_MAX,
                POT_5024 = @POT_5024,
                POT_2540 = @POT_2540,
                DOBLECERO = @DOBLECERO,
                CERO_GASOL = @CERO_GASOL,
                CERO_GASLP = @CERO_GASLP,
                CERO_GASNC = @CERO_GASNC,
                CERO_DSL = @CERO_DSL,
                REF_00 = @REF_00
            WHERE Id = @Id";

            foreach (var item in items)
            {
                using (SqlCommand command = new SqlCommand(updateQuery, connection, transaction))
                {
                    command.CommandTimeout = 1200;

                    command.Parameters.AddWithValue("@Id", item.Id);
                    command.Parameters.AddWithValue("@IdCatMarcaVehiculo", item.IdCatMarcaVehiculo);
                    command.Parameters.AddWithValue("@IdCatSubmarcaVehiculo", item.IdCatSubmarcaVehiculo);
                    command.Parameters.AddWithValue("@Motor_DSL", item.Motor_DSL);
                    command.Parameters.AddWithValue("@COMB_ORIG", item.COMB_ORIG);
                    command.Parameters.AddWithValue("@CARROCERIA", item.CARROCERIA);
                    command.Parameters.AddWithValue("@ALIM_COMB", item.ALIM_COMB);
                    command.Parameters.AddWithValue("@CILINDROS", item.CILINDROS);
                    command.Parameters.AddWithValue("@CILINDRADA", item.CILINDRADA);
                    command.Parameters.AddWithValue("@PBV", item.PBV);
                    command.Parameters.AddWithValue("@PBV_EQUIV", item.PBV_EQUIV);
                    command.Parameters.AddWithValue("@PBV_ASM", item.PBV_ASM);
                    command.Parameters.AddWithValue("@CONV_CATAL", item.CONV_CATAL);
                    command.Parameters.AddWithValue("@OBD", item.OBD);
                    command.Parameters.AddWithValue("@C_ABS", item.C_ABS);
                    command.Parameters.AddWithValue("@T_TRACC", item.T_TRACC);
                    command.Parameters.AddWithValue("@C_TRACC", item.C_TRACC);
                    command.Parameters.AddWithValue("@T_PRUEBA", item.T_PRUEBA);
                    command.Parameters.AddWithValue("@PROTOCOLO", item.PROTOCOLO);
                    command.Parameters.AddWithValue("@POTMAX_RPM", item.POTMAX_RPM);
                    command.Parameters.AddWithValue("@ANO_DESDE", item.ANO_DESDE);
                    command.Parameters.AddWithValue("@ANO_HASTA", item.ANO_HASTA);
                    command.Parameters.AddWithValue("@O2_MAX", item.O2_MAX);
                    command.Parameters.AddWithValue("@LAMDA_MAX", item.LAMDA_MAX);
                    command.Parameters.AddWithValue("@POT_5024", item.POT_5024);
                    command.Parameters.AddWithValue("@POT_2540", item.POT_2540);
                    command.Parameters.AddWithValue("@DOBLECERO", item.DOBLECERO);
                    command.Parameters.AddWithValue("@CERO_GASOL", item.CERO_GASOL);
                    command.Parameters.AddWithValue("@CERO_GASLP", item.CERO_GASLP);
                    command.Parameters.AddWithValue("@CERO_GASNC", item.CERO_GASNC);
                    command.Parameters.AddWithValue("@CERO_DSL", item.CERO_DSL);
                    command.Parameters.AddWithValue("@REF_00", item.REF_00);

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertItems(List<TablaMaestra> items, SqlConnection connection, SqlTransaction transaction)
        {
            // Consulta para eliminar todos los registros de la tabla
            string deleteQuery = @"DELETE FROM TablaMaestra";

            // Ejecutar la consulta de eliminación
            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction))
            {
                deleteCommand.CommandTimeout = 1200;

                deleteCommand.ExecuteNonQuery();
            }
            string insertQuery = @"
            INSERT INTO TablaMaestra
            (
                Id, IdCatMarcaVehiculo, IdCatSubmarcaVehiculo, Motor_DSL, COMB_ORIG, CARROCERIA, ALIM_COMB,
                CILINDROS, CILINDRADA, PBV, PBV_EQUIV, PBV_ASM, CONV_CATAL, OBD, C_ABS, T_TRACC, C_TRACC,
                T_PRUEBA, PROTOCOLO, POTMAX_RPM, ANO_DESDE, ANO_HASTA, O2_MAX, LAMDA_MAX, POT_5024,
                POT_2540, DOBLECERO, CERO_GASOL, CERO_GASLP, CERO_GASNC, CERO_DSL, REF_00
            )
            VALUES
            (
                @Id, @IdCatMarcaVehiculo, @IdCatSubmarcaVehiculo, @Motor_DSL, @COMB_ORIG, @CARROCERIA, @ALIM_COMB,
                @CILINDROS, @CILINDRADA, @PBV, @PBV_EQUIV, @PBV_ASM, @CONV_CATAL, @OBD, @C_ABS, @T_TRACC, @C_TRACC,
                @T_PRUEBA, @PROTOCOLO, @POTMAX_RPM, @ANO_DESDE, @ANO_HASTA, @O2_MAX, @LAMDA_MAX, @POT_5024,
                @POT_2540, @DOBLECERO, @CERO_GASOL, @CERO_GASLP, @CERO_GASNC, @CERO_DSL, @REF_00
            )";

            foreach (var item in items)
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection, transaction))
                {
                    command.CommandTimeout = 1200;
                    command.Parameters.AddWithValue("@Id", item.Id);
                    command.Parameters.AddWithValue("@IdCatMarcaVehiculo", item.IdCatMarcaVehiculo);
                    command.Parameters.AddWithValue("@IdCatSubmarcaVehiculo", item.IdCatSubmarcaVehiculo);
                    command.Parameters.AddWithValue("@Motor_DSL", item.Motor_DSL);
                    command.Parameters.AddWithValue("@COMB_ORIG", item.COMB_ORIG);
                    command.Parameters.AddWithValue("@CARROCERIA", item.CARROCERIA);
                    command.Parameters.AddWithValue("@ALIM_COMB", item.ALIM_COMB);
                    command.Parameters.AddWithValue("@CILINDROS", item.CILINDROS);
                    command.Parameters.AddWithValue("@CILINDRADA", item.CILINDRADA);
                    command.Parameters.AddWithValue("@PBV", item.PBV);
                    command.Parameters.AddWithValue("@PBV_EQUIV", item.PBV_EQUIV);
                    command.Parameters.AddWithValue("@PBV_ASM", item.PBV_ASM);
                    command.Parameters.AddWithValue("@CONV_CATAL", item.CONV_CATAL);
                    command.Parameters.AddWithValue("@OBD", item.OBD);
                    command.Parameters.AddWithValue("@C_ABS", item.C_ABS);
                    command.Parameters.AddWithValue("@T_TRACC", item.T_TRACC);
                    command.Parameters.AddWithValue("@C_TRACC", item.C_TRACC);
                    command.Parameters.AddWithValue("@T_PRUEBA", item.T_PRUEBA);
                    command.Parameters.AddWithValue("@PROTOCOLO", item.PROTOCOLO);
                    command.Parameters.AddWithValue("@POTMAX_RPM", item.POTMAX_RPM);
                    command.Parameters.AddWithValue("@ANO_DESDE", item.ANO_DESDE);
                    command.Parameters.AddWithValue("@ANO_HASTA", item.ANO_HASTA);
                    command.Parameters.AddWithValue("@O2_MAX", item.O2_MAX);
                    command.Parameters.AddWithValue("@LAMDA_MAX", item.LAMDA_MAX);
                    command.Parameters.AddWithValue("@POT_5024", item.POT_5024);
                    command.Parameters.AddWithValue("@POT_2540", item.POT_2540);
                    command.Parameters.AddWithValue("@DOBLECERO", item.DOBLECERO);
                    command.Parameters.AddWithValue("@CERO_GASOL", item.CERO_GASOL);
                    command.Parameters.AddWithValue("@CERO_GASLP", item.CERO_GASLP);
                    command.Parameters.AddWithValue("@CERO_GASNC", item.CERO_GASNC);
                    command.Parameters.AddWithValue("@CERO_DSL", item.CERO_DSL);
                    command.Parameters.AddWithValue("@REF_00", item.REF_00);

                    command.ExecuteNonQuery();
                }
            }
        }

        private DataTable CreateDataTable(List<long> ids)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(long));

            foreach (var id in ids)
            {
                table.Rows.Add(id);
            }

            return table;
        }
        #endregion
    }

    public interface IConsultaTablaMaestraNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<ConsultaTablaMaestraResponseGrid>>> Consulta(ConsultaTablaMaestraRequestList request);
        public Task<ResponseGeneric<ResponseGrid<Catalogo>>> ConsultaSubMarca(SubMarcaVehiculoRequest request);
        public Task<ResponseGeneric<List<ConsultaTablaMaestraCicloVerificacionResponse>>> ConsultaCicloVerificacion(RequestList request);
        public Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteMarca(string prefix);
        public Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteSubmarca(string prefix);
        public Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteProtocolo(string prefix);
        public Task<ResponseGeneric<List<TablaMaestraAutocompleteResponse>>> AutocompleteModelo(string prefix);
        public Task<ResponseGeneric<bool>> ActualizarTablaMaestra(TablaMaestraDbfRequest request);
        public Task<ResponseGeneric<bool>> Registro(TablaMaestraRequest request);
        public Task<ResponseGeneric<RegistroTablaMaestraResponse>> GetById(long Id);

    }
}
