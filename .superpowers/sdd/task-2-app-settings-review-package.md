diff --git a/Package.appxmanifest b/Package.appxmanifest
index 74a6a66..277cfd6 100644
--- a/Package.appxmanifest
+++ b/Package.appxmanifest
@@ -4,9 +4,10 @@
   xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
   xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
   xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
+  xmlns:uap5="http://schemas.microsoft.com/appx/manifest/uap/windows10/5"
   xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
   xmlns:systemai="http://schemas.microsoft.com/appx/manifest/systemai/windows10"
-  IgnorableNamespaces="uap rescap systemai">
+  IgnorableNamespaces="uap uap5 rescap systemai">
 
   <Identity
     Name="34114be7-3b1f-4822-bcf4-971a979670c2"
@@ -43,6 +44,14 @@
         <uap:DefaultTile Wide310x150Logo="Assets\Wide310x150Logo.png" />
         <uap:SplashScreen Image="Assets\SplashScreen.png" />
       </uap:VisualElements>
+      <Extensions>
+        <uap5:Extension Category="windows.startupTask">
+          <uap5:StartupTask
+            TaskId="ScoopXStartup"
+            Enabled="false"
+            DisplayName="ScoopX" />
+        </uap5:Extension>
+      </Extensions>
     </Application>
   </Applications>
 
