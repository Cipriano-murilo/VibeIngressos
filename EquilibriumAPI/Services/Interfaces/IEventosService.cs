using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EquilibriumAPI.DTOs;

namespace EquilibriumAPI.Services.Interfaces
{
    public interface IEventosService
    {
        List<EventoDto> ListarTodos(string? categoria = null);
        EventoDto? BuscarPorId(Guid id);
        Task<EventoDto> CriarAsync(CriarEventoDto dto);
        Task<EventoDto?> EditarAsync(Guid id, CriarEventoDto dto);
        Task<bool> ExcluirAsync(Guid id);
    }
}
