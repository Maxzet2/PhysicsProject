export interface LoginResponse {
  userId: string
  userName: string
  error?: string
}

export interface RegisterResponse {
  userId: string
  userName: string
}

export interface StudyPlanResponse {
  userId: string | null
  chapters: StudyPlanChapter[]
}

export interface StudyPlanChapter {
  id: string
  title: string
  description: string
  orderIndex: number
  sections: StudyPlanSection[]
}

export interface StudyPlanSection {
  id: string
  chapterId: string
  title: string
  description: string
  orderIndex: number
  defaultQuestionCount: number
  testTimeLimitSeconds: number
  progress: SectionProgress | null
}

export interface SectionProgress {
  attemptCycle: number
  attemptsUsedInCycle: number
  attemptsRemaining: number
  maxAttemptsPerCycle: number
  hasAttemptsAvailable: boolean
  activeTestSessionId: string | null
  activeTestExpiresAt: string | null
  lastTrainingCompletedAt: string | null
  lastTestPassedAt: string | null
}

export interface SectionSessionResponse {
  sessionId: string
  sectionId: string
  mode: string
  startedAt: string
  expiresAt: string | null
  questionCount: number
  progress: SectionProgress | null
}

export interface SessionItemDto {
  itemId: string
  instanceId: string
  order: number
  maxScore: number
  statement: string
  parameters: Record<string, unknown>
  submitted: boolean
}

export interface SessionDto {
  id: string
  userId: string
  sectionId: string | null
  mode: string
  startedAt: string
  finishedAt: string | null
  items: SessionItemDto[]
}

export interface SubmitAnswerResponse {
  isCorrect: boolean
  scoreAwarded: number
  feedback: string
  totalSubmissions: number
  correctSubmissions: number
}

export interface FinishSessionResponse {
  totalScore: number
  correctAnswers: number
  totalAnswers: number
  passed: boolean
}

export interface ResetSectionAttemptsResponse {
  sectionId: string
  attemptCycle: number
  attemptsRemaining: number
  maxAttemptsPerCycle: number
  hasAttemptsAvailable: boolean
}
