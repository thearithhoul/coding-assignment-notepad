import { defineComponent } from 'vue'

const icons = {
  back: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M15 19 8 12l7-7"/></svg>`,
  pin: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 17v4"/><path d="M8.5 3h7l.6 5.2a2 2 0 0 0 1.1 1.6l.4.2a1.6 1.6 0 0 1 .9 1.5v.5a1 1 0 0 1-1 1H7.5a1 1 0 0 1-1-1v-.5a1.6 1.6 0 0 1 .9-1.5l.4-.2a2 2 0 0 0 1.1-1.6L9.5 3Z"/></svg>`,
  more: `<svg viewBox="0 0 24 24" fill="currentColor"><circle cx="12" cy="6" r="1.4"/><circle cx="12" cy="12" r="1.4"/><circle cx="12" cy="18" r="1.4"/></svg>`,
}

export default defineComponent({
  name: 'NotepadView',
  data() {
    return {
      title: '',
      content: '',
      pinned: false,
      showMenu: false,
      isSaving: false,
      icons,
      saveTimer: undefined as ReturnType<typeof setTimeout> | undefined,
    }
  },
  computed: {
    wordCount(): number {
      const trimmed = this.content.trim()
      return trimmed ? trimmed.split(/\s+/).length : 0
    },
    wordCountLabel(): string {
      return `${this.wordCount} word${this.wordCount === 1 ? '' : 's'}`
    },
    statusLabel(): string {
      return this.isSaving ? 'Saving…' : 'All changes saved'
    },
  },
  created() {
    const id = this.$route.params.id
    if (id && id !== 'new') {
      this.title = 'Trip to Kyoto — packing list'
      this.content =
        'Passport, rail pass, the good walking shoes, adapter, gift for the Tanakas, spare SD cards.\n\nCheck in with Aya about the ryokan address before we land — last time the taxi driver had no idea.'
      this.pinned = true
    }
  },
  beforeUnmount() {
    if (this.saveTimer) clearTimeout(this.saveTimer)
  },
  methods: {
    onBack() {
      this.$router.push('/')
    },
    onEdit() {
      this.isSaving = true
      if (this.saveTimer) clearTimeout(this.saveTimer)
      this.saveTimer = setTimeout(() => {
        this.isSaving = false
      }, 700)
    },
    onTogglePin() {
      this.pinned = !this.pinned
    },
    onDelete() {
      this.showMenu = false
      this.$router.push('/')
    },
  },
})
