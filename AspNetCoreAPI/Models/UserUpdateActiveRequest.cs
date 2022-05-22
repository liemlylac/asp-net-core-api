namespace ASPNetCoreAPI.Models
{
    public class UserUpdateActiveRequest
    {
        public long[] Ids { get; set; }
        public bool Active { get; set; }
    }
}