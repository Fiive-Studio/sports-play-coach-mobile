using FutbolPlay.WebApi.Model;
using FutbolPlay.WebApi.Services;
using FutbolPlayAdmin.Services;
using FutbolPlayAdmin.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FutbolPlayAdmin.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Properties

        // Status
        public bool ProgramationStatus { get { return SessionCustomerService.Place.HasMutiple; } }

        // Actions
        public ICommand ProgramationCommand { get; set; }
        public ICommand ProgramationMultipleCommand { get; set; }
        public ICommand PendingReservationCommand { get; set; }
        public ICommand ProfileCommand { get; set; }
        public ICommand LogoffCommand { get; set; }

        #endregion

        #region Methods

        public HomeViewModel(Page page) : base(page)
        {
            ProgramationCommand = new Command(async () => { ReservationService.HasChanges = false; await OpenPageAsync(new ScheduleView(PitchType.Single)); });
            ProgramationMultipleCommand = new Command(async () => { ReservationService.HasChanges = false; await OpenPageAsync(new ScheduleView(PitchType.Multiple)); });
            PendingReservationCommand = new Command(async () => { ReservationService.HasChanges = false; await OpenPageAsync(new ReservationsListView()); });
            ProfileCommand = new Command(async () => { await OpenPageAsync(new ProfileView()); });
            LogoffCommand = new Command(() => { App.LogOff(); });
        }

        #endregion
    }
}
