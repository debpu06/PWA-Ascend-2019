var Search = {
    init: function() {
        $(document)
            //.on('focusin focusout', '.jsSearchInput', Search.focusToggle)
            //.on('keyup', '.jsQuickSearch', Search.quickSearch)
            //.on('focus', '.jsQuickSearch, .jsQuickSearchResult', Search.showResults)
            .on('keyup', '.jsQuickSearch', Search.autoSearch)
            .on('focusin.jsQuickSearchResult click.jsQuickSearchResult',
                function(e) {
                    if ($(e.target).closest('.jsQuickSearchResult, .jsQuickSearch').length) return;
                    $('.jsQuickSearchResult').fadeOut('medium');
                });
        $(window).bind('popstate ',
            function(event) {
                //this will handle browser back/forward navigation
                Search.updatePage(location.search);
            });

        $(document).on('change', '.jsSearchSort', Search.sort)
            .on('change', '.jsSearchFacet', Search.sort)
            .on('click', '.jsSearchFacetRemoveAll', Search.removeAll);

        // Set input width
        $('.js-search-input').on('focusout', function() { $('.search-input').removeClass("is-active"); });
        $('.js-search-input').on('focusin', function() { $('.search-input').addClass("is-active"); });

        $('.js-search-input').focus(function() {
            $(this).addClass('has-focus');
        });
        $('.js-search-input').blur(function() {
            $(this).removeClass('has-focus');
        });


        if ($(".search-option").val() == "QuickSearch") {
            $(document).off('keyup', '.jsQuickSearch', Search.autoSearch)
                .on('focusin focusout', '.jsSearchInput', Search.focusToggle)
                .on('keyup', '.jsQuickSearch', Search.quickSearch)
                .on('focus', '.jsQuickSearch, .jsQuickSearchResult', Search.showResults)
        }
        if ($(".search-option").val() == "AutoSearch") {
            // Auto complete
            Search.initData();
            Search.autoComplete();

            // Show popular search
            $('.jsSearchInput').focus(function() {
                $('.search-box-container')[0].classList.add('is-active');
                Search.showPopularSearch();
            });
            $('.jsSearchInput').blur(function() {
                setTimeout(function() {
                        $('.search-box-container')[0].classList.remove('is-active');
                        if ($('#eac-container-popular').length > 0) {
                            $('#eac-container-popular').remove();
                        }
                    },
                    500);
            });
        }
        //Set default value for confidence when not search by image
        $('#confidence').val('1.00');
    },
    fetchingNewPage: false,
    lastPage: false,
    lastKeyWord: "",
    quickSearch: function() {
        if ($(this).val().length > 1 && Search.lastKeyWord != $(this).val()) {
            var url = $(this).data('url');
            var form = $(this).closest('form');
            var jqXhr = $(this).data('jqXhr');
            if (jqXhr)
                jqXhr.abort();
            Search.lastKeyWord = $(this).val();
            $(this).data('jqXhr',
                $.ajax({
                    type: "POST",
                    url: url,
                    data: form.serialize(),
                    context: this,
                    success: function(result) {
                        $(this).removeData('jqXhr');
                        $('.jsQuickSearchResult').empty();
                        $('.jsQuickSearchResult').append(result);
                    }
                })
            );
        }
    },
    focusToggle: function(e) {
        if (e.type === 'focusin') {
            $('.search-box-container')[0].classList.add('is-active');
        } else {
            $('.search-box-container')[0].classList.remove('is-active');
        }
    },
    showResults: function() {
        $('.jsQuickSearchResult').show();
    },
    infinityScroll: function() {
        $(window).scroll(function() {
            if (Search.fetchingNewPage == true || Search.lastPage) {
                return null;
            }
            if ($(window).scrollTop() >= ($(document).height() - $(window).height()) - 1000) {
                Search.fetchingNewPage = true;
                var form = $(document).find('.jsSearchForm');
                $.ajax({
                    url: Search.getUrlWithFacets(),
                    type: "POST",
                    data: form.serialize(),
                    success: function(result) {
                        Search.fetchingNewPage = false;
                        if ($(result).find('.product').length > 0) {
                            $('.jsSearchPage').replaceWith($(result).find('.jsSearchPage'));
                            $('.products-grid').replaceWith($(result).find('.products-grid'));
                            $('.jsSearchFacets').replaceWith($(result).find('.jsSearchFacets'));
                        } else {
                            Search.lastPage = true;
                        }
                    }
                });
            }
        });
    },
    sort: function() {
        Search.lastPage = false;
        var form = $(document).find('.jsSearchForm');
        $('.jsSearchPage').val(1);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePage(url,
            form.serialize(),
            function() {
                history.pushState({ url: url }, "", url); //put the new url to browser history
            });
    },
    changePage: function(page) {
        var form = $(document).find('.jsSearchForm');
        $('.jsSearchPage').val(page);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePage(url,
            form.serialize(),
            function() {
                history.pushState({ url: url }, "", url); //put the new url to browser history
            });
    },
    changePageSize: function(pageSize) {
        var form = $(document).find('.jsSearchForm');
        $('.jsSearchPageSize').val(pageSize);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePage(url,
            form.serialize(),
            function() {
                history.pushState({ url: url }, "", url); //put the new url to browser history
            });
    },
    changeSort: function(sort) {
        var form = $(document).find('.jsSearchForm');
        $('.jsSearchSort').val(sort);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePage(url,
            form.serialize(),
            function() {
                history.pushState({ url: url }, "", url); //put the new url to browser history
            });
    },
    changeSortDirection: function(sort) {
        var form = $(document).find('.jsSearchForm');
        $('.jsSearchSortDirection').val(sort);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePage(url,
            form.serialize(),
            function() {
                history.pushState({ url: url }, "", url); //put the new url to browser history
            });
    },
    getUrlWithFacets: function() {
        var facets = [];
        $('.jsSearchFacet:input:checked').each(function() {
            var selectedFacet = encodeURIComponent($(this).data('facetkey'));
            facets.push(selectedFacet);
        });
        return Search.getUrl(facets);
    },
    getUrl: function(facets) {
        var urlParams = Search.getUrlParams();
        urlParams.facets = facets ? facets.join(',') : null;
        //var sort = $('.jsSearchSort')[0].value;
        urlParams.sort = '';
        var url = "?";
        for (var key in urlParams) {
            var value = urlParams[key];
            if (value) {
                url += key + '=' + value + '&';
            }
        }
        return url.slice(0, -1); //remove last char
    },
    getUrlParams: function() {
        var match,
            search = /([^&=]+)=?([^&]*)/g, //regex to find key value pairs in querystring
            query = window.location.search.substring(1);

        var urlParams = {};
        while (match = search.exec(query)) {
            urlParams[match[1]] = match[2];
        }
        return urlParams;
    },
    removeAll: function() {
        Search.lastPage = false;
        $('.jsSearchFacet:input:checked').each(function() { $(this).attr('checked', false); });
        var form = $(document).find('.jsSearchForm');
        $('.jsSearchPage').val(1);
        var url = Search.getUrl();
        Search.updatePage(url,
            form.serialize(),
            function() {
                history.pushState({ url: url }, "", url); //put the new url to browser history
            })
    },
    updatePage: function(url, data, onSuccess) {
        $.ajax({
            type: "POST",
            url: url || "",
            data: data,
            success: function(result) {
                $('.toolbar').replaceWith($(result).find('.toolbar'));
                $('.category-products').replaceWith($(result).find('.category-products'));
                $('.jsSearchFacets').replaceWith($(result).find('.jsSearchFacets'));
                if (onSuccess) {
                    onSuccess(result);
                }
            }
        });
    },
    changeView: function(view) {
        var form = $(document).find('.jsSearchForm');
        $('.jsSearchViewSwitcher').val(view);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePage(url,
            form.serialize(),
            function() {
                history.pushState({ url: url }, "", url);
            });
    },
    changePageContent: function(page) {
        var form = $(document).find('.jsSearchContentForm');
        $('.jsSearchContentPage').val(page);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePageContent(url,
            form.serialize(),
            null);
    },
    updatePageContent: function(url, data, onSuccess) {
        $.ajax({
            type: "POST",
            url: url || "",
            data: data,
            success: function(result) {
                $('#contentResult').replaceWith($(result).find('#contentResult'));
                if (onSuccess) {
                    onSuccess(result);
                }
            }
        });
    },
    changePdfContent: function (page) {
        var form = $(document).find('.jsSearchPdfForm');
        $('.jsSearchContentPage').val(page);
        $('.jsSelectedFacet').val($(this).data('facetgroup') + ':' + $(this).data('facetkey'));
        var url = Search.getUrlWithFacets();
        Search.updatePdfContent(url,
            form.serialize(),
            null);
    },
    updatePdfContent: function (url, data, onSuccess) {
        $.ajax({
            type: "POST",
            url: url || "",
            data: data,
            success: function (result) {
                $('#pdfResult').replaceWith($(result).find('#pdfResult'));
                if (onSuccess) {
                    onSuccess(result);
                }
            }
        });
    },
    data: {
        previousSearch: [],
        popularSearch: [],
        suggestions: []
    },
    initData: function() {
        var to = moment().format('YYYY-MM-DD');
        var from = moment().subtract(1, 'years').format('YYYY-MM-DD');
        $.ajax({
            type: 'GET',
            async: false,
            url: "/find_v2/_autocomplete?prefix=",
            success: function(result) {
                $.each(result.hits,
                    function(index, item) {
                        Search.data.suggestions.push(item.query);
                    });
            }
        });
        $.ajax({
            type: 'GET',
            async: false,
            url: "/episerver/Find/proxy/_stats/query/top?from=" + from + "&to=" + to + "&interval=month&size=5",
            success: function(result) {
                if (Object.prototype.toString.call(result) !== "[object String]") {
                    $.each(result.hits,
                        function(index, item) {
                            Search.data.popularSearch.push(item.query);
                        });
                }
            }
        });
        var queryString = Search.getUrlParams();
        if (queryString["search"]) {
            $('#search').val(queryString["search"]);
            if (localStorage.PreviousSearch && localStorage.PreviousSearch.indexOf(queryString["search"]) !== -1) {
                Search.addPreviousSearch(queryString["search"]);
            }
        }
        if (localStorage.PreviousSearch) {
            Search.data.previousSearch = localStorage.PreviousSearch.split(",");
        }
    },
    autoComplete: function() {
        var options = {
            data: {
                "previous": Search.data.previousSearch,
                "suggestions": Search.data.suggestions,
            },
            categories: [
                {
                    listLocation: "previous",
                    header: "Your previous searches"
                },
                {
                    listLocation: "suggestions",
                    header: "Search suggestions"
                }
            ],
            list: {
                maxNumberOfElements: 10,
                match: {
                    enabled: true
                },
                sort: {
                    enabled: true
                },
                onShowListEvent: function() {
                    $('.easy-autocomplete-container')[0].classList.add('is-active');
                },
                onHideListEvent: function() {
                    $('.easy-autocomplete-container')[0].classList.remove('is-active');
                },
                onChooseEvent: function() {
                    var keyword = $("#search").getSelectedItemData();
                    window.location.href = "/Search/Index?search=" + keyword;
                }
            }
        };
        $("#search").easyAutocomplete(options);
    },
    addPreviousSearch: function(keyword) {
        if (typeof (Storage) !== "undefined") {
            if (localStorage.PreviousSearch) {
                var keywords = localStorage.PreviousSearch.split(',');
                if (keywords.indexOf(keyword) == -1) {
                    localStorage.PreviousSearch = keyword + "," + localStorage.PreviousSearch;
                }
            } else {
                localStorage.PreviousSearch = keyword;
            }
        }
    },
    autoSearch: function() {
        if ($('#search').val() == '') {
            Search.showPopularSearch();
        } else {
            $('#eac-container-popular').remove();
        }
        setTimeout(Search.triggerSearch, 5000);
    },
    triggerSearch: function() {
        var queryString = Search.getUrlParams();
        if ($('#search').val().length > 0 && $('#search').val() != queryString["search"]) {
            window.location.href = "/Search/Index?search=" + $('#search').val();
        }
    },
    showPopularSearch: function() {
        if ($('#search').val() == '' && $('#eac-container-popular').length == 0) {
            var html = '<div class="easy-autocomplete-container is-active" id="eac-container-popular"><ul>';
            if (localStorage.PreviousSearch) {
                html += '<div class="eac-category">Your previous searches</div>';
                $.each(Search.data.previousSearch.slice(0, 3),
                    function(index, item) {
                        html += '<li><div class="eac-item previous-item">' + item + '</div></li>';
                    });
            }
            html += '<div class="eac-category">Popular searches</div>';
            $.each(Search.data.popularSearch,
                function(index, item) {
                    html += '<li><div class="eac-item popular-item">' + item + '</div></li>';
                });
            html += '</ul></div>';
            $('.search-box').find('.easy-autocomplete').append(html);

            //Popular Search click
            $('.previous-item').on('click',
                function(e) {
                    window.location.href = "/Search/Index?search=" + e.currentTarget.innerText;
                });
            $('.popular-item').on('click',
                function(e) {
                    window.location.href = "/Search/Index?search=" + e.currentTarget.innerText;
                });
        }
    },
    apiUrl: "https://eastus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description",
    authKey: "38192ad9dc5647d1b4d6328d420ac505",
    imageSizeLimit: 5,
    processImage: function(files) {
        var imageData = "";
        $(".validateErrorMsg").hide();
        Search.inputValidation(files);
    },
    inputValidation: function(files) {
        if (files.length == 1) {
            var errormessage = "";
            var regexForExtension = /(?:\.([^.]+))?$/;
            var extension = regexForExtension.exec(files[0].name)[1];
            var size = files[0].size / 1024 / 1024;
            if ((size > Search.imageSizeLimit)) {
                errorMessage = "Image Size Should be lesser than " + Search.imageSizeLimit + "MB";
                $(".validateErrorMsg").text(errorMessage).show();
                return false;
            } else if ((extension != "jpg" && extension != "png" && extension != "jpeg")) {
                errorMessage = "Uploaded File Should Be An Image";
                $(".validateErrorMsg").text(errorMessage).show();
                return false;
            }
            var reader = new FileReader();
            reader.onload = function() {
                Search.processImage.imageData = reader.result;
                var arrayBuffer = this.result, array = new Uint8Array(arrayBuffer);
                if (typeof (Search.processImage.imageData) == "undefined") {
                    errorMessage = "Upload File A Vaild Image";
                    $(".validateErrorMsg").text(errorMessage).show();
                }
                Search.imageProcess(Search.processImage.imageData);
            };
            reader.readAsDataURL(files[0]);
        }
    },
    imageProcess: function(DataURL) {
        var request = new XMLHttpRequest();
        request.open('POST', Search.apiUrl);
        request.setRequestHeader('Content-Type', 'application/octet-stream');
        request.setRequestHeader('Ocp-Apim-Subscription-Key', Search.authKey);
        request.onreadystatechange = function() {
            if (this.readyState === 4) {
                var result = JSON.parse(this.response);
                if (result.description) {
                    $('#confidence').val(result.description.captions[0].confidence);
                    $('.jsSearchInput').val(result.description.captions[0].text);
                    $('.search-form-submit').submit();
                } else {
                    errorMessage = "Uploaded Image has been failed.";
                    $(".validateErrorMsg").text(errorMessage).show();
                    return false;
                }
            }
        };
        var body = {
            'image': DataURL,
            'locale': 'en_US'
        };
        var response = request.send(Search.makeblob(DataURL));
    },
    makeblob: function(dataURL) {
        var BASE64_MARKER = ';base64,';
        if (dataURL.indexOf(BASE64_MARKER) == -1) {
            var parts = dataURL.split(',');
            var contentType = parts[0].split(':')[1];
            var raw = decodeURIComponent(parts[1]);
            return new Blob([raw], { type: contentType });
        }
        var parts = dataURL.split(BASE64_MARKER);
        var contentType = parts[0].split(':')[1];
        var raw = window.atob(parts[1]);
        var rawLength = raw.length;

        var uInt8Array = new Uint8Array(rawLength);

        for (var i = 0; i < rawLength; ++i) {
            uInt8Array[i] = raw.charCodeAt(i);
        }

        return new Blob([uInt8Array], { type: contentType });
    }
};