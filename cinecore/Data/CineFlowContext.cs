using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using cinecore.Models;

namespace cinecore.Data
{
    public class CineFlowContext : DbContext
    {
        public CineFlowContext(DbContextOptions<CineFlowContext> options) : base(options)
        {
        }

        public DbSet<Cinema> Cinemas => Set<Cinema>();
        public DbSet<Sala> Salas => Set<Sala>();
        public DbSet<Assento> Assentos => Set<Assento>();
        public DbSet<Filme> Filmes => Set<Filme>();
        public DbSet<Sessao> Sessoes => Set<Sessao>();
        public DbSet<Ingresso> Ingressos => Set<Ingresso>();
        public DbSet<IngressoInteira> IngressosInteira => Set<IngressoInteira>();
        public DbSet<IngressoMeia> IngressosMeia => Set<IngressoMeia>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Administrador> Administradores => Set<Administrador>();
        public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
        public DbSet<PedidoAlimento> PedidosAlimento => Set<PedidoAlimento>();
        public DbSet<ItemPedidoAlimento> ItensPedidoAlimento => Set<ItemPedidoAlimento>();
        public DbSet<ProdutoAlimento> ProdutosAlimento => Set<ProdutoAlimento>();
        public DbSet<AluguelSala> AlugueisSala => Set<AluguelSala>();
        public DbSet<EscalaLimpeza> EscalasLimpeza => Set<EscalaLimpeza>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasDiscriminator<string>("TipoUsuario")
                .HasValue<Administrador>("Administrador")
                .HasValue<Cliente>("Cliente");

            modelBuilder.Entity<Ingresso>()
                .HasDiscriminator<string>("TipoIngresso")
                .HasValue<IngressoInteira>("Inteira")
                .HasValue<IngressoMeia>("Meia");

            modelBuilder.Entity<Cinema>()
                .HasMany(c => c.Salas)
                .WithOne(s => s.Cinema)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cinema>()
                .HasMany(c => c.Funcionarios)
                .WithOne(f => f.Cinema)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sala>()
                .HasMany(s => s.Assentos)
                .WithOne(a => a.Sala)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Filme>()
                .HasMany(f => f.Sessoes)
                .WithOne(s => s.Filme)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sessao>()
                .HasOne(s => s.Sala)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sessao>()
                .HasMany(s => s.Ingressos)
                .WithOne(i => i.Sessao)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assento>()
                .HasOne(a => a.Ingresso)
                .WithOne(i => i.Assento)
                .HasForeignKey<Ingresso>("AssentoId")
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Ingressos)
                .WithOne(i => i.Cliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Pedidos)
                .WithOne(p => p.Cliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Cortesias)
                .WithMany();

            modelBuilder.Entity<PedidoAlimento>()
                .HasMany(p => p.Itens)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemPedidoAlimento>()
                .HasOne(i => i.Produto)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AluguelSala>()
                .HasOne(a => a.Sala)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AluguelSala>()
                .HasOne(a => a.Cliente)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EscalaLimpeza>()
                .HasOne(e => e.Sala)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EscalaLimpeza>()
                .HasOne(e => e.Funcionario)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            var jsonOptions = new JsonSerializerOptions();
            var trocoComparer = new ValueComparer<Dictionary<decimal, int>>(
                (left, right) => JsonSerializer.Serialize(left ?? new Dictionary<decimal, int>(), jsonOptions) ==
                                 JsonSerializer.Serialize(right ?? new Dictionary<decimal, int>(), jsonOptions),
                value => JsonSerializer.Serialize(value ?? new Dictionary<decimal, int>(), jsonOptions).GetHashCode(),
                value => value == null ? new Dictionary<decimal, int>() : new Dictionary<decimal, int>(value));

            modelBuilder.Entity<Ingresso>()
                .Property(i => i.TrocoDetalhado)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new Dictionary<decimal, int>(), jsonOptions),
                    v => string.IsNullOrWhiteSpace(v)
                        ? new Dictionary<decimal, int>()
                        : JsonSerializer.Deserialize<Dictionary<decimal, int>>(v, jsonOptions) ?? new Dictionary<decimal, int>())
                .Metadata.SetValueComparer(trocoComparer);

            modelBuilder.Entity<Ingresso>()
                .Property(i => i.ValorPago)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Ingresso>()
                .Property(i => i.ValorTroco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoAlimento>()
                .Property(p => p.TrocoDetalhado)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new Dictionary<decimal, int>(), jsonOptions),
                    v => string.IsNullOrWhiteSpace(v)
                        ? new Dictionary<decimal, int>()
                        : JsonSerializer.Deserialize<Dictionary<decimal, int>>(v, jsonOptions) ?? new Dictionary<decimal, int>())
                .Metadata.SetValueComparer(trocoComparer);

            modelBuilder.Entity<PedidoAlimento>()
                .Property(p => p.ValorPago)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoAlimento>()
                .Property(p => p.ValorTroco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<AluguelSala>()
                .Property(a => a.Valor)
                .HasPrecision(18, 2);

            // Precisão para campos decimal
            modelBuilder.Entity<Sessao>()
                .Property(s => s.PrecoBase)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sessao>()
                .Property(s => s.PrecoFinal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProdutoAlimento>()
                .Property(p => p.Preco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoAlimento>()
                .Property(p => p.ValorTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoAlimento>()
                .Property(p => p.ValorDesconto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PedidoAlimento>()
                .Property(p => p.TaxaCancelamento)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemPedidoAlimento>()
                .Property(i => i.Preco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Ingresso>()
                .Property(i => i.TaxaReserva)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngressoInteira>()
                .Property(i => i.Preco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IngressoMeia>()
                .Property(i => i.Preco)
                .HasPrecision(18, 2);

            // ÍNDICES - Melhoram performance em consultas
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email);
            
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CPF)
                .IsUnique();
            
            modelBuilder.Entity<Sessao>()
                .HasIndex(s => s.DataHorario);
            
            modelBuilder.Entity<PedidoAlimento>()
                .HasIndex(p => p.DataPedido);
            
            modelBuilder.Entity<Ingresso>()
                .HasIndex(i => i.DataCompra);
            
            modelBuilder.Entity<AluguelSala>()
                .HasIndex(a => new { a.Inicio, a.Fim });
            
            // CONSTRAINTS - Garantem qualidade dos dados
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .IsRequired();
            
            modelBuilder.Entity<ProdutoAlimento>()
                .Property(p => p.Descricao)
                .HasDefaultValue("");
            
            
        }
    }
}
