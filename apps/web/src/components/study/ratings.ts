import { ReviewRating, type RatingKey } from '../../types/learning'

export interface RatingMeta {
  value: ReviewRating
  key: RatingKey
  color: string
  buttonClass: string
}

export const ratingOrder: RatingMeta[] = [
  {
    value: ReviewRating.Again,
    key: 'again',
    color: '#f5434c',
    buttonClass: 'border-memorio-danger/50 text-memorio-danger hover:bg-memorio-danger/10',
  },
  {
    value: ReviewRating.Hard,
    key: 'hard',
    color: '#f5b60c',
    buttonClass: 'border-memorio-warning/50 text-memorio-warning hover:bg-memorio-warning/10',
  },
  {
    value: ReviewRating.Good,
    key: 'good',
    color: '#8863f9',
    buttonClass: 'border-memorio-primary/60 text-memorio-primary-light hover:bg-memorio-primary/10',
  },
  {
    value: ReviewRating.Easy,
    key: 'easy',
    color: '#3ecf8e',
    buttonClass: 'border-memorio-success/50 text-memorio-success hover:bg-memorio-success/10',
  },
]
