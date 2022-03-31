using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Parlivote.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhitespace(this string str) => string.IsNullOrWhiteSpace(str);
        public static bool HasValue(this string str) => !string.IsNullOrWhiteSpace(str);
    }
}
