using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Smadsot.Historico.Models.DataBase;

public partial class SmadsotHistoricoDbContext : DbContext
{
    public SmadsotHistoricoDbContext(DbContextOptions<SmadsotHistoricoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CitaVerificacionHistorico> CitaVerificacionHistoricos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CitaVerificacionHistorico>(entity =>
        {
            entity.ToTable("CitaVerificacionHistorico");

            entity.Property(e => e.CitaSerie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ColorVehiculo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaReinicio).HasColumnType("datetime");
            entity.Property(e => e.Fecha_Historico)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Folio)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Linea)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NombreCentro)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombrePersona)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Placa).HasMaxLength(10);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Serie)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UrlComprobante)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
