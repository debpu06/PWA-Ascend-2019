﻿let GiftCards = {
    init: () => {
        $(document).ready(() => {
            GiftCards.loadData();
        });

        $(document)
            .on('click', '.jsShowCreateRow', GiftCards.showCreateRow)
            .on('click', '.jsShowUpdateRow', GiftCards.showUpdateRow)
            .on('click', '.jsCancelEditableRow', GiftCards.cancelEditableRow)
            .on('click', '.jsCreateGiftCard', GiftCards.createGiftCard)
            .on('click', '.jsUpdateGiftCard', GiftCards.updateGiftCard)
            .on('click', '.jsDeleteGiftCard', GiftCards.deleteGiftCard);
    },

    ContactList: [
    ],

    loadData: () => {
        $.ajax({
            url: "/GiftCardManager/GetAllGiftCards",
            type: "GET",
            success(result) {
                let html = "";
                let giftCard = {
                };
                $.each(result, (index, item) => {
                    giftCard = {
                        giftCardId: item.GiftCardId,
                        giftCardName: item.GiftCardName,
                        contactId: item.ContactId,
                        contactName: item.ContactName,
                        initialAmount: item.InitialAmount,
                        remainBalance: item.RemainBalance,
                        redemptionCode: item.RedemptionCode,
                        isActive: item.IsActive
                    };
                    html += GiftCards.loadDisplayRow(giftCard);
                });
                $('.gift-cards-table tbody').append(html);
            }
        });

        $.ajax({
            url: "/GiftCardManager/GetAllContacts",
            type: "POST",
            success(result) {
                $.each(result, (index, item) => {
                    GiftCards.ContactList.push({ ContactId: item.ContactId, ContactName: item.ContactName });
                });
            }
        });
    },

    loadDropdownList: () => {
        let html = "";
        $.each(GiftCards.ContactList, (index, item) => {
            html += '<option value="' + item.ContactId + '">' + item.ContactName + '</option>';
        });

        if ($('#GiftCard_EditableRow') !== null) {
            $('#GiftCard_EditableRow select').append(html);
        }
    },

    loadDisplayRow: (giftCard) => {
        let html = "";
        let isActive = false;

        if (giftCard.isActive) {
            isActive = "Yes";
        }
        else {
            isActive = "No";
        }

        html = `<tr id="${giftCard.giftCardId}">
                    <td>${giftCard.giftCardName}</td>
                    <td id="${giftCard.contactId}">${giftCard.contactName}</td>
                    <td>${giftCard.initialAmount}</td>
                    <td>${giftCard.remainBalance}</td>
                    <td>${giftCard.redemptionCode}</td>
                    <td>${isActive}</td>
                    <td class="text-right">
                        <button class="btn btn-primary btn-xs jsShowUpdateRow" title="Edit gift card">
                            <span class="glyphicon glyphicon-pencil" aria-hidden="true"></span>
                        </button>
                        <button class="btn btn-primary btn-xs jsDeleteGiftCard" title="Delete gift card">
                            <span class="glyphicon glyphicon-trash" aria-hidden="true"></span>
                        </button>
                    </td>
                 </tr>`;
        return html;
    },

    loadEditableRow: (giftCard) => {
        GiftCards.cancelEditableRow(giftCard);

        let editableRow = `<tr id="GiftCard_EditableRow" data-value="">
                               <td><input style="margin: 0; padding: 3px" id="GiftCard_GiftCardName" name="GiftCard.GiftCardName" type="text" value=""></td>
                               <td>
                                   <select style="width: 100%; height: 25px; padding: 1px; margin-bottom: 0px" data-val="true" id="GiftCard_ContactName" name="CreditCard.CreditCardType">
                                   </select>
                               </td>
                               <td><input style="margin: 0; padding: 3px" id="GiftCard_InitialAmount" name="GiftCard.InitialAmount" type="text" value=""></td>
                               <td><input style="margin: 0; padding: 3px" id="GiftCard_RemainBalance" name="GiftCard.RemainBalance" type="text" value=""></td>
                               <td><input style="margin: 0; padding: 3px" id="GiftCard_RedemptionCode" name="GiftCard.RedemptionCode" type="text" value=""></td>
                               <td><input style="margin-top: 0;" id="GiftCard_IsActive" name="GiftCard.IsActive" type="checkbox"><span> Is Active</span></td>
                           </tr>`;

        // Create a gift card
        if (giftCard === undefined) {
            $('.gift-cards-table tbody').append(editableRow);
            $('#GiftCard_EditableRow').append(`<td class="text-right">
                                                   <button class="btn btn-primary btn-xs jsCreateGiftCard" title="Add gift card">
                                                       <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                                                   </button>
                                                    <button class="btn btn-primary btn-xs jsCancelEditableRow" title="Cancel">
                                                       <span class="glyphicon glyphicon-remove" aria-hidden="true"></span>
                                                   </button>
                                               </td>`);

            GiftCards.loadDropdownList();
        }
        // Edit a gift card
        else {
            $(`#${giftCard.giftCardId}`).after(editableRow);
            $('#GiftCard_EditableRow').append(`<td class="text-right">
                                                   <button class="btn btn-primary btn-xs jsUpdateGiftCard" title="Save gift card">
                                                       <span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span>
                                                   </button>
                                                    <button class="btn btn-primary btn-xs jsCancelEditableRow" title="Cancel">
                                                       <span class="glyphicon glyphicon-remove" aria-hidden="true"></span>
                                                   </button>
                                               </td>`);

            GiftCards.loadDropdownList();
            $(`#${giftCard.giftCardId}`).remove();
            $('#GiftCard_EditableRow').data('value', giftCard.giftCardId);
            $('#GiftCard_GiftCardName').val(giftCard.giftCardName);
            $('#GiftCard_ContactName').val(giftCard.contactId);
            $('#GiftCard_InitialAmount').val(giftCard.initialAmount);
            $('#GiftCard_RemainBalance').val(giftCard.remainBalance);
            $('#GiftCard_RedemptionCode').val(giftCard.redemptionCode);
            if (giftCard.isActive === "Yes") {
                $('#GiftCard_IsActive').prop('checked', true);
            }
            else {
                $('#GiftCard_IsActive').prop('checked', false);
            }
        }

        $('#GiftCard_GiftCardName').focus();
    },

    enableAllButtons: () => {
        $(':button').prop('disabled', false);
    },

    disableAllButtons: () => {
        $(':button').prop('disabled', true);
    },

    showCreateRow: () => {
        GiftCards.loadEditableRow();
        GiftCards.disableAllButtons();
        $('#GiftCard_EditableRow button').prop('disabled', false);
    },

    showUpdateRow: (e) => {
        let giftCardId = $(e.currentTarget).closest('tr').attr('id');
        let giftCard = {
            giftCardId: giftCardId,
            giftCardName: $(`#${giftCardId} td:nth-child(1)`).text(),
            contactId: $(`#${giftCardId} td:nth-child(2)`).attr('id'),
            contactName: $(`#${giftCardId} td:nth-child(2)`).text(),
            initialAmount: $(`#${giftCardId} td:nth-child(3)`).text(),
            remainBalance: $(`#${giftCardId} td:nth-child(4)`).text(),
            redemptionCode: $(`#${giftCardId} td:nth-child(5)`).text(),
            isActive: $(`#${giftCardId} td:nth-child(6)`).text()
        };

        GiftCards.loadEditableRow(giftCard);
        GiftCards.disableAllButtons();
        $('#GiftCard_EditableRow button').prop('disabled', false);
    },

    cancelEditableRow: () => {
        let giftCardId = $('#GiftCard_EditableRow').data('value');

        if (giftCardId !== undefined && giftCardId !== "") {
            let giftCard = {
                giftCardId: giftCardId,
                giftCardName: $('#GiftCard_GiftCardName').val(),
                contactId: $('#GiftCard_ContactName option:selected').val(),
                contactName: $('#GiftCard_ContactName option:selected').text(),
                initialAmount: $('#GiftCard_InitialAmount').val(),
                remainBalance: $('#GiftCard_RemainBalance').val(),
                redemptionCode: $('#GiftCard_RedemptionCode').val(),
                isActive: $('#GiftCard_IsActive').is(':checked')
            };

            let html = "";
            html += GiftCards.loadDisplayRow(giftCard);

            $('#GiftCard_EditableRow').before(html);
        }
        if ($('#GiftCard_EditableRow') !== null) {
            $('#GiftCard_EditableRow').remove();
        }
        GiftCards.enableAllButtons();
    },

    createGiftCard: () => {
        let giftCard = {
            giftCardId: "",
            giftCardName: $('#GiftCard_GiftCardName').val(),
            contactId: $('#GiftCard_ContactName option:selected').val(),
            contactName: $('#GiftCard_ContactName option:selected').text(),
            initialAmount: $('#GiftCard_InitialAmount').val(),
            remainBalance: $('#GiftCard_RemainBalance').val(),
            redemptionCode: $('#GiftCard_RedemptionCode').val(),
            isActive: $('#GiftCard_IsActive').is(':checked')
        };

        $.ajax({
            url: "/GiftCardManager/AddGiftCard",
            type: "POST",
            data: giftCard,
            success: function (result) {
                giftCard.GiftCardId = result;

                let html = "";
                html += GiftCards.loadDisplayRow(giftCard);

                $('.gift-cards-table tbody').append(html);
                $('#GiftCard_EditableRow').remove();
                GiftCards.enableAllButtons();
            }
        });
    },

    updateGiftCard: () => {
        let giftCardId = $('#GiftCard_EditableRow').data('value');
        let giftCard = {
            giftCardId: giftCardId,
            giftCardName: $('#GiftCard_GiftCardName').val(),
            contactId: $('#GiftCard_ContactName option:selected').val(),
            contactName: $('#GiftCard_ContactName option:selected').text(),
            initialAmount: $('#GiftCard_InitialAmount').val(),
            remainBalance: $('#GiftCard_RemainBalance').val(),
            redemptionCode: $('#GiftCard_RedemptionCode').val(),
            isActive: $('#GiftCard_IsActive').is(':checked')
        };

        $.ajax({
            url: "/GiftCardManager/UpdateGiftCard",
            type: "POST",
            data: giftCard,
            success: () => {
                $(`#${giftCard.giftCardId}`).remove();

                let html = GiftCards.loadDisplayRow(giftCard);

                $('#GiftCard_EditableRow').before(html);
                $('#GiftCard_EditableRow').remove();
                GiftCards.enableAllButtons();
            }
        });
    },

    deleteGiftCard: (e) => {
        let giftCardId = $(e.currentTarget).closest('tr').attr('id');

        if (confirm("Do you really want to delete this gift card?")) {
            $.ajax({
                url: "/GiftCardManager/DeleteGiftCard",
                type: "POST",
                data: {
                    giftCardId: giftCardId
                },
                success: () => {
                    $(`#${giftCardId}`).remove();
                }
            });
        }
    }
};