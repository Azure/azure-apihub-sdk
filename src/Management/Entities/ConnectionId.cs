namespace Microfoft.Azure.ApiHub.Sdk.Management.Entities
{
    public class ConnectionId : ArmResourceId
    {
        public string ConnectionName { get; set; }

        public string ApiName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", this.Id, this.ApiName, this.ConnectionName);
        }
    }
}
