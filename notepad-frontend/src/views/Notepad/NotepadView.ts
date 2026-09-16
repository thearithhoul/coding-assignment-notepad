import { defineComponent } from 'vue'
import { createNotePadApi, getNotePadDetailApi, removeNotePadApi, updateNotePadApi } from '@/api/notepadapi'
import type { notedPadsCreateRequestdto } from '@/api/model/notepaddto'

const icons = {
  back: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M15 19 8 12l7-7"/></svg>`,
  pin: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 17v4"/><path d="M8.5 3h7l.6 5.2a2 2 0 0 0 1.1 1.6l.4.2a1.6 1.6 0 0 1 .9 1.5v.5a1 1 0 0 1-1 1H7.5a1 1 0 0 1-1-1v-.5a1.6 1.6 0 0 1 .9-1.5l.4-.2a2 2 0 0 0 1.1-1.6L9.5 3Z"/></svg>`,
  more: `<svg viewBox="0 0 24 24" fill="currentColor"><circle cx="12" cy="6" r="1.4"/><circle cx="12" cy="12" r="1.4"/><circle cx="12" cy="18" r="1.4"/></svg>`,
}

export default defineComponent({
  name: 'NotepadView',
  data() {
    return {
      noteId: null as number | null,
      title: '',
      content: '',
      pinned: false,
      showMenu: false,
      loading: false,
      loadError: false,
      isSaving: false,
      saveError: false,
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
      if (this.isSaving) {
        return 'Saving…'
      }
      if (this.saveError) {
        return "Couldn't save — edit again to retry"
      }
      return 'All changes saved'
    },
  },
  async created() {
    const routeId = this.$route.params.id
    if (routeId && routeId !== 'new') {
      this.loading = true
      try {
        const notepad = await getNotePadDetailApi(Number(routeId))
        this.noteId = notepad.id
        this.title = notepad.title
        this.content = notepad.detail.content
        this.pinned = notepad.isPinned
      } catch {
        this.loadError = true
      } finally {
        this.loading = false
      }
    }
  },
  beforeUnmount() {
    if (this.saveTimer) {
      clearTimeout(this.saveTimer)
    }
  },
  methods: {
    onBack() {
      this.$router.push('/')
    },
    onEdit() {
      this.isSaving = true
      if (this.saveTimer) {
        clearTimeout(this.saveTimer)
      }
      this.saveTimer = setTimeout(() => {
        this.save()
      }, 700)
    },
    async save() {
      const payload: notedPadsCreateRequestdto = {
        title: this.title.trim() || 'Untitled note',
        subTitle: this.content.trim().slice(0, 140),
        isPinned: this.pinned,
        detail: { content: this.content },
      }

      this.isSaving = true
      try {
        if (this.noteId === null) {
          const created = await createNotePadApi(payload)
          this.noteId = created.id
          this.$router.replace(`/notes/${created.id}`)
        } else {
          await updateNotePadApi(this.noteId, payload)
        }
        this.saveError = false
      } catch {
        this.saveError = true
      } finally {
        this.isSaving = false
      }
    },
    onTogglePin() {
      this.pinned = !this.pinned
      if (this.saveTimer) {
        clearTimeout(this.saveTimer)
      }
      this.save()
    },
    async onDelete() {
      this.showMenu = false
      if (this.noteId !== null) {
        try {
          await removeNotePadApi(this.noteId)
        } catch {
          return
        }
      }
      this.$router.push('/')
    },
  },
})
