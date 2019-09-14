var accounts = {
    impersonateUser: function (name) {
        $.ajax({
            url: '/SalesRep/ImpersonateUser',
            type: 'post',
            dataType: 'json',
            data: {
                username: name
            },
            success: function (data) {
                if (data) {
                    if (data.success)
                        location.href = '/';
                    else {
                        //show some message
                    }
                }
            },
            error: function (a, b) {
               
            }
        });
    }
}
