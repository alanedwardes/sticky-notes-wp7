namespace StickyNotes.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Text;
    using Microsoft.Phone.Net.NetworkInformation;
    using Newtonsoft.Json;
    using StickyNotes.Data;

    public class OnlineRepository : INotifyPropertyChanged
    {
        struct API
        {
            public struct User
            {
                public const string Save = "user/register";
                public const string Login = "user/login";
                public const string Get = "user/getUser";
            }
            public struct Board
            {
                public const string List = "boards/list";
                public const string Save = "boards/save";
            }
            public struct Note
            {
                public const string List = "notes/list";
                public const string Save = "notes/save";
            }
        }

        public class Session
        {
            public string id;
            public string created;
        }

        public class UserLoginResponse
        {
            public User user;
            public Session session;
        }

        public class UserSaveResponse : User { }

        public class UserGetResponse : User { }

        public class NotesListResponse
        {
            public List<Note> notes;
        }

        public class NotesSaveResponse : Note { }

        public class RepositoryResponse<T>
        {
            public HttpStatusCode code;
            public bool WasSuccessful() { return ((int)code >= 200 && (int)code < 300); }
            public T data;
        }

        public class BoardsListResponse
        {
            public List<Board> boards;
        }

        public class BoardsSaveResponse: Board
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        const string ENDPOINT = "http://stickyapi.alanedwardes.com/";

        public string DictionaryToQueryString(Dictionary<string, string> dictionary)
        {
            var stringBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (var dict in dictionary)
            {
                if (!isFirst) stringBuilder.Append("&");
                isFirst = false;
                stringBuilder.AppendFormat("{0}={1}",
                    HttpUtility.UrlEncode(dict.Key),
                    HttpUtility.UrlEncode(dict.Value));
            }

            return stringBuilder.ToString();
        }

        public bool HasLoaded
        {
            get { return !IsLoading; }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            private set
            {
                isLoading = value;
                NotifyPropertyChanged("IsLoading");
                NotifyPropertyChanged("HasLoaded");
            }
        }

        public void HttpPost<T>(string apiMethod, Dictionary<string, string> parameters, Action<RepositoryResponse<T>> action)
        {
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                action.Invoke(new RepositoryResponse<T> { code = HttpStatusCode.RequestTimeout });
                return;
            }

            this.IsLoading = true;

            var data = this.DictionaryToQueryString(parameters);

            var webClient = new WebClient();

            var timeoutTimer = new System.Windows.Threading.DispatcherTimer();
            timeoutTimer.Interval = new TimeSpan(0, 0, 2);
            timeoutTimer.Tick += new EventHandler((sender, e) => webClient.CancelAsync());

            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadStringCompleted += (object sender, UploadStringCompletedEventArgs e) =>
            {
                this.IsLoading = false;

                timeoutTimer.Stop();

                var repositoryResponse = new RepositoryResponse<T>();

                var serialiserSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCaseToUnderscorePropertyNamesContractResolver()
                };

                if (e.Cancelled)
                {
                    repositoryResponse.code = HttpStatusCode.RequestTimeout;
                }
                else if (e.Error != null)
                {
                    var webException = e.Error as WebException;
                    using (HttpWebResponse response = (HttpWebResponse)webException.Response)
                    {
                        if (response == null)
                        {
                            repositoryResponse.code = HttpStatusCode.RequestTimeout;
                        }
                        else
                        {
                            repositoryResponse.code = response.StatusCode;
                        }
                    }
                }
                else
                {
                    repositoryResponse.data = JsonConvert.DeserializeObject<T>(e.Result, serialiserSettings);
                    repositoryResponse.code = HttpStatusCode.OK;
                }

                action.Invoke(repositoryResponse);
            };
            webClient.UploadStringAsync(new Uri(ENDPOINT + apiMethod), data);
            timeoutTimer.Start();
        }

        public void UserLogin(User user, Action<RepositoryResponse<UserLoginResponse>> action)
        {
            this.HttpPost<UserLoginResponse>(API.User.Login, new Dictionary<string, string> {
                { "username", user.Email },
                { "password", user.Password }
            }, action);
        }

        public void UserLogin(string username, string password, Action<RepositoryResponse<UserLoginResponse>> action)
        {
            this.HttpPost<UserLoginResponse>(API.User.Login, new Dictionary<string, string> {
                { "username", username },
                { "password", password }
            }, action);
        }

        public void UserGet(string token, Action<RepositoryResponse<UserGetResponse>> action)
        {
            this.HttpPost<UserGetResponse>(API.User.Get, new Dictionary<string, string> {
                { "token", token }
            }, action);
        }

        public void UserSave(User newUser, Action<RepositoryResponse<UserSaveResponse>> action)
        {
            this.HttpPost<UserSaveResponse>(API.User.Save, new Dictionary<string, string> {
                { "firstName", newUser.FirstName },
                { "surname", newUser.Surname },
                { "email", newUser.Email },
                { "password", newUser.Password }
            }, action);
        }

        public void BoardsList(string token, Action<RepositoryResponse<BoardsListResponse>> action)
        {
            this.HttpPost<BoardsListResponse>(API.Board.List, new Dictionary<string, string> {
                { "token", token }
            }, action);
        }

        public void NotesList(string token, int boardId, Action<RepositoryResponse<NotesListResponse>> action)
        {
            this.HttpPost<NotesListResponse>(API.Note.List, new Dictionary<string, string> {
                { "token", token },
                { "boardID", boardId.ToString() }
            }, action);
        }

        public void NotesSave(string token, Note note, int boardId, Action<RepositoryResponse<NotesSaveResponse>> action)
        {
            this.HttpPost<NotesSaveResponse>(API.Note.Save, new Dictionary<string, string> {
                { "token", token },
                { "title", note.Title },
                { "body", note.Body },
                { "boardID", boardId.ToString() }
            }, action);
        }

        public void BoardsSave(string token, Board board, Action<RepositoryResponse<BoardsSaveResponse>> action)
        {
            this.HttpPost<BoardsSaveResponse>(API.Board.Save, new Dictionary<string, string> {
                { "token", token },
                { "name", board.Name }
            }, action);
        }
    }
}
