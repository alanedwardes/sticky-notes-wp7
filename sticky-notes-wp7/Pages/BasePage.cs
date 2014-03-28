namespace StickyNotes.Pages
{
    using System.ComponentModel;
    using Microsoft.Phone.Controls;
    using StickyNotes.Services;

    /// <summary>
    /// Provides a base for all Sticky Notes app
    /// view models, with access to common services.
    /// </summary>
    public abstract class BasePage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public void InitializeDataContext()
        {
            this.DataContext = this;
        }

        public StickyNotesSettingsManager SettingsManager
        {
            get { return Locator.Instance<StickyNotesSettingsManager>(); }
        }

        public StickyNotesOnlineRepository OnlineRepository
        {
            get { return Locator.Instance<StickyNotesOnlineRepository>(); }
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