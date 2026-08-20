import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '../services/api.js'
import { EventoModel } from '../services/EventoModel.js'

/**
 * EventosStore — repositório reativo de eventos.
 *
 * OOP/Padrões:
 *  - Repository Pattern: o store é a única fonte de verdade para eventos
 *  - Factory Method: todos os objetos brutos da API são mapeados para EventoModel
 *
 * Segurança:
 *  - Dados são sempre normalizados via EventoModel.fromApi() antes de entrar no estado
 *  - Erros expostos ao usuário nunca incluem stack traces
 */
export const useEventosStore = defineStore('eventos', () => {
  /** @type {import('vue').Ref<EventoModel[]>} */
  const eventos    = ref([])
  const carregando = ref(false)
  const erro       = ref(null)

  // ---- Getters -------------------------------------------------------

  const eventosAtivos    = computed(() => eventos.value.filter(e => !e.expirado))
  const eventosExpirados = computed(() => eventos.value.filter(e =>  e.expirado))
  const eventosDestaque  = computed(() => eventosAtivos.value.filter(e => e.destaque))
  const categorias       = computed(() => [...new Set(eventosAtivos.value.map(e => e.categoria))])

  // ---- Helpers -------------------------------------------------------

  function _fromApiList(lista) {
    return Array.isArray(lista) ? lista.map(EventoModel.fromApi) : []
  }

  function _indexById(id) {
    return eventos.value.findIndex(e => String(e.id) === String(id))
  }

  // ---- Actions -------------------------------------------------------

  async function carregar(categoria = null) {
    carregando.value = true
    erro.value       = null
    try {
      const raw        = await api.eventos.listar(categoria)
      eventos.value    = _fromApiList(raw)
    } catch (e) {
      erro.value = e.message
    } finally {
      carregando.value = false
    }
  }

  /** Retorna instância já normalizada do cache local. */
  function getEvento(id) {
    return eventos.value.find(e => String(e.id) === String(id)) ?? null
  }

  /** Busca diretamente na API (cache miss ou dados detalhados). */
  async function buscarEvento(id) {
    const raw = await api.eventos.buscar(id)
    return EventoModel.fromApi(raw)
  }

  /** Filtra apenas eventos ativos por categoria. */
  function filtrarPorCategoria(cat) {
    if (!cat || cat === 'Todos') return eventosAtivos.value
    return eventosAtivos.value.filter(e => e.categoria === cat)
  }

  async function criarEvento(dados) {
    const raw        = await api.eventos.criar(dados)
    const novoEvento = EventoModel.fromApi(raw)
    eventos.value.push(novoEvento)
    return novoEvento
  }

  async function editarEvento(id, dados) {
    const raw              = await api.eventos.editar(id, dados)
    const eventoAtualizado = EventoModel.fromApi(raw)
    const idx              = _indexById(id)
    if (idx !== -1) eventos.value[idx] = eventoAtualizado
    return eventoAtualizado
  }

  async function excluirEvento(id) {
    await api.eventos.excluir(id)
    eventos.value = eventos.value.filter(e => String(e.id) !== String(id))
  }

  return {
    eventos, carregando, erro,
    eventosAtivos, eventosExpirados, eventosDestaque, categorias,
    carregar, getEvento, buscarEvento, filtrarPorCategoria,
    criarEvento, editarEvento, excluirEvento,
    // Exposição de isExpirado mantida para compatibilidade com templates existentes
    isExpirado: (e) => e instanceof EventoModel ? e.expirado : false,
  }
})
