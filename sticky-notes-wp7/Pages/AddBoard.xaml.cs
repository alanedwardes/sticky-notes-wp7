namespace StickyNotes.Pages
{
    using System;
    using System.Windows;
    using System.Windows.Navigation;
    using StickyNotes.Data;

    public partial class AddBoard : BaseStickyNotesPage
    {
        public AddBoard()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        private Board currentBoard;

        public Board CurrentBoard
        {
            get { return currentBoard; }
            set { currentBoard = value; NotifyPropertyChanged("CurrentBoard"); }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.CurrentBoard = new Board();
        }

        private void AddBoardButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnlineRepository.BoardsSave(this.SettingsManager.SessionToken, this.CurrentBoard, (response) => {
                if (!response.WasSuccessful())
                {
                    MessageBox.Show("Unable to create board!", "Error", MessageBoxButton.OK);
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
                }
            });
        }
    }
}