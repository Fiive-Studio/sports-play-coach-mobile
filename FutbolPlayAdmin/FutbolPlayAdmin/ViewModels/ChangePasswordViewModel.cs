using FutbolPlayAdmin.Services;
using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlayAdmin.Services.TranslateExtension;

namespace FutbolPlayAdmin.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        #region Vars

        string _oldPassword, _newPassword, _reNewPassword;

        #endregion

        #region Properties

        // Entries
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                _oldPassword = value;
                OnPropertyChanged("ChangePasswordButtonStatus");
            }
        }
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged("ChangePasswordButtonStatus");
            }
        }
        public string ReNewPassword
        {
            get { return _reNewPassword; }
            set
            {
                _reNewPassword = value;
                OnPropertyChanged("ChangePasswordButtonStatus");
            }
        }

        // Status
        public bool ChangePasswordButtonStatus
        {
            get
            {
                if (string.IsNullOrEmpty(OldPassword) ||
                    string.IsNullOrEmpty(NewPassword) ||
                    string.IsNullOrEmpty(ReNewPassword)) { return false; }

                if (NewPassword.Length < 6) { return false; }

                return !IsBusy;
            }
        }

        // Actions
        public ICommand ChangePasswordCommand { get; set; }

        #endregion

        #region Methods

        public ChangePasswordViewModel(Page page) : base(page)
        {
            ChangePasswordCommand = new Command(async () =>
            {
                if (NewPassword != ReNewPassword)
                {
                    await DisplayAlertAsync(MessagesTexts.password_differents_message);
                    return;
                }

                IsBusy = true;

                try
                {
                    bool response = await RestService.Instance.ChangePasswordCustomerAsync(OldPassword, NewPassword);
                    if (response) { await DisplayAlertAsync(MessagesTexts.password_changed); }
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
            OnPropertyChanged("ChangePasswordButtonStatus");
        }

        #endregion
    }
}
