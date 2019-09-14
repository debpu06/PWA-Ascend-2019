var BulkContent = {
    contentInfo: [],
    init: function () {
        BulkContent.getContentTypes();
        BulkContent.getLanguages();
        $('#ContentGroupFilter')
            .on('change', BulkContent.getContentTypes);
        $('.content-type-filter')
            .on('change', BulkContent.getProperties);
        $('.button-apply-filters')
            .on('click', BulkContent.applyFilters);
    },
    getContentTypes: function () {
        var filter = $('#ContentGroupFilter').val();
        $.ajax({
            url: "/content/getcontenttypes/?type=" + filter,
            type: "GET",
            success: function (result) {
                $('.content-type-filter').empty();
                $.each(result,
                    function (index, entry) {
                        if (entry.DisplayName != null) {
                            $('.content-type-filter')
                                .append($('<option>').attr('value', entry.ID).text(entry.DisplayName));
                        } else {
                            $('.content-type-filter').append($('<option>').attr('value', entry.ID).text(entry.Name));
                        }
                    });
                if (typeof $('.content-type-filter').val() !== 'undefined') {
                    BulkContent.getDefaultProperties($('.content-type-filter').val());
                }
            }
        });
    },
    getProperties: function (e) {
        $.ajax({
            url: "/content/getproperties?id=" + e.currentTarget.value,
            type: "GET",
            success: function (result) {
                $('.table-content-info').empty();
                $('.checkbox-content-properties').empty();
                $.each(result, function (index, entry) {
                    $('.checkbox-content-properties').append('<input type="checkbox" value="' + entry.ID + '" name="' + entry.Name + '" />' + " " + entry.Name + '<br/>');
                })
            }
        })
    },
    getDefaultProperties: function (id) {
        $.ajax({
            url: "/content/getproperties?id=" + id,
            type: "GET",
            success: function (result) {
                $('.checkbox-content-properties').empty();
                $.each(result, function (index, entry) {
                    $('.checkbox-content-properties').append('<input type="checkbox" value="' + entry.ID + '" name="' + entry.Name + '" />' + " " + entry.Name + '<br/>');
                })
            }
        })
    },
    getLanguages: function () {
        $.ajax({
            url: "/content/getlanguages",
            type: "GET",
            success: function (result) {
                $.each(result, function (index, entry) {
                    if (entry.DisplayName != null) {
                        $('.content-language-filter').append($('<option>').attr('value', entry.LanguageID).text(entry.DisplayName));
                    }
                    else {
                        $('.content-language-filter').append($('<option>').attr('value', entry.LanguageID).text(entry.Name));
                    }
                })
            }
        })
    },
    applyFilters: function () {
        var contentTypeId = $('.content-type-filter').val();
        var properties = [];
        $('.checkbox-content-properties input:checked').each(function () {
            properties.push($(this).attr("name"));
        });
        var language = $('.content-language-filter').val();
        var keyword = $('.content-name-filter').val();

        $.ajax({
            url: "/content/get?contentTypeId=" + contentTypeId + "&language=" + language + "&properties=" + properties.join() + "&keyword=" + keyword,
            type: "GET",
            success: function (result) {
                BulkContent.contentInfo = result;
                if (result.length > 0) {

                    //Init content info
                    $.each(result, function (index, entry) {
                        BulkContent.contentInfo[index]["Properties"] = {};
                        $.each(properties, function (i, item) {
                            BulkContent.contentInfo[index]["Properties"][item] = entry[item];
                        });
                    });

                    //Render header
                    var html = '<table class="table table-bordered table-content"><thead><tr><th>Id</th><th>Name</th>';
                    $.each(properties, function (i, entry) {
                        html += "<th>" + entry + "</th>";
                    });
                    html += "</tr></thead>";

                    html += "<tbody>";

                    //Render Edit All Row
                    html += '<tr><td></td>';
                    html += '<td>' + BulkContent.generateInputTagForProperty(-1, "Name", "") + '</td>';
                    $.each(properties, function (i, item) {
                        if (typeof (result[0][item]) == "boolean") {
                            html += '<td>' + BulkContent.generateInputTagForProperty(-1, item, false) + '</td>';
                        } else {
                            html += '<td>' + BulkContent.generateInputTagForProperty(-1, item, "") + '</td>';
                        }
                    });
                    html += '</tr>';

                    //Render content row
                    $.each(result, function (index, entry) {
                        html += '<tr><td>' + entry.ContentLink.Id + '</td>';
                        html += '<td>' + BulkContent.generateInputTagForProperty(index, "Name", entry.Name) + '</td>';
                        $.each(properties, function (i, item) {
                            html += '<td>' + BulkContent.generateInputTagForProperty(index, item, entry[item]) + '</td>';
                        });
                        html += '</tr>';
                    });
                    html += '</tbody></table>';

                    $('.table-content-info').empty();
                    $('.table-content-info').append("<button style='margin-bottom: 20px' class='button button-save-content'>Save all</button><br />");
                    $('.table-content-info').append(html);
                    $('.table-content-info').append("<button class='button button-save-content'>Save all</button>");

                    //Set event
                    $('.prop')
                        .on('change', BulkContent.updateContentInfo);
                    $('.button-save-content')
                        .on('click', BulkContent.saveAll);
                } else {
                    $('.table-content-info').empty();
                    $('.table-content-info').append("<div style='text-align: center;padding: 10px;background: #dadada'><h2>No content found</h2></div>");
                }
            }
        })
    },
    generateInputTagForProperty: function (index, property, value) {
        var html = "";
        if (typeof (value) == "boolean") {
            if (value == true) {
                html = "<input class='prop' index=" + index + " name='" + property + "' type='checkbox' checked/>";
            }
            else {
                html = "<input class='prop' index=" + index + " name='" + property + "' type='checkbox' />";
            }
        }
        else {
            if (value == null) {
                value = "";
            }
            html = "<input class='prop' index=" + index + " name='" + property + "' type='text' value='" + value + "' />";
        }
        return html;
    },
    updateContentInfo: function (e) {
        if (e.currentTarget.attributes["index"].value == -1) {
            if (e.currentTarget.type == "checkbox") {
                $('input[class="prop"][name="' + e.currentTarget.attributes["name"].value + '"]').prop("checked", e.currentTarget.checked);
                $.each(BulkContent.contentInfo, function (index, item) {
                    item[e.currentTarget.attributes["name"].value] = e.currentTarget.checked;
                    if (e.currentTarget.attributes["name"].value != "Name") {
                        item["Properties"][e.currentTarget.attributes["name"].value] = e.currentTarget.checked;
                    }
                });
            } else {
                $('input[class="prop"][name="' + e.currentTarget.attributes["name"].value + '"]').val(e.currentTarget.value);
                $.each(BulkContent.contentInfo, function (index, item) {
                    item[e.currentTarget.attributes["name"].value] = e.currentTarget.value;
                    if (e.currentTarget.attributes["name"].value != "Name") {
                        item["Properties"][e.currentTarget.attributes["name"].value] = e.currentTarget.value;
                    }
                });
            }
        }
        else {
            if (e.currentTarget.type == "checkbox") {
                BulkContent.contentInfo[e.currentTarget.attributes["index"].value][e.currentTarget.attributes["name"].value] = e.currentTarget.checked;
                if (e.currentTarget.attributes["name"].value != "Name") {
                    BulkContent.contentInfo[e.currentTarget.attributes["index"].value]["Properties"][e.currentTarget.attributes["name"].value] = e.currentTarget.checked;
                }
            } else {
                BulkContent.contentInfo[e.currentTarget.attributes["index"].value][e.currentTarget.attributes["name"].value] = e.currentTarget.value;
                if (e.currentTarget.attributes["name"].value != "Name") {
                    BulkContent.contentInfo[e.currentTarget.attributes["index"].value]["Properties"][e.currentTarget.attributes["name"].value] = e.currentTarget.value;
                }
            }
        }
    },
    saveAll: function () {
        var properties = [];
        $('.checkbox-content-properties input:checked').each(function () {
            properties.push($(this).attr("name"));
        });

        var content = {
            Contents: BulkContent.contentInfo,
            Properties: properties.join()
        }

        $.ajax({
            url: '/content/updateContent',
            type: 'post',
            contentType: 'application/json',
            data: JSON.stringify(content),
            success: function (result) {
                alert(result);
            },
        });
    }
}