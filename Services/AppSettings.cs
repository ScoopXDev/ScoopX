using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.ApplicationModel;

namespace ScoopX.Services
{
    public sealed class AppSettings
    {
        public const string StartupTaskId = "ScoopXStartup";
        private const string KeyCloseToTray = "CloseToTray";
        private const string KeyStartWithWindows = "StartWithWindows";
        private const string KeyStartHiddenToTray = "StartHiddenToTray";
        private const string KeyScoopRootPath = "ScoopRootPath";
        private const string KeyWebsitesRootPath = "WebsitesRootPath";
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "ScoopX";

        private static readonly Lazy<AppSettings> Lazy = new(() => new AppSettings());
        public static AppSettings Instance => Lazy.Value;

        public event EventHandler? Changed;

        private AppSettings() { }

        public bool CloseToTray
        {
            get => LocalSettingsStore.TryGetBool(KeyCloseToTray, out var v) ? v : true;
            set { LocalSettingsStore.SetBool(KeyCloseToTray, value); RaiseChanged(); }
        }

        public bool StartHiddenToTray
        {
            get => LocalSettingsStore.TryGetBool(KeyStartHiddenToTray, out var v) ? v : false;
            set { LocalSettingsStore.SetBool(KeyStartHiddenToTray, value); RaiseChanged(); }
        }

        public string ScoopRootPath
        {
            get => LocalSettingsStore.TryGetString(KeyScoopRootPath, out var v) ? v : "";
            set { LocalSettingsStore.SetString(KeyScoopRootPath, value ?? ""); RaiseChanged(); }
        }

        public string WebsitesRootPath
        {
            get => LocalSettingsStore.TryGetString(KeyWebsitesRootPath, out var v) ? v : "";
            set { LocalSettingsStore.SetString(KeyWebsitesRootPath, value ?? ""); RaiseChanged(); }
        }

        public bool StartWithWindows
        {
            get => LocalSettingsStore.TryGetBool(KeyStartWithWindows, out var v) ? v : false;
            set => _ = SetStartWithWindowsAsync(value);
        }

        public async Task<(bool ok, string? error)> SetStartWithWindowsAsync(bool enabled)
        {
            try
            {
                var task = await StartupTask.GetAsync(StartupTaskId);
                if (enabled)
                {
                    var state = await task.RequestEnableAsync();
                    if (state is StartupTaskState.Disabled
                        or StartupTaskState.DisabledByUser
                        or StartupTaskState.DisabledByPolicy)
                    {
                        LocalSettingsStore.SetBool(KeyStartWithWindows, false);
                        RaiseChanged();
                        return (false, GetStartupString("SettingsStartupDisabled"));
                    }
                }
                else
                {
                    task.Disable();
                }

                LocalSettingsStore.SetBool(KeyStartWithWindows, enabled);
                RaiseChanged();
                return (true, null);
            }
            catch (Exception)
            {
                return SetStartWithWindowsViaRunKey(enabled);
            }
        }

        private (bool ok, string? error) SetStartWithWindowsViaRunKey(bool enabled)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath);
                if (key is null)
                {
                    LocalSettingsStore.SetBool(KeyStartWithWindows, false);
                    RaiseChanged();
                    return (false, GetStartupString("SettingsStartupRegistryOpenFailed"));
                }

                if (enabled)
                {
                    var exe = Environment.ProcessPath
                        ?? Process.GetCurrentProcess().MainModule?.FileName
                        ?? Assembly.GetExecutingAssembly().Location;

                    if (string.IsNullOrWhiteSpace(exe) || !System.IO.File.Exists(exe))
                    {
                        LocalSettingsStore.SetBool(KeyStartWithWindows, false);
                        RaiseChanged();
                        return (false, GetStartupString("SettingsStartupExeMissing"));
                    }

                    key.SetValue(RunValueName, $"\"{exe}\"");
                }
                else
                {
                    key.DeleteValue(RunValueName, throwOnMissingValue: false);
                }

                LocalSettingsStore.SetBool(KeyStartWithWindows, enabled);
                RaiseChanged();
                return (true, null);
            }
            catch (Exception ex)
            {
                LocalSettingsStore.SetBool(KeyStartWithWindows, false);
                RaiseChanged();
                return (false, FormatStartupString("SettingsStartupRegistryFailed", ex.Message));
            }
        }

        private static string GetStartupString(string key) =>
            new ResourceLoader().GetString(key);

        private static string FormatStartupString(string key, string arg0) =>
            string.Format(GetStartupString(key), arg0);

        private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
    }
}
