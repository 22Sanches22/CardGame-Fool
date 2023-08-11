using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CardGameFool.Model.Players;

namespace CardGameFool.UI;

/// <summary>
/// Логика взаимодействия для PlayerCards.xaml
/// </summary>
public partial class PlayerCards : UserControl
{
    private static readonly int[] _angles = { -65, -40, -15, 15, 40, 65 };
    private static readonly Point _transformOrigin = new(0.5, 1);

    private static readonly Brush? _satusBarBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom("#d43256");

    private readonly CardUI?[] _cards = new CardUI[Player.MaxCardsCount];

    private bool _isHidden = false;
    private bool _isActive = false;

    private MouseButtonEventHandler? _cardsMouseDown;

    public PlayerCards()
    {
        InitializeComponent();
    }

    public event MouseButtonEventHandler CardsMouseDown
    {
        add
        {
            DoToAllCards((card) => card.MouseDown += value);

            _cardsMouseDown = value;
        }
        remove
        {
            DoToAllCards((card) => card.MouseDown -= value);

            _cardsMouseDown = null;
        }
    }

    public bool IsHidden
    {
        get => _isHidden;
        set
        {
            _isHidden = value;

            if (value)
            {
                DoToAllCards((card) => card.Side = CardSides.Shirt);
            }
            else
            {
                DoToAllCards((card) => card.Side = CardSides.Face);
            }
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            if (value)
            {
                SatusBar.Fill = _satusBarBrush;
                
                if (!IsHidden)
                {
                    DoToAllCards((card) => card.Cursor = Cursors.Hand);
                }
                
            }
            else
            {
                SatusBar.Fill = null;

                DoToAllCards((card) => card.Cursor = Cursors.Arrow);
            }
        }
    }

    public void AddCard(CardUI card, int positionIndex)
    {
        if (positionIndex < 0 || positionIndex > Player.MaxCardsCount - 1)
        {
            throw new ArgumentException($"The index entered is greater than the maximum available" +
                $"({_cards.Length - 1}) or less than zero.");
        }

        if (_cards[positionIndex] is not null)
        {
            throw new InvalidOperationException("There is already a card in this position.");
        }

        card.Rotate = _angles[positionIndex];
        card.RenderTransformOrigin = _transformOrigin;

        _cards[positionIndex] = card;

        WorkspaceCanvas.Children.Add(card);

        if (_isHidden)
        {
            card.Side = CardSides.Shirt;
        }
        else
        {
            card.Cursor = Cursors.Hand;

            if (_cardsMouseDown is not null)
            {
                card.MouseDown += _cardsMouseDown;
            }         
        }
    }

    public void RemoveCard(int positionIndex)
    {
        if (positionIndex < 0 || positionIndex > _cards.Length - 1)
        {
            throw new ArgumentException($"The entered index is larger than the available one or less than zero.");
        }

        if (_cards[positionIndex] is null)
        {
            throw new InvalidOperationException("There is no card in this position.");
        }

        WorkspaceCanvas.Children.Remove(_cards[positionIndex]);

        _cards[positionIndex] = null;
    }

    public void RemoveCard(CardUI card)
    {
        if (!_cards.Contains(card))
        {
            throw new InvalidOperationException("This card is not on the list.");
        }

        RemoveCard(Array.IndexOf(_cards, card));
    }

    private void DoToAllCards(Action<CardUI> action)
    {
        foreach (CardUI? card in _cards)
        {
            if (card is not null)
            {
                action(card);
            }  
        }
    }
}