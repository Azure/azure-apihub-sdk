"get resource group"
armclient get "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/resourceGroups/AzureFunctions?api-version=2016-02-01" -verbose

"list apis"
armclient get "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis?api-version=2015-08-01-preview" -verbose

"get api"
armclient get "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis/dropbox?api-version=2015-08-01-preview" -verbose

"list connections"
armclient get "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis/dropbox/connections?api-version=2015-08-01-preview" -verbose

$connectionId = [System.Guid]::NewGuid().toString()
$connectionId

"create connection"
$connection = @{ 
    Location = "brazilsouth"
    Properties = @{
        api = @{ 
            id = "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/providers/Microsoft.Web/locations/brazilsouth/managedApis/dropbox"
        }
    } 
 }
$payload = ConvertTo-Json $connection
$payload | Out-File .\ManagedApiPayload.json 

armclient PUT "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/resourceGroups/AzureFunctions/providers/Microsoft.Web/connections/$($connectionId)?api-version=2015-08-01-preview" "@ManagedApiPayload.json" -verbose

# armclient DELETE "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/resourceGroups/AzureFunctions/providers/Microsoft.Web/connections/$($connectionId)?api-version=2015-08-01-preview" -verbose

# get consent links
$token = armclient token
$token  = [System.String]::Join("", $token)
$token = $token.Replace("Token copied to clipboard successfully.", "")
$tokenJson = ConvertFrom-Json $token

$consentPayload = @{
    Parameters = @(@{
        parameterName = "token"
        redirectUrl="http://PowerApps.com"
        ObjectId = $tokenJson.oid
        TenantId = $tokenJson.tid
    })
}
$payload = ConvertTo-Json $consentPayload
$payload | Out-File .\ManagedApiPayload.json 

armclient POST "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/resourceGroups/AzureFunctions/providers/Microsoft.Web/connections/$($connectionId)/listConsentLinks?api-version=2015-08-01-preview" "@ManagedApiPayload.json" 
# $linksJson = ConvertTo-Json ([System.String]::Join("", $links))
#navgate to the link to authenticate the connection

"list connection keys"
armclient POST "/subscriptions/83e6374a-dfa5-428b-82ef-eab6c6bdd383/resourceGroups/AzureFunctions/providers/Microsoft.Web/connections/$($connectionId)/listConnectionKeys?api-version=2015-08-01-preview" " " 
 