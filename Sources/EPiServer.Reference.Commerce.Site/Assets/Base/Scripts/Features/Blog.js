var Blog = {
    init: function () {
        $(document).on("click", ".get-blog-comment", Blog.getBlogComment);
    },
    getBlogComment: function (e) {
        e.preventDefault();
        var page = $(e.target).attr("pageIndex");
        Blog.changePageComment(page);
        return false;
    },
    changePageComment: function (page) {

        var form = $(document).find('.jsBlogPagingForm');
        $('#pageIndex').val(page);
        $.ajax({
            type: "POST",
            url: "/BlogCommentBlock/GetComment",
            data: form.serialize(),
            success: function (response) {
                $('#blogCommentBlock').replaceWith($(response).find('#blogCommentBlock'));
            },
            failure: function (response) {
                console.log(response);
            },
            error: function (response) {
                console.log(response);
            }
        });
    },
    changeBlogListPageSize: function (pageSize) {
        var form = $(document).find('.jsGetBlogItemListPage');
        $('#PageSize').val(pageSize);
        Blog.getBlogList();
    },
    changeBlogListPage: function (page) {
        var form = $(document).find('#jsGetBlogItemListPage');
        $('#PageIndex').val(page);
        Blog.getBlogList();
    },
    getBlogList: function () {
        var form = $(document).find('#jsGetBlogItemListPage');
        $.ajax({
            type: "POST",
            url: "/BlogListBlock/GetItemList",
            data: form.serialize(),
            success: function (response) {
                $('#main').html($(response));
                $("html, body").animate({ scrollTop: 0 }, "slow");
            },
            failure: function (response) {
                console.log(response);
            },
            error: function (response) {
                console.log(response);
            }
        });

    },
    toJson: function (form) {
        var o = {};
        var a = $(form).serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return JSON.stringify(o);
    }
}
