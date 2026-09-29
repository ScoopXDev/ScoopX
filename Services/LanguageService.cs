using System;
using Windows.Globalization;
using Windows.Storage;

namespace ScoopX.Services
{
    public sealed class LanguageService
    {
        public const string SettingsKey = "AppLanguage";
        public const string ZhCn = "zh-CN";
        public const string EnUs = "en-US";

        private static readonly Lazy<LanguageService> Lazy = new(() => new LanguageService());
        public static LanguageService Instance => Lazy.Value;

        public string CurrentTag { get; private set; } = ZhCn;

        private LanguageService() { }

        public void ApplyBeforeUi()
        {
            CurrentTag = ReadTag();
            ApplicationLanguages.PrimaryLanguageOverride = CurrentTag;
        }

        public void SetLanguage(string tag)
        {
            var normalized = Normalize(tag);
            CurrentTag = normalized;
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = normalized;
            ApplicationLanguages.PrimaryLanguageOverride = normalized;
        }

        private static string ReadTag()
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(SettingsKey, out var v)
                && v is string s)
            {
                return Normalize(s);
            }

            return ZhCn;
        }

        private static string Normalize(string tag)
        {
            if (string.Equals(tag, EnUs, StringComparison.OrdinalIgnoreCase)
                || string.Equals(tag, "en", StringComparison.OrdinalIgnoreCase))
            {
                return EnUs;
            }

            return ZhCn;
        }
    }
}
