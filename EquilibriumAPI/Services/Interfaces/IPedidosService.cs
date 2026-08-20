using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EquilibriumAPI.DTOs;

namespace EquilibriumAPI.Services.Interfaces
{
    public interface IPedidosService
    {
        List<PedidoResponseDto> ListarPorUsuario(Guid usuarioId);
        List<PedidoResponseDto> ListarTodos();
        Task<PedidoResponseDto> CriarAsync(Guid usuarioId, CriarPedidoDto dto);
    }
}
