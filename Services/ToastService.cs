using System;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace ScoopX.Services
{
    public static class ToastService
    {
        private static bool _registered;

        public static void Initialize()
        {
            if (_registered)
            {
                return;
            }

            try
            {
                AppNotificationManager.Default.Register();
                _registered = true;
            }
            catch (Exception)
            {
                // Unpackaged 首次注册失败时仍可尝试 Show；下次启动再注册。
            }
        }

        public static void Show(string title, string message)
        {
            try
            {
                Initialize();

                var notification = new AppNotificationBuilder()
                    .AddText(title ?? string.Empty)
                    .AddText(message ?? string.Empty)
                    .BuildNotification();

                AppNotificationManager.Default.Show(notification);
            }
            catch (Exception)
            {
                // 通知失败不应影响主流程
            }
        }
    }
}
