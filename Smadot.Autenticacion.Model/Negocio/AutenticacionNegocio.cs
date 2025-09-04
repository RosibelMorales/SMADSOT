using Smadot.Utilities.Modelos;
using Microsoft.EntityFrameworkCore;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Autenticacion.Response;
using Smadot.Models.Entities.Autenticacion.Request;
using System.ComponentModel.DataAnnotations;

namespace Smadot.Autenticacion.Model.Negocio
{
    public class AutenticacionNegocio : IAutenticacionNegocio
    {
        private SmadotDbContext _context;
        public AutenticacionNegocio(SmadotDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseGeneric<AutenticacionResponse>> Consultar(AutenticacionRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Where(x => (x.UserName.Equals(request.Usuario) || x.Email.Equals(request.Usuario)) && x.PasswordHash == request.Contrasenia && !x.LockoutEnabled && x.EmailConfirmed && (x.IdRols.Any(y => y.AccesoTotalVerificentros) || x.UserPuestoVerificentroIdUserNavigations.Any(y => y.IdVerificentro == request.IdVerificentro) || x.IdVerificentro == request.IdVerificentro))
                    .Select(x => new AutenticacionResponse
                    {
                        IdUser = x.Id,
                        UserName = x.UserName,
                        Nombre = x.Nombre,
                    }).FirstOrDefaultAsync() ?? throw new ValidationException("No fue posible iniciar sesión. Usuario o contraseña son incorrectos.");
                //Obtenemos los Roles, Permisos e información adicional que se utilizará a lo largo de la aplicación
                var roles = _context.Rols.Include(x => x.IdPermisos).Where(x => x.IdUsers.Any(u => u.Id == user.IdUser));
                var fechaActual = DateTime.Now;
                var semestre = fechaActual.Month <= 6 ? 1 : 2;
                var ciclosVerificacion = _context.CicloVerificacions;
                var cicloVerificacion = ciclosVerificacion.FirstOrDefault(x => (x.FechaInicio.Month <= 6 ? 1 : 2) == semestre && fechaActual.Year == x.FechaInicio.Year && x.Activo);
                var ultimoCicloVerificacion = _context.CicloVerificacions.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Activo);
                if (cicloVerificacion == null)
                {
                    if (semestre == 1)
                    {
                        cicloVerificacion = new CicloVerificacion
                        {
                            Activo = true,
                            Nombre = $"Primer Semestre {fechaActual.Year}",
                            FechaInicio = new DateTime(fechaActual.Year, 1, 1, 0, 0, 0),
                            FechaFin = new DateTime(fechaActual.Year, 6, 30, 23, 59, 59),
                            ImporteAdministrativo = ultimoCicloVerificacion?.ImporteAdministrativo ?? 0,
                            ImporteExento = ultimoCicloVerificacion?.ImporteExento ?? 0,
                            ImporteConstanciaUltimaVer = ultimoCicloVerificacion?.ImporteConstanciaUltimaVer ?? 0,
                            ImporteFv = ultimoCicloVerificacion?.ImporteFv ?? 0,
                            ImporteReposicion = ultimoCicloVerificacion?.ImporteReposicion ?? 0,
                            ImporteTestificacion = ultimoCicloVerificacion?.ImporteTestificacion ?? 0
                        };
                    }
                    else
                    {
                        cicloVerificacion = new CicloVerificacion
                        {
                            Activo = true,
                            Nombre = $"Segundo Semestre {fechaActual.Year}",
                            FechaInicio = new DateTime(fechaActual.Year, 7, 1, 0, 0, 0),
                            FechaFin = new DateTime(fechaActual.Year, 12, 31, 23, 59, 59),
                            ImporteAdministrativo = ultimoCicloVerificacion?.ImporteAdministrativo ?? 0,
                            ImporteExento = ultimoCicloVerificacion?.ImporteExento ?? 0,
                            ImporteConstanciaUltimaVer = ultimoCicloVerificacion?.ImporteConstanciaUltimaVer ?? 0,
                            ImporteFv = ultimoCicloVerificacion?.ImporteFv ?? 0,
                            ImporteReposicion = ultimoCicloVerificacion?.ImporteReposicion ?? 0,
                            ImporteTestificacion = ultimoCicloVerificacion?.ImporteTestificacion ?? 0
                        };

                    }
                    await _context.CicloVerificacions.AddAsync(cicloVerificacion);
                    await _context.SaveChangesAsync();
                }
                // Validamos que semestre es
                user.Semestre = semestre;
                user.CicloVerificacion = cicloVerificacion.Nombre;
                user.IdCicloVerificacion = cicloVerificacion.Id;
                user.Roles = roles.Select(x => x.Id).ToList();
                user.RoleNames = roles.Select(x => x.Nombre).ToList();
                user.Permisos = new List<long>();
                foreach (var role in roles)
                {
                    if (role.IdPermisos != null)
                    {
                        user.Permisos.AddRange(role.IdPermisos.Select(x => x.Id).ToList());
                    }
                }

                if (user.Permisos != null)
                {
                    user.Permisos = user.Permisos.GroupBy(x => x).Select(x => x.Key).ToList();
                }

                var verificentro = _context.vVerificentros.FirstOrDefault(v => v.Id == request.IdVerificentro);
                user.IdVerificentro = request.IdVerificentro;
                user.NombreVerificentro = verificentro.NombreCorto;
                user.ClaveVerificentro = verificentro.Clave;

                return new ResponseGeneric<AutenticacionResponse>(user);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<AutenticacionResponse>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<AutenticacionResponse>(ex) { mensaje = "El usuario o contraseña son incorrectos." };
            }
        }
        public async Task<ResponseGeneric<bool>> Registro(RegistroRequest request)
        {
            try
            {
                var user = new User
                {
                    Email = request.Email,
                    AccessFailedCount = 0,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    //LockoutEnd = DateTime.Now.AddDays(5),
                    PasswordHash = request.Contrasenia,
                    PhoneNumber = string.Empty,
                    PhoneNumberConfirmed = true,
                    UserName = $"{request.Nombre}.{request.APaterno}".ToLower(),
                };
                _context.Users.Add(user);
                var result = await _context.SaveChangesAsync() > 0;
                return new ResponseGeneric<bool>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> CambiarContrasenia(CambioContraseniaRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.IdUser && x.PasswordHash == request.ContraseniaActual);
                if (user != null)
                {
                    user.PasswordHash = request.ContraseniaConfirmar;
                    _context.Update(user);
                    _context.SaveChanges();
                    return new ResponseGeneric<bool>(true);
                }
                return new ResponseGeneric<bool>(false);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<List<AutenticacionVerificentrosResponse>>> GetVerificentrosByUsername(string username)
        {
            try
            {
                var verificentros = new List<AutenticacionVerificentrosResponse>();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username && x.IdRols.Any(y => y.AccesoTotalVerificentros));

                if (user != null)
                {
                    verificentros = await _context.Verificentros.Where(x => x.Activo)
                    .OrderBy(x => x.Clave)
                    .Select(x => new AutenticacionVerificentrosResponse
                    {
                        Id = x.Id,
                        Nombre = x.Nombre
                    }).ToListAsync();
                }
                else
                {
                    verificentros = await _context.Verificentros
                    .Where(x => (x.UserPuestoVerificentros.Any(y => y.IdUserNavigation.UserName == username) || x.Users.Any(x => x.UserName == username)) && x.Activo)
                    .OrderBy(x => x.Clave)
                    .Select(x => new AutenticacionVerificentrosResponse
                    {
                        Id = x.Id,
                        Nombre = x.Nombre
                    }).ToListAsync();
                }
                return new ResponseGeneric<List<AutenticacionVerificentrosResponse>>(verificentros);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<AutenticacionVerificentrosResponse>>(ex);
            }
        }

        public ResponseGeneric<vVerificentro> ValidarUsuarioVerificentro(AutenticacionResponse request)
        {
            try
            {
                //var verificentros = _context.Verificentros
                //    .Where(x => x.UserPuestoVerificentros.Any(y => y.IdUserNavigation.Id == request.IdUser) && x.Activo).ToList();
                var verificentro = _context.vVerificentros.FirstOrDefault(v => v.Id == request.IdVerificentro);
                return new ResponseGeneric<vVerificentro>() { Response = verificentro };
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
    public interface IAutenticacionNegocio
    {
        public Task<ResponseGeneric<AutenticacionResponse>> Consultar(AutenticacionRequest request);
        public Task<ResponseGeneric<bool>> Registro(RegistroRequest request);
        public Task<ResponseGeneric<bool>> CambiarContrasenia(CambioContraseniaRequest request);
        public Task<ResponseGeneric<List<AutenticacionVerificentrosResponse>>> GetVerificentrosByUsername(string username);
        ResponseGeneric<vVerificentro> ValidarUsuarioVerificentro(AutenticacionResponse request);
    }
}
