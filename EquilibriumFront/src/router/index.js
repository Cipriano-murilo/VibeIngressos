import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore.js'
import { TokenManager } from '../services/TokenManager.js'

const routes = [
  // Rotas públicas
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/public/HomeView.vue'),
    meta: { title: 'VibeIngressos — Sua Bilheteria Digital' }
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/public/LoginView.vue'),
    meta: { title: 'Entrar — VibeIngressos', guest: true }
  },
  {
    path: '/cadastro',
    name: 'Cadastro',
    component: () => import('../views/public/CadastroView.vue'),
    meta: { title: 'Criar Conta — VibeIngressos', guest: true }
  },
  {
    path: '/esqueci-senha',
    name: 'EsqueciSenha',
    component: () => import('../views/public/EsqueciSenhaView.vue'),
    meta: { title: 'Recuperar Senha — VibeIngressos', guest: true }
  },
  {
    path: '/evento/:id',
    name: 'EventoDetail',
    component: () => import('../views/public/EventoDetailView.vue'),
    meta: { title: 'Evento — VibeIngressos' }
  },

  // Rotas Admin (requer auth + role admin)
  {
    path: '/admin',
    redirect: '/admin/dashboard',
    component: () => import('../views/admin/AdminLayout.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
    children: [
      {
        path: 'dashboard',
        name: 'AdminDashboard',
        component: () => import('../views/admin/AdminDashboard.vue'),
        meta: { title: 'Dashboard — Admin', requiresAuth: true, requiresAdmin: true }
      },
      {
        path: 'eventos',
        name: 'AdminEventos',
        component: () => import('../views/admin/AdminEventos.vue'),
        meta: { title: 'Eventos — Admin', requiresAuth: true, requiresAdmin: true }
      },
      {
        path: 'clientes',
        name: 'AdminClientes',
        component: () => import('../views/admin/AdminClientes.vue'),
        meta: { title: 'Clientes — Admin', requiresAuth: true, requiresAdmin: true }
      },
      {
        path: 'cupons',
        name: 'AdminCupons',
        component: () => import('../views/admin/AdminCupons.vue'),
        meta: { title: 'Cupons — Admin', requiresAuth: true, requiresAdmin: true }
      }
    ]
  },

  // 404
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0, behavior: 'smooth' }
  }
})

// Navigation Guard com verificação de token
router.beforeEach((to, from, next) => {
  if (to.meta.title) document.title = to.meta.title

  const authStore = useAuthStore()

  // Se a rota exige auth, valida o token localmente antes de qualquer redirect
  if (to.meta.requiresAuth) {
    if (!TokenManager.isValid()) {
      TokenManager.clear()
      return next({ name: 'Login' })
    }
    if (!authStore.isAuthenticated) return next({ name: 'Login' })
  }

  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    return next({ name: 'Home' })
  }

  if (to.meta.guest && authStore.isAuthenticated) {
    return next(authStore.isAdmin ? { name: 'AdminDashboard' } : { name: 'Home' })
  }

  next()
})

export default router
