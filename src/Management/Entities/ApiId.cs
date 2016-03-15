namespace Microsoft.Azure.ApiHub.Management.Entities
{
    public class ApiId : ArmResourceId
    {
        public override string ToString()
        {
            return string.Format("{0}/{1}", this.Id, this.Name);
        }
    }
}
