using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vCitaVerificacion
{
    public long IdCita { get; set; }

    public long? IdVerificacion { get; set; }

    public string Folio { get; set; } = null!;

    public string? RazonSocial { get; set; }

    public string? NombrePropietario { get; set; }

    public DateTime Fecha { get; set; }

    public string Verificentro { get; set; } = null!;

    public string Placa { get; set; } = null!;

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string Serie { get; set; } = null!;

    public long IdCVV { get; set; }

    public short Anio { get; set; }

    public string ColorVehiculo { get; set; } = null!;

    public string NombreGeneraCita { get; set; } = null!;

    public int IdTipoCombustible { get; set; }

    public int? IdRecepcionDocumento { get; set; }

    public int IdCatMarcaVehiculo { get; set; }

    public int IdSubMarcaVehiculo { get; set; }

    public int? Orden { get; set; }

    public string? LineaNombre { get; set; }

    public string? DireccionVerificentro { get; set; }

    public string UrlComprobante { get; set; } = null!;

    public bool Poblano { get; set; }

    public string? Estado { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public int? EstatusPrueba { get; set; }
}
