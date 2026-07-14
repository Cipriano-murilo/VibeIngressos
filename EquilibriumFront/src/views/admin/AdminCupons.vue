<template>
  <div class="admin-cupons">
    <div class="toolbar">
      <input v-model="search" type="search" placeholder="Buscar cupom..." class="search-input" id="search-cupons" />
      <BaseButton id="btn-novo-cupom" variant="primary" @click="openModal()">
        🎟️ Novo cupom
      </BaseButton>
    </div>

    <!-- Stats row -->
    <div class="cupons-stats">
      <div class="cupons-stat-card">
        <p class="cstat-label">Total de cupons</p>
        <p class="cstat-value">{{ cuponsStore.cupons.length }}</p>
      </div>
      <div class="cupons-stat-card">
        <p class="cstat-label">Ativos</p>
        <p class="cstat-value success">{{ cuponsStore.cupons.filter(c => c.ativo && new Date(c.validoAte) >= new Date()).length }}</p>
      </div>
      <div class="cupons-stat-card">
        <p class="cstat-label">Usos totais</p>
        <p class="cstat-value">{{ cuponsStore.cupons.reduce((s, c) => s + c.usosAtuais, 0) }}</p>
      </div>
      <div class="cupons-stat-card">
        <p class="cstat-label">Expirados/Inativos</p>
        <p class="cstat-value warning">{{ cuponsStore.cupons.filter(c => !c.ativo || new Date(c.validoAte) < new Date()).length }}</p>
      </div>
    </div>

    <!-- Table -->
    <div class="table-wrapper">
      <table class="data-table" aria-label="Lista de cupons">
        <thead>
          <tr>
            <th>Código</th>
            <th>Desconto</th>
            <th>Tipo</th>
            <th>Válido até</th>
            <th>Uso</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="cupom in filteredCupons" :key="cupom.id" :id="`row-cupom-${cupom.id}`">
            <td><code class="cupom-code">{{ cupom.codigo }}</code></td>
            <td class="cupom-desconto">
              {{ cupom.tipo === 'percentual' ? cupom.desconto + '%' : 'R$ ' + cupom.desconto.toFixed(2) }}
            </td>
            <td>
              <BaseBadge :variant="cupom.tipo === 'percentual' ? 'info' : 'warning'">
                {{ cupom.tipo === 'percentual' ? 'Percentual' : 'Valor fixo' }}
              </BaseBadge>
            </td>
            <td :class="isExpired(cupom.validoAte) ? 'expired-date' : ''">
              {{ formatDate(cupom.validoAte) }}
            </td>
            <td>
              <div class="uso-bar-wrapper">
                <div class="uso-bar">
                  <div class="uso-bar-fill" :style="{ width: Math.min((cupom.usosAtuais / cupom.usoMaximo) * 100, 100) + '%' }"></div>
                </div>
                <span class="uso-text">{{ cupom.usosAtuais }}/{{ cupom.usoMaximo }}</span>
              </div>
            </td>
            <td>
              <BaseBadge :variant="getStatusVariant(cupom)">{{ getStatus(cupom) }}</BaseBadge>
            </td>
            <td>
              <div class="actions">
                <button class="action-btn edit" @click="openModal(cupom)" :id="`btn-editar-cupom-${cupom.id}`" title="Editar">✏️</button>
                <button class="action-btn delete" @click="confirmDelete(cupom)" :id="`btn-excluir-cupom-${cupom.id}`" title="Excluir">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-if="filteredCupons.length === 0" class="empty-table">
        <span>🎟️</span>
        <p>Nenhum cupom encontrado.</p>
      </div>
    </div>

    <!-- Modal Criar/Editar -->
    <BaseModal v-model="modalOpen" :title="editingCupom ? 'Editar cupom' : 'Criar novo cupom'" width="500px">
      <form @submit.prevent="handleSave" id="form-cupom" class="form-grid" novalidate>
        <div class="form-field">
          <label class="field-label" for="cupom-codigo">Código do cupom *</label>
          <input id="cupom-codigo" v-model="form.codigo" type="text" class="field-input code-input" placeholder="EX: VERAO2026" style="text-transform: uppercase;" />
          <p v-if="formErrors.codigo" class="field-error">{{ formErrors.codigo }}</p>
        </div>

        <div class="form-row-2">
          <div class="form-field">
            <label class="field-label" for="cupom-tipo">Tipo de desconto *</label>
            <select id="cupom-tipo" v-model="form.tipo" class="field-input">
              <option value="percentual">Percentual (%)</option>
              <option value="fixo">Valor fixo (R$)</option>
            </select>
          </div>
          <div class="form-field">
            <label class="field-label" for="cupom-desconto">
              Valor do desconto {{ form.tipo === 'percentual' ? '(%)' : '(R$)' }} *
            </label>
            <input id="cupom-desconto" v-model.number="form.desconto" type="number" class="field-input" :placeholder="form.tipo === 'percentual' ? '0-100' : '0.00'" min="0" :max="form.tipo === 'percentual' ? 100 : undefined" step="0.01" />
            <p v-if="formErrors.desconto" class="field-error">{{ formErrors.desconto }}</p>
          </div>
        </div>

        <div class="form-row-2">
          <div class="form-field">
            <label class="field-label" for="cupom-validade">Válido até *</label>
            <input id="cupom-validade" v-model="form.validoAte" type="date" class="field-input" />
            <p v-if="formErrors.validoAte" class="field-error">{{ formErrors.validoAte }}</p>
          </div>
          <div class="form-field">
            <label class="field-label" for="cupom-uso-max">Limite de usos *</label>
            <input id="cupom-uso-max" v-model.number="form.usoMaximo" type="number" class="field-input" placeholder="100" min="1" />
          </div>
        </div>

        <div class="form-field">
          <label class="toggle-label">
            <span class="field-label">Cupom ativo</span>
            <div class="toggle" :class="{ on: form.ativo }" @click="form.ativo = !form.ativo" id="toggle-ativo" role="switch" :aria-checked="form.ativo">
              <div class="toggle-thumb"></div>
            </div>
          </label>
        </div>
      </form>

      <template #footer>
        <BaseButton variant="ghost" @click="modalOpen = false" id="btn-cancelar-cupom">Cancelar</BaseButton>
        <BaseButton variant="primary" @click="handleSave" :loading="saving" id="btn-salvar-cupom">
          {{ editingCupom ? 'Salvar alterações' : 'Criar cupom' }}
        </BaseButton>
      </template>
    </BaseModal>

    <!-- Delete Confirm -->
    <BaseModal v-model="deleteModalOpen" title="Excluir cupom" width="400px">
      <div class="confirm-delete">
        <span class="confirm-icon">⚠️</span>
        <p>Excluir o cupom <strong>{{ deletingCupom?.codigo }}</strong>?</p>
      </div>
      <template #footer>
        <BaseButton variant="ghost" @click="deleteModalOpen = false" id="btn-cancelar-del-cupom">Cancelar</BaseButton>
        <BaseButton variant="danger" @click="handleDelete" id="btn-confirmar-del-cupom">Excluir</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted } from 'vue'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseModal from '../../components/ui/BaseModal.vue'
import BaseBadge from '../../components/ui/BaseBadge.vue'
import { useCuponsStore } from '../../stores/cuponsStore.js'
import { useToast } from '../../composables/useToast.js'

const cuponsStore = useCuponsStore()
const { success, error: toastError } = useToast()

// Carrega cupons do backend ao abrir a página
onMounted(() => cuponsStore.carregar())

const search = ref('')
const modalOpen = ref(false)
const deleteModalOpen = ref(false)
const editingCupom = ref(null)
const deletingCupom = ref(null)
const saving = ref(false)

const filteredCupons = computed(() => {
  if (!search.value) return cuponsStore.cupons
  const q = search.value.toLowerCase()
  return cuponsStore.cupons.filter(c => c.codigo.toLowerCase().includes(q))
})

const form = reactive({ codigo: '', tipo: 'percentual', desconto: 0, validoAte: '', usoMaximo: 100, ativo: true })
const formErrors = reactive({ codigo: '', desconto: '', validoAte: '' })

function openModal(cupom = null) {
  editingCupom.value = cupom
  Object.keys(formErrors).forEach(k => formErrors[k] = '')

  if (cupom) {
    // Converte validoAte para YYYY-MM-DD para o input type=date
    const validoStr = cupom.validoAte ? new Date(cupom.validoAte).toISOString().slice(0, 10) : ''
    Object.assign(form, { ...cupom, validoAte: validoStr })
  } else {
    Object.assign(form, { codigo: '', tipo: 'percentual', desconto: 0, validoAte: '', usoMaximo: 100, ativo: true })
  }
  modalOpen.value = true
}

function validate() {
  Object.keys(formErrors).forEach(k => formErrors[k] = '')
  let v = true
  if (!form.codigo.trim()) { formErrors.codigo = 'Obrigatório'; v = false }
  if (!form.desconto || form.desconto <= 0) { formErrors.desconto = 'Informe um valor válido'; v = false }
  if (!form.validoAte) { formErrors.validoAte = 'Obrigatório'; v = false }
  return v
}

async function handleSave() {
  if (!validate()) return
  saving.value = true
  try {
    const data = { 
      codigo: form.codigo.toUpperCase(),
      tipoDesconto: form.tipo,
      valorDesconto: form.desconto,
      limiteUsos: form.usoMaximo,
      validoAte: form.validoAte,
      ativo: form.ativo
    }
    
    if (editingCupom.value) {
      await cuponsStore.editarCupom(editingCupom.value.id, data)
      success('Cupom atualizado!')
    } else {
      await cuponsStore.criarCupom(data)
      success('Cupom criado!')
    }
    modalOpen.value = false
  } catch (e) {
    toastError(e.message)
  } finally {
    saving.value = false
  }
}

function confirmDelete(cupom) {
  deletingCupom.value = cupom
  deleteModalOpen.value = true
}

async function handleDelete() {
  try {
    await cuponsStore.excluirCupom(deletingCupom.value.id)
    success('Cupom excluído.')
    deleteModalOpen.value = false
  } catch (e) {
    toastError(e.message)
  }
}

function isExpired(dateStr) { return dateStr ? new Date(dateStr) < new Date() : false }
function formatDate(dateStr) {
  if (!dateStr) return '—'
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return '—'
  return d.toLocaleDateString('pt-BR')
}
function getStatus(cupom) {
  if (!cupom.ativo) return 'Inativo'
  if (new Date(cupom.validoAte) < new Date()) return 'Expirado'
  if (cupom.usosAtuais >= cupom.usoMaximo) return 'Esgotado'
  return 'Ativo'
}
function getStatusVariant(cupom) {
  const s = getStatus(cupom)
  return { Ativo: 'success', Expirado: 'error', Inativo: 'default', Esgotado: 'warning' }[s]
}
</script>

<style scoped>
.admin-cupons { display: flex; flex-direction: column; gap: var(--space-lg); }

.toolbar { display: flex; align-items: center; justify-content: space-between; gap: var(--space-md); }

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
.search-input:focus { border-color: var(--clr-primary); outline: none; }
.search-input::placeholder { color: var(--clr-text-subtle); }

/* Stats */
.cupons-stats { display: grid; grid-template-columns: repeat(4, 1fr); gap: var(--space-md); }
.cupons-stat-card {
  background: var(--clr-bg-2);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-md);
  padding: var(--space-lg);
  text-align: center;
}
.cstat-label { font-size: 12px; color: var(--clr-text-muted); text-transform: uppercase; letter-spacing: 0.06em; margin-bottom: 6px; }
.cstat-value { font-size: 28px; font-weight: 900; font-family: var(--font-display); }
.cstat-value.success { background: var(--grad-text); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }
.cstat-value.warning { color: #FCD34D; }

/* Table */
.table-wrapper { background: var(--clr-bg-2); border: 1px solid var(--clr-border); border-radius: var(--radius-lg); overflow: hidden; }
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

.cupom-code { font-family: monospace; font-size: 14px; font-weight: 700; color: var(--clr-primary-light); background: rgba(124, 58, 237, 0.1); padding: 3px 8px; border-radius: 4px; }
.cupom-desconto { font-size: 16px; font-weight: 800; color: #6EE7B7; }
.expired-date { color: var(--clr-error) !important; }

.uso-bar-wrapper { display: flex; align-items: center; gap: 8px; }
.uso-bar { flex: 1; height: 4px; background: var(--clr-surface-hover); border-radius: var(--radius-full); overflow: hidden; min-width: 60px; }
.uso-bar-fill { height: 100%; background: var(--grad-primary); border-radius: var(--radius-full); }
.uso-text { font-size: 12px; color: var(--clr-text-muted); white-space: nowrap; }

.actions { display: flex; gap: 8px; }
.action-btn {
  width: 32px; height: 32px;
  border-radius: var(--radius-sm);
  border: 1px solid var(--clr-border);
  background: var(--clr-surface);
  cursor: pointer; font-size: 14px;
  display: flex; align-items: center; justify-content: center;
  transition: all var(--transition-fast);
}
.action-btn.edit:hover { border-color: var(--clr-primary); background: rgba(124, 58, 237, 0.1); }
.action-btn.delete:hover { border-color: var(--clr-error); background: rgba(239, 68, 68, 0.1); }

.empty-table { padding: var(--space-3xl); text-align: center; color: var(--clr-text-muted); display: flex; flex-direction: column; align-items: center; gap: var(--space-md); }
.empty-table span { font-size: 40px; }

/* Form */
.form-grid { display: flex; flex-direction: column; gap: var(--space-md); }
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
.code-input { font-family: monospace; font-weight: 700; letter-spacing: 0.08em; font-size: 15px; }

.toggle-label { display: flex; align-items: center; justify-content: space-between; cursor: pointer; }
.toggle { width: 44px; height: 24px; background: var(--clr-surface-hover); border-radius: var(--radius-full); position: relative; transition: background var(--transition-md); cursor: pointer; }
.toggle.on { background: var(--clr-primary); }
.toggle-thumb { position: absolute; top: 3px; left: 3px; width: 18px; height: 18px; background: white; border-radius: 50%; transition: transform var(--transition-md); }
.toggle.on .toggle-thumb { transform: translateX(20px); }

.confirm-delete { display: flex; flex-direction: column; align-items: center; text-align: center; gap: var(--space-md); }
.confirm-icon { font-size: 48px; }
.confirm-delete p { color: var(--clr-text-muted); }
.confirm-delete strong { color: var(--clr-text); }

@media (max-width: 768px) { .cupons-stats { grid-template-columns: repeat(2, 1fr); } }
</style>
