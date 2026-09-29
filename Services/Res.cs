using System;
using Microsoft.Windows.ApplicationModel.Resources;

namespace ScoopX.Services
{
    /// <summary>
    /// WASDK ResourceLoader lookup. Resw keys like "Foo.Content" are often stored as "Foo" in PRI.
    /// </summary>
    public static class Res
    {
        private static readonly ResourceLoader Loader = new();

        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            var value = TryGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            var dot = key.LastIndexOf('.');
            if (dot > 0)
            {
                value = TryGet(key[..dot]);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private static string TryGet(string key)
        {
            try
            {
                return Loader.GetString(key) ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
