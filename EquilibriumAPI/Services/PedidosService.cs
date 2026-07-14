using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EquilibriumAPI.Data;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Models;

namespace EquilibriumAPI.Services
{
    public class PedidosService
    {
        private readonly AppDbContext _db;
        private readonly CuponsService _cuponsService;

        public PedidosService(AppDbContext db, CuponsService cuponsService)
        {
            _db = db;
            _cuponsService = cuponsService;
        }

        public List<PedidoResponseDto> ListarPorUsuario(Guid usuarioId) =>
            _db.Pedidos
                .Include(p => p.Evento)
                .Include(p => p.Itens).ThenInclude(i => i.TipoIngresso)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.CriadoEm)
                .Select(p => MapearPedido(p))
                .ToList();

        public List<PedidoResponseDto> ListarTodos() =>
            _db.Pedidos
                .Include(p => p.Evento)
                .Include(p => p.Itens).ThenInclude(i => i.TipoIngresso)
                .OrderByDescending(p => p.CriadoEm)
                .Select(p => MapearPedido(p))
                .ToList();

        public async Task<PedidoResponseDto> CriarAsync(Guid usuarioId, CriarPedidoDto dto)
        {
            var evento = _db.Eventos.Include(e => e.TiposIngresso).FirstOrDefault(e => e.Id == dto.EventoId)
                ?? throw new KeyNotFoundException("Evento não encontrado.");

            decimal valorTotal = 0;
            var itens = new List<ItemPedido>();

            foreach (var item in dto.Itens)
            {
                var tipo = evento.TiposIngresso.FirstOrDefault(t => t.Id == item.TipoIngressoId)
                    ?? throw new InvalidOperationException($"Tipo de ingresso {item.TipoIngressoId} não encontrado.");

                if (tipo.Disponivel < item.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente para '{tipo.Nome}'. Disponível: {tipo.Disponivel}.");

                valorTotal += tipo.Preco * item.Quantidade;
                
                // Cria 1 ticket (ItemPedido) para cada Quantidade solicitada
                for (int i = 0; i < item.Quantidade; i++)
                {
                    itens.Add(new ItemPedido
                    {
                        TipoIngressoId = tipo.Id,
                        OwnerName = item.NomeDono,
                        OwnerEmail = item.EmailDono,
                        QrCodeHash = Guid.NewGuid().ToString("N"),
                        Status = "valid",
                        CriadoEm = DateTime.UtcNow
                    });
                }

                // Baixar estoque
                tipo.Disponivel -= item.Quantidade;
            }

            decimal? valorDesconto = null;
            Guid? cupomId = null;

            if (!string.IsNullOrEmpty(dto.CodigoCupom))
            {
                var validacao = _cuponsService.Validar(new(dto.CodigoCupom, valorTotal));
                if (!validacao.Valido)
                    throw new InvalidOperationException(validacao.Mensagem);

                valorDesconto = validacao.DescontoCalculado;
                cupomId = validacao.Cupom!.Id;
                valorTotal -= valorDesconto!.Value;
                await _cuponsService.IncrementarUsoAsync(cupomId.Value);
            }

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                EventoId = dto.EventoId,
                CupomId = cupomId,
                ValorTotal = valorTotal,
                ValorDesconto = valorDesconto,
                CriadoEm = DateTime.UtcNow,
                Itens = itens
            };

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();

            _db.Entry(pedido).Reference(p => p.Evento).Load();
            foreach (var i in pedido.Itens)
                _db.Entry(i).Reference(x => x.TipoIngresso).Load();

            return MapearPedido(pedido);
        }

        private static PedidoResponseDto MapearPedido(Pedido p) => new(
            p.Id, p.EventoId, p.Evento?.Nome ?? "",
            p.ValorTotal, p.ValorDesconto, p.CriadoEm,
            p.Itens.Select(i => new ItemResponseDto(i.TipoIngresso?.Nome ?? "", i.OwnerName, i.Status)).ToList()
        );
    }
}
