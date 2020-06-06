using FutbolPlayAdmin.ViewModels;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using T = FutbolPlayAdmin.Services.TranslateExtension;
using FutbolPlay.WebApi.Services;
using FutbolPlayAdmin.Services;

namespace FutbolPlayAdmin.Views
{
    public partial class ScheduleView : ContentPage
    {
        #region Vars

        ScheduleViewModel _binding;
        PlaceModel _place;
        PitchType _pitchType;
        bool _headerExist;
        StringBuilder sbTimeStart = new StringBuilder();
        int _startHour;
        int _endHour;
        int _hourToShow;
        int _fontSize = 10;
        int _rowsToShow = 5;

        #endregion

        public ScheduleView(PitchType pitchType)
        {
            InitializeComponent();
            _place = SessionCustomerService.Place;
            _pitchType = pitchType;
            dpDate.DateSelected += (sender, e) => { GetSchedule(); };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                _binding.IsBusy = true; await Task.Delay(1000);
                _hourToShow = _hourToShow - (_rowsToShow * 2);
                AddDetail(false);
                _binding.IsBusy = false;
            };
            imgUp.GestureRecognizers.Add(tapGestureRecognizer);

            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => { _binding.IsBusy = true; await Task.Delay(1000); AddDetail(false); _binding.IsBusy = false; };
            imgDown.GestureRecognizers.Add(tapGestureRecognizer);

            _binding = new ScheduleViewModel(this, _place, pitchType);
            _binding.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "PitchesBusy")
                {
                    if (_binding.Pitches == null) { return; }

                    CreateHeadersAsync();
                }
            };
            BindingContext = _binding;
            GetSchedule();
        }

        void CreateHeadersAsync()
        {
            if (!_headerExist)
            {
                int pitchCount = 1;
                string pitchPrefix = string.Concat(T.GetValue("pitch_prefix_length"), " ");
                if (_binding.Pitches.Count > 3 && _binding.Pitches.Count <= 6) { pitchPrefix = T.GetValue("pitch_prefix_short"); }
                else if (_binding.Pitches.Count > 6) { pitchPrefix = string.Empty; }

                foreach (PitchModel pitch in _binding.Pitches)
                {
                    #region Header

                    gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    gridHeader.Children.Add(new Button { Text = string.Concat(pitchPrefix, pitchCount), Style = (Style)Resources["titleButton"], Command =_binding.ShowPitchDescriptionCommand, CommandParameter = pitch.Description }
                                                , pitchCount, 0);

                    #endregion

                    pitchCount++;
                }

                if (_place.FormatHour > 0) { _fontSize = 8; }
                _headerExist = true; // Var for only create the header the first time 
            }

            CreateDetailsAsync();
        }

        void CreateDetailsAsync()
        {
            HourModel hour = _place.GetHourOfDay(dpDate.Date.DayOfWeek);
            _startHour = hour.HourStart.Hour;
            _endHour = hour.HourEnd.Hour;

            gridDetail.RowDefinitions.Clear();
            gridDetail.ColumnDefinitions.Clear();
            _hourToShow = DateTime.Now.Hour;

            if (_binding.Pitches.Count <= 3) { _rowsToShow = 7; }

            AddDetail(true);
        }

        void AddDetail(bool isNewDay)
        {
            #region Time Validation

            imgUp.IsVisible = imgDown.IsVisible = true;

            if (_hourToShow > _startHour)
            {
                if ((_hourToShow >= _endHour) || (_hourToShow >= _endHour - (_rowsToShow - 1)))
                {
                    _hourToShow = _endHour - (_rowsToShow - 1);
                    imgDown.IsVisible = false;
                }
            }
            else
            {
                _hourToShow = _startHour;
                imgUp.IsVisible = false;
            }

            #endregion

            bool createRows = isNewDay;
            bool createColumns = isNewDay;
            gridDetail.Children.Clear();

            for (int row = 0; row < _rowsToShow; row++)
            {
                #region ViewValidation

                GetHour(_hourToShow, sbTimeStart);
                GetHour(_hourToShow + 1, sbTimeStart, true);

                #endregion

                if (createRows) { gridDetail.RowDefinitions.Add(new RowDefinition { Height = 50 }); }

                for (int column = 0; column <= _binding.Pitches.Count; column++)
                {
                    #region Detail

                    if (column == 0)
                    {
                        if (createColumns) { gridDetail.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) }); }
                        gridDetail.Children.Add(new Button { Text = sbTimeStart.ToString(), Style = (Style)Resources["titleButton"], FontSize = _fontSize }, column, row);
                    }
                    else
                    {
                        if (createColumns) { gridDetail.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); }

                        var result = _binding.ValidateBusyTime(_binding.Pitches[column - 1].Id, _hourToShow);
                        Button button = null;

                        if (result != null && (result.Status != ReservationStatus.None && result.Status != ReservationStatus.CancelPlace))
                        {
                            if (result.Source == 1)
                            {
                                DateTime dtButton = new DateTime(dpDate.Date.Year, dpDate.Date.Month, dpDate.Date.Day, _hourToShow, _place.FormatHour, 0);
                                result.Place = _place;
                                result.Pitch.PitchType = _pitchType;

                                button = new Button { Style = GetStyle(result.Status), Command = _binding.ReservationSummaryCommand };
                                button.CommandParameter = new CommandWrapper { Button = button, Reservation = result };
                            }
                            else
                            {
                                button = new Button { Style = (Style)Resources["blockButton"] };
                            }

                            gridDetail.Children.Add(button, column, row);
                        }
                        else
                        {
                            if (dpDate.Date.Date < DateTime.Now.Date)
                            {
                                button = new Button { Style = (Style)Resources["notAvailableButton"] };
                            }
                            else
                            {
                                DateTime dtButton = new DateTime(dpDate.Date.Year, dpDate.Date.Month, dpDate.Date.Day, _hourToShow, _place.FormatHour, 0);
                                ReservationModel reservation = new ReservationModel { Place = _place, Pitch = new PitchModel { Id = _binding.Pitches[column - 1].Id, PitchType = _pitchType }, Date = dtButton };

                                button = new Button { Style = (Style)Resources["availableButton"], Command = _binding.ReservationCommand };
                                button.CommandParameter = new CommandWrapper { Button = button, Reservation = reservation };
                            }

                            gridDetail.Children.Add(button, column, row);
                        }
                    }

                    #endregion
                }

                createColumns = false;
                _hourToShow++;
            }
        }

        private Style GetStyle(ReservationStatus status)
        {
            Style style = (Style)Resources["pendingButton"];
            switch (status)
            {
                case ReservationStatus.Ok:
                    style = (Style)Resources["okButton"];
                    break;
                case ReservationStatus.Running:
                    style = (Style)Resources["runningButton"];
                    break;
                case ReservationStatus.Close:
                    style = (Style)Resources["closeButton"];
                    break;
                case ReservationStatus.CancelPlace:
                    style = (Style)Resources["availableButton"];
                    break;
            }

            return style;
        }

        void GetHour(int hourToShow, StringBuilder sb, bool end = false)
        {
            if (!end) { sb.Clear(); }
            else { sb.AppendLine(); }

            int originalHour = hourToShow;
            if (hourToShow > 12) { hourToShow = hourToShow - 12; }

            if (_place.FormatHour > 0)
            {
                sb.AppendFormat("{0}:{1}", hourToShow, (_place.FormatHour < 10 ? "0" + _place.FormatHour : _place.FormatHour.ToString()));
            }
            else { sb.Append(hourToShow); }

            if (originalHour >= 12 && originalHour <= 23) { sb.Append("pm"); }
            else { sb.Append("am"); }
        }

        async void GetSchedule()
        {
            await _binding.GetSchedule();
        }

        protected override void OnAppearing()
        {
            if (ReservationService.HasChanges)
            {
                ReservationService.Button.Style = GetStyle(ReservationService.Reservation.Status);
                if (ReservationService.Reservation.Status == ReservationStatus.CancelPlace) { ReservationService.Button.Command = _binding.ReservationCommand; }
                else { ReservationService.Button.Command = _binding.ReservationSummaryCommand; }

                if (ReservationService.IsNew)
                {
                    ReservationModel reservation = new ReservationModel
                    {
                        Pitch = new PitchModel { Id = ReservationService.Reservation.Pitch.Id },
                        Date = ReservationService.Reservation.Date,
                        Status = ReservationService.Reservation.Status,
                        IdReservation = ReservationService.Reservation.IdReservation,
                        Source = 1
                    };

                    _binding.PitchesBusy.Add(reservation);
                }
            }
        }
    }
}
