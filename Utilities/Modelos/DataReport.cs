using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos
{
    /// <summary>
    /// Modelo de la información del reporte (Respuesta del reporte creado)
    /// </summary>
    public class DataReport
    {
        public string NombreDocumento { get; set; }
        public byte[] DocumentoPDF { get; set; }
    }

    public class DataExcelReport
    {
        public string NombreDocumento { get; set; }
        public byte[] DocumentoExcel { get; set; }
    }
}
