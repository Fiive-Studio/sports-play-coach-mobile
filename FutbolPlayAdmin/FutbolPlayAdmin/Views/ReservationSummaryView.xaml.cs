using FutbolPlayAdmin.ViewModels;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T = FutbolPlayAdmin.Services.TranslateExtension;

using Xamarin.Forms;

namespace FutbolPlayAdmin.Views
{
    public partial class ReservationSummaryView : ContentPage
    {
        public ReservationSummaryView(ReservationModel reservation, bool loadReservationData)
        {
            InitializeComponent();
            FillPickerStatus();
            BindingContext = new ReservationSummaryViewModel(this, reservation, loadReservationData);
            entValue.TextChanged += (s, e) => { if (string.IsNullOrWhiteSpace(entValue.Text)) { entValue.Text = "0"; } };
        }

        void FillPickerStatus()
        {
            picStatus.Items.Add(T.GetValue("status_pending"));
            picStatus.Items.Add(T.GetValue("status_ok"));
            picStatus.Items.Add(T.GetValue("status_cancel"));
            picStatus.Items.Add(T.GetValue("status_running"));
            picStatus.Items.Add(T.GetValue("status_close"));
        }
    }
}
