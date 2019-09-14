var Notification = {
    init: function () {
        Notification.get();
        $(document)
            .on('click', '.notification-acknowledged', Notification.acknowledged);
    },
    get: function () {
        var notificationHub = $.connection.notificationHub;
        notificationHub.client.putNotifications = function (notifications) {
            Notification.render(notifications);
        };
        $.connection.hub.start();
    },
    render: function (notifications) {
        var email = $(".notification-container").attr("email");
        var messageIds = [];
        $(".notification-message").each(function () { messageIds.push(this.id); });
        if (notifications.length === 0) {
            // Remove all notifications
            $(".notification-container").empty();
        } 
        else
        {
            $.each(notifications, function () {
                // Put new notification
                if ((this.Acknowledgements.length === 0 
                    || this.Acknowledgements.find(x => x.Email.toLowerCase() === email.toLowerCase()) === undefined
                    || this.Acknowledgements.find(x => x.Email.toLowerCase() === email.toLowerCase() && x.Acknowledged === false) !== undefined)
                    && messageIds.find(i => i === this.ContentGuid) === undefined) {
                    var html = `<div class="alert alert-primary notification-message" role="alert" id="` + this.ContentGuid + `">
                                    <span>` + this.Message + `</span>
                                    <button type="button" class="close notification-acknowledged" data-dismiss="alert" aria-label="Close" id="` + this.ContentGuid + `">
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </div>`;
                    $(html).hide().prependTo(".notification-container").fadeIn(1000);
                }
                // Remove not valid notification 
                if (messageIds.find(i => i === this.ContentGuid) !== undefined
                    && (this.Acknowledgements.find(x => x.Email.toLowerCase() === email.toLowerCase()) === undefined
                        || this.Acknowledgements.find(x => x.Email.toLowerCase() === email.toLowerCase() && x.Acknowledged === true) !== undefined)) {
                    $('div[role="alert"][id="' + this.ContentGuid + '"]').fadeOut(1000).remove();
                }
                // Update notification message
                if (this.Acknowledgements.find(x => x.Email.toLowerCase() === email.toLowerCase() && x.Acknowledged === false) !== undefined
                    && messageIds.find(i => i === this.ContentGuid) !== undefined
                    && this.Message.toLowerCase() !== $('div[role="alert"][id="' + this.ContentGuid + '"] span').first().text().toLowerCase()) {
                    $('div[role="alert"][id="' + this.ContentGuid + '"] span').first().text(this.Message);
                }
            });
            // Remove not existed notification
            $.each(messageIds, function (index, value) {
                if (notifications.find(n => n.ContentGuid === value) === undefined) {
                    $('div[role="alert"][id="' + value +'"]').fadeOut(1000).remove();
                }
            });
        }
    },
    acknowledged: function (e) {
        var contentGuid = e.currentTarget.attributes["id"].value;
        $.ajax({
            type: "POST",
            url: "/NotificationBlock/Acknowledged",
            data: {
                contentGuid: contentGuid
            },
            success: function (result) {
                console.log("Acknowledged");
            }
        });
    }
};