import axios, { AxiosError } from 'axios'
import { useAuthStore } from '@/stores/auth'
import router from '@/router'

const baseURL = import.meta.env.VITE_API_BASE_URL

export const publicHttpClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})


export const httpclient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

httpclient.interceptors.request.use(async (config) => {
  const token = await useAuthStore().getValidAccessToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

httpclient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      useAuthStore().clearToken()
      router.push({ name: 'login' })
    }
    return Promise.reject(error)
  },
)
