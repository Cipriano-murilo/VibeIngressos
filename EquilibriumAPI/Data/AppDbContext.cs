using Microsoft.EntityFrameworkCore;
using EquilibriumAPI.Models;

namespace EquilibriumAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<TipoIngresso> TiposIngresso { get; set; }
        public DbSet<Cupom> Cupons { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // profiles
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("profiles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("char(36)");
                entity.Property(e => e.Nome).HasColumnName("full_name").IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.SenhaHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.Cpf).HasColumnName("cpf");
                entity.Property(e => e.Celular).HasColumnName("phone_number");
                entity.Property(e => e.DataNascimento).HasColumnName("birth_date");
                entity.Property(e => e.CriadoEm).HasColumnName("created_at");
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // events
            modelBuilder.Entity<Evento>(entity =>
            {
                entity.ToTable("events");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("char(36)");
                entity.Property(e => e.Nome).HasColumnName("title").IsRequired();
                entity.Property(e => e.Descricao).HasColumnName("description");
                entity.Property(e => e.Categoria).HasColumnName("category");
                entity.Property(e => e.Data).HasColumnName("event_date");
                entity.Property(e => e.Local).HasColumnName("location");
                entity.Property(e => e.Imagem).HasColumnName("image_url");
                entity.Property(e => e.Destaque).HasColumnName("destaque").HasDefaultValue(false);
                entity.Property(e => e.CriadoEm).HasColumnName("created_at");
            });

            // ticket_types
            modelBuilder.Entity<TipoIngresso>(entity =>
            {
                entity.ToTable("ticket_types");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("char(36)");
                entity.Property(e => e.EventoId).HasColumnName("event_id").HasColumnType("char(36)");
                entity.Property(e => e.Nome).HasColumnName("name").IsRequired();
                entity.Property(e => e.Preco).HasColumnName("price").HasColumnType("decimal(10,2)");
                entity.Property(e => e.Disponivel).HasColumnName("available_tickets");
                entity.HasOne(e => e.Evento)
                      .WithMany(ev => ev.TiposIngresso)
                      .HasForeignKey(e => e.EventoId);
            });

            // coupons
            modelBuilder.Entity<Cupom>(entity =>
            {
                entity.ToTable("coupons");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("char(36)");
                entity.Property(e => e.Codigo).HasColumnName("code").IsRequired();
                entity.Property(e => e.TipoDesconto).HasColumnName("type");
                entity.Property(e => e.ValorDesconto).HasColumnName("value").HasColumnType("decimal(10,2)");
                entity.Property(e => e.LimiteUsos).HasColumnName("max_uses");
                entity.Property(e => e.TotalUsado).HasColumnName("used_count").HasDefaultValue(0);
                entity.Property(e => e.ValidoAte).HasColumnName("valid_until");
                entity.Property(e => e.CriadoEm).HasColumnName("created_at");
                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            // purchases
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("purchases");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("char(36)");
                entity.Property(e => e.UsuarioId).HasColumnName("user_id").HasColumnType("char(36)");
                entity.Property(e => e.EventoId).HasColumnName("event_id").HasColumnType("char(36)");
                entity.Property(e => e.CupomId).HasColumnName("coupon_id").HasColumnType("char(36)");
                entity.Property(e => e.ValorTotal).HasColumnName("total_amount").HasColumnType("decimal(10,2)");
                entity.Property(e => e.ValorDesconto).HasColumnName("discount_amount").HasColumnType("decimal(10,2)");
                entity.Property(e => e.CriadoEm).HasColumnName("purchase_date");
                entity.HasOne(e => e.Usuario).WithMany(u => u.Pedidos).HasForeignKey(e => e.UsuarioId);
                entity.HasOne(e => e.Evento).WithMany(ev => ev.Pedidos).HasForeignKey(e => e.EventoId);
            });

            // tickets -> Itens do Pedido
            modelBuilder.Entity<ItemPedido>(entity =>
            {
                entity.ToTable("tickets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasColumnType("char(36)");
                entity.Property(e => e.PedidoId).HasColumnName("purchase_id").HasColumnType("char(36)");
                entity.Property(e => e.TipoIngressoId).HasColumnName("ticket_type_id").HasColumnType("char(36)");
                entity.Property(e => e.OwnerName).HasColumnName("owner_name");
                entity.Property(e => e.OwnerEmail).HasColumnName("owner_email");
                entity.Property(e => e.QrCodeHash).HasColumnName("qr_code_hash");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.CriadoEm).HasColumnName("created_at");
                entity.HasOne(e => e.Pedido).WithMany(p => p.Itens).HasForeignKey(e => e.PedidoId);
                entity.HasOne(e => e.TipoIngresso).WithMany(t => t.ItensPedido).HasForeignKey(e => e.TipoIngressoId);
            });
        }
    }
}