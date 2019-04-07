using RazorLight.Razor;
using System.IO;
using System.Text;

namespace Aiplugs.CMS.Core.RazorLight
{
    public class AiplugsRazorProjectItem : RazorLightProjectItem
    {
        private string _content;

        public AiplugsRazorProjectItem(string key, string content)
        {
            Key = key;
            _content = content;
        }
        public override string Key { get; }

        public override bool Exists => _content != null;

        public override Stream Read()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_content));
        }
    }
}
