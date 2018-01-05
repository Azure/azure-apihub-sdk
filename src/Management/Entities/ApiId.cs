namespace Microsoft.Azure.ApiHub.Management.Entities
{
    public class ApiId : ArmResourceId
    {
        public override string ToString()
        {
            return $"{Id}/{Name}";
        }
    }
}
