using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
using FutbolPlayAdmin.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using T = FutbolPlayAdmin.Services.TranslateExtension;

namespace FutbolPlayAdmin.ViewModels
{
    public class ReservationCreateViewModel : BaseViewModel
    {
        #region Vars

        UserModel _user;
        ReservationModel _reservation;
        string _gettingValue;
        decimal _value;
        bool _reservationButtonStatus, _gettingValueVisible;
        int _idUser;

        #endregion

        #region Properties

        // Data
        public UserModel User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Mail");
                OnPropertyChanged("Phone");
                OnPropertyChanged("ReservationButtonStatus");
            }
        }
        public ReservationModel Reservation
        {
            get { return _reservation; }
            set
            {
                _reservation = value;
                OnPropertyChanged("Reservation");
            }
        }
        public decimal Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        public string GettingValue
        {
            get { return _gettingValue; }
            set
            {
                _gettingValue = value;
                OnPropertyChanged("GettingValue");
            }
        }
        public int IdUser
        {
            get { return _idUser; }
            set
            {
                _idUser = value; OnPropertyChanged("IdUser");
            }
        }
        public int IdUserType { get; set; }

        // Visible
        public bool GettingValueVisible
        {
            get { return _gettingValueVisible; }
            set
            {
                _gettingValueVisible = value;
                OnPropertyChanged("GettingValueVisible");
            }
        }

        // Entries
        public string Name
        {
            get
            {
                if (User == null) { return string.Empty; }
                return _user.Name;
            }
            set
            {
                _user.Name = value;
                OnPropertyChanged("ReservationButtonStatus");
                OnPropertyChanged("Name");
            }
        }
        public string Mail
        {
            get
            {
                if (User == null) { return string.Empty; }
                return _user.Mail;
            }
            set
            {
                _user.Mail = value;
                OnPropertyChanged("ReservationButtonStatus");
                OnPropertyChanged("Mail");
            }
        }
        public string Phone
        {
            get
            {
                if (User == null) { return string.Empty; }
                return _user.Phone;
            }
            set
            {
                _user.Phone = value;
                OnPropertyChanged("ReservationButtonStatus");
                OnPropertyChanged("Phone");
            }
        }
        public string Description
        {
            get
            {
                if (Reservation == null) { return string.Empty; }
                return Reservation.Description;
            }
            set
            {
                Reservation.Description = value;
                OnPropertyChanged("ReservationButtonStatus");
                OnPropertyChanged("Description");
            }
        }

        // Status
        public bool ReservationButtonStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(User.Name) ||
                    string.IsNullOrWhiteSpace(User.Phone) ||
                    !ValidateOptionalField(User.Mail) ||
                    !ValidateOptionalField(Reservation.Description))
                { return false; }

                if (IsBusy) { return false; }

                return _reservationButtonStatus;
            }
            set
            {
                _reservationButtonStatus = value;
                OnPropertyChanged("ReservationButtonStatus");
            }
        }

        // Actions
        public ICommand DoReservationCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        #endregion

        #region Methods

        public ReservationCreateViewModel(Page page, ReservationModel reservation) : base(page)
        {
            User = new UserModel();
            IdUser = -1;
            _reservation = reservation;
            ReservationButtonStatus = true;

            DoReservationCommand = new Command(async () =>
            {
                bool result = await UserValidations();
                if (!result) { return; }

                ReservationButtonStatus = false;
                IsBusy = true;

                try
                {
                    if (IdUser == -1) { IdUser = await RestService.Instance.CreateUserOfflineAsync(User); }
                    if (IdUser != -1)
                    {
                        if (Value < 1)
                        {
                            if (!await page.DisplayAlert(T.GetValue("app_name"), string.Format(T.GetValue("reservation_with_price_zero"), Value), T.GetValue("ok"), T.GetValue("cancel")))
                            {
                                ReservationButtonStatus = true;
                                IsBusy = false;
                                return;
                            }
                        }


                        int response;
                        _reservation.User = new UserModel { IdUser = IdUser };
                        _reservation.Price = Value;
                        _reservation.Description = Description;

                        if (_reservation.Pitch.PitchType == PitchType.Multiple) { response = await RestService.Instance.DoReservationMultipleCustomerAsync(_reservation, IdUserType); }
                        else { response = await RestService.Instance.DoReservationSingleCustomerAsync(_reservation, IdUserType); }

                        if (response == -1) { await DisplayAlertAsync(MessagesTexts.reservation_conflict); }
                        else
                        {
                            ReservationService.HasChanges = true;
                            ReservationService.Reservation.Status = ReservationStatus.Pending;
                            ReservationService.Reservation.IdReservation = response;

                            await DisplayAlertAsync(MessagesTexts.reservation_ok);
                            await _navigation.PopAsync();
                        }
                    }
                    else
                    {
                        await DisplayAlertAsync(MessagesTexts.error_message);
                        ReservationButtonStatus = true;
                    }
                }
                catch
                {
                    await DisplayAlertAsync(MessagesTexts.error_message);
                    ReservationButtonStatus = true;
                }

                IsBusy = false;
            });
            SearchCommand = new Command(async () =>
            {
                IsBusy = true;

                try
                {
                    List<UserModel> users = await RestService.Instance.SearchUsersAsync(Name);
                    if (users != null && users.Count != 0)
                    {
                        int consecutive = 0;

                        // Se agrega consecutivo ya que opcion que se usa para mostrar resultado retorna texto siempre
                        var data = (from u in users select string.Concat((consecutive = consecutive + 1), ". ", u.Name)).ToArray();
                        string result = await _page.DisplayActionSheet(T.GetValue("app_name"), T.GetValue("ok"), null, data);

                        if (result != null && result != T.GetValue("ok"))
                        {
                            int pos = Convert.ToInt32(result.Split(new char[] { '.' }, 2)[0]);
                            UserModel user = users[pos - 1];
                            Name = user.Name;
                            Phone = user.Phone;
                            Mail = user.Mail;
                            IdUser = user.IdUser;
                            IdUserType = user.IdUserType;
                        }
                    }
                    else { await DisplayAlertAsync(MessagesTexts.search_without_data); }
                }
                catch
                {
                    await DisplayAlertAsync(MessagesTexts.error_message);
                }

                IsBusy = false;
            });

            GetPitchPrice();
        }

        public async void GetPitchPrice()
        {
            IsBusy = true;
            GettingValue = T.GetValue("getting_price");
            GettingValueVisible = true;

            try
            {
                Value = await RestService.Instance.GetPitchPriceAsync(_reservation.Pitch.Id, _reservation.Date);
                GettingValueVisible = false;
            }
            catch
            {
                GettingValue = T.GetValue("getting_price_error");
            }

            IsBusy = false;
        }

        async Task<bool> UserValidations()
        {
            if (!string.IsNullOrEmpty(User.Mail) && !FunctionsService.ValidateEmail(User.Mail))
            {
                await _page.DisplayAlert(T.GetValue("app_name"), T.GetValue("mail_error"), T.GetValue("ok"));
                return false;
            }

            if (!FunctionsService.ValidatePhone(User.Phone))
            {
                await _page.DisplayAlert(T.GetValue("app_name"), T.GetValue("phone_error"), T.GetValue("ok"));
                return false;
            }

            return true;
        }

        protected override void UpdateAdditionalProperties() { OnPropertyChanged("ReservationButtonStatus"); }

        #endregion
    }
}
