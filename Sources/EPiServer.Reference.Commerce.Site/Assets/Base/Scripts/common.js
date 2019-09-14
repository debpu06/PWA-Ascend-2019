$(document).ready(function () {
    "use strict";
    /* Navigation */
    $(".top-nav:not(.hidden-sub-nav) #nav > li").hover(function () {
        var e = $(this).find(".level0-wrapper");
        e.hide(), e.css("left", "0"), e.stop(true, true).delay(150).fadeIn(300, "easeOutCubic");
    },
        function () {
            $(this).find(".level0-wrapper").stop(true, true).delay(300).fadeOut(300, "easeInCubic");
        });
    slideEffectAjax();
    $("#nav li.level0.drop-menu").mouseover(function () {
        return $(window).width() >= 740 && $(this).children("ul.level1").fadeIn(100), !1;
    }).mouseleave(function () {
        return $(window).width() >= 740 && $(this).children("ul.level1").fadeOut(100), !1;
    }), $("#nav li.level0.drop-menu li").mouseover(function () {
        if ($(window).width() >= 740) {
            $(this).children("ul").css(
                {
                    top: 0,
                    left: "165px"
                });
            var e = $(this).offset();
            e && $(window).width() < e.left + 325
                ? ($(this).children("ul").removeClass("right-sub"), $(this).children("ul").addClass("left-sub"), $(
                    this)
                    .children("ul").css(
                    {
                        top: 0,
                        left: "-167px"
                    }))
                : ($(this).children("ul").removeClass("left-sub"), $(this).children("ul").addClass("right-sub")), $(
                    this).children("ul").fadeIn(100);
        }
    }).mouseleave(function () {
        $(window).width() >= 740 && $(this).children("ul").fadeOut(100);
    }),
        function deleteCartInCheckoutPage() {
            return $(".checkout-cart-index a.btn-remove2,.checkout-cart-index a.btn-remove").on("click",
                function (e) {
                    return e.preventDefault(), confirm(confirm_content) ? void 0 : !1;
                }), !1;
        }

    function slideEffectAjax() {
        //$(document)
        //    .on('mouseenter', '.top-cart-contain', function () {
        //        $(this).find(".top-cart-content").stop(true, true).slideDown();
        //    })
        //    .on('mouseleave', '.top-cart-contain', function () {
        //        $(this).find(".top-cart-content").stop(true, true).slideUp();
        //    })
        //    .on('mouseenter', '.top-market-contain', function () {
        //        $(this).find(".lang-curr").stop(true, true).slideDown();
        //    })
        //    .on('mouseleave', '.top-market-contain', function () {
        //        $(this).find(".lang-curr").stop(true, true).slideUp();
        //    });
        $(document)
            .on('click', '.top-cart-contain .toggle-btn', function () {
                var $content = $(this).closest('.top-cart-contain').find('.mini-cart .top-cart-content');

                if ($content.css('display') == 'block') {
                    $content.css('display', 'none');
                } else {
                    $content.css('display', 'block');
                }
            });
    }

    function deleteCartInSidebar() {
        return is_checkout_page > 0
            ? !1
            : void $("#cart-sidebar a.btn-remove, #mini_cart_block a.btn-remove").each(function () { });
    }

    /* Best-Seller Slider */
    //$("#best-seller-slider .slider-items").owlCarousel(
    //    {
    //        items: 4,
    //        itemsDesktop: [1024, 4],
    //        itemsDesktopSmall: [900, 3],
    //        itemsTablet: [600, 2],
    //        itemsMobile: [320, 1],
    //        navigation: !0,
    //        navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
    //        slideSpeed: 500,
    //        pagination: !0
    //    }),
    $('.recommendations-slider').slick({
        dots: true,
        infinite: false,
        speed: 300,
        slidesToShow: 4,
        slidesToScroll: 4,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 4,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });

    $('.recommendations-slider-similar').slick({
        dots: true,
        infinite: false,
        speed: 300,
        slidesToShow: 3,
        slidesToScroll: 3,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });

    $('.carousel-slider-full-width').slick({
        dots: true,
        infinite: true,
        speed: 600,
        slidesToShow: 1,
        slidesToScroll: 1,
        adaptiveHeight: true,
        autoplay: true
    });

    /* Bag-Seller Slider */
    $("#bag-seller-slider .slider-items").owlCarousel(
        {
            items: 3,
            itemsDesktop: [1024, 4],
            itemsDesktopSmall: [900, 3],
            itemsTablet: [600, 2],
            itemsMobile: [320, 1],
            navigation: !0,
            navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
            slideSpeed: 500,
            pagination: !1
        }),
        $("#bag-seller-slider1 .slider-items").owlCarousel(
            {
                items: 3,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !1
            }),
        /* Featured Slider */
        $("#featured-slider .slider-items").owlCarousel(
            {
                items: 6,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !0
            }),
        /* Recommended Slider */
        $("#recommend-slider .slider-items").owlCarousel(
            {
                items: 6,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !1
            }),
        /* Brand Logo Slider */
        $("#brand-logo-slider .slider-items").owlCarousel(
            {
                autoplay: !0,
                items: 6,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !1,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !0
            }),
        /* Category Description Slider */
        $("#category-desc-slider .slider-items").owlCarousel(
            {
                autoplay: !0,
                items: 1,
                itemsDesktop: [1024, 1],
                itemsDesktopSmall: [900, 1],
                itemsTablet: [600, 1],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !1
            }),
        /* More View Slider */
        $("#more-views-slider .slider-items").owlCarousel(
            {
                autoplay: !0,
                items: 3,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !1
            }),
        /* Related Products Slider */
        $("#related-products-slider .slider-items").owlCarousel(
            {
                items: 4,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !0
            }),
        /* UpSell Products Slider */
        $("#upsell-products-slider .slider-items").owlCarousel(
            {
                items: 4,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !0
            }),
        /* Cross-Sell Products */
        $("#cross-sell-products .slider-items").owlCarousel(
            {
                items: 4,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !0
            }),
        $("#crosssel-products-slider .slider-items").owlCarousel(
            {
                items: 5,
                itemsDesktop: [1024, 4],
                itemsDesktopSmall: [900, 3],
                itemsTablet: [600, 2],
                itemsMobile: [320, 1],
                navigation: !0,
                navigationText: ['<a class="flex-prev"></a>', '<a class="flex-next"></a>'],
                slideSpeed: 500,
                pagination: !1
            }),
        /* Mobile Menu */
        $(document).ready(function (e) {
            e("#mobile-menu").mobileMenu(
                {
                    MenuWidth: 250,
                    SlideSpeed: 300,
                    WindowsMaxWidth: 767,
                    PagePush: !0,
                    FromLeft: !0,
                    Overlay: !0,
                    CollapseMenu: !0,
                    ClassName: "mobile-menu"
                });
        }),
        $(document).ready(function () {
            $(".subDropdown")[0] &&
                $(".subDropdown").on("click",
                    function () {
                        $(this).toggleClass("plus"), $(this).toggleClass("minus"), $(this).parent().find("ul")
                            .slideToggle();
                    });
        });
});
var isTouchDevice = "ontouchstart" in window || navigator.msMaxTouchPoints > 0;
$(window).on("load", function () {
    isTouchDevice &&
        $("#nav a.level-top").on("click",
            function () {
                if ($t = $(this), $parent = $t.parent(), $parent.hasClass("parent")) {
                    if (!$t.hasClass("menu-ready"))
                        return $("#nav a.level-top").removeClass("menu-ready"), $t.addClass("menu-ready"), !1;
                    $t.removeClass("menu-ready");
                }
            }), $().UItoTop();
}),
    /* ToTop */
    function (e) {
        $.fn.UItoTop = function (t) {
            var n = {
                text: "",
                min: 200,
                inDelay: 600,
                outDelay: 400,
                containerID: "toTop",
                containerHoverID: "toTopHover",
                scrollSpeed: 1200,
                easingType: "linear"
            },
                i = e.extend(n, t),
                s = "#" + i.containerID,
                o = "#" + i.containerHoverID;
            $("body").append('<a href="#" id="' + i.containerID + '">' + i.text + "</a>"), $(s).hide().on("click",
                function () {
                    return $("html, body").animate(
                        {
                            scrollTop: 0
                        },
                        i.scrollSpeed,
                        i.easingType), $("#" + i.containerHoverID, this).stop().animate(
                            {
                                opacity: 0
                            },
                            i.inDelay,
                            i.easingType), !1
                }).prepend('<span id="' + i.containerHoverID + '"></span>').hover(function () {
                    $(o, this).stop().animate(
                        {
                            opacity: 1
                        },
                        600,
                        "linear");
                },
                function () {
                    $(o, this).stop().animate(
                        {
                            opacity: 0
                        },
                        700,
                        "linear")
                }), $(window).scroll(function () {
                    var t = e(window).scrollTop();
                    "undefined" == typeof document.body.style.maxHeight &&
                        $(s).css(
                            {
                                position: "absolute",
                                top: e(window).scrollTop() + e(window).height() - 50
                            }), t > i.min ? $(s).fadeIn(i.inDelay) : $(s).fadeOut(i.Outdelay);
                });
        }
    }($), $(document).ready(function () { }), $.extend($.easing,
        {
            easeInCubic: function (e, t, n, i, s) {
                return i * (t /= s) * t * t + n;
            },
            easeOutCubic: function (e, t, n, i, s) {
                return i * ((t = t / s - 1) * t * t + 1) + n;
            }
        }),
    /* Accordion */
    function (e) {
        e.fn.extend(
            {
                accordion: function () {
                    return this.each(function () { })
                }
            });
    }($), $(function (e) {
        e(".accordion").accordion(), e(".accordion").each(function () {
            var t = e(this).find("li.active");
            t.each(function (n) {
                e(this).children("ul").css("display", "block"), n == t.length - 1 && e(this).addClass("current");
            });
        });
    }),
    function (e) {
        e.fn.extend(
            {
                accordionNew: function () {
                    return this.each(function () {
                        function t(t, i) {
                            e(t).parent(l).siblings().removeClass(s).children(a).slideUp(c), e(t).siblings(a)[i || o](
                                "show" == i ? c : !1,
                                function () {
                                    e(t).siblings(a).is(":visible")
                                        ? e(t).parents(l).not(n.parents()).addClass(s)
                                        : e(t).parent(l).removeClass(s), "show" == i &&
                                        e(t).parents(l).not(n.parents()).addClass(s), e(t).parents().show();
                                });
                        }

                        var n = e(this),
                            i = "accordiated",
                            s = "active",
                            o = "slideToggle",
                            a = "ul, div",
                            c = "fast",
                            l = "li";
                        if (n.data(i)) return !1;
                        e.each(n.find("ul, li>div"),
                            function () {
                                e(this).data(i, !0), e(this).hide();
                            }), e.each(n.find("em.open-close"),
                                function () {
                                    e(this).on("click",
                                        function () {
                                            return void t(this, o);
                                        }), e(this).bind("activate-node",
                                            function () {
                                                n.find(a).not(e(this).parents()).not(e(this).siblings()).slideUp(c), t(this,
                                                    "slideDown");
                                            });
                                });
                        var r = location.hash ? n.find("a[href=" + location.hash + "]")[0] : n.find("li.current a")[0];
                        r && t(r, !1);
                    });
                }
            });
    }($), $(function (e) {
        e(".accordion").accordion(), e(".accordion").each(function () {
            var t = e(this).find("li.active");
            t.each(function (n) {
                e(this).children("ul").css("display", "block"), n == t.length - 1 && e(this).addClass("current");
            });
        });
    });