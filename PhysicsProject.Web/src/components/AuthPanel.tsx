import { useState } from 'react'
import type { FormEvent } from 'react'
import { api } from '../api/client'

export interface AppUser {
  id: string
  userName: string
}

interface AuthPanelProps {
  onAuthenticated: (user: AppUser) => void
}

export function AuthPanel({ onAuthenticated }: AuthPanelProps) {
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [userName, setUserName] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  const toggleMode = () => {
    setMode((prev) => (prev === 'login' ? 'register' : 'login'))
    setError(null)
    setSuccessMessage(null)
  }

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    if (!userName.trim() || !password.trim()) {
      setError('Введите логин и пароль')
      return
    }

    setLoading(true)
    setError(null)

    try {
      if (mode === 'login') {
        const result = await api.login(userName.trim(), password.trim())
        if (!result.userId || result.userId === '00000000-0000-0000-0000-000000000000') {
          throw new Error(result.error || 'Неверное имя пользователя или пароль')
        }
        onAuthenticated({ id: result.userId, userName: result.userName })
      } else {
        const result = await api.register(userName.trim(), password.trim())
        setSuccessMessage('Учётная запись создана. Можно войти.')
        setMode('login')
        setUserName(result.userName)
        setPassword('')
      }
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Неизвестная ошибка'
      setError(message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="card auth-card">
      <h2>{mode === 'login' ? 'Вход в систему' : 'Регистрация'}</h2>
      <p className="muted">
        Используйте учётные данные, выданные преподавателем. Данные сохраняются локально в
        браузере.
      </p>

      <form className="auth-form" onSubmit={handleSubmit}>
        <label>
          Логин
          <input
            type="text"
            placeholder="student01"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            disabled={loading}
          />
        </label>

        <label>
          Пароль
          <input
            type="password"
            placeholder="********"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
          />
        </label>

        {error && <div className="banner error">{error}</div>}
        {successMessage && <div className="banner success">{successMessage}</div>}

        <button className="btn" type="submit" disabled={loading}>
          {loading ? 'Подождите…' : mode === 'login' ? 'Войти' : 'Создать аккаунт'}
        </button>
      </form>

      <button className="btn-link" type="button" onClick={toggleMode} disabled={loading}>
        {mode === 'login' ? 'Нет аккаунта? Зарегистрируйтесь' : 'Уже есть аккаунт? Войти'}
      </button>
    </div>
  )
}
