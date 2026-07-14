<template>
  <header class="app-header" :class="{ scrolled: isScrolled }">
    <div class="container header-inner">
      <!-- Logo -->
      <router-link to="/" class="logo" id="header-logo">
        <svg width="32" height="32" viewBox="0 0 32 32" fill="none">
          <circle cx="16" cy="16" r="16" fill="url(#grad)"/>
          <path d="M10 16 L16 10 L22 16 L16 22 Z" fill="white" opacity="0.9"/>
          <circle cx="16" cy="16" r="3" fill="white"/>
          <defs>
            <linearGradient id="grad" x1="0" y1="0" x2="32" y2="32">
              <stop offset="0%" stop-color="#7C3AED"/>
              <stop offset="100%" stop-color="#EC4899"/>
            </linearGradient>
          </defs>
        </svg>
        <span class="logo-text">Equilibrium</span>
      </router-link>

      <!-- Navigation (desktop) -->
      <nav class="header-nav" role="navigation" aria-label="Navegação principal">
        <router-link to="/" class="nav-link" exact-active-class="nav-link--active" id="nav-home">Eventos</router-link>
        <a href="#organizadores" class="nav-link" id="nav-organizers" @click.prevent="scrollTo('#organizadores')">Organizadores</a>
      </nav>

      <!-- Right actions -->
      <div class="header-actions">
        <template v-if="authStore.isAuthenticated">
          <router-link v-if="authStore.isAdmin" to="/admin/dashboard" class="nav-link" id="nav-admin">
            <span class="admin-badge">Admin</span>
          </router-link>
          <div class="user-menu" @click="userMenuOpen = !userMenuOpen" id="user-menu-toggle">
            <div class="user-avatar-initial">{{ firstName.charAt(0).toUpperCase() }}</div>
            <span class="user-name">{{ firstName }}</span>
            <span class="chevron" :class="{ open: userMenuOpen }">▾</span>

            <div v-if="userMenuOpen" class="user-dropdown" @click.stop>
              <p class="dropdown-user-name">{{ authStore.currentUser?.nome }}</p>
              <p class="dropdown-user-email">{{ authStore.currentUser?.email }}</p>
              <div class="dropdown-divider"></div>
              <button class="dropdown-item" @click="handleLogout" id="btn-logout">
                <span>🚪</span> Sair
              </button>
            </div>
          </div>
        </template>

        <template v-else>
          <router-link to="/login" class="nav-link" id="nav-login">Entrar</router-link>
          <router-link to="/cadastro" id="nav-cadastro">
            <button class="btn-header-cta">Criar conta</button>
          </router-link>
        </template>

        <!-- Mobile menu toggle -->
        <button class="mobile-menu-btn" @click="mobileMenuOpen = !mobileMenuOpen" aria-label="Menu" id="mobile-menu-btn">
          <span :class="{ open: mobileMenuOpen }"></span>
        </button>
      </div>
    </div>

    <!-- Mobile Menu -->
    <transition name="mobile-menu">
      <div v-if="mobileMenuOpen" class="mobile-menu" id="mobile-menu">
        <router-link to="/" class="mobile-nav-link" @click="mobileMenuOpen = false">🎭 Eventos</router-link>
        <template v-if="authStore.isAuthenticated">
          <router-link v-if="authStore.isAdmin" to="/admin/dashboard" class="mobile-nav-link" @click="mobileMenuOpen = false">⚡ Painel Admin</router-link>
          <button class="mobile-nav-link" @click="handleLogout" style="text-align:left;background:none;border:none;color:inherit;width:100%;cursor:pointer;">🚪 Sair</button>
        </template>
        <template v-else>
          <router-link to="/login" class="mobile-nav-link" @click="mobileMenuOpen = false">🔑 Entrar</router-link>
          <router-link to="/cadastro" class="mobile-nav-link" @click="mobileMenuOpen = false">✨ Criar conta</router-link>
        </template>
      </div>
    </transition>
  </header>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore.js'
import { useToast } from '../../composables/useToast.js'

const authStore = useAuthStore()
const router = useRouter()
const { success } = useToast()

const isScrolled = ref(false)
const userMenuOpen = ref(false)
const mobileMenuOpen = ref(false)

const firstName = computed(() => authStore.currentUser?.nome?.split(' ')[0] || '')

function handleScroll() { isScrolled.value = window.scrollY > 20 }
function handleClickOutside(e) {
  if (!e.target.closest('.user-menu')) userMenuOpen.value = false
  if (!e.target.closest('.mobile-menu') && !e.target.closest('.mobile-menu-btn')) mobileMenuOpen.value = false
}

function scrollTo(selector) {
  document.querySelector(selector)?.scrollIntoView({ behavior: 'smooth' })
}

function handleLogout() {
  authStore.logout()
  userMenuOpen.value = false
  mobileMenuOpen.value = false
  success('Você saiu da sua conta.')
  router.push('/')
}

onMounted(() => {
  window.addEventListener('scroll', handleScroll)
  document.addEventListener('click', handleClickOutside)
})
onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.app-header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 100;
  transition: all var(--transition-md);
  padding: 16px 0;
}

.app-header.scrolled {
  background: rgba(13, 13, 18, 0.85);
  backdrop-filter: blur(20px);
  border-bottom: 1px solid var(--clr-border);
  padding: 10px 0;
}

.header-inner {
  display: flex;
  align-items: center;
  gap: var(--space-xl);
}

/* Logo */
.logo {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-shrink: 0;
}
.logo-text {
  font-family: var(--font-display);
  font-size: 20px;
  font-weight: 800;
  background: var(--grad-text);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Nav */
.header-nav {
  display: flex;
  align-items: center;
  gap: var(--space-lg);
  flex: 1;
}

.nav-link {
  font-size: 14px;
  font-weight: 500;
  color: var(--clr-text-muted);
  transition: color var(--transition-fast);
  cursor: pointer;
}
.nav-link:hover, .nav-link--active { color: var(--clr-text); }

.header-actions {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  margin-left: auto;
}

.admin-badge {
  background: var(--grad-primary);
  color: white;
  font-size: 11px;
  font-weight: 700;
  padding: 3px 10px;
  border-radius: var(--radius-full);
  letter-spacing: 0.05em;
}

.btn-header-cta {
  background: var(--grad-primary);
  color: white;
  border: none;
  padding: 9px 20px;
  border-radius: var(--radius-md);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all var(--transition-md);
  box-shadow: 0 4px 12px rgba(124, 58, 237, 0.3);
}
.btn-header-cta:hover { box-shadow: 0 6px 20px rgba(124, 58, 237, 0.5); transform: translateY(-1px); }

/* User menu */
.user-menu {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  position: relative;
  padding: 6px 12px;
  border-radius: var(--radius-md);
  border: 1px solid var(--clr-border);
  transition: all var(--transition-fast);
}
.user-menu:hover { background: var(--clr-surface); border-color: var(--clr-primary); }

.user-avatar-initial {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: var(--grad-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
}
.user-name { font-size: 14px; font-weight: 500; }
.chevron { font-size: 11px; color: var(--clr-text-muted); transition: transform var(--transition-fast); }
.chevron.open { transform: rotate(180deg); }

.user-dropdown {
  position: absolute;
  top: calc(100% + 8px);
  right: 0;
  background: var(--clr-bg-3);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-md);
  padding: var(--space-md);
  min-width: 200px;
  box-shadow: var(--shadow-lg);
  animation: fadeIn 0.2s ease;
}

.dropdown-user-name { font-size: 14px; font-weight: 600; }
.dropdown-user-email { font-size: 12px; color: var(--clr-text-muted); margin-top: 2px; }
.dropdown-divider { height: 1px; background: var(--clr-border); margin: 10px 0; }
.dropdown-item {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
  padding: 8px 10px;
  border: none;
  background: none;
  color: var(--clr-text-muted);
  font-size: 14px;
  cursor: pointer;
  border-radius: var(--radius-sm);
  transition: all var(--transition-fast);
}
.dropdown-item:hover { background: var(--clr-surface); color: var(--clr-text); }

/* Mobile */
.mobile-menu-btn {
  display: none;
  flex-direction: column;
  gap: 5px;
  background: none;
  border: none;
  cursor: pointer;
  padding: 4px;
}
.mobile-menu-btn span::before,
.mobile-menu-btn span::after,
.mobile-menu-btn span {
  display: block;
  width: 22px;
  height: 2px;
  background: var(--clr-text);
  border-radius: 2px;
  transition: all var(--transition-md);
}

.mobile-menu {
  background: var(--clr-bg-3);
  border-top: 1px solid var(--clr-border);
  padding: var(--space-lg);
  display: flex;
  flex-direction: column;
  gap: var(--space-sm);
}

.mobile-nav-link {
  display: block;
  padding: 12px var(--space-md);
  border-radius: var(--radius-md);
  font-size: 15px;
  font-weight: 500;
  color: var(--clr-text-muted);
  transition: all var(--transition-fast);
}
.mobile-nav-link:hover { background: var(--clr-surface); color: var(--clr-text); }

.mobile-menu-enter-active, .mobile-menu-leave-active { transition: all 0.25s ease; }
.mobile-menu-enter-from, .mobile-menu-leave-to { opacity: 0; transform: translateY(-10px); }

@media (max-width: 768px) {
  .header-nav { display: none; }
  .header-actions > a, .header-actions > template > a:last-child { display: none; }
  .mobile-menu-btn { display: flex; }
  .btn-header-cta { display: none; }
}
</style>
