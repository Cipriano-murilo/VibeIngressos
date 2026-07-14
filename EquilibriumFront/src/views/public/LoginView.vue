<template>
  <div class="auth-page">
    <div class="auth-bg">
      <div class="auth-orb auth-orb-1"></div>
      <div class="auth-orb auth-orb-2"></div>
    </div>

    <div class="auth-container">
      <!-- Logo -->
      <router-link to="/" class="auth-logo" id="auth-logo">
        <svg width="36" height="36" viewBox="0 0 32 32" fill="none">
          <circle cx="16" cy="16" r="16" fill="url(#grad-login)"/>
          <path d="M10 16 L16 10 L22 16 L16 22 Z" fill="white" opacity="0.9"/>
          <circle cx="16" cy="16" r="3" fill="white"/>
          <defs>
            <linearGradient id="grad-login" x1="0" y1="0" x2="32" y2="32">
              <stop offset="0%" stop-color="#7C3AED"/>
              <stop offset="100%" stop-color="#EC4899"/>
            </linearGradient>
          </defs>
        </svg>
        <span class="auth-logo-text">Equilibrium</span>
      </router-link>

      <div class="auth-card glass animate-fade-in">
        <h1 class="auth-title">Bem-vindo de volta!</h1>
        <p class="auth-subtitle">Entre na sua conta para continuar</p>

        <form @submit.prevent="handleLogin" class="auth-form" id="form-login" novalidate>
          <BaseInput
            id="input-email"
            v-model="form.email"
            label="E-mail"
            type="email"
            placeholder="seu@email.com"
            autocomplete="email"
            :error="errors.email"
          />

          <BaseInput
            id="input-senha"
            v-model="form.senha"
            label="Senha"
            type="password"
            placeholder="Sua senha"
            autocomplete="current-password"
            :error="errors.senha"
          />

          <div class="form-options">
            <router-link to="/esqueci-senha" class="link-forgot" id="link-esqueci-senha">
              Esqueci minha senha
            </router-link>
          </div>

          <BaseButton
            id="btn-entrar"
            type="submit"
            variant="primary"
            size="lg"
            :full="true"
            :loading="loading"
          >
            Entrar na conta
          </BaseButton>
        </form>

        <p class="auth-switch">
          Não tem uma conta?
          <router-link to="/cadastro" id="link-criar-conta" class="auth-link">Criar agora</router-link>
        </p>

        <!-- Demo credentials hint -->
        <div class="demo-hint">
          <p class="demo-title">💡 Credenciais de demonstração</p>
          <div class="demo-row">
            <span><strong>Cliente:</strong> joao@email.com / 123456</span>
          </div>
          <div class="demo-row">
            <span><strong>Admin:</strong> admin@equilibrium.com / admin123</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore.js'
import { useToast } from '../../composables/useToast.js'
import BaseInput from '../../components/ui/BaseInput.vue'
import BaseButton from '../../components/ui/BaseButton.vue'

const router = useRouter()
const authStore = useAuthStore()
const { success, error: toastError } = useToast()

const loading = ref(false)
const form = reactive({ email: '', senha: '' })
const errors = reactive({ email: '', senha: '' })

function validate() {
  errors.email = ''
  errors.senha = ''
  let valid = true

  if (!form.email) { errors.email = 'E-mail é obrigatório.'; valid = false }
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) { errors.email = 'E-mail inválido.'; valid = false }

  if (!form.senha) { errors.senha = 'Senha é obrigatória.'; valid = false }
  else if (form.senha.length < 6) { errors.senha = 'Senha deve ter pelo menos 6 caracteres.'; valid = false }

  return valid
}

async function handleLogin() {
  if (!validate()) return
  loading.value = true
  try {
    const user = await authStore.login(form.email, form.senha)
    success(`Bem-vindo, ${user.nome.split(' ')[0]}!`)
    router.push(authStore.isAdmin ? '/admin/dashboard' : '/')
  } catch (e) {
    toastError(e.message)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
  padding: var(--space-lg);
}

.auth-bg {
  position: absolute;
  inset: 0;
  background: var(--grad-hero);
}

.auth-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.3;
  animation: float 6s ease-in-out infinite;
}
.auth-orb-1 { width: 500px; height: 500px; background: #7C3AED; top: -150px; left: -150px; }
.auth-orb-2 { width: 350px; height: 350px; background: #EC4899; bottom: -100px; right: -100px; animation-delay: 3s; }

.auth-container {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 440px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-xl);
}

.auth-logo {
  display: flex;
  align-items: center;
  gap: 10px;
}
.auth-logo-text {
  font-family: var(--font-display);
  font-size: 22px;
  font-weight: 800;
  background: var(--grad-text);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.auth-card {
  width: 100%;
  border-radius: var(--radius-xl);
  padding: var(--space-2xl);
}

.auth-title {
  font-size: 26px;
  font-weight: 800;
  margin-bottom: 6px;
  text-align: center;
}
.auth-subtitle {
  font-size: 14px;
  color: var(--clr-text-muted);
  text-align: center;
  margin-bottom: var(--space-xl);
}

.auth-form { display: flex; flex-direction: column; gap: var(--space-md); }

.form-options {
  display: flex;
  justify-content: flex-end;
}

.link-forgot {
  font-size: 13px;
  color: var(--clr-primary-light);
  transition: color var(--transition-fast);
}
.link-forgot:hover { color: var(--clr-secondary-light); }

.auth-switch {
  text-align: center;
  font-size: 14px;
  color: var(--clr-text-muted);
  margin-top: var(--space-lg);
}

.auth-link {
  color: var(--clr-primary-light);
  font-weight: 600;
  margin-left: 4px;
  transition: color var(--transition-fast);
}
.auth-link:hover { color: var(--clr-secondary-light); }

.demo-hint {
  margin-top: var(--space-lg);
  padding: var(--space-md);
  background: rgba(124, 58, 237, 0.08);
  border: 1px solid rgba(124, 58, 237, 0.2);
  border-radius: var(--radius-md);
}
.demo-title { font-size: 12px; font-weight: 700; color: var(--clr-primary-light); margin-bottom: 8px; }
.demo-row { font-size: 12px; color: var(--clr-text-muted); line-height: 1.8; }
</style>
