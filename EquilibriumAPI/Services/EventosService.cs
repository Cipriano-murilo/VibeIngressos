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
    public class EventosService
    {
        private readonly AppDbContext _db;

        public EventosService(AppDbContext db) => _db = db;

        public List<EventoDto> ListarTodos(string? categoria = null)
        {
            var query = _db.Eventos
                .Include(e => e.TiposIngresso)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoria) && categoria != "Todos")
                query = query.Where(e => e.Categoria == categoria);

            return query.OrderByDescending(e => e.Data)
                        .Select(e => MapearEvento(e))
                        .ToList();
        }

        public EventoDto? BuscarPorId(Guid id)
        {
            var evento = _db.Eventos
                .Include(e => e.TiposIngresso)
                .FirstOrDefault(e => e.Id == id);
            return evento == null ? null : MapearEvento(evento);
        }

        public async Task<EventoDto> CriarAsync(CriarEventoDto dto)
        {
            var evento = new Evento
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Categoria = dto.Categoria,
                Data = dto.Data,
                Local = dto.Local,
                Imagem = dto.Imagem,
                Destaque = dto.Destaque,
                CriadoEm = DateTime.UtcNow,
                TiposIngresso = dto.Tipos.Select(t => new TipoIngresso
                {
                    Nome = t.Nome,
                    Preco = t.Preco,
                    Disponivel = t.Disponivel
                }).ToList()
            };
            _db.Eventos.Add(evento);
            await _db.SaveChangesAsync();
            return MapearEvento(evento);
        }

        public async Task<EventoDto?> EditarAsync(Guid id, CriarEventoDto dto)
        {
            // Carrega o evento SEM os tipos (para evitar conflito no change tracker)
            var evento = _db.Eventos.FirstOrDefault(e => e.Id == id);
            if (evento == null) return null;

            evento.Nome = dto.Nome;
            evento.Descricao = dto.Descricao;
            evento.Categoria = dto.Categoria;
            evento.Data = dto.Data;
            evento.Local = dto.Local;
            evento.Imagem = dto.Imagem;
            evento.Destaque = dto.Destaque;

            // Salva as mudanças do evento primeiro
            await _db.SaveChangesAsync();

            // Deleta tipos antigos diretamente no banco (sem passar pelo change tracker)
            await _db.TiposIngresso
                .Where(t => t.EventoId == id)
                .ExecuteDeleteAsync();

            // Insere os novos tipos
            var novosTipos = dto.Tipos.Select(t => new TipoIngresso
            {
                EventoId = evento.Id,
                Nome = t.Nome,
                Preco = t.Preco,
                Disponivel = t.Disponivel
            }).ToList();

            _db.TiposIngresso.AddRange(novosTipos);
            await _db.SaveChangesAsync();

            // Recarrega os tipos para retornar dados completos
            evento.TiposIngresso = novosTipos;

            return MapearEvento(evento);
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var evento = _db.Eventos.FirstOrDefault(e => e.Id == id);
            if (evento == null) return false;
            
            // Hard delete since active column is missing
            _db.Eventos.Remove(evento); 
            await _db.SaveChangesAsync();
            return true;
        }

        private static EventoDto MapearEvento(Evento e) => new(
            e.Id, e.Nome, e.Descricao, e.Categoria,
            e.Data, e.Local, e.Imagem, e.Destaque,
            e.TiposIngresso.Select(t => new TipoIngressoDto(t.Id, t.Nome, t.Preco, t.Disponivel)).ToList()
        );
    }
}
