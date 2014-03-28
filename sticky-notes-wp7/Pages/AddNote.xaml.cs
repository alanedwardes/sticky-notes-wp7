﻿namespace StickyNotes.Views
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Input;
    using StickyNotes.Data;
    using StickyNotes.Pages;

    public partial class AddNote : BaseStickyNotesPage
    {
        private Note currentNote;
        public Note CurrentNote
        {
            get { return currentNote; }
            set { currentNote = value; NotifyPropertyChanged("CurrentNote"); }
        }

        private Board currentBoard;
        public bool InEditMode;

        public AddNote()
        {
            InitializeComponent();
            InitializeDataContext();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base method.
            base.OnNavigatedFrom(e);
            this.LocalRepository.Commit();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); 
            string noteId;
            if (NavigationContext.QueryString.TryGetValue("noteId", out noteId))
            {
                this.CurrentNote = this.LocalRepository.GetNote().Where(n => n.LocalStorageId == int.Parse(noteId)).Single();
                this.PageTitle.Text = "edit note";
                this.InEditMode = true;
            }
            else
            {
                this.CurrentNote = new Note();
                this.PageTitle.Text = "new note";
                this.InEditMode = false;
            }

            string boardId;
            if (NavigationContext.QueryString.TryGetValue("boardId", out boardId))
            {
                this.currentBoard = this.LocalRepository.GetBoard().Where(b => b.LocalStorageId == int.Parse(boardId)).Single();
            }
            else
            {
                this.currentBoard = null;
            }
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NoteBody.Text))
            {
                if (!this.InEditMode)
                {
                    this.CurrentNote.Created = DateTime.Now;
                    this.LocalRepository.StoreNote(currentNote);
                }

                this.LocalRepository.Commit();

                if (this.currentBoard != null)
                {
                    this.OnlineRepository.NotesSave(this.SettingsManager.SessionToken, this.currentNote, this.currentBoard.Id, (reaponse) =>
                    {
                        if (reaponse.WasSuccessful())
                        {
                            NavigationService.Navigate(new Uri(string.Format("/Pages/NoteList.xaml?boardId={0}", this.currentBoard.Id), UriKind.Relative));
                        }
                        else
                        {
                            MessageBox.Show("Unable to add note to board.", "Error", MessageBoxButton.OK);
                        }
                    });
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/NoteList.xaml", UriKind.Relative));
                }
            }
            else
            {
                MessageBox.Show("Please enter some note text.", "Error", MessageBoxButton.OK);
            }
        }

        private void NoteBody_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.SaveNote.Focus();
            }
        }

        private void NoteTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.NoteBody.Focus();
            }
        }
    }
}