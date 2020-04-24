window.SaveScrollPosition = function (scrollerId) {
    var LastPage = localStorage.getItem("LastPage");
    var scrollPosition = document.getElementById(scrollerId).scrollTop;
    localStorage.setItem("scrollPosition_" + LastPage, scrollPosition.toString());
};
window.SaveCurrentPagePath = function () {
    var pathName = document.location.pathname;
    localStorage.setItem("LastPage", pathName);
};
window.LoadScrollPosition = function (scrollerId) {
    var pathName = document.location.pathname;
    var scroll = parseInt(localStorage.getItem("scrollPosition_" + pathName));
    if (!isNaN(scroll))
        document.getElementById(scrollerId).scrollTop = scroll;
};
window.ScrollToBottom = function (scrollerId) {
    var elem = document.getElementById(scrollerId);
    elem.scrollTop = elem.scrollHeight;
};