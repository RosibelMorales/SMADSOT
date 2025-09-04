using Smadot.Models.DataBase;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Smadot.Models.Entities.FoliosCancelados.Response.FoliosCanceladosResponseData;
using System.Linq.Dynamic.Core;
using static Smadot.Models.Entities.ConsultaCircular.Response.ConsultaCircularResponseData;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using static Smadot.Models.Entities.ConsultaCircular.Request.ConsultaCircularRequestData;
using Smadot.Utilities.EnvioCorreoElectronico;
using Microsoft.Extensions.Configuration;
using System.IO;
using Smadot.Utilities.Seguridad.Modelo;
using Smadot.Utilities.Seguridad;
using Smadot.Utilities.GestionTokens;

namespace Smadot.SolicitudFormaValorada.Model.Negocio
{
	public class ConsultaCircularNegocio : ICircularNegocio
	{
		private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly IConfiguration _configuration;

		public ConsultaCircularNegocio(SmadotDbContext context, IConfiguration configuration, IUserResolver userResolver)
		{
			_context = context;
			_configuration = configuration;
			_userResolver = userResolver;
		}

		public async Task<ResponseGeneric<List<ConsultaCircularResponse>>> Consulta(RequestList request)
		{
			try
			{
				///Se agrego ordenamiento debido a que el grid mostraba los registros desordenados
				///REVISAR OPCIONES
				var catalogo = _context.vConsultaCirculares.AsQueryable();
				//var catalogo = _context.vFoliosCancelados.AsQueryable();

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
					catalogo = catalogo.Where(x => x.NumeroCircular.Contains(request.Busqueda.ToLower()) || x.Leido.ToString().Contains(request.Busqueda.ToLower()) || x.NoLeido.ToString().Contains(request.Busqueda.ToLower()));
				}
				if (request.Pagina > 0 && request.Registros > 0)
				{
					catalogo = catalogo.Skip((request.Pagina.Value - 1) * request.Registros.Value).Take(request.Registros.Value);
				}

				var result = await catalogo.Select(x => new ConsultaCircularResponse
				{
					Id = x.IdCircular,
					NumeroCircular = x.NumeroCircular,
					Fecha = x.Fecha,
					Leidos = x.Leido,
					NoLeidos = x.NoLeido,
					Total = tot,
				}).ToListAsync();

				return new ResponseGeneric<List<ConsultaCircularResponse>>(result);
			}
			catch (Exception ex)
			{
				return new ResponseGeneric<List<ConsultaCircularResponse>>(ex);
			}
		}

		public async Task<ResponseGeneric<ConsultaCircularResponse>> GetById(long Id)
		{
			try
			{
				var isLeido = _context.CircularVerificentros.FirstOrDefault(x => x.IdCircular == Id && x.IdVerificentro == _userResolver.GetUser().IdVerificentro);
				if(isLeido != null)
				{
					if (!isLeido.Leido)
					{
						isLeido.Leido = true;
						_context.SaveChanges();
					}
				}

				var consulta = _context.vConsultaCirculares.FirstOrDefault(x => x.IdCircular == Id);

				var result = new ConsultaCircularResponse
				{
					Id = consulta.IdCircular,
					NumeroCircular = consulta.NumeroCircular,
					Fecha = consulta.Fecha,
					Leidos = consulta.Leido,
					NoLeidos = consulta.NoLeido,
					Mensaje = consulta.Mensaje
				};

				return new ResponseGeneric<ConsultaCircularResponse>(result);
			}
			catch (Exception ex)
			{
				return new ResponseGeneric<ConsultaCircularResponse>(ex);
			}
		}

		public async Task<ResponseGeneric<bool>> EnvioCircular(ConsultaCircularRequest request)
		{
			try
			{
				EmailMessage emailMessage = new EmailMessage();
				EnvioCorreoSMTP envioCorreo = new EnvioCorreoSMTP();
				string imageLogo = "";
				imageLogo = !string.IsNullOrEmpty(request.Logo) ? request.Logo : "";

				//var currentD = Directory.GetCurrentDirectory();
				//var DirectorP = Directory.GetParent(currentD);
				
				//if (DirectorP != null)
				//{
				//	var directories = Directory.GetDirectories(DirectorP.FullName);
				//	var directW = directories.FirstOrDefault(x => x.ToLower().Contains("web"));
				//	var path = Path.Combine(directW, "wwwroot", "assets", "media", "logos", "smadot_logo_simple2.png");
				//	if (File.Exists(path))
				//	{
				//		byte[] imageArray = System.IO.File.ReadAllBytes(path);
				//		imageLogo = Convert.ToBase64String(imageArray);
				//	}
				//}

				emailMessage.Subject = "Circular";

				//emailMessage.Body = bodyCorreo;
				var consecutivo = _context.Circulars.Count() + 1;
				var Circular = new Circular
				{
					//NumeroCircular = Guid.NewGuid().ToString(),
					NumeroCircular = consecutivo.ToString("00000000"),
					Fecha = DateTime.Now,
					Mensaje = request.Mensaje,
					IdUserRegistro = request.IdUserRegistro,
					FechaRegistro = DateTime.Now
				};

				_context.Circulars.Add(Circular);
				_context.SaveChanges();

				var verificentros = _context.Verificentros.Where(x => x.Activo).ToList();
				foreach (var v in verificentros)
				{
					var circVerificentro = new CircularVerificentro
					{
						IdCircular = Circular.Id,
						IdVerificentro = v.Id,
						Leido = false,
						AcuseLeido = null
					};
					//var cifV = SeguridadBase64.Encriptar(v.Id.ToString());
					//var cifC = SeguridadBase64.Encriptar(circVerificentro.IdCircular.ToString());
					//               //var btn = "<br><br><a style='background-color: #4CAF50;border: none;color: white;padding: 15px 32px;text-align: center;text-decoration: none;display: inline-block;font-size: 16px;margin: 4px 2px;cursor: pointer;border-radius: 15px; href=" + string.Format("{0}ConsultaCirculares/Confirmar/{1}", _configuration["SiteUrl"],v.Id) +"'>Leído</a>";
					//               var btn = "<br><br><a style='background-color: #4CAF50;border: none;color: white;padding: 15px 32px;text-align: center;text-decoration: none;display: inline-block;font-size: 16px;margin: 4px 2px;cursor: pointer;border-radius: 15px;' href=" + string.Format("{0}ConsultaCirculares/Confirmar?idV={1}&&idC={2}", _configuration["SiteUrl"], cifV, cifC) + ">Leído</a>";
					//               String bodyCorreo = envioCorreo.BodyEmail(emailMessage.Subject, request.Mensaje, imageLogo, btn);

					//               var destinatario = new List<string>();
					//destinatario.Add(v.Correo);

					//var sedm = envioCorreo.Send(
					//	destinatario,
					//	emailMessage.Subject,
					//	bodyCorreo,
					//	_configuration["Correo:email"], _configuration["Correo:usuario"],
					//	_configuration["Correo:contrasena"], _configuration["Correo:smtp"],
					//	null, null, _configuration["Correo:puerto"], true
					//	);


					//if (sedm == "Correcto")
					if (true)
					{
						_context.CircularVerificentros.Add(circVerificentro);
						_context.SaveChanges();
					}

				}
				
				return new ResponseGeneric<bool>(true);
			}
			catch (Exception ex)
			{
				return new ResponseGeneric<bool>(ex);
			}
		}

        public async Task<ResponseGeneric<bool>> ConfirmarCircular(ConfirmarCircularRequest request)
        {
            try
            {
				var circularVerificentro = _context.CircularVerificentros.FirstOrDefault(x => x.IdVerificentro == request.IdV && x.IdCircular == request.IdC);
				if (circularVerificentro != null)
				{
					circularVerificentro.Leido = true;
					_context.SaveChanges();
				}

                return new ResponseGeneric<bool>(true);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<bool>(ex);
            }
        }

    }

	public interface ICircularNegocio
	{
		public Task<ResponseGeneric<List<ConsultaCircularResponse>>> Consulta(RequestList request);
		public Task<ResponseGeneric<ConsultaCircularResponse>> GetById(long Id);
		public Task<ResponseGeneric<bool>> EnvioCircular(ConsultaCircularRequest request);
		public Task<ResponseGeneric<bool>> ConfirmarCircular(ConfirmarCircularRequest request);

    }
}
