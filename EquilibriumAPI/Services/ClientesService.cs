using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquilibriumAPI.Data;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Models;
using EquilibriumAPI.Services.Interfaces;

namespace EquilibriumAPI.Services
{
    public class ClientesService : IClientesService
    {
        private readonly AppDbContext _db;

        public ClientesService(AppDbContext db) => _db = db;

        public List<UsuarioDto> Listar(string? busca = null)
        {
            var query = _db.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.ToLowerInvariant();
                query = query.Where(u =>
                    u.Nome.ToLower().Contains(termo) ||
                    u.Email.ToLower().Contains(termo) ||
                    (u.Cpf != null && u.Cpf.Contains(termo)));
            }

            return query
                .OrderByDescending(u => u.CriadoEm)
                .Select(u => Mapear(u))
                .ToList();
        }

        public UsuarioDto? BuscarPorId(Guid id)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == id);
            return usuario is null ? null : Mapear(usuario);
        }

        public async Task<UsuarioDto?> AtualizarAsync(Guid id, AtualizarPerfilDto dto)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario is null) return null;

            if (dto.Nome is not null)              usuario.Nome          = dto.Nome.Trim();
            if (dto.Celular is not null)           usuario.Celular       = dto.Celular;
            if (dto.DataNascimento.HasValue)       usuario.DataNascimento = dto.DataNascimento;

            await _db.SaveChangesAsync();
            return Mapear(usuario);
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario is null) return false;

            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<UsuarioDto?> AlternarRoleAsync(Guid solicitanteId, Guid alvoId)
        {
            if (solicitanteId == alvoId)
                throw new InvalidOperationException("Não é possível alterar o próprio perfil de acesso.");

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == alvoId);
            if (usuario is null) return null;

            usuario.Role = usuario.Role == "admin" ? "client" : "admin";
            await _db.SaveChangesAsync();
            return Mapear(usuario);
        }

        private static UsuarioDto Mapear(Usuario u) =>
            new(u.Id, u.Nome, u.Email, u.Role, u.Cpf, u.Celular, u.DataNascimento);
    }
}
