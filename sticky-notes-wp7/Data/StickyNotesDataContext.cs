using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;

namespace StickyNotes.Data
{
    public class StickyNotesDataContext : DataContext
    {
        public const string DBConnectionString = "Data Source=isostore:/StickyNotes.sdf";

        public StickyNotesDataContext(string connectionString) : base(connectionString) { }

        public Table<Note> Notes;
        public Table<Board> Boards;
    }
}
