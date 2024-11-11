using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ToBinary(this string value)
        {
            var bytes = value.ToByteArray();

            return string.Join(" ", bytes.Select(s => Convert.ToString(s,2).PadLeft(8,'0')));
        }
    }
}
