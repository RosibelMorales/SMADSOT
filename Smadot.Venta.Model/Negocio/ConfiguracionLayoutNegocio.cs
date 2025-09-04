using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using Polly;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.ConfiguracionLayout.Response;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Smadot.Venta.Model.Negocio
{
    public class ConfiguracionLayoutNegocio : IConfiguracionLayoutNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        public ConfiguracionLayoutNegocio(SmadotDbContext context, IUserResolver userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        public async Task<ResponseGeneric<List<ConfiguracionLayoutResponse>>> Consulta()
        {
            try
            {
                var user = _userResolver.GetUser();
                var result = await _context.vFoliosFormaValoradaVerificentroActuales.Where(x => x.IdVerificentro == user.IdVerificentro).Select(x => new ConfiguracionLayoutResponse
                {
                    Id = x.Id,
                    IdVerificentro = x.IdVerificentro,
                    CatTipoCertificado = x.CatTipoCertificado,
                    Folio = x.Folio + 1,
                    IdCatTipoCertificado = x.IdCatTipoCertificado,
                    Verificentro = x.Verificentro
                }).ToListAsync();
                var tiposCertificado = user.ClaveVerificentro.Equals("SMADSOT-00") ? TipoCertificado.DictNombreCertificado : TipoCertificado.DictNombreCertificadoCVV;
                foreach (var item in tiposCertificado)
                {
                    if (!result.Exists(x => x.IdCatTipoCertificado == item.Key))
                    {
                        result.Add(new ConfiguracionLayoutResponse
                        {
                            IdCatTipoCertificado = item.Key,
                            CatTipoCertificado = item.Value,
                            Verificentro = user.NombreVerificentro,
                            Folio = 1,
                            ClaveCertificado = TipoCertificado.DictNombreClave[item.Key]
                        });
                    }
                }
                result = result.OrderBy(x => x.ClaveCertificado).ToList();
                var precio = _context.CicloVerificacions.FirstOrDefault(x => x.Activo);
                result.InsertRange(0, new List<ConfiguracionLayoutResponse>
                {
                    new ConfiguracionLayoutResponse
                    {
                    Precio = precio?.ImporteFv ?? new decimal(),
                    TipoTramiteLayout = 1,
                    CatTipoCertificado="Verificaciones"
                    },
                    new ConfiguracionLayoutResponse
                    {
                    Precio = precio?.ImporteExento ?? new decimal(),
                    TipoTramiteLayout = 2,
                    CatTipoCertificado="Exentos"
                    },
                    new ConfiguracionLayoutResponse
                    {
                    Precio = precio?.ImporteTestificacion ?? new decimal(),
                    TipoTramiteLayout = 3,
                    CatTipoCertificado="Testificaciones"
                    },
                    new ConfiguracionLayoutResponse
                    {
                    Precio = precio?.ImporteAdministrativo ?? new decimal(),
                    TipoTramiteLayout = 4,
                    CatTipoCertificado="Administrativas"
                    },
                    new ConfiguracionLayoutResponse
                    {
                    Precio = precio?.ImporteReposicion ?? new decimal(),
                    TipoTramiteLayout = 5,
                    CatTipoCertificado="Reposiciones"
                    },
                    new ConfiguracionLayoutResponse
                    {
                    Precio = precio?.ImporteConstanciaUltimaVer ?? new decimal(),
                    TipoTramiteLayout = 6,
                    CatTipoCertificado="Constancias de última verificación"
                    }
                });
                return new ResponseGeneric<List<ConfiguracionLayoutResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<ConfiguracionLayoutResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(List<ConfiguracionLayoutResponse> request)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = false;
                    var modificarPrecio = request.Where(x => x.IdCatTipoCertificado == null && x.Selected);
                    var cicloActual = _context.CicloVerificacions.FirstOrDefault(x => x.Activo);
                    foreach (var i in modificarPrecio)
                    {
                        switch (i.TipoTramiteLayout)
                        {
                            case 1:
                                cicloActual.ImporteFv = i.Precio;
                                break;
                            case 2:
                                cicloActual.ImporteExento = i.Precio;
                                break;
                            case 3:
                                cicloActual.ImporteTestificacion = i.Precio;
                                break;
                            case 4:
                                cicloActual.ImporteAdministrativo = i.Precio;
                                break;
                            case 5:
                                cicloActual.ImporteReposicion = i.Precio;
                                break;
                            case 6:
                                cicloActual.ImporteConstanciaUltimaVer = i.Precio;
                                break;
                        }
                    }
                    result = await _context.SaveChangesAsync() > 0;

                    var user = _userResolver.GetUser();
                    var idVerific = user.IdVerificentro ?? 0;
                    var formas = _context.FoliosFormaValoradaActuales.Where(x => x.IdVerificentro == idVerific).ToList();
                    var date = DateTime.Now;
                    foreach (var config in request.Where(x => x.Selected && x.IdCatTipoCertificado > 0))
                    {
                        if (config.Folio < 1)
                        {
                            throw new ValidationException("El folio ingresado debe ser mayor a 0.");
                        }
                        var folio = formas.FirstOrDefault(x => x.IdCatTipoCertificado == config.IdCatTipoCertificado);
                        folio ??= new()
                        {
                            IdCatTipoCertificado = config.IdCatTipoCertificado,
                            IdVerificentro = idVerific,
                            IdFolioFormaValoradaVerificentro = null,
                            FechaCreacion = date,
                            FechaModificacion = date,
                        };
                        folio.Folio = config.Folio - 1;
                        if (folio.Id == 0)
                            await _context.FoliosFormaValoradaActuales.AddAsync(folio);
                        else
                            _context.Update(folio);
                    }
                    await _context.SaveChangesAsync();

                    scope.Complete();
                    return result ? new ResponseGeneric<long>(1) : new ResponseGeneric<long>(1);
                }
            }
            catch (ValidationException ex)
            {

                return new ResponseGeneric<long>(ex) { mensaje = "Ocurrió un error al guardar la información." };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "Ocurrió un error al guardar la información." };
            }
        }
    }

    public interface IConfiguracionLayoutNegocio
    {
        public Task<ResponseGeneric<List<ConfiguracionLayoutResponse>>> Consulta();
        public Task<ResponseGeneric<long>> Registro(List<ConfiguracionLayoutResponse> request);

    }
}
