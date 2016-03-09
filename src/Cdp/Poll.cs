using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microfoft.Azure.ApiHub.Sdk.Cdp
{
    public class Poll : IFileWatcher, IDisposable
    {
        internal int _pollIntervalInSeconds;
        internal CdpHelper _cdpHelper;
        internal Func<CdpItemInfo, Task> _callback;
        internal Task _runTask;

        private CancellationTokenSource _cancel = new CancellationTokenSource();

        int _totalCounted;
        string _mostRecentName;

        public override string ToString()
        {
            return string.Format("{0} new files; {1}", _totalCounted, _mostRecentName);
        }

        // Call to stop the poller. 
        // task completes when the poll has stopped
        public async Task StopAsync()
        {
            _cancel.Cancel();

            // Wait for the run loop to Exit.
            await _runTask;
        }

        public async Task Run(Uri pollUri)
        {
            while (!_cancel.IsCancellationRequested)
            {
                // Make call 
                HttpResponseMessage response = await _cdpHelper.SendAsync(HttpMethod.Get, pollUri);

                pollUri = response.Headers.Location; // poll next

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string fileId = GetHeader(response, "x-ms-file-id");
                    string fileName = GetHeader(response, "x-ms-file-name");
                    string fullpath = GetHeader(response, "x-ms-file-path"); 
                    string etag = GetHeader(response, "x-ms-file-etag");
                    string contentType = GetHeader(response, "Content-Type");

                    // Chop off leading 
                    if (fullpath[0] == '/')
                    {
                        fullpath = fullpath.Substring(1);
                    }

                    // Got a new file 
                    var item = new CdpItemInfo
                    {
                        Id = fileId,
                        Name = fileName,
                        Path = fullpath,
                        Etag = etag,
                        MediaType = contentType                        
                    };

                    this._totalCounted++;
                    this._mostRecentName = fileName;

                    await _callback(item);
                    // CDP only dispatches one at a time, so poll immediately to see if there's more.                         
                }
                else if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    var rt = response.Headers.RetryAfter;
                    TimeSpan delay;
                    if (rt.Delta.HasValue)
                    {
                        delay = rt.Delta.Value;
                    }
                    else {
                        delay = TimeSpan.FromSeconds(_pollIntervalInSeconds);
                    }

                    await Task.Delay(delay);
                }
                else {
                    // Stop polling. 
                    return;
                }
            }
        }

        static string GetHeader(HttpResponseMessage response, string name)
        {
            IEnumerable<string> x;
            response.Headers.TryGetValues(name, out x);
            if (x != null && x.Any())
            {
                return x.First();
            }
            return null;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(_cancel != null)
                {
                    _cancel.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
