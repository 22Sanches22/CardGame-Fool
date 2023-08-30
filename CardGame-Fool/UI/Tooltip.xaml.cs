using System.Windows.Controls;
using System.Windows.Media;

namespace CardGameFool.UI;

/// <summary>
/// Логика взаимодействия для Tooltip.xaml
/// </summary>
public partial class Tooltip : UserControl
{
    private static readonly Brush? ActiveBorderBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom("#d43256");
    private static readonly Brush? ActiveTextBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom("#fe9593");

    private static readonly Brush? DisableBorderBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom("#FF611627");
    private static readonly Brush? DisableTextBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom("#FF734444");

    public Tooltip()
    {
        InitializeComponent();
    }

    public void ChangeValue(string text, bool isActive)
    {
        if (isActive)
        {
            Border.BorderBrush = ActiveBorderBrush;
            Text.Foreground = ActiveTextBrush;
        }
        else
        {
            Border.BorderBrush = DisableBorderBrush;
            Text.Foreground = DisableTextBrush;
        }

        Text.Text = text;
    }
}
