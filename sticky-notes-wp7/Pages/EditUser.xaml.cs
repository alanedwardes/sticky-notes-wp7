namespace StickyNotes.Pages
{
    using System;
    using System.Windows;
    using StickyNotes.Data;

    /// <summary>
    /// Provides view code for the Edit User page.
    /// </summary>
    public partial class EditUser : BasePage
    {
        public EditUser()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private User user;

        public User User
        {
            get { return user; }
            set { user = value; NotifyPropertyChanged("User"); }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.OnlineRepository.UserGet(SettingsManager.SessionToken, (response) =>
            {
                if (response.WasSuccessful() && response.data != null)
                {
                    this.User = response.data;
                }
                else
                {
                    MessageBox.Show("Error retrieving user!", "Error", MessageBoxButton.OK);
                }
            });

            base.OnNavigatedTo(e);
        }

        public void SaveButton_Click(object sender, EventArgs e)
        {
            this.OnlineRepository.UserEdit(SettingsManager.SessionToken, User, (response) =>
            {
                if (response.WasSuccessful())
                {
                    MessageBox.Show("Account details updated.", "Success", MessageBoxButton.OK);
                    NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("There was an error updating your account details.", "Error", MessageBoxButton.OK);
                }
            });
        }
    }
}