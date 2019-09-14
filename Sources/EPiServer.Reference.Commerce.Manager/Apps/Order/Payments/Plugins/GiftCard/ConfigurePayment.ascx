<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigurePayment.ascx.cs"
    Inherits="EPiServer.Reference.Commerce.GiftCardPayment.ConfigurePayment" %>
<div id="DataForm">
    <table cellpadding="0" cellspacing="2">
        <tr>
            <td class="FormLabelCell" colspan="2"><b>Configure Gift Card</b></td>
        </tr>
    </table>
    <br />
    <table class="DataForm">
        <tr>
            <td class="FormLabelCell">
                <asp:Literal ID="Literal4" runat="server" Text="MetaClass name" />:</td>
            <td class="FormFieldCell">
                <asp:TextBox runat="server" ID="MetaClassName"
                    Width="230"></asp:TextBox><br>
                <asp:RequiredFieldValidator ControlToValidate="MetaClassName" Display="dynamic"
                    Font-Name="verdana" Font-Size="9pt" ErrorMessage="'MetaClass name' must not be left blank."
                    runat="server" ID="RequiredFieldValidator2"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</div>