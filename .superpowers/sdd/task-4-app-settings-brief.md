### Task 4: SettingsPage UI + 鎺ョ嚎

**Files:**
- Modify: `Pages/SettingsPage.xaml`
- Modify: `Pages/SettingsPage.xaml.cs`

**Interfaces:**
- Consumes: `AppSettings.Instance` 鍏ㄩ儴灞炴€т笌 `SetStartWithWindowsAsync`
- Consumes: 鐜版湁涓婚鍖轰笉鍙?

- [ ] **Step 1: XAML 鈥?鍦ㄥ瑙傚崱鐗囧悗杩藉姞鍒嗗尯**

缁撴瀯锛堟斁鍦ㄧ幇鏈夊瑙?`Border` 涔嬪悗銆佸灞?`StackPanel` 鍐咃級锛沗MaxWidth` 鏀逛负 `640`锛?

```xml
<!-- 甯歌 -->
<TextBlock Text="甯歌" FontSize="13" FontWeight="SemiBold"
           Foreground="{ThemeResource ScoopTextSecondaryBrush}" Margin="0,8,0,0" />
<Border Background="{ThemeResource ScoopSurfaceFillBrush}" BorderThickness="0"
        CornerRadius="10" Padding="16,8">
  <StackPanel>
    <!-- 姣忚锛欸rid 宸︽爣棰?璇存槑锛屽彸 ToggleSwitch锛涗笁琛岋細CloseToTray / StartWithWindows / StartHiddenToTray -->
  </StackPanel>
</Border>

<!-- 璺緞 -->
<TextBlock Text="璺緞" ... />
<Border ... Padding="16,14">
  <!-- Scoop 鏍圭洰褰曪細TextBox + 娴忚鎸夐挳 -->
  <!-- 缃戠珯榛樿鏍圭洰褰曪細鍚屼笂 -->
</Border>

<!-- 鍏充簬 -->
<TextBlock Text="鍏充簬" ... />
<Border ... Padding="16,14">
  <StackPanel Spacing="6">
    <TextBlock Text="ScoopX" FontSize="15" FontWeight="SemiBold"
               Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
    <TextBlock x:Name="AboutVersionText" FontSize="12"
               Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
  </StackPanel>
</Border>
```

Toggle 鍛藉悕锛歚CloseToTrayToggle`銆乣StartWithWindowsToggle`銆乣StartHiddenToggle`  
璺緞锛歚ScoopRootTextBox`銆乣WebsitesRootTextBox`銆乣BrowseScoopRootButton`銆乣BrowseWebsitesRootButton`

琛屽唴鏂囨锛?
- 鍏抽棴鏃舵渶灏忓寲鍒版墭鐩?/ 鍏抽棴绐楀彛鏃堕殣钘忓埌绯荤粺鎵樼洏锛岃€屼笉鏄€€鍑?
- 寮€鏈烘椂鍚姩 / 鐧诲綍 Windows 鍚庤嚜鍔ㄥ惎鍔?ScoopX
- 鍚姩鏃堕殣钘忓埌鎵樼洏 / 鍚姩鍚庝笉鏄剧ず涓荤獥鍙ｏ紝浠呮樉绀烘墭鐩樺浘鏍?

- [ ] **Step 2: code-behind 鎺ョ嚎**

鏋勯€犲嚱鏁帮細鍔犺浇寮€鍏充笌璺緞锛涚粦瀹?`Toggled` / `Click`锛涘啓 `AboutVersionText`锛?

```csharp
private static string GetVersionString()
{
    try
    {
        var v = Package.Current.Id.Version;
        return $"鐗堟湰 {v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
    }
    catch
    {
        var v = typeof(App).Assembly.GetName().Version;
        return v is null ? "鐗堟湰 1.0.0" : $"鐗堟湰 {v.Major}.{v.Minor}.{v.Build}";
    }
}
```

`StartWithWindowsToggle`锛歚await AppSettings.Instance.SetStartWithWindowsAsync(...)`锛涘け璐ユ椂鐢?`ContentDialog` 鎻愮ず锛屽苟鎶?Toggle 鎷ㄥ洖 `AppSettings` 瀹為檯鍊笺€?

璺緞娴忚锛氬鐢?EditSiteDialog 鐨?`FolderPicker` + `InitializeWithWindow` 妯″紡銆?

娉ㄦ剰锛氬姞杞芥椂璁?Toggle 浼氳Е鍙?`Toggled`鈥斺€旂敤 `_suppressToggle` 鏍囧織璺宠繃銆?

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 鎴愬姛

- [ ] **Step 4: 鎵嬪姩楠屾敹娓呭崟**

1. 鍏炽€屽叧闂埌鎵樼洏銆嶁啋 鐐?X 搴旈€€鍑鸿繘绋? 
2. 寮€銆屽叧闂埌鎵樼洏銆嶁啋 鐐?X 杩涙墭鐩橈紝鎵樼洏銆岄€€鍑恒€嶅彲閫€鍑? 
3. 璺緞娴忚淇濆瓨鍚庨噸鍚粛鍦? 
4. 鍏充簬鏄剧ず鐗堟湰  
5. 娴呰壊/娣辫壊涓嬫柊鍒嗗尯鍙  

---


