### Task 2: Package.appxmanifest StartupTask

**Files:**
- Modify: `Package.appxmanifest`

**Interfaces:**
- Consumes: `AppSettings.StartupTaskId` = `"ScoopXStartup"`

- [ ] **Step 1: 鍦?`Application` 鍐呭鍔?Extensions**

鍦?`</uap:VisualElements>` 涔嬪悗銆乣</Application>` 涔嬪墠鍔犲叆锛堝苟鍦ㄦ牴 `Package` 澧炲姞 `xmlns:uap5` 涓?IgnorableNamespaces锛夛細

鏍瑰厓绱狅細

```xml
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:uap5="http://schemas.microsoft.com/appx/manifest/uap/windows10/5"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  xmlns:systemai="http://schemas.microsoft.com/appx/manifest/systemai/windows10"
  IgnorableNamespaces="uap uap5 rescap systemai">
```

Application 鍐咃細

```xml
      <Extensions>
        <uap5:Extension Category="windows.startupTask">
          <uap5:StartupTask
            TaskId="ScoopXStartup"
            Enabled="false"
            DisplayName="ScoopX" />
        </uap5:Extension>
      </Extensions>
```

- [ ] **Step 2: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 鎴愬姛

---


