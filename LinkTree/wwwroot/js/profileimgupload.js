var $uploadCrop,
    tempFilename,
    rawImg,
    imageId;

function readFile(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('.upload-demo').addClass('ready');
            $('#cropImagePop').modal('show');
            rawImg = e.target.result;
            console.log("File read success");
        }
        reader.onerror = function (e) {
            console.error("File read error", e.target.error);
        }
        reader.readAsDataURL(input.files[0]);
        console.log("Reading file...");
    }
    else {
        console.log("Sorry - your browser doesn't support the FileReader API");
    }
}

$uploadCrop = $('#upload-demo').croppie({
    viewport: {
        width: 160,
        height: 160,
        type: 'circle'
    },
    enforceBoundary: false,
    enableExif: true
});

$('#cropImagePop').on('shown.bs.modal', function () {
    $('.cr-slider-wrap').prepend('<p>Image Zoom</p>');
    $uploadCrop.croppie('bind', {
        url: rawImg
    }).then(function () {
        console.log('jQuery bind complete');
    });
});

$('#cropImagePop').on('hidden.bs.modal', function () {
    $('.item-img').val('');
    $('.cr-slider-wrap p').remove();
});

$('.item-img').on('change', function () {
    console.log("Image file changed");
    readFile(this);
});

$('.replacePhoto').on('click', function () {
    $('#cropImagePop').modal('hide');
    $('.item-img').trigger('click');
})

$('#cropImageBtn').on('click', function (ev) {
    $uploadCrop.croppie('result', {
        type: 'base64',
        // format: 'jpeg',
        backgroundColor: "#000000",
        format: 'png',
        size: { width: 160, height: 160 }
    }).then(function (resp) {
        $('#item-img-output').attr('src', resp);
        $('#cropImagePop').modal('hide');
        $('.item-img').val('');
        console.log("Image cropped and saved successfully");
    });
});