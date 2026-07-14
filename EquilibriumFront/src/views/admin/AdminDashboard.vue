<template>
  <div class="admin-dashboard">
    <!-- Metric cards -->
    <div class="metrics-grid">
      <div v-for="metric in metrics" :key="metric.id" class="metric-card" :id="`metric-${metric.id}`">
        <div class="metric-icon">{{ metric.icon }}</div>
        <div class="metric-info">
          <p class="metric-label">{{ metric.label }}</p>
          <p class="metric-value">{{ metric.value }}</p>
          <p class="metric-change" :class="metric.up ? 'up' : 'down'">
            {{ metric.up ? '↑' : '↓' }} {{ metric.change }} este mês
          </p>
        </div>
        <div class="metric-chart">
          <div
            v-for="(bar, i) in metric.bars"
            :key="i"
            class="bar"
            :style="{ height: bar + '%' }"
          ></div>
        </div>
      </div>
    </div>

    <!-- Main content grid -->
    <div class="dashboard-grid">
      <!-- Recent events -->
      <div class="dashboard-card">
        <div class="card-header">
          <h2 class="card-title">Eventos recentes</h2>
          <router-link to="/admin/eventos" class="card-link" id="link-ver-eventos">Ver todos →</router-link>
        </div>
        <div class="event-list">
          <div v-for="evento in eventosStore.eventos.slice(0, 5)" :key="evento.id" class="event-row" :id="`dashboard-event-${evento.id}`">
            <img :src="evento.imagem" :alt="evento.nome" class="event-row-img" />
            <div class="event-row-info">
              <p class="event-row-nome">{{ evento.nome }}</p>
              <p class="event-row-meta">{{ formatDate(evento.data) }} · {{ evento.local.split(',')[0] }} · <strong>{{ ticketsPorEvento[evento.id] || 0 }} ingressos vendidos</strong></p>
            </div>
            <BaseBadge :variant="evento.destaque ? 'info' : 'default'">
              {{ evento.destaque ? 'Destaque' : 'Normal' }}
            </BaseBadge>
          </div>
        </div>
      </div>

      <!-- Quick actions -->
      <div class="dashboard-card">
        <h2 class="card-title" style="margin-bottom: var(--space-lg);">Ações rápidas</h2>
        <div class="quick-actions">
          <router-link v-for="action in quickActions" :key="action.to" :to="action.to" class="quick-action" :id="`quick-action-${action.id}`">
            <span class="qa-icon">{{ action.icon }}</span>
            <span class="qa-label">{{ action.label }}</span>
            <span class="qa-arrow">→</span>
          </router-link>
        </div>

        <!-- Cupons ativos -->
        <h3 class="card-subtitle" style="margin-top: var(--space-xl);">Cupons ativos</h3>
        <div class="cupons-quick">
          <div v-for="cupom in cuponsStore.cupons.filter(c => c.ativo).slice(0, 3)" :key="cupom.id" class="cupom-row">
            <span class="cupom-code">{{ cupom.codigo }}</span>
            <span class="cupom-desconto">
              {{ cupom.tipo === 'percentual' ? cupom.desconto + '%' : 'R$ ' + cupom.desconto }}
            </span>
            <div class="cupom-progress-bar">
              <div class="cupom-progress-fill" :style="{ width: (cupom.usosAtuais / cupom.usoMaximo * 100) + '%' }"></div>
            </div>
            <span class="cupom-usage">{{ cupom.usosAtuais }}/{{ cupom.usoMaximo }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted } from 'vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import { useEventosStore } from '../../stores/eventosStore.js'
import { useCuponsStore } from '../../stores/cuponsStore.js'
import { useClientesStore } from '../../stores/clientesStore.js'
import { api } from '../../services/api.js'

const eventosStore = useEventosStore()
const cuponsStore = useCuponsStore()
const clientesStore = useClientesStore()

const pedidos = ref([])

onMounted(async () => {
  eventosStore.carregar()
  cuponsStore.carregar()
  clientesStore.carregar()
  try {
    pedidos.value = await api.pedidos.listarTodos()
  } catch (e) {
    console.error('Erro ao carregar pedidos no dashboard', e)
  }
})

const formatter = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })
const totalReceita = computed(() => pedidos.value.reduce((acc, p) => acc + p.valorTotal, 0))
const totalIngressos = computed(() => pedidos.value.reduce((acc, p) => acc + p.itens.length, 0))

const ticketsPorEvento = computed(() => {
  const counts = {}
  pedidos.value.forEach(p => {
    counts[p.eventoId] = (counts[p.eventoId] || 0) + (p.itens ? p.itens.length : 0)
  })
  return counts
})

const metrics = computed(() => [
  {
    id: 'eventos', label: 'Total de eventos', icon: '🎭',
    value: eventosStore.eventos.length,
    change: 'Ativo', up: true,
    bars: [40, 60, 45, 70, 55, 80, 65]
  },
  {
    id: 'ingressos', label: 'Ingressos vendidos', icon: '🎫',
    value: totalIngressos.value,
    change: 'Ativo', up: true,
    bars: [30, 50, 70, 45, 80, 60, 90]
  },
  {
    id: 'receita', label: 'Receita total', icon: '💰',
    value: formatter.format(totalReceita.value),
    change: 'Ativo', up: true,
    bars: [50, 65, 40, 75, 60, 85, 70]
  },
  {
    id: 'clientes', label: 'Clientes cadastrados', icon: '👥',
    value: clientesStore.clientes.length,
    change: 'Ativo', up: true,
    bars: [20, 40, 30, 60, 45, 70, 55]
  }
])

const quickActions = [
  { to: '/admin/eventos', label: 'Criar novo evento', icon: '➕', id: 'novo-evento' },
  { to: '/admin/cupons', label: 'Criar cupom de desconto', icon: '🎟️', id: 'novo-cupom' },
  { to: '/admin/clientes', label: 'Ver clientes', icon: '👥', id: 'ver-clientes' }
]

function formatDate(dateStr) {
  const d = new Date(dateStr + 'T00:00:00')
  return d.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' })
}
</script>

<style scoped>
.admin-dashboard { display: flex; flex-direction: column; gap: var(--space-xl); }

/* Metrics */
.metrics-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: var(--space-lg);
}

.metric-card {
  background: var(--clr-bg-2);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-lg);
  padding: var(--space-lg);
  display: flex;
  gap: var(--space-md);
  position: relative;
  overflow: hidden;
  transition: all var(--transition-md);
}
.metric-card:hover { border-color: rgba(124, 58, 237, 0.3); box-shadow: var(--shadow-md); }

.metric-icon { font-size: 28px; }
.metric-info { flex: 1; }
.metric-label { font-size: 12px; color: var(--clr-text-muted); text-transform: uppercase; letter-spacing: 0.06em; }
.metric-value { font-size: 28px; font-weight: 900; font-family: var(--font-display); margin: 4px 0; }
.metric-change { font-size: 12px; font-weight: 600; }
.metric-change.up { color: var(--clr-success); }
.metric-change.down { color: var(--clr-error); }

.metric-chart {
  display: flex;
  align-items: flex-end;
  gap: 3px;
  height: 40px;
  opacity: 0.5;
}
.bar {
  width: 6px;
  background: var(--grad-primary);
  border-radius: 3px;
  min-height: 4px;
  transition: height var(--transition-md);
}

/* Dashboard grid */
.dashboard-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-xl);
}

.dashboard-card {
  background: var(--clr-bg-2);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-lg);
  padding: var(--space-xl);
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-lg);
}

.card-title { font-size: 17px; font-weight: 700; }
.card-subtitle { font-size: 15px; font-weight: 600; color: var(--clr-text-muted); }

.card-link { font-size: 13px; color: var(--clr-primary-light); transition: color var(--transition-fast); }
.card-link:hover { color: var(--clr-secondary-light); }

/* Event list */
.event-list { display: flex; flex-direction: column; gap: var(--space-md); }
.event-row {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  padding: var(--space-sm) 0;
  border-bottom: 1px solid var(--clr-border);
}
.event-row:last-child { border-bottom: none; }
.event-row-img { width: 48px; height: 48px; border-radius: var(--radius-sm); object-fit: cover; flex-shrink: 0; }
.event-row-info { flex: 1; }
.event-row-nome { font-size: 14px; font-weight: 600; }
.event-row-meta { font-size: 12px; color: var(--clr-text-muted); margin-top: 2px; }

/* Quick actions */
.quick-actions { display: flex; flex-direction: column; gap: 8px; }
.quick-action {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  padding: var(--space-md);
  background: var(--clr-surface);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
}
.quick-action:hover { border-color: var(--clr-primary); background: rgba(124, 58, 237, 0.08); }
.qa-icon { font-size: 20px; }
.qa-label { flex: 1; font-size: 14px; font-weight: 500; }
.qa-arrow { color: var(--clr-text-muted); }

/* Cupons quick */
.cupons-quick { display: flex; flex-direction: column; gap: var(--space-sm); margin-top: var(--space-sm); }
.cupom-row {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  padding: 8px 12px;
  background: var(--clr-surface);
  border-radius: var(--radius-md);
  border: 1px solid var(--clr-border);
}
.cupom-code { font-size: 13px; font-weight: 700; font-family: monospace; color: var(--clr-primary-light); min-width: 100px; }
.cupom-desconto { font-size: 13px; font-weight: 700; color: #6EE7B7; min-width: 50px; text-align: right; }
.cupom-progress-bar { flex: 1; height: 4px; background: var(--clr-surface-hover); border-radius: var(--radius-full); overflow: hidden; }
.cupom-progress-fill { height: 100%; background: var(--grad-primary); border-radius: var(--radius-full); }
.cupom-usage { font-size: 11px; color: var(--clr-text-subtle); white-space: nowrap; }

@media (max-width: 1200px) { .metrics-grid { grid-template-columns: repeat(2, 1fr); } }
@media (max-width: 900px) { .dashboard-grid { grid-template-columns: 1fr; } }
@media (max-width: 600px) { .metrics-grid { grid-template-columns: 1fr; } }
</style>
