export interface User {
  id: string
  email: string
  displayName: string
  createdAt: string
}

export interface Card {
  id: string
  deckId: string
  front: string
  back: string
  createdAt: string
}

export interface Deck {
  id: string
  ownerId: string
  name: string
  description?: string
  createdAt: string
}
