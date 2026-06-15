using Microsoft.EntityFrameworkCore;
using ProyectoSantaMonica_Cesar.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProyectoSantaMonica_Cesar.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
        {
        }

        
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Paciente> Paciente { get; set; }
        public DbSet<Medico> Medico { get; set; }
        public DbSet<Especialidad> Especialidad { get; set; }
        public DbSet<Horario> HorariosAtencion { get; set; }
        public DbSet<Cita> Cita { get; set; }

        public DbSet<ComprobanteDePago> ComprobanteDePago { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================================
            // COMPROBANTE
            // ================================
            modelBuilder.Entity<ComprobanteDePago>(entity =>
            {
                entity.ToTable("Comprobante_Pago");

                entity.Property(c => c.Metodo_Pago)
                      .HasConversion<string>();

                entity.Property(c => c.Estado)
                      .HasConversion<string>();
            });

            // ================================
            // CITA
            // ================================
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.Property(c => c.Estado)
                      .HasConversion<string>();
            });

            // ================================
            // HORARIO
            // ================================
            modelBuilder.Entity<Horario>(entity =>
            {
                entity.HasIndex(h => new
                {
                    h.Id_Medico,
                    h.Dia_Semana,
                    h.Horario_Entrada,
                    h.Horario_Salida
                }).IsUnique();

                entity.Property(h => h.Dia_Semana)
                      .HasConversion<string>();
            });
        }




    }
}
