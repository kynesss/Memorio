import type { Card, CardMedia } from '../types/flashcards'

export interface CardContentImage {
  id: string
  src: string
  fileName: string
  fileSize?: number
  mediaId?: string
  file?: File
}

export interface ParsedCardContent {
  text: string
  images: CardContentImage[]
  segments: CardContentSegment[]
}

export type CardContentSegment =
  | { type: 'text'; value: string }
  | { type: 'image'; image: CardContentImage }

export interface ImageSourceReplacement {
  imageId: string
  to: string
}

const imageTokenRegex = /!\[([^\]]*)]\(([^)]+)\)/g
const imageIdTokenRegex = /^memorio-image:([a-zA-Z0-9-]+)(?::.*)?$/

export function parseCardContent(value: string, mediaItems: CardMedia[] = []): ParsedCardContent {
  const images: CardContentImage[] = []
  const segments: CardContentSegment[] = []
  let cursor = 0
  let match: RegExpExecArray | null

  imageTokenRegex.lastIndex = 0
  while ((match = imageTokenRegex.exec(value)) !== null) {
    const text = value.slice(cursor, match.index)
    if (text) {
      segments.push({ type: 'text', value: text })
    }

    const alt = match[1]
    const src = match[2]
    const media = mediaItems.find((item) => item.url === src)
    const imageId = getImageId(alt, media?.id ?? src)
    const image = {
      id: imageId,
      src,
      fileName: media?.fileName ?? getFileNameFromUrl(src),
      fileSize: media?.fileSize,
      mediaId: media?.id,
    }

    images.push(image)
    segments.push({ type: 'image', image })
    cursor = match.index + match[0].length
  }

  const trailingText = value.slice(cursor)
  if (trailingText) {
    segments.push({ type: 'text', value: trailingText })
  }

  return {
    text: segments
      .filter((segment): segment is Extract<CardContentSegment, { type: 'text' }> => segment.type === 'text')
      .map((segment) => segment.value)
      .join('')
      .replace(/\n{3,}/g, '\n\n')
      .trim(),
    images,
    segments,
  }
}

export function serializeCardContent(text: string, images: CardContentImage[]) {
  const tokens = images.map((image) => `![${normalizeAltText(image.fileName)}](${image.src})`)
  return [text.trim(), ...tokens].filter(Boolean).join('\n\n')
}

export function serializeEditorImageToken(image: Pick<CardContentImage, 'id' | 'fileName' | 'src'>) {
  return `![memorio-image:${image.id}:${normalizeAltText(image.fileName)}](${image.src})`
}

export function replaceImageSources(value: string, replacements: ImageSourceReplacement[]) {
  const replacementMap = new Map(replacements.map((replacement) => [replacement.imageId, replacement.to]))

  return value.replace(imageTokenRegex, (token, alt: string, src: string) => {
    const replacement = replacementMap.get(getImageId(alt, src))
    return replacement ? token.replace(src, replacement) : token
  })
}

export function hasCardContent(value: string) {
  const parsed = parseCardContent(value)
  return parsed.text.length > 0 || parsed.images.length > 0
}

export function getCardImages(card: Card) {
  const parsedImages = [
    ...parseCardContent(card.front, card.mediaItems).images,
    ...parseCardContent(card.back, card.mediaItems).images,
  ]
  const parsedUrls = new Set(parsedImages.map((image) => image.src))
  const unattachedImages = card.mediaItems
    .filter((media) => !parsedUrls.has(media.url))
    .map((media) => ({
      id: media.id,
      src: media.url,
      fileName: media.fileName,
      fileSize: media.fileSize,
      mediaId: media.id,
    }))

  return [...parsedImages, ...unattachedImages]
}

export function createPendingImage(file: File, src: string): CardContentImage {
  return {
    id: `${Date.now()}-${crypto.randomUUID()}`,
    src,
    file,
    fileName: file.name || 'image',
    fileSize: file.size,
  }
}

export function readImageFile(file: File) {
  return new Promise<string>((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = () => resolve(String(reader.result))
    reader.onerror = () => reject(reader.error)
    reader.readAsDataURL(file)
  })
}

export function isSupportedImage(file: File) {
  return /^image\/(jpeg|png|gif|webp)$/.test(file.type) || /\.(jpe?g|png|gif|webp)$/i.test(file.name)
}

function normalizeAltText(value: string) {
  return value.replace(/[[\]\n\r]/g, ' ').trim() || 'image'
}

function getImageId(alt: string, fallback: string) {
  return imageIdTokenRegex.exec(alt)?.[1] ?? fallback
}

function getFileNameFromUrl(value: string) {
  try {
    const url = new URL(value)
    const segment = url.pathname.split('/').filter(Boolean).at(-1)
    return segment ? decodeURIComponent(segment) : 'image'
  } catch {
    return 'image'
  }
}
