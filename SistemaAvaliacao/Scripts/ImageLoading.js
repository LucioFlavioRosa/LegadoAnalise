$(function () {
    $("#fileLoader").change(function () {
        alert("hahaha");
        readURL(this);
    });
    });

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
//alert(e.target.result);
$('#fotoCadastro').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}