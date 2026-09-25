using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace ScoopX.Controls
{
    /// <summary>侧栏导航按钮：指针为手型。</summary>
    public sealed class HandCursorButton : Button
    {
        public HandCursorButton()
        {
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
        }
    }
}
