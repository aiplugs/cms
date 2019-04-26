using System;
using System.Collections.Generic;

namespace Aiplugs.CMS.Core.Models
{
    public class User : IUser
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}