<script setup lang="ts">
import TrainCategorySelect from "./components/TrainCategorySelect.vue";
</script>

<template>
  <div class="w-screen h-screen">
    <div class="title-bar w-full h-12 bg-neutral-300 shadow-2xs dark:bg-neutral-800 flex items-center justify-center px-4">
      <h1>vagonWEB consist submission tool</h1>
    </div>
    <div class="content m-auto py-4 px-10 w-fit">
      <div>
        <h2>Please search for and select the train you wish to submit a consist for.</h2>
      </div>
      <form class="py-5">
      <TrainCategorySelect class="w-full"/>
      </form>
    </div>
  </div>
</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
