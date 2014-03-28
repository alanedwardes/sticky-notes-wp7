namespace StickyNotes.Pages
{
    using System.Windows;

    public partial class Settings : BaseStickyNotesPage
    {
        public Settings()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                isLoggedIn = value;
                NotifyPropertyChanged("IsLoggedIn");
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!string.IsNullOrEmpty(this.SettingsManager.SessionToken))
            {
                this.OnlineRepository.UserGet(this.SettingsManager.SessionToken, (response) =>
                {
                    this.IsLoggedIn = response.WasSuccessful();
                });
            }
        }

        private void Logout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.SettingsManager.SessionToken = null;
                this.IsLoggedIn = false;
            }
        }
    }
}