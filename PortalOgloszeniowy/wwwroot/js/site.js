// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function DeleteAdvert(id) {
    //console.log(id);
    $.ajax({
        url: 'advert/DeleteAdvert/'+ id,
        type: 'GET',
        success: function () {
            window.location.reload();

        }
    })
}

