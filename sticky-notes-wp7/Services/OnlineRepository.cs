using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Net;
//using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using StickyNotes.Data;

namespace StickyNotes.Services
{
    public class OnlineRepository
    {
        struct APIMethods
        {
            public struct User
            {
                public const string Login = "user/login";
            }
            public struct Boards
            {
                public const string List = "boards/list";
                public const string Save = "boards/save";
            }
            public struct Notes
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

        public class User
        {
            public int id;
            public string firstName;
            public string surname;
            public string email;
            public string password;
        }

        public class LoginResponse
        {
            public User user;
            public Session session;
        }

        public class NotesListResponse
        {
            public List<Note> notes;
        }

        public class NotesSaveResponse : Note
        {

        }

        public class RepositoryResponse<T>
        {
            public int code;
            public bool WasSuccessful() { return (code >= 200 && code < 300); }
            public T data;
        }

        public class BoardsListResponse
        {
            public List<Board> boards;
        }

        public class BoardsSaveResponse: Board
        {

        }

        const string ENDPOINT = "http://stickyapi.alanedwardes.com/";

        public static string DictionaryToQueryString(Dictionary<string, string> dictionary)
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

        public static void HttpPost<T>(string apiMethod, Dictionary<string, string> parameters, Action<RepositoryResponse<T>> action)
        {
            var data = DictionaryToQueryString(parameters);

            var webClient = new WebClient();
            var timeoutTimer = new System.Windows.Threading.DispatcherTimer();
            timeoutTimer.Interval = new TimeSpan(0, 0, 2);
            timeoutTimer.Tick += new EventHandler((sender, e) => webClient.CancelAsync());

            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadStringCompleted += (object sender, UploadStringCompletedEventArgs e) =>
            {
                timeoutTimer.Stop();

                var repositoryResponse = new RepositoryResponse<T>();

                var serialiserSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCaseToUnderscorePropertyNamesContractResolver()
                };

                if (e.Cancelled)
                {
                    repositoryResponse.code = (int)HttpStatusCode.RequestTimeout;
                }
                else if (e.Error != null)
                {
                    var webException = e.Error as WebException;
                    using (HttpWebResponse response = (HttpWebResponse)webException.Response)
                    {
                        if (response == null)
                        {
                            repositoryResponse.code = (int)HttpStatusCode.RequestTimeout;
                        }
                        else
                        {
                            repositoryResponse.code = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    repositoryResponse.data = JsonConvert.DeserializeObject<T>(e.Result, serialiserSettings);
                    repositoryResponse.code = (int)HttpStatusCode.OK;
                }

                action.Invoke(repositoryResponse);
            };
            webClient.UploadStringAsync(new Uri(ENDPOINT + apiMethod), data);
            timeoutTimer.Start();
        }

        public void UserLogin(string username, string password, Action<RepositoryResponse<LoginResponse>> action)
        {
            var data = new Dictionary<string, string>();
            data.Add("username", username);
            data.Add("password", password);
            HttpPost<LoginResponse>(APIMethods.User.Login, data, action);
        }

        public void BoardsList(string token, Action<RepositoryResponse<BoardsListResponse>> action)
        {
            var data = new Dictionary<string, string>();
            data.Add("token", token);
            HttpPost<BoardsListResponse>(APIMethods.Boards.List, data, action);
        }

        public void NotesList(string token, int boardId, Action<RepositoryResponse<NotesListResponse>> action)
        {
            var data = new Dictionary<string, string>();
            data.Add("token", token);
            data.Add("boardID", boardId.ToString());
            HttpPost<NotesListResponse>(APIMethods.Notes.List, data, action);
        }

        public void NotesSave(string token, Note note, int boardId, Action<RepositoryResponse<NotesSaveResponse>> action)
        {
            var data = new Dictionary<string, string>();
            data.Add("token", token);
            data.Add("title", note.Title);
            data.Add("body", note.Body);
            data.Add("boardID", boardId.ToString());
            HttpPost<NotesSaveResponse>(APIMethods.Notes.Save, data, action);
        }

        public void BoardsSave(string token, Board board, Action<RepositoryResponse<BoardsSaveResponse>> action)
        {
            var data = new Dictionary<string, string>();
            data.Add("token", token);
            data.Add("name", board.Name);
            HttpPost<BoardsSaveResponse>(APIMethods.Boards.Save, data, action);
        }
    }
}
