<template>
  <button
    :id="id"
    class="base-btn"
    :class="[`btn-${variant}`, `btn-${size}`, { 'btn-full': full, 'btn-loading': loading }]"
    :type="type"
    :disabled="disabled || loading"
    v-bind="$attrs"
  >
    <span v-if="loading" class="btn-spinner" aria-hidden="true"></span>
    <slot v-else />
  </button>
</template>

<script setup>
defineProps({
  variant: { type: String, default: 'primary' }, // primary | secondary | ghost | danger | outline
  size: { type: String, default: 'md' },          // sm | md | lg
  type: { type: String, default: 'button' },
  full: Boolean,
  loading: Boolean,
  disabled: Boolean,
  id: { type: String, default: () => `btn-${Math.random().toString(36).slice(2, 9)}` }
})
</script>

<style scoped>
.base-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  font-family: var(--font-body);
  font-weight: 600;
  border: none;
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all var(--transition-md);
  white-space: nowrap;
  position: relative;
  overflow: hidden;
}

.base-btn::after {
  content: '';
  position: absolute;
  inset: 0;
  background: rgba(255,255,255,0);
  transition: background var(--transition-fast);
}

.base-btn:hover::after { background: rgba(255,255,255,0.08); }
.base-btn:active { transform: scale(0.97); }
.base-btn:disabled { opacity: 0.5; cursor: not-allowed; transform: none; }

/* Sizes */
.btn-sm { padding: 8px 16px; font-size: 13px; border-radius: var(--radius-sm); }
.btn-md { padding: 12px 24px; font-size: 15px; }
.btn-lg { padding: 16px 32px; font-size: 16px; border-radius: var(--radius-lg); }

/* Variants */
.btn-primary {
  background: var(--grad-primary);
  color: white;
  box-shadow: 0 4px 15px rgba(124, 58, 237, 0.35);
}
.btn-primary:hover:not(:disabled) {
  box-shadow: 0 6px 25px rgba(124, 58, 237, 0.55);
  transform: translateY(-1px);
}

.btn-secondary {
  background: var(--clr-surface-hover);
  color: var(--clr-text);
  border: 1px solid var(--clr-border);
}
.btn-secondary:hover:not(:disabled) {
  border-color: var(--clr-primary);
  color: var(--clr-primary-light);
}

.btn-ghost {
  background: transparent;
  color: var(--clr-text-muted);
}
.btn-ghost:hover:not(:disabled) { color: var(--clr-text); background: var(--clr-surface); }

.btn-outline {
  background: transparent;
  color: var(--clr-primary-light);
  border: 1.5px solid var(--clr-primary);
}
.btn-outline:hover:not(:disabled) {
  background: rgba(124, 58, 237, 0.1);
}

.btn-danger {
  background: rgba(239, 68, 68, 0.15);
  color: #FCA5A5;
  border: 1px solid rgba(239, 68, 68, 0.3);
}
.btn-danger:hover:not(:disabled) {
  background: rgba(239, 68, 68, 0.25);
}

.btn-full { width: 100%; }

/* Spinner */
.btn-spinner {
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}
</style>
