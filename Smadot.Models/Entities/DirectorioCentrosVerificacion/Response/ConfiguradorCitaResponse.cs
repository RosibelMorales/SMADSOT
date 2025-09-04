using Newtonsoft.Json;
using Smadot.Models.DataBase;
using Smadot.Models.Entities.DirectorioCentrosVerificacion.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.DirectorioCentrosVerificacion.Response
{
    public class ConfiguradorCitaResponse : vConfiguradorCitum
    {
        public string HoraInicioString
        {
            get
            {
                return HoraInicio.ToString(@"hh\:mm");
            }
        }
        public string HoraFinString
        {
            get
            {
                return HoraFin.ToString(@"hh\:mm");
            }
        }
        public string HabilitadoStr
        {
            get
            {
                return Habilitado? "<span class=\"badge badge-success  badge-circle p-4\"><i class=\"bi bi-calendar-check text-white\"></i></span>" : "<span class=\"badge badge-danger badge-circle \"><i class=\"bi bi-calendar-x text-white p-4\"></i></span>";
            }
        }
        public string FechaStr
        {
            get
            {
                return Fecha.ToString("dd/MM/yyyy");
            }
        }
        [JsonIgnore]
        public string Acciones { get; set; }
    }
    public class EditFechaResponse
    {
        public bool Error { get; set; }
        public bool Modificar { get; set; }
    }
    public class FileResponse
    {
        public string Name { get; set; }
        public string File { get; set; }
    }
    public class PersonaCita
    {
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
    }
}
