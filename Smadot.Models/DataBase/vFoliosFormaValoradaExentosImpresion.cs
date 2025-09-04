using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class vFoliosFormaValoradaExentosImpresion
{
    public long Id { get; set; }

    public string Submarca { get; set; } = null!;

    public string Placa { get; set; } = null!;

    public string TarjetaCirculacion { get; set; } = null!;

    public string Serie { get; set; } = null!;

    public int Modelo { get; set; }

    public DateTime Fecha { get; set; }

    public int IdUserTecnico { get; set; }

    public string Combustible { get; set; } = null!;

    public long IdVerificentro { get; set; }

    public string Propietario { get; set; } = null!;

    public string NombreTecnico { get; set; } = null!;

    public DateTime Vigencia { get; set; }

    public string Marca { get; set; } = null!;

    public long Folio { get; set; }

    public int? IdCatTipoCertificado { get; set; }

    public string TipoCertificado { get; set; } = null!;

    public string NombreVerificentro { get; set; } = null!;

    public string ApiEndPoint { get; set; } = null!;

    public string ApiKey { get; set; } = null!;
}
