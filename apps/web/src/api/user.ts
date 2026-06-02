import { apiClient } from '../auth/api'
import type { CurrentUser } from '../types/user'

export async function getCurrentUser() {
  const response = await apiClient.get<CurrentUser>('/api/v1/auth/me')
  return response.data
}
