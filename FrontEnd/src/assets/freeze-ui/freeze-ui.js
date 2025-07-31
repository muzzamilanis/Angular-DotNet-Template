// Basic freeze-ui implementation
(function () {
    window.FreezeUI = function () {
        document.body.classList.add('freeze-ui');
    };

    window.UnFreezeUI = function () {
        document.body.classList.remove('freeze-ui');
    };
})();
