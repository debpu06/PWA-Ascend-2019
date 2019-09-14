var Campaign = {
    init: function () {
    },
    sendTestMailsForTriggers: function (trigger) {
        var form = $(document).find(".jsQuickSendMailForm").serializeArray();
        form.push({ name: 'trigger', value: trigger });
        $('.quick-send-mail-result').text("Please, wait a second");
        $.ajax({
            type: "POST",
            url: "/CampaignTriggerPage/SendTestMailsForTriggers",
            data: $.param(form),
            success: function (result) {
                $('.quick-send-mail-result').text("Send mail result: " + result);
            }
        });
    },
    subscribeEmail: function () {
        var form = $(document).find(".jsSubscribeEmail").serialize();
        $('.subscribe-notification').text("Please, wait a second!");
        $.ajax({
            type: "POST",
            url: "/Start/Subscribe",
            data: $.param(form),
            success: function (result) {
                console.log(result);
                $('.subscribe-notification').text(result).delay(3000).slideUp(500);
                $(".jsSubscribeEmail").val("");
            }
        });
    }
}