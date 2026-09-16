import { defineComponent } from 'vue'
import ShellView from './Shell/ShellView.vue'
import { getNotePadsApi } from '@/api/notepadapi'
import type { notedPadListResponcedto } from '@/api/model/notepaddto'
import { useNoteFilterStore } from '@/stores/notefilter'
import { useNoteCountStore } from '@/stores/notecount.js'

interface Note {
  id: number
  title: string
  preview: string
  updatedAt: string
  pinned: boolean
}

const PAGE_SIZE = 20

function formatUpdatedAt(iso: string): string {
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) {
    return ''
  }
  return `Updated ${date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' })}`
}

function toNote(item: notedPadListResponcedto): Note {
  return {
    id: item.id,
    title: item.title || 'Untitled note',
    preview: item.subTitle,
    updatedAt: formatUpdatedAt(item.updatedAt),
    pinned: item.isPinned,
  }
}

export default defineComponent({
  name: 'DashboardView',
  components: { ShellView },
  data() {
    return {
      notes: [] as Note[],
      loading: false,
      error: '',
      searchDebounce: undefined as ReturnType<typeof setTimeout> | undefined,
      pinIcon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 17v4"/><path d="M8.5 3h7l.6 5.2a2 2 0 0 0 1.1 1.6l.4.2a1.6 1.6 0 0 1 .9 1.5v.5a1 1 0 0 1-1 1H7.5a1 1 0 0 1-1-1v-.5a1.6 1.6 0 0 1 .9-1.5l.4-.2a2 2 0 0 0 1.1-1.6L9.5 3Z"/></svg>`,
      menuIcon: `<svg viewBox="0 0 24 24" fill="currentColor"><circle cx="12" cy="6" r="1.4"/><circle cx="12" cy="12" r="1.4"/><circle cx="12" cy="18" r="1.4"/></svg>`,
    }
  },
  computed: {
    filterKey(): string | null {
      return useNoteFilterStore().filterKey
    },
    searchQuery(): string {
      return useNoteFilterStore().search
    },
  },
  watch: {
    filterKey: {
      immediate: true,
      handler() {
        this.fetchNotes()
      },
    },
    searchQuery() {
      if (this.searchDebounce) {
        clearTimeout(this.searchDebounce)
      }
      this.searchDebounce = setTimeout(() => {
        this.fetchNotes()
      }, 300)
    },
  },
  beforeUnmount() {
    if (this.searchDebounce) {
      clearTimeout(this.searchDebounce)
    }
  },
  methods: {
    async fetchNotes() {
      this.loading = true
      this.error = ''


      const filterStore = useNoteFilterStore()
      const requestedFilter = filterStore.currFilter
      const requestedFilterKey = filterStore.filterKey
      const requestedSearch = filterStore.search.trim()

      const filterMatches = () => filterStore.currFilter === requestedFilter
      const isStillCurrent = () => filterMatches() && filterStore.search.trim() === requestedSearch

      try {
        const response = await getNotePadsApi({
          page: 1,
          pageSize: PAGE_SIZE,
          search: requestedSearch || undefined,
          filter: requestedFilterKey ? `${requestedFilterKey}=true` : undefined,
        })

        if (filterMatches()) {
          useNoteCountStore().setCount(requestedFilter, response.totalCount)
        }

        if (isStillCurrent()) {
          this.notes = response.items.map(toNote)
        }
      } catch {
        if (isStillCurrent()) {
          this.notes = []
          this.error = 'Could not load notes. Try again.'
        }
      } finally {
        if (isStillCurrent()) {
          this.loading = false
        }
      }
    },
    onOpenNote(id: number) {
      this.$router.push(`/notes/${id}`)
    },
    onNewNote() {
      this.$router.push('/notes/new')
    },
  },
})
