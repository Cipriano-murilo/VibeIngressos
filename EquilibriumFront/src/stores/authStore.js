import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '../services/api.js'

export const useAuthStore = defineStore('auth', () => {
  // State — carregado do localStorage para persistir entre recarregamentos
  const user = ref(JSON.parse(localStorage.getItem('eq_user') || 'null'))
  const token = ref(localStorage.getItem('eq_token') || null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const isAdmin = computed(() => user.value?.role === 'admin')
  const currentUser = computed(() => user.value)

  // Persistência no localStorage
  function persistUser() {
    localStorage.setItem('eq_user', JSON.stringify(user.value))
    localStorage.setItem('eq_token', token.value || '')
  }

  // Actions — agora chamam o backend C# real
  async function login(email, senha) {
    const result = await api.auth.login(email, senha)
    // A API retorna { token, usuario }
    user.value = result.usuario
    token.value = result.token
    persistUser()
    return result.usuario
  }

  async function cadastrar(dados) {
    // Mapear os campos do formulário Vue (camelCase) para os que a API C# espera (PascalCase)
    const payload = {
      Nome: dados.nome,
      Email: dados.email,
      Senha: dados.senha,
      Cpf: dados.cpf,
      Celular: dados.celular,
      DataNascimento: dados.dataNascimento ? new Date(dados.dataNascimento).toISOString() : null
    }
    const result = await api.auth.cadastro(payload)
    // A API retorna { token, usuario }
    user.value = result.usuario
    token.value = result.token
    persistUser()
    return result.usuario
  }

  async function recuperarSenha(email) {
    await api.auth.recuperarSenha(email)
    return true
  }

  function logout() {
    user.value = null
    token.value = null
    localStorage.removeItem('eq_user')
    localStorage.removeItem('eq_token')
  }

  function atualizarPerfil(dados) {
    user.value = { ...user.value, ...dados }
    persistUser()
  }

  return {
    user, token,
    isAuthenticated, isAdmin, currentUser,
    login, cadastrar, recuperarSenha, logout, atualizarPerfil
  }
})
