using System.Windows;
using System.Windows.Controls;
using CardGameFool.Model.Cards;

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

    public CardUI TrumpCard
    {
        get => TrumpCardUI;
        set
        {
            TrumpCardUI.Rank = value.Rank;
            TrumpCardUI.Suit = value.Suit;
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
                return;
            }
            else if (value < 2)
            {
                TopCard.Visibility = Visibility.Hidden;
                
            }
            else if (value > 1)
            {
                TopCard.Visibility = Visibility.Visible;
            }

            Workspace.Visibility = Visibility.Visible;
        }
    }
}
