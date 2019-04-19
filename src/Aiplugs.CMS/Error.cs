using System;

namespace Aiplugs.CMS
{
  public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public Error(string code)
            : this(code, null)
        { }

        public Error(string code, string desc)
        {
            Code = code;
            Description = desc;
        }


        public override bool Equals(Object obj)
        {
            var other = obj as Error;
            if (other == null)
                return false;

            return this.Code == other.Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{Code}] {Description??string.Empty}";
        }
    }
}