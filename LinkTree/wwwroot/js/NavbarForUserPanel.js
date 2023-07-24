const subMenu = document.getElementById("subMenu");
const subNav = document.getElementById("suNav");
const hamburgerEI = document.querySelector(".hamburger");
let prevScrollPos = window.scrollY
let isSubMenuOpen = false;
let isSubNavOpen = false;
const threshold = 20;


function toggleNav() {
    if (isSubNavOpen) {
        subNav.classList.remove("open-menu");
        hamburgerEI.classList.toggle("hamburger--open");
        isSubNavOpen = false;
    }
    else {
        hamburgerEI.classList.toggle("hamburger--open");
        subNav.classList.add("open-menu");
        isSubNavOpen = true;

        if (isSubMenuOpen) {
            subMenu.classList.remove("open-menu");
            isSubMenuOpen = false;
        }
    }
}

function toggleMenu() {
    if (isSubMenuOpen) {
        subMenu.classList.remove("open-menu");
        isSubMenuOpen = false;
    } else {
        subMenu.classList.add("open-menu");
        isSubMenuOpen = true;

        if (isSubNavOpen) {
            subNav.classList.remove("open-menu");
            hamburgerEI.classList.toggle("hamburger--open");
            isSubNavOpen = false;
        }
    }
}