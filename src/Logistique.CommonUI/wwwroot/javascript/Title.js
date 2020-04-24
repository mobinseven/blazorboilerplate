window.SetDocumentTitle = function (title) {
    document.title = title;
};
window.GetPageTitle = function () {
    let title = document.getElementById("PageTitle"); if (title) return title.innerText;
};