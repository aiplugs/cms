(function () {
    $(document).on('beforeAjaxSend.ic', function (_, ajaxSetting, elt) {
        elt = elt[0];
        if (ajaxSetting.type.toLowerCase() === 'post' && elt.name && elt.value) {
            const prefix = (ajaxSetting.data || '').length > 0 ? '&' : '';
            ajaxSetting.data += prefix + elt.name + '=' + encodeURIComponent(elt.value);
        }
    })
}());
