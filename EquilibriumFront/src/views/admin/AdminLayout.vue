<template>
  <div class="admin-layout">
    <!-- Sidebar -->
    <aside class="admin-sidebar" :class="{ collapsed: sidebarCollapsed }">
      <div class="sidebar-header">
        <router-link to="/" class="sidebar-logo" id="admin-logo">
          <svg width="28" height="28" viewBox="0 0 32 32" fill="none">
            <circle cx="16" cy="16" r="16" fill="url(#grad-admin)"/>
            <path d="M10 16 L16 10 L22 16 L16 22 Z" fill="white" opacity="0.9"/>
            <circle cx="16" cy="16" r="3" fill="white"/>
            <defs>
              <linearGradient id="grad-admin" x1="0" y1="0" x2="32" y2="32">
                <stop offset="0%" stop-color="#7C3AED"/>
                <stop offset="100%" stop-color="#EC4899"/>
              </linearGradient>
            </defs>
          </svg>
          <span v-if="!sidebarCollapsed" class="sidebar-logo-text">Equilibrium</span>
        </router-link>
        <button class="sidebar-toggle" @click="sidebarCollapsed = !sidebarCollapsed" id="btn-sidebar-toggle" aria-label="Colapsar menu">
          {{ sidebarCollapsed ? '→' : '←' }}
        </button>
      </div>

      <nav class="sidebar-nav" role="navigation" aria-label="Menu admin">
        <router-link
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          class="sidebar-item"
          active-class="sidebar-item--active"
          :id="`nav-admin-${item.id}`"
          :title="sidebarCollapsed ? item.label : ''"
        >
          <span class="sidebar-icon">{{ item.icon }}</span>
          <span v-if="!sidebarCollapsed" class="sidebar-label">{{ item.label }}</span>
        </router-link>
      </nav>

      <div class="sidebar-footer">
        <router-link to="/" class="sidebar-item" id="nav-ver-site" :title="sidebarCollapsed ? 'Ver site' : ''">
          <span class="sidebar-icon">🌐</span>
          <span v-if="!sidebarCollapsed" class="sidebar-label">Ver site</span>
        </router-link>
        <button class="sidebar-item sidebar-logout" @click="handleLogout" id="btn-admin-logout" :title="sidebarCollapsed ? 'Sair' : ''">
          <span class="sidebar-icon">🚪</span>
          <span v-if="!sidebarCollapsed" class="sidebar-label">Sair</span>
        </button>
      </div>
    </aside>

    <!-- Main content -->
    <main class="admin-main">
      <!-- Top bar -->
      <header class="admin-topbar">
        <div class="topbar-left">
          <button class="mobile-sidebar-btn" @click="mobileSidebar = !mobileSidebar" id="btn-mobile-sidebar">☰</button>
          <div>
            <h1 class="topbar-title">{{ currentPageTitle }}</h1>
            <p class="topbar-breadcrumb">Admin · {{ currentPageTitle }}</p>
          </div>
        </div>
        <div class="topbar-right">
            <div class="admin-user">
            <div class="admin-avatar-initial">{{ authStore.currentUser?.nome?.charAt(0).toUpperCase() }}</div>
            <div class="admin-user-info">
              <p class="admin-user-name">{{ authStore.currentUser?.nome }}</p>
              <p class="admin-user-role">Administrador</p>
            </div>
          </div>
        </div>
      </header>

      <div class="admin-content">
        <router-view />
      </div>
    </main>

    <!-- Mobile sidebar overlay -->
    <div v-if="mobileSidebar" class="mobile-overlay" @click="mobileSidebar = false"></div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore.js'
import { useToast } from '../../composables/useToast.js'

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()
const { success } = useToast()

const sidebarCollapsed = ref(false)
const mobileSidebar = ref(false)

const navItems = [
  { to: '/admin/dashboard', label: 'Dashboard', icon: '📊', id: 'dashboard' },
  { to: '/admin/eventos', label: 'Eventos', icon: '🎭', id: 'eventos' },
  { to: '/admin/clientes', label: 'Clientes', icon: '👥', id: 'clientes' },
  { to: '/admin/cupons', label: 'Cupons', icon: '🎟️', id: 'cupons' }
]

const pageTitles = {
  '/admin/dashboard': 'Dashboard',
  '/admin/eventos': 'Gerenciar Eventos',
  '/admin/clientes': 'Gerenciar Clientes',
  '/admin/cupons': 'Cupons de Desconto'
}

const currentPageTitle = computed(() => pageTitles[route.path] || 'Admin')

function handleLogout() {
  authStore.logout()
  success('Você saiu do painel admin.')
  router.push('/')
}
</script>

<style scoped>
.admin-layout {
  display: flex;
  min-height: 100vh;
  background: var(--clr-bg);
}

/* Sidebar */
.admin-sidebar {
  width: 240px;
  background: var(--clr-bg-2);
  border-right: 1px solid var(--clr-border);
  display: flex;
  flex-direction: column;
  transition: width var(--transition-md);
  position: fixed;
  top: 0;
  left: 0;
  bottom: 0;
  z-index: 50;
  overflow: hidden;
}

.admin-sidebar.collapsed { width: 64px; }

.sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-lg);
  border-bottom: 1px solid var(--clr-border);
  min-height: 64px;
}

.sidebar-logo { display: flex; align-items: center; gap: 10px; overflow: hidden; }
.sidebar-logo-text {
  font-family: var(--font-display);
  font-size: 16px;
  font-weight: 800;
  background: var(--grad-text);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  white-space: nowrap;
}

.sidebar-toggle {
  background: var(--clr-surface);
  border: 1px solid var(--clr-border);
  color: var(--clr-text-muted);
  width: 28px;
  height: 28px;
  border-radius: var(--radius-sm);
  cursor: pointer;
  font-size: 12px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast);
}
.sidebar-toggle:hover { background: var(--clr-surface-hover); color: var(--clr-text); }

.sidebar-nav {
  flex: 1;
  padding: var(--space-md);
  display: flex;
  flex-direction: column;
  gap: 4px;
  overflow-y: auto;
}

.sidebar-item {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  padding: 11px var(--space-md);
  border-radius: var(--radius-md);
  color: var(--clr-text-muted);
  font-size: 14px;
  font-weight: 500;
  transition: all var(--transition-fast);
  white-space: nowrap;
  background: none;
  border: none;
  cursor: pointer;
  width: 100%;
  text-align: left;
  font-family: var(--font-body);
}

.sidebar-item:hover { background: var(--clr-surface-hover); color: var(--clr-text); }
.sidebar-item--active { background: rgba(124, 58, 237, 0.15); color: var(--clr-primary-light) !important; }

.sidebar-icon { font-size: 18px; flex-shrink: 0; width: 20px; text-align: center; }
.sidebar-label { overflow: hidden; text-overflow: ellipsis; }

.sidebar-footer {
  padding: var(--space-md);
  border-top: 1px solid var(--clr-border);
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.sidebar-logout { color: #FCA5A5 !important; }
.sidebar-logout:hover { background: rgba(239, 68, 68, 0.1) !important; }

/* Main */
.admin-main {
  flex: 1;
  margin-left: 240px;
  transition: margin-left var(--transition-md);
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.admin-sidebar.collapsed ~ .admin-main { margin-left: 64px; }

.admin-topbar {
  position: sticky;
  top: 0;
  background: rgba(13, 13, 18, 0.9);
  backdrop-filter: blur(20px);
  border-bottom: 1px solid var(--clr-border);
  padding: var(--space-md) var(--space-xl);
  display: flex;
  align-items: center;
  justify-content: space-between;
  z-index: 40;
  min-height: 64px;
}

.topbar-left { display: flex; align-items: center; gap: var(--space-md); }
.topbar-title { font-size: 20px; font-weight: 800; }
.topbar-breadcrumb { font-size: 12px; color: var(--clr-text-muted); }

.mobile-sidebar-btn { display: none; background: none; border: none; color: var(--clr-text); font-size: 20px; cursor: pointer; }

.admin-user { display: flex; align-items: center; gap: var(--space-md); }
.admin-avatar-initial {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 2px solid var(--clr-primary);
  background: var(--grad-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 15px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
}
.admin-user-name { font-size: 14px; font-weight: 600; }
.admin-user-role { font-size: 11px; color: var(--clr-primary-light); }

.admin-content { flex: 1; padding: var(--space-xl); }

.mobile-overlay {
  display: none;
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.5);
  z-index: 49;
}

@media (max-width: 768px) {
  .admin-sidebar { transform: translateX(-100%); }
  .admin-sidebar.mobile-open { transform: translateX(0); }
  .admin-main { margin-left: 0 !important; }
  .mobile-sidebar-btn { display: block; }
  .mobile-overlay { display: block; }
}
</style>
