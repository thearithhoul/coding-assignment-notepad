import { defineComponent } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useNoteFilterStore } from '@/stores/notefilter'
import { useNoteCountStore } from '@/stores/notecount'

type NavKey = 'all' | 'pinned' | 'trash'

interface NavItem {
  key: NavKey
  label: string
  count: number
  filter: number
}

type NavItemMeta = Omit<NavItem, 'count'>

const icons: Record<NavKey | 'search' | 'plus' | 'account', string> = {
  all: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M7 3.5h7l4 4V20a.5.5 0 0 1-.5.5h-11A.5.5 0 0 1 6 20V4a.5.5 0 0 1 .5-.5Z"/><path d="M14 3.5V8h4.5"/><path d="M9 12.5h6M9 15.5h6M9 9.5h3"/></svg>`,
  pinned: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M12 21s6-5.686 6-10.5S15.314 4 12 4s-6 2.686-6 6.5S12 21 12 21Z"/><circle cx="12" cy="10.5" r="2"/></svg>`,
  trash: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M5 7h14M9.5 7V5a1 1 0 0 1 1-1h3a1 1 0 0 1 1 1v2M7 7l.8 12.1a1 1 0 0 0 1 .9h6.4a1 1 0 0 0 1-.9L17 7"/><path d="M10 11v6M14 11v6"/></svg>`,
  search: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="6.5"/><path d="m20 20-3.8-3.8"/></svg>`,
  plus: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="M12 5v14M5 12h14"/></svg>`,
  account: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="8.5" r="3.25"/><path d="M5 20c0-3.6 3.1-6.5 7-6.5s7 2.9 7 6.5"/></svg>`,
}

export default defineComponent({
  name: 'ShellView',
  emits: ['new-note'],
  data() {
    return {
      navMeta: [
        { key: 'all', label: 'All notes', filter: 0 },
        { key: 'pinned', label: 'Pinned', filter: 1 },
        { key: 'trash', label: 'Trash', filter: 2 },
      ] as NavItemMeta[],
      icons,
    }
  },
  computed: {
    searchQuery: {
      get(): string {
        return useNoteFilterStore().search
      },
      set(value: string) {
        useNoteFilterStore().setSearch(value)
      },
    },
    navItems(): NavItem[] {
      const counts = useNoteCountStore()
      const countByFilter: Record<number, number> = {
        0: counts.allNoteCount,
        1: counts.pinNoteCount,
        2: counts.trashNoteCount,
      }
      return this.navMeta.map((item) => ({ ...item, count: countByFilter[item.filter] ?? 0 }))
    },
    activeKey(): NavKey {
      const currFilter = useNoteFilterStore().currFilter
      return this.navItems.find((item) => item.filter === currFilter)?.key ?? 'all'
    },
    activeLabel(): string {
      return this.navItems.find((item) => item.key === this.activeKey)?.label ?? 'Notes'
    },
  },
  methods: {
    onSelectFilter(item: NavItem) {
      useNoteFilterStore().setFilter(item.filter)
    },
    onSignOut() {
      useAuthStore().clearToken()
      this.$router.push({ name: 'login' })
    },
  },
})
