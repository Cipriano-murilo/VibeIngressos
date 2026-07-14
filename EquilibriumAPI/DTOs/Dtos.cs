namespace EquilibriumAPI.DTOs
{
    using System;
    using System.Collections.Generic;

    // Auth DTOs
    public record LoginDto(string Email, string Senha);
    public record CadastroDto(
        string Nome,
        string Email,
        string Senha,
        string? Cpf,
        string? Celular,
        DateTime? DataNascimento
    );
    public record AuthResponseDto(string Token, UsuarioDto Usuario);

    // Usuario DTOs
    public record UsuarioDto(
        Guid Id,
        string Nome,
        string Email,
        string Role,
        string? Cpf,
        string? Celular,
        DateTime? DataNascimento
    );
    public record AtualizarPerfilDto(
        string? Nome,
        string? Celular,
        DateTime? DataNascimento
    );

    // Evento DTOs
    public record EventoDto(
        Guid Id,
        string Nome,
        string? Descricao,
        string? Categoria,
        DateTime Data,
        string? Local,
        string? Imagem,
        bool Destaque,
        List<TipoIngressoDto> Tipos
    );
    public record CriarEventoDto(
        string Nome,
        string? Descricao,
        string? Categoria,
        DateTime Data,
        string? Local,
        string? Imagem,
        bool Destaque,
        List<CriarTipoIngressoDto> Tipos
    );
    public record TipoIngressoDto(Guid Id, string Nome, decimal Preco, int Disponivel);
    public record CriarTipoIngressoDto(string Nome, decimal Preco, int Disponivel);

    // Cupom DTOs
    public record CupomDto(
        Guid Id,
        string Codigo,
        string TipoDesconto,
        decimal ValorDesconto,
        int? LimiteUsos,
        int TotalUsado,
        DateTime? ValidoAte
    );
    public record CriarCupomDto(
        string Codigo,
        string TipoDesconto,
        decimal ValorDesconto,
        int? LimiteUsos,
        DateTime? ValidoAte
    );
    public record ValidarCupomDto(string Codigo, decimal ValorPedido);
    public record ValidarCupomResponseDto(bool Valido, string? Mensagem, decimal? DescontoCalculado, CupomDto? Cupom);

    // Pedido DTOs
    public record CriarPedidoDto(
        Guid EventoId,
        string? CodigoCupom,
        List<ItemPedidoDto> Itens
    );
    public record ItemPedidoDto(Guid TipoIngressoId, int Quantidade, string? NomeDono, string? EmailDono);
    public record PedidoResponseDto(
        Guid Id,
        Guid EventoId,
        string EventoNome,
        decimal ValorTotal,
        decimal? ValorDesconto,
        DateTime CriadoEm,
        List<ItemResponseDto> Itens
    );
    public record ItemResponseDto(string TipoNome, string? NomeDono, string Status);
}
