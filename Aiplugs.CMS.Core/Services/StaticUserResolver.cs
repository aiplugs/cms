namespace Aiplugs.CMS.Core.Services
{
    public class StaticUserResolver : IUserResolver
    {
        private string _id;
        public StaticUserResolver(string userId)
        {
            _id = userId;
        }

        public string GetUserId()
        {
            return _id;
        }
    }
}