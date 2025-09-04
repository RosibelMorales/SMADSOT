using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smadot.Models.Entities.InfraccionesPendientesPago.Response;
using Smadot.Utilities.Modelos;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Smadot.Utilities.ServicioMultas
{
    public class ConsultaVehiculoServicio : IConsultaVehiculoServicio
    {
        private readonly string _serviceEndPoint;
        private readonly string token;
        private readonly string password;

        public ConsultaVehiculoServicio(IConfiguration configuration)
        {
            _serviceEndPoint = configuration["ConsultaVehiculoServicio:ServiceEndPoint"];
            token = configuration["ConsultaVehiculoServicio:Token"];
            password = configuration["ConsultaVehiculoServicio:Password"];
        }
        public async Task<InfraccionesPendientesResponse> Consulta(string nserie, string nplaca)
        {
            var _ = new InfraccionesPendientesResponse();
            try
            {
                var client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(1)
                };
                var request = new HttpRequestMessage(HttpMethod.Post, _serviceEndPoint);
                request.Headers.Add("Authorization", "Bearer " + token);
                StringContent content = new StringContent(JsonSerializer.Serialize(new
                {
                    consulta = $"sr {nserie}",
                }), Encoding.UTF8, "application/json");
                request.Content = content;
                var response1 = await client.SendAsync(request);
                if (response1.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    _ = JsonSerializer.Deserialize<InfraccionesPendientesResponse>(await response1.Content.ReadAsStringAsync(), options);
                    
                }
                if (_.codigo == 2)
                {
                    var request2 = new HttpRequestMessage(HttpMethod.Post, _serviceEndPoint);
                    request2.Headers.Add("Authorization", "Bearer " + token);
                    content = new StringContent(JsonSerializer.Serialize(new
                    {
                        consulta = $"plc {nplaca}",
                    }), Encoding.UTF8, "application/json");
                    request2.Content = content;
                    var response2 = await client.SendAsync(request2);
                    if (response2.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        _ = JsonSerializer.Deserialize<InfraccionesPendientesResponse>(await response2.Content.ReadAsStringAsync(), options);
                    }
                    else
                    {
                        _.codigo = 0;
                        _.desc = "NO HAY REGISTRO DE SU VEHÍCULO, POR FAVOR ACUDA A SU OFICINA RECAUDADORA MÁS CERCANA";
                    }

                }
            }
            catch (Exception e)
            {
                _.codigo = 0;
                _.desc = "NO HAY REGISTRO DE SU VEHÍCULO, POR FAVOR ACUDA A SU OFICINA RECAUDADORA MÁS CERCANA";
            }
            return _;
        }
    }

    public interface IConsultaVehiculoServicio
    {
        public Task<InfraccionesPendientesResponse> Consulta(string nserie, string nplaca);
    }

}
