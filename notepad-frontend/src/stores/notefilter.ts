import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const useNoteFilterStore = defineStore("filterNote", () => {
  const currFilter = ref(0);
  const search = ref("");

  const filterKey = computed<string | null>(() => {
    switch (currFilter.value) {
      case 1:
        return "isPinned";
      case 2:
        return "isDeleted";
      default:
        return null;
    }
  });

  function setFilter(filter: number) {
    currFilter.value = filter;
  }

  function setSearch(value: string) {
    search.value = value;
  }

  return { currFilter, search, filterKey, setFilter, setSearch };
});
