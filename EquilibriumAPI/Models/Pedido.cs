using System;
using System.Collections.Generic;

namespace EquilibriumAPI.Models
{
    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public Guid EventoId { get; set; }
        public Guid? CupomId { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal? ValorDesconto { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Navigation
        public Usuario Usuario { get; set; } = null!;
        public Evento Evento { get; set; } = null!;
        public Cupom? Cupom { get; set; }
        public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }

    public class ItemPedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PedidoId { get; set; }
        public Guid TipoIngressoId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? QrCodeHash { get; set; }
        public string Status { get; set; } = "valid"; // valid, used, cancelled
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Navigation
        public Pedido Pedido { get; set; } = null!;
        public TipoIngresso TipoIngresso { get; set; } = null!;
    }
}
