namespace StickyNotes.Data
{
    using System.Data.Linq;

    /// <summary>
    /// Provides specific data storage needs for the Sticky Notes application.
    /// </summary>
    public class StickyNotesDataContext : DataContext
    {
        public const string DBConnectionString = "Data Source=isostore:/StickyNotes.sdf";

        public StickyNotesDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public Table<Note> Notes;
        public Table<Board> Boards;
    }
}