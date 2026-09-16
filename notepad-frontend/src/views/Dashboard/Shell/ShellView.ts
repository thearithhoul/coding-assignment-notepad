import { defineComponent } from 'vue'
import { useAuthStore } from '@/stores/auth'

type NavKey = 'all' | 'pinned' | 'archived' | 'trash'

interface NavItem {
  key: NavKey
  label: string
  count: number
}

const icons: Record<NavKey | 'search' | 'plus' | 'account', string> = {
  all: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M7 3.5h7l4 4V20a.5.5 0 0 1-.5.5h-11A.5.5 0 0 1 6 20V4a.5.5 0 0 1 .5-.5Z"/><path d="M14 3.5V8h4.5"/><path d="M9 12.5h6M9 15.5h6M9 9.5h3"/></svg>`,
  pinned: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M12 21s6-5.686 6-10.5S15.314 4 12 4s-6 2.686-6 6.5S12 21 12 21Z"/><circle cx="12" cy="10.5" r="2"/></svg>`,
  archived: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round"><path d="M4 8.5h16M4 8.5v10a1 1 0 0 0 1 1h14a1 1 0 0 0 1-1v-10M4 8.5 5.5 4h13L20 8.5"/><path d="M10 12.5h4"/></svg>`,
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
      searchQuery: '',
      activeKey: 'all' as NavKey,
      navItems: [
        { key: 'all', label: 'All notes', count: 12 },
        { key: 'pinned', label: 'Pinned', count: 3 },
        { key: 'archived', label: 'Archived', count: 5 },
        { key: 'trash', label: 'Trash', count: 2 },
      ] as NavItem[],
      icons,
    }
  },
  computed: {
    activeLabel(): string {
      return this.navItems.find((item) => item.key === this.activeKey)?.label ?? 'Notes'
    },
  },
  methods: {
    onSignOut() {
      useAuthStore().clearToken()
      this.$router.push({ name: 'login' })
    },
  },
})
