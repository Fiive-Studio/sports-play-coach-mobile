using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace FutbolPlayAdmin.Services
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        readonly static CultureInfo ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
        readonly static string ResourceId = "FutbolPlayAdmin.Resources.Values.AppResources";

        public TranslateExtension() { }

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null) { return ""; }
            return GetValue(Text);
        }

        public static string GetValue(string key)
        {
            ResourceManager resmgr = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            var translation = resmgr.GetString(key, ci);
            if (translation == null) { translation = key; }

            return translation;
        }
    }

    public enum MessagesTexts
    {
        error_message,
        error_code_003,
        error_code_004,
        error_code_005,
        error_code_006,
        error_code_008,
        error_code_009,
        error_code_012,
        error_code_014,
        error_code_016,
        update_profile_message,
        password_differents_message,
        password_changed,
        recover_password_message,
        reservation_ok,
        reservation_pending,
        reservation_conflict,
        reservation_cancel_ok,
        change_status_ok,
        search_without_data
    }
}
