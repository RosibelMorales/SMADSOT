using System;
using System.Collections.Generic;

namespace Smadsot.FotoMulta.Model.DataBase;

public partial class User
{
    public long Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public string? Nombre { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public string Genero { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public string Curp { get; set; } = null!;

    public string UrlIne { get; set; } = null!;

    public string UrlFoto { get; set; } = null!;

    public string UrlSeguroSocial { get; set; } = null!;

    public string UrlFirma { get; set; } = null!;

    public long? IdVerificentro { get; set; }

    public string? PersonId { get; set; }

    public string? PersonGroupId { get; set; }

    public virtual ICollection<UserPuestoVerificentro> UserPuestoVerificentroIdUserNavigations { get; set; } = new List<UserPuestoVerificentro>();

    public virtual ICollection<UserPuestoVerificentro> UserPuestoVerificentroIdUserRegistroNavigations { get; set; } = new List<UserPuestoVerificentro>();

    public virtual ICollection<Rol> IdRols { get; set; } = new List<Rol>();
}
