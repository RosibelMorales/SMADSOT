using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vHistoricoCitum
{
    public long? IdVerificacion { get; set; }

    public long Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Placa { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public long IdVerificentro { get; set; }

    public string? NombrePersona { get; set; }

    public string? Correo { get; set; }

    public bool Acepto { get; set; }

    public long? IdUserCancelo { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaReinicio { get; set; }

    public long? IdUserReinicio { get; set; }

    public string? RazonSocial { get; set; }

    public string CitaSerie { get; set; } = null!;

    public bool? Cancelada { get; set; }

    public string UrlComprobante { get; set; } = null!;

    public short Anio { get; set; }

    public bool Poblano { get; set; }

    public string? Estado { get; set; }

    public string Folio { get; set; } = null!;

    public string ColorVehiculo { get; set; } = null!;

    public int IdCatMarcaVehiculo { get; set; }

    public int IdSubMarcaVehiculo { get; set; }

    public string? Modelo { get; set; }

    public string Marca { get; set; } = null!;

    public long? IdLinea { get; set; }

    public string? Linea { get; set; }

    public bool? IngresoManual { get; set; }

    public int? Orden { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public int? EstatusPrueba { get; set; }

    public int? C_RECHAZO { get; set; }

    public int? RESULTADO { get; set; }

    public bool? PruebaObd { get; set; }

    public bool? PruebaEmisiones { get; set; }

    public bool? PruebaOpacidad { get; set; }

    public int? IdTipoCertificado { get; set; }

    public string NombreCentro { get; set; } = null!;

    public int? IdRecepcionDocumentos { get; set; }

    public string? Direccion { get; set; }
}
