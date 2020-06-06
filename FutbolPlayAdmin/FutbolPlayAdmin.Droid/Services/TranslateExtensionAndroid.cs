using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Globalization;
using FutbolPlayAdmin.Services;
using Xamarin.Forms;
using System.Threading;

[assembly: Dependency(typeof(FutbolPlayAdmin.Droid.Services.TranslateExtensionAndroid))]


namespace FutbolPlayAdmin.Droid.Services
{
    public class TranslateExtensionAndroid : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-");
            Console.WriteLine("android:" + androidLocale.ToString());
            Console.WriteLine("net:" + netLanguage);

            return new CultureInfo(netLanguage);
        }

        public void SetLocale()
        {
            var androidLocale = Java.Util.Locale.Default; // user's preferred locale
            var netLocale = androidLocale.ToString().Replace("_", "-");
            var ci = new CultureInfo(netLocale);

            NumberFormatInfo info3 = new NumberFormatInfo();
            info3.PositiveSign = "+";
            info3.NegativeSign = "-";
            info3.NumberDecimalSeparator = ".";
            info3.NumberDecimalDigits = 3;
            info3.NumberGroupSeparator = ",";
            info3.PercentDecimalSeparator = ".";
            info3.PercentDecimalDigits = 3;
            info3.PercentGroupSeparator = ",";
            info3.PercentSymbol = "%";
            info3.CurrencyDecimalSeparator = ".";
            info3.CurrencyDecimalDigits = 3;
            info3.CurrencyGroupSeparator = ",";
            info3.CurrencySymbol = "$";
            ci.NumberFormat = info3;

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}