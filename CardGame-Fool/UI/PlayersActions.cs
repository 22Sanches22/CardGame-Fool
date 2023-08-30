using System.Collections.Generic;
using System.Text;
using System.Windows;

using CardGameFool.Model.Players;

namespace CardGameFool.UI;

public static class PlayersActions
{
    public static readonly DependencyProperty Action = DependencyProperty.RegisterAttached(
        nameof(Action),
        typeof(PlayerActions?),
        typeof(PlayersActions),
        new UIPropertyMetadata(OnActionChanged));

    public static List<DependencyObject> Users { get; } = new(4);

    public static PlayerActions GetAction(DependencyObject obj)
    {
        return (PlayerActions)obj.GetValue(Action);
    }

    public static void SetAction(DependencyObject obj, PlayerActions value)
    {
        obj.SetValue(Action, value);
    }

    public static string ToNormalizeString(PlayerActions value)
    {
        StringBuilder action = new(value.ToString());

        for (int i = 1; i < action.Length; i++)
        {
            if (action[i] >= 'A' && action[i] <= 'Z')
            {
                action[i] = (char)('a' > 'A' ? (action[i] + 'a' - 'A') : (action[i] - 'A' - 'a'));
                action.Insert(i, ' ');
            }
        }

        return action.ToString();
    }

    private static void OnActionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        Users.Add(obj);
    }
}