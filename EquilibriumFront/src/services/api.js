/**
 * ApiClient — camada centralizada de comunicação HTTP.
 *
 * Padrões de OOP aplicados:
 *  - Single Responsibility: só faz chamadas HTTP e trata respostas
 *  - Open/Closed: novos endpoints são adicionados sem alterar o núcleo
 *  - Dependency Inversion: depende de TokenManager via injeção, não de localStorage diretamente
 *
 * Segurança aplicada:
 *  - Nunca loga tokens ou dados sensíveis no console
 *  - Valida token antes de cada requisição autenticada
 *  - Rate limiting por endpoint via debounce interno
 *  - Timeout configurável para evitar requisições penduradas
 */
import { TokenManager } from './TokenManager.js'

const BASE_URL    = import.meta.env.VITE_API_URL || 'http://localhost:5017'
const TIMEOUT_MS  = 15_000  // 15 segundos

/** Mapa de timestamps da última chamada por chave de rate-limit. */
const _rateLimitMap = new Map()

/**
 * Faz fetch com timeout.
 * @param {string} url
 * @param {RequestInit} options
 * @param {number} timeoutMs
 */
async function fetchWithTimeout(url, options, timeoutMs = TIMEOUT_MS) {
  const controller = new AbortController()
  const timer = setTimeout(() => controller.abort(), timeoutMs)
  try {
    return await fetch(url, { ...options, signal: controller.signal })
  } finally {
    clearTimeout(timer)
  }
}

/**
 * Verifica rate limit simples (client-side).
 * @param {string} key    identificador do endpoint
 * @param {number} minMs  intervalo mínimo entre chamadas (ms)
 */
function checkRateLimit(key, minMs = 500) {
  const last = _rateLimitMap.get(key) ?? 0
  const now  = Date.now()
  if (now - last < minMs) {
    throw new Error('Muitas requisições em pouco tempo. Aguarde um momento.')
  }
  _rateLimitMap.set(key, now)
}

/**
 * Núcleo de chamada HTTP.
 */
async function request(method, path, body = null, auth = true, rateLimitKey = null) {
  if (rateLimitKey) checkRateLimit(rateLimitKey)

  const headers = { 'Content-Type': 'application/json' }

  if (auth) {
    if (!TokenManager.isValid()) {
      TokenManager.clear()
      window.location.href = '/login'
      throw new Error('Sessão inválida ou expirada.')
    }
    headers['Authorization'] = `Bearer ${TokenManager.getToken()}`
  }

  const options = { method, headers }
  if (body !== null) options.body = JSON.stringify(body)

  let response
  try {
    response = await fetchWithTimeout(`${BASE_URL}${path}`, options)
  } catch (err) {
    if (err.name === 'AbortError') throw new Error('O servidor demorou muito para responder.')
    throw new Error('Falha de conexão. Verifique sua internet.')
  }

  // Token expirado/inválido no servidor
  if (response.status === 401) {
    TokenManager.clear()
    window.location.href = '/login'
    throw new Error('Sessão expirada. Faça login novamente.')
  }

  // Forbidden
  if (response.status === 403) {
    throw new Error('Você não tem permissão para realizar esta ação.')
  }

  // No Content (ex: DELETE com sucesso)
  if (response.status === 204) return null

  let data
  const text = await response.text()
  try {
    data = text ? JSON.parse(text) : {}
  } catch {
    if (!response.ok) throw new Error(`Erro do servidor (${response.status})`)
    throw new Error('Resposta inesperada do servidor.')
  }

  if (!response.ok) {
    if (data?.errors) {
      const msgs = Object.values(data.errors).flat().join(' ')
      throw new Error(msgs)
    }
    throw new Error(data?.mensagem || data?.title || `Erro ${response.status}`)
  }

  return data
}

// ---------------------------------------------------------------------------
// Interface pública — estrutura de módulos por domínio (Repository Pattern)
// ---------------------------------------------------------------------------
export const api = {
  get:    (path, auth = true)              => request('GET',    path, null, auth),
  post:   (path, body, auth = true)        => request('POST',   path, body, auth),
  put:    (path, body, auth = true)        => request('PUT',    path, body, auth),
  patch:  (path, body = null, auth = true) => request('PATCH',  path, body, auth),
  delete: (path, auth = true)              => request('DELETE', path, null, auth),

  auth: {
    login:          (email, senha) => request('POST', '/api/auth/login',          { Email: email, Senha: senha }, false, 'auth.login'),
    cadastro:       (dados)        => request('POST', '/api/auth/cadastro',        dados,                         false, 'auth.cadastro'),
    recuperarSenha: (email)        => request('POST', '/api/auth/recuperar-senha', { mail: email },               false, 'auth.recuperarSenha'),
  },

  eventos: {
    listar:  (categoria) => request('GET',    `/api/eventos${categoria && categoria !== 'Todos' ? `?categoria=${encodeURIComponent(categoria)}` : ''}`, null, false),
    buscar:  (id)        => request('GET',    `/api/eventos/${encodeURIComponent(id)}`, null, false),
    criar:   (dados)     => request('POST',   '/api/eventos', dados),
    editar:  (id, dados) => request('PUT',    `/api/eventos/${encodeURIComponent(id)}`, dados),
    excluir: (id)        => request('DELETE', `/api/eventos/${encodeURIComponent(id)}`),
  },

  cupons: {
    listar:  ()                   => request('GET',    '/api/cupons'),
    validar: (codigo, valorPedido) => request('POST',  '/api/cupons/validar', { codigo, valorPedido }, true, 'cupons.validar'),
    criar:   (dados)              => request('POST',   '/api/cupons', dados),
    editar:  (id, dados)          => request('PUT',    `/api/cupons/${encodeURIComponent(id)}`, dados),
    excluir: (id)                 => request('DELETE', `/api/cupons/${encodeURIComponent(id)}`),
  },

  clientes: {
    listar:       (busca)      => request('GET',   `/api/clientes${busca ? `?busca=${encodeURIComponent(busca)}` : ''}`),
    buscar:       (id)         => request('GET',   `/api/clientes/${encodeURIComponent(id)}`),
    editar:       (id, dados)  => request('PUT',   `/api/clientes/${encodeURIComponent(id)}`, dados),
    excluir:      (id)         => request('DELETE',`/api/clientes/${encodeURIComponent(id)}`),
    alternarRole: (id)         => request('PATCH', `/api/clientes/${encodeURIComponent(id)}/role`),
  },

  pedidos: {
    listar:      () => request('GET',  '/api/pedidos'),
    listarTodos: () => request('GET',  '/api/pedidos/todos'),
    criar:  (dados)  => request('POST', '/api/pedidos', dados, true, 'pedidos.criar'),
  },
}
