import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const useAuthStore = defineStore("auth", ()=>{
const token = ref<string | null>("");

const isLoggedIn = computed(()=> !!token.value )


})