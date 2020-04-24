window.OpenNav = function () {
    document.getElementById("SideNav").style.bottom = "0";
    document.getElementById("NavButton").classList.remove("fa-bars");
    document.getElementById("NavButton").classList.add("fa-times");
}

window.CloseNav = function () {
    document.getElementById("SideNav").style.bottom = "100vh";
    document.getElementById("NavButton").classList.remove("fa-times");
    document.getElementById("NavButton").classList.add("fa-bars");
}