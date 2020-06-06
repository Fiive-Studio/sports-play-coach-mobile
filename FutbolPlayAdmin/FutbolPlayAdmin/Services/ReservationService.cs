using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FutbolPlayAdmin.Services
{
    public class ReservationService
    {
        public static Button Button { get; set; }
        public static ReservationModel Reservation { get; set; }
        public static bool HasChanges { get; set; }
        public static bool IsNew { get; set; }
    }
}
