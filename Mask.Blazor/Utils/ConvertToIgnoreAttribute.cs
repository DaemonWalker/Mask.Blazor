using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Utils
{
    public class ConvertToIgnoreAttribute : Attribute
    {
        public bool Ignore { get; set; } = true;
    }
}
