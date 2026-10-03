// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const speciesSelect = document.getElementById("pokemon-species");

speciesSelect?.addEventListener("change", () => {
    speciesSelect.form?.requestSubmit();
});

const nameSearch = document.getElementById("pokemon-name");

nameSearch?.addEventListener("input", () => {
    if (nameSearch.value.trim() === "") {
        nameSearch.form?.requestSubmit();
    }
});