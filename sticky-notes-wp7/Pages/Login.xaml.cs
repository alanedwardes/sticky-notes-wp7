namespace StickyNotes
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Navigation;
    using StickyNotes.Pages;
    using StickyNotes.Services;

    public partial class Login : BaseStickyNotesPage
    {
        private string redirectUri;

        public Login()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private void LogonButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnlineRepository.UserLogin(this.username.Text, this.password.Password, (response) => {
                if (response.WasSuccessful())
                {
                    SaveUserTokenFromLoginResponse(response);
                }
                else
                {
                    ShowMessageBasedOnResponseCode(response.code);
                }
            });
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Register.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string redirectTo;
            if (NavigationContext.QueryString.TryGetValue("redirectTo", out redirectTo))
            {
                NavigationService.RemoveBackEntry();
                redirectUri = redirectTo;
            }
        }

        private void ShowMessageBasedOnResponseCode(System.Net.HttpStatusCode code)
        {
            switch (code)
            {
                case HttpStatusCode.Forbidden:
                    MessageBox.Show("Invalid username or password.", "Incorrect Credentials", MessageBoxButton.OK);
                    break;
                default:
                    MessageBox.Show("An error occurred whilst logging in. Please try again.", "Login Error", MessageBoxButton.OK);
                    break;
            }
        }

        private void SaveUserTokenFromLoginResponse(OnlineRepository.RepositoryResponse<OnlineRepository.UserLoginResponse> response)
        {
            this.SettingsManager.SessionToken = response.data.session.id;

            if (redirectUri != null)
            {
                NavigationService.Navigate(new Uri(redirectUri, UriKind.Relative));
            }
        }
    }
}