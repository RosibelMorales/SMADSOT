using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.DashboardLineas.Response
{
    public class DashboardLineaResponseData
    {
        public long IdLinea { get; set; }
        public string Linea { get; set; }
        public int AutosEspera { get; set; }
        public string Turno { get; set; }
        public string Duenio { get; set; }
        public string Serie { get; set; }
        public string Modelo { get; set; }
        public string Placas { get; set; }
        public int EstatusPrueba { get; set; }
        public bool PruebaOBD { get; set; }
        public bool PruebaEmisiones { get; set; }
        public bool PruebaOpacidad { get; set; }
        public bool Camara { get; set; }
        public bool Line { get; set; }
        public int CRechazo { get; set; }
        public int? Resultado { get; set; }
        public DashboardLineaResponseData()
        {
            IdLinea = 0;
            Linea = string.Empty;
            AutosEspera = 0;
            Turno = "----";
            Duenio = string.Empty;
            Serie = string.Empty;
            Modelo = string.Empty;
            Placas = string.Empty;
            EstatusPrueba = -1;
            Camara = true;
            Line = true;
            CRechazo = 0;

        }
    }
}
