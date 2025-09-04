using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.IngresoFormaValorada.Response
{
    public class IngresoFormaValoradaResponse
    {
        public long Id { get; set; }

        public DateTime FechaEntrega { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public DateTime FechaSolicitudFV { get; set; }

        public string NombreRecibio { get; set; }

        public int IdSolicitudFV { get; set; }

        public int IdUserRegistro { get; set; }

        public string? NombreUser { get; set; }

        public int Estatus { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long IdAlmacen { get; set; }



        public long IdCatTipoCertificado { get; set; }

        public string TipoCertificadoSC { get; set; }

        public int CantidadSolicitada { get; set; }

        public int CantidadRecibida { get; set; }

        public int CantidadSolicitadaTotal { get; set; }

        public int CantidadStock { get; set; }

        public int FolioInicial { get; set; }

        public int FolioFinal { get; set; }


        public int kt_dropzonejs_example_1 { get; set; }

        public int kt_dropzonejs_example_2 { get; set; }

        public int kt_dropzonejs_example_3 { get; set; }


        public long IdInventario { get; set; }

        public long? IdDevolucionCertificado { get; set; }

        public long? IdAsignacionCertificado { get; set; }

        public long? IdIngresoCertificado { get; set; }

        public long? IdVentaCertificado { get; set; }


        public List<AlmacenResponse>? Almacenes { get; set; }



        public int IdCatTipoCertificadoSC { get; set; }

        public string? NombreCatTipoCertificado { get; set; }

        //public string TipoCertificadoSC { get; set; } = null!;

        public string? ClaveCertificadoSC { get; set; }

        public int CantidadSC { get; set; }

        public int FolioInicialSC { get; set; }

        public int FolioFinalSC { get; set; }
        public int? Total { get; set; }

        public string? FilesString { get; set; }
        public List<IngresoFormaValoradaFiles>? Files { get; set; }
    }

    public class IngresoFormaValoradaResponseGrid : IngresoFormaValoradaResponse
    {
        public string Acciones { get; set; }
    }
    public class IngresoFormaValoradaFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }
}
