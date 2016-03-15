namespace Microfoft.WindowsAzure.ApiHub.Management.Entities
{
    public class ArmResourceId
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return this.Id;
        }
    }
}
