namespace Smadot.Models.Entities.Permiso.Request
{
    public class PermisoTreeRequest
    {
        public int Rol { get; set; }

        public List<long> Permisos { get; set; }
    }

    public class PermisoTreeRolesRequest
    {
        public long Id { get; set; }

        public string Nombre { get; set; } = null!;
    }
}
