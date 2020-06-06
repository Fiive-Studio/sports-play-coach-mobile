using FutbolPlayAdmin.Services;
using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
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
    public class ReservationSummaryViewModel : BaseViewModel
    {
        #region Vars

        ReservationModel _reservation;
        bool _reservationButtonStatus, _retryButtonVisible;
        int _reservationStatusId;

        #endregion

        #region Properties

        // Data
        public ReservationModel Reservation
        {
            get { return _reservation; }
            set
            {
                _reservation = value;
                OnPropertyChanged("Reservation");
                OnPropertyChanged("ReservationStatusId");
                OnPropertyChanged("Price");
                OnPropertyChanged("Description");
            }
        }
        public int ReservationStatusId
        {
            get { return _reservationStatusId; }
            set
            {
                _reservationStatusId = value;
                OnPropertyChanged("ReservationStatusId");
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
                OnPropertyChanged("ChangeStatusButtonStatus");
                OnPropertyChanged("Description");
            }
        }

        // Status
        public bool ChangeStatusButtonStatus
        {
            get
            {
                if (!ValidateOptionalField(Reservation.Description))
                { return false; }

                if (IsBusy) { return false; }

                return _reservationButtonStatus;
            }
            set
            {
                _reservationButtonStatus = value;
                OnPropertyChanged("ChangeStatusButtonStatus");
            }
        }
        public bool RetryButtonVisible
        {
            get { return _retryButtonVisible; }
            set
            {
                _retryButtonVisible = value;
                OnPropertyChanged("RetryButtonVisible");
            }
        }

        // Actions
        public ICommand ChangeStatusCommand { get; set; }
        public ICommand RetryCommand { get; set; }

        #endregion

        #region Methods

        public ReservationSummaryViewModel(Page page, ReservationModel reservation, bool loadReservationData) : base(page)
        {
            ReservationStatusId = -1;
            ChangeStatusButtonStatus = true;
            _reservation = reservation;

            ChangeStatusCommand = new Command(async () =>
            {
                ChangeStatusButtonStatus = false;
                IsBusy = true;

                try
                {
                    if (Reservation.Price < 1)
                    {
                        if (!await page.DisplayAlert(T.GetValue("app_name"), string.Format(T.GetValue("reservation_with_price_zero"), Reservation.Price), T.GetValue("ok"), T.GetValue("cancel")))
                        {
                            ChangeStatusButtonStatus = true;
                            IsBusy = false;
                            return;
                        }
                    }

                    _reservation.Status = GetStatusCode();
                    var response = await RestService.Instance.ReservationUpdateAsync(_reservation);

                    if (response)
                    {
                        if (ReservationService.Reservation != null)
                        {
                            // Proceso para responder al Scheduler
                            ReservationService.HasChanges = true;
                            ReservationService.Reservation.Status = GetReservationStatus();
                        }
                        else
                        {
                            // Proceso para responder a la lista de pendientes
                            if (ReservationStatusId != 0) { ReservationService.HasChanges = true; }
                        }

                        await DisplayAlertAsync(MessagesTexts.change_status_ok);
                        await _navigation.PopAsync();
                    }
                    else { await DisplayAlertAsync(MessagesTexts.error_message); }
                }
                catch { await DisplayAlertAsync(MessagesTexts.error_message); }

                IsBusy = false;
            });
            RetryCommand = new Command(() => { LoadReservationData(); });

            if (loadReservationData) { LoadReservationData(); }
            else
            {
                Reservation = reservation;
                SetStatusCode();
            }
        }

        async void LoadReservationData()
        {
            ChangeStatusButtonStatus = RetryButtonVisible = false;
            IsBusy = true;

            try
            {
                Reservation = await RestService.Instance.GetReservationDetailAsync(_reservation.IdReservation);
                if (Reservation == null) { await DisplayAlertAsync(MessagesTexts.error_message); }
                else
                {
                    SetStatusCode();
                    ChangeStatusButtonStatus = true;
                }
            }
            catch
            {
                await DisplayAlertAsync(MessagesTexts.error_message);
                RetryButtonVisible = true;
            }

            IsBusy = false;
        }

        ReservationStatus GetStatusCode()
        {
            switch (ReservationStatusId)
            {
                case 0: return ReservationStatus.Pending;
                case 1: return ReservationStatus.Ok;
                case 2: return ReservationStatus.CancelPlace;
                case 3: return ReservationStatus.Running;
                case 4: return ReservationStatus.Close;
            }

            return ReservationStatus.Ok;
        }

        void SetStatusCode()
        {
            if (Reservation == null) { ReservationStatusId = -1; }

            switch (Reservation.Status)
            {
                case ReservationStatus.Pending: ReservationStatusId = 0; break;
                case ReservationStatus.Ok: ReservationStatusId = 1; break;
                case ReservationStatus.CancelPlace: ReservationStatusId = 2; break;
                case ReservationStatus.Running: ReservationStatusId = 3; break;
                case ReservationStatus.Close: ReservationStatusId = 4; break;
            }

            if (ReservationStatusId == -1) { ReservationStatusId = 2; }
        }

        ReservationStatus GetReservationStatus()
        {
            switch (ReservationStatusId)
            {
                case 0: return ReservationStatus.Pending;
                case 1: return ReservationStatus.Ok;
                case 2: return ReservationStatus.CancelPlace;
                case 3: return ReservationStatus.Running;
                case 4: return ReservationStatus.Close;
            }

            return ReservationStatus.Pending;
        }

        protected override void UpdateAdditionalProperties() { OnPropertyChanged("ChangeStatusButtonStatus"); }

        #endregion
    }
}
