using System;
using System.Collections.Generic;

namespace EquilibriumAPI.Models
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Role { get; set; } = "cliente"; 
        public string? Cpf { get; set; }
        public string? Celular { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
