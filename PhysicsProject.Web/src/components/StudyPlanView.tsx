import type { StudyPlanResponse, StudyPlanSection } from '../api/types'

const dateFormatter = new Intl.DateTimeFormat('ru-RU', {
  day: '2-digit',
  month: 'short',
  hour: '2-digit',
  minute: '2-digit'
})

function formatDate(value?: string | null): string {
  if (!value) return '—'
  try {
    return dateFormatter.format(new Date(value))
  } catch {
    return '—'
  }
}

export interface SectionContext {
  chapterTitle: string
  chapterOrder: number
}

interface StudyPlanViewProps {
  plan: StudyPlanResponse | null
  loading: boolean
  error: string | null
  onReload: () => void
  onStart: (section: StudyPlanSection, mode: 'training' | 'test', context: SectionContext) => void
  onReset: (section: StudyPlanSection) => void
  pendingSectionId?: string | null
}

export function StudyPlanView({
  plan,
  loading,
  error,
  onReload,
  onStart,
  onReset,
  pendingSectionId
}: StudyPlanViewProps) {
  return (
    <div className="plan-wrapper">
      <div className="plan-header">
        <div>
          <h2>Учебный план</h2>
          <p className="muted">
            Каждая глава содержит несколько разделов. Начните с тренировки, затем переходите к тесту.
          </p>
        </div>
        <button className="btn btn-secondary" onClick={onReload} disabled={loading}>
          {loading ? 'Обновляем…' : 'Обновить'}
        </button>
      </div>

      {error && <div className="banner error">{error}</div>}

      {loading && !plan && <div className="empty-state">Загружаем актуальный план…</div>}

      {!loading && plan?.chapters.length === 0 && (
        <div className="empty-state">
          План ещё не настроен. Обратитесь к преподавателю, чтобы добавить главы и разделы.
        </div>
      )}

      <div className="chapter-grid">
        {plan?.chapters
          .slice()
          .sort((a, b) => a.orderIndex - b.orderIndex)
          .map((chapter) => (
            <section key={chapter.id} className="chapter-card">
              <header className="chapter-header">
                <div>
                  <p className="chapter-index">Глава {chapter.orderIndex}</p>
                  <h3>{chapter.title}</h3>
                </div>
                <span className="pill pill-ghost">{chapter.sections.length} разделов</span>
              </header>
              <p className="muted">{chapter.description}</p>

              <div className="section-list">
                {chapter.sections
                  .slice()
                  .sort((a, b) => a.orderIndex - b.orderIndex)
                  .map((section) => {
                    const attemptsRemaining =
                      section.progress?.attemptsRemaining ??
                      section.progress?.maxAttemptsPerCycle ??
                      3
                    const maxAttempts = section.progress?.maxAttemptsPerCycle ?? 3

                    return (
                      <article key={section.id} className="section-card">
                      <div className="section-header">
                        <div>
                          <p className="section-index">Раздел {chapter.orderIndex}.{section.orderIndex}</p>
                          <h4>{section.title}</h4>
                        </div>
                        <span className="pill">{section.defaultQuestionCount} вопросов</span>
                      </div>
                      <p className="muted">{section.description}</p>

                      <div className="section-stats">
                        <div>
                          <p className="stat-label">Попытки теста</p>
                          <p className="stat-value">{`${attemptsRemaining}/${maxAttempts}`}</p>
                          <p className="stat-footnote">Цикл #{section.progress?.attemptCycle ?? 1}</p>
                        </div>
                        <div>
                          <p className="stat-label">Последняя тренировка</p>
                          <p className="stat-value">{formatDate(section.progress?.lastTrainingCompletedAt)}</p>
                          <p className="stat-label">Последний успешный тест</p>
                          <p className="stat-value">{formatDate(section.progress?.lastTestPassedAt)}</p>
                        </div>
                        <div>
                          <p className="stat-label">Лимит теста</p>
                          <p className="stat-value">{section.testTimeLimitSeconds / 60} мин</p>
                          <p className="stat-footnote">
                            {section.progress?.activeTestSessionId
                              ? `Активен до ${formatDate(section.progress.activeTestExpiresAt)}`
                              : 'Нет активного теста'}
                          </p>
                        </div>
                      </div>

                      <div className="section-actions">
                        <button
                          className="btn"
                          onClick={() =>
                            onStart(section, 'training', {
                              chapterTitle: chapter.title,
                              chapterOrder: chapter.orderIndex
                            })
                          }
                          disabled={pendingSectionId === section.id}
                        >
                          {pendingSectionId === section.id ? 'Запускаем…' : 'Тренировка'}
                        </button>
                        <button
                          className="btn btn-accent"
                          onClick={() =>
                            onStart(section, 'test', {
                              chapterTitle: chapter.title,
                              chapterOrder: chapter.orderIndex
                            })
                          }
                          disabled={
                            pendingSectionId === section.id ||
                            section.progress?.hasAttemptsAvailable === false
                          }
                        >
                          {section.progress?.hasAttemptsAvailable === false
                            ? 'Нет попыток'
                            : 'Тест'}
                        </button>
                        <button
                          className="btn btn-secondary"
                          onClick={() => onReset(section)}
                          disabled={pendingSectionId === section.id}
                        >
                          Сброс попыток
                        </button>
                      </div>
                    </article>
                    )
                  })}
              </div>
            </section>
          ))}
      </div>
    </div>
  )
}
