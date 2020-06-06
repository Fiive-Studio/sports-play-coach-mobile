using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FutbolPlayAdmin.Services
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public static string ImagePath { get { return "FutbolPlayAdmin.Resources.Drawable."; } }

        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) { return null; }

            var imageSource = ImageSource.FromResource(Source);
            return imageSource;
        }
    }
}
