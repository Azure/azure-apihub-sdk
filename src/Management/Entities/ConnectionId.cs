namespace Microfoft.Azure.ApiHub.Sdk.Management.Entities
{
    public class ConnectionId : ArmResourceId
    {
        public string ApiName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}/{1}", this.Id, this.ApiName);
        }
    }
}
