import { defineComponent } from 'vue'

export default defineComponent({
  name: 'LoginView',
  data() {
    return {
      username: '',
      password: '',
      showPassword: false,
      rememberMe: false,
      loading: false,
      error: '',
    }
  },
  methods: {
    async onSubmit() {
      this.error = ''
      this.loading = true

      try {
        const response = await fetch(
          `${import.meta.env.VITE_API_BASE_URL}/v1/auth/login/user-password`,
          {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
              username: this.username,
              password: this.password,
            }),
          },
        )

        if (!response.ok) {
          const message = await response.text()
          throw new Error(message || 'Invalid username or password.')
        }

        const tokens = await response.json()
        localStorage.setItem('accessToken', tokens.accessToken)
        this.$router.push('/')
      } catch (err) {
        this.error = err instanceof Error ? err.message : 'Something went wrong.'
      } finally {
        this.loading = false
      }
    },
  },
})
