using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Smadsot.FotoMulta.Model.DataBase;

public partial class SmadsotDbContext : DbContext
{
    public SmadsotDbContext(DbContextOptions<SmadsotDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPuestoVerificentro> UserPuestoVerificentros { get; set; }

    public virtual DbSet<vVigenciaVerificacion> vVigenciaVerificacions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.ToTable("Permiso");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).IsUnicode(false);
            entity.Property(e => e.Nombre).IsUnicode(false);

            entity.HasOne(d => d.IdPermisoPadreNavigation).WithMany(p => p.InverseIdPermisoPadreNavigation)
                .HasForeignKey(d => d.IdPermisoPadre)
                .HasConstraintName("FK_Permiso_Permiso");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC07385752A3");

            entity.ToTable("Rol");

            entity.Property(e => e.Alias)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolPermiso_Permiso"),
                    l => l.HasOne<Rol>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolPermiso_Rol"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdPermiso");
                        j.ToTable("RolPermiso");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC074AE0379E");

            entity.Property(e => e.Curp)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FechaNacimiento).HasColumnType("datetime");
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LockoutEnd).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(256);
            entity.Property(e => e.PersonGroupId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PersonId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Rfc)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UrlFirma)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlIne)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UrlSeguroSocial)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.IdRols).WithMany(p => p.IdUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioRol",
                    r => r.HasOne<Rol>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsuarioRol_Rol"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsuarioRol_Users"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdRol").HasName("PK__UsuarioR__656DB3BC530CB369");
                        j.ToTable("UsuarioRol");
                    });
        });

        modelBuilder.Entity<UserPuestoVerificentro>(entity =>
        {
            entity.ToTable("UserPuestoVerificentro");

            entity.Property(e => e.FechaAcreditacionNorma).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionFinal).HasColumnType("datetime");
            entity.Property(e => e.FechaCapacitacionInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaIncorpacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.FechaSeparacion).HasColumnType("datetime");
            entity.Property(e => e.NumeroTrabajador)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlContrato)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserPuestoVerificentroIdUserNavigations)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPuestoVerificentro_Users");

            entity.HasOne(d => d.IdUserRegistroNavigation).WithMany(p => p.UserPuestoVerificentroIdUserRegistroNavigations)
                .HasForeignKey(d => d.IdUserRegistro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPuestoVerificentro_Users1");
        });

        modelBuilder.Entity<vVigenciaVerificacion>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vVigenciaVerificacion");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Marca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Placa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Submarca)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vigencia).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
