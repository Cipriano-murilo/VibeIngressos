/**
 * TokenManager — gerencia ciclo de vida do JWT no lado do cliente.
 * Usa sessionStorage por padrão (token não persiste após fechar a aba),
 * com fallback opcional para localStorage quando "lembrar sessão" for marcado.
 *
 * Princípios aplicados:
 *  - Single Responsibility: esta classe só cuida de token
 *  - Encapsulation: chaves internas são privadas ao módulo
 *  - Defense in Depth: valida estrutura do JWT antes de aceitar
 */

const TOKEN_KEY = 'vb_tk'
const USER_KEY  = 'vb_us'
const REMEMBER_KEY = 'vb_rm'

/**
 * Decodifica o payload de um JWT SEM verificar assinatura.
 * Usado apenas para ler claims localmente (ex.: expiração).
 * A assinatura real é sempre verificada pelo backend.
 */
function decodeJwtPayload(token) {
  try {
    const [, payloadB64] = token.split('.')
    if (!payloadB64) return null
    const json = atob(payloadB64.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(json)
  } catch {
    return null
  }
}

export class TokenManager {
  /** Persiste token e usuário. Se `remember` for true, usa localStorage. */
  static save(token, user, remember = false) {
    const storage = remember ? localStorage : sessionStorage

    // Remove da outra storage para evitar duplicidade
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
    sessionStorage.removeItem(TOKEN_KEY)
    sessionStorage.removeItem(USER_KEY)

    storage.setItem(TOKEN_KEY, token)
    storage.setItem(USER_KEY, JSON.stringify(user))
    if (remember) localStorage.setItem(REMEMBER_KEY, '1')
  }

  /** Recupera o token do storage correto. */
  static getToken() {
    return localStorage.getItem(TOKEN_KEY) ?? sessionStorage.getItem(TOKEN_KEY) ?? null
  }

  /** Recupera o usuário desserializado. */
  static getUser() {
    const raw = localStorage.getItem(USER_KEY) ?? sessionStorage.getItem(USER_KEY)
    try { return raw ? JSON.parse(raw) : null }
    catch { return null }
  }

  /** Verifica se o token existe E ainda não expirou (claim `exp`). */
  static isValid() {
    const token = this.getToken()
    if (!token) return false

    const payload = decodeJwtPayload(token)
    if (!payload?.exp) return false

    // `exp` é em segundos; Date.now() em ms
    return payload.exp * 1000 > Date.now()
  }

  /** Remove todos os dados de sessão de ambas as storages. */
  static clear() {
    ;[localStorage, sessionStorage].forEach(s => {
      s.removeItem(TOKEN_KEY)
      s.removeItem(USER_KEY)
    })
    localStorage.removeItem(REMEMBER_KEY)
  }
}
