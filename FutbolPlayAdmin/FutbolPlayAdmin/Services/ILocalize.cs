using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlayAdmin.Services
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale();
    }
}
