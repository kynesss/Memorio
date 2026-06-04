import { useEffect, useRef, useState, type ClipboardEvent, type DragEvent, type KeyboardEvent } from 'react'
import { useTranslation } from 'react-i18next'
import type { CardMedia } from '../../types/flashcards'
import type { CardContentImage, CardContentSegment } from '../../utils/cardContent'
import { createPendingImage, isSupportedImage, parseCardContent, readImageFile, serializeEditorImageToken } from '../../utils/cardContent'

const maxMediaFileSize = 10 * 1024 * 1024
const editingMarker = '\u200B'

interface CardContentFieldProps {
  label: string
  placeholder: string
  value: string
  mediaItems?: CardMedia[]
  pendingImages: CardContentImage[]
  autoFocus?: boolean
  onChange: (value: string) => void
  onPendingImagesChange: (images: CardContentImage[]) => void
  onRemovePersistedImage: (mediaId: string) => void
  onError: (message: string) => void
}

export function CardContentField({
  label,
  placeholder,
  value,
  mediaItems = [],
  pendingImages,
  autoFocus,
  onChange,
  onPendingImagesChange,
  onRemovePersistedImage,
  onError,
}: CardContentFieldProps) {
  const { t } = useTranslation()
  const editorRef = useRef<HTMLDivElement>(null)
  const initialValue = useRef(value)
  const initialMediaItems = useRef(mediaItems)
  const initialMediaIds = useRef(
    parseCardContent(value, mediaItems)
      .images
      .map((image) => image.mediaId)
      .filter((mediaId): mediaId is string => Boolean(mediaId)),
  )
  const shouldAutoFocus = useRef(autoFocus)
  const pendingImagesRef = useRef(pendingImages)
  const removedMediaIdsRef = useRef<string[]>([])
  const [isDragging, setIsDragging] = useState(false)
  const [isEmpty, setIsEmpty] = useState(!value)

  useEffect(() => {
    pendingImagesRef.current = pendingImages
  }, [pendingImages])

  useEffect(() => {
    const editor = editorRef.current
    if (!editor) {
      return
    }

    renderContent(editor, parseCardContent(initialValue.current, initialMediaItems.current).segments)
    setIsEmpty(isEditorEmpty(editor))

    if (shouldAutoFocus.current) {
      editor.focus()
      moveCaretToEnd(editor)
    }
  }, [])

  const emitChange = () => {
    const editor = editorRef.current
    if (!editor) {
      return
    }

    setIsEmpty(isEditorEmpty(editor))
    syncImagesWithEditor(editor)
    onChange(serializeEditorContent(editor))
  }

  const addFiles = async (files: File[]) => {
    const imageFiles = files.filter(isSupportedImage)
    if (imageFiles.length === 0) {
      return false
    }

    const acceptedImages: CardContentImage[] = []

    for (const file of imageFiles) {
      if (file.size > maxMediaFileSize) {
        onError(t('cards.media.fileTooLarge'))
        continue
      }

      const image = createPendingImage(file, await readImageFile(file))
      acceptedImages.push(image)
      insertImageAtSelection(image)
    }

    if (acceptedImages.length > 0) {
      const nextPendingImages = [...pendingImagesRef.current, ...acceptedImages]
      pendingImagesRef.current = nextPendingImages
      onPendingImagesChange(nextPendingImages)
      emitChange()
    }

    return true
  }

  const handlePaste = async (event: ClipboardEvent<HTMLDivElement>) => {
    const files = Array.from(event.clipboardData.files)
    if (files.some(isSupportedImage)) {
      event.preventDefault()
      await addFiles(files)
      return
    }

    event.preventDefault()
    insertTextAtSelection(event.clipboardData.getData('text/plain'))
    emitChange()
  }

  const handleDrop = async (event: DragEvent<HTMLDivElement>) => {
    event.preventDefault()
    event.stopPropagation()
    setIsDragging(false)
    await addFiles(Array.from(event.dataTransfer.files))
  }

  const handleKeyDown = (event: KeyboardEvent<HTMLDivElement>) => {
    if ((event.key === 'Backspace' || event.key === 'Delete') && removeAdjacentImage(event.key)) {
      event.preventDefault()
      emitChange()
      return
    }

    if (event.key !== 'Enter') {
      return
    }

    event.preventDefault()
    insertLineBreakAtSelection()
    emitChange()
  }

  const insertImageAtSelection = (image: CardContentImage) => {
    const editor = editorRef.current
    if (!editor) {
      return
    }

    editor.focus()
    const range = getEditorRange(editor)
    range.deleteContents()
    insertImageNode(range, image)
  }

  const syncImagesWithEditor = (editor: HTMLElement) => {
    const imageNodes = Array.from(editor.querySelectorAll<HTMLElement>('[data-card-image]'))
    const pendingIds = new Set(imageNodes.map((node) => node.dataset.pendingId).filter(Boolean))
    const mediaIds = new Set(imageNodes.map((node) => node.dataset.mediaId).filter(Boolean))
    const nextPendingImages = pendingImagesRef.current.filter((image) => pendingIds.has(image.id))

    if (nextPendingImages.length !== pendingImagesRef.current.length) {
      pendingImagesRef.current = nextPendingImages
      onPendingImagesChange(nextPendingImages)
    }

    for (const mediaId of initialMediaIds.current) {
      if (mediaIds.has(mediaId) || removedMediaIdsRef.current.includes(mediaId)) {
        continue
      }

      removedMediaIdsRef.current = [...removedMediaIdsRef.current, mediaId]
      onRemovePersistedImage(mediaId)
    }
  }

  return (
    <label className="block">
      <span className="mb-1.5 block text-xs font-medium text-memorio-muted">{label}</span>
      <div
        className={`relative min-h-36 rounded-[10px] border bg-memorio-input transition ${
          isDragging ? 'border-memorio-primary' : 'border-memorio-border focus-within:border-memorio-primary'
        }`}
      >
        {isEmpty && (
          <span className="pointer-events-none absolute left-4 top-3 text-sm text-memorio-subtle">{placeholder}</span>
        )}
        <div
          ref={editorRef}
          contentEditable
          role="textbox"
          aria-label={label}
          aria-multiline="true"
          onInput={emitChange}
          onPaste={handlePaste}
          onKeyDown={handleKeyDown}
          onDragEnter={() => setIsDragging(true)}
          onDragOver={(event) => event.preventDefault()}
          onDragLeave={() => setIsDragging(false)}
          onDrop={handleDrop}
          className="min-h-36 w-full rounded-[10px] px-4 py-3 text-sm text-memorio-text outline-none empty:before:content-none"
        />
      </div>
    </label>
  )
}

function renderContent(editor: HTMLDivElement, segments: CardContentSegment[]) {
  editor.replaceChildren()

  for (const segment of segments) {
    if (segment.type === 'text') {
      appendText(editor, segment.value)
    } else {
      editor.appendChild(createImageNode(segment.image))
      editor.appendChild(createEditingMarker())
    }
  }
}

function appendText(parent: HTMLElement, value: string) {
  const lines = value.split('\n')
  lines.forEach((line, index) => {
    if (index > 0) {
      parent.appendChild(document.createElement('br'))
    }

    if (line) {
      parent.appendChild(document.createTextNode(line))
    }
  })
}

function createImageNode(image: CardContentImage) {
  const wrapper = document.createElement('span')
  wrapper.dataset.cardImage = 'true'
  wrapper.dataset.imageId = image.id
  wrapper.dataset.src = image.src
  wrapper.dataset.fileName = image.fileName
  if (image.mediaId) {
    wrapper.dataset.mediaId = image.mediaId
  }
  if (image.file) {
    wrapper.dataset.pendingId = image.id
  }
  wrapper.contentEditable = 'false'
  wrapper.className = 'relative my-3 block max-w-full'

  const img = document.createElement('img')
  img.src = image.src
  img.alt = image.fileName
  img.className = 'block max-h-72 max-w-full rounded-lg border border-memorio-border bg-memorio-bg object-contain'

  wrapper.append(img)
  return wrapper
}

function serializeEditorContent(editor: HTMLElement) {
  return serializeNodes(Array.from(editor.childNodes)).replace(/\n{3,}/g, '\n\n').trim()
}

function serializeNodes(nodes: ChildNode[]) {
  return nodes.map(serializeNode).join('')
}

function serializeNode(node: ChildNode): string {
  if (node.nodeType === Node.TEXT_NODE) {
    return (node.textContent ?? '').replaceAll(editingMarker, '')
  }

  if (!(node instanceof HTMLElement)) {
    return ''
  }

  if (node.dataset.cardImage === 'true') {
    return `\n${serializeEditorImageToken({
      id: node.dataset.imageId ?? node.dataset.src ?? '',
      fileName: node.dataset.fileName ?? 'image',
      src: node.dataset.src ?? '',
    })}\n`
  }

  if (node.tagName === 'BR') {
    return '\n'
  }

  return serializeNodes(Array.from(node.childNodes))
}

function insertImageNode(range: Range, image: CardContentImage) {
  const marker = createEditingMarker()
  const fragment = document.createDocumentFragment()
  fragment.append(createImageNode(image), marker)
  range.insertNode(fragment)

  range.setStart(marker, marker.textContent?.length ?? 0)
  range.collapse(true)
  setSelectionRange(range)
}

function createEditingMarker() {
  return document.createTextNode(editingMarker)
}

function removeAdjacentImage(key: 'Backspace' | 'Delete') {
  const range = getCurrentRange()
  if (!range.collapsed) {
    return false
  }

  const candidate = key === 'Backspace'
    ? getPreviousEditableNode(range)
    : getNextEditableNode(range)

  const image = getImageNode(candidate)
  if (!image) {
    return false
  }

  const marker = key === 'Backspace'
    ? image.nextSibling
    : image.previousSibling
  const caretAnchor = key === 'Backspace'
    ? getPreviousEditableNodeFromNode(image)
    : getNextEditableNodeFromNode(image)

  image.remove()
  if (isEditingMarkerNode(marker) && marker instanceof CharacterData) {
    marker.remove()
  }

  placeCaretNear(caretAnchor, key)
  return true
}

function getPreviousEditableNode(range: Range) {
  if (range.startContainer.nodeType === Node.TEXT_NODE) {
    const text = range.startContainer.textContent ?? ''
    if (text.slice(0, range.startOffset).replaceAll(editingMarker, '').length > 0) {
      return null
    }

    return getPreviousEditableNodeFromNode(range.startContainer)
  }

  return (range.startContainer as HTMLElement).childNodes.item(range.startOffset - 1)
}

function getNextEditableNode(range: Range) {
  if (range.startContainer.nodeType === Node.TEXT_NODE) {
    const text = range.startContainer.textContent ?? ''
    if (text.slice(range.startOffset).replaceAll(editingMarker, '').length > 0) {
      return null
    }

    return getNextEditableNodeFromNode(range.startContainer)
  }

  return (range.startContainer as HTMLElement).childNodes.item(range.startOffset)
}

function getPreviousEditableNodeFromNode(node: Node | null) {
  let current = node?.previousSibling ?? null
  while (isIgnorableNode(current)) {
    if (!current) {
      return null
    }
    current = current.previousSibling
  }

  return current
}

function getNextEditableNodeFromNode(node: Node | null) {
  let current = node?.nextSibling ?? null
  while (isIgnorableNode(current)) {
    if (!current) {
      return null
    }
    current = current.nextSibling
  }

  return current
}

function getImageNode(node: Node | null) {
  return node instanceof HTMLElement && node.dataset.cardImage === 'true' ? node : null
}

function isIgnorableNode(node: Node | null) {
  return node?.nodeType === Node.TEXT_NODE && (node.textContent ?? '').replaceAll(editingMarker, '').length === 0
}

function isEditingMarkerNode(node: Node | null) {
  return node?.nodeType === Node.TEXT_NODE && (node.textContent ?? '') === editingMarker
}

function placeCaretNear(anchor: Node | null, key: 'Backspace' | 'Delete') {
  const range = document.createRange()

  if (anchor?.nodeType === Node.TEXT_NODE) {
    const offset = key === 'Backspace' ? anchor.textContent?.length ?? 0 : 0
    range.setStart(anchor, offset)
  } else if (anchor?.parentNode) {
    const offset = Array.from(anchor.parentNode.childNodes).indexOf(anchor as ChildNode) + (key === 'Backspace' ? 1 : 0)
    range.setStart(anchor.parentNode, offset)
  } else {
    const editor = document.activeElement
    if (editor instanceof HTMLElement) {
      range.selectNodeContents(editor)
      range.collapse(key === 'Delete')
    } else {
      return
    }
  }

  range.collapse(true)
  setSelectionRange(range)
}

function insertTextAtSelection(value: string) {
  const range = getCurrentRange()
  range.deleteContents()
  const fragment = document.createDocumentFragment()
  const lines = value.split('\n')

  lines.forEach((line, index) => {
    if (index > 0) {
      fragment.appendChild(document.createElement('br'))
    }
    if (line) {
      fragment.appendChild(document.createTextNode(line))
    }
  })

  range.insertNode(fragment)
  range.collapse(false)
  setSelectionRange(range)
}

function insertLineBreakAtSelection() {
  const range = getCurrentRange()
  range.deleteContents()
  range.insertNode(document.createElement('br'))
  range.collapse(false)
  setSelectionRange(range)
}

function getEditorRange(editor: HTMLElement) {
  const selection = window.getSelection()
  if (selection?.rangeCount) {
    const range = selection.getRangeAt(0)
    if (editor.contains(range.commonAncestorContainer)) {
      return range
    }
  }

  const range = document.createRange()
  range.selectNodeContents(editor)
  range.collapse(false)
  return range
}

function getCurrentRange() {
  const selection = window.getSelection()
  if (selection?.rangeCount) {
    return selection.getRangeAt(0)
  }

  return document.createRange()
}

function setSelectionRange(range: Range) {
  const selection = window.getSelection()
  selection?.removeAllRanges()
  selection?.addRange(range)
}

function moveCaretToEnd(editor: HTMLElement) {
  const range = document.createRange()
  range.selectNodeContents(editor)
  range.collapse(false)
  setSelectionRange(range)
}

function isEditorEmpty(editor: HTMLElement) {
  return serializeEditorContent(editor).length === 0
}
