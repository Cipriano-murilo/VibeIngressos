<template>
  <div class="admin-eventos">
    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-left">
        <input v-model="search" type="search" placeholder="Buscar evento..." class="search-input" id="search-eventos" />
      </div>
      <BaseButton id="btn-novo-evento" variant="primary" @click="openModal()">
        ➕ Novo evento
      </BaseButton>
    </div>

    <!-- Tabela de eventos ATIVOS -->
    <div class="table-wrapper">
      <table class="data-table" aria-label="Lista de eventos">
        <thead>
          <tr>
            <th>Evento</th>
            <th>Data</th>
            <th>Local</th>
            <th>Destaque</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="evento in filteredEventos" :key="evento.id" :id="`row-evento-${evento.id}`">
            <td>
              <div class="cell-evento">
                <img :src="evento.imagem" :alt="evento.nome" class="cell-img" />
                <span class="cell-nome">{{ evento.nome }}</span>
              </div>
            </td>
            <td>{{ formatDate(evento.data) }}</td>
            <td>{{ evento.local.split(',')[0] }}</td>
            <td>
              <BaseBadge :variant="evento.destaque ? 'success' : 'default'">
                {{ evento.destaque ? 'Sim' : 'Não' }}
              </BaseBadge>
            </td>
            <td>
              <div class="actions">
                <button class="action-btn edit" @click="openModal(evento)" :id="`btn-editar-${evento.id}`" title="Editar">✏️</button>
                <button class="action-btn delete" @click="confirmDelete(evento)" :id="`btn-excluir-${evento.id}`" title="Excluir">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="filteredEventos.length === 0" class="empty-table">
        <span>🎭</span>
        <p>Nenhum evento ativo encontrado.</p>
      </div>
    </div>

    <!-- Seção de Eventos Expirados -->
    <div class="expired-section" v-if="eventosExpirados.length > 0">
      <div class="expired-header">
        <span class="expired-title">⏰ Eventos expirados</span>
        <span class="expired-count">{{ eventosExpirados.length }} evento{{ eventosExpirados.length > 1 ? 's' : '' }}</span>
      </div>
      <div class="expired-grid">
        <div
          v-for="evento in eventosExpirados"
          :key="evento.id"
          class="expired-card"
          :id="`expired-${evento.id}`"
        >
          <img :src="evento.imagem" :alt="evento.nome" class="expired-img" />
          <div class="expired-info">
            <div class="expired-top">
              <span class="expired-badge">⏳ Expirado</span>
              <button class="action-btn delete" @click="confirmDelete(evento)" :title="'Excluir ' + evento.nome">🗑️</button>
            </div>
            <p class="expired-nome">{{ evento.nome }}</p>
            <p class="expired-data">📅 {{ formatDate(evento.data) }} · {{ evento.local.split(',')[0] }}</p>
            <div class="expired-stats">
              <div class="expired-stat">
                <span class="estat-icon">🎫</span>
                <div>
                  <p class="estat-value">{{ ticketsPorEvento[evento.id] || 0 }}</p>
                  <p class="estat-label">ingressos vendidos</p>
                </div>
              </div>
              <div class="expired-stat">
                <span class="estat-icon">💰</span>
                <div>
                  <p class="estat-value">{{ fmtBRL(lucroEvento[evento.id] || 0) }}</p>
                  <p class="estat-label">receita total</p>
                </div>
              </div>
              <div class="expired-stat">
                <span class="estat-icon">📈</span>
                <div>
                  <p class="estat-value">{{ fmtBRL(mediaTicket[evento.id] || 0) }}</p>
                  <p class="estat-label">é a média por ingresso</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <BaseModal v-model="modalOpen" :title="editingEvento ? 'Editar evento' : 'Criar novo evento'" width="680px">
      <form @submit.prevent="handleSave" id="form-evento" class="form-grid" novalidate>
        <div class="form-row-2">
          <div class="form-field">
            <label class="field-label" for="evento-nome">Nome do evento *</label>
            <input id="evento-nome" v-model="form.nome" type="text" class="field-input" placeholder="Nome do evento" required />
            <p v-if="formErrors.nome" class="field-error">{{ formErrors.nome }}</p>
          </div>
        </div>

        <div class="form-row-2">
          <div class="form-field">
            <label class="field-label" for="evento-data">Data *</label>
            <input id="evento-data" v-model="form.data" type="date" class="field-input" required />
            <p v-if="formErrors.data" class="field-error">{{ formErrors.data }}</p>
          </div>
          <div class="form-field">
            <label class="field-label" for="evento-hora">Horário *</label>
            <input id="evento-hora" v-model="form.hora" type="time" class="field-input" required />
          </div>
        </div>

        <div class="form-field">
          <label class="field-label" for="evento-local">Local *</label>
          <input id="evento-local" v-model="form.local" type="text" class="field-input" placeholder="Nome do local, Cidade - Estado" />
          <p v-if="formErrors.local" class="field-error">{{ formErrors.local }}</p>
        </div>

        <div class="form-field">
          <label class="field-label" for="evento-descricao">Descrição</label>
          <textarea id="evento-descricao" v-model="form.descricao" class="field-input" rows="3" placeholder="Descrição do evento..."></textarea>
        </div>

        <div class="form-field">
          <label class="field-label" for="evento-imagem">URL da imagem</label>
          <input id="evento-imagem" v-model="form.imagem" type="url" class="field-input" placeholder="https://..." />
          <img v-if="form.imagem" :src="form.imagem" class="img-preview" alt="Preview" />
        </div>

        <div class="form-field">
          <label class="toggle-label">
            <span class="field-label">Marcar como destaque</span>
            <div class="toggle" :class="{ on: form.destaque }" @click="form.destaque = !form.destaque" id="toggle-destaque" role="switch" :aria-checked="form.destaque">
              <div class="toggle-thumb"></div>
            </div>
          </label>
        </div>

        <!-- Ticket types -->
        <div class="form-field">
          <div class="tipos-header">
            <span class="field-label">Tipos de ingresso</span>
            <button type="button" class="btn-add-tipo" @click="addTipo" id="btn-add-tipo">+ Adicionar</button>
          </div>
          <!-- Header de colunas -->
          <div v-if="form.tipos.length > 0" class="tipo-cols-header">
            <span>Nome do ingresso</span>
            <span>Valor (R$)</span>
            <span>Quantidade</span>
            <span></span>
          </div>
          <div v-for="(tipo, i) in form.tipos" :key="i" class="tipo-row">
            <input v-model="tipo.nome" type="text" class="field-input tipo-input" :placeholder="`Ex: Pista, VIP...`" :id="`tipo-nome-${i}`" />
            <input v-model.number="tipo.preco" type="number" class="field-input tipo-input" placeholder="0,00" min="0" step="0.01" :id="`tipo-preco-${i}`" />
            <input v-model.number="tipo.disponivel" type="number" class="field-input tipo-input" placeholder="100" min="1" :id="`tipo-qtd-${i}`" />
            <button type="button" class="btn-rm-tipo" @click="form.tipos.splice(i, 1)" :id="`btn-rm-tipo-${i}`">✕</button>
          </div>
          <p v-if="form.tipos.length === 0" class="tipos-empty">Nenhum tipo adicionado ainda. Clique em "+ Adicionar" para começar.</p>
        </div>
      </form>

      <template #footer>
        <BaseButton variant="ghost" @click="modalOpen = false" id="btn-cancelar-evento">Cancelar</BaseButton>
        <BaseButton variant="primary" @click="handleSave" :loading="saving" id="btn-salvar-evento">
          {{ editingEvento ? 'Salvar alterações' : 'Criar evento' }}
        </BaseButton>
      </template>
    </BaseModal>

    <!-- Delete Confirm Modal -->
    <BaseModal v-model="deleteModalOpen" title="Excluir evento" width="440px">
      <div class="confirm-delete">
        <span class="confirm-icon">⚠️</span>
        <p>Tem certeza que deseja excluir <strong>{{ deletingEvento?.nome }}</strong>?<br />Esta ação não pode ser desfeita.</p>
      </div>
      <template #footer>
        <BaseButton variant="ghost" @click="deleteModalOpen = false" id="btn-cancelar-delete">Cancelar</BaseButton>
        <BaseButton variant="danger" @click="handleDelete" id="btn-confirmar-delete">Excluir</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted } from 'vue'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseModal from '../../components/ui/BaseModal.vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import { useEventosStore } from '../../stores/eventosStore.js'
import { useToast } from '../../composables/useToast.js'
import { api } from '../../services/api.js'

const eventosStore = useEventosStore()
const { success, error: toastError } = useToast()
const pedidos = ref([])

onMounted(async () => {
  eventosStore.carregar()
  try { pedidos.value = await api.pedidos.listarTodos() } catch {}
})

const search = ref('')
const modalOpen = ref(false)
const deleteModalOpen = ref(false)
const editingEvento = ref(null)
const deletingEvento = ref(null)
const saving = ref(false)

const eventosExpirados = computed(() => eventosStore.eventosExpirados)

// Stats de ingressos e lucro por evento (baseado nos pedidos)
const ticketsPorEvento = computed(() => {
  const m = {}
  pedidos.value.forEach(p => {
    m[p.eventoId] = (m[p.eventoId] || 0) + (p.itens?.length || 0)
  })
  return m
})
const lucroEvento = computed(() => {
  const m = {}
  pedidos.value.forEach(p => {
    m[p.eventoId] = (m[p.eventoId] || 0) + (p.valorTotal || 0)
  })
  return m
})
const mediaTicket = computed(() => {
  const m = {}
  Object.keys(lucroEvento.value).forEach(id => {
    const qtd = ticketsPorEvento.value[id] || 1
    m[id] = lucroEvento.value[id] / qtd
  })
  return m
})

function fmtBRL(v) {
  return (v || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

const filteredEventos = computed(() => {
  // Só mostra eventos ativos (não expirados)
  const ativos = eventosStore.eventosAtivos
  if (!search.value) return ativos
  const q = search.value.toLowerCase()
  return ativos.filter(e => e.nome.toLowerCase().includes(q) || e.local.toLowerCase().includes(q))
})

const form = reactive({
  nome: '', categoria: 'Shows', data: '', hora: '20:00',
  local: '', descricao: '', imagem: '', destaque: false, tipos: []
})
const formErrors = reactive({ nome: '', data: '', local: '' })

function openModal(evento = null) {
  editingEvento.value = evento
  Object.keys(formErrors).forEach(k => formErrors[k] = '')

  if (evento) {
    const dt = evento.data ? new Date(evento.data) : null
    const dateStr = dt ? dt.toISOString().slice(0, 10) : ''
    const horaStr = dt ? dt.toTimeString().slice(0, 5) : '20:00'
    Object.assign(form, JSON.parse(JSON.stringify(evento)), { data: dateStr, hora: horaStr })
  } else {
    Object.assign(form, {
      nome: '', categoria: '', data: '', hora: '20:00',
      local: '', descricao: '', imagem: '', destaque: false, tipos: []
    })
  }
  modalOpen.value = true
}

function addTipo() {
  form.tipos.push({ id: `tipo-${Date.now()}`, nome: '', preco: 0, disponivel: 100 })
}

function validate() {
  Object.keys(formErrors).forEach(k => formErrors[k] = '')
  let v = true
  if (!form.nome) { formErrors.nome = 'Obrigatório'; v = false }
  if (!form.data) { formErrors.data = 'Obrigatório'; v = false }
  if (!form.local) { formErrors.local = 'Obrigatório'; v = false }
  return v
}

async function handleSave() {
  if (!validate()) return
  saving.value = true
  try {
    const dataHora = new Date(`${form.data}T${form.hora}:00`).toISOString()
    // Sempre categoria Shows
    const payload = { ...form, categoria: 'Shows', data: dataHora, destaque: form.destaque }
    delete payload.hora
    
    if (editingEvento.value) {
      await eventosStore.editarEvento(editingEvento.value.id, payload)
      success('Evento atualizado com sucesso!')
    } else {
      await eventosStore.criarEvento(payload)
      success('Evento criado com sucesso!')
    }
    modalOpen.value = false
  } catch (e) {
    toastError(e.message)
  } finally {
    saving.value = false
  }
}

function confirmDelete(evento) {
  deletingEvento.value = evento
  deleteModalOpen.value = true
}

async function handleDelete() {
  try {
    await eventosStore.excluirEvento(deletingEvento.value.id)
    success('Evento excluído.')
    deleteModalOpen.value = false
  } catch (e) {
    toastError(e.message)
  }
}

function formatDate(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  return d.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' })
}

</script>

<style scoped>
.admin-eventos { display: flex; flex-direction: column; gap: var(--space-lg); }

.toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-md);
}

.search-input {
  padding: 11px 16px;
  background: var(--clr-surface);
  border: 1.5px solid var(--clr-border);
  border-radius: var(--radius-md);
  color: var(--clr-text);
  font-size: 14px;
  width: 280px;
  transition: all var(--transition-md);
}
.search-input:focus { border-color: var(--clr-primary); outline: none; background: rgba(124, 58, 237, 0.05); }
.search-input::placeholder { color: var(--clr-text-subtle); }

.table-wrapper {
  background: var(--clr-bg-2);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.data-table { width: 100%; border-collapse: collapse; }

.data-table th {
  padding: 14px var(--space-lg);
  text-align: left;
  font-size: 12px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--clr-text-muted);
  background: var(--clr-bg-3);
  border-bottom: 1px solid var(--clr-border);
}

.data-table td {
  padding: 14px var(--space-lg);
  font-size: 14px;
  border-bottom: 1px solid var(--clr-border);
  vertical-align: middle;
}

.data-table tr:last-child td { border-bottom: none; }
.data-table tr:hover td { background: var(--clr-surface); }

.cell-evento { display: flex; align-items: center; gap: var(--space-md); }
.cell-img { width: 48px; height: 36px; border-radius: var(--radius-sm); object-fit: cover; flex-shrink: 0; }
.cell-nome { font-weight: 600; }

.actions { display: flex; gap: 8px; }
.action-btn {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-sm);
  border: 1px solid var(--clr-border);
  background: var(--clr-surface);
  cursor: pointer;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast);
}
.action-btn.edit:hover { border-color: var(--clr-primary); background: rgba(124, 58, 237, 0.1); }
.action-btn.delete:hover { border-color: var(--clr-error); background: rgba(239, 68, 68, 0.1); }

.empty-table { padding: var(--space-3xl); text-align: center; color: var(--clr-text-muted); display: flex; flex-direction: column; align-items: center; gap: var(--space-md); }
.empty-table span { font-size: 40px; }

/* Form */
.form-grid { display: flex; flex-direction: column; gap: var(--space-lg); }
.form-row-2 { display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-md); }

.form-field { display: flex; flex-direction: column; gap: 6px; }
.field-label { font-size: 13px; font-weight: 500; color: var(--clr-text-muted); }
.field-error { font-size: 12px; color: #FCA5A5; }

.field-input {
  padding: 11px 14px;
  background: var(--clr-surface);
  border: 1.5px solid var(--clr-border);
  border-radius: var(--radius-md);
  color: var(--clr-text);
  font-size: 14px;
  transition: all var(--transition-md);
  font-family: var(--font-body);
}
.field-input:focus { border-color: var(--clr-primary); outline: none; background: rgba(124, 58, 237, 0.05); }
.field-input option { background: var(--clr-bg-3); }

textarea.field-input { resize: vertical; min-height: 80px; }

.img-preview { width: 100%; height: 120px; object-fit: cover; border-radius: var(--radius-md); margin-top: 6px; }

/* Toggle */
.toggle-label { display: flex; align-items: center; justify-content: space-between; cursor: pointer; }
.toggle {
  width: 44px;
  height: 24px;
  background: var(--clr-surface-hover);
  border-radius: var(--radius-full);
  position: relative;
  transition: background var(--transition-md);
}
.toggle.on { background: var(--clr-primary); }
.toggle-thumb {
  position: absolute;
  top: 3px;
  left: 3px;
  width: 18px;
  height: 18px;
  background: white;
  border-radius: 50%;
  transition: transform var(--transition-md);
}
.toggle.on .toggle-thumb { transform: translateX(20px); }

/* Tipos */
.tipos-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 8px; }
.btn-add-tipo {
  font-size: 12px;
  color: var(--clr-primary-light);
  background: none;
  border: none;
  cursor: pointer;
  font-weight: 600;
}
.tipo-cols-header {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr auto;
  gap: 8px;
  margin-bottom: 4px;
  padding: 0 2px;
}
.tipo-cols-header span {
  font-size: 11px;
  font-weight: 600;
  color: var(--clr-text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
.tipo-row { display: grid; grid-template-columns: 2fr 1fr 1fr auto; gap: 8px; margin-bottom: 8px; }
.tipos-empty { font-size: 13px; color: var(--clr-text-subtle); font-style: italic; margin: 4px 0 0; }
.tipo-input { }
.btn-rm-tipo {
  width: 36px;
  height: 36px;
  background: rgba(239,68,68,0.1);
  border: 1px solid rgba(239,68,68,0.25);
  border-radius: var(--radius-sm);
  color: #FCA5A5;
  cursor: pointer;
  font-size: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

/* Expirados */
.expired-section {
  margin-top: var(--space-lg);
}
.expired-header {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  margin-bottom: var(--space-md);
  padding: 0 4px;
}
.expired-title {
  font-size: 14px;
  font-weight: 700;
  color: var(--clr-text-muted);
  text-transform: uppercase;
  letter-spacing: 0.08em;
}
.expired-count {
  font-size: 12px;
  background: rgba(239, 68, 68, 0.12);
  color: #FCA5A5;
  border: 1px solid rgba(239, 68, 68, 0.25);
  padding: 2px 10px;
  border-radius: 20px;
  font-weight: 700;
}
.expired-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: var(--space-md);
}
.expired-card {
  background: var(--clr-bg-2);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: var(--radius-lg);
  overflow: hidden;
  opacity: 0.85;
  transition: opacity 0.2s;
}
.expired-card:hover { opacity: 1; }
.expired-img {
  width: 100%;
  height: 120px;
  object-fit: cover;
  filter: grayscale(30%);
}
.expired-info { padding: 14px; }
.expired-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 8px;
}
.expired-badge {
  font-size: 11px;
  font-weight: 800;
  background: rgba(239, 68, 68, 0.12);
  color: #FCA5A5;
  border: 1px solid rgba(239, 68, 68, 0.25);
  padding: 3px 10px;
  border-radius: 20px;
  text-transform: uppercase;
  letter-spacing: 0.06em;
}
.expired-nome {
  font-size: 15px;
  font-weight: 700;
  color: var(--clr-text);
  margin: 0 0 4px;
}
.expired-data {
  font-size: 12px;
  color: var(--clr-text-muted);
  margin: 0 0 12px;
}
.expired-stats {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}
.expired-stat {
  display: flex;
  align-items: center;
  gap: 8px;
  background: var(--clr-surface);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-md);
  padding: 8px 12px;
  flex: 1;
  min-width: 90px;
}
.estat-icon { font-size: 18px; }
.estat-value {
  font-size: 14px;
  font-weight: 800;
  color: var(--clr-text);
  margin: 0;
}
.estat-label {
  font-size: 10px;
  color: var(--clr-text-muted);
  margin: 2px 0 0;
}

/* Confirm delete */
.confirm-delete { display: flex; flex-direction: column; align-items: center; text-align: center; gap: var(--space-md); }
.confirm-icon { font-size: 48px; }
.confirm-delete p { color: var(--clr-text-muted); line-height: 1.6; }
.confirm-delete strong { color: var(--clr-text); }
</style>
