using System;
using System.Collections.Generic;

namespace EquilibriumAPI.Models
{
    public class Evento
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string? Local { get; set; }
        public string? Imagem { get; set; }
        public bool Destaque { get; set; } = false;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<TipoIngresso> TiposIngresso { get; set; } = new List<TipoIngresso>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
