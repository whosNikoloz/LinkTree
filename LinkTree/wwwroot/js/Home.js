document.querySelectorAll('a[data-scroll-to]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();

        const sectionId = this.getAttribute('data-scroll-to');
        const section = document.getElementById(sectionId);

        if (section) {
            const offset = -100; // Adjust the offset value as needed
            const sectionPosition = section.getBoundingClientRect().top + window.scrollY;
            const offsetPosition = sectionPosition + offset;

            window.scrollTo({
                top: offsetPosition,
                behavior: 'smooth'
            });
        }
    });
});