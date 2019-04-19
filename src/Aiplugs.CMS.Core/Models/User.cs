using System;

namespace Aiplugs.CMS.Core.Models
{
    public class User : IUser
    {
        public string Id => new Guid().ToString();

        public string UserName => "admin";
    }
}