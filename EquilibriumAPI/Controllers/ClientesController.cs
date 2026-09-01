using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Services.Interfaces;

namespace EquilibriumAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClientesService _clientesService;

        public ClientesController(IClientesService clientesService)
        {
            _clientesService = clientesService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Listar([FromQuery] string? busca = null)
        {
            var result = _clientesService.Listar(busca);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public IActionResult BuscarPorId(Guid id)
        {
            var logadoId = ObterUsuarioLogadoId();
            var isAdmin  = User.IsInRole("admin");

            if (!isAdmin && logadoId != id)
                return Forbid();

            var usuario = _clientesService.BuscarPorId(id);
            return usuario is null
                ? NotFound(new { mensagem = "Cliente não encontrado." })
                : Ok(usuario);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPerfilDto dto)
        {
            var logadoId = ObterUsuarioLogadoId();
            var isAdmin  = User.IsInRole("admin");

            if (!isAdmin && logadoId != id)
                return Forbid();

            var resultado = await _clientesService.AtualizarAsync(id, dto);
            return resultado is null
                ? NotFound(new { mensagem = "Cliente não encontrado." })
                : Ok(resultado);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var excluido = await _clientesService.ExcluirAsync(id);
            return excluido ? NoContent() : NotFound(new { mensagem = "Usuário não encontrado." });
        }

        [HttpPatch("{id:guid}/role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AlternarRole(Guid id)
        {
            var logadoId = ObterUsuarioLogadoId();

            var resultado = await _clientesService.AlternarRoleAsync(logadoId, id);
            return resultado is null
                ? NotFound(new { mensagem = "Usuário não encontrado." })
                : Ok(resultado);
        }

        private Guid ObterUsuarioLogadoId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }
    }
}
