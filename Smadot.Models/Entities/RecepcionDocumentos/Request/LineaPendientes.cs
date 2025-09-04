using Smadot.Models.DataBase;

namespace Smadot.Models.Entities.RecepcionDocumentos.Request
{
    public class LineaPendientes
    {
        public Smadot.Models.DataBase.Linea Linea { get; set; }
        public int Pendientes { get; set; }
        public int Estatus { get; set; }
        public int UltimoEnCola { get; set; }
        public long IdEquipo { get; set; }
    }
}