using System.Windows;
using System.Windows.Controls;

using CardGameFool.Model;

namespace CardGameFool.UI;

/// <summary>
/// Логика взаимодействия для DeckUI.xaml
/// </summary>
public partial class DeckUI : UserControl
{
    private int _cardsCount = Deck.MaxCardsCount;

    public DeckUI()
    {
        InitializeComponent();
    }

    public Ranks TrumpRank
    {
        get => Trump.Rank;
        set
        {
            Trump.Rank = value;
        }
    }

    public Suits TrumpSuit
    {
        get => Trump.Suit;
        set
        {
            Trump.Suit = value;
        }
    }

    public int Count
    { 
        get => _cardsCount;
        set
        {
            _cardsCount = value;

            if (value < 1)
            {
                Workspace.Visibility = Visibility.Hidden;
            }
            else if (value < 2)
            {
                TopCard.Visibility = Visibility.Hidden;
                Workspace.Visibility = Visibility.Visible;
            }
            else if (value > 1)
            {
                TopCard.Visibility = Visibility.Visible;
                Workspace.Visibility = Visibility.Visible;
            }
        }
    }
}
