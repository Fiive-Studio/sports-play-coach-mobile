using FutbolPlay.WebApi.Model;
using FutbolPlayAdmin.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using T = FutbolPlayAdmin.Services.TranslateExtension;

namespace FutbolPlayAdmin.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Vars

        protected bool _isBusy;
        protected INavigation _navigation;
        protected Page _page;

        #endregion

        #region Properties

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
                UpdateAdditionalProperties();
            }
        }

        #endregion

        #region Methods

        public BaseViewModel(Page page)
        {
            _page = page;
            _navigation = page.Navigation;
        }

        public async Task OpenPageAsync(Page page)
        {
            if (IsBusy) { return; }
            IsBusy = true;
            await _navigation.PushAsync(page);
            IsBusy = false;
        }

        public async Task DisplayAlertAsync(string message)
        {
            await _page.DisplayAlert(T.GetValue("app_name"), message, T.GetValue("ok"));
        }

        public async Task DisplayAlertAsync(MessagesTexts messageKey)
        {
            await DisplayAlertAsync(T.GetValue(messageKey.ToString()));
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void UpdateAdditionalProperties() { }

        protected MessagesTexts GetMessageCode(string code)
        {
            return (MessagesTexts)Enum.Parse(typeof(MessagesTexts), string.Concat("error_code_", code));
        }

        public string GetReservationStatus(ReservationStatus status)
        {
            switch (status)
            {
                case ReservationStatus.Ok:
                    return T.GetValue("reservation_ok_short");
                case ReservationStatus.Pending:
                    return T.GetValue("reservation_pending_short");
                case ReservationStatus.CancelPlace:
                    return T.GetValue("reservation_cancel_place_short");
                case ReservationStatus.CancelUser:
                    return T.GetValue("reservation_cancel_user_short");
                case ReservationStatus.Running:
                    return T.GetValue("reservation_running_short");
                case ReservationStatus.Close:
                    return T.GetValue("reservation_close_short");
            }

            return T.GetValue("cancel");
        }

        public bool ValidateOptionalField(string value)
        {
            if (value != null && value.Length > 0 && string.IsNullOrWhiteSpace(value)) { return false; }
            return true;
        }

        #endregion
    }
}
