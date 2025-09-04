using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Entities.Capacitacion.Request;
using Smadot.Models.Entities.Capacitacion.Response;
using Smadot.Models.Entities.ReporteMensual.Request;
using Smadot.Models.Entities.ReporteMensual.Response;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.DirectorioCentrosVerificacion.Model.Negocio
{
	public class CapacitacionNegocio : ICapacitacionNegocio
	{

		private SmadotDbContext _context;
		private readonly IUserResolver _userResolver;
		private readonly BlobStorage _blobStorage;
		public CapacitacionNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
		{
			_context = context;
			_userResolver = userResolver;
			_blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
		}

		public async Task<ResponseGeneric<List<CapacitacionResponse>>> Consulta(CapacitacionListRequest request)
		{
			try
			{

				var seguimiento = _context.vCapacitacions.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro).AsQueryable();


				if (!string.IsNullOrEmpty(request.Busqueda))
				{

					seguimiento = seguimiento.Where(x => x.TemaCapacitacion.ToLower().Contains(request.Busqueda.ToLower()) || x.IdCapacitacion.ToString().Contains(request.Busqueda.ToLower()) || x.NombreCatEstatusCapacitacion.ToLower().Contains(request.Busqueda.ToLower()) || x.TotalAsistentes.ToString().Contains(request.Busqueda.ToLower()));
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
				var j = 0;
				DateTime now = DateTime.Now;
				var result = await seguimiento.Select(x => new CapacitacionResponse
				{
					IdCapacitacion = (int)x.IdCapacitacion,
					FechaCapacitacion = x.FechaCapacitacion,
					TemaCapacitacion = x.TemaCapacitacion,
					IdCatEstatusCapacitacion = x.IdCatEstatusCapacitacion,
					NombreCatEstatusCapacitacion = x.NombreCatEstatusCapacitacion,
					ActivoCatEstatusCapacitacion = x.ActivoCatEstatusCapacitacion,
					Total = tot,
					TotalAsistentes = x.TotalAsistentes,
				}).ToListAsync();

				return new ResponseGeneric<List<CapacitacionResponse>>(result);
			}
			catch (Exception ex)
			{
				return new ResponseGeneric<List<CapacitacionResponse>>(ex);
			}
		}

		public async Task<ResponseGeneric<List<CapacitacionResponse>>> GetById(long Id)
		{
			try
			{
				var result = new List<CapacitacionResponse>();

				//Consulta tabla CapacitacionEmpleado
				if (Id > 0)
				{

					var solicitudEmpleado = _context.vCapacitacionEmpleados.Where(x => x.IdCapacitacion == Id).AsQueryable();

					if (solicitudEmpleado != null)
					{
						string t = JsonConvert.SerializeObject(solicitudEmpleado);
						result = JsonConvert.DeserializeObject<List<CapacitacionResponse>>(t);

						var fecha = result[0].FechaCapacitacion.Value;


						var format = fecha.ToString("dd/MM/yyyy");
						result[0].FechaCapacitacionFormat = format;
						//result[0].FechaCapacitacion = formatoFecha;
					}

				}
				//Para llenar el select2
				else
				{
					var users = _context.vPersonalAutorizacions.Where(x => x.IdVerificentro == _userResolver.GetUser().IdVerificentro && x.FechaSeparacionPuesto == null);

					if (users != null)
					{
						string t = JsonConvert.SerializeObject(users);
						result = JsonConvert.DeserializeObject<List<CapacitacionResponse>>(t);

						if (result.Count == 0)
						{
							result.Add(new CapacitacionResponse
							{
								Id = 0,
								IdUserPuestoVerificentro = 0,
								Nombre = "Sin empleados registrados",
								NombreVerificentro = _userResolver.GetUser().NombreVerificentro,
							});
						}
						else
						{
							result[0].NombreVerificentro = _userResolver.GetUser().NombreVerificentro;
						}
					}
				}


				return new ResponseGeneric<List<CapacitacionResponse>>(result);
			}
			catch (Exception ex)
			{
				return new ResponseGeneric<List<CapacitacionResponse>>(ex);
			}
		}

		public async Task<ResponseGeneric<List<CapacitacionResponse>>> GetByIdEmpleado(long Id)
		{
			try
			{
				var result = new List<CapacitacionResponse>();
				if (Id > 0)
				{
					var solicitudEmpleado = _context.vCapacitacionEmpleados.Where(x => x.Id == Id).AsQueryable();

					if (solicitudEmpleado != null)
					{
						string t = JsonConvert.SerializeObject(solicitudEmpleado);
						result = JsonConvert.DeserializeObject<List<CapacitacionResponse>>(t);
					}
				}

				return new ResponseGeneric<List<CapacitacionResponse>>(result);
			}
			catch (Exception ex)
			{
				return new ResponseGeneric<List<CapacitacionResponse>>(ex);
			}
		}

		public async Task<ResponseGeneric<long>> Registro(List<CapacitacionResponse> request)
		{
			try
			{
				var capacitacionQuery = new Capacitacion();
				var asistencia = new CapacitacionEmpleado();

				//Validación de documentos
				if (request[0].Id > 0 && request[0].IdEmpleado > 0)
				{
					asistencia = _context.CapacitacionEmpleados.Where(x => x.Id == request[0].Id).FirstOrDefault();

					//Actualizar fotografia
					if (request[0].Total == 2)
					{
						var urlFotografia = asistencia.UrlFotografia;

						var res = await _blobStorage.Remove(urlFotografia);
						asistencia.UrlFotografia = "";

						foreach (var file in request[0].Files)
						{

							var url = await _blobStorage.UploadFileAsync(new byte[0], "CapacitacionEmpleadoFotografia/" + request[0].Id + "/" + file.Nombre, file.Base64);
							if (!string.IsNullOrEmpty(url))
							{
								asistencia.UrlFotografia = url;
							}

						}
						var result = await _context.SaveChangesAsync() > 0;

						return result ? new ResponseGeneric<long>(asistencia.Id) : new ResponseGeneric<long>();
					}
					else //Actualizar Evaluación
					{
						var urlEvaluacion = asistencia.UrlEvaluacion;

						var res = await _blobStorage.Remove(urlEvaluacion);
						asistencia.UrlEvaluacion = "";

						foreach (var file in request[0].Files)
						{
							var url = await _blobStorage.UploadFileAsync(new byte[0], "CapacitacionEmpleadoEvaluacion/" + request[0].Id + "/" + file.Nombre, file.Base64);
							if (!string.IsNullOrEmpty(url))
							{
								asistencia.UrlEvaluacion = url;
							}

						}
						var result = await _context.SaveChangesAsync() > 0;

						return result ? new ResponseGeneric<long>(asistencia.Id) : new ResponseGeneric<long>();
					}

				}
				//Autorización de la capacitación
				else if (request[0].IdCapacitacion > 0 && request[0].AcceptOrDenied > 0)
				{
					var autorizacion = _context.Capacitacions.Where(x => x.Id == request[0].IdCapacitacion).FirstOrDefault();

					autorizacion.IdCatEstatusCapacitacion = (int)request[0].AcceptOrDenied;
					_context.Update(autorizacion);

					var result = await _context.SaveChangesAsync() > 0;
					return result ? new ResponseGeneric<long>(autorizacion.Id) : new ResponseGeneric<long>();
				}

				//Actualiza la asistencia del empleado
				else if (request[0].Id > 0)
				{
					var autorizacion = _context.CapacitacionEmpleados.Where(x => x.Id == request[0].Id).FirstOrDefault();
					var t = autorizacion.IdCapacitacion;
					var validar = _context.Capacitacions.Where(y => y.Id == t).FirstOrDefault();
					var validacionIf = validar.IdCatEstatusCapacitacion;

					if (validacionIf == 2)
					{
						asistencia = _context.CapacitacionEmpleados.Where(x => x.Id == request[0].Id).FirstOrDefault();

						if (asistencia.Asistio == false)
						{
							asistencia.Asistio = true;
							_context.Update(asistencia);
						}
						else
						{
							asistencia.Asistio = false;
							_context.Update(asistencia);
						}

						var result = await _context.SaveChangesAsync() > 0;
						return result ? new ResponseGeneric<long>(asistencia.Id) : new ResponseGeneric<long>();
					}
					else
					{
						//return new ResponseGeneric<List<CapacitacionResponse>>(ex);
						return new ResponseGeneric<long>(0);
					}

				}
				//Realiza el registro de la lista de empleado inscritos a la Capacitación
				else
				{
					capacitacionQuery = new Capacitacion
					{
						Fecha = (DateTime)request[0].FechaCapacitacion,
						Tema = request[0].TemaCapacitacion,
						IdCatEstatusCapacitacion = CatEstatusCapacitacionDic.SolicitarAutorizacion,
						IdVerificentro = (long)request[0].IdVerificentro,

					};
					_context.Capacitacions.Add(capacitacionQuery);
					var result = await _context.SaveChangesAsync() > 0;

					foreach (var obj in request)
					{
						asistencia = new CapacitacionEmpleado
						{
							IdUserPuestoVerificentro = (long)obj.IdCapEmp,
							IdCapacitacion = capacitacionQuery.Id,
							UrlFotografia = "",
							Asistio = false,
							UrlEvaluacion = "",
						};
						_context.CapacitacionEmpleados.Add(asistencia);
						await _context.SaveChangesAsync();
					}

					return result ? new ResponseGeneric<long>(capacitacionQuery.Id) : new ResponseGeneric<long>();
				}


			}
			catch (Exception ex)
			{
				return new ResponseGeneric<long>(ex);
			}
		}


	}
}

public interface ICapacitacionNegocio
{
	public Task<ResponseGeneric<List<CapacitacionResponse>>> Consulta(CapacitacionListRequest request);

	public Task<ResponseGeneric<List<CapacitacionResponse>>> GetById(long Id);

	public Task<ResponseGeneric<List<CapacitacionResponse>>> GetByIdEmpleado(long Id);

	Task<ResponseGeneric<long>> Registro(List<CapacitacionResponse> request);
}
