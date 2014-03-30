namespace StickyNotes.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Navigation;
    using StickyNotes.Data;

    public partial class BoardMembers : BasePage
    {
        public BoardMembers()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private int BoardId;

        private List<User> boardMembers;

        public List<User> BoardMemberList
        {
            get { return boardMembers; }
            set { boardMembers = value; NotifyPropertyChanged("BoardMemberList"); }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;
            if (NavigationContext.QueryString.TryGetValue("boardId", out id))
            {
                this.BoardId = int.Parse(id);
            }

            this.OnlineRepository.BoardUsers(SettingsManager.SessionToken, BoardId, (response) =>
            {
                if (response.WasSuccessful() && response.data != null)
                {
                    this.BoardMemberList = response.data.Users;
                }
            });
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/InviteUser.xaml?boardId=" + this.BoardId, UriKind.Relative));
        }
    }
}