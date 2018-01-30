using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Aiplugs.CMS.Web.ViewModels
{
  public class CollectionViewModel
  {
    public string CollectionName { get; set; }
    public IEnumerable<IItem> Items { get; set; }
    public string Prev { get; set; }
    public string Next { get; set; }
  }
}