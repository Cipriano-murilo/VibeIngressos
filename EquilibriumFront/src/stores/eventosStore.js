import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '../services/api.js'

export const useEventosStore = defineStore('eventos', () => {
  const eventos = ref([])
  const carregando = ref(false)
  const erro = ref(null)

  const eventosDestaque = computed(() => eventos.value.filter(e => e.destaque && !isExpirado(e)))
  const categorias = computed(() => [...new Set(eventosAtivos.value.map(e => e.categoria))])

  function isExpirado(evento) {
    if (!evento.data) return false
    return new Date(evento.data) < new Date()
  }

  const eventosAtivos = computed(() => eventos.value.filter(e => !isExpirado(e)))
  const eventosExpirados = computed(() => eventos.value.filter(e => isExpirado(e)))

  async function carregar(categoria = null) {
    carregando.value = true
    erro.value = null
    try {
      eventos.value = await api.eventos.listar(categoria)
    } catch (e) {
      erro.value = e.message
    } finally {
      carregando.value = false
    }
  }

  function getEvento(id) {
    return eventos.value.find(e => String(e.id) === String(id))
  }

  async function buscarEvento(id) {
    return await api.eventos.buscar(id)
  }

  function filtrarPorCategoria(cat) {
    if (!cat || cat === 'Todos') return eventosAtivos.value
    return eventosAtivos.value.filter(e => e.categoria === cat)
  }

  async function criarEvento(dados) {
    const novoEvento = await api.eventos.criar(dados)
    eventos.value.push(novoEvento)
    return novoEvento
  }

  async function editarEvento(id, dados) {
    const eventoAtualizado = await api.eventos.editar(id, dados)
    const idx = eventos.value.findIndex(e => String(e.id) === String(id))
    if (idx !== -1) eventos.value[idx] = eventoAtualizado
    return eventoAtualizado
  }

  async function excluirEvento(id) {
    await api.eventos.excluir(id)
    eventos.value = eventos.value.filter(e => String(e.id) !== String(id))
  }

  return {
    eventos, carregando, erro,
    eventosDestaque, categorias, eventosAtivos, eventosExpirados, isExpirado,
    carregar, getEvento, buscarEvento, filtrarPorCategoria,
    criarEvento, editarEvento, excluirEvento
  }
})
