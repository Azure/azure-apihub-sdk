using Microsoft.Azure.ApiHub.Common;
using System;
using System.Collections.Specialized;
using Xunit;

namespace Microsoft.Azure.ApiHub.Sdk.Tests.Common
{
    public class CommonTests
    {
        [Fact]
        public void CreateRequestUri()
        {
            const string expectedUri = "https://logic-apis-northeurope.azure-apim.net/apim/sql/00000000000000000000000000000000/datasets('default')/tables('ManifestationView')/items";
            const string actualRuntimeEndpoint = "https://logic-apis-northeurope.azure-apim.net/apim/sql/00000000000000000000000000000000/";
            const string actualAccessTokenScheme = "";
            const string actualAccessToken = "";
            const string acutalTemplate = "datasets('{datasetName}')/tables('{tableName}')/items";

            var actualParameters = new NameValueCollection();
            actualParameters.Add("datasetName", "default");
            actualParameters.Add("tableName", "ManifestationView");

            Uri actualRuntimeEndpointUri = new Uri(actualRuntimeEndpoint);
            var target = new ConnectorHttpClient(actualRuntimeEndpointUri, actualAccessTokenScheme, actualAccessToken);


            Uri result = target.CreateRequestUri(acutalTemplate, actualParameters);

            Assert.NotNull(result);
            Assert.Equal(new Uri(expectedUri), result);
        }
    }
}
