import { defineComponent } from 'vue'
import ShellView from './Shell/ShellView.vue'

interface Note {
  id: number
  title: string
  preview: string
  updatedAt: string
  pinned: boolean
}

const notes: Note[] = [
  {
    id: 1,
    title: 'Trip to Kyoto — packing list',
    preview:
      'Passport, rail pass, the good walking shoes, adapter, gift for the Tanakas, spare SD cards.',
    updatedAt: 'Updated 2h ago',
    pinned: true,
  },
  {
    id: 2,
    title: 'Grocery list for the weekend',
    preview: 'Oat milk, sourdough, coffee beans, tomatoes, basil, the good parmesan.',
    updatedAt: 'Updated 5h ago',
    pinned: true,
  },
  {
    id: 3,
    title: 'Standup notes — Tuesday',
    preview:
      'Finished the export flow, blocked on API keys from the platform team, pairing with Sam at 2.',
    updatedAt: 'Updated yesterday',
    pinned: false,
  },
  {
    id: 4,
    title: '1:1 with Priya',
    preview: 'Promo case is in good shape. Wants more design review reps before Q3. Follow up Friday.',
    updatedAt: 'Updated yesterday',
    pinned: false,
  },
  {
    id: 5,
    title: 'Book: Klara and the Sun',
    preview: 'The narrator\'s limited point of view is doing so much quiet work — reread ch. 4.',
    updatedAt: 'Updated 2 days ago',
    pinned: false,
  },
  {
    id: 6,
    title: 'Recipe: brown butter cookies',
    preview: 'Brown the butter longer than feels right. Chill dough overnight, not just an hour.',
    updatedAt: 'Updated 3 days ago',
    pinned: true,
  },
  {
    id: 7,
    title: 'Ideas for the balcony garden',
    preview: 'Cherry tomatoes, basil, a trellis for the peas. Ask Dad about the self-watering pots.',
    updatedAt: 'Updated 4 days ago',
    pinned: false,
  },
  {
    id: 8,
    title: 'Apartment lease renewal',
    preview: 'Renewal offer came in $40 over last year. Worth asking if they\'ll hold flat for a 2-year term.',
    updatedAt: 'Updated 5 days ago',
    pinned: false,
  },
]

export default defineComponent({
  name: 'DashboardView',
  components: { ShellView },
  data() {
    return {
      notes,
      pinIcon: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 17v4"/><path d="M8.5 3h7l.6 5.2a2 2 0 0 0 1.1 1.6l.4.2a1.6 1.6 0 0 1 .9 1.5v.5a1 1 0 0 1-1 1H7.5a1 1 0 0 1-1-1v-.5a1.6 1.6 0 0 1 .9-1.5l.4-.2a2 2 0 0 0 1.1-1.6L9.5 3Z"/></svg>`,
      menuIcon: `<svg viewBox="0 0 24 24" fill="currentColor"><circle cx="12" cy="6" r="1.4"/><circle cx="12" cy="12" r="1.4"/><circle cx="12" cy="18" r="1.4"/></svg>`,
    }
  },
  methods: {
    onOpenNote(id: number) {
      this.$router.push(`/notes/${id}`)
    },
    onNewNote() {
      this.$router.push('/notes/new')
    },
  },
})
