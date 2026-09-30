using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace ScoopX.Controls
{
    /// <summary>下拉按钮：指针为手型。</summary>
    public sealed class HandCursorDropDownButton : DropDownButton
    {
        public HandCursorDropDownButton()
        {
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
        }
    }
}
