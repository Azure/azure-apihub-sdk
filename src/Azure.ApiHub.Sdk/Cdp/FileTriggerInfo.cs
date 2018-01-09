// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.ApiHub
{
    /// <summary>
    /// Contains information about the current triggered file or the next retry url
    /// </summary>
    public class FileTriggerInfo
    {
        public IFileItem FileItem { get; internal set; }

        public Uri NextUri { get; internal set; }

        public FileWatcherType WatcherType { get; internal set; }

        public TimeSpan RetryAfter { get; internal set; }

        public bool ShouldResetState { get; internal set; }
    }
}
