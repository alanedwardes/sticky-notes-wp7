namespace StickyNotes.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Text;
    using Microsoft.Phone.Net.NetworkInformation;
    using Newtonsoft.Json;

    /// <summary>
    /// The online repository layer, access to all
    /// online services.
    /// </summary>
    public abstract class OnlineRepository : INotifyPropertyChanged
    {
        public class RepositoryResponse<T>
        {
            public HttpStatusCode code;

            public bool WasSuccessful()
            {
                return ((int)code >= 200 && (int)code < 300);
            }

            public T data;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual string DictionaryToQueryString(Dictionary<string, string> dictionary)
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

        public virtual void HttpPost<T>(Uri address, Dictionary<string, string> parameters, Action<RepositoryResponse<T>> action)
        {
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                action.Invoke(new RepositoryResponse<T> { code = HttpStatusCode.RequestTimeout });
                return;
            }

            this.IsLoading = true;

            var data = this.DictionaryToQueryString(parameters);

            var webClient = new HttpWebClient();

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

                if (e.Cancelled || webClient.HttpWebResponse == null)
                {
                    repositoryResponse.code = HttpStatusCode.RequestTimeout;
                }
                else
                {
                    repositoryResponse.data = JsonConvert.DeserializeObject<T>(e.Result, serialiserSettings);
                    repositoryResponse.code = webClient.HttpWebResponse.StatusCode;
                }

                action.Invoke(repositoryResponse);
            };
            webClient.UploadStringAsync(address, data);
            timeoutTimer.Start();
        }
    }
}