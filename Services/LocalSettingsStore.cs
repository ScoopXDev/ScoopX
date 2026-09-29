using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Windows.Storage;

namespace ScoopX.Services
{
    /// <summary>
    /// LocalSettings for MSIX (ApplicationData) and unpackaged Setup installs.
    /// Never probe ApplicationData without package identity — it stows 0x80073D54 and
    /// WinUI later fail-fasts with 0xc000027b in Microsoft.UI.Xaml.dll.
    /// </summary>
    internal static class LocalSettingsStore
    {
        private const int AppModelErrorNoPackage = 15700;

        private static readonly object Gate = new();
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
        private static readonly bool HasPackageIdentity = DetectPackageIdentity();
        private static Dictionary<string, JsonElement>? _fileValues;

        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ScoopX",
            "settings.json");

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder? packageFullName);

        private static bool DetectPackageIdentity()
        {
            int length = 0;
            var rc = GetCurrentPackageFullName(ref length, null);
            return rc != AppModelErrorNoPackage && length > 0;
        }

        public static bool TryGetBool(string key, out bool value)
        {
            if (HasPackageIdentity)
            {
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is bool b)
                {
                    value = b;
                    return true;
                }

                value = default;
                return false;
            }

            lock (Gate)
            {
                EnsureFileLoaded();
                if (_fileValues!.TryGetValue(key, out var el) &&
                    (el.ValueKind is JsonValueKind.True or JsonValueKind.False))
                {
                    value = el.GetBoolean();
                    return true;
                }
            }

            value = default;
            return false;
        }

        public static void SetBool(string key, bool value)
        {
            if (HasPackageIdentity)
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
                return;
            }

            lock (Gate)
            {
                EnsureFileLoaded();
                _fileValues![key] = JsonSerializer.SerializeToElement(value);
                SaveFile();
            }
        }

        public static bool TryGetString(string key, out string value)
        {
            if (HasPackageIdentity)
            {
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is string s)
                {
                    value = s;
                    return true;
                }

                value = "";
                return false;
            }

            lock (Gate)
            {
                EnsureFileLoaded();
                if (_fileValues!.TryGetValue(key, out var el) && el.ValueKind == JsonValueKind.String)
                {
                    value = el.GetString() ?? "";
                    return true;
                }
            }

            value = "";
            return false;
        }

        public static void SetString(string key, string value)
        {
            if (HasPackageIdentity)
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
                return;
            }

            lock (Gate)
            {
                EnsureFileLoaded();
                _fileValues![key] = JsonSerializer.SerializeToElement(value ?? "");
                SaveFile();
            }
        }

        private static void EnsureFileLoaded()
        {
            if (_fileValues is not null)
            {
                return;
            }

            _fileValues = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
            try
            {
                if (!File.Exists(FilePath))
                {
                    return;
                }

                var json = File.ReadAllText(FilePath);
                var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (parsed is null)
                {
                    return;
                }

                foreach (var pair in parsed)
                {
                    _fileValues[pair.Key] = pair.Value;
                }
            }
            catch
            {
                // Keep empty in-memory store if file is corrupt/unreadable.
            }
        }

        private static void SaveFile()
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(FilePath, JsonSerializer.Serialize(_fileValues, JsonOptions));
        }
    }
}
