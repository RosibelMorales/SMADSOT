using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class LimiteVerificacion
{
    public int Id { get; set; }

    public int? IdCatTipoProtolo { get; set; }

    public int IdCatTipoCertificado { get; set; }

    public int? AnioInicio { get; set; }

    public int? AnioFin { get; set; }

    public int? PBVMin { get; set; }

    public int? PBVMax { get; set; }

    public bool CombustibleGas { get; set; }

    public int? Hc { get; set; }

    public decimal? Co { get; set; }

    public int? No { get; set; }

    public decimal? O2 { get; set; }

    public decimal? Coco2_Min { get; set; }

    public decimal? Coco2_Max { get; set; }

    public decimal? Factor_Lamda { get; set; }

    public decimal? Coeficiente_Absorcion_Luz { get; set; }

    public decimal? Opacidad { get; set; }
}
