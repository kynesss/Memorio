export type CardType = 'Basic' | 'BasicReversed' | 'Cloze'

export interface Deck {
  id: string
  name: string
  description: string | null
  createdAt: string
  updatedAt: string | null
}

export interface Card {
  id: string
  deckId: string
  front: string
  back: string
  tags: string | null
  type: CardType
  createdAt: string
  updatedAt: string | null
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface DeckInput {
  name: string
  description?: string
}

export interface CardInput {
  front: string
  back: string
  tags?: string
  type: CardType
}
