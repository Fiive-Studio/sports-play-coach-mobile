using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutbolPlayAdmin.Services;
using Xamarin.Forms;
using System.Globalization;
using Foundation;
using System.Threading;

[assembly: Dependency(typeof(FutbolPlayAdmin.iOS.Services.TranslateExtensioniPhone))]

namespace FutbolPlayAdmin.iOS.Services
{
    public class TranslateExtensioniPhone : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var iosLocaleAuto = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;    // en_FR
            var iosLanguageAuto = NSLocale.AutoUpdatingCurrentLocale.LanguageCode;      // en
            var netLocale = iosLocaleAuto.Replace("_", "-");
            const string defaultCulture = "en";

            CultureInfo ci = null;
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                try
                {
                    var pref = NSLocale.PreferredLanguages[0];
                    var netLanguage = pref.Replace("_", "-");
                    ci = CultureInfo.CreateSpecificCulture(netLanguage);
                }
                catch
                {
                    ci = new CultureInfo(defaultCulture);
                }
            }
            else
            {
                ci = new CultureInfo(defaultCulture);
            }
            return ci;
        }

        public void SetLocale()
        {
            var ci = GetCurrentCultureInfo();

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