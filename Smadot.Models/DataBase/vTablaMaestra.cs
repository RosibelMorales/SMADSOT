using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vTablaMaestra
{
    public string Marca { get; set; } = null!;

    public int IdRegistroSubMarca { get; set; }

    public string SubMarca { get; set; } = null!;

    public long Id { get; set; }

    public int IdCatMarcaVehiculo { get; set; }

    public int IdCatSubmarcaVehiculo { get; set; }

    public int Motor_DSL { get; set; }

    public int COMB_ORIG { get; set; }

    public int CARROCERIA { get; set; }

    public int ALIM_COMB { get; set; }

    public int CILINDROS { get; set; }

    public int CILINDRADA { get; set; }

    public int PBV { get; set; }

    public int PBV_EQUIV { get; set; }

    public int PBV_ASM { get; set; }

    public int CONV_CATAL { get; set; }

    public int OBD { get; set; }

    public int C_ABS { get; set; }

    public int T_TRACC { get; set; }

    public int C_TRACC { get; set; }

    public int T_PRUEBA { get; set; }

    public int PROTOCOLO { get; set; }

    public int POTMAX_RPM { get; set; }

    public int ANO_DESDE { get; set; }

    public int ANO_HASTA { get; set; }

    public decimal O2_MAX { get; set; }

    public decimal LAMDA_MAX { get; set; }

    public decimal POT_5024 { get; set; }

    public decimal POT_2540 { get; set; }

    public int DOBLECERO { get; set; }

    public int CERO_GASOL { get; set; }

    public int CERO_GASLP { get; set; }

    public int CERO_GASNC { get; set; }

    public int CERO_DSL { get; set; }

    public int REF_00 { get; set; }
}
