var toggleTheme = function () {

    var theme = localStorage.getItem("theme");

    if (theme == undefined) {
        matches = window.matchMedia('(prefers-color-scheme: dark)').matches;

        if (matches) {
            theme = "dark";
        } else {
            theme = "light";
        }
    }

    var finalTheme = theme == "dark" ? "light" : "dark";

    document.documentElement.setAttribute('data-theme', finalTheme);
    localStorage.setItem("theme", finalTheme);
}