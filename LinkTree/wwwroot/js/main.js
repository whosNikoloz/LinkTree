// Get the URL input and Add button elements
const urlInput = document.getElementById('urlInput');
const addButton = document.getElementById('addButton');

// Add an event listener to the URL input
urlInput.addEventListener('input', () => {
    // Enable the Add button if there is some text in the URL input, otherwise disable it
    addButton.disabled = urlInput.value.trim() === '';
});
