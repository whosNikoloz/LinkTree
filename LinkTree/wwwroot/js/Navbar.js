const subMenu = document.getElementById("subMenu");
const subNav = document.getElementById("suNav");
const hamburgerEI = document.querySelector(".hamburger");
let prevScrollPos = window.scrollY
let isSubMenuOpen = false;
let isSubNavOpen = false;
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
        subNav.classList.remove("open-menu"); // Hide the sub-nav when scrolling down
        isSubNavOpen = false;
        subMenu.classList.remove("open-menu"); // Hide the sub-menu when scrolling down
        isSubMenuOpen = false;
        hamburgerEI.classList.remove("hamburger--open"); // Remove the open class from the hamburger when scrolling down
    }

    prevScrollPos = currentScrollPos;
};
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