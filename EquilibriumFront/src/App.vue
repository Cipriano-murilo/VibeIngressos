<template>
  <router-view v-slot="{ Component, route }">
    <transition name="page" mode="out-in">
      <component :is="Component" :key="route.path" />
    </transition>
  </router-view>

  <!-- Toast notifications global -->
  <div class="toast-container" role="alert" aria-live="polite">
    <div
      v-for="toast in toasts"
      :key="toast.id"
      class="toast"
      :class="toast.type"
      @click="dismiss(toast.id)"
    >
      <span class="toast-icon">
        {{ toast.type === 'success' ? '✓' : toast.type === 'error' ? '✕' : 'ℹ' }}
      </span>
      {{ toast.message }}
    </div>
  </div>
</template>

<script setup>
import { useToast } from './composables/useToast.js'
const { toasts, dismiss } = useToast()
</script>

<style>
.toast-icon {
  font-weight: 700;
  font-size: 14px;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  flex-shrink: 0;
}
</style>
