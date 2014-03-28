namespace StickyNotes.Pages
{
    using System;
    using System.Windows;
    using StickyNotes.Data;
    using Microsoft.Phone.Controls;

    public partial class Register : BaseStickyNotesPage
    {
        public Register()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private string currentAction;
        public string CurrentAction
        {
            get { return currentAction; }
            set
            {
                currentAction = value;
                NotifyPropertyChanged("CurrentAction");
            }
        }

        private User newUser;
        public User NewUser
        {
            get { return newUser; }
            set { newUser = value; NotifyPropertyChanged("NewUser"); }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NewUser = new User();
        }

        private void RegisterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CurrentAction = "Registering user...";

            this.OnlineRepository.UserSave(NewUser, (userSaveResponse) => {
                if (userSaveResponse.WasSuccessful())
                {
                    this.CurrentAction = "Logging in...";

                    this.OnlineRepository.UserLogin(NewUser, (userLoginResponse) => {
                        if (userLoginResponse.WasSuccessful())
                        {
                            this.SettingsManager.SessionToken = userLoginResponse.data.session.id;
                            NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
                        }
                        else
                        {
                            MessageBox.Show("Your account was created, but there was an error logging in. Please try again.", "Error", MessageBoxButton.OK);
                            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Error registering user", "Error", MessageBoxButton.OK);
                }
            });
        }
    }
}