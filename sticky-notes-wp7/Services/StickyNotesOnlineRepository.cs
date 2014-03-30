namespace StickyNotes.Services
{
    using System;
    using System.Collections.Generic;
    using StickyNotes.Data;

    public class StickyNotesOnlineRepository : OnlineRepository
    {
        private const string ENDPOINT = "http://stickyapi.alanedwardes.com/";

        private struct API
        {
            public struct User
            {
                public const string Save = "user/register";
                public const string Edit = "user/editDetails";
                public const string Login = "user/login";
                public const string Password = "user/editPassword";
                public const string Get = "user/getUser";
            }

            public struct Board
            {
                public const string List = "boards/list";
                public const string Save = "boards/save";
                public const string Delete = "boards/delete";
                public const string Invite = "board/addUser";
                public const string Users = "board/getUsers";
            }

            public struct Note
            {
                public const string List = "notes/list";
                public const string Save = "notes/save";
                public const string Delete = "notes/delete";
                public const string Edit = "notes/edit";
            }
        }

        public class Session
        {
            public string id;
            public string created;
        }

        private void Post<T>(string apiMethod, Dictionary<string, string> parameters, Action<OnlineRepository.RepositoryResponse<T>> action)
        {
            base.HttpPost<T>(new Uri(ENDPOINT + apiMethod), parameters, action);
        }

        public void UserLogin(User user, Action<RepositoryResponse<UserLoginResponse>> action)
        {
            this.Post<UserLoginResponse>(API.User.Login, new Dictionary<string, string> {
                { "username", user.Email },
                { "password", user.Password }
            }, action);
        }

        public class UserLoginResponse { public User user; public Session session; }

        public void UserLogin(string username, string password, Action<RepositoryResponse<UserLoginResponse>> action)
        {
            this.Post<UserLoginResponse>(API.User.Login, new Dictionary<string, string> {
                { "username", username },
                { "password", password }
            }, action);
        }

        public class UserGetResponse : User { }

        public void UserGet(string token, Action<RepositoryResponse<UserGetResponse>> action)
        {
            this.Post<UserGetResponse>(API.User.Get, new Dictionary<string, string> {
                { "token", token }
            }, action);
        }

        public class UserSaveResponse : User { }

        public void UserSave(User newUser, Action<RepositoryResponse<UserSaveResponse>> action)
        {
            this.Post<UserSaveResponse>(API.User.Save, new Dictionary<string, string> {
                { "firstName", newUser.FirstName },
                { "surname", newUser.Surname },
                { "email", newUser.Email },
                { "password", newUser.Password }
            }, action);
        }

        public void UserEdit(string token, User editedUser, Action<RepositoryResponse<UserSaveResponse>> action)
        {
            this.Post<UserSaveResponse>(API.User.Edit, new Dictionary<string, string> {
                { "token", token },
                { "firstName", editedUser.FirstName },
                { "surname", editedUser.Surname },
                { "email", editedUser.Email },
            }, action);
        }

        public class UserPasswordResponse { }

        public void UserPassword(string token, string password, string password2, string oldPassword, Action<RepositoryResponse<UserPasswordResponse>> action)
        {
            this.Post<UserPasswordResponse>(API.User.Password, new Dictionary<string, string> {
                { "token", token },
                { "password", password },
                { "password2", password2 },
                { "oldPassword", oldPassword }
            }, action);
        }

        public class BoardListResponse { public List<Board> boards; }

        public void BoardList(string token, Action<RepositoryResponse<BoardListResponse>> action)
        {
            this.Post<BoardListResponse>(API.Board.List, new Dictionary<string, string> {
                { "token", token }
            }, action);
        }

        public class BoardDeleteResponse { }

        public void BoardDelete(string token, Board board, Action<RepositoryResponse<BoardDeleteResponse>> action)
        {
            this.Post<BoardDeleteResponse>(API.Board.Delete, new Dictionary<string, string> {
                { "token", token },
                { "id", board.Id.ToString() }
            }, action);
        }

        public class BoardSaveResponse : Board { }

        public void BoardSave(string token, Board board, Action<RepositoryResponse<BoardSaveResponse>> action)
        {
            this.Post<BoardSaveResponse>(API.Board.Save, new Dictionary<string, string> {
                { "token", token },
                { "name", board.Name }
            }, action);
        }

        public class NoteListResponse { public List<Note> notes; }

        public void NoteList(string token, int boardId, Action<RepositoryResponse<NoteListResponse>> action)
        {
            this.Post<NoteListResponse>(API.Note.List, new Dictionary<string, string> {
                { "token", token },
                { "boardID", boardId.ToString() }
            }, action);
        }

        public class NoteSaveResponse : Note { }

        public void NoteSave(string token, Note note, int boardId, Action<RepositoryResponse<NoteSaveResponse>> action)
        {
            this.Post<NoteSaveResponse>(API.Note.Save, new Dictionary<string, string> {
                { "token", token },
                { "title", note.Title },
                { "body", note.Body },
                { "boardID", boardId.ToString() }
            }, action);
        }

        public class NoteDeleteResponse { }

        public void NoteDelete(string token, Note note, Action<RepositoryResponse<NoteDeleteResponse>> action)
        {
            this.Post<NoteDeleteResponse>(API.Note.Delete, new Dictionary<string, string> {
                { "token", token },
                { "id", note.Id.ToString() },
            }, action);
        }

        public class NoteEditResponse { }

        public void NoteEdit(string token, Note note, Action<RepositoryResponse<NoteEditResponse>> action)
        {
            this.Post<NoteEditResponse>(API.Note.Edit, new Dictionary<string, string> {
                { "token", token },
                { "id", note.Id.ToString() },
                { "title", note.Title },
                { "body", note.Body },
            }, action);
        }

        public class BoardInviteResponse { }

        public void BoardInvite(string token, string email, int boardId, Action<RepositoryResponse<BoardInviteResponse>> action)
        {
            this.Post<BoardInviteResponse>(API.Board.Invite, new Dictionary<string, string> {
                { "token", token },
                { "email", email },
                { "boardID", boardId.ToString() }
            }, action);
        }

        public class BoardUsersResponse { public List<User> Users; }

        public void BoardUsers(string token, int boardId, Action<RepositoryResponse<BoardUsersResponse>> action)
        {
            this.Post<BoardUsersResponse>(API.Board.Users, new Dictionary<string, string> {
                { "token", token },
                { "id", boardId.ToString() }
            }, action);
        }
    }
}