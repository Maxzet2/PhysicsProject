import { useCallback, useEffect, useMemo, useState } from 'react'
import './App.css'
import { api } from './api/client'
import type {
  FinishSessionResponse,
  SessionDto,
  StudyPlanResponse,
  StudyPlanSection
} from './api/types'
import { AuthPanel, type AppUser } from './components/AuthPanel'
import { StudyPlanView, type SectionContext } from './components/StudyPlanView'
import { SessionPanel } from './components/SessionPanel'

const USER_STORAGE_KEY = 'pp_user'

type ActiveSessionMeta = {
  sessionId: string
  infoMode: string
  expiresAt: string | null
  startedAt: string
  sectionTitle: string
  sectionId: string
  chapterTitle: string
  chapterOrder: number
}

const initialStats = { answered: 0, correct: 0 }

function App() {
  const [user, setUser] = useState<AppUser | null>(() => {
    try {
      const raw = localStorage.getItem(USER_STORAGE_KEY)
      if (!raw) return null
      const parsed = JSON.parse(raw)
      return parsed?.id && parsed?.userName ? parsed : null
    } catch {
      return null
    }
  })

  const [plan, setPlan] = useState<StudyPlanResponse | null>(null)
  const [planLoading, setPlanLoading] = useState(false)
  const [planError, setPlanError] = useState<string | null>(null)
  const [pendingSectionId, setPendingSectionId] = useState<string | null>(null)

  const [activeSession, setActiveSession] = useState<ActiveSessionMeta | null>(null)
  const [sessionDetails, setSessionDetails] = useState<SessionDto | null>(null)
  const [sessionStats, setSessionStats] = useState(initialStats)
  const [sessionLog, setSessionLog] = useState<string[]>([])
  const [sessionBusy, setSessionBusy] = useState(false)
  const [sessionError, setSessionError] = useState<string | null>(null)
  const [sessionResult, setSessionResult] = useState<FinishSessionResponse | null>(null)

  const appendLog = useCallback((message: string, reset = false) => {
    const entry = `[${new Date().toLocaleTimeString()}] ${message}`
    setSessionLog((prev) => (reset ? [entry] : [...prev, entry]))
  }, [])

  useEffect(() => {
    if (user) {
      localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user))
    } else {
      localStorage.removeItem(USER_STORAGE_KEY)
    }
  }, [user])

  const loadPlan = useCallback(async () => {
    if (!user) return
    setPlanLoading(true)
    setPlanError(null)
    try {
      const data = await api.getStudyPlan(user.id)
      setPlan(data)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Не удалось загрузить учебный план'
      setPlanError(message)
    } finally {
      setPlanLoading(false)
    }
  }, [user])

  useEffect(() => {
    if (user) {
      setPlan(null)
      loadPlan()
    } else {
      setPlan(null)
    }
  }, [user, loadPlan])

  const loadSessionDetails = useCallback(async (sessionId: string) => {
    const data = await api.getSession(sessionId)
    setSessionDetails(data)
    return data
  }, [])

  const handleLogout = () => {
    setUser(null)
    setPlan(null)
    setPlanError(null)
    setPendingSectionId(null)
    setActiveSession(null)
    setSessionDetails(null)
    setSessionStats(initialStats)
    setSessionLog([])
    setSessionResult(null)
    setSessionError(null)
  }

  const handleResetSection = async (section: StudyPlanSection) => {
    if (!user) return
    setPendingSectionId(section.id)
    setPlanError(null)
    try {
      await api.resetSection(section.id, user.id)
      await loadPlan()
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Не удалось сбросить попытки'
      setPlanError(message)
    } finally {
      setPendingSectionId(null)
    }
  }

  const handleStartSession = async (
    section: StudyPlanSection,
    mode: 'training' | 'test',
    context: SectionContext
  ) => {
    if (!user) return
    setPendingSectionId(section.id)
    setSessionError(null)
    setSessionResult(null)

    try {
      const response =
        mode === 'training'
          ? await api.startTraining(section.id, user.id)
          : await api.startTest(section.id, user.id)

      await loadPlan()

      setActiveSession({
        sessionId: response.sessionId,
        infoMode: response.mode,
        expiresAt: response.expiresAt,
        startedAt: response.startedAt,
        sectionTitle: section.title,
        sectionId: section.id,
        chapterTitle: context.chapterTitle,
        chapterOrder: context.chapterOrder
      })
      setSessionStats(initialStats)
      appendLog(`Сессия началась (${mode === 'training' ? 'тренировка' : 'тест'})`, true)
      await loadSessionDetails(response.sessionId)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Не удалось запустить сессию'
      setSessionError(message)
      setActiveSession(null)
      setSessionDetails(null)
      setSessionLog([])
    } finally {
      setPendingSectionId(null)
    }
  }

  const handleRefreshSession = async () => {
    if (!activeSession) return
    setSessionError(null)
    try {
      await loadSessionDetails(activeSession.sessionId)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Не удалось обновить сессию'
      setSessionError(message)
    }
  }

  const handleSubmitAnswer = async (answer: string) => {
    if (!activeSession || !sessionDetails) return
    const currentItem = sessionDetails.items.find((item) => !item.submitted)
    if (!currentItem) {
      setSessionError('Все вопросы пройдены. Завершите сессию.')
      return
    }

    setSessionBusy(true)
    setSessionError(null)

    try {
      const result = await api.submitAnswer(activeSession.sessionId, currentItem.itemId, answer)
      const totalQuestions = sessionDetails.items.length

      setSessionStats((prev) => {
        const next = {
          answered: prev.answered + 1,
          correct: prev.correct + (result.isCorrect ? 1 : 0)
        }
        appendLog(
          `Ответ ${next.answered}/${totalQuestions}: ${result.isCorrect ? 'верно' : 'неверно'}, баллы +${Number(result.scoreAwarded).toFixed(2)}`
        )
        return next
      })

      await loadSessionDetails(activeSession.sessionId)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Не удалось отправить ответ'
      setSessionError(message)
    } finally {
      setSessionBusy(false)
    }
  }

  const handleSkipQuestion = () => handleSubmitAnswer('')

  const handleFinishSession = async () => {
    if (!activeSession) return
    setSessionBusy(true)
    setSessionError(null)
    try {
      const result = await api.finishSession(activeSession.sessionId)
      setSessionResult(result)
      appendLog('Сессия завершена.')
      await loadPlan()
      await loadSessionDetails(activeSession.sessionId)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Не удалось завершить сессию'
      setSessionError(message)
    } finally {
      setSessionBusy(false)
    }
  }

  const handleCloseSession = () => {
    setActiveSession(null)
    setSessionDetails(null)
    setSessionStats(initialStats)
    setSessionLog([])
    setSessionResult(null)
    setSessionError(null)
  }

  const greeting = useMemo(() => (user ? user.userName : 'Гость'), [user])
  const currentItem = sessionDetails?.items.find((item) => !item.submitted) ?? null
  const totalItems = sessionDetails?.items.length ?? 0

  return (
    <div className="app-shell">
      <header className="app-header">
        <div>
          <h1>Physics Project</h1>
          <p className="muted">Тренажёр по разделам курса с автоматической проверкой.</p>
        </div>
        {user && (
          <div className="user-badge">
            <span>{greeting}</span>
            <button className="btn btn-secondary" onClick={handleLogout}>
              Выйти
            </button>
          </div>
        )}
      </header>

      <main className="app-main">
        {!user && <AuthPanel onAuthenticated={setUser} />}

        {user && (
          <>
            <StudyPlanView
              plan={plan}
              loading={planLoading}
              error={planError}
              onReload={loadPlan}
              onStart={handleStartSession}
              onReset={handleResetSection}
              pendingSectionId={pendingSectionId}
            />

            <SessionPanel
              session={activeSession}
              details={sessionDetails}
              stats={sessionStats}
              log={sessionLog}
              error={sessionError}
              busy={sessionBusy}
              result={sessionResult}
              currentItem={currentItem}
              totalItems={totalItems}
              onRefresh={handleRefreshSession}
              onSubmit={handleSubmitAnswer}
              onSkip={handleSkipQuestion}
              onFinish={handleFinishSession}
              onClose={handleCloseSession}
            />
          </>
        )}
      </main>
    </div>
  )
}

export default App
