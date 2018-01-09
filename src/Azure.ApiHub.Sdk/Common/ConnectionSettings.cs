// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.ApiHub.Common
{
    internal class ConnectionSettings
    {
        public Uri RuntimeEndpoint { get; set; }

        public string AccessTokenScheme { get; set; }

        public string AccessToken { get; set; }

        // TODO: Handle special character escaping.
        public static ConnectionSettings Parse(string connectionString)
        {
            var settings = new ConnectionSettings();

            var parts = connectionString.Split(';');

            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length != 2)
                {
                    break;
                }

                var key = keyValue[0];
                var value = keyValue[1];

                switch (key.ToLowerInvariant())
                {
                    case "endpoint":
                        settings.RuntimeEndpoint = new Uri(value);
                        break;

                    case "scheme":
                        settings.AccessTokenScheme = value;
                        break;

                    case "accesstoken":
                        settings.AccessToken = value;
                        break;
                }
            }

            if (settings.RuntimeEndpoint == null ||
                string.IsNullOrEmpty(settings.AccessTokenScheme) ||
                string.IsNullOrEmpty(settings.AccessToken))
            {
                throw new FormatException("Invalid connectionstring: " + connectionString);
            }

            return settings;
        }
    }
}
