$(document).ready(function () {
    function KT() {
        var userSession = $("#userSession").val();
        if (userSession) {
            PYT();
        } else {
            top7();
        }
    }
    $(".btn-like").click(function () {
        const like = "rgb(255, 26, 198)";
        const unlike = "rgb(26, 140, 255)";
        let obj = $(this);
        let MaPhim = obj.val();
        let color = obj.css('color');
        let value = 0;
        if (color === like) {
            color = unlike;
        }
        else {
            value = 1;
            color = like;
        }
        obj.css('color', color);
        $.ajax({
            type: "POST",
            url: 'https://localhost:44308/YeuThich/YeuThich',
            data: { value: value, MaPhim: MaPhim },
            success: function () {
                KT();
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        }); 
    });
    function top7() {
        $.ajax({
            type: 'GET',
            url: 'https://localhost:44308/YeuThich/Top7YTPartial', 
            success: function (result) {
                $('#banner-bottom').html(result);
            }
        });
    }
    function PYT() {
        $.ajax({
            type: 'GET',
            url: 'https://localhost:44308/YeuThich/TKPYTPartial',
            success: function (result) {
                $('#banner-bottom').html(result);
            }
        });
    }
    $(".btn-login-like").click(function () {
        window.location.href = 'https://localhost:44308/DangNhapDangKy/DangNhap';
    });
});