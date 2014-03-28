namespace StickyNotes.Pages
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Navigation;

    public partial class InviteUser : BasePage
    {
        public InviteUser()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; NotifyPropertyChanged("Email"); }
        }

        private int BoardId;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string id;
            if (NavigationContext.QueryString.TryGetValue("boardId", out id))
            {
                this.BoardId = int.Parse(id);
            }

            base.OnNavigatedTo(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.OnlineRepository.BoardInvite(SettingsManager.SessionToken, this.Email, this.BoardId, (response) =>
            {
                if (response.code != HttpStatusCode.Created)
                {
                    MessageBox.Show("There was an error adding the user to the board. The board may have been deleted, the user may not exist, or the user may already have been added the board.", "Error", MessageBoxButton.OK);
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
                }
            });
        }
    }
}