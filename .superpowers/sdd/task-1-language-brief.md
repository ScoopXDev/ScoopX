### Task 1: LanguageService

**Files:**
- Create: `Services/LanguageService.cs`

**Interfaces:**
- Produces:
  - `const string ZhCn = "zh-CN"`
  - `const string EnUs = "en-US"`
  - `const string SettingsKey = "AppLanguage"`
  - `string CurrentTag { get; }`
  - `void ApplyBeforeUi()` 鈥?璇?LocalSettings锛岄潪娉曞垯 `zh-CN`锛岃 `ApplicationLanguages.PrimaryLanguageOverride`
  - `void SetLanguage(string tag)` 鈥?鏍￠獙鍚庡啓鍏ュ苟璁?Override锛堜笉鑷姩閲嶅惎锛?

- [ ] **Step 1: 鍒涘缓 LanguageService.cs**

```csharp
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
```

- [ ] **Step 2: App.xaml.cs 鈥?鍦?Theme 涔嬪悗銆両nitializeComponent 涔嬪墠璋冪敤**

```csharp
ThemeService.Instance.ApplyApplicationThemeBeforeInit(this);
LanguageService.Instance.ApplyBeforeUi();
InitializeComponent();
```

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`  
Expected: 0 Error

---


