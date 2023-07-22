using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool
{
    internal static class IntExtension
    {
        public static uint ToUint(this int value)
        {
            return value < 0 ? 0 : (uint)value;
        }
    }
}
