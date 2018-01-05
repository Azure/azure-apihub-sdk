namespace Microsoft.Azure.ApiHub.Management.Entities
{
    public class ArmResourceId
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}
