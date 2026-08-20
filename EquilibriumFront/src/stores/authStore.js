import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '../services/api.js'
import { TokenManager } from '../services/TokenManager.js'
import { Sanitizer } from '../services/Sanitizer.js'

/**
 * AuthStore — gerencia estado de autenticação do usuário.
 *
 * OOP aplicado:
 *  - Single Responsibility: este store só cuida de autenticação/sessão
 *  - Encapsulation: token não é exposto diretamente; lido via TokenManager
 *
 * Segurança:
 *  - Token gerenciado pelo TokenManager (sessionStorage por padrão)
 *  - Validação de e-mail e senha ANTES de bater no backend
 *  - Nenhum dado sensível é logado
 *  - isValid() verifica expiração do JWT localmente
 */
export const useAuthStore = defineStore('auth', () => {
  // Estado carregado do storage via TokenManager
  const user  = ref(TokenManager.getUser())
  const _tokenValid = ref(TokenManager.isValid())

  // Getters
  const isAuthenticated = computed(() => _tokenValid.value && !!user.value)
  const isAdmin         = computed(() => user.value?.role === 'admin')
  const currentUser     = computed(() => user.value)

  /** Atualiza estado reativo a partir do storage atual. */
  function _sync() {
    user.value       = TokenManager.getUser()
    _tokenValid.value = TokenManager.isValid()
  }

  /**
   * Realiza login.
   * @param {string} email
   * @param {string} senha
   * @param {boolean} remember  - se true, persiste sessão no localStorage
   */
  async function login(email, senha, remember = false) {
    if (!Sanitizer.isValidEmail(email)) {
      throw new Error('Formato de e-mail inválido.')
    }
    if (!senha || senha.length < 4) {
      throw new Error('Senha muito curta.')
    }

    const result = await api.auth.login(email, senha)
    TokenManager.save(result.token, result.usuario, remember)
    _sync()
    return result.usuario
  }

  /**
   * Cadastra novo usuário com validações client-side.
   */
  async function cadastrar(dados) {
    if (!Sanitizer.isValidEmail(dados.email)) {
      throw new Error('Formato de e-mail inválido.')
    }

    const pwCheck = Sanitizer.validatePassword(dados.senha)
    if (!pwCheck.valid) throw new Error(pwCheck.message)

    if (dados.cpf && !Sanitizer.isValidCpf(dados.cpf)) {
      throw new Error('CPF inválido.')
    }

    const payload = {
      Nome:           Sanitizer.sanitizeText(dados.nome),
      Email:          dados.email.trim().toLowerCase(),
      Senha:          dados.senha,
      Cpf:            dados.cpf  ? dados.cpf.replace(/\D/g, '') : null,
      Celular:        dados.celular ? dados.celular.replace(/\D/g, '') : null,
      DataNascimento: dados.dataNascimento
        ? new Date(dados.dataNascimento).toISOString()
        : null,
    }

    const result = await api.auth.cadastro(payload)
    TokenManager.save(result.token, result.usuario, false)
    _sync()
    return result.usuario
  }

  async function recuperarSenha(email) {
    if (!Sanitizer.isValidEmail(email)) {
      throw new Error('Formato de e-mail inválido.')
    }
    await api.auth.recuperarSenha(email)
    return true
  }

  /** Encerra a sessão e limpa todos os dados locais. */
  function logout() {
    TokenManager.clear()
    _sync()
  }

  /** Atualiza dados do usuário em cache sem refazer login. */
  function atualizarPerfil(dados) {
    const current = TokenManager.getUser()
    if (!current) return
    const updated = { ...current, ...dados }
    const token   = TokenManager.getToken()
    TokenManager.save(token, updated, !!localStorage.getItem('vb_rm'))
    _sync()
  }

  return {
    // Estado
    user, isAuthenticated, isAdmin, currentUser,
    // Ações
    login, cadastrar, recuperarSenha, logout, atualizarPerfil,
  }
})
