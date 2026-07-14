<template>
  <teleport to="body">
    <transition name="modal">
      <div v-if="modelValue" class="pag-overlay" @click.self="fechar" role="dialog" aria-modal="true" aria-label="Pagamento">

        <!-- Tela de Sucesso -->
        <div v-if="step === 'sucesso'" class="pag-panel pag-sucesso-panel">
          <div class="sucesso-content">
            <div class="sucesso-icon-wrap">
              <div class="sucesso-ring sucesso-ring-1"></div>
              <div class="sucesso-ring sucesso-ring-2"></div>
              <span class="sucesso-icon">✓</span>
            </div>
            <h2 class="sucesso-titulo">Pagamento confirmado!</h2>
            <p class="sucesso-sub">Seus ingressos foram gerados com sucesso.</p>

            <div class="sucesso-detalhe">
              <div class="sucesso-row">
                <span>Evento</span>
                <strong>{{ eventoNome }}</strong>
              </div>
              <div class="sucesso-row">
                <span>Ingresso</span>
                <strong>{{ qty }}x {{ tipoNome }}</strong>
              </div>
              <div class="sucesso-row">
                <span>Total pago</span>
                <strong class="sucesso-valor">{{ fmtBRL(total) }}</strong>
              </div>
              <div class="sucesso-row">
                <span>Método</span>
                <strong>{{ metodoPago === 'pix' ? '🟢 PIX' : '💳 Cartão de Crédito' }}</strong>
              </div>
            </div>

            <p class="sucesso-email">📧 Um e-mail de confirmação será enviado em breve.</p>

            <button class="btn-sucesso" @click="fechar" id="btn-fechar-sucesso">
              Fechar
            </button>
          </div>
        </div>

        <!-- Modal Principal -->
        <div v-else class="pag-panel">
          <!-- Header -->
          <div class="pag-header">
            <div class="pag-header-left">
              <span class="pag-lock">🔒</span>
              <div>
                <p class="pag-header-title">Pagamento seguro</p>
                <p class="pag-header-sub">Dados protegidos com criptografia SSL</p>
              </div>
            </div>
            <button class="pag-close" @click="fechar" id="btn-fechar-pagamento" aria-label="Fechar">✕</button>
          </div>

          <!-- Resumo do pedido -->
          <div class="pag-resumo">
            <div class="pag-resumo-info">
              <p class="pag-resumo-label">{{ qty }}x {{ tipoNome }}</p>
              <p class="pag-resumo-evento">{{ eventoNome }}</p>
            </div>
            <div class="pag-resumo-valor">
              <p class="pag-resumo-total">{{ fmtBRL(total) }}</p>
              <p v-if="desconto > 0" class="pag-resumo-desconto">Desconto: -{{ fmtBRL(desconto) }}</p>
            </div>
          </div>

          <!-- Seleção do método -->
          <div class="metodo-section" v-if="step === 'metodo' || step === 'pix' || step === 'cartao'">
            <p class="metodo-titulo">Como você quer pagar?</p>
            <div class="metodo-opcoes">
              <button
                id="btn-metodo-pix"
                class="metodo-btn"
                :class="{ active: metodo === 'pix' }"
                @click="selecionarMetodo('pix')"
              >
                <div class="metodo-icon pix-icon">
                  <svg viewBox="0 0 24 24" fill="currentColor" width="28" height="28">
                    <path d="M11.9 2C9.5 2 7.3 3 5.7 4.7L4.1 6.3C2.7 7.6 2 9.3 2 11.1v1.8c0 1.8.7 3.5 2.1 4.8L5.7 19.3C7.3 21 9.5 22 11.9 22c2.4 0 4.6-1 6.2-2.7l1.6-1.6c1.4-1.3 2.1-3 2.1-4.8v-1.8c0-1.8-.7-3.5-2.1-4.8L18.1 4.7C16.5 3 14.3 2 11.9 2zm-1.2 6.3c.3-.3.7-.3 1 0l2.1 2.1c.2.2.5.3.7.3.3 0 .5-.1.7-.3l2.1-2.1c.3-.3.7-.3 1 0 .3.3.3.7 0 1L16.2 11.4c-.6.6-1.4.9-2.1.9-.8 0-1.6-.3-2.1-.9L9.7 9.3c-.3-.3-.3-.7 0-1zm5.5 5.4c.8 0 1.6.3 2.1.9l2.1 2.1c.3.3.3.7 0 1-.3.3-.7.3-1 0l-2.1-2.1c-.2-.2-.5-.3-.7-.3-.3 0-.5.1-.7.3l-2.1 2.1c-.3.3-.7.3-1 0-.3-.3-.3-.7 0-1l2.1-2.1c.5-.6 1.3-.9 2.3-.9z"/>
                  </svg>
                </div>
                <div class="metodo-info">
                  <span class="metodo-nome">PIX</span>
                  <span class="metodo-desc">Aprovação imediata</span>
                </div>
                <div class="metodo-badge pix-badge">Instantâneo</div>
              </button>

              <button
                id="btn-metodo-cartao"
                class="metodo-btn"
                :class="{ active: metodo === 'cartao' }"
                @click="selecionarMetodo('cartao')"
              >
                <div class="metodo-icon card-icon">💳</div>
                <div class="metodo-info">
                  <span class="metodo-nome">Cartão de Crédito</span>
                  <span class="metodo-desc">Visa, Master, Elo, Amex</span>
                </div>
                <div class="metodo-badge card-badge">Até 12x</div>
              </button>
            </div>
          </div>

          <!-- ===== TELA PIX ===== -->
          <transition name="slide">
            <div v-if="step === 'pix'" class="pix-section">
              <div class="pix-card">
                <div class="pix-qr-wrapper">
                  <!-- QR Code simulado com padrão visual -->
                  <div class="pix-qr" id="pix-qr-code">
                    <div class="qr-inner">
                      <div v-for="r in 9" :key="r" class="qr-row">
                        <div v-for="c in 9" :key="c" class="qr-cell" :class="qrPattern(r, c)"></div>
                      </div>
                    </div>
                    <div class="qr-logo">⚡</div>
                  </div>
                  <div class="pix-timer">
                    <div class="timer-ring">
                      <svg viewBox="0 0 36 36" class="timer-svg">
                        <circle cx="18" cy="18" r="15.9" class="timer-bg"/>
                        <circle cx="18" cy="18" r="15.9" class="timer-fill" :style="{ strokeDashoffset: timerOffset }"/>
                      </svg>
                      <span class="timer-num">{{ timerDisplay }}</span>
                    </div>
                    <p class="timer-label">segundos restantes</p>
                  </div>
                </div>

                <div class="pix-instrucoes">
                  <p class="pix-step"><span class="pix-num">1</span> Abra seu app de banco</p>
                  <p class="pix-step"><span class="pix-num">2</span> Escaneie o QR Code acima</p>
                  <p class="pix-step"><span class="pix-num">3</span> Confirme o pagamento de <strong>{{ fmtBRL(total) }}</strong></p>
                </div>

                <div class="pix-copy-section">
                  <p class="pix-copy-label">Ou copie o código PIX:</p>
                  <div class="pix-copy-row">
                    <input id="pix-code-input" class="pix-code" :value="pixCode" readonly />
                    <button id="btn-copiar-pix" class="btn-copiar" @click="copiarPix">
                      {{ copiado ? '✓ Copiado!' : 'Copiar' }}
                    </button>
                  </div>
                </div>

                <button id="btn-simular-pix" class="btn-confirmar-pix" @click="confirmarPagamento('pix')" :class="{ loading: processando }">
                  <span v-if="!processando">✓ Confirmar pagamento</span>
                  <span v-else class="spinner"></span>
                </button>
              </div>
            </div>
          </transition>

          <!-- ===== TELA CARTÃO ===== -->
          <transition name="slide">
            <div v-if="step === 'cartao'" class="cartao-section">
              <!-- Card preview com flip -->
              <div class="card-preview-wrapper" :class="{ flipped: cardFlipped }">
                <div class="card-face card-front">
                  <div class="card-chip">
                    <div class="chip-lines">
                      <div v-for="i in 3" :key="i" class="chip-line"></div>
                    </div>
                  </div>
                  <div class="card-number-display">
                    {{ cartao.numero ? formatCardNum(cartao.numero) : '•••• •••• •••• ••••' }}
                  </div>
                  <div class="card-bottom">
                    <div>
                      <p class="card-field-label">TITULAR</p>
                      <p class="card-field-value">{{ cartao.nome || 'NOME DO TITULAR' }}</p>
                    </div>
                    <div>
                      <p class="card-field-label">VALIDADE</p>
                      <p class="card-field-value">{{ cartao.validade || 'MM/AA' }}</p>
                    </div>
                    <div class="card-brand">{{ detectBrand(cartao.numero) }}</div>
                  </div>
                </div>
                <div class="card-face card-back">
                  <div class="card-stripe"></div>
                  <div class="card-cvv-row">
                    <div class="cvv-label">CVV</div>
                    <div class="cvv-box">{{ cartao.cvv || '•••' }}</div>
                  </div>
                </div>
              </div>

              <!-- Formulário do cartão -->
              <form @submit.prevent="confirmarPagamento('cartao')" class="cartao-form" id="form-cartao">
                <div class="cf-field">
                  <label class="cf-label" for="cartao-numero">Número do cartão</label>
                  <input
                    id="cartao-numero"
                    v-model="cartao.numero"
                    class="cf-input"
                    type="text"
                    placeholder="0000 0000 0000 0000"
                    maxlength="19"
                    @input="formatarNumero"
                    @focus="cardFlipped = false"
                    autocomplete="cc-number"
                  />
                </div>

                <div class="cf-field">
                  <label class="cf-label" for="cartao-nome">Nome do titular</label>
                  <input
                    id="cartao-nome"
                    v-model="cartao.nome"
                    class="cf-input"
                    type="text"
                    placeholder="Como aparece no cartão"
                    @input="cartao.nome = cartao.nome.toUpperCase()"
                    @focus="cardFlipped = false"
                    autocomplete="cc-name"
                  />
                </div>

                <div class="cf-row">
                  <div class="cf-field">
                    <label class="cf-label" for="cartao-validade">Validade</label>
                    <input
                      id="cartao-validade"
                      v-model="cartao.validade"
                      class="cf-input"
                      type="text"
                      placeholder="MM/AA"
                      maxlength="5"
                      @input="formatarValidade"
                      @focus="cardFlipped = false"
                      autocomplete="cc-exp"
                    />
                  </div>
                  <div class="cf-field">
                    <label class="cf-label" for="cartao-cvv">CVV</label>
                    <input
                      id="cartao-cvv"
                      v-model="cartao.cvv"
                      class="cf-input"
                      type="text"
                      placeholder="•••"
                      maxlength="4"
                      @focus="cardFlipped = true"
                      @blur="cardFlipped = false"
                      autocomplete="cc-csc"
                    />
                  </div>
                </div>

                <div class="cf-field">
                  <label class="cf-label" for="cartao-parcelas">Parcelamento</label>
                  <select id="cartao-parcelas" v-model="cartao.parcelas" class="cf-input cf-select">
                    <option v-for="p in parcelasOptions" :key="p.n" :value="p.n">
                      {{ p.label }}
                    </option>
                  </select>
                </div>

                <p v-if="cartaoErro" class="cartao-erro">{{ cartaoErro }}</p>

                <button type="submit" id="btn-pagar-cartao" class="btn-pagar" :class="{ loading: processando }">
                  <span v-if="!processando">🔒 Pagar {{ fmtBRL(total) }}</span>
                  <span v-else class="spinner"></span>
                </button>
              </form>
            </div>
          </transition>

          <!-- Rodapé de segurança -->
          <div class="pag-footer">
            <span>🔒 SSL</span>
            <span>🛡️ Dados criptografados</span>
            <span>✅ Pagamento seguro</span>
          </div>
        </div>

      </div>
    </transition>
  </teleport>
</template>

<script setup>
import { ref, computed, watch, onUnmounted } from 'vue'
import { api } from '../../services/api.js'
import { useAuthStore } from '../../stores/authStore.js'

const authStore = useAuthStore()

const props = defineProps({
  modelValue: Boolean,
  eventoId: { type: String, default: '' },
  eventoNome: { type: String, default: '' },
  tipoIngressoId: { type: String, default: '' },
  tipoNome: { type: String, default: '' },
  cupomCode: { type: String, default: '' },
  qty: { type: Number, default: 1 },
  subtotal: { type: Number, default: 0 },
  desconto: { type: Number, default: 0 },
  taxa: { type: Number, default: 0 },
  total: { type: Number, default: 0 },
})

const emit = defineEmits(['update:modelValue', 'pagamento-concluido'])

// Estado geral
const step = ref('metodo') // metodo | pix | cartao | sucesso
const metodo = ref('')
const processando = ref(false)
const metodoPago = ref('')

// PIX
const copiado = ref(false)
const pixCode = ref('00020126580014BR.GOV.BCB.PIX0136a1b2c3d4-e5f6-7890-abcd-ef1234567890520400005303986540' + '10.005802BR5915Equilibrium6008Vitoria62070503***63041D3D')
const timerSecs = ref(300) // 5 min
let timerInterval = null
const timerOffset = computed(() => {
  const pct = timerSecs.value / 300
  return 100 - (pct * 100)
})
const timerDisplay = computed(() => {
  const m = Math.floor(timerSecs.value / 60)
  const s = timerSecs.value % 60
  return `${m}:${s.toString().padStart(2, '0')}`
})

// Cartão
const cardFlipped = ref(false)
const cartaoErro = ref('')
const cartao = ref({ numero: '', nome: '', validade: '', cvv: '', parcelas: 1 })

const parcelasOptions = computed(() => {
  const opts = []
  for (let i = 1; i <= 12; i++) {
    const val = props.total / i
    opts.push({
      n: i,
      label: i === 1
        ? `À vista — ${fmtBRL(props.total)}`
        : `${i}x de ${fmtBRL(val)}${val > 10 ? ' sem juros' : ''}`
    })
  }
  return opts
})

// Funções
function fmtBRL(v) {
  return (v || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

function selecionarMetodo(m) {
  metodo.value = m
  step.value = m
  if (m === 'pix') {
    timerSecs.value = 300
    timerInterval = setInterval(() => {
      timerSecs.value--
      if (timerSecs.value <= 0) clearInterval(timerInterval)
    }, 1000)
  }
}

function fechar() {
  if (step.value === 'sucesso') {
    emit('pagamento-concluido')
  }
  emit('update:modelValue', false)
  resetar()
}

function resetar() {
  step.value = 'metodo'
  metodo.value = ''
  cartao.value = { numero: '', nome: '', validade: '', cvv: '', parcelas: 1 }
  cartaoErro.value = ''
  cardFlipped.value = false
  copiado.value = false
  clearInterval(timerInterval)
}

async function confirmarPagamento(metodoUsado) {
  if (metodoUsado === 'cartao') {
    cartaoErro.value = ''
    if (!cartao.value.numero || cartao.value.numero.replace(/\s/g,'').length < 16) {
      cartaoErro.value = 'Informe um número de cartão válido.'
      return
    }
    if (!cartao.value.nome.trim()) {
      cartaoErro.value = 'Informe o nome do titular.'
      return
    }
    if (!cartao.value.validade || !/^\d{2}\/\d{2}$/.test(cartao.value.validade)) {
      cartaoErro.value = 'Informe a validade no formato MM/AA.'
      return
    }
    if (!cartao.value.cvv || cartao.value.cvv.length < 3) {
      cartaoErro.value = 'Informe o CVV.'
      return
    }
  }

  processando.value = true
  
  try {
    // Chamada real para a API criar o pedido e consumir estoque/cupom
    await api.pedidos.criar({
      eventoId: props.eventoId,
      codigoCupom: props.cupomCode || null,
      itens: [{
        tipoIngressoId: props.tipoIngressoId,
        quantidade: props.qty,
        nomeDono: authStore.currentUser?.nome || 'Usuário',
        emailDono: authStore.currentUser?.email || 'email@exemplo.com'
      }]
    })

    processando.value = false
    metodoPago.value = metodoUsado
    clearInterval(timerInterval)
    step.value = 'sucesso'
  } catch (e) {
    if (metodoUsado === 'cartao') {
      cartaoErro.value = e.message || 'Erro ao processar pagamento'
    } else {
      alert(e.message || 'Erro ao processar pagamento')
    }
    processando.value = false
  }
}

async function copiarPix() {
  try {
    await navigator.clipboard.writeText(pixCode.value)
    copiado.value = true
    setTimeout(() => { copiado.value = false }, 3000)
  } catch {
    copiado.value = true
    setTimeout(() => { copiado.value = false }, 3000)
  }
}

function formatarNumero(e) {
  let v = e.target.value.replace(/\D/g, '').slice(0, 16)
  cartao.value.numero = v.replace(/(.{4})/g, '$1 ').trim()
}

function formatarValidade(e) {
  let v = e.target.value.replace(/\D/g, '').slice(0, 4)
  if (v.length >= 3) v = v.slice(0, 2) + '/' + v.slice(2)
  cartao.value.validade = v
}

function formatCardNum(num) {
  const clean = num.replace(/\s/g, '')
  const masked = clean.slice(0, 4) + ' ' +
    (clean.slice(4, 8) ? '••••' : '') + ' ' +
    (clean.slice(8, 12) ? '••••' : '') + ' ' +
    (clean.slice(12) || '')
  return masked.trim() || '•••• •••• •••• ••••'
}

function detectBrand(num) {
  const n = (num || '').replace(/\s/g, '')
  if (/^4/.test(n)) return 'VISA'
  if (/^5[1-5]/.test(n)) return 'MASTER'
  if (/^3[47]/.test(n)) return 'AMEX'
  if (/^6(?:011|5)/.test(n)) return 'ELO'
  return '💳'
}

// Gera padrão visual para QR code fake
function qrPattern(r, c) {
  const corners = [[1,1],[1,2],[1,3],[2,1],[3,1],[3,2],[3,3],[2,3],
                   [1,7],[1,8],[1,9],[2,7],[3,7],[3,8],[3,9],[2,9],
                   [7,1],[7,2],[7,3],[8,1],[9,1],[9,2],[9,3],[8,3]]
  const fill = [[1,1],[1,2],[1,3],[2,2],[3,1],[3,2],[3,3],
                [1,7],[1,8],[1,9],[2,8],[3,7],[3,8],[3,9],
                [7,1],[7,2],[7,3],[8,2],[9,1],[9,2],[9,3],
                [5,1],[5,3],[5,5],[5,7],[5,9],[1,5],[3,5],[7,5],[9,5],
                [4,4],[4,6],[6,4],[6,6],[5,5],[2,5],[5,2],[8,5],[5,8]]
  const key = fill.some(([fr, fc]) => fr === r && fc === c)
  return key ? 'qr-dark' : ((r + c) % 3 === 0 ? 'qr-dark' : '')
}

// Reset ao fechar
watch(() => props.modelValue, (v) => {
  if (!v) resetar()
})

onUnmounted(() => clearInterval(timerInterval))
</script>

<style scoped>
/* ========= Overlay ========= */
.pag-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.82);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 16px;
}

/* ========= Panel ========= */
.pag-panel {
  background: linear-gradient(145deg, #1a1a2e 0%, #16213e 50%, #0f0f23 100%);
  border: 1px solid rgba(124, 58, 237, 0.3);
  border-radius: 24px;
  width: 100%;
  max-width: 480px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 40px 80px rgba(0,0,0,0.6), 0 0 0 1px rgba(124,58,237,0.15), inset 0 1px 0 rgba(255,255,255,0.05);
  scrollbar-width: thin;
  scrollbar-color: rgba(124,58,237,0.4) transparent;
}

/* ========= Header ========= */
.pag-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 24px 16px;
  border-bottom: 1px solid rgba(255,255,255,0.06);
}
.pag-header-left { display: flex; align-items: center; gap: 12px; }
.pag-lock { font-size: 22px; }
.pag-header-title { font-size: 15px; font-weight: 700; color: #E2E8F0; margin: 0; }
.pag-header-sub { font-size: 11px; color: #64748B; margin: 2px 0 0; }
.pag-close {
  width: 32px; height: 32px;
  background: rgba(255,255,255,0.06);
  border: 1px solid rgba(255,255,255,0.1);
  border-radius: 8px;
  color: #94A3B8;
  cursor: pointer;
  font-size: 13px;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s;
}
.pag-close:hover { background: rgba(239,68,68,0.15); border-color: rgba(239,68,68,0.3); color: #FCA5A5; }

/* ========= Resumo ========= */
.pag-resumo {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  background: rgba(124, 58, 237, 0.08);
  border-bottom: 1px solid rgba(124,58,237,0.15);
}
.pag-resumo-label { font-size: 14px; font-weight: 700; color: #E2E8F0; margin: 0; }
.pag-resumo-evento { font-size: 12px; color: #64748B; margin: 3px 0 0; }
.pag-resumo-total { font-size: 22px; font-weight: 900; background: linear-gradient(135deg, #A78BFA, #EC4899); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; margin: 0; text-align: right; }
.pag-resumo-desconto { font-size: 11px; color: #6EE7B7; margin: 2px 0 0; text-align: right; }

/* ========= Métodos ========= */
.metodo-section { padding: 20px 24px 0; }
.metodo-titulo { font-size: 13px; font-weight: 600; color: #94A3B8; text-transform: uppercase; letter-spacing: 0.08em; margin: 0 0 12px; }
.metodo-opcoes { display: flex; flex-direction: column; gap: 10px; }

.metodo-btn {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 14px 16px;
  background: rgba(255,255,255,0.03);
  border: 1.5px solid rgba(255,255,255,0.08);
  border-radius: 14px;
  cursor: pointer;
  transition: all 0.25s;
  text-align: left;
  width: 100%;
}
.metodo-btn:hover { border-color: rgba(124,58,237,0.5); background: rgba(124,58,237,0.06); }
.metodo-btn.active { border-color: #7C3AED; background: rgba(124,58,237,0.12); box-shadow: 0 0 0 3px rgba(124,58,237,0.15); }

.metodo-icon { font-size: 24px; width: 44px; height: 44px; border-radius: 12px; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.pix-icon { background: linear-gradient(135deg, #00B37E, #00875A); color: white; }
.card-icon { background: linear-gradient(135deg, #3B82F6, #1D4ED8); }

.metodo-info { flex: 1; }
.metodo-nome { display: block; font-size: 15px; font-weight: 700; color: #E2E8F0; }
.metodo-desc { display: block; font-size: 12px; color: #64748B; margin-top: 2px; }

.metodo-badge { font-size: 10px; font-weight: 800; padding: 3px 8px; border-radius: 20px; white-space: nowrap; text-transform: uppercase; letter-spacing: 0.06em; }
.pix-badge { background: rgba(0,179,126,0.15); color: #6EE7B7; border: 1px solid rgba(0,179,126,0.3); }
.card-badge { background: rgba(59,130,246,0.15); color: #93C5FD; border: 1px solid rgba(59,130,246,0.3); }

/* ========= PIX ========= */
.pix-section { padding: 20px 24px 0; }
.pix-card { display: flex; flex-direction: column; align-items: center; gap: 20px; }

.pix-qr-wrapper { display: flex; align-items: center; gap: 20px; }

.pix-qr {
  width: 140px; height: 140px;
  background: white;
  border-radius: 12px;
  padding: 10px;
  position: relative;
  box-shadow: 0 0 0 4px rgba(0,179,126,0.3), 0 8px 24px rgba(0,0,0,0.4);
}
.qr-inner { display: grid; grid-template-rows: repeat(9, 1fr); gap: 1px; height: 100%; }
.qr-row { display: grid; grid-template-columns: repeat(9, 1fr); gap: 1px; }
.qr-cell { border-radius: 1px; background: white; }
.qr-cell.qr-dark { background: #0D0D0D; }
.qr-logo {
  position: absolute;
  top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  width: 28px; height: 28px;
  background: #00B37E;
  border-radius: 6px;
  display: flex; align-items: center; justify-content: center;
  font-size: 14px;
  box-shadow: 0 0 0 3px white;
}

.pix-timer { display: flex; flex-direction: column; align-items: center; gap: 6px; }
.timer-ring { position: relative; width: 70px; height: 70px; }
.timer-svg { transform: rotate(-90deg); width: 100%; height: 100%; }
.timer-bg { fill: none; stroke: rgba(255,255,255,0.08); stroke-width: 3; }
.timer-fill { fill: none; stroke: #00B37E; stroke-width: 3; stroke-linecap: round; stroke-dasharray: 100; transition: stroke-dashoffset 1s linear; }
.timer-num {
  position: absolute; top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  font-size: 12px; font-weight: 800; color: #E2E8F0;
}
.timer-label { font-size: 10px; color: #64748B; text-align: center; }

.pix-instrucoes { width: 100%; }
.pix-step {
  display: flex; align-items: center; gap: 10px;
  font-size: 13px; color: #94A3B8;
  padding: 8px 0;
  border-bottom: 1px solid rgba(255,255,255,0.04);
}
.pix-step:last-child { border-bottom: none; }
.pix-step strong { color: #A78BFA; }
.pix-num {
  width: 22px; height: 22px; border-radius: 50%;
  background: rgba(0,179,126,0.2); border: 1px solid rgba(0,179,126,0.4);
  color: #6EE7B7;
  display: flex; align-items: center; justify-content: center;
  font-size: 11px; font-weight: 800; flex-shrink: 0;
}

.pix-copy-section { width: 100%; }
.pix-copy-label { font-size: 12px; color: #64748B; margin: 0 0 8px; }
.pix-copy-row { display: flex; gap: 8px; }
.pix-code {
  flex: 1; padding: 10px 12px;
  background: rgba(255,255,255,0.04);
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 10px;
  color: #64748B; font-size: 11px; font-family: monospace;
  overflow: hidden; text-overflow: ellipsis; white-space: nowrap;
}
.btn-copiar {
  padding: 10px 14px;
  background: rgba(0,179,126,0.15); border: 1px solid rgba(0,179,126,0.3);
  border-radius: 10px; color: #6EE7B7; font-size: 12px; font-weight: 700;
  cursor: pointer; white-space: nowrap; transition: all 0.2s;
}
.btn-copiar:hover { background: rgba(0,179,126,0.25); }

.btn-confirmar-pix {
  width: 100%; padding: 15px;
  background: linear-gradient(135deg, #00B37E, #00875A);
  border: none; border-radius: 14px;
  color: white; font-size: 15px; font-weight: 800;
  cursor: pointer; transition: all 0.25s;
  box-shadow: 0 6px 20px rgba(0,179,126,0.35);
  display: flex; align-items: center; justify-content: center; min-height: 52px;
}
.btn-confirmar-pix:hover:not(.loading) { box-shadow: 0 10px 30px rgba(0,179,126,0.5); transform: translateY(-2px); }

/* ========= CARTÃO ========= */
.cartao-section { padding: 20px 24px 0; }

/* Card Preview */
.card-preview-wrapper {
  width: 100%; max-width: 320px; height: 190px;
  margin: 0 auto 20px;
  perspective: 1000px;
}
.card-face {
  width: 100%; height: 100%; border-radius: 16px;
  position: absolute;
  backface-visibility: hidden;
  transition: transform 0.6s ease;
}
.card-preview-wrapper { position: relative; transform-style: preserve-3d; transition: transform 0.6s ease; }
.card-preview-wrapper.flipped { transform: rotateY(180deg); }

.card-front {
  background: linear-gradient(135deg, #1E1B4B 0%, #312E81 40%, #7C3AED 100%);
  padding: 20px 22px;
  display: flex; flex-direction: column; justify-content: space-between;
  box-shadow: 0 20px 50px rgba(0,0,0,0.5), inset 0 1px 0 rgba(255,255,255,0.1);
  border: 1px solid rgba(255,255,255,0.12);
}
.card-back {
  background: linear-gradient(135deg, #1E1B4B 0%, #312E81 100%);
  transform: rotateY(180deg);
  padding: 20px 0;
  display: flex; flex-direction: column; justify-content: center;
  box-shadow: 0 20px 50px rgba(0,0,0,0.5);
  border: 1px solid rgba(255,255,255,0.12);
}

.card-chip {
  width: 36px; height: 28px;
  background: linear-gradient(135deg, #F59E0B, #D97706);
  border-radius: 5px;
  display: flex; align-items: center; justify-content: center;
}
.chip-lines { display: flex; flex-direction: column; gap: 4px; padding: 4px; }
.chip-line { height: 2px; background: rgba(0,0,0,0.3); border-radius: 1px; }

.card-number-display {
  font-size: 16px; letter-spacing: 0.15em; font-weight: 600;
  color: rgba(255,255,255,0.9); font-family: 'Courier New', monospace;
}
.card-bottom { display: flex; align-items: flex-end; gap: 20px; }
.card-field-label { font-size: 9px; color: rgba(255,255,255,0.5); text-transform: uppercase; letter-spacing: 0.08em; margin: 0; }
.card-field-value { font-size: 13px; font-weight: 600; color: white; margin: 3px 0 0; letter-spacing: 0.04em; }
.card-brand { margin-left: auto; font-size: 14px; font-weight: 900; color: white; letter-spacing: 0.04em; }

.card-stripe { height: 44px; background: #1a1a1a; margin-bottom: 20px; }
.card-cvv-row { display: flex; align-items: center; justify-content: flex-end; gap: 12px; padding: 0 22px; }
.cvv-label { font-size: 11px; color: rgba(255,255,255,0.5); }
.cvv-box {
  background: white; color: #1a1a2e;
  padding: 6px 16px; border-radius: 4px;
  font-size: 14px; font-weight: 700; font-family: monospace;
  letter-spacing: 0.12em;
}

/* Formulário */
.cartao-form { display: flex; flex-direction: column; gap: 12px; }
.cf-field { display: flex; flex-direction: column; gap: 6px; }
.cf-row { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.cf-label { font-size: 12px; font-weight: 600; color: #94A3B8; text-transform: uppercase; letter-spacing: 0.06em; }
.cf-input {
  padding: 12px 14px;
  background: rgba(255,255,255,0.05);
  border: 1.5px solid rgba(255,255,255,0.1);
  border-radius: 12px;
  color: #E2E8F0;
  font-size: 14px;
  font-family: inherit;
  transition: all 0.25s;
  -webkit-appearance: none;
}
.cf-input:focus { outline: none; border-color: #7C3AED; background: rgba(124,58,237,0.06); box-shadow: 0 0 0 3px rgba(124,58,237,0.15); }
.cf-input::placeholder { color: #475569; }
.cf-select { cursor: pointer; }
.cf-select option { background: #1a1a2e; }

.cartao-erro { font-size: 12px; color: #FCA5A5; background: rgba(239,68,68,0.1); border: 1px solid rgba(239,68,68,0.2); border-radius: 8px; padding: 8px 12px; margin: 0; }

.btn-pagar {
  width: 100%; padding: 16px;
  background: linear-gradient(135deg, #7C3AED, #EC4899);
  border: none; border-radius: 14px;
  color: white; font-size: 16px; font-weight: 800;
  cursor: pointer; transition: all 0.25s;
  box-shadow: 0 6px 24px rgba(124,58,237,0.4);
  display: flex; align-items: center; justify-content: center; min-height: 54px;
}
.btn-pagar:hover:not(.loading) { box-shadow: 0 12px 36px rgba(124,58,237,0.6); transform: translateY(-2px); }

/* Spinner */
.loading { opacity: 0.8; pointer-events: none; }
.spinner {
  width: 22px; height: 22px;
  border: 3px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ========= Footer ========= */
.pag-footer {
  display: flex; gap: 16px; justify-content: center;
  padding: 16px 24px;
  font-size: 11px; color: #475569;
  border-top: 1px solid rgba(255,255,255,0.05);
  margin-top: 20px;
}

/* ========= Sucesso ========= */
.pag-sucesso-panel {
  max-width: 400px;
  background: linear-gradient(145deg, #0D1117 0%, #1a1a2e 100%);
  border-color: rgba(110,231,183,0.3);
}
.sucesso-content {
  padding: 40px 32px;
  display: flex; flex-direction: column; align-items: center; gap: 16px;
  text-align: center;
}

.sucesso-icon-wrap {
  position: relative;
  width: 80px; height: 80px;
  display: flex; align-items: center; justify-content: center;
  margin-bottom: 8px;
}
.sucesso-ring {
  position: absolute; border-radius: 50%;
  border: 2px solid #6EE7B7;
  animation: ring-pulse 1.5s ease-out infinite;
}
.sucesso-ring-1 { width: 80px; height: 80px; animation-delay: 0s; }
.sucesso-ring-2 { width: 100px; height: 100px; animation-delay: 0.3s; opacity: 0.5; }
@keyframes ring-pulse {
  0% { transform: scale(0.8); opacity: 1; }
  100% { transform: scale(1.2); opacity: 0; }
}
.sucesso-icon {
  width: 64px; height: 64px; border-radius: 50%;
  background: linear-gradient(135deg, #00B37E, #059669);
  display: flex; align-items: center; justify-content: center;
  font-size: 28px; color: white; font-weight: 900;
  position: relative; z-index: 1;
  box-shadow: 0 0 30px rgba(0,179,126,0.5);
}

.sucesso-titulo { font-size: 24px; font-weight: 900; color: #E2E8F0; margin: 0; }
.sucesso-sub { font-size: 14px; color: #64748B; margin: 0; }

.sucesso-detalhe {
  width: 100%;
  background: rgba(255,255,255,0.04);
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 14px;
  padding: 16px;
  display: flex; flex-direction: column; gap: 12px;
}
.sucesso-row { display: flex; justify-content: space-between; font-size: 13px; }
.sucesso-row span { color: #64748B; }
.sucesso-row strong { color: #E2E8F0; }
.sucesso-valor { background: linear-gradient(135deg, #A78BFA, #EC4899); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; font-size: 16px; }

.sucesso-email { font-size: 12px; color: #64748B; margin: 0; }

.btn-sucesso {
  width: 100%; padding: 15px;
  background: linear-gradient(135deg, #7C3AED, #EC4899);
  border: none; border-radius: 14px; color: white;
  font-size: 15px; font-weight: 700; cursor: pointer;
  transition: all 0.25s;
  box-shadow: 0 6px 20px rgba(124,58,237,0.4);
}
.btn-sucesso:hover { transform: translateY(-2px); box-shadow: 0 12px 30px rgba(124,58,237,0.6); }

/* ========= Transitions ========= */
.modal-enter-active, .modal-leave-active { transition: opacity 0.3s ease; }
.modal-enter-from, .modal-leave-to { opacity: 0; }
.modal-enter-active .pag-panel, .modal-leave-active .pag-panel { transition: transform 0.3s ease, opacity 0.3s ease; }
.modal-enter-from .pag-panel, .modal-leave-to .pag-panel { transform: scale(0.92) translateY(20px); opacity: 0; }

.slide-enter-active, .slide-leave-active { transition: all 0.3s ease; }
.slide-enter-from { opacity: 0; transform: translateY(16px); }
.slide-leave-to { opacity: 0; transform: translateY(-8px); }

@media (max-width: 520px) {
  .pix-qr-wrapper { flex-direction: column; }
  .card-preview-wrapper { max-width: 280px; height: 170px; }
  .card-number-display { font-size: 13px; }
}
</style>
