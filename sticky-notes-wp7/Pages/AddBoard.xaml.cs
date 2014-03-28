namespace StickyNotes.Pages
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Navigation;
    using StickyNotes.Data;

    /// <summary>
    /// Provides specific Add Board implementation code.
    /// </summary>
    public partial class AddBoard : BasePage
    {
        public AddBoard()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private Board currentBoard = new Board();

        public Board CurrentBoard
        {
            get { return currentBoard; }
            set { currentBoard = value; NotifyPropertyChanged("CurrentBoard"); }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.CurrentBoard = new Board();
        }

        private void AddBoardButton_Click(object sender, EventArgs e)
        {
            this.OnlineRepository.BoardsSave(this.SettingsManager.SessionToken, this.CurrentBoard, (response) =>
            {
                if (response.code == HttpStatusCode.Created)
                {
                    NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Unable to create board!", "Error", MessageBoxButton.OK);
                }
            });
        }
    }
}