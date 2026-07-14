<template>
  <div class="base-input-wrapper" :class="{ 'has-error': error }">
    <label v-if="label" :for="inputId" class="input-label">{{ label }}</label>
    <div class="input-container">
      <span v-if="$slots.prefix" class="input-prefix">
        <slot name="prefix" />
      </span>
      <input
        :id="inputId"
        v-model="model"
        class="base-input"
        :class="{ 'has-prefix': $slots.prefix, 'has-suffix': $slots.suffix || type === 'password' }"
        :type="computedType"
        :placeholder="placeholder"
        :disabled="disabled"
        :autocomplete="autocomplete"
        v-bind="$attrs"
        @blur="$emit('blur', $event)"
      />
      <button
        v-if="type === 'password'"
        type="button"
        class="input-toggle-pw"
        tabindex="-1"
        @click="showPw = !showPw"
        :aria-label="showPw ? 'Ocultar senha' : 'Mostrar senha'"
      >
        {{ showPw ? '👁' : '👁‍🗨' }}
      </button>
      <span v-else-if="$slots.suffix" class="input-suffix">
        <slot name="suffix" />
      </span>
    </div>
    <p v-if="error" class="input-error">{{ error }}</p>
    <p v-if="hint && !error" class="input-hint">{{ hint }}</p>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  modelValue: { type: [String, Number], default: '' },
  label: String,
  placeholder: String,
  type: { type: String, default: 'text' },
  error: String,
  hint: String,
  disabled: Boolean,
  autocomplete: String,
  id: String
})

const emit = defineEmits(['update:modelValue', 'blur'])

const model = computed({
  get: () => props.modelValue,
  set: val => emit('update:modelValue', val)
})

const showPw = ref(false)
const computedType = computed(() => props.type === 'password' && showPw.value ? 'text' : props.type)
const inputId = computed(() => props.id || `input-${Math.random().toString(36).slice(2, 9)}`)
</script>

<style scoped>
.base-input-wrapper { display: flex; flex-direction: column; gap: 6px; }

.input-label {
  font-size: 13px;
  font-weight: 500;
  color: var(--clr-text-muted);
  letter-spacing: 0.02em;
}

.input-container { position: relative; display: flex; align-items: center; }

.base-input {
  width: 100%;
  padding: 13px 16px;
  background: var(--clr-surface);
  border: 1.5px solid var(--clr-border);
  border-radius: var(--radius-md);
  color: var(--clr-text);
  font-size: 15px;
  transition: all var(--transition-md);
}

.base-input:focus {
  border-color: var(--clr-primary);
  background: rgba(124, 58, 237, 0.05);
  box-shadow: 0 0 0 3px rgba(124, 58, 237, 0.15);
}

.base-input::placeholder { color: var(--clr-text-subtle); }

.base-input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.base-input.has-prefix { padding-left: 44px; }
.base-input.has-suffix { padding-right: 44px; }

.input-prefix, .input-suffix {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  color: var(--clr-text-muted);
  font-size: 14px;
  pointer-events: none;
}

.input-prefix { left: 14px; }
.input-suffix { right: 14px; }

.input-toggle-pw {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  color: var(--clr-text-muted);
  padding: 4px;
  transition: color var(--transition-fast);
}
.input-toggle-pw:hover { color: var(--clr-text); }

.has-error .base-input { border-color: var(--clr-error); }
.has-error .base-input:focus { box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.15); }

.input-error { font-size: 12px; color: #FCA5A5; }
.input-hint { font-size: 12px; color: var(--clr-text-subtle); }
</style>
