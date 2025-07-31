// ABP SweetAlert integration
(function () {
    if (typeof abp === 'undefined') {
        window.abp = {};
    }

    abp.message = {
        info: function (message, title) {
            Swal.fire({
                icon: 'info',
                title: title || 'Info',
                text: message,
            });
        },
        success: function (message, title) {
            Swal.fire({
                icon: 'success',
                title: title || 'Success',
                text: message,
            });
        },
        warn: function (message, title) {
            Swal.fire({
                icon: 'warning',
                title: title || 'Warning',
                text: message,
            });
        },
        error: function (message, title) {
            Swal.fire({
                icon: 'error',
                title: title || 'Error',
                text: message,
            });
        },
        confirm: function (message, callback) {
            Swal.fire({
                title: 'Are you sure?',
                text: message,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No',
            }).then(function (result) {
                if (callback) {
                    callback(result.isConfirmed);
                }
            });
        },
    };
})();
