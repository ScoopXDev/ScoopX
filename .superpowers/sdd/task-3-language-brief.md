### Task 3: SettingsPage UI 鈥?璇█鍖?+ x:Uid

**Files:**
- Modify: `Pages/SettingsPage.xaml`
- Modify: `Pages/SettingsPage.xaml.cs`

**Interfaces:**
- Consumes: `LanguageService.Instance`銆乣ResourceLoader`锛堢増鏈牸寮忎笌瀵硅瘽妗嗭級

- [ ] **Step 1: 澶栬鍗＄墖鍚庢彃鍏ヨ瑷€璁剧疆**

```xml
<!-- 璇█璁剧疆 -->
<TextBlock x:Uid="SettingsLanguageHeader" FontSize="13" FontWeight="SemiBold"
           Foreground="{ThemeResource ScoopTextSecondaryBrush}" Margin="0,8,0,0" />
<Border Background="{ThemeResource ScoopSurfaceFillBrush}" BorderThickness="0"
        CornerRadius="10" Padding="16,12">
  <StackPanel Orientation="Horizontal" Spacing="24">
    <RadioButton x:Name="LanguageZhRadio" x:Uid="SettingsLanguageZh"
                 GroupName="AppLanguage" Tag="zh-CN"
                 Checked="LanguageRadio_Checked" />
    <RadioButton x:Name="LanguageEnRadio" x:Uid="SettingsLanguageEn"
                 GroupName="AppLanguage" Tag="en-US"
                 Checked="LanguageRadio_Checked" />
  </StackPanel>
</Border>
```

- [ ] **Step 2: 璁剧疆椤靛叾浣欏彲瑙佷腑鏂囨敼 x:Uid**

瀵规爣棰樸€佷富棰樹笁鏍囩銆佸父瑙勪笁琛屻€佽矾寰勬爣绛?鍗犱綅/娴忚銆佸叧浜庢爣棰樸€丟itHub 鎸夐挳璁剧疆瀵瑰簲 `x:Uid`锛?*鍘绘帀**纭紪鐮?`Text`/`Content`/`PlaceholderText`锛堢敱 resw 娉ㄥ叆锛夈€備富棰橀瑙堝浘鍐呴儴瑁呴グ鏂囨鍙繚鎸佺‖缂栫爜锛堥潪鐢ㄦ埛璇存槑鏂囧瓧锛夈€?

椤甸潰澶ф爣棰橈細`x:Uid="SettingsTitle"`銆?

- [ ] **Step 3: code-behind**

鍔犺浇鏃讹細

```csharp
_suppressLanguage = true;
try {
  if (LanguageService.Instance.CurrentTag == LanguageService.EnUs)
    LanguageEnRadio.IsChecked = true;
  else
    LanguageZhRadio.IsChecked = true;
} finally { _suppressLanguage = false; }
```

`LanguageRadio_Checked`锛氳嫢 suppress 鎴?Tag 涓?CurrentTag 鐩稿悓鍒?return锛涘惁鍒?`SetLanguage(tag)`锛岀劧鍚庡脊 ContentDialog锛堣祫婧愪覆锛夛紝Primary=绔嬪嵆閲嶅惎锛?

```csharp
var path = Environment.ProcessPath;
if (!string.IsNullOrEmpty(path))
{
  System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
  {
    FileName = path,
    UseShellExecute = true
  });
}
if (App.MainWindow is MainWindow mw)
  // 璋冪敤鐜版湁閫€鍑猴細鍙€氳繃鍏紑鏂规硶鎴?Window.Close + tray dispose
```

鑻?`MainWindow.ExitApplication` 涓?private锛氬湪 `MainWindow` 澧炲姞 `public void RequestExit()` 鍖呰鐜版湁 `ExitApplication()`锛岃缃〉璋冪敤涔嬶紱鎴?`Application.Current.Exit()` + 鍏?Dispose 鎵樼洏锛堜紭鍏?RequestExit 浠ユ竻鐞嗘墭鐩橈級銆?

鐗堟湰锛?

```csharp
var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
var fmt = loader.GetString("SettingsVersionFormat");
AboutVersionText.Text = string.Format(fmt, versionDigits);
```

瀵硅瘽妗嗘枃妗堝悓鏍?`ResourceLoader.GetString(...)`銆?

- [ ] **Step 4: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`  
Expected: 0 Error

- [ ] **Step 5: 鎵嬪姩楠屾敹**

1. 榛樿涓枃璁剧疆椤? 
2. 閫?English 鈫?绔嬪嵆閲嶅惎 鈫?璁剧疆椤佃嫳鏂? 
3. 閫夌◢鍚?鈫?涓嬫鍚姩鎵嶅彉  
4. 鍏跺畠椤典粛鍙负涓枃  

---


