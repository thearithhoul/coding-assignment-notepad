import { defineComponent } from 'vue'
import { isAxiosError } from 'axios'
import { signinApi } from '@/api/authapi'
import { useAuthStore } from '@/stores/auth'

export default defineComponent({
  name: 'SignInView',
  data() {
    return {
      firstName: '',
      lastName: '',
      username: '',
      email: '',
      phoneNumber: '',
      password: '',
      confirmPassword: '',
      showPassword: false,
      showConfirmPassword: false,
      loading: false,
      error: '',
    }
  },
  methods: {
    async onSubmit() {
      this.error = ''

      if (this.password !== this.confirmPassword) {
        this.error = 'Passwords do not match.'
        return
      }

      this.loading = true

      try {
        const tokens = await signinApi({
          firstName: this.firstName,
          lastName: this.lastName,
          username: this.username,
          email: this.email,
          phoneNumber: this.phoneNumber,
          password: this.password,
        })
        useAuthStore().setTokens(tokens)
        this.$router.push('/')
      } catch (err) {
        if (isAxiosError(err)) {
          this.error =
            (typeof err.response?.data === 'string' ? err.response.data : undefined) ||
            'Could not create your account.'
        } else {
          this.error = 'Something went wrong.'
        }
      } finally {
        this.loading = false
      }
    },
  },
})
