using FutbolPlayAdmin.Views;
using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlayAdmin.Services.TranslateExtension;
using FutbolPlayAdmin.Services;

namespace FutbolPlayAdmin.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Vars

        string _mail, _password;

        #endregion

        #region Properties

        // Entries
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged("LoginButtonStatus");
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("LoginButtonStatus");
            }
        }

        // Status
        public bool LoginButtonStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Mail) || string.IsNullOrEmpty(Password)) { return false; }
                return !IsBusy;
            }
        }

        // Actions
        public ICommand DoLoginCommand { get; set; }
        public ICommand ForgotPasswordCommand { get; set; }

        #endregion

        #region Methods

        public LoginViewModel(Page page) : base(page)
        {
            DoLoginCommand = new Command(async () => {
                IsBusy = true;
                var user = new CustomerModel { Mail = Mail, Password = Password };

                try
                {
                    bool response = true;
                    response = await RestService.Instance.DoLoginCustomerAsync(user);

                    if (response)
                    {
                        App.IsLogin = true;
                        App.NavigateToHome();
                    }
                    else
                    {
                        if (RestService.Instance.ErrorCode == "003")
                        { await DisplayAlertAsync(MessagesTexts.error_code_003); }
                        else { await DisplayAlertAsync(MessagesTexts.error_message); }
                    }
                }
                catch { await DisplayAlertAsync(Services.MessagesTexts.error_message); }

                IsBusy = false;
            });
            ForgotPasswordCommand = new Command(async() => { await OpenPageAsync(new RememberPasswordView()); });
        }

        protected override void UpdateAdditionalProperties()
        {
            OnPropertyChanged("LoginButtonStatus");
        }

        #endregion
    }
}
