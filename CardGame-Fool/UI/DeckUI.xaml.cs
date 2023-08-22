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

    public CardUI Trump
    {
        get => TrumpCard;
        set
        {
            TrumpCard.RankVisual = value.RankVisual;
            TrumpCard.SuitVisual = value.SuitVisual;

            SuitTextBlock.Text = CardUI.SuitSymbols[value.SuitVisual];
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
                TopCard.Visibility = Visibility.Hidden;
                TrumpCard.Visibility = Visibility.Hidden;
            }
            else if (value < 2)
            {
                TopCard.Visibility = Visibility.Hidden;
                TrumpCard.Visibility = Visibility.Visible;

            }
            else if (value > 1)
            {
                TopCard.Visibility = Visibility.Visible;
                TrumpCard.Visibility = Visibility.Visible;
            }
        }
    }
}
