import axios, { type InternalAxiosRequestConfig } from 'axios'
import i18n from '../i18n'
import { tokenStore } from './tokenStore'

interface AccessTokenResponse {
  access_token: string
  token_type: string
  expires_in: number
}

interface ApiError {
  detail?: string
  title?: string
}

interface RetryableRequest extends InternalAxiosRequestConfig {
  _retry?: boolean
}

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5243',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
})

const refreshClient = axios.create({
  baseURL: apiClient.defaults.baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
})

let refreshPromise: Promise<string> | null = null

apiClient.interceptors.request.use((request) => {
  const accessToken = tokenStore.getAccessToken()

  if (accessToken) {
    request.headers.Authorization = `Bearer ${accessToken}`
  }

  if (request.data instanceof FormData) {
    delete request.headers['Content-Type']
  }

  return request
})

apiClient.interceptors.response.use(
  (response) => response,
  async (error: unknown) => {
    if (!axios.isAxiosError(error) || error.response?.status !== 401 || !error.config) {
      return Promise.reject(error)
    }

    const request = error.config as RetryableRequest

    if (request._retry || isAuthRequest(request.url)) {
      return Promise.reject(error)
    }

    request._retry = true

    try {
      const accessToken = await refreshAccessToken()
      request.headers.Authorization = `Bearer ${accessToken}`
      return apiClient(request)
    } catch (refreshError) {
      tokenStore.clear()
      return Promise.reject(refreshError)
    }
  },
)

export async function login(email: string, password: string) {
  const response = await apiClient.post<AccessTokenResponse>('/api/v1/auth/login', { email, password })
  tokenStore.setAccessToken(response.data.access_token)
}

export async function register(email: string, password: string) {
  const response = await apiClient.post<AccessTokenResponse>('/api/v1/auth/register', { email, password })
  tokenStore.setAccessToken(response.data.access_token)
}

export async function refreshAccessToken() {
  if (!refreshPromise) {
    refreshPromise = requestAccessToken().finally(() => {
      refreshPromise = null
    })
  }

  return refreshPromise
}

async function requestAccessToken() {
  const response = await refreshClient.post<AccessTokenResponse>('/api/v1/auth/refresh')
  tokenStore.setAccessToken(response.data.access_token)
  return response.data.access_token
}

export function getApiErrorMessage(error: unknown) {
  if (!axios.isAxiosError<ApiError>(error)) {
    return i18n.t('errors.unexpected')
  }

  if (error.response?.status === 401) {
    return i18n.t('errors.invalidCredentials')
  }

  return error.response?.data.detail
    ?? error.response?.data.title
    ?? i18n.t('errors.serverUnavailable')
}

function isAuthRequest(url?: string) {
  return url === '/api/v1/auth/login'
    || url === '/api/v1/auth/register'
    || url === '/api/v1/auth/refresh'
}
