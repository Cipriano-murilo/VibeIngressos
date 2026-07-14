<template>
  <div class="event-card" @click="$router.push(`/evento/${evento.id}`)" :id="`event-card-${evento.id}`" role="article" tabindex="0" @keydown.enter="$router.push(`/evento/${evento.id}`)">
    <div class="card-image-wrapper">
      <img :src="evento.imagem" :alt="evento.nome" class="card-image" loading="lazy" />
      <div class="card-image-overlay"></div>
      <span class="card-category">{{ evento.categoria }}</span>
      <span v-if="evento.destaque" class="card-featured">⚡ Destaque</span>
    </div>

    <div class="card-body">
      <h3 class="card-title">{{ evento.nome }}</h3>
      <div class="card-meta">
        <span class="meta-item">
          📅 {{ formatDate(evento.data) }}
        </span>
        <span class="meta-item">
          📍 {{ shortLocal(evento.local) }}
        </span>
      </div>
      <div class="card-footer">
        <div class="card-price">
          <span class="price-label">A partir de</span>
          <span class="price-value">{{ minPrice(evento.tipos) }}</span>
        </div>
        <button class="card-btn" @click.stop="$router.push(`/evento/${evento.id}`)">
          Comprar →
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  evento: { type: Object, required: true }
})

function formatDate(dateStr) {
  const d = new Date(dateStr + 'T00:00:00')
  return d.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' })
}

function shortLocal(local) {
  return local.split(',')[0]
}

function minPrice(tipos) {
  if (!tipos?.length) return 'Grátis'
  const min = Math.min(...tipos.map(t => t.preco))
  return min.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}
</script>

<style scoped>
.event-card {
  background: var(--clr-surface);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-lg);
  overflow: hidden;
  cursor: pointer;
  transition: all var(--transition-md);
  display: flex;
  flex-direction: column;
}

.event-card:hover {
  border-color: rgba(124, 58, 237, 0.4);
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg), var(--shadow-glow);
}

.event-card:focus { outline: 2px solid var(--clr-primary); outline-offset: 2px; }

.card-image-wrapper {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.card-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform var(--transition-slow);
}

.event-card:hover .card-image { transform: scale(1.06); }

.card-image-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(13,13,18,0.8) 0%, transparent 60%);
}

.card-category {
  position: absolute;
  top: 12px;
  left: 12px;
  background: rgba(13,13,18,0.7);
  backdrop-filter: blur(8px);
  border: 1px solid var(--clr-border);
  color: var(--clr-text);
  font-size: 11px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: var(--radius-full);
  letter-spacing: 0.04em;
}

.card-featured {
  position: absolute;
  top: 12px;
  right: 12px;
  background: var(--grad-primary);
  color: white;
  font-size: 11px;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: var(--radius-full);
}

.card-body { padding: var(--space-lg); display: flex; flex-direction: column; gap: var(--space-sm); flex: 1; }

.card-title {
  font-size: 17px;
  font-weight: 700;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-meta { display: flex; flex-direction: column; gap: 4px; }
.meta-item { font-size: 13px; color: var(--clr-text-muted); }

.card-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: auto;
  padding-top: var(--space-sm);
  border-top: 1px solid var(--clr-border);
}

.card-price { display: flex; flex-direction: column; }
.price-label { font-size: 11px; color: var(--clr-text-subtle); }
.price-value { font-size: 18px; font-weight: 800; background: var(--grad-text); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }

.card-btn {
  background: var(--grad-primary);
  color: white;
  border: none;
  padding: 8px 18px;
  border-radius: var(--radius-md);
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all var(--transition-md);
  box-shadow: 0 3px 10px rgba(124, 58, 237, 0.3);
}
.card-btn:hover { box-shadow: 0 5px 18px rgba(124, 58, 237, 0.5); transform: translateY(-1px); }
</style>
