using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.Personal.Request;
using Smadot.Models.Entities.Personal.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Seguridad;
using Smadot.Utilities.Seguridad.Modelo;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Transactions;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
    public class PersonalNegocio : IPersonalNegocio
    {
        private readonly SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly string _jwtKey;
        public PersonalNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _jwtKey = configuration["JWT:ClaveSecreta"];
        }

        public async Task<ResponseGeneric<ResponseGrid<PersonalGridResponse>>> Consulta(PersonalListRequest request)
        {
            try
            {
                var response = new ResponseGrid<PersonalGridResponse>();
                var data = _context.vPersonalAutorizacions.AsQueryable();//GroupBy(o => o.Id).Select(g => g.OrderByDescending(o => o.IdPuestoVerificentro).FirstOrDefault()).ToList();
                                                                         // if (request.IdVerificentro != null)
                                                                         //     data = data.Where(x => x.IdVerificentro == request.IdVerificentro.Value);
                                                                         // else
                data = data.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro.Value);
                var ListData = new List<PersonalGridResponse>().AsQueryable();
                ListData = data.Select(x => new PersonalGridResponse
                {
                    Id = x.Id,
                    IdPuestoVerificentro = x.IdPuestoVerificentro,
                    Curp = x.Curp,
                    FechaIncorporacionPuesto = x.FechaIncorporacionPuesto,
                    FechaSeparacionPuesto = x.FechaSeparacionPuesto,
                    Genero = x.Genero,
                    Nombre = x.Nombre,
                    NombrePuesto = x.NombrePuesto,
                    NumeroTrabajador = x.NumeroTrabajador,
                    Rfc = x.Rfc,
                    TipoPuesto = x.TipoPuesto,
                    UrlFoto = x.UrlFoto,
                    IdCatEstatusPuesto = x.IdCatEstatusPuesto,
                    EstatusPuesto = x.EstatusPuesto
                }).AsQueryable();

                response.RecordsTotal = ListData.Count();
                if (!string.IsNullOrEmpty(request.Busqueda))
                    ListData = ListData.Where(x => x.Nombre.ToLower().Contains(request.Busqueda.ToLower())
                                        || x.NumeroTrabajador.ToString().Contains(request.Busqueda)
                                        || x.NombrePuesto.ToLower().ToString().Contains(request.Busqueda.ToLower())
                                        || x.Curp.ToString().ToLower().Contains(request.Busqueda.ToLower())
                                        || x.Rfc.ToString().ToLower().Contains(request.Busqueda.ToLower())
                                        || x.TipoPuesto.ToLower().ToString().Contains(request.Busqueda.ToLower())
                                        || x.FechaIncorporacionPuesto.ToString().Contains(request.Busqueda)
                                        || x.FechaSeparacionPuesto.ToString().Contains(request.Busqueda)
                                        || x.Id.ToString().Contains(request.Busqueda)
                                    );
                ListData = ListData.OrderBy(request.ColumnaOrdenamiento + " " + request.Ordenamiento);
                response.Data = ListData.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value).ToList();


                response.RecordsFiltered = ListData.Count();

                return new ResponseGeneric<ResponseGrid<PersonalGridResponse>>(response);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<ResponseGrid<PersonalGridResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Ingreso(PersonalRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var validarUser = await _context.vPersonalAutorizacions
                                                                            .Where(x => x.Curp.ToUpper() == request.Curp.ToUpper())
                                                                            .AnyAsync();
                    if (validarUser)
                        throw new ValidationException("Este usuario debe darse de baja o realizar un movimiento. El CURP se encuentra en uso.");

                    var valRfc = await _context.vPersonalAutorizacions.AnyAsync(x => x.Rfc.ToUpper() == request.Rfc.ToUpper());
                    if (valRfc)
                        throw new ValidationException("Este usuario debe darse de baja o realizar un movimiento. El RFC ingresado ya se encuentra registrado.");

                    // var valCurp = await _context.Users.AnyAsync(x => x.Curp.ToUpper() == request.Curp.ToUpper());
                    // if (valCurp)
                    //     throw new ValidationException("El CURP ingresado ya se encuentra registrado.");

                    var valNumEmpleado = await _context.UserPuestoVerificentros.AnyAsync(x => x.NumeroTrabajador.ToUpper() == request.NumeroTrabajador.ToUpper() && x.FechaSeparacion == null);
                    if (valNumEmpleado)
                        throw new ValidationException("Este usuario debe darse de baja o realizar un movimiento. El número de trabajador ingresado ya se encuentra registrado.");

                    var existeCorreoElectronico = await _context.Users.AnyAsync(user => user.Email.ToUpper() == request.CorreoUsuario.ToUpper());
                    if (existeCorreoElectronico)
                        throw new ValidationException("El correo eléctronico ingresado ya se encuentra registrado para otro usuario.");

                    User user = new()
                    {
                        Nombre = request.Nombre.ToUpper(),
                        Email = request.CorreoUsuario.ToLower(),
                        Rfc = request.Rfc.ToUpper(),
                        Curp = request.Curp.ToUpper(),
                        Genero = request.Genero,
                        FechaNacimiento = request.FechaNacimiento,
                        UrlFirma = string.Empty,
                        UrlSeguroSocial = string.Empty,
                        UrlFoto = string.Empty,
                        UrlIne = string.Empty,
                        UserName = request.CorreoUsuario,
                        PhoneNumber = request.TelefonoUsuario,
                        EmailConfirmed = true,
                        PasswordHash = GestioEncriptacion.Cifrar(new SeguridadModelo
                        {
                            Valor = request.CorreoUsuario.ToLower(),
                            LlaveCifrado = _jwtKey
                        })
                    };

                    await _context.Users.AddAsync(user);
                    var userGuardado = await _context.SaveChangesAsync() > 0;

                    if (user.Id == 0)
                        throw new ValidationException("No se pudo registrar el usuario.");

                    foreach (var file in request.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlIne = url; break;
                        }
                    }
                    foreach (var file in request.FilesFoto)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlFoto = url; break;
                        }
                    }
                    foreach (var file in request.FilesSeguroSocial)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlSeguroSocial = url; break;
                        }
                    }
                    foreach (var file in request.FilesFirma)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlFirma = url; break;
                        }
                    }
                    await _context.SaveChangesAsync();

                    var puesto = await _context.Puestos.FirstOrDefaultAsync(p => p.IdCatTipoPuesto == request.IdPuesto);

                    UserPuestoVerificentro userPuestoVerificentro = new()
                    {
                        IdVerificentro = _userResolver.GetUser().IdVerificentro.Value,
                        FechaRegistro = DateTime.Now,
                        IdUser = user.Id,
                        IdPuesto = puesto.Id,
                        FechaCapacitacionInicio = request.FechaCapacitacionInicio,
                        FechaCapacitacionFinal = request.FechaCapacitacionFinal,
                        FechaAcreditacionNorma = request.FechaAcreditacionNorma,
                        FechaIncorpacion = request.FechaIncorporacion,
                        FechaSeparacion = request.FechaSeparacion,
                        NumeroTrabajador = request.NumeroTrabajador.ToUpper(),
                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        UrlContrato = string.Empty,
                        IdCatEstatusPuesto = CatEstatusPuestoDic.PendienteValidacion
                    };

                    await _context.UserPuestoVerificentros.AddAsync(userPuestoVerificentro);
                    var userPuestoVerificentroGuardado = await _context.SaveChangesAsync() > 0;

                    if (userPuestoVerificentro.Id == 0)
                        throw new ValidationException("No se pudo registrar el usuario.");

                    //foreach (var file in request.FilesContrato)
                    //{
                    //    var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                    //    if (!string.IsNullOrEmpty(url))
                    //    {
                    //        userPuestoVerificentro.UrlContrato = url; break;
                    //    }
                    //}
                    //await _context.SaveChangesAsync();

                    GenerarAlerta(userPuestoVerificentro, CatEstatusPuestoDic.PendienteValidacion, string.Format(MovimientosDicts.DictMovimientoPuesto[PuestoEstatus.Pendiente], userPuestoVerificentro.IdUserNavigation.Nombre, "registro"));

                    var horarios = request.HorarioRequest.Select(x => new HorarioUserPuestoVerificentro
                    {
                        Dia = x.Dia,
                        HoraFin = x.HoraFinTS,
                        HoraInicio = x.HoraInicioTS,
                        IdUserPuestoVerificentro = userPuestoVerificentro.Id
                    }).ToList();

                    await _context.HorarioUserPuestoVerificentros.AddRangeAsync(horarios);
                    var horariosGuardados = await _context.SaveChangesAsync() is 7;

                    var result = userGuardado && userPuestoVerificentroGuardado && horariosGuardados;

                    transaction.Complete();
                    return new ResponseGeneric<bool>(true);
                }
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<bool>(false)
                {
                    mensaje = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex.Message);
            }
        }

        public async Task<ResponseGeneric<bool>> Editar(PersonalRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var validarUser = await _context.vPersonalAutorizacions
                                                                        .Where(x => x.Curp.ToUpper() == request.Curp.ToUpper() && x.Id != request.IdUsuario)
                                                                        .AnyAsync();
                    if (validarUser)
                        throw new ValidationException("Este usuario debe darse de baja o realizar un movimiento. El CURP se encuentra en uso.");

                    var valRfc = await _context.vPersonalAutorizacions.AnyAsync(x => x.Rfc.ToUpper() == request.Rfc.ToUpper() && x.Id != request.IdUsuario);
                    if (valRfc)
                        throw new ValidationException("Este usuario debe darse de baja o realizar un movimiento. El RFC ingresado ya se encuentra registrado.");

                    // var valCurp = await _context.Users.AnyAsync(x => x.Curp.ToUpper() == request.Curp.ToUpper());
                    // if (valCurp)
                    //     throw new ValidationException("El CURP ingresado ya se encuentra registrado.");

                    var valNumEmpleado = await _context.vPersonalAutorizacions.AnyAsync(x => x.NumeroTrabajador.ToUpper() == request.NumeroTrabajador.ToUpper() && x.Id != request.IdUsuario);
                    if (valNumEmpleado)
                        throw new ValidationException("Este usuario debe darse de baja o realizar un movimiento. El número de trabajador ingresado ya se encuentra registrado.");

                    var existeCorreoElectronico = await _context.Users.AnyAsync(user => user.Email.ToUpper() == request.CorreoUsuario.ToUpper() && user.Id != request.IdUsuario);
                    if (existeCorreoElectronico)
                        throw new ValidationException("El correo eléctronico ingresado ya se encuentra registrado para otro usuario.");

                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.IdUsuario) ?? throw new ValidationException("No se pudo actualizar la información. No se encontró el usuario");
                    user.Nombre = request.Nombre;
                    user.Email = request.CorreoUsuario;
                    user.Rfc = request.Rfc;
                    user.Curp = request.Curp;
                    user.Genero = request.Genero;
                    user.FechaNacimiento = request.FechaNacimiento;
                    user.PhoneNumber = request.TelefonoUsuario;
                    user.UserName = request.CorreoUsuario;

                    foreach (var file in request.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlIne = url; break;
                        }
                    }
                    foreach (var file in request.FilesFoto)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlFoto = url; break;
                        }
                    }
                    foreach (var file in request.FilesSeguroSocial)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlSeguroSocial = url; break;
                        }
                    }
                    foreach (var file in request.FilesFirma)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlFirma = url; break;
                        }
                    }

                    var userPuestoVerificentro = await _context.UserPuestoVerificentros.Where(x => x.IdUser == request.IdUsuario)
                                                                        .OrderBy(o => o.Id)
                                                                        .LastOrDefaultAsync();

                    if (userPuestoVerificentro == null)
                        throw new ValidationException("No se pudo actualizar la información.");

                    //userPuestoVerificentro.IdPuesto = request.IdPuesto;
                    userPuestoVerificentro.FechaCapacitacionInicio = request.FechaCapacitacionInicio;
                    userPuestoVerificentro.FechaCapacitacionFinal = request.FechaCapacitacionFinal;
                    userPuestoVerificentro.FechaAcreditacionNorma = request.FechaAcreditacionNorma;
                    userPuestoVerificentro.FechaIncorpacion = request.FechaIncorporacion;
                    //userPuestoVerificentro.NumeroTrabajador = request.NumeroTrabajador;
                    userPuestoVerificentro.IdCatEstatusPuesto = CatEstatusPuestoDic.PendienteValidacion;

                    //foreach (var file in request.FilesContrato)
                    //{
                    //    var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                    //    if (!string.IsNullOrEmpty(url))
                    //    {
                    //        userPuestoVerificentro.UrlContrato = url; break;
                    //    }
                    //}

                    if (request.HorarioRequest.Any())
                    {
                        foreach (var item in request.HorarioRequest)
                        {
                            var dia = await _context.HorarioUserPuestoVerificentros.FirstOrDefaultAsync(x => x.Id == item.Id && EF.Functions.Collate(x.Dia, "Latin1_General_CI_AI") == item.Dia);
                            dia.HoraInicio = item.HoraInicioTS;
                            dia.HoraFin = item.HoraFinTS;
                        }
                    }

                    int response = await _context.SaveChangesAsync();

                    transaction.Complete();
                    return new ResponseGeneric<bool>(true);
                }
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<bool>(false) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex.Message);
            }
        }
        //Método para validar si un usuario está registrado como activo en algún verificentro.
        public async Task<ResponseGeneric<bool>> ValidarUsuario(string curp) =>
            new ResponseGeneric<bool>(await _context.UserPuestoVerificentros.Include(x => x.IdUserNavigation)
                                                                            .Where(x => x.IdUserNavigation.Curp == curp.ToUpper() && x.FechaSeparacion == null)
                                                                            .AnyAsync());

        public async Task<ResponseGeneric<PersonalResponse>> GetById(long id, bool edit = false)
        {
            try
            {
                var user = new vPersonalAutorizacion();
                if (!edit)
                {
                    user = await _context.vPersonalAutorizacions.Where(x => x.Id == id).FirstOrDefaultAsync();
                }
                else
                {
                    user = await _context.vPersonalAutorizacions.Where(x => x.Id == id).FirstOrDefaultAsync();
                }
                if (user is null)
                    throw new ValidationException("No se pudo obtener el usuario.");

                var horarios = _context.HorarioUserPuestoVerificentros.Where(x => x.IdUserPuestoVerificentro == user.IdPuestoVerificentro);
                var horarios2 = _context.HorarioUserPuestoVerificentros.Where(x => x.IdUserPuestoVerificentro == user.IdPuestoVerificentro).ToList();
                if (horarios.Count() is not 7)
                    throw new ValidationException("No se pudo obtener el horario.");

                PersonalResponse response = new()
                {
                    IdUsuario = user.Id,
                    Curp = user.Curp,
                    FechaIncorporacion = user.FechaIncorporacionPuesto,
                    FechaSeparacion = user.FechaSeparacionPuesto,
                    Genero = user.Genero,
                    Nombre = user.Nombre,
                    NumeroTrabajador = user.NumeroTrabajador,
                    Rfc = user.Rfc,
                    FechaAcreditacionNorma = user.FechaAcreditacionNorma,
                    FechaCapacitacionFinal = user.FechaCapacitacionFinal,
                    FechaCapacitacionInicio = user.FechaCapacitacionInicio,
                    FechaNacimiento = user.FechaNacimiento,
                    CorreoUsuario = user.CorreoUsuario,
                    IdPuesto = user.IdPuesto,
                    NombrePuesto = user.NombrePuesto,
                    TelefonoUsuario = user.TelefonoUsuario,
                    IdCatEstatusPuesto = user.IdCatEstatusPuesto,
                    IdPuestoVerificentro = user.IdPuestoVerificentro
                };

                if (!edit)
                {
                    response.HorarioRequest = new List<HorarioUserRequest>()
                    {
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Lunes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes  ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00") , HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Martes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00") , HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Miercoles, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Jueves, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves  ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Viernes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Sabado, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Domingo, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") }
                    };
                }
                else
                {
                    response.HorarioRequest = new List<HorarioUserRequest>()
                    {
                        new HorarioUserRequest
                        {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes],
                            HoraInicioResponseTS =
                                horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"),
                            HoraFinResponseTS =
                                horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ],
                            HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00") ,
                            HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves  ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        }
                    };
                }
                return new ResponseGeneric<PersonalResponse>(response);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<PersonalResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Baja(BajaPersonalRequest request)
        {
            try
            {
                var user = await _context.UserPuestoVerificentros.Include(x => x.IdUserNavigation).Where(x => x.Id == request.IdUserPuestoVerificentro && x.IdUser == request.IdUser)
                                                                .FirstOrDefaultAsync() ?? throw new ValidationException("No se pudo obtener el usuario.");
                user.FechaSeparacion = request.FechaSeparacion;

                _context.UserPuestoVerificentros.Update(user);
                user.IdUserNavigation.LockoutEnabled = true;
                var usuarioActulizado = _context.SaveChanges() > 0;

                return new ResponseGeneric<bool>(true);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> Movimiento(PersonalRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var valRfc = await _context.vPersonalAutorizacions.AnyAsync(x => x.Rfc.ToUpper() == request.Rfc.ToUpper() && x.Id != request.IdUsuario);
                    if (valRfc)
                        throw new ValidationException("El RFC ingresado ya se encuentra registrado.");

                    var valCurp = await _context.vPersonalAutorizacions.AnyAsync(x => x.Curp.ToUpper() == request.Curp.ToUpper() && x.Id != request.IdUsuario);
                    if (valCurp)
                        throw new ValidationException("El CURP ingresado ya se encuentra registrado.");

                    var valNumEmpleado = await _context.vPersonalAutorizacions.AnyAsync(x => x.NumeroTrabajador.ToUpper() == request.NumeroTrabajador.ToUpper() && x.Id != request.IdUsuario);
                    if (valNumEmpleado)
                        throw new ValidationException("El número de trabajador ingresado ya se encuentra registrado.");

                    var existeCorreoElectronico = await _context.vPersonalAutorizacions.AnyAsync(user => user.CorreoUsuario.ToUpper() == request.CorreoUsuario.ToUpper() && user.Id != request.IdUsuario);
                    if (existeCorreoElectronico)
                        throw new ValidationException("El correo eléctronico ingresado ya se encuentra registrado.");

                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.IdUsuario);

                    if (user == null)
                        throw new ValidationException("No se pudo actualizar la información.");

                    user.Nombre = request.Nombre;
                    user.Email = request.CorreoUsuario;
                    user.Rfc = request.Rfc;
                    user.Curp = request.Curp;
                    user.Genero = request.Genero;
                    user.FechaNacimiento = request.FechaNacimiento;
                    user.PhoneNumber = request.TelefonoUsuario;

                    var userGuardado = await _context.SaveChangesAsync() > 0;

                    foreach (var file in request.Files)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlIne = url; break;
                        }
                    }
                    foreach (var file in request.FilesFoto)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlFoto = url; break;
                        }
                    }
                    foreach (var file in request.FilesSeguroSocial)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlSeguroSocial = url; break;
                        }
                    }
                    foreach (var file in request.FilesFirma)
                    {
                        var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                        if (!string.IsNullOrEmpty(url))
                        {
                            user.UrlFirma = url; break;
                        }
                    }
                    await _context.SaveChangesAsync();


                    var previus = await _context.UserPuestoVerificentros.Where(x => x.IdUser == request.IdUsuario)
                                                                        .OrderBy(o => o.Id)
                                                                        .LastOrDefaultAsync();

                    previus.FechaSeparacion = request.FechaSeparacion;

                    var puesto = await _context.Puestos.FirstOrDefaultAsync(p => p.IdCatTipoPuesto == request.IdPuesto);
                    UserPuestoVerificentro userPuestoVerificentro = new()
                    {
                        IdVerificentro = _userResolver.GetUser().IdVerificentro ?? 1,
                        FechaRegistro = DateTime.Now,
                        IdUser = user.Id,
                        IdPuesto = puesto.Id,
                        FechaCapacitacionInicio = request.FechaCapacitacionInicio,
                        FechaCapacitacionFinal = request.FechaCapacitacionFinal,
                        FechaAcreditacionNorma = request.FechaAcreditacionNorma,
                        FechaIncorpacion = request.FechaIncorporacion,
                        FechaSeparacion = null,
                        NumeroTrabajador = request.NumeroTrabajador.ToUpper(),
                        IdUserRegistro = _userResolver.GetUser().IdUser,
                        UrlContrato = string.Empty,
                        IdCatEstatusPuesto = CatEstatusPuestoDic.PendienteValidacion
                    };

                    userPuestoVerificentro.IdUserRegistro = userPuestoVerificentro.IdUserRegistro == 0 ? 1 : userPuestoVerificentro.IdUserRegistro;

                    await _context.UserPuestoVerificentros.AddAsync(userPuestoVerificentro);
                    var userPuestoVerificentroGuardado = await _context.SaveChangesAsync() > 0;

                    if (userPuestoVerificentro.Id == 0)
                        throw new ValidationException("No se pudo registrar el usuario.");

                    //foreach (var file in request.FilesContrato)
                    //{
                    //    var url = await _blobStorage.UploadFileAsync(new byte[0], "Personal/" + user.Id + "/" + file.Nombre, file.Base64);
                    //    if (!string.IsNullOrEmpty(url))
                    //    {
                    //        userPuestoVerificentro.UrlContrato = url; break;
                    //    }
                    //}
                    //await _context.SaveChangesAsync();

                    var horarios = request.HorarioRequest.Select(x => new HorarioUserPuestoVerificentro
                    {
                        Dia = x.Dia,
                        HoraFin = x.HoraFinTS,
                        HoraInicio = x.HoraInicioTS,
                        IdUserPuestoVerificentro = userPuestoVerificentro.Id
                    }).ToList();

                    await _context.HorarioUserPuestoVerificentros.AddRangeAsync(horarios);

                    GenerarAlerta(userPuestoVerificentro, userPuestoVerificentro.IdCatEstatusPuesto, string.Format(MovimientosDicts.DictMovimientoPuesto[PuestoEstatus.Pendiente], userPuestoVerificentro.IdUserNavigation.Nombre, "actualizó"));
                    var horariosGuardados = await _context.SaveChangesAsync() > 0;

                    var result = userGuardado && userPuestoVerificentroGuardado && horariosGuardados;
                    transaction.Complete();
                    return new ResponseGeneric<bool>(true);
                }
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<bool>(false) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex.Message);
            }
        }

        public async Task<ResponseGeneric<List<PersonalResponse>>> GetPuestos(long id)
        {
            try
            {
                var catalogo = _context.vPersonals.Where(x => x.Id == id).AsQueryable();

                var result = await catalogo.Select(user => new PersonalResponse
                {
                    IdUsuario = user.Id,
                    Curp = user.Curp,
                    FechaIncorporacion = user.FechaIncorporacionPuesto,
                    FechaSeparacion = user.FechaSeparacionPuesto,
                    Genero = user.Genero,
                    Nombre = user.Nombre,
                    NumeroTrabajador = user.NumeroTrabajador,
                    Rfc = user.Rfc,
                    FechaAcreditacionNorma = user.FechaAcreditacionNorma,
                    FechaCapacitacionFinal = user.FechaCapacitacionFinal,
                    FechaCapacitacionInicio = user.FechaCapacitacionInicio,
                    FechaNacimiento = user.FechaNacimiento,
                    CorreoUsuario = user.CorreoUsuario,
                    IdPuesto = user.IdPuesto,
                    NombrePuesto = user.NombrePuesto,
                    IdPuestoVerificentro = user.IdPuestoVerificentro
                }).ToListAsync();

                return new ResponseGeneric<List<PersonalResponse>>(result);

            }
            catch (Exception ex)
            {
                return new ResponseGeneric<List<PersonalResponse>>(ex);
            }
        }

        public async Task<ResponseGeneric<PersonalResponse>> GetByIdPuestoVerificentro(long id, bool edit = false)
        {
            try
            {
                var user = new vPersonalAutorizacion();
                if (!edit)
                {
                    user = await _context.vPersonalAutorizacions.Where(x => x.IdPuestoVerificentro == id).FirstOrDefaultAsync();
                }
                else
                {
                    user = await _context.vPersonalAutorizacions.Where(x => x.IdPuestoVerificentro == id).FirstOrDefaultAsync();
                }
                if (user is null)
                    throw new ValidationException("No se pudo obtener el usuario.");

                var horarios = _context.HorarioUserPuestoVerificentros.Where(x => x.IdUserPuestoVerificentro == user.IdPuestoVerificentro);
                var horarios2 = _context.HorarioUserPuestoVerificentros.Where(x => x.IdUserPuestoVerificentro == user.IdPuestoVerificentro).ToList();
                if (horarios.Count() is not 7)
                    throw new ValidationException("No se pudo obtener el horario.");

                PersonalResponse response = new()
                {
                    IdUsuario = user.Id,
                    Curp = user.Curp,
                    FechaIncorporacion = user.FechaIncorporacionPuesto,
                    FechaSeparacion = user.FechaSeparacionPuesto,
                    Genero = user.Genero,
                    Nombre = user.Nombre,
                    NumeroTrabajador = user.NumeroTrabajador,
                    Rfc = user.Rfc,
                    FechaAcreditacionNorma = user.FechaAcreditacionNorma,
                    FechaCapacitacionFinal = user.FechaCapacitacionFinal,
                    FechaCapacitacionInicio = user.FechaCapacitacionInicio,
                    FechaNacimiento = user.FechaNacimiento,
                    CorreoUsuario = user.CorreoUsuario,
                    IdPuesto = user.IdPuesto,
                    NombrePuesto = user.NombrePuesto,
                    UrlIne = user.UrlIne,
                    UrlFoto = user.UrlFoto,
                    UrlSeguroSocial = user.UrlSeguroSocial,
                    UrlFirma = user.UrlFirma,
                    UrlContrato = user.UrlContrato,
                    TelefonoUsuario = user.TelefonoUsuario,
                    IdCatEstatusPuesto = user.IdCatEstatusPuesto,
                    IdPuestoVerificentro = user.IdPuestoVerificentro
                };

                if (!edit)
                {
                    response.HorarioRequest = new List<HorarioUserRequest>()
                    {
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Lunes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes  ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00") , HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Martes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00") , HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Miercoles, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Jueves, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves  ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Viernes, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Sabado, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") },
                        new HorarioUserRequest { Id = CatHorarioUserPuestoVerificentro.Domingo, Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00") }
                    };
                }
                else
                {
                    response.HorarioRequest = new List<HorarioUserRequest>()
                    {
                        new HorarioUserRequest
                        {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes],
                            HoraInicioResponseTS =
                                horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"),
                            //HoraInicio = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString(),
                            HoraFinResponseTS =
                                horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            //HoraFin = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString()
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Lunes]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ],
                            HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00") ,
                            HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Martes]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Miercoles]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves  ] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Jueves]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Viernes]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Sabado]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        },
                        new HorarioUserRequest {
                            Id = (int)horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.Id,
                            Dia = CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ], HoraInicioResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo ] ).FirstOrDefault()?.HoraInicio ?? TimeSpan.Parse("00:00"), HoraFinResponseTS = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo] ).FirstOrDefault()?.HoraFin ?? TimeSpan.Parse("00:00"),
                            //HoraInicio = horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString(),
                            HoraInicio = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraInicio.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraFin)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraInicio.ToString() ?? TimeSpan.Parse("00:00").ToString()),
                            HoraFin = ((bool)(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraFin.Equals(horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraInicio)) ? "" : horarios.Where(x => x.Dia == CatHorarioUserPuestoVerificentro.Nombres[CatHorarioUserPuestoVerificentro.Domingo]).FirstOrDefault()?.HoraFin.ToString() ?? TimeSpan.Parse("00:00").ToString())
                        }
                    };
                }
                return new ResponseGeneric<PersonalResponse>(response);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<PersonalResponse>(ex);
            }
        }

        public async Task<ResponseGeneric<bool>> ModificarEstatusPuesto(EstatusPuestoPersonalRequest request)
        {
            try
            {
                var userPuestoVerificentro = await _context.UserPuestoVerificentros.Include(x => x.IdUserNavigation)
                    .FirstOrDefaultAsync(x => x.Id == request.IdUserPuestoVerificentro);
                if (userPuestoVerificentro == null)
                    throw new ValidationException("No se pudo obtener el usuario.");

                userPuestoVerificentro.IdCatEstatusPuesto = request.IdCatEstatusPuesto;

                var reponse = _context.SaveChanges() > 0;

                if (request.IdCatEstatusPuesto == CatEstatusPuestoDic.SolicitaModificar)
                {
                    GenerarAlerta(userPuestoVerificentro, userPuestoVerificentro.IdCatEstatusPuesto,
                        string.Format(MovimientosDicts.DictMovimientoPuesto[request.IdCatEstatusPuesto],
                                    userPuestoVerificentro.IdUserNavigation.Nombre));

                }
                else
                {
                    if (request.IdCatEstatusPuesto == CatEstatusPuestoDic.RechazaModificar || request.IdCatEstatusPuesto == CatEstatusPuestoDic.PermiteModificar)
                    {
                        var alerta = await _context.Alerta.Where(d => d.TableId == userPuestoVerificentro.Id)
                                            .Where(d => d.TableName == DictAlertas.UserPuestoVerificentro)
                                            .Where(d => d.IdEstatusInicial == CatEstatusPuestoDic.SolicitaModificar)
                                            .FirstOrDefaultAsync();

                        UpdateAlertaPersonal(userPuestoVerificentro, alerta,
                                            request.IdCatEstatusPuesto,
                                            string.Format(
                                                MovimientosDicts.DictMovimientoPuesto[request.IdCatEstatusPuesto],
                                                userPuestoVerificentro.IdUserNavigation.Nombre));
                        //await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var alerta = await _context.Alerta.Where(d => d.TableId == userPuestoVerificentro.Id)
                                            .Where(d => d.TableName == DictAlertas.UserPuestoVerificentro)
                                            .FirstOrDefaultAsync();

                        UpdateAlertaPersonal(userPuestoVerificentro, alerta,
                                            request.IdCatEstatusPuesto,
                                            string.Format(
                                                MovimientosDicts.DictMovimientoPuesto[request.IdCatEstatusPuesto],
                                                userPuestoVerificentro.IdUserNavigation.Nombre));
                        //await _context.SaveChangesAsync();
                    }
                }
                await _context.SaveChangesAsync();

                return new ResponseGeneric<bool>(reponse);
            }
            catch (ValidationException ex)
            {
                return new ResponseGeneric<bool>(false) { mensaje = ex.Message };
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

        private async void GenerarAlerta(UserPuestoVerificentro userPuestoVerificentro, int idEstatusInicial, string? movimientoInicial)
        {
            Alertum alerta = new()
            {
                TableName = DictAlertas.UserPuestoVerificentro,
                TableId = userPuestoVerificentro.Id,
                IdVerificentro = userPuestoVerificentro.IdVerificentro,
                Data = JsonConvert.SerializeObject(userPuestoVerificentro.IdUserNavigation, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    NullValueHandling = NullValueHandling.Ignore
                }),
                IdUser = _userResolver.GetUser().IdUser,
                //MovimientoInicial = string.Format(MovimientosDicts.DictMovimientoPuesto[PuestoEstatus.Pendiente], userPuestoVerificentro.IdUserNavigation.Nombre),
                //IdEstatusInicial = userPuestoVerificentro.IdCatEstatusPuesto,
                MovimientoInicial = movimientoInicial,
                IdEstatusInicial = idEstatusInicial,
                Fecha = DateTime.Now,
                Leido = false,
                Procesada=false
            };

            await _context.Alerta.AddAsync(alerta);
            //await _context.SaveChangesAsync();
        }

        private async void UpdateAlertaPersonal(UserPuestoVerificentro userPuestoVerificentro, Alertum? alerta, int? idEstatusFinal, string? movimientoFinal)
        {
            if (alerta != null)
            {
                alerta.IdEstatusFinal = idEstatusFinal;
                alerta.MovimientoFinal = movimientoFinal;
                alerta.Data = JsonConvert.SerializeObject(userPuestoVerificentro, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    NullValueHandling = NullValueHandling.Ignore
                });
            }

            //await _context.SaveChangesAsync();
        }
    }

    public interface IPersonalNegocio
    {
        public Task<ResponseGeneric<ResponseGrid<PersonalGridResponse>>> Consulta(PersonalListRequest request);

        public Task<ResponseGeneric<bool>> Ingreso(PersonalRequest request);

        public Task<ResponseGeneric<bool>> Editar(PersonalRequest request);

        public Task<ResponseGeneric<bool>> ValidarUsuario(string curp);

        public Task<ResponseGeneric<PersonalResponse>> GetById(long id, bool edit = false);

        public Task<ResponseGeneric<bool>> Baja(BajaPersonalRequest request);

        public Task<ResponseGeneric<bool>> Movimiento(PersonalRequest request);

        public Task<ResponseGeneric<List<PersonalResponse>>> GetPuestos(long id);

        public Task<ResponseGeneric<PersonalResponse>> GetByIdPuestoVerificentro(long id, bool edit = false);

        public Task<ResponseGeneric<bool>> ModificarEstatusPuesto(EstatusPuestoPersonalRequest request);

    }
}