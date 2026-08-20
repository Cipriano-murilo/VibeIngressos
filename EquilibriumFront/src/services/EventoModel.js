/**
 * EventoModel — representa um Evento como entidade de domínio.
 *
 * OOP aplicado:
 *  - Encapsulation: dados internos isolados; lógica de negócio dentro da classe
 *  - Single Responsibility: a classe sabe tudo sobre o domínio "Evento"
 *  - Factory Method: `EventoModel.fromApi()` normaliza dados vindos do backend
 */
export class EventoModel {
  constructor({
    id,
    nome,
    descricao,
    data,
    local,
    imagem,
    categoria,
    destaque,
    tipos,
  }) {
    this.id        = id
    this.nome      = String(nome || '').trim()
    this.descricao = String(descricao || '').trim()
    this.data      = data ? new Date(data) : null
    this.local     = String(local || '').trim()
    this.imagem    = String(imagem || '').trim()
    this.categoria = String(categoria || 'Shows').trim()
    this.destaque  = Boolean(destaque)
    this.tipos     = Array.isArray(tipos) ? tipos : []
  }

  /** Retorna true se a data do evento já passou. */
  get expirado() {
    if (!this.data) return false
    return this.data < new Date()
  }

  /** Formata a data para exibição em pt-BR. */
  get dataFormatada() {
    if (!this.data || isNaN(this.data.getTime())) return ''
    return this.data.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    })
  }

  /** Retorna somente a cidade do local (antes da vírgula). */
  get cidade() {
    return this.local.split(',')[0].trim()
  }

  /** Preço mínimo entre os tipos de ingresso disponíveis. */
  get precoMinimo() {
    if (!this.tipos.length) return null
    const precos = this.tipos.map(t => Number(t.preco)).filter(p => p > 0)
    return precos.length ? Math.min(...precos) : null
  }

  /**
   * Factory: cria uma instância a partir do objeto bruto da API.
   * @param {object} raw
   * @returns {EventoModel}
   */
  static fromApi(raw) {
    return new EventoModel(raw)
  }

  /**
   * Converte para o formato esperado pelo backend (payload de criação/edição).
   * @param {string} hora  "HH:MM"
   * @returns {object}
   */
  toPayload(hora = '20:00') {
    const [h, m] = hora.split(':').map(Number)
    const dataHora = this.data ? new Date(this.data) : new Date()
    dataHora.setHours(h, m, 0, 0)

    return {
      nome:      this.nome,
      descricao: this.descricao,
      data:      dataHora.toISOString(),
      local:     this.local,
      imagem:    this.imagem,
      categoria: this.categoria,
      destaque:  this.destaque,
      tipos:     this.tipos,
    }
  }
}
