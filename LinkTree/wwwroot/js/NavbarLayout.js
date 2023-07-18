const subMenu = document.getElementById("subMenu");
let prevScrollPos = window.scrollY
let isSubMenuOpen = false;
const threshold = 20;

window.onscroll = function () {
    const currentScrollPos = window.scrollY;
    const navbar = document.getElementById("navbar");

    if (currentScrollPos < threshold) {
        navbar.style.top = "0";
    } else if (prevScrollPos > currentScrollPos) {
        navbar.style.top = "0";
    } else {
        navbar.style.top = "-100px";
        subMenu.classList.remove("open-menu"); // Hide the sub-menu when scrolling down
        isSubMenuOpen = false;
    }

    prevScrollPos = currentScrollPos;
};

function toggleMenu() {
    if (isSubMenuOpen) {
        subMenu.classList.remove("open-menu");
        isSubMenuOpen = false;
    } else {
        subMenu.classList.add("open-menu");
        isSubMenuOpen = true;
    }
}