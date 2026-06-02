import i18n from 'i18next'
import LanguageDetector from 'i18next-browser-languagedetector'
import HttpBackend from 'i18next-http-backend'
import { initReactI18next } from 'react-i18next'

const setDocumentLanguage = (language: string) => {
  document.documentElement.lang = language
}

void i18n
  .use(HttpBackend)
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    backend: {
      loadPath: '/locales/{{lng}}/translation.json',
    },
    detection: {
      caches: [],
      order: ['navigator'],
    },
    fallbackLng: 'en',
    interpolation: {
      escapeValue: false,
    },
    load: 'languageOnly',
    supportedLngs: ['en', 'pl'],
  })
  .then(() => setDocumentLanguage(i18n.resolvedLanguage ?? i18n.language))

i18n.on('languageChanged', setDocumentLanguage)

export default i18n
