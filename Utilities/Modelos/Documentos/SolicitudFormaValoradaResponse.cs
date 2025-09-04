using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class SolicitudFormaValoradaDocument
    {
        public long IdSolicitudFV { get; set; }

        public DateTime FechaSolicitudFV { get; set; }

        public long IdUserSolicitaFV { get; set; }

        public string? UserSolicitaFV { get; set; }

        public int IdCatEstatusSolicitudFV { get; set; }

        public string EstatusFV { get; set; } = null!;

        public bool ActivoFV { get; set; }

        public DateTime FechaRegistroFV { get; set; }

        public long IdAlmacenFV { get; set; }

        public string AlmacenFV { get; set; } = null!;

        public long IdSC { get; set; }

        public int IdCatTipoCertificadoSC { get; set; }

        public string TipoCertificadoSC { get; set; } = null!;

        public string? ClaveCertificadoSC { get; set; }

        public int CantidadSC { get; set; }

        public int FolioInicialSC { get; set; }

        public int FolioFinalSC { get; set; }

        public DateTime? FechaEntregaIFV { get; set; }

        public string? NombreRecibioIFV { get; set; }

        public int? Total { get; set; }

        public List<AlmacenDocument>? Almacenes { get; set; }
        public long? IdVerificentro { get; set; }
        public string? Almacen { get; set; }
    }

    public class AlmacenDocument
    {
        public long Id { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Activo { get; set; }

        public long? IdVerificentro { get; set; }
    }
}
