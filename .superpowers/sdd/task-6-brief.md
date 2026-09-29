### Task 6: Dialogs theme

**Files:**
- Modify: `Pages/AddSiteDialog.xaml`
- Modify: `Pages/EditSiteDialog.xaml`

- [ ] **Step 1: Theme dialog chrome**

Replace local overrides:

```xml
<SolidColorBrush x:Key="ContentDialogBackground" Color="#FFFFFF" />
```

with ThemeDictionaries entries (add keys to App.xaml Light/Dark):

Light: `#FFFFFF`  
Dark: `#2A2A2E`

And in dialogs use:

```xml
<!-- Remove hard-coded white brushes from dialog Resources if they force Light;
     either delete overrides to inherit Fluent, or set:
-->
<StaticResource x:Key="ContentDialogBackground" ResourceKey="ScoopCardFillBrush" />
```

WinUI may not allow StaticResource alias easily for this; simplest: delete the three hard-coded ContentDialog* brushes from dialog Resources so system theme dialog chrome applies, and change title `Foreground="#111827"` / `#1F2937` to `{ThemeResource ScoopTextPrimaryBrush}`.

Keep terminal/black SSH panels in EditSiteDialog as intentional dark panels (`#000000`) 鈥?do not theme those to light.

- [ ] **Step 2: Build + open Add Site dialog in Dark**

Expected: dialog background dark, labels readable.

---

