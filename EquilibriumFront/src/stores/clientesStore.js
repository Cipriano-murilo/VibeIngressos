import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api } from '../services/api.js'

export const useClientesStore = defineStore('clientes', () => {
  const clientes = ref([])
  const carregando = ref(false)

  async function carregar(busca = null) {
    carregando.value = true
    try {
      clientes.value = await api.clientes.listar(busca)
    } finally {
      carregando.value = false
    }
  }

  function getCliente(id) {
    return clientes.value.find(c => c.id === id)
  }

  async function editarCliente(id, dados) {
    const atualizado = await api.clientes.editar(id, dados)
    const idx = clientes.value.findIndex(c => c.id === id)
    if (idx !== -1) clientes.value[idx] = atualizado
    else clientes.value.push(atualizado)
    return atualizado
  }

  async function excluirCliente(id) {
    await api.clientes.excluir(id)
    clientes.value = clientes.value.filter(c => c.id !== id)
  }

  async function alternarRole(id) {
    const atualizado = await api.clientes.alternarRole(id)
    const idx = clientes.value.findIndex(c => c.id === id)
    if (idx !== -1) clientes.value[idx] = atualizado
    return atualizado
  }

  return { clientes, carregando, carregar, getCliente, editarCliente, excluirCliente, alternarRole }
})
