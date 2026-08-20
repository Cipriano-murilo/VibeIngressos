/**
 * Sanitizer — utilitários de segurança para inputs do usuário.
 *
 * Princípios:
 *  - Previne XSS ao escapar HTML antes de renderizar qualquer string dinâmica
 *  - Valida formatos com regex restritas (whitelist, não blacklist)
 *  - Nunca confia em dados vindos do usuário sem passar por validação
 */
export class Sanitizer {
  /**
   * Escapa caracteres HTML especiais.
   * Use antes de inserir qualquer input do usuário no DOM via innerHTML.
   */
  static escapeHtml(str) {
    if (typeof str !== 'string') return ''
    return str
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#x27;')
      .replace(/\//g, '&#x2F;')
  }

  /** Remove caracteres de controle e espaços extras. */
  static sanitizeText(str) {
    if (typeof str !== 'string') return ''
    return str.replace(/[\x00-\x1F\x7F]/g, '').trim()
  }

  /** Valida e-mail com regex restrita (RFC 5321 simplificado). */
  static isValidEmail(email) {
    const re = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*\.[a-zA-Z]{2,}$/
    return typeof email === 'string' && re.test(email.trim())
  }

  /**
   * Valida força de senha:
   *  - Mínimo 8 caracteres
   *  - Pelo menos 1 letra maiúscula
   *  - Pelo menos 1 número
   *  Retorna { valid: bool, message: string }
   */
  static validatePassword(password) {
    if (typeof password !== 'string' || password.length < 8)
      return { valid: false, message: 'A senha deve ter ao menos 8 caracteres.' }
    if (!/[A-Z]/.test(password))
      return { valid: false, message: 'A senha deve conter ao menos uma letra maiúscula.' }
    if (!/[0-9]/.test(password))
      return { valid: false, message: 'A senha deve conter ao menos um número.' }
    return { valid: true, message: '' }
  }

  /** Valida CPF (dígitos verificadores). */
  static isValidCpf(cpf) {
    const digits = (typeof cpf === 'string' ? cpf : '').replace(/\D/g, '')
    if (digits.length !== 11 || /^(\d)\1+$/.test(digits)) return false

    const calc = (len) =>
      digits.slice(0, len).split('').reduce((acc, d, i) => acc + Number(d) * (len + 1 - i), 0)

    const mod = (n) => {
      const r = (n % 11)
      return r < 2 ? 0 : 11 - r
    }

    return mod(calc(9)) === Number(digits[9]) && mod(calc(10)) === Number(digits[10])
  }
}
