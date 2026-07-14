<template>
  <div class="auth-page">
    <div class="auth-bg">
      <div class="auth-orb auth-orb-1"></div>
      <div class="auth-orb auth-orb-2"></div>
    </div>

    <div class="auth-container">
      <router-link to="/" class="auth-logo" id="esqueci-logo">
        <svg width="36" height="36" viewBox="0 0 32 32" fill="none">
          <circle cx="16" cy="16" r="16" fill="url(#grad-esq)"/>
          <path d="M10 16 L16 10 L22 16 L16 22 Z" fill="white" opacity="0.9"/>
          <circle cx="16" cy="16" r="3" fill="white"/>
          <defs>
            <linearGradient id="grad-esq" x1="0" y1="0" x2="32" y2="32">
              <stop offset="0%" stop-color="#7C3AED"/>
              <stop offset="100%" stop-color="#EC4899"/>
            </linearGradient>
          </defs>
        </svg>
        <span class="auth-logo-text">Equilibrium</span>
      </router-link>

      <div class="auth-card glass animate-fade-in">
        <!-- Success State -->
        <div v-if="sent" class="success-state">
          <div class="success-icon">📬</div>
          <h2 class="success-title">E-mail enviado!</h2>
          <p class="success-text">
            Enviamos um link de recuperação para <strong>{{ form.email }}</strong>.
            Verifique sua caixa de entrada e spam.
          </p>
          <router-link to="/login" id="btn-voltar-login">
            <BaseButton variant="primary" :full="true">Voltar para o login</BaseButton>
          </router-link>
        </div>

        <!-- Form State -->
        <template v-else>
          <div class="back-link">
            <router-link to="/login" id="link-voltar" class="back-arrow">← Voltar ao login</router-link>
          </div>

          <div class="lock-icon">🔐</div>
          <h1 class="auth-title">Esqueceu sua senha?</h1>
          <p class="auth-subtitle">
            Informe seu e-mail e enviaremos um link para redefinir sua senha.
          </p>

          <form @submit.prevent="handleRecover" id="form-esqueci-senha" class="auth-form" novalidate>
            <BaseInput
              id="input-email-recover"
              v-model="form.email"
              label="E-mail cadastrado"
              type="email"
              placeholder="seu@email.com"
              autocomplete="email"
              :error="errors.email"
            />

            <BaseButton
              id="btn-recuperar"
              type="submit"
              variant="primary"
              size="lg"
              :full="true"
              :loading="loading"
            >
              Enviar link de recuperação
            </BaseButton>
          </form>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useAuthStore } from '../../stores/authStore.js'
import { useToast } from '../../composables/useToast.js'
import BaseInput from '../../components/ui/BaseInput.vue'
import BaseButton from '../../components/ui/BaseButton.vue'

const authStore = useAuthStore()
const { error: toastError } = useToast()

const loading = ref(false)
const sent = ref(false)
const form = reactive({ email: '' })
const errors = reactive({ email: '' })

async function handleRecover() {
  errors.email = ''
  if (!form.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = 'Informe um e-mail válido.'; return
  }
  loading.value = true
  try {
    await authStore.recuperarSenha(form.email)
    sent.value = true
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

.auth-bg { position: absolute; inset: 0; background: var(--grad-hero); }

.auth-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.3;
  animation: float 6s ease-in-out infinite;
}
.auth-orb-1 { width: 400px; height: 400px; background: #7C3AED; top: -100px; left: -100px; }
.auth-orb-2 { width: 300px; height: 300px; background: #EC4899; bottom: -80px; right: -80px; animation-delay: 3s; }

.auth-container {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 420px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-xl);
}

.auth-logo { display: flex; align-items: center; gap: 10px; }
.auth-logo-text {
  font-family: var(--font-display);
  font-size: 22px;
  font-weight: 800;
  background: var(--grad-text);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.auth-card { width: 100%; border-radius: var(--radius-xl); padding: var(--space-2xl); }

.back-link { margin-bottom: var(--space-md); }
.back-arrow { font-size: 13px; color: var(--clr-text-muted); transition: color var(--transition-fast); }
.back-arrow:hover { color: var(--clr-primary-light); }

.lock-icon { font-size: 40px; margin-bottom: var(--space-md); }
.auth-title { font-size: 24px; font-weight: 800; margin-bottom: 8px; }
.auth-subtitle { font-size: 14px; color: var(--clr-text-muted); margin-bottom: var(--space-xl); line-height: 1.6; }

.auth-form { display: flex; flex-direction: column; gap: var(--space-md); }

/* Success state */
.success-state { display: flex; flex-direction: column; align-items: center; text-align: center; gap: var(--space-lg); }
.success-icon { font-size: 56px; }
.success-title { font-size: 24px; font-weight: 800; }
.success-text { font-size: 15px; color: var(--clr-text-muted); line-height: 1.6; }
.success-text strong { color: var(--clr-text); }
</style>
