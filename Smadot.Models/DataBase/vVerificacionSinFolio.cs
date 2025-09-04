using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacionSinFolio
{
    public long IdCitaVerificacion { get; set; }

    public long IdVerificacion { get; set; }

    public long? IdFolioFormaVaoradaVerificentro { get; set; }
}
