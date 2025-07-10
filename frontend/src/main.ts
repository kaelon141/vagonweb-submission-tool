import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import Lara from '@primeuix/themes/lara';
import { VueQueryPlugin } from '@tanstack/vue-query'
import './style.css'
import App from './App.vue'

const app = createApp(App)

app.use(PrimeVue, {
    theme: {
        preset: Lara
    }
});

app.use(VueQueryPlugin)

app.mount('#app')
