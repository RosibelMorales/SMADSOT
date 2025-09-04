using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Smadsot.FotoMulta.Model.DataBase;
using Smadsot.FotoMulta.Model.Entities.Request;
using Smadsot.FotoMulta.Model.Entities.Response;
using Smadsot.FotoMulta.Model.Modelos;
using Smadsot.FotoMulta.Model.Respositories;

namespace Smadsot.FotoMulta.Negocio.Operaciones
{
    public class VerificacionVehicularNegocio : IVerificacionVehicularNegocio
    {

        private readonly SmadsotDbContext _context;
        private readonly VerificacionRepository verificacionRepository;
        public VerificacionVehicularNegocio(SmadsotDbContext context, VerificacionRepository verificacionRepository)
        {
            _context = context;
            this.verificacionRepository = verificacionRepository;
        }

        public async Task<ResponseGeneric<VerificacionResponse?>> ValidarVerificacion(VerificacionRequest request)
        {
            var _ = new ResponseGeneric<VerificacionResponse?>();
            try
            {
                TimeZoneInfo mexicoCentralTime = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

                // Convertir la fecha y hora a la hora central de México
                DateTime fechaMaxima = TimeZoneInfo.ConvertTime(DateTime.Now, mexicoCentralTime);

                fechaMaxima = fechaMaxima.AddDays(-2);
                var verificaciones = await verificacionRepository.ObtenerVerificacionesAsync(request.Placa, fechaMaxima);
                if (verificaciones.Any())
                {
                    var ultimaVerificacion = verificaciones[^1];
                    _ = new ResponseGeneric<VerificacionResponse?>(new VerificacionResponse()
                    {
                        Placa = ultimaVerificacion.Placa ?? "",
                        VerificacionVigente = DateTime.Now < ultimaVerificacion.Vigencia
                    });
                }
                else
                {
                    _.Status = ResponseStatus.Success;
                    _.Response = null;
                    _.mensaje = "No se encontró información sobre la placa proporcionada.";
                }
            }
            catch (Exception ex)
            {
                _.CurrentException = ex.Message;
                _.mensaje = "Ocurrió un error al intentar obtener la información de la placa";
            }
            return _;
        }
    }

    public interface IVerificacionVehicularNegocio
    {
        Task<ResponseGeneric<VerificacionResponse?>> ValidarVerificacion(VerificacionRequest request);
    }
}