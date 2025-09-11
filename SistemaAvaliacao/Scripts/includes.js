/**
 * este script tem como objetivo incluir partes principais do site no lado do cliente
 * quando o sistema for realmente construido nesta base, esse modelo de inclusão deve ser substituido
 * para uma maneira mais apropriada (back-end include, server render)
 */
(function () {

    let proccess = {};

    function _include() {
        let live = $('.client-side-include');
        live.each(function () {
            let _this = $(this);
            let src = _this.attr('data-src');
            if (proccess[src] == null) {
                if (src != null) {
                    proccess[src] = true;
                    $.ajax({
                        url: src,
                        async: false,
                        cache: false,
                        success: function (data) {
                            _this.replaceWith(data);
                            delete proccess[src];
                        }
                    });
                }
            }
        });
    }

    document.body.addEventListener('DOMSubtreeModified', function () {
        _include();
    }, false);

    let jQueryTest = setInterval(function () {
        if (typeof $ !== 'undefined') {
            clearInterval(jQueryTest);
            _include();
        }
    }, 10);
})();