$(document).ready(function () {
    // Fetch the messages from hidden input fields
    var successMessage = $('#successMessage').val();
    var errorMessage = $('#errorMessage').val();

    if (successMessage && successMessage.trim().length > 0) {
        Swal.fire({
            title: 'Success!',
            text: successMessage,
            icon: 'success',
            confirmButtonText: 'OK'
        });
    }

    if (errorMessage && errorMessage.trim().length > 0) {
        Swal.fire({
            title: 'Error!',
            text: errorMessage,
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
});
