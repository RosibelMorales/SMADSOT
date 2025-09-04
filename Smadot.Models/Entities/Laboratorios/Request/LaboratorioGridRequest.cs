using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Laboratorios.Request
{
    public class LaboratorioGridRequest:RequestList
    {
        public bool IsLaboratorio { get; set; }
    }
}
