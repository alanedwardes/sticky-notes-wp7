﻿namespace StickyNotes.Data
{
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    /// <summary>
    /// Provides storage for Board objects, with component model events.
    /// </summary>
    [Table]
    public class Board : INotifyPropertyChanged
    {
        private int localStorageId;

        [Column(IsPrimaryKey = true,
            IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int LocalStorageId
        {
            get { return localStorageId; }
            set { localStorageId = value; NotifyPropertyChanged("LocalStorageId"); }
        }

        private int id;

        [Column(DbType = "INT")]
        public int Id
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("Id"); }
        }

        private string name;

        [Column(DbType = "NVarChar(255) NOT NULL",
            CanBeNull = false)]
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}