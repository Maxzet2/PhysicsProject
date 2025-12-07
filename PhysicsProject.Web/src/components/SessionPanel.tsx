import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import type { FinishSessionResponse, SessionDto, SessionItemDto } from '../api/types'

interface ActiveSessionMeta {
  sessionId: string
  infoMode: string
  expiresAt: string | null
  startedAt: string
  sectionTitle: string
  sectionId: string
  chapterTitle: string
  chapterOrder: number
}

interface SessionPanelProps {
  session: ActiveSessionMeta | null
  details: SessionDto | null
  stats: { answered: number; correct: number }
  log: string[]
  error: string | null
  busy: boolean
  result: FinishSessionResponse | null
  currentItem: SessionItemDto | null
  totalItems: number
  onRefresh: () => void
  onSubmit: (answer: string) => Promise<void> | void
  onSkip: () => Promise<void> | void
  onFinish: () => Promise<void> | void
  onClose: () => void
}

export function SessionPanel({
  session,
  details,
  stats,
  log,
  error,
  busy,
  result,
  currentItem,
  totalItems,
  onRefresh,
  onSubmit,
  onSkip,
  onFinish,
  onClose
}: SessionPanelProps) {
  const [answer, setAnswer] = useState('')
  const [timeLeft, setTimeLeft] = useState<string | null>(null)

  useEffect(() => {
    if (!session?.expiresAt) {
      setTimeLeft(null)
      return
    }

    const target = new Date(session.expiresAt).getTime()
    const update = () => {
      const diff = target - Date.now()
      if (diff <= 0) {
        setTimeLeft('00:00')
        return
      }
      const minutes = Math.floor(diff / 60000)
      const seconds = Math.floor((diff % 60000) / 1000)
      setTimeLeft(`${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`)
    }

    update()
    const timer = window.setInterval(update, 1000)
    return () => window.clearInterval(timer)
  }, [session?.expiresAt])

  useEffect(() => {
    setAnswer('')
  }, [currentItem?.itemId])

  const modeLabel = useMemo(() => {
    if (!session) return 'Сессия'
    return session.infoMode?.toLowerCase() === 'training' ? 'Тренировка' : 'Контрольный тест'
  }, [session])

  const finishedLabel = useMemo(() => {
    if (!details?.finishedAt) return null
    try {
      return new Date(details.finishedAt).toLocaleString('ru-RU')
    } catch {
      return details.finishedAt
    }
  }, [details?.finishedAt])

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    if (!session || !currentItem) return
    const prepared = answer.trim()
    if (!prepared) return
    await onSubmit(prepared)
    setAnswer('')
  }

  const handleSkip = async () => {
    if (!session || !currentItem) return
    await onSkip()
    setAnswer('')
  }

  return (
    <section className="card session-card">
      <div className="session-header">
        <div>
          <p className="section-index">
            {session ? `Глава ${session.chapterOrder} · ${session.chapterTitle}` : 'Нет активной сессии'}
          </p>
          <h2>{session ? session.sectionTitle : 'Запустите тренировку или тест'}</h2>
          {session && (
            <p className="muted">{modeLabel}. Начато {new Date(session.startedAt).toLocaleString('ru-RU')}.</p>
          )}
        </div>
        <div className="session-actions">
          {session && (
            <button className="btn btn-secondary" type="button" onClick={onRefresh} disabled={busy}>
              Обновить
            </button>
          )}
          {session && (
            <button className="btn btn-secondary" type="button" onClick={onClose} disabled={busy}>
              Свернуть
            </button>
          )}
        </div>
      </div>

      {session && (
        <div className="session-meta">
          <span className="pill">{modeLabel}</span>
          <span className="pill">Пройдено: {stats.answered}/{totalItems}</span>
          <span className="pill">Правильных: {stats.correct}</span>
          <span className="pill">
            {session.expiresAt ? `Таймер: ${timeLeft ?? '—'}` : 'Без ограничения времени'}
          </span>
          {finishedLabel && <span className="pill pill-ghost">Завершено {finishedLabel}</span>}
        </div>
      )}

      {error && <div className="banner error">{error}</div>}

      {!session && <div className="empty-state">Начните раздел в учебном плане, чтобы приступить к вопросам.</div>}

      {session && (
        <>
          <div className="question-card">
            {!details ? (
              <div className="empty-state">Загружаем задания…</div>
            ) : currentItem ? (
              <>
                <p className="question-index">Вопрос {stats.answered + 1} из {totalItems}</p>
                <h3>{currentItem.statement}</h3>
                {currentItem.parameters && Object.keys(currentItem.parameters).length > 0 && (
                  <pre className="question-params">{JSON.stringify(currentItem.parameters, null, 2)}</pre>
                )}

                <form className="answer-form" onSubmit={handleSubmit}>
                  <label>
                    Ответ
                    <input
                      type="text"
                      value={answer}
                      onChange={(e) => setAnswer(e.target.value)}
                      placeholder="Введите числовой или текстовый ответ"
                      disabled={busy}
                    />
                  </label>
                  <div className="answer-actions">
                    <button
                      className="btn"
                      type="submit"
                      disabled={busy || !currentItem || !answer.trim()}
                    >
                      Отправить
                    </button>
                    <button
                      className="btn btn-secondary"
                      type="button"
                      onClick={handleSkip}
                      disabled={busy || !currentItem}
                    >
                      Пропустить
                    </button>
                    <button className="btn btn-accent" type="button" onClick={onFinish} disabled={busy}>
                      Завершить
                    </button>
                  </div>
                </form>
              </>
            ) : (
              <div className="empty-state">
                Все вопросы пройдены. Вы можете завершить попытку или обновить сессию.
              </div>
            )}
          </div>

          {result && (
            <div className="banner success">
              Итог: {result.correctAnswers}/{result.totalAnswers} верных, баллы {Number(result.totalScore).toFixed(2)}.
              {result.passed ? ' Раздел зачтён!' : ' Можно попробовать снова.'}
            </div>
          )}

          <div className="log-panel">
            <div className="log-header">
              <h4>Журнал событий</h4>
            </div>
            <pre>{log.length ? log.join('\n') : 'Журнал будет появляться здесь после старта сессии.'}</pre>
          </div>
        </>
      )}
    </section>
  )
}
