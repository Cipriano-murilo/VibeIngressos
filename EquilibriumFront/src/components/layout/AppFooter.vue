<template>
  <footer class="app-footer" role="contentinfo">
    <div class="container footer-inner">
      <div class="footer-brand">
        <router-link to="/" class="logo">
          <svg width="28" height="28" viewBox="0 0 32 32" fill="none">
            <circle cx="16" cy="16" r="16" fill="url(#grad-footer)"/>
            <path d="M10 16 L16 10 L22 16 L16 22 Z" fill="white" opacity="0.9"/>
            <circle cx="16" cy="16" r="3" fill="white"/>
            <defs>
              <linearGradient id="grad-footer" x1="0" y1="0" x2="32" y2="32">
                <stop offset="0%" stop-color="#7C3AED"/>
                <stop offset="100%" stop-color="#EC4899"/>
              </linearGradient>
            </defs>
          </svg>
          <span class="logo-text">VibeIngressos</span>
        </router-link>
        <p class="footer-tagline">Sua bilheteria digital. Viva momentos inesquecíveis.</p>
      </div>

      <div class="footer-links">
        <div class="footer-col">
          <h4>Plataforma</h4>
          <router-link to="/">Eventos</router-link>
          <router-link to="/cadastro">Criar conta</router-link>
          <router-link to="/login">Entrar</router-link>
        </div>
        <div class="footer-col">
          <h4>Suporte</h4>
          <a href="#">Central de ajuda</a>
          <a href="#">Política de privacidade</a>
          <a href="#">Termos de uso</a>
        </div>
        <div class="footer-col">
          <h4>Contato</h4>
          <a href="#">contato@vibeingressos.com.br</a>
          <a href="#">(11) 4000-0000</a>
        </div>
      </div>
    </div>

    <div class="footer-bottom">
      <div class="container">
        <p>© {{ year }} VibeIngressos. Todos os direitos reservados.</p>
        <p class="footer-made">Feito com ❤️ no Brasil</p>
      </div>
    </div>
  </footer>
</template>

<script setup>
const year = new Date().getFullYear()
</script>

<style scoped>
.app-footer {
  background: var(--clr-bg-2);
  border-top: 1px solid var(--clr-border);
  margin-top: var(--space-3xl);
}

.footer-inner {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: var(--space-3xl);
  padding: var(--space-3xl) 0 var(--space-2xl);
}

.logo { display: flex; align-items: center; gap: 10px; margin-bottom: var(--space-md); }
.logo-text {
  font-family: var(--font-display);
  font-size: 18px;
  font-weight: 800;
  background: var(--grad-text);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.footer-tagline { font-size: 14px; color: var(--clr-text-muted); max-width: 220px; line-height: 1.5; }

.footer-links {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-xl);
}

.footer-col { display: flex; flex-direction: column; gap: 10px; }
.footer-col h4 { font-size: 13px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.08em; color: var(--clr-text); margin-bottom: 4px; }
.footer-col a { font-size: 14px; color: var(--clr-text-muted); transition: color var(--transition-fast); }
.footer-col a:hover { color: var(--clr-primary-light); }

.footer-bottom {
  border-top: 1px solid var(--clr-border);
  padding: var(--space-md) 0;
}
.footer-bottom .container {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.footer-bottom p { font-size: 13px; color: var(--clr-text-subtle); }
.footer-made { color: var(--clr-text-muted); }

@media (max-width: 768px) {
  .footer-inner { grid-template-columns: 1fr; gap: var(--space-xl); }
  .footer-links { grid-template-columns: repeat(2, 1fr); }
  .footer-bottom .container { flex-direction: column; gap: 6px; text-align: center; }
}
</style>
