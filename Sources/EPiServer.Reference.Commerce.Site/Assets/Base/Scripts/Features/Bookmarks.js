var Bookmarks = {
    init: function () {
        $(document)
            .on('click', '#bookmark-toggle', Bookmarks.bookmark)
            .on('click', '.remove-bookmark', Bookmarks.remove);    
    },
    bookmark: function (e) {
        e.preventDefault();
        if ($('#bookmark-toggle').attr('bookmarked') === undefined) {
            $.ajax({
                type: "POST",
                url: "/Bookmarks/Bookmark",
                data: {
                    contentId: $('#bookmark-toggle').attr('contentid')
                },
                success: function (result) {
                    $('#bookmark-toggle').removeAttr("href");
                    $('#bookmark-toggle').attr('bookmarked', true);
                    $('#bookmark-toggle').html('<span title="Bookmarked" class="glyphicon glyphicon-heart"></span>');
                }
            });
        }
    },
    remove: function (e) {
        e.preventDefault();
        var contentGuid = e.currentTarget.attributes["contentguid"].value;
        $.ajax({
            type: "POST",
            url: "/Bookmarks/Remove",
            data: {
                contentGuid: contentGuid
            },
            success: function (result) {
                var rowId = '#bookmark-' + contentGuid;
                $(rowId).remove();
            }
        });
    }
}