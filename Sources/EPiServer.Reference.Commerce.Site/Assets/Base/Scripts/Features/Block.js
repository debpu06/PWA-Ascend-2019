var Block = {
    init: function () {
        $('.heroblock').click(function (e) {
            Block.trackingHeroBlock(e);
        });

        $('.video-block').on('ended', function (e) {
            Block.trackingVideoBlock(e);
        });
    },
    trackingHeroBlock: function (e) {
        $.ajax({
            url: '/BlocksTracking/TrackHeroBlock',
            type: 'POST',
            data: {
                blockId: $(e.currentTarget).children('div').attr('blockId'),
                blockName: $(e.currentTarget).children('div').attr('name'),
                pageName: $('title').text().replace(' - NOT FOR COMMERCIAL USE', '')
            },
            success: function () {
                console.log("Hero Block clicked: '" + $(e.currentTarget).children('div').attr('name') + "' on page - '" + $('title').text().replace(' - NOT FOR COMMERCIAL USE', '') + "'");
            },
            error: function () {
                console.log("Hero Block clicked tracking error");
            }
        });
    },
    trackingVideoBlock: function (e) {
        $.ajax({
            url: '/BlocksTracking/TrackVideoBlock',
            type: 'POST',
            data: {
                blockId: $(e.currentTarget).attr('blockId'),
                blockName: $(e.currentTarget).attr('name'),
                pageName: $('title').text().replace(' - NOT FOR COMMERCIAL USE', '')
            },
            success: function () {
                console.log("Video Block viewed: '" + $(e.currentTarget).attr('name') + "' on page - '" + $('title').text().replace(' - NOT FOR COMMERCIAL USE', '') + "'");
            },
            error: function () {
                console.log("Video Block viewed tracking error");
            }
        });
    }
}