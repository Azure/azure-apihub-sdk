using Microsoft.Azure.ApiHub.Extensions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    internal class Poll : IFileWatcher
    {
        internal int _pollIntervalInSeconds;
        internal CdpHelper _cdpHelper;
        internal Func<IFileItem, Uri, Task> _callback;
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
                int maxRetries = 5;
                int retry = 0;

                HttpResponseMessage response = null;

                // This is just a temp fix to do a retry on transient web errors.
                while (retry < maxRetries)
                {
                    try
                    {
                        // Only the header is required and there is no need to read the entire file content which the connector returns.
                        response = await _cdpHelper.SendAsync(HttpMethod.Get, pollUri, HttpCompletionOption.ResponseHeadersRead);
                    }
                    catch(Exception ex)
                    {
                        _cdpHelper.Logger.Error(string.Format("Retry {0} out of {1} for uri: {2}", retry, maxRetries, pollUri.AbsoluteUri), ex);

                        retry++;

                        if(retry < maxRetries)
                        {
                            retry++;
                            await Task.Delay(200);
                            continue;
                        }
                        else
                        {
                            throw ex;
                        }
                    }

                    if(response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                    {
                        string content = "";
                        if (response.Content != null)
                        {
                            content = await response.Content.ReadAsStringAsync();
                        }

                        _cdpHelper.Logger.Error(string.Format("Returned status code {0}, Retry {1} out of {2} for uri: {3} returned: {4}", response.StatusCode, retry, maxRetries, pollUri.AbsoluteUri, content));

                        retry++;
                        await Task.Delay(200);
                    }
                    else
                    {
                        // we are good, so no more retries.
                        pollUri = response.Headers.Location; // poll next
                        break;
                    }
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string fileId = response.GetHeader("x-ms-file-id");
                    string fileName = response.GetHeader("x-ms-file-name");
                    string fullpath = response.GetHeader("x-ms-file-path"); 
                    string etag = response.GetHeader("x-ms-file-etag");
                    string contentType = response.GetHeader("Content-Type");

                    // Chop off leading 
                    if (fullpath[0] == '/')
                    {
                        fullpath = fullpath.Substring(1);
                    }

                    // Got a new file 
                    var fileItem = new FileItem
                    {
                        _path = fullpath,
                        _handleId = fileId,
                    };
                    
                    this._totalCounted++;
                    this._mostRecentName = fileName;

                    _cdpHelper.Logger.Info(string.Format("file {0} was retrieved for uri: {1}:", fullpath, pollUri.AbsoluteUri));

                     await _callback(fileItem, pollUri);
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
                    var delay = TimeSpan.FromSeconds(_pollIntervalInSeconds * 2);

                    _cdpHelper.Logger.Warning(string.Format("Unexpected status code: {0}, retry after {1} seconds for uri: {2}", response.StatusCode, delay.TotalSeconds, pollUri != null ? pollUri.AbsoluteUri : ""));

                    await Task.Delay(delay);
                }
            }
        }
    }
}
