using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EquilibriumAPI.DTOs;

namespace EquilibriumAPI.Services.Interfaces
{
    public interface ICuponsService
    {
        List<CupomDto> ListarTodos();
        ValidarCupomResponseDto Validar(ValidarCupomDto dto);
        Task<CupomDto> CriarAsync(CriarCupomDto dto);
        Task<CupomDto?> EditarAsync(Guid id, CriarCupomDto dto);
        Task<bool> ExcluirAsync(Guid id);
        Task IncrementarUsoAsync(Guid cupomId);
    }
}
