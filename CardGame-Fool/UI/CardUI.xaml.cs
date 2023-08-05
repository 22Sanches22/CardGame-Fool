using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Shapes;
using CardGameFool.Model;

namespace CardGameFool.UI;

/// <summary>
/// Логика взаимодействия для CardUI.xaml
/// </summary>
public partial class CardUI : UserControl
{
    private static readonly Dictionary<Suits, string> suitSymbols = new()
    {
        [Suits.Spades] = "♠",
        [Suits.Diamonds] = "♦",
        [Suits.Clubs] = "♣",
        [Suits.Hearts] = "♥"
    };

    private Ranks _rank;
    private Suits _suit;

    public CardUI()
    {
        InitializeComponent();
    }

    public CardUI(Ranks rank, Suits suit) : this()
    {
        Rank = rank;
        Suit = suit;
    }

    public Ranks Rank
    {
        set
        {
            _rank = value;

            Resources["RankText"] = $"{(_rank < Ranks.Jack ? ((int)_rank) : _rank)}"[0].ToString();
        }
    }

    public Suits Suit
    {
        set
        {
            _suit = value;

            Resources["SuitText"] = suitSymbols[_suit];
        }
    }

    public CardSides Side
    {
        set
        {
            Face.Visibility = (Visibility)value;
            Shirt.Visibility = value == CardSides.Shirt ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
