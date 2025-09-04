using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vVerificacion
{
    public long Id { get; set; }

    public long? FolioFormaValorada { get; set; }

    public DateTime Fecha { get; set; }

    public string Placa { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public string? FolioCertificado { get; set; }

    public DateTime Vigencia { get; set; }

    public long? IdResultadosVerificacion { get; set; }

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int Anio { get; set; }

    public string Semestre { get; set; } = null!;

    public string TipoCertificado { get; set; } = null!;

    public string Combustible { get; set; } = null!;

    public string TarjetaCirculacion { get; set; } = null!;

    public long IdVereficentro { get; set; }

    public DateTime? FechaVerificacion { get; set; }

    public DateTime? InicioPruebas { get; set; }

    public DateTime? FinalizacionPruebas { get; set; }

    public string? NombreDueñoVeh { get; set; }
}
