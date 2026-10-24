// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const systemAlert = document.getElementById("system-alert");
    if (systemAlert) {
        window.setTimeout(function () {
            bootstrap.Alert.getOrCreateInstance(systemAlert).close();
        }, 3000);
    }
});
