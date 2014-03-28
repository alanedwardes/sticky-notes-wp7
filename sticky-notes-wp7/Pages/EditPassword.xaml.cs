namespace StickyNotes.Pages
{
    using System;
    using System.Net;
    using System.Windows;

    public partial class EditPassword : BasePage
    {
        public EditPassword()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private string oldPassword;

        public string OldPassword
        {
            get { return oldPassword; }
            set { oldPassword = value; NotifyPropertyChanged("OldPassword"); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; NotifyPropertyChanged("Password"); }
        }

        private string password2;

        public string Password2
        {
            get { return password2; }
            set { password2 = value; NotifyPropertyChanged("Password2"); }
        }

        public void ChangeButton_Click(object sender, EventArgs e)
        {
            this.OnlineRepository.UserPassword(SettingsManager.SessionToken, Password, Password2, OldPassword, (response) =>
            {
                if (response.code == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Sorry, your old password was incorrect.", "Error", MessageBoxButton.OK);
                }
                else if (response.code == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Sorry, your passwords didn't match.", "Error", MessageBoxButton.OK);
                }
                else if (response.WasSuccessful())
                {
                    MessageBox.Show("Your password was changed.", "Success", MessageBoxButton.OK);
                    NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Sorry, there was an error changing your password.", "Error", MessageBoxButton.OK);
                }
            });
        }
    }
}