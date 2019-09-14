using Mediachase.Commerce.Orders.Dto;
using Mediachase.Web.Console.Interfaces;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EPiServer.Reference.Commerce.GiftCardPayment
{
    public partial class ConfigurePayment : System.Web.UI.UserControl, IGatewayControl
    {
        string _validationGroup = string.Empty;
        private PaymentMethodDto _paymentMethodDto = null;
        private const string _metaClassParameterName = "GiftCardMetaClass";

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            BindData();
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            BindData();
            base.DataBind();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        public void BindData()
        {
            // fill in the form fields
            if (_paymentMethodDto != null && _paymentMethodDto.PaymentMethodParameter != null)
            {
                var param = GetParameterByName(_metaClassParameterName);
                if (param != null)
                {
                    MetaClassName.Text = param.Value;
                }
            }
            else
            {
                this.Visible = false;
            }
        }

        /// <summary>
        /// Saves the object changes.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveChanges(object dto)
        {
            if (this.Visible)
            {
                _paymentMethodDto = dto as PaymentMethodDto;
                if (_paymentMethodDto != null && _paymentMethodDto.PaymentMethodParameter != null)
                {
                    var paymentMethodId = Guid.Empty;
                    if (_paymentMethodDto.PaymentMethod.Count > 0)
                    {
                        paymentMethodId = _paymentMethodDto.PaymentMethod[0].PaymentMethodId;
                    }

                    var param = GetParameterByName(_metaClassParameterName);
                    if (param != null)
                    {
                        param.Value = MetaClassName.Text;
                    }
                    else
                    {
                        CreateParameter(_paymentMethodDto, _metaClassParameterName, MetaClassName.Text, paymentMethodId);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of the parameter by.
        /// </summary>
        /// <param name="name">The name.</param>
        private PaymentMethodDto.PaymentMethodParameterRow GetParameterByName(string name)
        {
            var rows = (PaymentMethodDto.PaymentMethodParameterRow[])_paymentMethodDto.PaymentMethodParameter.Select(String.Format("Parameter = '{0}'", name));
            if (rows != null && rows.Length > 0)
            {
                return rows[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="paymentMethodId">The payment method id.</param>
        private void CreateParameter(PaymentMethodDto dto,
            string name, string value, Guid paymentMethodId)
        {
            var newRow = dto.PaymentMethodParameter.NewPaymentMethodParameterRow();
            newRow.PaymentMethodId = paymentMethodId;
            newRow.Parameter = name;
            newRow.Value = value;
            // add the row to the dtoif (newRow.RowState == DataRowState.Detached)
            dto.PaymentMethodParameter.Rows.Add(newRow);
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void LoadObject(object dto)
        {
            _paymentMethodDto = dto as PaymentMethodDto;
        }

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        public string ValidationGroup
        {
            get
            {
                return _validationGroup;
            }
            set
            {
                _validationGroup = value;
            }
        }
    }
}