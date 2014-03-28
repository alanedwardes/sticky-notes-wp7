﻿namespace StickyNotes.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using StickyNotes.Data;

    /// <summary>
    /// Provides all specific implementation code for
    /// writing, reading and deleting models.
    /// </summary>
    public class LocalRepository
    {
        private StickyNotesDataContext dataContext;

        public LocalRepository(StickyNotesDataContext context)
        {
            this.dataContext = context;
        }

        public virtual List<Board> GetBoard()
        {
            return this.dataContext.Boards.ToList();
        }

        public virtual Board GetBoard(int boardId)
        {
            return this.dataContext.Boards.Where(b => b.Id == boardId).Single();
        }

        public virtual void ClearBoard()
        {
            this.dataContext.Boards.DeleteAllOnSubmit(this.dataContext.Boards);
        }

        public virtual void ClearBoard(List<Board> boards)
        {
            this.dataContext.Boards.DeleteAllOnSubmit(boards);
        }

        public virtual void ClearBoard(Board board)
        {
            this.dataContext.Boards.DeleteOnSubmit(board);
        }

        public virtual void StoreBoard(Board board)
        {
            this.dataContext.Boards.InsertOnSubmit(board);
        }

        public virtual void StoreBoard(List<Board> boards)
        {
            this.dataContext.Boards.InsertAllOnSubmit(boards);
        }

        public virtual List<Note> GetNote()
        {
            return this.dataContext.Notes.ToList();
        }

        public virtual Note GetNote(int noteId)
        {
            return this.dataContext.Notes.Where(n => n.Id == noteId).Single();
        }

        public virtual void ClearNote()
        {
            this.dataContext.Notes.DeleteAllOnSubmit(this.dataContext.Notes);
        }

        public virtual void ClearNote(List<Note> notes)
        {
            this.dataContext.Notes.DeleteAllOnSubmit(notes);
        }

        public virtual void ClearNote(Note note)
        {
            this.dataContext.Notes.DeleteOnSubmit(note);
        }

        public virtual void StoreNote(Note note)
        {
            this.dataContext.Notes.InsertOnSubmit(note);
        }

        public virtual void StoreNote(List<Note> notes)
        {
            this.dataContext.Notes.InsertAllOnSubmit(notes);
        }

        public virtual void Commit()
        {
            this.dataContext.SubmitChanges();
        }

        public virtual void Rollback()
        {
            this.dataContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, this.dataContext.Notes);
            this.dataContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, this.dataContext.Boards);
        }
    }
}