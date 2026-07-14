import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api } from '../services/api.js'

export const useCuponsStore = defineStore('cupons', () => {
  const cupons = ref([])
  const carregando = ref(false)

  // Normaliza os campos da API para os nomes usados no front
  function normalizar(c) {
    return {
      ...c,
      tipo: c.tipoDesconto ?? c.tipo,
      desconto: c.valorDesconto ?? c.desconto,
      usoMaximo: c.limiteUsos ?? c.usoMaximo ?? 0,
      usosAtuais: c.totalUsado ?? c.usosAtuais ?? 0,
      validoAte: c.validoAte ?? null,
      ativo: c.ativo !== undefined ? c.ativo : true
    }
  }

  async function carregar() {
    carregando.value = true
    try {
      const data = await api.cupons.listar()
      cupons.value = (data || []).map(normalizar)
    } finally {
      carregando.value = false
    }
  }

  async function validarCupom(codigo, valorPedido) {
    return await api.cupons.validar(codigo, valorPedido)
  }

  async function criarCupom(dados) {
    const novo = await api.cupons.criar(dados)
    cupons.value.unshift(normalizar(novo))
    return novo
  }

  async function editarCupom(id, dados) {
    const atualizado = await api.cupons.editar(id, dados)
    const idx = cupons.value.findIndex(c => c.id === id)
    if (idx !== -1) cupons.value[idx] = normalizar(atualizado)
    return atualizado
  }

  async function excluirCupom(id) {
    await api.cupons.excluir(id)
    cupons.value = cupons.value.filter(c => c.id !== id)
  }

  return { cupons, carregando, carregar, validarCupom, criarCupom, editarCupom, excluirCupom }
})
