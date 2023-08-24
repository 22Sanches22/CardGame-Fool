using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
