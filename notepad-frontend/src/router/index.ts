import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/Dashboard/DashboardView.vue'
import NotepadView from '../views/Notepad/NotepadView.vue'
import { useAuthStore } from '@/stores/auth.js';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: DashboardView,
      meta: { requiresAuth: true },
    },
    {
      path: '/notes/new',
      name: 'note-new',
      component: NotepadView,
      meta: { requiresAuth: true },
    },
    {
      path: '/notes/:id',
      name: 'note',
      component: NotepadView,
      meta: { requiresAuth: true },
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/loginPage/LoginView.vue'),
      meta: { guestOnly: true },
    },
    {
      path: '/signin',
      name: 'signin',
      component: () => import('../views/SignInPage/SignInView.vue'),
      meta: { guestOnly: true },
    },
  ],
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
  if (to.meta.guestOnly && auth.isLoggedIn) {
    return { name: 'home' }
  }
});

export default router
