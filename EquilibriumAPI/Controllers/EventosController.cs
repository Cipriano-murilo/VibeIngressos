using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Services;

namespace EquilibriumAPI.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosController : ControllerBase
    {
        private readonly EventosService _eventosService;

        public EventosController(EventosService eventosService) => _eventosService = eventosService;

        /// <summary>Lista todos os eventos (público). Filtra por categoria via ?categoria=Shows</summary>
        [HttpGet]
        public IActionResult Listar([FromQuery] string? categoria)
        {
            return Ok(_eventosService.ListarTodos(categoria));
        }

        /// <summary>Busca um evento pelo ID (público)</summary>
        [HttpGet("{id:guid}")]
        public IActionResult BuscarPorId(Guid id)
        {
            var evento = _eventosService.BuscarPorId(id);
            return evento == null ? NotFound(new { mensagem = "Evento não encontrado." }) : Ok(evento);
        }

        /// <summary>Cria um novo evento (somente admin)</summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Criar([FromBody] CriarEventoDto dto)
        {
            var evento = await _eventosService.CriarAsync(dto);
            return Created($"/api/eventos/{evento.Id}", evento);
        }

        /// <summary>Edita um evento existente (somente admin)</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] CriarEventoDto dto)
        {
            var evento = await _eventosService.EditarAsync(id, dto);
            return evento == null ? NotFound(new { mensagem = "Evento não encontrado." }) : Ok(evento);
        }

        /// <summary>Remove (soft delete) um evento (somente admin)</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var sucesso = await _eventosService.ExcluirAsync(id);
            return sucesso ? NoContent() : NotFound(new { mensagem = "Evento não encontrado." });
        }
    }
}
