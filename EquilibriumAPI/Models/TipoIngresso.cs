using System;
using System.Collections.Generic;

namespace EquilibriumAPI.Models
{
    public class TipoIngresso
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EventoId { get; set; }
        public string Nome { get; set; } = string.Empty; 
        public decimal Preco { get; set; }
        public int Disponivel { get; set; }

        // Navigation
        public Evento Evento { get; set; } = null!;
        public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
    }
}
