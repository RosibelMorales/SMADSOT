using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Smadsot.FotoMulta.Model.DataBase;
using Smadsot.FotoMulta.Model.Modelos;
using Smadsot.FotoMulta.Model.Entities.Request;
using Smadsot.FotoMulta.Model.Entities.Response;
using Smadsot.FotoMulta.Negocio.Seguridad;
using Microsoft.Extensions.Configuration;

namespace Smadsot.FotoMulta.Negocio.Operaciones
{
    public class AutenticacionNegocio : IAutenticacionNegocio
    {
        private readonly SmadsotDbContext _context;
        private IConfiguration _config;
        public AutenticacionNegocio(SmadsotDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<ResponseGeneric<AutenticacionResponse>> Autenticar(AutenticacionRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Where(x => (x.UserName.Equals(request.Usuario) || x.Email.Equals(request.Usuario)) && x.PasswordHash == request.Contrasenia && !x.LockoutEnabled && x.EmailConfirmed)
                    .Select(x => new AutenticacionResponse
                    {
                        IdUser = x.Id,
                        UserName = x.UserName,
                        Nombre = x.Nombre,
                    }).FirstOrDefaultAsync() ?? throw new ValidationException("No fue posible iniciar sesión. Usuario o contraseña son incorrectos.");
                //Obtenemos los Roles, Permisos e información adicional que se utilizará a lo largo de la aplicación
                var roles = _context.Rols.Include(x => x.IdPermisos).Where(x => x.IdUsers.Any(u => u.Id == user.IdUser)).ToList();
                // var fechaActual = DateTime.Now;
                // var semestre = fechaActual.Month <= 6 ? 1 : 2;
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

                // var verificentro = _context.vVerificentros.FirstOrDefault(v => v.Id == request.IdVerificentro);
                // user.IdVerificentro = request.IdVerificentro;
                // user.NombreVerificentro = verificentro.NombreCorto;
                // user.ClaveVerificentro = verificentro.Clave;

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

        public async Task<ResponseGeneric<bool>> CambiarContrasenia(CambioContraseniaRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email) && x.PasswordHash == request.ContraseniaActual);
                if (user != null && request.ContraseniaNueva.Equals(request.ContraseniaConfirmar))
                {

                    user.PasswordHash = GestioEncriptacion.Cifrar(new Seguridad.Modelo.SeguridadModelo
                    {
                        Valor = request.ContraseniaConfirmar,
                        LlaveCifrado = _config["JWT:ClaveSecreta"]
                    });
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


    }
    public interface IAutenticacionNegocio
    {
        Task<ResponseGeneric<bool>> CambiarContrasenia(CambioContraseniaRequest request);
        Task<ResponseGeneric<AutenticacionResponse>> Autenticar(AutenticacionRequest request);
    }
}
