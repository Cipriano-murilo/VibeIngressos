/**
 * Módulo centralizador de chamadas HTTP para a Equilibrium API
 * Injeta automaticamente o Bearer Token em todas as requisições autenticadas
 */

const BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5017'

function getToken() {
  return localStorage.getItem('eq_token')
}

async function request(method, path, body = null, auth = true) {
  const headers = { 'Content-Type': 'application/json' }

  if (auth) {
    const token = getToken()
    if (token) headers['Authorization'] = `Bearer ${token}`
  }

  const options = { method, headers }
  if (body) options.body = JSON.stringify(body)

  const response = await fetch(`${BASE_URL}${path}`, options)

  // Token expirado ou inválido → força logout
  if (response.status === 401) {
    localStorage.removeItem('eq_token')
    localStorage.removeItem('eq_user')
    window.location.href = '/login'
    throw new Error('Sessão expirada. Faça login novamente.')
  }

  // Sem conteúdo (ex: DELETE)
  if (response.status === 204) return null

  let data
  try {
    const text = await response.text()
    data = text ? JSON.parse(text) : {}
  } catch (err) {
    if (!response.ok) {
      throw new Error(`Erro do servidor (Status ${response.status})`)
    }
    throw new Error('Erro ao processar resposta do servidor.')
  }

  if (!response.ok) {
    if (data?.errors) {
      // Formata ValidationProblemDetails do C#
      const msgs = Object.values(data.errors).flat().join(' ')
      throw new Error(msgs)
    }
    throw new Error(data?.mensagem || data?.title || `Erro ${response.status}`)
  }

  return data
}

// Helpers por método HTTP
export const api = {
  get:    (path, auth = true)         => request('GET',    path, null, auth),
  post:   (path, body, auth = true)   => request('POST',   path, body, auth),
  put:    (path, body, auth = true)   => request('PUT',    path, body, auth),
  delete: (path, auth = true)         => request('DELETE', path, null, auth),

  // Auth endpoints (não requerem token)
  auth: {
    login:           (email, senha)  => request('POST', '/api/auth/login',          { Email: email, Senha: senha }, false),
    cadastro:        (dados)         => request('POST', '/api/auth/cadastro',        dados,                         false),
    recuperarSenha:  (email)         => request('POST', '/api/auth/recuperar-senha', { mail: email },              false),
  },

  // Eventos
  eventos: {
    listar:     (categoria) => request('GET',    `/api/eventos${categoria && categoria !== 'Todos' ? `?categoria=${encodeURIComponent(categoria)}` : ''}`, null, false),
    buscar:     (id)        => request('GET',    `/api/eventos/${id}`, null, false),
    criar:      (dados)     => request('POST',   '/api/eventos', dados),
    editar:     (id, dados) => request('PUT',    `/api/eventos/${id}`, dados),
    excluir:    (id)        => request('DELETE', `/api/eventos/${id}`),
  },

  // Cupons
  cupons: {
    listar:   ()            => request('GET',    '/api/cupons'),
    validar:  (codigo, valorPedido) => request('POST', '/api/cupons/validar', { codigo, valorPedido }),
    criar:    (dados)       => request('POST',   '/api/cupons', dados),
    editar:   (id, dados)   => request('PUT',    `/api/cupons/${id}`, dados),
    excluir:  (id)          => request('DELETE', `/api/cupons/${id}`),
  },

  // Clientes
  clientes: {
    listar:       (busca)       => request('GET',    `/api/clientes${busca ? `?busca=${encodeURIComponent(busca)}` : ''}`),
    buscar:       (id)          => request('GET',    `/api/clientes/${id}`),
    editar:       (id, dados)   => request('PUT',    `/api/clientes/${id}`, dados),
    excluir:      (id)          => request('DELETE', `/api/clientes/${id}`),
    alternarRole: (id)          => request('PATCH',  `/api/clientes/${id}/role`),
  },

  // Pedidos
  pedidos: {
    listar:   ()            => request('GET',    '/api/pedidos'),
    listarTodos: ()         => request('GET',    '/api/pedidos/todos'),
    criar:    (dados)       => request('POST',   '/api/pedidos', dados),
  }
}
