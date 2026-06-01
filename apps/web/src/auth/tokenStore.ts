let accessToken: string | null = null

export const tokenStore = {
  getAccessToken: () => accessToken,
  setAccessToken: (token: string) => {
    accessToken = token
  },
  clear: () => {
    accessToken = null
  },
}
