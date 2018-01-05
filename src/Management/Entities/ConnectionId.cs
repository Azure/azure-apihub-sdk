namespace Microsoft.Azure.ApiHub.Management.Entities
{
    public class ConnectionId : ArmResourceId
    {
        public string ApiName { get; set; }

        public override string ToString()
        {
            return $"{Id}/{ApiName}";
        }
    }
}
