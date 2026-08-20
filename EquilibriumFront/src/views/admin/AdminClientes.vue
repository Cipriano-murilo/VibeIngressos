<template>
  <div class="admin-clientes">
    <div class="toolbar">
      <input v-model="search" type="search" placeholder="Buscar cliente..." class="search-input" id="search-clientes" />
    </div>

    <div class="table-wrapper">
      <table class="data-table" aria-label="Lista de clientes">
        <thead>
          <tr>
            <th>Cliente</th>
            <th>E-mail</th>
            <th>CPF</th>
            <th>Celular</th>
            <th>Nascimento</th>
            <th>Perfil</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="cliente in filteredClientes" :key="cliente.id" :id="`row-cliente-${cliente.id}`">
            <td>
              <div class="cell-cliente">
                <div class="cell-avatar-initial">{{ cliente.nome?.charAt(0).toUpperCase() }}</div>
                <span class="cell-nome">{{ cliente.nome }}</span>
              </div>
            </td>
            <td>{{ cliente.email }}</td>
            <td><code class="cell-code">{{ cliente.cpf || '—' }}</code></td>
            <td>{{ cliente.celular || '—' }}</td>
            <td>{{ cliente.dataNascimento ? formatDate(cliente.dataNascimento) : '—' }}</td>
            <td>
              <span class="role-badge" :class="cliente.role === 'admin' ? 'role-admin' : 'role-client'">
                {{ cliente.role === 'admin' ? '👑 Admin' : '👤 Cliente' }}
              </span>
            </td>
            <td>
              <div class="actions">
                <button class="action-btn edit" @click="openModal(cliente)" :id="`btn-editar-cliente-${cliente.id}`" title="Editar">✏️</button>
                <button
                  class="action-btn role-toggle"
                  @click="confirmRole(cliente)"
                  :id="`btn-role-${cliente.id}`"
                  :title="cliente.role === 'admin' ? 'Rebaixar para Cliente' : 'Promover a Admin'"
                >
                  {{ cliente.role === 'admin' ? '🔴' : '🟢' }}
                </button>
                <button class="action-btn delete" @click="confirmDelete(cliente)" :id="`btn-excluir-cliente-${cliente.id}`" title="Excluir">🗑️</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="filteredClientes.length === 0" class="empty-table">
        <span>👥</span>
        <p>Nenhum cliente encontrado.</p>
      </div>
    </div>

    <!-- Edit Modal -->
    <BaseModal v-model="modalOpen" title="Editar perfil do cliente" width="540px">
      <form @submit.prevent="handleSave" id="form-cliente" class="form-grid" novalidate>
        <div class="form-field">
          <label class="field-label" for="cliente-nome">Nome completo *</label>
          <input id="cliente-nome" v-model="form.nome" type="text" class="field-input" placeholder="Nome completo" />
          <p v-if="formErrors.nome" class="field-error">{{ formErrors.nome }}</p>
        </div>

        <div class="form-row-2">
          <div class="form-field">
            <label class="field-label" for="cliente-email">E-mail *</label>
            <input id="cliente-email" v-model="form.email" type="email" class="field-input" placeholder="email@exemplo.com" />
            <p v-if="formErrors.email" class="field-error">{{ formErrors.email }}</p>
          </div>
          <div class="form-field">
            <label class="field-label" for="cliente-celular">Celular</label>
            <input id="cliente-celular" v-model="form.celular" type="tel" class="field-input" placeholder="(11) 99999-9999" />
          </div>
        </div>

        <div class="form-row-2">
          <div class="form-field">
            <label class="field-label" for="cliente-cpf">CPF</label>
            <input id="cliente-cpf" v-model="form.cpf" type="text" class="field-input" placeholder="000.000.000-00" maxlength="14" />
          </div>
          <div class="form-field">
            <label class="field-label" for="cliente-nascimento">Data de nascimento</label>
            <input id="cliente-nascimento" v-model="form.dataNascimento" type="date" class="field-input" />
          </div>
        </div>
      </form>

      <template #footer>
        <BaseButton variant="ghost" @click="modalOpen = false" id="btn-cancelar-cliente">Cancelar</BaseButton>
        <BaseButton variant="primary" @click="handleSave" :loading="saving" id="btn-salvar-cliente">Salvar alterações</BaseButton>
      </template>
    </BaseModal>

    <!-- Delete Confirm -->
    <BaseModal v-model="deleteModalOpen" title="Remover cliente" width="420px">
      <div class="confirm-delete">
        <span class="confirm-icon">⚠️</span>
        <p>Tem certeza que deseja remover o cliente <strong>{{ deletingCliente?.nome }}</strong>?</p>
      </div>
      <template #footer>
        <BaseButton variant="ghost" @click="deleteModalOpen = false" id="btn-cancelar-delete-cliente">Cancelar</BaseButton>
        <BaseButton variant="danger" @click="handleDelete" id="btn-confirmar-delete-cliente">Remover</BaseButton>
      </template>
    </BaseModal>

    <!-- Role Toggle Confirm -->
    <BaseModal v-model="roleModalOpen" title="Alterar perfil do usuário" width="440px">
      <div class="confirm-delete">
        <span class="confirm-icon">{{ roleCliente?.role === 'admin' ? '🔴' : '🟢' }}</span>
        <p v-if="roleCliente?.role === 'admin'">
          Tem certeza que deseja <strong>rebaixar</strong> o usuário <strong>{{ roleCliente?.nome }}</strong> para <strong>Cliente</strong>?
        </p>
        <p v-else>
          Tem certeza que deseja <strong>promover</strong> o cliente <strong>{{ roleCliente?.nome }}</strong> a <strong>Administrador</strong>?
          <br/><small style="color: #FCA5A5">⚠️ Administradores têm acesso total ao painel.</small>
        </p>
      </div>
      <template #footer>
        <BaseButton variant="ghost" @click="roleModalOpen = false" id="btn-cancelar-role">Cancelar</BaseButton>
        <BaseButton
          :variant="roleCliente?.role === 'admin' ? 'danger' : 'primary'"
          @click="handleAlternarRole"
          :loading="savingRole"
          id="btn-confirmar-role"
        >
          {{ roleCliente?.role === 'admin' ? 'Rebaixar para Cliente' : 'Promover a Admin' }}
        </BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted } from 'vue'
import BaseButton from '../../components/ui/BaseButton.vue'
import BaseModal from '../../components/ui/BaseModal.vue'
import { useClientesStore } from '../../stores/clientesStore.js'
import { useToast } from '../../composables/useToast.js'

const clientesStore = useClientesStore()
const { success, error: toastError } = useToast()

// Carrega clientes do backend ao abrir a página
onMounted(() => clientesStore.carregar())

const search = ref('')
const modalOpen = ref(false)
const deleteModalOpen = ref(false)
const editingCliente = ref(null)
const deletingCliente = ref(null)
const saving = ref(false)
const roleModalOpen = ref(false)
const roleCliente = ref(null)
const savingRole = ref(false)

const filteredClientes = computed(() => {
  if (!search.value) return clientesStore.clientes
  const q = search.value.toLowerCase()
  return clientesStore.clientes.filter(c =>
    c.nome?.toLowerCase().includes(q) ||
    c.email?.toLowerCase().includes(q) ||
    c.cpf?.includes(q)
  )
})

const form = reactive({ nome: '', email: '', celular: '', cpf: '', dataNascimento: '' })
const formErrors = reactive({ nome: '', email: '' })

function openModal(cliente) {
  editingCliente.value = cliente
  // Prepara dataNascimento: input type=date precisa de YYYY-MM-DD
  const dnRaw = cliente.dataNascimento
  let dnFormatted = ''
  if (dnRaw) {
    const d = new Date(dnRaw)
    if (!isNaN(d.getTime())) {
      dnFormatted = d.toISOString().slice(0, 10)
    }
  }
  Object.assign(form, { nome: '', email: '', celular: '', cpf: '', dataNascimento: '', ...cliente, dataNascimento: dnFormatted })
  formErrors.nome = ''
  formErrors.email = ''
  modalOpen.value = true
}

function validate() {
  formErrors.nome = form.nome ? '' : 'Obrigatório'
  formErrors.email = form.email && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email) ? '' : 'E-mail inválido'
  return !formErrors.nome && !formErrors.email
}

async function handleSave() {
  if (!validate()) return
  saving.value = true
  try {
    await clientesStore.editarCliente(editingCliente.value.id, { ...form })
    success('Perfil do cliente atualizado!')
    modalOpen.value = false
  } catch (e) {
    toastError(e.message)
  } finally {
    saving.value = false
  }
}

function confirmDelete(cliente) {
  deletingCliente.value = cliente
  deleteModalOpen.value = true
}

async function handleDelete() {
  try {
    await clientesStore.excluirCliente(deletingCliente.value.id)
    success('Cliente removido.')
    deleteModalOpen.value = false
  } catch (e) {
    toastError(e.message)
  }
}

function confirmRole(cliente) {
  roleCliente.value = cliente
  roleModalOpen.value = true
}

async function handleAlternarRole() {
  savingRole.value = true
  try {
    const atualizado = await clientesStore.alternarRole(roleCliente.value.id)
    const novoRole = atualizado.role === 'admin' ? 'Administrador' : 'Cliente'
    success(`Perfil atualizado para ${novoRole} com sucesso!`)
    roleModalOpen.value = false
  } catch (e) {
    toastError(e.message)
  } finally {
    savingRole.value = false
  }
}

function formatDate(dateStr) {
  if (!dateStr) return '—'
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return '—'
  return d.toLocaleDateString('pt-BR')
}
</script>

<style scoped>
.admin-clientes { display: flex; flex-direction: column; gap: var(--space-lg); }

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
  padding: 12px var(--space-lg);
  font-size: 14px;
  border-bottom: 1px solid var(--clr-border);
  vertical-align: middle;
}
.data-table tr:last-child td { border-bottom: none; }
.data-table tr:hover td { background: var(--clr-surface); }

.cell-cliente { display: flex; align-items: center; gap: var(--space-md); }
.cell-avatar-initial {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: var(--grad-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 15px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
  border: 2px solid var(--clr-border);
}
.cell-nome { font-weight: 600; }
.cell-code { font-family: monospace; font-size: 13px; color: var(--clr-text-muted); background: var(--clr-surface); padding: 2px 6px; border-radius: 4px; }

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
.action-btn.role-toggle:hover { border-color: #F59E0B; background: rgba(245, 158, 11, 0.1); }

.role-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 700;
  white-space: nowrap;
}
.role-admin {
  background: rgba(124, 58, 237, 0.15);
  color: #A78BFA;
  border: 1px solid rgba(124, 58, 237, 0.3);
}
.role-client {
  background: rgba(100, 116, 139, 0.12);
  color: #94A3B8;
  border: 1px solid rgba(100, 116, 139, 0.2);
}

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

/* Confirm */
.confirm-delete { display: flex; flex-direction: column; align-items: center; text-align: center; gap: var(--space-md); }
.confirm-icon { font-size: 48px; }
.confirm-delete p { color: var(--clr-text-muted); line-height: 1.6; }
.confirm-delete strong { color: var(--clr-text); }
</style>