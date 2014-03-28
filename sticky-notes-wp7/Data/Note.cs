namespace StickyNotes.Data
{
    using System;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;

    /// <summary>
    /// Provides storage for Note objects, with component model events.
    /// </summary>
    [Table]
    public class Note : INotifyPropertyChanged
    {
        private int localStorageId;

        /// <summary>
        /// A local primary key for the note.
        /// </summary>
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

        /// <summary>
        /// The online ID of the note.
        /// </summary>
        [Column(DbType = "INT")]
        public int Id
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("Id"); }
        }

        private int boardId;

        [Column(DbType = "INT")]
        public int BoardId
        {
            get { return boardId; }
            set { boardId = value; NotifyPropertyChanged("BoardId"); }
        }

        private string title;

        [Column(DbType = "NVarChar(255) NULL", CanBeNull = true)]
        public string Title
        {
            get { return title; }
            set { title = value; NotifyPropertyChanged("Title"); }
        }

        private string body;

        [Column(DbType = "NVarChar(1024) NOT NULL", CanBeNull = false)]
        public string Body
        {
            get { return body; }
            set { body = value; NotifyPropertyChanged("Body"); }
        }

        private DateTime created;

        [Column(DbType = "DATETIME")]
        public DateTime Created
        {
            get { return created; }
            set { created = value; NotifyPropertyChanged("Created"); }
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