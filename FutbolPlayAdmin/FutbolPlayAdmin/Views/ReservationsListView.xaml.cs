using FutbolPlayAdmin.ViewModels;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T = FutbolPlayAdmin.Services.TranslateExtension;
using I = FutbolPlayAdmin.Services.ImageResourceExtension;

using Xamarin.Forms;
using System.Globalization;
using FutbolPlayAdmin.Layout;
using FutbolPlayAdmin.Services;

namespace FutbolPlayAdmin.Views
{
    public partial class ReservationsListView : ContentPage
    {
        ReservationsListViewModel _binding;
        ReservationModel _currentItem;

        public ReservationsListView()
        {
            InitializeComponent();
            _binding = new ReservationsListViewModel(this);
            _binding.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Reservations")
                {
                    if (_binding.Reservations == null) { return; }

                    lsReservations.HasUnevenRows = true;
                    lsReservations.ItemTemplate = new DataTemplate(typeof(ReservationCell));
                    lsReservations.ItemsSource = _binding.Reservations;
                    lsReservations.SeparatorColor = Color.FromHex("#ddd");
                    lsReservations.IsRefreshing = false;
                }
            };

            lsReservations.ItemTapped += async (sender, args) =>
            {
                _currentItem = (ReservationModel)args.Item;
                await _binding.OpenReservationSummaryAsync(_currentItem);
                lsReservations.SelectedItem = null;
            };

            lsReservations.BackgroundColor = Color.Transparent;

            BindingContext = _binding;
            GetReservations();
        }

        async void GetReservations()
        {
            await _binding.GetReservations();
        }

        protected override void OnAppearing()
        {
            if (_currentItem != null && ReservationService.HasChanges)
            {
                _binding.Reservations.Remove(_currentItem);
                ReservationService.HasChanges = false;
            }
        }
    }
}
