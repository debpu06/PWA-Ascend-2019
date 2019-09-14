var CommentManager = {
    init: function () {
        CommentManager.get();
        $('.comment-show-more')
            .on('click', CommentManager.showMore);
    },
    page: 1,
    total: 0,
    numberOfItems: 0,
    get: function () {
        $('.btn-loading').show();
        $.ajax({
            url: "/CommentManager/get?page=1&limit=50",
            type: "GET",
            cache: true,
            success: function (result) {
                CommentManager.total = result.Total;
                CommentManager.numberOfItems = result.NumberOfItems;

                $('.comment-numberOfItems').text(CommentManager.numberOfItems);
                $('.comment-total').text(CommentManager.total);

                //Render header
                var html = '<table class="table table-bordered table-content table-comment"><thead><tr>';
                html += "<th>Title</th>";
                html += "<th>Body</th>";
                html += "<th>Rating</th>";
                html += "<th>Author</th>";
                html += "<th>IsVisible</th>";
                html += "<th>Created</th>";
                html += "<th>Approve</th>";
                html += "<th>Delete</th>";
                html += "</tr></thead>";

                //Render content row
                html += "<tbody class='comment-table-body'>";
                $.each(result.Comments, function (index, item) {
                    html += '<tr>';
                    html += '<td><a class="comment-title" commentId="' + item.Id.Id + '">' + item.Title + '</a></td>';
                    html += '<td>' + item.Body + '</td>';
                    html += '<td>' + (item.Rating === 0 ? "" : item.Rating) + '</td>';
                    html += '<td>' + (item.Nickname === "" ? item.Author.Id : item.Nickname) + '</td>';
                    html += '<td class="comment-isvisible" commentId="' + item.Id.Id + '">' + item.IsVisible + '</td>';
                    html += '<td>' + item.AddedOnStr + '</td>';
                    html += item.IsVisible ? '<td><span class="approved-label">Approved</span></td>' : '<td><span class="glyphicon glyphicon-ok approve-comment" commentId="' + item.Id.Id + '"></span></td>';
                    html += '<td><span class="glyphicon glyphicon-remove remove-comment" commentId="' + item.Id.Id + '"></span></td>';
                    html += '</tr>';
                });
                html += '</tbody></table>';

                $('.table-content-info').empty();
                $('.table-content-info').append(html);

                if (CommentManager.total > CommentManager.numberOfItems) {
                    $('.content-footer').show();
                }
                $('.btn-loading').hide();

                $('.approve-comment')
                    .on('click', CommentManager.approve);
                $('.remove-comment')
                    .on('click', CommentManager.delete);
            },
            error: function (result) {
                $('.btn-loading').hide();
            }
        });
    },
    showMore: function () {
        $('.btn-loading').show();
        CommentManager.page += 1;
        $.ajax({
            url: "/CommentManager/get?page=" + CommentManager.page + "&limit=50",
            type: "GET",
            cache: true,
            success: function (result) {
                var html = '';
                $.each(result.Comments, function (index, item) {
                    html += '<tr>';
                    html += '<td><a class="comment-title" commentId="' + item.Id.Id + '">' + item.Title + '</a></td>';
                    html += '<td>' + item.Body + '</td>';
                    html += '<td>' + (item.Rating === 0 ? "" : item.Rating) + '</td>';
                    html += '<td>' + (item.Nickname === "" ? item.Author.Id : item.Nickname) + '</td>';
                    html += '<td class="comment-isvisible" commentId="' + item.Id.Id + '">' + item.IsVisible + '</td>';
                    html += '<td>' + item.AddedOnStr + '</td>';
                    html += item.IsVisible ? '<td><span class="approved-label">Approved</span></td>' : '<td><span class="glyphicon glyphicon-ok approve-comment" commentId="' + item.Id.Id + '"></span></td>';
                    html += '<td><span class="glyphicon glyphicon-remove remove-comment" commentId="' + item.Id.Id + '"></span></td>';
                    html += '</tr>';
                });

                CommentManager.numberOfItems = result.NumberOfItems;
                $('.comment-numberOfItems').text(CommentManager.numberOfItems);

                $('.comment-table-body').append(html);

                if (CommentManager.total > CommentManager.numberOfItems) {
                    $('.content-footer').show();
                } else {
                    $('.content-footer').hide();
                }
                $('.btn-loading').hide();

                $('.approve-comment')
                    .off('click', CommentManager.approve);
                $('.remove-comment')
                    .off('click', CommentManager.delete);

                $('.approve-comment')
                    .on('click', CommentManager.approve);
                $('.remove-comment')
                    .on('click', CommentManager.delete);
            },
            error: function (result) {
                $('.btn-loading').hide();
            }
        });
    },
    approve: function (e) {
        var c = confirm("Are you sure you want to approve a comment?");
        if (c === true) {
            $('.btn-loading').show();
            $.ajax({
                url: '/CommentManager/Approve',
                type: 'POST',
                data: {
                    id: e.currentTarget.attributes["commentId"].value
                },
                success: function (result) {
                    $('td[class="comment-isvisible"][commentId="' + e.currentTarget.attributes["commentId"].value + '"]').text('true');
                    var obj = $(e.currentTarget.parentNode);
                    obj.empty();
                    obj.append('<span class="approved-label">Approved</span>');
                    $('.btn-loading').hide();
                },
                error: function (result) {
                    $('.btn-loading').hide();
                }
            });
        }
    },
    delete: function (e) {
        var c = confirm("Are you sure you want to delete a comment?");
        if (c === true) {
            $('.btn-loading').show();
            $.ajax({
                url: '/CommentManager/Delete',
                type: 'POST',
                dataType: 'text',
                data: {
                    id: e.currentTarget.attributes["commentId"].value
                },
                success: function (result) {
                    CommentManager.get();
                },
                error: function (result) {
                    $('.btn-loading').hide();
                }
            });
        }
    }
};