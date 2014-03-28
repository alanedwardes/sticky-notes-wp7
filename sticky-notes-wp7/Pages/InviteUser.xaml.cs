namespace StickyNotes.Pages
{
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
                MessageBox.Show(response.code.ToString());
                if (response.code == HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("User doesn't exist.", "Error", MessageBoxButton.OK);
                }
            });
        }
    }
}