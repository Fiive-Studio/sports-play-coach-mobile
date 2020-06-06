using FutbolPlayAdmin.Views;
using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlayAdmin.Services.TranslateExtension;
using FutbolPlayAdmin.Services;

namespace FutbolPlayAdmin.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        #region Vars

        CustomerModel _customer;

        #endregion

        #region Properties

        // Model
        public CustomerModel Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Mail");
                OnPropertyChanged("ChangePasswordVisible");
                OnPropertyChanged("UpdateButtonStatus");
            }
        }

        // Entries
        public string Name
        {
            get
            {
                if (Customer == null) { return string.Empty; }
                return _customer.Name;
            }
            set
            {
                _customer.Name = value;
                OnPropertyChanged("UpdateButtonStatus");
            }
        }
        public string Mail
        {
            get
            {
                if (Customer == null) { return string.Empty; }
                return _customer.Mail;
            }
            set
            {
                _customer.Mail = value;
                OnPropertyChanged("UpdateButtonStatus");
            }
        }

        // Status
        public bool UpdateButtonStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name) ||
                    string.IsNullOrWhiteSpace(Mail)) { return false; }
                return !IsBusy;
            }
        }
        public bool ChangePasswordVisible
        {
            get
            {
                if (IsBusy) { return false; }
                if (Customer == null) { return false; }
                return true;
            }
        }

        // Actions
        public ICommand UpdateCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        #endregion

        #region Methods

        public ProfileViewModel(Page page) : base(page)
        {
            Customer = new CustomerModel();
            UpdateCommand = new Command(async () =>
            {
                bool result = await RegisterValidations();
                if (!result) { return; }

                IsBusy = true;

                try
                {
                    bool response = await RestService.Instance.UpdateProfileCustomerAsync(Customer);
                    if (response){ await DisplayAlertAsync(MessagesTexts.update_profile_message); }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(RestService.Instance.ErrorCode))
                        {
                            await DisplayAlertAsync(GetMessageCode(RestService.Instance.ErrorCode));
                        }
                        else { await DisplayAlertAsync(MessagesTexts.error_message); }
                    }
                }
                catch { await DisplayAlertAsync(MessagesTexts.error_message); }

                IsBusy = false;
            });
            ChangePasswordCommand = new Command(async () => { await OpenPageAsync(new ChangePasswordView()); });

            LoadUserData();
        }

        async void LoadUserData()
        {
            IsBusy = true;

            try
            {
                Customer = await RestService.Instance.GetProfileCustomerAsync();
                if (Customer == null)
                {
                    await DisplayAlertAsync(MessagesTexts.error_message);
                    Customer = new CustomerModel();
                }
            }
            catch { await DisplayAlertAsync(MessagesTexts.error_message); }

            IsBusy = false;
        }

        async Task<bool> RegisterValidations()
        {
            if (!FunctionsService.ValidateEmail(Mail))
            {
                await _page.DisplayAlert(T.GetValue("app_name"), T.GetValue("mail_error"), T.GetValue("ok"));
                return false;
            }

            return true;
        }

        protected override void UpdateAdditionalProperties()
        {
            OnPropertyChanged("UpdateButtonStatus");
            OnPropertyChanged("ChangePasswordVisible");
        }

        #endregion
    }
}
