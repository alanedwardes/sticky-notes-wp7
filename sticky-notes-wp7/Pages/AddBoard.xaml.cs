﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using StickyNotes.Data;
using StickyNotes.Services;

namespace StickyNotes.Pages
{
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
            this.PageLoading = true;
            this.OnlineRepository.BoardsSave(this.SettingsManager.SessionToken, this.CurrentBoard, (response) => {
                if (response.code != 201)
                {
                    MessageBox.Show("Unable to create board!", "Error", MessageBoxButton.OK);
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/BoardList.xaml", UriKind.Relative));
                }

                this.PageLoaded = true;
            });
        }
    }
}