import { defineStore } from "pinia";
import { computed, ref } from "vue";
import type { tokenResponcedto } from "@/api/model/authdto";
import { publicHttpClient } from "@/api/httpclient";

const authEndpoint = 'api/v1/auth'

export const useAuthStore = defineStore("auth", () => {
  const token = ref<string | null>(localStorage.getItem('accessToken'))
  const refreshToken = ref<string | null>(localStorage.getItem('refreshToken'))
  const expiresAt = ref<string | null>(localStorage.getItem('expiresAt'))

  const isLoggedIn = computed(() => !!token.value)

  // Concurrent requests that fire while the token is expired share one in-flight
  // refresh call instead of each burning the (single-use) refresh token.
  let refreshPromise: Promise<string | null> | null = null

  function setTokens(tokens: tokenResponcedto) {
    token.value = tokens.accessToken
    refreshToken.value = tokens.refreshToken
    expiresAt.value = tokens.expiresAt
    localStorage.setItem('accessToken', tokens.accessToken)
    localStorage.setItem('refreshToken', tokens.refreshToken)
    localStorage.setItem('expiresAt', tokens.expiresAt)
  }

  function clearToken() {
    token.value = null
    refreshToken.value = null
    expiresAt.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('expiresAt')
  }

  function isTokenExpired(): boolean {
    if (!expiresAt.value) return false
    return new Date(expiresAt.value).getTime() <= Date.now()
  }

  async function refreshAccessToken(): Promise<string | null> {
    if (!refreshToken.value) {
      clearToken()
      return null
    }

    try {
      const { data } = await publicHttpClient.post<tokenResponcedto>(`${authEndpoint}/refrash`, {
        refreshToken: refreshToken.value,
      })
      setTokens(data)
      return data.accessToken
    } catch {
      clearToken()
      return null
    }
  }

  // Returns a usable access token: the stored one if it hasn't hit `expiresAt` yet,
  // otherwise exchanges the stored refresh token for a new one first.
  async function getValidAccessToken(): Promise<string | null> {
    if (!token.value) return null

    if (!isTokenExpired()) {
      return token.value
    }

    if (!refreshPromise) {
      refreshPromise = refreshAccessToken().finally(() => {
        refreshPromise = null
      })
    }
    return refreshPromise
  }

  return { token, isLoggedIn, setTokens, clearToken, getValidAccessToken }
})
