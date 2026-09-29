### Task 2: resw 璧勬簮

**Files:**
- Create: `Strings/zh-CN/Resources.resw`
- Create: `Strings/en-US/Resources.resw`

**Interfaces:**
- Produces: keys listed below (name 鈫?value). For `x:Uid="Foo"` on TextBlock, resw name is `Foo.Text`.

- [ ] **Step 1: 鍒涘缓涓や晶 Resources.resw**锛堝彲鐢?Visual Studio resw XML 鎴栨墜鍐欙紱鏈€灏戝惈涓嬪垪閿級

`zh-CN` 鏍稿績閿細

| Name | Value |
|------|-------|
| SettingsTitle.Text | 璁剧疆 |
| SettingsAppearanceHeader.Text | 澶栬璁剧疆 |
| SettingsThemeLight.Text | 娴呰壊妯″紡 |
| SettingsThemeDark.Text | 娣辫壊妯″紡 |
| SettingsThemeSystem.Text | 璺熼殢绯荤粺 |
| SettingsLanguageHeader.Text | 璇█璁剧疆 |
| SettingsLanguageZh.Content | 绠€浣撲腑鏂?|
| SettingsLanguageEn.Content | English |
| SettingsGeneralHeader.Text | 甯歌璁剧疆 |
| SettingsCloseToTrayTitle.Text | 鍏抽棴鏃舵渶灏忓寲鍒版墭鐩?|
| SettingsCloseToTrayDesc.Text | 鍏抽棴绐楀彛鏃堕殣钘忓埌绯荤粺鎵樼洏锛岃€屼笉鏄€€鍑?|
| SettingsStartWithWindowsTitle.Text | 寮€鏈烘椂鍚姩 |
| SettingsStartWithWindowsDesc.Text | 鐧诲綍 Windows 鍚庤嚜鍔ㄥ惎鍔?ScoopX |
| SettingsStartHiddenTitle.Text | 鍚姩鏃堕殣钘忓埌鎵樼洏 |
| SettingsStartHiddenDesc.Text | 鍚姩鍚庝笉鏄剧ず涓荤獥鍙ｏ紝浠呮樉绀烘墭鐩樺浘鏍?|
| SettingsPathsHeader.Text | 璺緞璁剧疆 |
| SettingsScoopRootLabel.Text | Scoop 鏍圭洰褰?|
| SettingsScoopRootPlaceholder.PlaceholderText | 閫夋嫨 Scoop 鏍圭洰褰?|
| SettingsWebsitesRootLabel.Text | 缃戠珯榛樿鏍圭洰褰?|
| SettingsWebsitesRootPlaceholder.PlaceholderText | 閫夋嫨缃戠珯榛樿鏍圭洰褰?|
| SettingsBrowse.Content | 娴忚 |
| SettingsAboutHeader.Text | 鍏充簬 |
| SettingsGitHub.Content | GitHub 浠撳簱 |
| SettingsVersionFormat | 鐗堟湰 {0} |
| SettingsLanguageRestartTitle | 鍒囨崲璇█ |
| SettingsLanguageRestartMessage | 璇█灏嗗湪閲嶅惎搴旂敤鍚庣敓鏁堛€傛槸鍚︾珛鍗抽噸鍚紵 |
| SettingsLanguageRestartNow | 绔嬪嵆閲嶅惎 |
| SettingsLanguageLater | 绋嶅悗 |

`en-US` 瀵瑰簲鑻辨枃锛圱itle鈫扴ettings锛孉ppearance鈫扐ppearance锛孋loseToTrayDesc鈫扝ide to the system tray instead of quitting锛岀瓑锛涜瑷€閫夐」鏄剧ず鍚嶄繚鎸併€岀畝浣撲腑鏂囥€?銆孍nglish銆嶏級銆?

resw 鏂囦欢澶翠娇鐢ㄦ爣鍑?ResX schema锛坄resheader` + `data` 鑺傜偣锛夈€傝嫢鎵嬪啓鍥伴毦锛屽彲鐢ㄦ渶灏忓悎娉?ResX 妯℃澘澶嶅埗鍚庡彧鏀?`data`銆?

- [ ] **Step 2: Build锛堢‘璁?PRI 鐢熸垚鏃犳姤閿欙級**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`  
Expected: 0 Error

---


