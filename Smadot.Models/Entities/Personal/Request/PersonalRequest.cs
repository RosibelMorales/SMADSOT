using Newtonsoft.Json;

namespace Smadot.Models.Entities.Personal.Request
{
    public class PersonalRequest
    {
        public long IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string CorreoUsuario { get; set; }
        public string TelefonoUsuario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string Rfc { get; set; }
        public string Curp { get; set; }
        public int IdPuesto { get; set; }
        public DateTime FechaCapacitacionInicio { get; set; }
        public DateTime FechaCapacitacionFinal { get; set; }
        public DateTime FechaIncorporacion { get; set; }
        public DateTime? FechaAcreditacionNorma { get; set; }
        public string NumeroTrabajador { get; set; }
        public DateTime? FechaSeparacion { get; set; }
        public List<HorarioUserRequest> HorarioRequest { get; set; }
        public string FilesString { get; set; }
        public List<PersonalFiles> Files { get; set; }
        public List<PersonalFiles> FilesFoto { get; set; }
        public List<PersonalFiles> FilesSeguroSocial { get; set; }
        public List<PersonalFiles> FilesFirma { get; set; }
        public List<PersonalFiles>? FilesContrato { get; set; }
        public string FilesFotoString { get; set; }
        public string FilesSeguroSocialString { get; set; }
        public string FilesFirmaString { get; set; }
        public string? FilesContratoString { get; set; }

    }
    public class HorarioUserRequest
    {
        public int Id { get; set; }
        public string? Dia { get; set; }
        public string? HoraInicio { get; set; }
        public TimeSpan HoraInicioTS => !string.IsNullOrEmpty(HoraInicio) ? TimeSpan.Parse(HoraInicio) : TimeSpan.Parse("00:00");
        public string? HoraFin { get; set; }
        public TimeSpan HoraFinTS => !string.IsNullOrEmpty(HoraFin) ? TimeSpan.Parse(HoraFin) : TimeSpan.Parse("00:00");

        public string? HoraInicioResponse => HoraInicioResponseTS.ToString() is "00:00:00" ? string.Empty : HoraInicioResponseTS.ToString();
        public TimeSpan HoraInicioResponseTS { get; set; }
        public string? HoraFinResponse => HoraFinResponseTS.ToString() is "00:00:00" ? string.Empty : HoraFinResponseTS.ToString();
        public TimeSpan HoraFinResponseTS { get; set; }
    }

    public class BajaPersonalRequest
    {
        public long IdUser { get; set; }
        public long IdUserPuestoVerificentro { get; set; }
        public DateTime FechaSeparacion { get; set; }
    }
    public class EstatusPuestoPersonalRequest
    {
        [JsonProperty("idCatEstatusPuesto")]
        public int IdCatEstatusPuesto { get; set; }
        [JsonProperty("idUserPuestoVerificentro")]
        public long IdUserPuestoVerificentro { get; set; }
    }
    public class PersonalFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
