using System;
using System.ComponentModel.DataAnnotations;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
  public class FileViewModel
  {
    public string Name {get;set;}
    public DateTimeOffset LastModifiedAt {get;set;}
    public string LastModifiedBy {get;set;}
    public long Size {get;set;}
  }
}