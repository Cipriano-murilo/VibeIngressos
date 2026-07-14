<template>
  <div class="auth-page">
    <div class="auth-bg">
      <div class="auth-orb auth-orb-1"></div>
      <div class="auth-orb auth-orb-2"></div>
    </div>

    <div class="auth-container" style="max-width: 520px;">
      <router-link to="/" class="auth-logo" id="cadastro-logo">
        <svg width="36" height="36" viewBox="0 0 32 32" fill="none">
          <circle cx="16" cy="16" r="16" fill="url(#grad-cad)"/>
          <path d="M10 16 L16 10 L22 16 L16 22 Z" fill="white" opacity="0.9"/>
          <circle cx="16" cy="16" r="3" fill="white"/>
          <defs>
            <linearGradient id="grad-cad" x1="0" y1="0" x2="32" y2="32">
              <stop offset="0%" stop-color="#7C3AED"/>
              <stop offset="100%" stop-color="#EC4899"/>
            </linearGradient>
          </defs>
        </svg>
        <span class="auth-logo-text">Equilibrium</span>
      </router-link>

      <div class="auth-card glass animate-fade-in">
        <h1 class="auth-title">Criar sua conta</h1>
        <p class="auth-subtitle">Junte-se a milhares de pessoas nos melhores eventos</p>

        <form @submit.prevent="handleCadastro" id="form-cadastro" class="auth-form" novalidate>
          <div class="form-row">
            <BaseInput
              id="input-nome"
              v-model="form.nome"
              label="Nome completo"
              type="text"
              placeholder="João da Silva"
              autocomplete="name"
              :error="errors.nome"
            />
          </div>

          <div class="form-row-2">
            <BaseInput
              id="input-cpf"
              v-model="form.cpf"
              label="CPF"
              type="text"
              placeholder="000.000.000-00"
              maxlength="14"
              :error="errors.cpf"
              @input="maskCPF"
            />
            <BaseInput
              id="input-nascimento"
              v-model="form.dataNascimento"
              label="Data de nascimento"
              type="date"
              :error="errors.dataNascimento"
            />
          </div>

          <div class="form-row-2">
            <BaseInput
              id="input-email-cad"
              v-model="form.email"
              label="E-mail"
              type="email"
              placeholder="seu@email.com"
              autocomplete="email"
              :error="errors.email"
            />
            <BaseInput
              id="input-celular"
              v-model="form.celular"
              label="Celular"
              type="tel"
              placeholder="(11) 99999-9999"
              maxlength="15"
              :error="errors.celular"
              @input="maskPhone"
            />
          </div>

          <div class="form-row-2">
            <BaseInput
              id="input-senha-cad"
              v-model="form.senha"
              label="Senha"
              type="password"
              placeholder="Mínimo 6 caracteres"
              autocomplete="new-password"
              :error="errors.senha"
            />
            <BaseInput
              id="input-confirmar-senha"
              v-model="form.confirmarSenha"
              label="Confirmar senha"
              type="password"
              placeholder="Repita a senha"
              autocomplete="new-password"
              :error="errors.confirmarSenha"
            />
          </div>

          <!-- Password strength indicator -->
          <div v-if="form.senha" class="pw-strength">
            <div class="pw-strength-bar">
              <div class="pw-strength-fill" :class="passwordStrength.class" :style="{ width: passwordStrength.width }"></div>
            </div>
            <span class="pw-strength-label" :class="passwordStrength.class">{{ passwordStrength.label }}</span>
          </div>

          <BaseButton
            id="btn-cadastrar"
            type="submit"
            variant="primary"
            size="lg"
            :full="true"
            :loading="loading"
          >
            Criar conta grátis
          </BaseButton>
        </form>

        <p class="auth-switch">
          Já tem conta?
          <router-link to="/login" id="link-entrar-cad" class="auth-link">Entrar</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore.js'
import { useToast } from '../../composables/useToast.js'
import BaseInput from '../../components/ui/BaseInput.vue'
import BaseButton from '../../components/ui/BaseButton.vue'

const router = useRouter()
const authStore = useAuthStore()
const { success, error: toastError } = useToast()

const loading = ref(false)
const form = reactive({
  nome: '', cpf: '', dataNascimento: '', email: '',
  celular: '', senha: '', confirmarSenha: ''
})
const errors = reactive({
  nome: '', cpf: '', dataNascimento: '', email: '',
  celular: '', senha: '', confirmarSenha: ''
})

// Masks
function maskCPF(e) {
  let v = e.target.value.replace(/\D/g, '').slice(0, 11)
  v = v.replace(/(\d{3})(\d)/, '$1.$2')
  v = v.replace(/(\d{3})(\d)/, '$1.$2')
  v = v.replace(/(\d{3})(\d{1,2})$/, '$1-$2')
  form.cpf = v
}

function maskPhone(e) {
  let v = e.target.value.replace(/\D/g, '').slice(0, 11)
  v = v.replace(/(\d{2})(\d)/, '($1) $2')
  v = v.replace(/(\d{5})(\d)/, '$1-$2')
  form.celular = v
}

// Password strength
const passwordStrength = computed(() => {
  const pw = form.senha
  if (pw.length === 0) return { label: '', class: '', width: '0%' }
  let score = 0
  if (pw.length >= 8) score++
  if (/[A-Z]/.test(pw)) score++
  if (/[0-9]/.test(pw)) score++
  if (/[^A-Za-z0-9]/.test(pw)) score++

  const levels = [
    { label: 'Muito fraca', class: 'pw-very-weak', width: '20%' },
    { label: 'Fraca', class: 'pw-weak', width: '40%' },
    { label: 'Razoável', class: 'pw-fair', width: '60%' },
    { label: 'Forte', class: 'pw-strong', width: '80%' },
    { label: 'Muito forte', class: 'pw-very-strong', width: '100%' }
  ]
  return levels[score]
})

function validate() {
  Object.keys(errors).forEach(k => errors[k] = '')
  let valid = true

  if (!form.nome || form.nome.trim().split(' ').length < 2) {
    errors.nome = 'Informe o nome completo.'; valid = false
  }
  if (!form.cpf || form.cpf.replace(/\D/g, '').length < 11) {
    errors.cpf = 'CPF inválido.'; valid = false
  }
  if (!form.dataNascimento) { errors.dataNascimento = 'Informe a data de nascimento.'; valid = false }
  if (!form.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = 'E-mail inválido.'; valid = false
  }
  if (!form.celular || form.celular.replace(/\D/g, '').length < 11) {
    errors.celular = 'Celular inválido.'; valid = false
  }
  if (form.senha.length < 6) { errors.senha = 'Mínimo 6 caracteres.'; valid = false }
  if (form.senha !== form.confirmarSenha) {
    errors.confirmarSenha = 'As senhas não coincidem.'; valid = false
  }

  return valid
}

async function handleCadastro() {
  if (!validate()) return
  loading.value = true
  try {
    const { confirmarSenha: _, ...dados } = form
    await authStore.cadastrar(dados)
    success('Conta criada com sucesso! Bem-vindo ao Equilibrium!')
    router.push('/')
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
  padding: var(--space-xl) var(--space-lg);
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
.auth-orb-1 { width: 500px; height: 500px; background: #7C3AED; top: -150px; right: -100px; }
.auth-orb-2 { width: 350px; height: 350px; background: #EC4899; bottom: -100px; left: -100px; animation-delay: 3s; }

.auth-container {
  position: relative;
  z-index: 1;
  width: 100%;
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

.auth-title { font-size: 26px; font-weight: 800; margin-bottom: 6px; text-align: center; }
.auth-subtitle { font-size: 14px; color: var(--clr-text-muted); text-align: center; margin-bottom: var(--space-xl); }

.auth-form { display: flex; flex-direction: column; gap: var(--space-md); }

.form-row { display: flex; flex-direction: column; gap: var(--space-md); }

.form-row-2 {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-md);
}

/* Password strength */
.pw-strength { display: flex; align-items: center; gap: var(--space-md); }
.pw-strength-bar { flex: 1; height: 4px; background: var(--clr-surface-hover); border-radius: var(--radius-full); overflow: hidden; }
.pw-strength-fill { height: 100%; border-radius: var(--radius-full); transition: all var(--transition-md); }

.pw-strength-label { font-size: 12px; font-weight: 600; white-space: nowrap; }

.pw-very-weak { background: var(--clr-error); color: #FCA5A5; }
.pw-weak { background: #F97316; color: #FED7AA; }
.pw-fair { background: var(--clr-warning); color: #FCD34D; }
.pw-strong { background: #22C55E; color: #86EFAC; }
.pw-very-strong { background: var(--clr-success); color: #6EE7B7; }

.auth-switch { text-align: center; font-size: 14px; color: var(--clr-text-muted); margin-top: var(--space-lg); }
.auth-link { color: var(--clr-primary-light); font-weight: 600; margin-left: 4px; transition: color var(--transition-fast); }
.auth-link:hover { color: var(--clr-secondary-light); }

@media (max-width: 500px) {
  .form-row-2 { grid-template-columns: 1fr; }
  .auth-card { padding: var(--space-lg); }
}
</style>
