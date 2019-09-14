$(document).ready(function () {
    AddressBook.init();
    Cart.init();
    Checkout.init();
    Market.init();
    Misc.init();
    login.init();
    ProductPage.init();
    Registration.init();
    Search.init();
    Navigation.init();
    Recommendations.init();
    OrdersPage.init();
    Budget.init();
    Blog.init();
    ProfileStore.init();
    Campaign.init();
    OrderDetails.init();
    Block.init();
    Bookmarks.init();
    Notification.init();
    $("[data-hide]").on("click", function () {
        $(this).closest("." + $(this).attr("data-hide")).hide();
    });
});