import type {
  FinishSessionResponse,
  LoginResponse,
  RegisterResponse,
  ResetSectionAttemptsResponse,
  SectionSessionResponse,
  SessionDto,
  StudyPlanResponse,
  SubmitAnswerResponse
} from './types'

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/$/, '')

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const url = path.startsWith('http') ? path : `${API_BASE_URL}${path}`
  const response = await fetch(url, init)
  if (!response.ok) {
    const message = await extractError(response)
    throw new Error(message)
  }
  const contentType = response.headers.get('content-type') || ''
  if (contentType.includes('application/json')) {
    return (await response.json()) as T
  }
  return (await response.text()) as T
}

async function extractError(response: Response): Promise<string> {
  const fallback = `${response.status} ${response.statusText}`.trim()
  try {
    const data = await response.json()
    if (typeof data === 'string') return data
    if ('error' in data && typeof data.error === 'string') return data.error
    if ('message' in data && typeof data.message === 'string') return data.message
  } catch {
    try {
      const text = await response.text()
      if (text) return text
    } catch {
      // ignore
    }
  }
  return fallback
}

export const api = {
  login(userName: string, password: string) {
    return request<LoginResponse>('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName, password })
    })
  },
  register(userName: string, password: string) {
    return request<RegisterResponse>('/api/auth/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userName, password })
    })
  },
  getStudyPlan(userId?: string | null) {
    const query = userId ? `?userId=${encodeURIComponent(userId)}` : ''
    return request<StudyPlanResponse>(`/api/study-plan${query}`)
  },
  startTraining(sectionId: string, userId: string) {
    return request<SectionSessionResponse>(`/api/sections/${sectionId}/training/start`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userId })
    })
  },
  startTest(sectionId: string, userId: string) {
    return request<SectionSessionResponse>(`/api/sections/${sectionId}/test/start`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userId })
    })
  },
  resetSection(sectionId: string, userId: string) {
    return request<ResetSectionAttemptsResponse>(`/api/sections/${sectionId}/reset`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userId })
    })
  },
  getSession(sessionId: string) {
    return request<SessionDto>(`/api/sessions/${sessionId}`)
  },
  submitAnswer(sessionId: string, itemId: string, answer: string) {
    return request<SubmitAnswerResponse>(`/api/sessions/${sessionId}/items/${itemId}/submit`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ answer })
    })
  },
  finishSession(sessionId: string) {
    return request<FinishSessionResponse>(`/api/sessions/${sessionId}/finish`, {
      method: 'POST'
    })
  }
}
