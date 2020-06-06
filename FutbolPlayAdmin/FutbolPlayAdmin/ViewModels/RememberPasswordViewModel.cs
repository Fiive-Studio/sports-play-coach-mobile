using FutbolPlayAdmin.Services;
using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlayAdmin.Services.TranslateExtension;

namespace FutbolPlayAdmin.ViewModels
{
    public class RememberPasswordViewModel : BaseViewModel
    {
        #region Vars

        string _mail;

        #endregion

        #region Properties

        // Entries
        public string Mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged("RememberPasswordButtonStatus");
            }
        }

        // Status
        public bool RememberPasswordButtonStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Mail)) { return false; }
                return !IsBusy;
            }
        }

        // Actions
        public ICommand RememberPasswordCommand { get; set; }

        #endregion

        #region Methods

        public RememberPasswordViewModel(Page page) : base(page)
        {
            RememberPasswordCommand = new Command(async () => {
                IsBusy = true;

                try
                {
                    bool response = await RestService.Instance.RecoverPasswordCustomerAsync(Mail);
                    if (response) { await DisplayAlertAsync(MessagesTexts.recover_password_message); }
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
        }

        protected override void UpdateAdditionalProperties()
        {
            OnPropertyChanged("RememberPasswordButtonStatus");
        }

        #endregion
    }
}
