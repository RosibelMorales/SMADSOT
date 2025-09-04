using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosFormaValoradaVerificentro
{
    public long Id { get; set; }

    public long Folio { get; set; }

    public long IdVerificentro { get; set; }

    public decimal ImporteActual { get; set; }

    public long? IdVerificacion { get; set; }

    public bool Cancelado { get; set; }

    public int? IdCatTipoCertificado { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public int? IdCatMotivoCancelacion { get; set; }

    public string? OtroMotivo { get; set; }

    public long? IdUserCancelo { get; set; }

    public bool Impreso { get; set; }

    public int? IdCatTipoTramite { get; set; }

    public DateTime FechaRegistro { get; set; }

    public string? UserCancelo { get; set; }

    public string CatTipoCertificado { get; set; } = null!;

    public string Verificentro { get; set; } = null!;

    public string? CatMotivoCancelacion { get; set; }

    public string? Marca { get; set; }

    public string? Placa { get; set; }

    public int? Anio { get; set; }

    public string? NombreDueñoVeh { get; set; }

    public DateTime? FechaCitaVerificacion { get; set; }

    public string? ColorVehiculo { get; set; }
}
