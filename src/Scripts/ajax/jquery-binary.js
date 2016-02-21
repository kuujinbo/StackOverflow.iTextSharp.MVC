(function ($) {
    $.fn.downloadFile = function(url, data, requestType) {
        $.ajax({
            url: url,
            data: data,
            type: requestType || 'POST',
            dataType: 'binary'
        })
        .done(function(data, textStatus, jqXHR) {
            var type = jqXHR.getResponseHeader('Content-Type');
            var filename = jqXHR.getResponseHeader('Content-Disposition');
            filename = filename && filename.indexOf('attachment') > -1
                ? filename.replace(/(?:[^=])+=/, '')
                : 'file.bin';
            var blob = new Blob([data], { type: type });
            saveAs(blob, filename);
        })
        .fail(function(jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        })
        ;
        return false;
    };
}(jQuery));