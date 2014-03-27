namespace StickyNotes.Pages
{
    using Microsoft.Phone.Controls;
    using StickyNotes.Data;
    using StickyNotes.Services;
    using System.ComponentModel;
    using Microsoft.Phone.Shell;
    using System.Windows;

    public abstract class BaseStickyNotesPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public void InitializeDataContext()
        {
            this.DataContext = this;
        }

        public bool PageLoaded
        {
            get { return !PageLoading; }
            set { PageLoading = !value; }
        }
        public bool PageLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set
            {
                SystemTray.ProgressIndicator.Text = value ? "Loading..." : string.Empty;
                SystemTray.ProgressIndicator.IsVisible = value;
                SystemTray.ProgressIndicator.IsIndeterminate = value;
                NotifyPropertyChanged("PageLoading");
                NotifyPropertyChanged("PageLoaded");
            }
        }

        public StickyNotesSettingsManager SettingsManager
        {
            get { return Locator.Instance<StickyNotesSettingsManager>(); }
        }

        public OnlineRepository OnlineRepository
        {
            get { return Locator.Instance<OnlineRepository>(); }
        }

        public LocalRepository LocalRepository
        {
            get { return Locator.Instance<LocalRepository>(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
