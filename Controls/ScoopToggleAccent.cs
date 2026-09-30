using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ScoopX.Controls
{
    /// <summary>给 ToggleSwitch 注入主题色资源（ToggleSwitch 密封，不能派生）。</summary>
    public static class ScoopToggleAccent
    {
        private static readonly Color Accent = Color.FromArgb(255, 0x25, 0x63, 0xEB);
        private static readonly Color AccentHover = Color.FromArgb(255, 0x3B, 0x82, 0xF6);
        private static readonly Color AccentPressed = Color.FromArgb(255, 0x1D, 0x4E, 0xD8);
        private static readonly Color AccentDisabled = Color.FromArgb(0x66, 0x25, 0x63, 0xEB);
        private static readonly Color Knob = Colors.White;
        private static readonly Color KnobPressed = Color.FromArgb(255, 0xF3, 0xF4, 0xF6);

        public static readonly DependencyProperty ApplyProperty =
            DependencyProperty.RegisterAttached(
                "Apply",
                typeof(bool),
                typeof(ScoopToggleAccent),
                new PropertyMetadata(false, OnApplyChanged));

        public static bool GetApply(DependencyObject obj) => (bool)obj.GetValue(ApplyProperty);

        public static void SetApply(DependencyObject obj, bool value) => obj.SetValue(ApplyProperty, value);

        private static void OnApplyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ToggleSwitch toggle || e.NewValue is not true)
            {
                return;
            }

            // 普通态用 Brush；PointerOver/Pressed 模板里有 ColorAnimation，需给 Color
            toggle.Resources["ToggleSwitchFillOn"] = new SolidColorBrush(Accent);
            toggle.Resources["ToggleSwitchFillOnPointerOver"] = AccentHover;
            toggle.Resources["ToggleSwitchFillOnPressed"] = AccentPressed;
            toggle.Resources["ToggleSwitchFillOnDisabled"] = new SolidColorBrush(AccentDisabled);

            toggle.Resources["ToggleSwitchStrokeOn"] = new SolidColorBrush(Accent);
            toggle.Resources["ToggleSwitchStrokeOnPointerOver"] = AccentHover;
            toggle.Resources["ToggleSwitchStrokeOnPressed"] = AccentPressed;
            toggle.Resources["ToggleSwitchStrokeOnDisabled"] = new SolidColorBrush(AccentDisabled);

            toggle.Resources["ToggleSwitchKnobFillOn"] = new SolidColorBrush(Knob);
            toggle.Resources["ToggleSwitchKnobFillOnPointerOver"] = Knob;
            toggle.Resources["ToggleSwitchKnobFillOnPressed"] = KnobPressed;
            toggle.Resources["ToggleSwitchKnobFillOnDisabled"] = new SolidColorBrush(Color.FromArgb(255, 0x9C, 0xA3, 0xAF));
        }
    }
}
