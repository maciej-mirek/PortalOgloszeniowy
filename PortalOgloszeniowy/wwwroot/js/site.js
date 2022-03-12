// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*
function DeleteAdvert(id) {
    console.log(id);
    $.ajax({
        url: 'advert/DeleteAdvert/'+ id,
        type: 'GET',
        success: function () {
           // window.location.reload();

        }
    })
}

function PremiumAdvert(id1) {
    console.log(id1);
    $.ajax({
        url: 'advert/PremiumAdvert/' + id1,
        type: 'GET',
        success: function () {
           // window.location.reload();

        }
    })
}
*/


$(".number-info").click(function () {

    $(".number-info").addClass("d-none");
    $(".number").removeClass("d-none");
})