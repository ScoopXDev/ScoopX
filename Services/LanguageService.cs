using System;
using System.Diagnostics;
using System.Globalization;
using WasdkLanguages = Microsoft.Windows.Globalization.ApplicationLanguages;

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
            TrySetPrimaryLanguageOverride(CurrentTag);
        }

        /// <summary>
        /// Persists language preference. Override may only take effect after restart
        /// once MRT has already loaded (typical for unpackaged).
        /// </summary>
        public bool SetLanguage(string tag)
        {
            try
            {
                var normalized = Normalize(tag);
                CurrentTag = normalized;
                LocalSettingsStore.SetString(SettingsKey, normalized);
                TrySetPrimaryLanguageOverride(normalized);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ScoopX: SetLanguage failed: {ex.Message}");
                return false;
            }
        }

        private static bool TrySetPrimaryLanguageOverride(string tag)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(tag);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ScoopX: CultureInfo failed for '{tag}': {ex.Message}");
            }

            try
            {
                // 未打包应用必须用 WASDK 的 Globalization API；
                // Windows.Globalization.ApplicationLanguages 需要包标识，会抛 0x80073D54。
                WasdkLanguages.PrimaryLanguageOverride = tag;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ScoopX: PrimaryLanguageOverride failed for '{tag}': {ex.Message}");
                return false;
            }
        }

        private static string ReadTag()
        {
            if (LocalSettingsStore.TryGetString(SettingsKey, out var s))
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

