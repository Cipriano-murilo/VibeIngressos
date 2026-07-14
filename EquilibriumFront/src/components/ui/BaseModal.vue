<template>
  <teleport to="body">
    <transition name="modal">
      <div v-if="modelValue" class="modal-overlay" @click.self="$emit('update:modelValue', false)" role="dialog" :aria-modal="true" :aria-labelledby="titleId">
        <div class="modal-panel" :style="{ maxWidth: width }">
          <!-- Header -->
          <div class="modal-header">
            <h2 :id="titleId" class="modal-title">{{ title }}</h2>
            <button class="modal-close" @click="$emit('update:modelValue', false)" aria-label="Fechar modal">✕</button>
          </div>

          <!-- Body -->
          <div class="modal-body">
            <slot />
          </div>

          <!-- Footer -->
          <div v-if="$slots.footer" class="modal-footer">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  modelValue: Boolean,
  title: { type: String, default: '' },
  width: { type: String, default: '560px' }
})

defineEmits(['update:modelValue'])

const titleId = computed(() => `modal-title-${Math.random().toString(36).slice(2, 7)}`)
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(6px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: var(--space-lg);
}

.modal-panel {
  background: var(--clr-bg-3);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-xl);
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: var(--shadow-lg);
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-lg) var(--space-xl);
  border-bottom: 1px solid var(--clr-border);
  position: sticky;
  top: 0;
  background: var(--clr-bg-3);
  z-index: 1;
  border-radius: var(--radius-xl) var(--radius-xl) 0 0;
}

.modal-title {
  font-size: 18px;
  font-weight: 700;
  background: var(--grad-text);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.modal-close {
  background: var(--clr-surface);
  border: 1px solid var(--clr-border);
  color: var(--clr-text-muted);
  width: 32px;
  height: 32px;
  border-radius: var(--radius-sm);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  font-size: 12px;
  transition: all var(--transition-fast);
}
.modal-close:hover { background: var(--clr-surface-hover); color: var(--clr-text); }

.modal-body { padding: var(--space-xl); }

.modal-footer {
  padding: var(--space-lg) var(--space-xl);
  border-top: 1px solid var(--clr-border);
  display: flex;
  gap: var(--space-sm);
  justify-content: flex-end;
}

/* Transition */
.modal-enter-active, .modal-leave-active {
  transition: opacity 0.25s ease;
}
.modal-enter-active .modal-panel, .modal-leave-active .modal-panel {
  transition: transform 0.25s ease, opacity 0.25s ease;
}
.modal-enter-from, .modal-leave-to { opacity: 0; }
.modal-enter-from .modal-panel, .modal-leave-to .modal-panel {
  transform: scale(0.95) translateY(10px);
  opacity: 0;
}
</style>
