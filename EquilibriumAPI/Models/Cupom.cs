using System;
using System.Collections.Generic;

namespace EquilibriumAPI.Models
{
    public class Cupom
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Codigo { get; set; } = string.Empty;
        public string TipoDesconto { get; set; } = "percentual"; // "percentual" ou "fixo"
        public decimal ValorDesconto { get; set; }
        public int? LimiteUsos { get; set; }
        public int TotalUsado { get; set; } = 0;
        public DateTime? ValidoAte { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
