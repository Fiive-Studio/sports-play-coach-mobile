using FutbolPlay.WebApi.Model;
using FutbolPlayAdmin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FutbolPlayAdmin.Views
{
    public partial class ReservationCreateView : ContentPage
    {
        public ReservationCreateView(ReservationModel reservation)
        {
            InitializeComponent();
            ReservationCreateViewModel binding = new ReservationCreateViewModel(this, reservation);
            BindingContext = binding;

            binding.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IdUser" && binding.IdUser != -1)
                {
                    entMail.IsEnabled = entPhone.IsEnabled = false;
                }
            };

            sbName.TextChanged += (s, e) =>
            {
                if (sbName.Text == string.Empty)
                {
                    binding.Name = binding.Phone = binding.Mail = string.Empty;
                    binding.IdUser = -1;
                    entMail.IsEnabled = entPhone.IsEnabled = true;
                }
            };
            entValue.TextChanged += (s, e) => 
            {
                if (string.IsNullOrWhiteSpace(e.NewTextValue)) { entValue.Text = "0"; }
            };
        }
    }
}
