<template>
  <div class="evento-page">
    <AppHeader />

    <div v-if="!evento" class="not-found">
      <div class="container">
        <h2>Evento não encontrado</h2>
        <router-link to="/">← Voltar para eventos</router-link>
      </div>
    </div>

    <template v-else>
      <!-- Hero Banner -->
      <div class="evento-hero">
        <img :src="evento.imagem" :alt="evento.nome" class="hero-img" />
        <div class="hero-overlay"></div>
        <div class="container hero-content">
          <span class="evento-category">{{ evento.categoria }}</span>
          <h1 class="evento-title">{{ evento.nome }}</h1>
          <div class="evento-meta">
            <span class="meta-chip">📅 {{ formatDate(evento.data) }} · {{ formatHora(evento.data) }}</span>
            <span class="meta-chip">📍 {{ evento.local }}</span>
          </div>
        </div>
      </div>

      <!-- Main content -->
      <div class="container evento-body">
        <div class="evento-main">
          <!-- About -->
          <section class="card-section" aria-labelledby="sobre-titulo">
            <h2 id="sobre-titulo" class="section-label">Sobre o evento</h2>
            <p class="evento-desc">{{ evento.descricao }}</p>
          </section>

          <!-- Info cards -->
          <section class="info-cards">
            <div class="info-card">
              <span class="info-icon">📅</span>
              <div>
                <p class="info-label">Data e hora</p>
                <p class="info-value">{{ formatDate(evento.data) }} às {{ formatHora(evento.data) }}</p>
              </div>
            </div>
            <div class="info-card">
              <span class="info-icon">📍</span>
              <div>
                <p class="info-label">Local</p>
                <p class="info-value">{{ evento.local }}</p>
              </div>
            </div>
            <div class="info-card">
              <span class="info-icon">🎫</span>
              <div>
                <p class="info-label">Categorias de ingresso</p>
                <p class="info-value">{{ evento.tipos.length }} opção(ões)</p>
              </div>
            </div>
          </section>
        </div>

        <!-- Checkout sidebar -->
        <aside class="checkout-sidebar">
          <div class="checkout-card glass">
            <h2 class="checkout-title">Ingressos</h2>

            <!-- Ticket types -->
            <div class="ticket-types">
              <div
                v-for="tipo in evento.tipos"
                :key="tipo.id"
                class="ticket-type"
                :class="{ selected: selectedTipo?.id === tipo.id, esgotado: tipo.disponivel === 0 }"
                @click="tipo.disponivel > 0 && selectTipo(tipo)"
                :id="`ticket-${tipo.id}`"
                role="button"
                :aria-disabled="tipo.disponivel === 0"
              >
                <div class="ticket-info">
                  <p class="ticket-nome">{{ tipo.nome }}</p>
                  <p class="ticket-disponivel">{{ tipo.disponivel > 0 ? `${tipo.disponivel} disponíveis` : 'Esgotado' }}</p>
                </div>
                <div class="ticket-preco">
                  {{ tipo.preco.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }}
                </div>
              </div>
            </div>

            <!-- Quantity selector -->
            <div v-if="selectedTipo" class="qty-section">
              <p class="qty-label">Quantidade</p>
              <div class="qty-controls">
                <button class="qty-btn" id="btn-qty-minus" @click="qty > 1 && qty--" :disabled="qty <= 1">−</button>
                <span class="qty-value" id="qty-display">{{ qty }}</span>
                <button class="qty-btn" id="btn-qty-plus" @click="qty < 10 && qty++" :disabled="qty >= 10">+</button>
              </div>
            </div>

            <!-- Cupom -->
            <div class="cupom-section" v-if="selectedTipo">
              <p class="qty-label">Cupom de desconto</p>
              <div class="cupom-input-row">
                <input
                  id="input-cupom"
                  v-model="cupomCode"
                  type="text"
                  placeholder="CÓDIGO DO CUPOM"
                  class="cupom-input"
                  :disabled="cupomAplicado"
                />
                <button
                  id="btn-aplicar-cupom"
                  class="cupom-btn"
                  @click="aplicarCupom"
                  :disabled="cupomAplicado || !cupomCode"
                >
                  {{ cupomAplicado ? '✓' : 'Aplicar' }}
                </button>
              </div>
              <p v-if="cupomError" class="cupom-error">{{ cupomError }}</p>
              <p v-if="cupomAplicado" class="cupom-success">
                🎉 Desconto de {{ cupomDesconto.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }} aplicado!
              </p>
            </div>

            <!-- Summary -->
            <div v-if="selectedTipo" class="checkout-summary">
              <div class="summary-row">
                <span>{{ selectedTipo.nome }} × {{ qty }}</span>
                <span>{{ subtotal.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }}</span>
              </div>
              <div v-if="cupomAplicado" class="summary-row discount-row">
                <span>Desconto</span>
                <span>−{{ cupomDesconto.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }}</span>
              </div>
              <div class="summary-row taxa-row">
                <span>Taxa de serviço</span>
                <span>{{ taxa.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }}</span>
              </div>
              <div class="summary-total">
                <span>Total</span>
                <span class="total-value">{{ total.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }}</span>
              </div>
            </div>

            <button
              id="btn-comprar"
              class="btn-comprar"
              :disabled="!selectedTipo"
              @click="handleCompra"
            >
              {{ selectedTipo ? 'Finalizar compra' : 'Selecione um ingresso' }}
            </button>

            <p class="checkout-security">🔒 Compra 100% segura</p>
          </div>
        </aside>
      </div>
    </template>

    <AppFooter />

    <!-- Modal de Pagamento -->
    <PagamentoModal
      v-model="pagamentoOpen"
      :eventoId="evento?.id || ''"
      :eventoNome="evento?.nome || ''"
      :tipoIngressoId="selectedTipo?.id || ''"
      :tipoNome="selectedTipo?.nome || ''"
      :cupomCode="cupomAplicado ? cupomCode : ''"
      :qty="qty"
      :subtotal="subtotal"
      :desconto="cupomDesconto"
      :taxa="taxa"
      :total="total"
      @pagamento-concluido="onPagamentoConcluido"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppHeader from '../../components/layout/AppHeader.vue'
import AppFooter from '../../components/layout/AppFooter.vue'
import PagamentoModal from '../../components/ui/PagamentoModal.vue'
import { useEventosStore } from '../../stores/eventosStore.js'
import { useCuponsStore } from '../../stores/cuponsStore.js'
import { useAuthStore } from '../../stores/authStore.js'
import { useToast } from '../../composables/useToast.js'

const route = useRoute()
const router = useRouter()
const eventosStore = useEventosStore()
const cuponsStore = useCuponsStore()
const authStore = useAuthStore()
const { success, error: toastError, info } = useToast()

const evento = ref(null)

onMounted(async () => {
  const id = route.params.id
  // Tenta no store local primeiro
  let ev = eventosStore.getEvento(id)
  if (!ev) {
    // Se não tiver no store, carrega todos os eventos
    await eventosStore.carregar()
    ev = eventosStore.getEvento(id)
  }
  evento.value = ev || null
})
const selectedTipo = ref(null)
const qty = ref(1)
const cupomCode = ref('')
const cupomAplicado = ref(false)
const cupomDesconto = ref(0)
const cupomError = ref('')

function selectTipo(tipo) {
  selectedTipo.value = tipo
  qty.value = 1
  cupomAplicado.value = false
  cupomDesconto.value = 0
  cupomCode.value = ''
  cupomError.value = ''
}

const subtotal = computed(() => (selectedTipo.value?.preco || 0) * qty.value)
const taxa = computed(() => subtotal.value * 0.05)
const total = computed(() => subtotal.value - cupomDesconto.value + taxa.value)

async function aplicarCupom() {
  cupomError.value = ''
  try {
    const result = await cuponsStore.validarCupom(cupomCode.value, subtotal.value)
    if (result?.valido) {
      cupomDesconto.value = result.descontoCalculado || 0
      cupomAplicado.value = true
    } else {
      cupomError.value = result?.mensagem || 'Cupom inválido.'
    }
  } catch (e) {
    cupomError.value = e.message || 'Erro ao validar cupom.'
  }
}

const pagamentoOpen = ref(false)

function handleCompra() {
  if (!authStore.isAuthenticated) {
    info('Faça login para comprar ingressos.')
    router.push('/login')
    return
  }
  pagamentoOpen.value = true
}

function onPagamentoConcluido() {
  // Aqui pode registrar o pedido na API futuramente
  success(`🎉 Compra realizada! ${qty.value}x ${selectedTipo.value?.nome} para ${evento.value?.nome}`)
  // Reset do checkout
  selectedTipo.value = null
  qty.value = 1
  cupomAplicado.value = false
  cupomDesconto.value = 0
  cupomCode.value = ''
}

function formatDate(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  return d.toLocaleDateString('pt-BR', { weekday: 'long', day: '2-digit', month: 'long', year: 'numeric' })
}

function formatHora(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  return d.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
}
</script>

<style scoped>
.evento-page { min-height: 100vh; }

.not-found { padding: 120px var(--space-lg); text-align: center; }
.not-found a { color: var(--clr-primary-light); margin-top: var(--space-md); display: block; }

/* Hero */
.evento-hero {
  position: relative;
  height: 420px;
  margin-top: 0;
  overflow: hidden;
}
.hero-img { width: 100%; height: 100%; object-fit: cover; }
.hero-overlay { position: absolute; inset: 0; background: linear-gradient(to top, rgba(13,13,18,1) 0%, rgba(13,13,18,0.5) 50%, transparent 100%); }
.hero-content { position: absolute; bottom: 0; left: 50%; transform: translateX(-50%); padding-bottom: var(--space-2xl); }
.evento-category {
  display: inline-block;
  background: rgba(124, 58, 237, 0.25);
  border: 1px solid rgba(124, 58, 237, 0.4);
  color: #C4B5FD;
  padding: 5px 14px;
  border-radius: var(--radius-full);
  font-size: 12px;
  font-weight: 600;
  margin-bottom: var(--space-md);
}
.evento-title { font-size: clamp(28px, 4.5vw, 52px); font-weight: 900; margin-bottom: var(--space-md); line-height: 1.1; }
.evento-meta { display: flex; gap: var(--space-md); flex-wrap: wrap; }
.meta-chip {
  background: rgba(255,255,255,0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255,255,255,0.15);
  padding: 6px 14px;
  border-radius: var(--radius-full);
  font-size: 13px;
}

/* Body */
.evento-body {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: var(--space-2xl);
  padding-top: var(--space-2xl);
  padding-bottom: var(--space-3xl);
}

.card-section { margin-bottom: var(--space-2xl); }
.section-label { font-size: 18px; font-weight: 700; margin-bottom: var(--space-md); color: var(--clr-primary-light); }
.evento-desc { font-size: 16px; color: var(--clr-text-muted); line-height: 1.8; }

.info-cards { display: flex; flex-direction: column; gap: var(--space-md); }
.info-card {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  padding: var(--space-md);
  background: var(--clr-surface);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-md);
}
.info-icon { font-size: 24px; }
.info-label { font-size: 12px; color: var(--clr-text-muted); text-transform: uppercase; letter-spacing: 0.06em; }
.info-value { font-size: 15px; font-weight: 600; margin-top: 2px; }

/* Checkout Sidebar */
.checkout-sidebar { position: sticky; top: 90px; height: fit-content; }
.checkout-card { border-radius: var(--radius-xl); padding: var(--space-xl); }
.checkout-title { font-size: 20px; font-weight: 800; margin-bottom: var(--space-lg); }

.ticket-types { display: flex; flex-direction: column; gap: var(--space-sm); margin-bottom: var(--space-lg); }

.ticket-type {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-md);
  background: var(--clr-surface);
  border: 1.5px solid var(--clr-border);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all var(--transition-md);
}
.ticket-type:hover:not(.esgotado) { border-color: var(--clr-primary); background: rgba(124, 58, 237, 0.08); }
.ticket-type.selected { border-color: var(--clr-primary); background: rgba(124, 58, 237, 0.12); box-shadow: 0 0 0 2px rgba(124, 58, 237, 0.25); }
.ticket-type.esgotado { opacity: 0.4; cursor: not-allowed; }

.ticket-nome { font-size: 15px; font-weight: 600; }
.ticket-disponivel { font-size: 12px; color: var(--clr-text-muted); margin-top: 2px; }
.ticket-preco { font-size: 16px; font-weight: 800; background: var(--grad-text); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }

/* Quantity */
.qty-section { margin-bottom: var(--space-lg); }
.qty-label { font-size: 13px; font-weight: 600; color: var(--clr-text-muted); margin-bottom: 8px; text-transform: uppercase; letter-spacing: 0.06em; }
.qty-controls { display: flex; align-items: center; gap: var(--space-md); }
.qty-btn {
  width: 36px;
  height: 36px;
  border-radius: var(--radius-md);
  background: var(--clr-surface-hover);
  border: 1px solid var(--clr-border);
  color: var(--clr-text);
  font-size: 18px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast);
}
.qty-btn:hover:not(:disabled) { background: rgba(124, 58, 237, 0.15); border-color: var(--clr-primary); }
.qty-btn:disabled { opacity: 0.4; cursor: not-allowed; }
.qty-value { font-size: 20px; font-weight: 800; min-width: 30px; text-align: center; }

/* Cupom */
.cupom-section { margin-bottom: var(--space-lg); }
.cupom-input-row { display: flex; gap: 8px; }
.cupom-input {
  flex: 1;
  padding: 11px 14px;
  background: var(--clr-surface);
  border: 1.5px solid var(--clr-border);
  border-radius: var(--radius-md);
  color: var(--clr-text);
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  transition: all var(--transition-md);
}
.cupom-input:focus { border-color: var(--clr-primary); outline: none; }
.cupom-input:disabled { opacity: 0.6; }

.cupom-btn {
  padding: 11px 16px;
  background: var(--grad-primary);
  color: white;
  border: none;
  border-radius: var(--radius-md);
  font-size: 13px;
  font-weight: 700;
  cursor: pointer;
  transition: all var(--transition-md);
  white-space: nowrap;
}
.cupom-btn:disabled { opacity: 0.5; cursor: not-allowed; }

.cupom-error { font-size: 12px; color: #FCA5A5; margin-top: 6px; }
.cupom-success { font-size: 12px; color: #6EE7B7; margin-top: 6px; }

/* Summary */
.checkout-summary {
  background: var(--clr-bg-2);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-md);
  padding: var(--space-md);
  margin-bottom: var(--space-lg);
}
.summary-row { display: flex; justify-content: space-between; font-size: 14px; padding: 4px 0; color: var(--clr-text-muted); }
.discount-row { color: #6EE7B7; }
.taxa-row { color: var(--clr-text-subtle); font-size: 13px; }
.summary-total {
  display: flex;
  justify-content: space-between;
  font-size: 17px;
  font-weight: 800;
  margin-top: var(--space-sm);
  padding-top: var(--space-sm);
  border-top: 1px solid var(--clr-border);
}
.total-value { background: var(--grad-text); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }

.btn-comprar {
  width: 100%;
  padding: 16px;
  background: var(--grad-primary);
  color: white;
  border: none;
  border-radius: var(--radius-lg);
  font-size: 16px;
  font-weight: 800;
  cursor: pointer;
  transition: all var(--transition-md);
  box-shadow: 0 6px 24px rgba(124, 58, 237, 0.4);
  margin-bottom: var(--space-md);
}
.btn-comprar:hover:not(:disabled) { box-shadow: 0 10px 36px rgba(124, 58, 237, 0.6); transform: translateY(-2px); }
.btn-comprar:disabled { background: var(--clr-surface-hover); color: var(--clr-text-muted); box-shadow: none; cursor: not-allowed; }

.checkout-security { text-align: center; font-size: 12px; color: var(--clr-text-subtle); }

@media (max-width: 900px) {
  .evento-body { grid-template-columns: 1fr; }
  .checkout-sidebar { position: static; }
}
</style>