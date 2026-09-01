using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EquilibriumAPI.DTOs;

namespace EquilibriumAPI.Services.Interfaces
{
    public interface IClientesService
    {
        List<UsuarioDto> Listar(string? busca = null);
        UsuarioDto? BuscarPorId(Guid id);
        Task<UsuarioDto?> AtualizarAsync(Guid id, AtualizarPerfilDto dto);
        Task<bool> ExcluirAsync(Guid id);
        Task<UsuarioDto?> AlternarRoleAsync(Guid solicitanteId, Guid alvoId);
    }
}
