using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Services.Interfaces;

namespace EquilibriumAPI.Controllers
{
    [ApiController]
    [Route("api/cupons")]
    public class CuponsController : ControllerBase
    {
        private readonly ICuponsService _cuponsService;

        public CuponsController(ICuponsService cuponsService) => _cuponsService = cuponsService;

        /// <summary>Lista todos os cupons (somente admin)</summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Listar() => Ok(_cuponsService.ListarTodos());

        /// <summary>Valida um cupom durante o checkout (autenticado)</summary>
        [HttpPost("validar")]
        [Authorize]
        public IActionResult Validar([FromBody] ValidarCupomDto dto) =>
            Ok(_cuponsService.Validar(dto));

        /// <summary>Cria um novo cupom (somente admin)</summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Criar([FromBody] CriarCupomDto dto)
        {
            try
            {
                var cupom = await _cuponsService.CriarAsync(dto);
                return Created($"/api/cupons/{cupom.Id}", cupom);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>Edita um cupom existente (somente admin)</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] CriarCupomDto dto)
        {
            var cupom = await _cuponsService.EditarAsync(id, dto);
            return cupom == null ? NotFound(new { mensagem = "Cupom não encontrado." }) : Ok(cupom);
        }

        /// <summary>Remove um cupom (somente admin)</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var sucesso = await _cuponsService.ExcluirAsync(id);
            return sucesso ? NoContent() : NotFound(new { mensagem = "Cupom não encontrado." });
        }
    }
}
