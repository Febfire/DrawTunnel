using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Model
{
    /// <summary>
    /// 用于用户自定义属性的model
    /// </summary>
    public class TagData : object
    {
        public string Item { get; set; }

        public string Value { get; set; }

        public TagData(string text, string value)
        {
            this.Item = text;
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
