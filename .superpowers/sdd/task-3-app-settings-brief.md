### Task 3: MainWindow / App 鎺ョ嚎鍏抽棴涓庡惎鍔ㄩ殣钘?

**Files:**
- Modify: `MainWindow.xaml.cs`
- Modify: `App.xaml.cs`锛堜粎褰撳惎鍔ㄩ殣钘忔斁鍦?OnLaunched 鏇存竻鏅版椂锛?

**Interfaces:**
- Consumes: `AppSettings.Instance.CloseToTray`, `StartHiddenToTray`

- [ ] **Step 1: 鏀瑰叧闂€昏緫**

`AppWindow_Closing`锛?

```csharp
private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
{
    if (_isExitRequested)
    {
        return;
    }

    if (AppSettings.Instance.CloseToTray)
    {
        args.Cancel = true;
        HideToTray();
        return;
    }

    // 鍏佽鍏抽棴锛氬厛娓呯悊鎵樼洏
    TrayIcon.Dispose();
}
```

`CloseButton_Click`锛?

```csharp
private void CloseButton_Click(object sender, RoutedEventArgs e)
{
    if (AppSettings.Instance.CloseToTray)
    {
        HideToTray();
    }
    else
    {
        ExitApplication();
    }
}
```

- [ ] **Step 2: 鍚姩闅愯棌**

鍦?`MainWindow` 鏋勯€犲嚱鏁版湯灏撅紙`NavigateTo("Home")` 涔嬪悗锛夛細

```csharp
if (AppSettings.Instance.StartHiddenToTray)
{
    // 寤惰繜鍒扮獥鍙ｆ縺娲诲悗鍐嶈棌锛岄伩鍏嶉棯涓€涓嬪張绔嬪埢 Hide 绔炴€侊細鐢?Activated 涓€娆℃€?
    void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        Activated -= OnActivated;
        HideToTray();
    }
    Activated += OnActivated;
}
```

锛堣嫢 `Activated` 鍦ㄥ凡婵€娲诲悗涓嶅啀瑙﹀彂锛屾敼涓?`DispatcherQueue.TryEnqueue` 鍦?Loaded/棣栧抚 Hide銆傦級

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 鎴愬姛

---


