using FutbolPlayAdmin.Views;
using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
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
    public class ScheduleViewModel : BaseViewModel
    {
        #region Vars

        List<PitchModel> _pitches;
        List<ReservationModel> _pitchesBusy;
        PlaceModel _place;
        PitchType _pitchType;
        DateTime _date = DateTime.Now;
        bool _dateVisible, _gridVisible, _retryButtonStatus;

        #endregion

        #region Properties

        // Data
        public List<PitchModel> Pitches
        {
            get { return _pitches; }
            private set
            {
                _pitches = value;
                OnPropertyChanged("Pitches");
            }
        }

        public List<ReservationModel> PitchesBusy
        {
            get { return _pitchesBusy; }
            private set
            {
                _pitchesBusy = value;
                OnPropertyChanged("PitchesBusy");
            }
        }

        // Entries
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        // Status
        public bool DateVisible
        {
            get { return _dateVisible; }
            set
            {
                _dateVisible = value;
                OnPropertyChanged("DateVisible");
            }
        }
        public bool GridVisible
        {
            get { return _gridVisible; }
            set
            {
                _gridVisible = value;
                OnPropertyChanged("GridVisible");
            }
        }

        public bool RetryButtonVisible
        {
            get { return _retryButtonStatus; }
            private set
            {
                _retryButtonStatus = value;
                OnPropertyChanged("RetryButtonVisible");
            }
        }

        // Actions
        public ICommand ReservationCommand { get; set; }
        public ICommand ReservationSummaryCommand { get; set; }
        public ICommand RetryCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ShowPitchDescriptionCommand { get; set; }

        #endregion

        #region Methods

        public ScheduleViewModel(Page page, PlaceModel place, PitchType pitchType) : base(page)
        {
            _place = place;
            _pitchType = pitchType;
            ReservationCommand = new Command<CommandWrapper>(async (wrapper) => 
            {
                ReservationService.Button = wrapper.Button;
                ReservationService.Reservation = wrapper.Reservation;
                ReservationService.HasChanges = false;
                ReservationService.IsNew = true;
                await OpenPageAsync(new ReservationCreateView(wrapper.Reservation));
            });
            ReservationSummaryCommand = new Command<CommandWrapper>(async (wrapper) => 
            {
                ReservationService.Button = wrapper.Button;
                ReservationService.Reservation = wrapper.Reservation;
                ReservationService.HasChanges = false;
                ReservationService.IsNew = false;
                await OpenPageAsync(new ReservationSummaryView(wrapper.Reservation, true));
            });
            RetryCommand = new Command(async () => { await GetSchedule(); });
            RefreshCommand = new Command(async () => { await GetSchedule(); });
            ShowPitchDescriptionCommand = new Command<string>(async (message) => { await DisplayAlertAsync(message); });
        }

        public async Task GetSchedule()
        {            
            DateVisible = GridVisible = RetryButtonVisible = false;
            IsBusy = true;
            await Task.Delay(1000);

            try
            {
                if (Pitches == null)
                {
                    if (_pitchType == PitchType.Multiple) { Pitches = await RestService.Instance.GetPitchesMultiplesAsync(_place.Id); }
                    else { Pitches = await RestService.Instance.GetPitchesSingleAsync(_place.Id); }
                }

                if (Pitches == null)
                {
                    await DisplayAlertAsync(Services.MessagesTexts.error_message);
                    RetryButtonVisible = true;
                    IsBusy = false;
                }
                else { await GetBusyTime(); }
            }
            catch
            {
                await DisplayAlertAsync(Services.MessagesTexts.error_message);
                RetryButtonVisible = true;
                IsBusy = false;
            }
        }

        public async Task GetBusyTime()
        {
            IsBusy = true;
            try
            {
                if (_pitchType == PitchType.Multiple) { PitchesBusy = await RestService.Instance.GetPitchesMultiplesBusyCustomerAsync(_place.Id, Date); }
                else { PitchesBusy = await RestService.Instance.GetPitchesSingleBusyCustomerAsync(_place.Id, Date); }

                if (PitchesBusy == null)
                {
                    await DisplayAlertAsync(Services.MessagesTexts.error_message);
                    RetryButtonVisible = true;
                }
                else { DateVisible = GridVisible = true; }
            }
            catch
            {
                await DisplayAlertAsync(Services.MessagesTexts.error_message);
                RetryButtonVisible = true;
            }

            IsBusy = false;
        }

        public ReservationModel ValidateBusyTime(int id, int hour)
        {
            var result = (from source in PitchesBusy
                         where source.Date.Hour == hour
                                && source.Pitch.Id == id
                         select source).FirstOrDefault();

            return result;
        }

        protected override void UpdateAdditionalProperties() { }

        #endregion
    }

    public class CommandWrapper
    {
        public Button Button { get; set; }
        public ReservationModel Reservation { get; set; }
    }
}
