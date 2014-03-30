﻿namespace StickyNotes
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Navigation;
    using StickyNotes.Data;
    using StickyNotes.Pages;

    /// <summary>
    /// Provides view code for the Board List page.
    /// </summary>
    public partial class BoardList : BasePage
    {
        public BoardList()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.RefreshBoards();
            base.OnNavigatedTo(e);
        }

        private ObservableCollection<Board> boards;

        public ObservableCollection<Board> Boards
        {
            get { return boards; }
            set { boards = value; NotifyPropertyChanged("Boards"); }
        }

        private void RefreshBoards(string query = "")
        {
            var boards = this.LocalRepository.GetBoard();

            if (!string.IsNullOrWhiteSpace(query))
            {
                boards = boards.Where(n => n.Name.Contains(this.SearchBox.Text)).ToList();
            }

            boards = boards.OrderBy(n => n.Name).ToList();
            Boards = new ObservableCollection<Board>(boards);
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.SearchBox.Text = string.Empty;
            this.RefreshBoards();
        }

        private void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.RefreshBoards(this.SearchBox.Text);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var board = frameworkElement.DataContext as Board;

            NavigationService.Navigate(new Uri(string.Format("/Pages/NoteList.xaml?boardId={0}", board.Id),
                UriKind.Relative));
        }

        private void TextBlock_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var board = frameworkElement.DataContext as Board;

            var result = MessageBox.Show(string.Format("Are you sure you want to delete board \"{0}\"?", board.Name), "Delete Board", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                this.OnlineRepository.BoardDelete(SettingsManager.SessionToken, board, (response) =>
                {
                    if (response.code == System.Net.HttpStatusCode.Forbidden)
                    {
                        MessageBox.Show(string.Format("You can't delete boards you don't own.", board.Name), "Error", MessageBoxButton.OK);
                    }
                    else if (!response.WasSuccessful())
                    {
                        MessageBox.Show(string.Format("Unable to delete board \"{0}\". It may have been deleted already.", board.Name), "Error", MessageBoxButton.OK);
                    }

                    this.RedownloadBoards();
                });
            }
        }

        private void BoardsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
        }

        private void RedownloadBoards()
        {
            this.OnlineRepository.BoardList(this.SettingsManager.SessionToken, (boardsResponse) =>
            {
                if (boardsResponse.WasSuccessful() && boardsResponse.data.boards != null)
                {
                    this.LocalRepository.ClearBoard();
                    this.LocalRepository.StoreBoard(boardsResponse.data.boards);
                    this.LocalRepository.Commit();
                    this.RefreshBoards();
                }
            });
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.SettingsManager.SessionToken == null)
            {
                NavigationService.Navigate(new Uri("/Pages/Login.xaml?redirectTo=/Pages/BoardList.xaml", UriKind.Relative));
                return;
            }

            this.RedownloadBoards();
        }

        private void NotesOnPhoneButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AddBoard.xaml", UriKind.Relative));
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Settings.xaml", UriKind.Relative));
        }
    }
}