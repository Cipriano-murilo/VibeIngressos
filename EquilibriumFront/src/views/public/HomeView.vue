<template>
  <div class="home-page">
    <AppHeader />

    <!-- Hero Section -->
    <section class="hero-section">
      <div class="hero-bg">
        <div class="hero-orb hero-orb-1"></div>
        <div class="hero-orb hero-orb-2"></div>
        <div class="hero-orb hero-orb-3"></div>
      </div>

      <div class="container hero-content">
        <div class="hero-text animate-fade-in">
          <div class="hero-eyebrow">🎉 Top #1 em festa rave do ES</div>
          <h1 class="hero-title">
            Viva o melhor dos<br />
            <span class="gradient-text">eventos ao vivo</span>
          </h1>
          <p class="hero-subtitle">
            Shows, festivais, teatro, esportes e muito mais. Encontre os melhores eventos
            e garanta seu ingresso com total segurança.
          </p>
          <div class="hero-actions">
            <button class="btn-hero-primary" id="btn-hero-explorar" @click="scrollToEventos">
              Explorar eventos
            </button>
            <router-link to="/cadastro" id="btn-hero-criar-conta">
              <button class="btn-hero-secondary">Criar conta grátis →</button>
            </router-link>
          </div>
          <div class="hero-stats">
            <div class="hero-stat">
              <span class="hstat-num">+2.400</span>
              <span class="hstat-label">Eventos realizados</span>
            </div>
            <div class="hero-stat-divider"></div>
            <div class="hero-stat">
              <span class="hstat-num">+180K</span>
              <span class="hstat-label">Ingressos vendidos</span>
            </div>
            <div class="hero-stat-divider"></div>
            <div class="hero-stat">
              <span class="hstat-num">+340</span>
              <span class="hstat-label">Organizadores</span>
            </div>
          </div>
        </div>

        <!-- Featured event card -->
        <div class="hero-featured animate-float" v-if="eventosDestaque[0]" @click="$router.push(`/evento/${eventosDestaque[0].id}`)" id="hero-featured-event">
          <div class="featured-img-wrapper">
            <img :src="eventosDestaque[0].imagem" :alt="eventosDestaque[0].nome" class="featured-img" />
            <div class="featured-overlay"></div>
          </div>
          <div class="featured-info">
            <span class="featured-badge">⚡ Em destaque</span>
            <h3 class="featured-title">{{ eventosDestaque[0].nome }}</h3>
            <p class="featured-meta">📅 {{ formatDate(eventosDestaque[0].data) }} · {{ eventosDestaque[0].local.split(',')[0] }}</p>
            <button class="featured-btn" id="btn-featured-comprar">Ver ingressos</button>
          </div>
        </div>
      </div>
    </section>

    <!-- Events Section -->
    <section id="eventos" class="events-section">
      <div class="container">
        <div class="section-header">
          <div>
            <p class="section-eyebrow">Confira os próximos</p>
            <h2 class="section-title">Eventos <span class="gradient-text">disponíveis</span></h2>
          </div>
          <!-- Category filter -->
          <div class="category-filter" role="group" aria-label="Filtrar por categoria">
            <button
              v-for="cat in ['Todos', ...categorias]"
              :key="cat"
              class="cat-btn"
              :class="{ active: selectedCategory === cat }"
              @click="selectedCategory = cat"
              :id="`cat-btn-${cat.toLowerCase().replace(/\s/g, '-')}`"
            >
              {{ catIcon(cat) }} {{ cat }}
            </button>
          </div>
        </div>

        <div class="events-grid">
          <EventCard
            v-for="evento in filteredEventos"
            :key="evento.id"
            :evento="evento"
          />
        </div>

        <div v-if="filteredEventos.length === 0" class="empty-state">
          <span class="empty-icon">🎭</span>
          <p>Nenhum evento encontrado nessa categoria.</p>
        </div>
      </div>
    </section>

    <!-- Organizers Section -->
    <OrganizerSection />


    <AppFooter />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import AppHeader from '../../components/layout/AppHeader.vue'
import AppFooter from '../../components/layout/AppFooter.vue'
import EventCard from '../../components/home/EventCard.vue'
import OrganizerSection from '../../components/home/OrganizerSection.vue'
import { useEventosStore } from '../../stores/eventosStore.js'

const eventosStore = useEventosStore()
const selectedCategory = ref('Todos')

// Carrega eventos do backend ao abrir a página
onMounted(() => eventosStore.carregar())

const eventosDestaque = computed(() => eventosStore.eventosDestaque)
const categorias = computed(() => eventosStore.categorias)
const filteredEventos = computed(() => eventosStore.filtrarPorCategoria(selectedCategory.value))

function catIcon(cat) {
  const map = { Todos: '🎭', Shows: '🎵', Teatro: '🎪', Esportes: '⚽', Festas: '🎉', Conferências: '💻' }
  return map[cat] || '🎫'
}

function formatDate(dateStr) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return ''
  return d.toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' })
}

function scrollToEventos() {
  document.getElementById('eventos')?.scrollIntoView({ behavior: 'smooth' })
}
</script>

<style scoped>
/* Hero */
.hero-section {
  min-height: 100vh;
  display: flex;
  align-items: center;
  position: relative;
  overflow: hidden;
  padding-top: 80px;
}

.hero-bg {
  position: absolute;
  inset: 0;
  background: var(--grad-hero);
  z-index: 0;
}

.hero-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.35;
  animation: float 6s ease-in-out infinite;
}
.hero-orb-1 { width: 600px; height: 600px; background: #7C3AED; top: -200px; left: -100px; animation-delay: 0s; }
.hero-orb-2 { width: 400px; height: 400px; background: #EC4899; bottom: -100px; right: 200px; animation-delay: 2s; }
.hero-orb-3 { width: 300px; height: 300px; background: #06B6D4; top: 30%; right: -50px; animation-delay: 4s; }

.hero-content {
  position: relative;
  z-index: 1;
  display: grid;
  grid-template-columns: 1fr 420px;
  gap: var(--space-3xl);
  align-items: center;
  padding: var(--space-3xl) var(--space-lg);
}

.hero-eyebrow {
  display: inline-block;
  background: rgba(124, 58, 237, 0.2);
  border: 1px solid rgba(124, 58, 237, 0.4);
  color: #C4B5FD;
  padding: 6px 16px;
  border-radius: var(--radius-full);
  font-size: 13px;
  font-weight: 600;
  margin-bottom: var(--space-lg);
}

.hero-title {
  font-size: clamp(36px, 5.5vw, 64px);
  font-weight: 900;
  line-height: 1.1;
  margin-bottom: var(--space-lg);
}

.hero-subtitle {
  font-size: 18px;
  color: var(--clr-text-muted);
  max-width: 520px;
  line-height: 1.7;
  margin-bottom: var(--space-xl);
}

.hero-actions { display: flex; gap: var(--space-md); flex-wrap: wrap; margin-bottom: var(--space-2xl); }

.btn-hero-primary {
  background: var(--grad-primary);
  color: white;
  border: none;
  padding: 16px 32px;
  border-radius: var(--radius-lg);
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition: all var(--transition-md);
  box-shadow: 0 6px 24px rgba(124, 58, 237, 0.4);
}
.btn-hero-primary:hover { box-shadow: 0 10px 36px rgba(124, 58, 237, 0.6); transform: translateY(-2px); }

.btn-hero-secondary {
  background: rgba(255,255,255,0.08);
  color: white;
  border: 1.5px solid rgba(255,255,255,0.2);
  padding: 16px 32px;
  border-radius: var(--radius-lg);
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all var(--transition-md);
  backdrop-filter: blur(10px);
}
.btn-hero-secondary:hover { background: rgba(255,255,255,0.14); border-color: rgba(255,255,255,0.35); }

.hero-stats { display: flex; align-items: center; gap: var(--space-xl); }
.hero-stat { display: flex; flex-direction: column; }
.hstat-num { font-size: 24px; font-weight: 800; background: var(--grad-text); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }
.hstat-label { font-size: 12px; color: var(--clr-text-muted); }
.hero-stat-divider { width: 1px; height: 40px; background: var(--clr-border); }

/* Featured Card */
.hero-featured {
  background: rgba(255,255,255,0.05);
  border: 1px solid rgba(255,255,255,0.12);
  border-radius: var(--radius-xl);
  overflow: hidden;
  cursor: pointer;
  transition: all var(--transition-md);
  backdrop-filter: blur(20px);
}
.hero-featured:hover { border-color: rgba(124, 58, 237, 0.5); box-shadow: var(--shadow-glow); }

.featured-img-wrapper { position: relative; height: 200px; }
.featured-img { width: 100%; height: 100%; object-fit: cover; }
.featured-overlay { position: absolute; inset: 0; background: linear-gradient(to top, rgba(0,0,0,0.7) 0%, transparent 50%); }

.featured-info { padding: var(--space-lg); display: flex; flex-direction: column; gap: 8px; }
.featured-badge { font-size: 11px; font-weight: 700; color: #FCD34D; }
.featured-title { font-size: 20px; font-weight: 800; line-height: 1.2; }
.featured-meta { font-size: 13px; color: var(--clr-text-muted); }
.featured-btn {
  margin-top: 8px;
  background: var(--grad-primary);
  color: white;
  border: none;
  padding: 12px;
  border-radius: var(--radius-md);
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
  width: 100%;
  transition: all var(--transition-md);
}
.featured-btn:hover { opacity: 0.9; }

/* Events Section */
.events-section { padding: var(--space-3xl) 0; }

.section-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--space-xl);
  margin-bottom: var(--space-2xl);
  flex-wrap: wrap;
}

.section-eyebrow {
  font-size: 12px;
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--clr-primary-light);
  margin-bottom: 4px;
}

.section-title { font-size: clamp(24px, 3.5vw, 36px); font-weight: 800; }

.category-filter {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.cat-btn {
  padding: 8px 16px;
  border-radius: var(--radius-full);
  border: 1px solid var(--clr-border);
  background: var(--clr-surface);
  color: var(--clr-text-muted);
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all var(--transition-fast);
}
.cat-btn:hover, .cat-btn.active {
  background: rgba(124, 58, 237, 0.15);
  border-color: var(--clr-primary);
  color: var(--clr-primary-light);
}
.cat-btn.active { background: var(--grad-primary); color: white; border-color: transparent; }

.events-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: var(--space-lg);
}

.empty-state {
  text-align: center;
  padding: var(--space-3xl);
  color: var(--clr-text-muted);
}
.empty-icon { font-size: 48px; display: block; margin-bottom: var(--space-md); }

/* CTA Banner */
.cta-section { padding: var(--space-3xl) 0; }
.cta-card {
  background: var(--grad-card);
  border: 1px solid rgba(124, 58, 237, 0.25);
  border-radius: var(--radius-xl);
  padding: var(--space-3xl);
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-xl);
  position: relative;
  overflow: hidden;
}
.cta-card::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -10%;
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(124, 58, 237, 0.15), transparent 70%);
  pointer-events: none;
}
.cta-text h2 { font-size: clamp(20px, 3vw, 28px); font-weight: 800; margin-bottom: 8px; }
.cta-text p { color: var(--clr-text-muted); font-size: 15px; }
.cta-btn {
  background: var(--grad-primary);
  color: white;
  border: none;
  padding: 16px 32px;
  border-radius: var(--radius-lg);
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  white-space: nowrap;
  transition: all var(--transition-md);
  box-shadow: 0 6px 24px rgba(124, 58, 237, 0.4);
}
.cta-btn:hover { box-shadow: 0 10px 36px rgba(124, 58, 237, 0.6); transform: translateY(-2px); }

@media (max-width: 1024px) {
  .hero-content { grid-template-columns: 1fr; }
  .hero-featured { display: none; }
}

@media (max-width: 768px) {
  .section-header { flex-direction: column; }
  .cta-card { flex-direction: column; text-align: center; }
  .hero-stats { gap: var(--space-md); }
}
</style>
