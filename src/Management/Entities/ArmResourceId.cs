namespace Microfoft.Azure.ApiHub.Sdk.Management.Entities
{
    public class ArmResourceId
    {
        public string Id { get; set; }

        public override string ToString()
        {
            return this.Id;
        }
    }
}
