$(document).ready(function () {
    // Hide the dropdown menu initially
    $(".dropdown-content").hide();

    // Toggle the dropdown menu when clicking on the dropdown toggle button
    $("#dropdownToggle").click(function () {
        $(".dropdown-content").toggle();
    });

    // Hide the dropdown menu when clicking outside of it
    $(document).click(function (event) {
        var dropdown = $(".dropdown-content");
        if (!dropdown.is(event.target) && dropdown.has(event.target).length === 0) {
            dropdown.hide();
        }
    });
});