using System;
using System.Collections.Generic;
using System.Text;
using treino_api.Models;
using Microsoft.EntityFrameworkCore;

namespace treino_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Organizador> Organizadores {get;set;}
        public DbSet<Evento> Eventos {get;set;}
        public DbSet<Pessoa> Pessoas {get; set;}
        public DbSet<EventoOrganizador> EventosOrganizadores {get;set;}
        public DbSet<PessoaEvento> PessoasEventos {get; set;}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base (options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //definindo uma chave composta de evento organizador
            modelBuilder.Entity<EventoOrganizador>()
                .HasKey(x => new {x.EventoID, x.OrganizadorID});

            //definido o relacionamento muito para muitos
            modelBuilder.Entity<EventoOrganizador>()
            .HasOne(bc => bc.Evento)
             .WithMany(b => b.Organizadores)
              .HasForeignKey(bc => bc.EventoID);
                modelBuilder.Entity<EventoOrganizador>()
                .HasOne(bc => bc.Organizador)
                    .WithMany(c => c.Eventos)
                        .HasForeignKey(bc => bc.OrganizadorID);

            //definindo uma chave composta de pessoaevento
            modelBuilder.Entity<PessoaEvento>()
                .HasKey(x => new {x.EventoID, x.PessoaID});

            //definido o relacionamento muito par muitos
            modelBuilder.Entity<PessoaEvento>()
            .HasOne(bc => bc.Evento)
             .WithMany(b => b.Pessoas)
              .HasForeignKey(bc => bc.EventoID);
                modelBuilder.Entity<PessoaEvento>()
                .HasOne(bc => bc.Pessoa)
                    .WithMany(c => c.Eventos)
                        .HasForeignKey(bc => bc.PessoaID);
        }
    }
}