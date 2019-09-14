var ProfileStore = function () {
    
    var $payloadForm,
        $payloadRows;

    function onDeleteIcon() {
        $payloadForm.on('click', '.delete-icon', function (e) {
            e.preventDefault();
            var $deleteIcon = $(this);
            if ($payloadForm.children('.payload-row').length > 0) {

                //Delete row on template
                var parent = $deleteIcon.closest('.payload-row');
                parent.hide();
                parent.find('input[name*=key]').val("removed");
                parent.find('input[name*=value]').val("removed");
                parent.removeClass('payload-row').addClass('payload-row-removed');

                //Update payload data
                $payloadRows = $('.payload-row', $payloadForm);
                updatePayloadData();
            }

        });
    }

    function onAddRow() {
        $('.js-add-row-btn').click(function () {
            var $this = $(this);
            var $rowToClone = $(".payload-row").first().clone();
            var $clone = $rowToClone.clone();
            $clone.find('input').each(function () {
                var $this = $(this);
                $this.val('');
            });
            $clone.css("display", "block");
            $clone.insertBefore($this);
            $payloadRows = $('.payload-row', $payloadForm);
        });
    }

    function onChangeData() {
        $payloadForm.on('change', ':input', function (e) {
            e.preventDefault();
            updatePayloadData();
        });
    }

    function updatePayloadData() {
        var jsonPayload = "";

        if ($payloadRows && $payloadRows.length > 0) {
            var $row;
            for (var i = 0; i < $payloadRows.length; i++) {
                $data = $(":input", $payloadRows[i]);
                if ($data[0].value !== "") {
                    jsonPayload += "\"" + $data[0].value + "\":" + "\"" + $data[1].value + "\",";
                }
            }
        }

        if (jsonPayload.length > 0) {
            jsonPayload = "{" + jsonPayload + "}";
        }

        $("#hfJsonPayload")[0].value = jsonPayload;
    }

    function bindEvents() {

        onDeleteIcon();
        onAddRow();
        onChangeData();
    }

    function init() {

        $payloadForm = $('#payloadForm');
        $payloadRows = $('.payload-row', $payloadForm);

        if ($payloadForm.length > 0 && $payloadRows.length > 0) {
            bindEvents();
        }
    }

    return {
        init: init
    };

}();

var ProfileStoreList = {

    changePage: function (page) {
        var form = $(document).find('.jsProfileStoreForm');
        $('.jsProfileStorePageNumber').val(page);
        ProfileStoreList.updatePage(form);
    },

    changePageSize: function (pageSize) {
        var form = $(document).find('.jsProfileStoreForm');
        $('.jsProfileStorePageSize').val(pageSize);
        ProfileStoreList.updatePage(form);
    },

    updatePage: function (form) {
        $.ajax({
            type: "POST",
            url: "/ProfileStore/ProfileStoreDetail",
            data: form.serialize(),
            success: function (response) {
                $('#profile-store-list').replaceWith($(response).find('#profile-store-list'));
            },
            failure: function (response) {
                console.log(response);
            },
            error: function (response) {
                console.log(response);
            }
        });
    }

}