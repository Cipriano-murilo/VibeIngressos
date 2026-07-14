<!-- Dados de organizadores mockados com @instagram -->
<template>
  <section id="organizadores" class="organizer-section">
    <div class="container">
      <div class="section-header">
        <p class="section-eyebrow">Quem faz acontecer</p>
        <h2 class="section-title">Organizadores <span class="gradient-text">em destaque</span></h2>
        <p class="section-subtitle">Conheça os criadores dos melhores eventos da plataforma</p>
      </div>

      <div class="organizers-grid">
        <div
          v-for="org in organizadores"
          :key="org.id"
          class="organizer-card"
          :id="`organizer-card-${org.id}`"
        >
          <div class="org-avatar-wrapper">
            <img :src="org.avatar" :alt="org.nome" class="org-avatar" />
            <div class="org-avatar-glow"></div>
          </div>
          <div class="org-info">
            <h3 class="org-nome">{{ org.nome }}</h3>
            <p class="org-desc">{{ org.descricao }}</p>
            <div class="org-stats">
              <div class="org-stat">
                <span class="stat-num">{{ org.eventos }}</span>
                <span class="stat-label">Eventos</span>
              </div>
              <div class="org-stat">
                <span class="stat-num">{{ org.seguidores }}</span>
                <span class="stat-label">Seguidores</span>
              </div>
            </div>
            <a
              :href="`https://instagram.com/${org.instagram}`"
              target="_blank"
              rel="noopener noreferrer"
              class="org-instagram"
              :id="`ig-link-${org.id}`"
              :aria-label="`Instagram de ${org.nome}`"
            >
              <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163c0-3.403-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/>
              </svg>
              @{{ org.instagram }}
            </a>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
const organizadores = [
  {
    id: 1,
    nome: 'Vibe Events',
    instagram: 'vibeevents_br',
    descricao: 'Produtora especializada em festivais de música eletrônica e eventos de grande porte.',
    avatar: 'https://api.dicebear.com/7.x/shapes/svg?seed=vibeevents&backgroundColor=7C3AED',
    eventos: 24,
    seguidores: '120K'
  },
  {
    id: 2,
    nome: 'Arena Cultural',
    instagram: 'arenacultural.sp',
    descricao: 'Shows, teatro e cultura popular. Levamos arte de qualidade para todos os públicos.',
    avatar: 'https://api.dicebear.com/7.x/shapes/svg?seed=arenacultural&backgroundColor=EC4899',
    eventos: 18,
    seguidores: '85K'
  },
  {
    id: 3,
    nome: 'Nexus Tech Events',
    instagram: 'nexustechevents',
    descricao: 'O maior hub de conferências de tecnologia e inovação do Brasil.',
    avatar: 'https://api.dicebear.com/7.x/shapes/svg?seed=nexustech&backgroundColor=06B6D4',
    eventos: 9,
    seguidores: '210K'
  },
  {
    id: 4,
    nome: 'Sport Nation',
    instagram: 'sportnation_br',
    descricao: 'Especialistas em eventos esportivos de alto nível. MMA, futebol, basquete e mais.',
    avatar: 'https://api.dicebear.com/7.x/shapes/svg?seed=sportnation&backgroundColor=10B981',
    eventos: 31,
    seguidores: '340K'
  }
]
</script>

<style scoped>
.organizer-section {
  padding: var(--space-3xl) 0;
}

.section-header {
  text-align: center;
  margin-bottom: var(--space-3xl);
}

.section-eyebrow {
  font-size: 13px;
  font-weight: 600;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: var(--clr-primary-light);
  margin-bottom: var(--space-sm);
}

.section-title {
  font-size: clamp(28px, 4vw, 40px);
  font-weight: 800;
  margin-bottom: var(--space-md);
}

.section-subtitle {
  font-size: 16px;
  color: var(--clr-text-muted);
}

.organizers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: var(--space-lg);
}

.organizer-card {
  background: var(--grad-card);
  border: 1px solid var(--clr-border);
  border-radius: var(--radius-lg);
  padding: var(--space-xl);
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: var(--space-md);
  transition: all var(--transition-md);
}

.organizer-card:hover {
  border-color: rgba(124, 58, 237, 0.35);
  transform: translateY(-4px);
  box-shadow: var(--shadow-md), var(--shadow-glow);
}

.org-avatar-wrapper {
  position: relative;
  width: 80px;
  height: 80px;
}

.org-avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  border: 3px solid var(--clr-border);
  object-fit: cover;
}

.org-avatar-glow {
  position: absolute;
  inset: -4px;
  border-radius: 50%;
  background: var(--grad-primary);
  opacity: 0;
  z-index: -1;
  filter: blur(10px);
  transition: opacity var(--transition-md);
}

.organizer-card:hover .org-avatar-glow { opacity: 0.4; }

.org-info { display: flex; flex-direction: column; align-items: center; gap: 8px; width: 100%; }

.org-nome { font-size: 18px; font-weight: 700; }

.org-desc { font-size: 13px; color: var(--clr-text-muted); line-height: 1.5; }

.org-stats {
  display: flex;
  gap: var(--space-xl);
  padding: var(--space-md) 0;
  border-top: 1px solid var(--clr-border);
  border-bottom: 1px solid var(--clr-border);
  width: 100%;
  justify-content: center;
}

.org-stat { display: flex; flex-direction: column; align-items: center; }
.stat-num { font-size: 20px; font-weight: 800; background: var(--grad-text); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }
.stat-label { font-size: 11px; color: var(--clr-text-muted); text-transform: uppercase; letter-spacing: 0.06em; }

.org-instagram {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: linear-gradient(135deg, rgba(225,48,108,0.15), rgba(255,220,128,0.1));
  border: 1px solid rgba(225, 48, 108, 0.25);
  border-radius: var(--radius-full);
  font-size: 13px;
  font-weight: 600;
  color: #F472B6;
  transition: all var(--transition-md);
}

.org-instagram:hover {
  background: linear-gradient(135deg, rgba(225,48,108,0.25), rgba(255,220,128,0.15));
  transform: scale(1.03);
}
</style>
