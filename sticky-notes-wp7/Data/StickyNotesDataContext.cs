namespace StickyNotes.Data
{
    using System.Data.Linq;

    public class StickyNotesDataContext : DataContext
    {
        public const string DBConnectionString = "Data Source=isostore:/StickyNotes.sdf";

        public StickyNotesDataContext(string connectionString) : base(connectionString) { }

        public Table<Note> Notes;
        public Table<Board> Boards;
    }
}
