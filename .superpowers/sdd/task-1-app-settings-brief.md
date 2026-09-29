### Task 1: AppSettings 鏈嶅姟

**Files:**
- Create: `Services/AppSettings.cs`

**Interfaces:**
- Produces: `AppSettings.Instance` with:
  - `bool CloseToTray { get; set; }` 榛樿 `true`
  - `bool StartWithWindows { get; set; }` 榛樿 `false`锛泂et 鏃跺皾璇曞悓姝?StartupTask
  - `bool StartHiddenToTray { get; set; }` 榛樿 `false`
  - `string ScoopRootPath { get; set; }` 榛樿 `""`
  - `string WebsitesRootPath { get; set; }` 榛樿 `""`
  - `event EventHandler? Changed`
  - `Task SetStartWithWindowsAsync(bool enabled)`锛堜緵 UI await锛涘睘鎬?set 鍙?fire-and-forget 璋冪敤锛?
  - `const string StartupTaskId = "ScoopXStartup"`

- [ ] **Step 1: 鍒涘缓 `Services/AppSettings.cs`**

```csharp
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

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

        private static readonly Lazy<AppSettings> Lazy = new(() => new AppSettings());
        public static AppSettings Instance => Lazy.Value;

        public event EventHandler? Changed;

        private AppSettings() { }

        public bool CloseToTray
        {
            get => ReadBool(KeyCloseToTray, defaultValue: true);
            set { WriteBool(KeyCloseToTray, value); RaiseChanged(); }
        }

        public bool StartHiddenToTray
        {
            get => ReadBool(KeyStartHiddenToTray, defaultValue: false);
            set { WriteBool(KeyStartHiddenToTray, value); RaiseChanged(); }
        }

        public string ScoopRootPath
        {
            get => ReadString(KeyScoopRootPath);
            set { WriteString(KeyScoopRootPath, value ?? ""); RaiseChanged(); }
        }

        public string WebsitesRootPath
        {
            get => ReadString(KeyWebsitesRootPath);
            set { WriteString(KeyWebsitesRootPath, value ?? ""); RaiseChanged(); }
        }

        public bool StartWithWindows
        {
            get => ReadBool(KeyStartWithWindows, defaultValue: false);
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
                        WriteBool(KeyStartWithWindows, false);
                        RaiseChanged();
                        return (false, "鏈兘鍚敤寮€鏈鸿嚜鍚紙鍙兘琚郴缁熸垨鐢ㄦ埛绛栫暐绂佺敤锛夈€?);
                    }
                }
                else
                {
                    task.Disable();
                }

                WriteBool(KeyStartWithWindows, enabled);
                RaiseChanged();
                return (true, null);
            }
            catch (Exception ex)
            {
                // unpackaged / missing extension
                WriteBool(KeyStartWithWindows, false);
                RaiseChanged();
                return (false, $"寮€鏈鸿嚜鍚笉鍙敤锛堥渶瀹夎鍖呯増鏈級锛歿ex.Message}");
            }
        }

        private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

        private static bool ReadBool(string key, bool defaultValue)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is bool b)
                return b;
            return defaultValue;
        }

        private static void WriteBool(string key, bool value) =>
            ApplicationData.Current.LocalSettings.Values[key] = value;

        private static string ReadString(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is string s)
                return s;
            return "";
        }

        private static void WriteString(string key, string value) =>
            ApplicationData.Current.LocalSettings.Values[key] = value;
    }
}
```

- [ ] **Step 2: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 鎴愬姛锛? Error锛?

---


