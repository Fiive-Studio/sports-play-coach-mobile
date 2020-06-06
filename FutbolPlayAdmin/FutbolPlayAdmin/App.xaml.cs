using FutbolPlayAdmin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using FutbolPlayAdmin.Views;
using FutbolPlay.WebApi.Services;
using FutbolPlay.WebApi.Model;
using T = FutbolPlayAdmin.Services.TranslateExtension;

namespace FutbolPlayAdmin
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();

                if (Device.OS != TargetPlatform.Windows) { DependencyService.Get<ILocalize>().SetLocale(); }

                Page page = null;
                if (IsLogin) { page = new HomeView(); }
                else { page = new LoadingView(); }

                // The root page of your application
                MainPage = new NavigationPage(page) { BarBackgroundColor = GetBackgroundColor() };
                if (!IsLogin) { ValidateLoginAsync(); }
            }
            catch
            {
                MainPage.DisplayAlert(T.GetValue("app_name"), T.GetValue("error_message"), T.GetValue("ok"));
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #region Helpers

        private static Color GetBackgroundColor()
        {
            return Device.OnPlatform<Color>(Color.White, Color.Transparent, Color.Transparent);
        }

        // Navigation
        public static void NavigateToHome()
        {
            if (IsLogin)
            {
                Current.MainPage.Navigation.InsertPageBefore(new HomeView(), Current.MainPage.Navigation.NavigationStack[0]);
                Current.MainPage.Navigation.PopToRootAsync();
            }
        }
        public static async void NavigateToHomeAsync(UserModel user)
        {
            try
            {
                bool response = await RestService.Instance.RegisterAsync(user);

                if (response) { IsLogin = true; }
                NavigateToHome();
            }
            catch { NavigateToHome(); }
        }
        public static void ShowStartView()
        {
            Current.MainPage.Navigation.InsertPageBefore(new LoginView(), Current.MainPage.Navigation.NavigationStack[0]);
            Current.MainPage.Navigation.PopToRootAsync();
        }

        // Login
        public static bool IsLogin { get; set; }
        public static async void ValidateLoginAsync()
        {
            try
            {
                bool result = await SessionCustomerService.ValidateLoginAsync();
                if (result)
                {
                    IsLogin = true;
                    NavigateToHome();
                }
                else { ShowStartView(); }
            }
            catch { ShowStartView(); }
        }
        public static void LogOff()
        {
            SessionCustomerService.DeleteAccount();
            Current.MainPage = new NavigationPage(new LoginView()) { BarBackgroundColor = GetBackgroundColor() };
            IsLogin = false;
        }

        #endregion
    }
}