using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CardGameFool.Model.Cards;
using CardGameFool.Model.Players;

namespace CardGameFool.UI;

/// <summary>
/// Логика взаимодействия для PlayerCards.xaml
/// </summary>
public partial class PlayerCards : UserControl
{
    private const int _cardsCountInSlide = Player.StartingCardsCount;
    private const int _slideCount = Deck.MaxCardsCount / _cardsCountInSlide;

    private static readonly int[] _angles = { -65, -40, -15, 15, 40, 65 };
    private static readonly Point _transformOrigin = new(0.5, 1);

    private static readonly Brush? _satusBarBrush = (SolidColorBrush?)new BrushConverter().ConvertFrom("#d43256");

    private readonly CardUI?[] _cardsUI = new CardUI[Deck.MaxCardsCount];

    private bool _isHidden = false;
    private bool _isActive = false;

    private int _currentSlideIndex = 0;

    private MouseButtonEventHandler? _cardsMouseDown;

    public PlayerCards()
    {
        InitializeComponent();
    }

    public event MouseButtonEventHandler CardsMouseDown
    {
        add
        {
            DoToAllCards((cardUI) => cardUI.MouseDown += value);

            _cardsMouseDown += value;
        }
        remove
        {
            DoToAllCards((cardUI) => cardUI.MouseDown -= value);

            _cardsMouseDown -= value;
        }
    }

    public int Count { get; private set; }

    public bool IsHidden
    {
        get => _isHidden;
        set
        {
            _isHidden = value;

            if (value)
            {
                DoToAllCards((cardUI) => cardUI.Side = CardSides.Shirt);
            }
            else
            {
                DoToAllCards((cardUI) => cardUI.Side = CardSides.Face);
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
                    DoToAllCards((cardUI) => cardUI.Cursor = Cursors.Hand);
                }
                
            }
            else
            {
                SatusBar.Fill = null;

                DoToAllCards((cardUI) => cardUI.Cursor = Cursors.Arrow);
            }
        }
    }

    public void Add(CardUI cardUI)
    {
        cardUI.RenderTransformOrigin = _transformOrigin;

        _cardsUI[FindFreeIndex()] = cardUI;
        Count++;

        if (_isHidden)
        {
            cardUI.Side = CardSides.Shirt;
        }
        else
        {
            cardUI.Cursor = Cursors.Hand;

            if (_cardsMouseDown is not null)
            {
                cardUI.MouseDown += _cardsMouseDown;
            }
        }
    }

    public void Remove(CardUI cardUI)
    {
        if (!_cardsUI.Contains(cardUI))
        {
            throw new InvalidOperationException("This card is not on the list.");
        }

        Count--;

        if (_cardsMouseDown is not null)
        {
            cardUI.MouseDown -= _cardsMouseDown;
        }

        WorkspaceCanvas.Children.Remove(cardUI);
        _cardsUI[Array.IndexOf(_cardsUI, cardUI)] = null;
    }

    public void Remove(Card card)
    {
        foreach (CardUI? cardUI in _cardsUI)
        {
            if (cardUI is not null && cardUI.Card.Equals(card))
            {
                Remove(cardUI);

                return;
            }
        }

        throw new InvalidOperationException("There is no match for this card in the array.");
    }

    public void ShowCards(int slideIndex)
    {
        if (slideIndex < 0 || slideIndex > _slideCount)
        {
            throw new ArgumentException(
                $"The entered slide index is less than 0 or exceeds the maximum allowable: {_slideCount}.");
        }

        int startIndex = slideIndex * _slideCount;

        if (_cardsUI[startIndex] is null)
        {
            return;
        }

        _currentSlideIndex = slideIndex;

        WorkspaceCanvas.Children.Clear();

        Array.Sort(_cardsUI);
        // The function Array.Sort() moves all the null elements forward.
        Array.Reverse(_cardsUI);

        for (int i = startIndex; i < (startIndex + _cardsCountInSlide); i++)
        {
            CardUI? cardUI = _cardsUI[i];

            if (cardUI is not null)
            {
                cardUI.Rotate = GetAngleByPosition(i);

                WorkspaceCanvas.Children.Add(cardUI);
            }   
        }
    }

    public void ShowCards()
    {
        int slideIndex = (int)Math.Floor(Count / _cardsCountInSlide - 1d);
        ShowCards(slideIndex > 0 ? slideIndex - 1 : slideIndex);
    }

    private static int GetAngleByPosition(int cardPositionIndex)
    {
        return _angles[cardPositionIndex % _angles.Length];
    }

    private void DoToAllCards(Action<CardUI> action)
    {
        foreach (CardUI? cardUI in _cardsUI)
        {
            if (cardUI is null)
            {
                return;
            }

            action(cardUI);
        }
    }

    private int FindFreeIndex()
    {
        if (_cardsUI.Contains(null))
        {
            return Array.IndexOf(_cardsUI, null);
        }

        throw new InvalidOperationException("There is no free index.");
    }

    private void NextSlideButton_Click(object sender, RoutedEventArgs e)
    {
        if ((_currentSlideIndex + 1) < _slideCount)
        {
            ShowCards(_currentSlideIndex + 1);
        }
    }

    private void PreviousSlideButton_Click(object sender, RoutedEventArgs e)
    {
        if ((_currentSlideIndex - 1) > -1)
        {
            ShowCards(_currentSlideIndex - 1);
        }
    }
}