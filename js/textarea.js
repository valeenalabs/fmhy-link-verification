export function autoResizeTextArea(textAreaElement) {
    textAreaElement.style.height = 'auto';
    textAreaElement.style.height = textAreaElement.scrollHeight + 'px';
    
}