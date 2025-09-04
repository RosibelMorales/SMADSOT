using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Smadsot.FotoMulta.Model.Respositories.Models;

namespace Smadsot.FotoMulta.Model.Respositories
{
    public class VerificacionRepository
    {
        private readonly string connectionString;
        public VerificacionRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<List<Verificacion>> ObtenerVerificacionesAsync(string placa, DateTime fechaMaxima)
        {
            var verificacionesList = new List<Verificacion>();

            string query = @"
                SELECT 
                    v.IdRegistro AS IdRegistro,
                    v.Fecha,
                    v.Placa,
                    v.Serie,
                    v.Vigencia,
                    v.Marca,
                    v.Submarca
                FROM dbo.vVigenciaVerificacion v
                WHERE v.Placa IS NOT NULL 
                AND v.Placa = @Placa
                AND v.Vigencia > @FechaMaxima";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Placa", placa.Trim());
                    command.Parameters.AddWithValue("@FechaMaxima", fechaMaxima);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var verificacion = new Verificacion
                            {
                                IdRegistro = reader.GetInt64(reader.GetOrdinal("IdRegistro")),
                                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                Placa = reader.IsDBNull(reader.GetOrdinal("Placa")) ? null : reader.GetString(reader.GetOrdinal("Placa")),
                                Serie = reader.GetString(reader.GetOrdinal("Serie")),
                                Vigencia = reader.IsDBNull(reader.GetOrdinal("Vigencia")) ? null : reader.GetDateTime(reader.GetOrdinal("Vigencia")),
                                Marca = reader.GetString(reader.GetOrdinal("Marca")),
                                Submarca = reader.GetString(reader.GetOrdinal("Submarca"))
                            };

                            verificacionesList.Add(verificacion);
                        }
                    }
                }
            }

            return verificacionesList;
        }
    }
}