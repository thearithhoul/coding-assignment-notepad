import { defineStore } from "pinia";
import { ref } from "vue";

export const useNoteCountStore = defineStore("noteCount", () => {
  const allNoteCount = ref(0);
  const pinNoteCount = ref(0);
  const trashNoteCount = ref(0);

  function setCount(filter: number, count: number) {
    switch (filter) {
      case 0:
        allNoteCount.value = count;
        break;
      case 1:
        pinNoteCount.value = count;
        break;
      case 2:
        trashNoteCount.value = count;
        break;
    }
  }

  return { allNoteCount, pinNoteCount, trashNoteCount, setCount };
});
