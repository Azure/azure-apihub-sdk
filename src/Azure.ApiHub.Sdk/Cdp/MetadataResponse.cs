// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.ApiHub
{
    internal class MetadataResponse
    {
        /// <summary>
        /// Blob Metadata Info
        /// </summary>
        public class BlobMetadataInfo
        {
            public string source { get; set; }

            public string displayName { get; set; }

            public string urlEncoding { get; set; }
        }

        /// <summary>
        /// The BLOB
        /// if != null, this supports blob operations
        /// </summary>
        public BlobMetadataInfo blob;

        /// <summary>
        /// Gets a value indicating whether this instance has file support.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has file support; otherwise, <c>false</c>.
        /// </value>
        public bool HasFileSupport
        {
            get { return blob != null; }
        }
    }
}
