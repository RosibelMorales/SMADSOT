using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Roles.Request;
using Smadot.Models.Entities.Roles.Response;
using Smadot.Models.Entities.Usuarios.Request;
using Smadot.Models.Entities.Usuarios.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.SeguimientoCVV.Model.Negocio
{
    public class UsuariosNegocio : IUsuariosNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly string _secretKey;
        public UsuariosNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _secretKey = configuration["JWT:ClaveSecreta"];
        }

        public async Task<ResponseGeneric<List<UsuariosResponse>>> Consulta(UsuariosListRequest request)
        {
            try
            {
                var seguimiento = _context.vUsuarios.Where(x => !x.CorreoElectronico.Equals("admin@smadot.com"));
                var user = _userResolver.GetUser();
                var rol = user.RoleNames.FirstOrDefault();
                var acceso = _context.Rols.ToList().Where(o => o.Nombre.Equals(rol, StringComparison.Ordinal)).FirstOrDefault();
                seguimiento = seguimiento.Where(x => x.IdVerificentro == user.IdVerificentro).AsQueryable();

                if (!string.IsNullOrEmpty(request.Busqueda))
                {

                    seguimiento = seguimiento.Where(x => x.CorreoElectronico.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreUsuario.ToLower().Contains(request.Busqueda.ToLower()) || x.NombreRol.ToLower().Contains(request.Busqueda.ToLower()));
                }

                var tot = seguimiento.Count();

                if (!string.IsNullOrEmpty(request.ColumnaOrdenamiento) && !string.IsNullOrEmpty(request.Ordenamiento))
                {
                    seguimiento = seguimiento.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                }

                if (request.Pagina > 0 && request.Registros > 0)
                {
                    seguimiento = seguimiento.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
                }


                DateTime now = DateTime.Now;
                var result = await seguimiento.Select(x => new UsuariosResponse
                {
                    Id = (int?)x.IdUsuario,
                    CorreoElectronico = x.CorreoElectronico,
                    NombreRol = x.NombreRol,
                    NombreUsuario = x.NombreUsuario,
                    LockoutEnabled = x.LockoutEnabled,
                    Total = tot

                }).ToListAsync();

                return new ResponseGeneric<List<UsuariosResponse>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<UsuariosResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<List<UsuariosResponse>>> GetById(long Id)
        {
            try
            {
                var result = new List<UsuariosResponse>();
                var result2 = new List<UsuariosResponse>();

                if (Id > 0)
                {

                    var usuarios = _context.vUsuarios.Where(x => x.IdUsuario == Id).AsQueryable();
                    var roles = _context.Rols.AsQueryable();

                    result = await roles.Select(x => new UsuariosResponse
                    {
                        IdRolSelect = (int?)x.Id,
                        NombreRolSelect = x.Nombre,
                    }).ToListAsync();

                    string rls = JsonConvert.SerializeObject(usuarios);
                    result2 = JsonConvert.DeserializeObject<List<UsuariosResponse>>(rls);

                    var i = 0;

                    result[0].IdRol = result2[i].IdRol;
                    result[0].NombreRol = result2[i].NombreRol;
                    result[0].IdUsuario = result2[i].IdUsuario;
                    result[0].CorreoElectronico = result2[i].CorreoElectronico;
                    result[0].NombreUsuario = result2[i].NombreUsuario;

                    return new ResponseGeneric<List<UsuariosResponse>>(result);
                }
                else
                {
                    var roles = _context.Rols.AsQueryable();

                    result = await roles.Select(x => new UsuariosResponse
                    {
                        IdRolSelect = (int?)x.Id,
                        NombreRolSelect = x.Nombre,
                    }).ToListAsync();

                    return new ResponseGeneric<List<UsuariosResponse>>(result);
                }
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<UsuariosResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<long>> Registro(List<UsuariosResponse> request)
        {
            try
            {
                var usuario = new User();
                var user = new User();
                var usuarioRol = new UsuarioRol();

                if (request[0].IdUsuario > 0) //ACTUALIZAR
                {
                    usuario = _context.Users.FirstOrDefault(x => x.Id == request[0].IdUsuario);

                    usuario.Nombre = request[0].NombreUsuario;
                    _context.Update(usuario);
                    var result = await _context.SaveChangesAsync() > 0;

                    user = _context.Users.Include(x => x.IdRols).FirstOrDefault(x => x.Id == request[0].IdUsuario) ?? new User();
                    user.IdRols.Clear();
                    result = await _context.SaveChangesAsync() > 0;

                    var rol = _context.Rols.FirstOrDefault(x => x.Id == request[0].IdRol);
                    usuario.IdRols.Add(rol);
                    result = await _context.SaveChangesAsync() > 0;

                    return result ? new ResponseGeneric<long>(usuario.Id) : new ResponseGeneric<long>();
                }
                else //AGREGAR
                {

                    var existe = await _context.Users.AnyAsync(x => x.Email != null && x.Email.ToLower().Equals(request[0].CorreoElectronico.ToLower()));
                    if (existe)
                    {
                        throw new ValidationException("El correo ya se encuentra registrado.");
                    }
                    usuario = new User
                    {
                        Nombre = request[0].NombreUsuario,
                        Email = request[0].CorreoElectronico?.ToLower(),
                        EmailConfirmed = true,
                        UserName = request[0].CorreoElectronico,
                        PasswordHash = request[0].Contrasenia,
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        LockoutEnd = null,
                        LockoutEnabled = false,
                        AccessFailedCount = 0,
                        FechaNacimiento = DateTime.Now,
                        Genero = " ",
                        Rfc = " ",
                        Curp = " ",
                        UrlIne = " ",
                        UrlFoto = " ",
                        UrlSeguroSocial = " ",
                        UrlFirma = " ",
                        IdVerificentro = _userResolver.GetUser().IdVerificentro
                    };
                    _context.Users.Add(usuario);
                    var result = await _context.SaveChangesAsync() > 0;

                    var rol = _context.Rols.FirstOrDefault(x => x.Id == request[0].IdRol);
                    usuario.IdRols.Add(rol);
                    _context.SaveChangesAsync();

                    return result ? new ResponseGeneric<long>(usuario.Id) : new ResponseGeneric<long>();


                }
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<long>(ex)
                {
                    mensaje = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "No se pudo crear al nuevo usuario en el sistema." };
            }
        }

        public async Task<ResponseGeneric<long>> ActivarDesactivar(UsuarioActivarDesRequest request)
        {
            try
            {
                var usuario = _context.Users.FirstOrDefault(x => x.Id == request.Id);
                if (usuario == null)
                    throw new Exception("No sé encontró un usuario.");
                usuario.LockoutEnabled = !usuario.LockoutEnabled;
                var result = await _context.SaveChangesAsync() > 0;
                return result ? new ResponseGeneric<long>(usuario.Id) : new ResponseGeneric<long>();
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex);
            }
        }
        public async Task<ResponseGeneric<long>> ResetPwd(UpdatePasswordRequest request)
        {
            try
            {
                var usuario = _context.Users.FirstOrDefault(x => x.Id == request.Id);
                if (usuario == null)
                    throw new ValidationException("No sé encontró un usuario.");
                usuario.PasswordHash = GestioEncriptacion.Cifrar(new Utilities.Seguridad.Modelo.SeguridadModelo
                {
                    Valor = (usuario.Email ?? "").ToLower(),
                    LlaveCifrado = _secretKey
                });
                var result = await _context.SaveChangesAsync() > 0;

                return result ? new ResponseGeneric<long>(usuario.Id) : new ResponseGeneric<long>();
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<long>(ex) { mensaje = "Ocurrió un error al actualizar la información del usuario." };

            }
        }
    }
}
public interface IUsuariosNegocio
{
    public Task<ResponseGeneric<List<UsuariosResponse>>> Consulta(UsuariosListRequest request);
    public Task<ResponseGeneric<List<UsuariosResponse>>> GetById(long Id);
    Task<ResponseGeneric<long>> Registro(List<UsuariosResponse> request);
    Task<ResponseGeneric<long>> ResetPwd(UpdatePasswordRequest request);
    Task<ResponseGeneric<long>> ActivarDesactivar(UsuarioActivarDesRequest request);
}