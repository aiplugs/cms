using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Aiplugs.CMS.Web.Models
{
  public class ApplicationRole : IdentityRole, Aiplugs.CMS.IRole
  {
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
  }
}
