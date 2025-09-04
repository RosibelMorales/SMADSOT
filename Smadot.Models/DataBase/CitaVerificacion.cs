using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class CitaVerificacion
{
    public long Id { get; set; }

    public string? Nombre { get; set; }

    public string? RazonSocial { get; set; }

    public string? Correo { get; set; }

    public DateTime Fecha { get; set; }

    public long IdCVV { get; set; }

    public string Placa { get; set; } = null!;

    public int IdCatMarcaVehiculo { get; set; }

    public int IdSubMarcaVehiculo { get; set; }

    public short Anio { get; set; }

    public bool Acepto { get; set; }

    public string Serie { get; set; } = null!;

    public string ColorVehiculo { get; set; } = null!;

    public bool? Cancelada { get; set; }

    public string UrlComprobante { get; set; } = null!;

    public string Folio { get; set; } = null!;

    public int IdTipoCombustible { get; set; }

    public string NombreGeneraCita { get; set; } = null!;

    public bool Poblano { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public int? IdCatMotivoCancelacionCita { get; set; }

    public long? IdUserCancelo { get; set; }

    public long? IdUserReinicio { get; set; }

    public DateTime? FechaReinicio { get; set; }

    public virtual DocumentosCitum? DocumentosCitum { get; set; }

    public virtual Verificentro IdCVVNavigation { get; set; } = null!;

    public virtual CatMotivoCancelacionCitum? IdCatMotivoCancelacionCitaNavigation { get; set; }

    public virtual User? IdUserCanceloNavigation { get; set; }

    public virtual ICollection<Verificacion> Verificacions { get; } = new List<Verificacion>();
}
