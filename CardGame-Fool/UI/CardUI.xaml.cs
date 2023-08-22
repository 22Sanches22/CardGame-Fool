using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using CardGameFool.Model.Cards;
using CardGameFool.Model;
using System;
using System.Runtime.ConstrainedExecution;

namespace CardGameFool.UI;

/// <summary>
/// Логика взаимодействия для CardUI.xaml
/// </summary>
public partial class CardUI : UserControl, IComparable
{
    public static readonly Dictionary<Suits, string> SuitSymbols = new()
    {
        [Suits.Spades] = "♠",
        [Suits.Diamonds] = "♦",
        [Suits.Clubs] = "♣",
        [Suits.Hearts] = "♥"
    };

    private readonly Card _card;

    private CardSides _side;

    public CardUI()
    {
        InitializeComponent();

        GenerateShirt();
    }

    public CardUI(Card card) : this()
    {
        _card = card;

        RankVisual = card.Rank;
        SuitVisual = card.Suit;
    }

    public Card Card => _card;

    public Ranks RankVisual
    {
        get => _card.Rank;
        set
        {
            Resources["RankText"] = $"{(value < Ranks.Jack ? ((int)value).ToString() : value.ToString()[0])}";
        }
    }

    public Suits SuitVisual
    {
        get => _card.Suit;
        set
        {
            Resources["SuitText"] = SuitSymbols[value];
        }
    }

    public CardSides Side
    {
        get => _side;
        set
        {
            if (value == CardSides.Face)
            {
                Face.Visibility = Visibility.Visible;
                ShirtCanvas.Visibility = Visibility.Hidden;
            }
            else
            {
                Face.Visibility = Visibility.Hidden;
                ShirtCanvas.Visibility = Visibility.Visible;
            }

            _side = value;
        }
    }

    public double Rotate
    {
        get => WorkspaceRotate.Angle;
        set
        {
            WorkspaceRotate.Angle = value;
        }
    }

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return -1;
        }

        CardUI cardUI = (CardUI)obj;
        return cardUI.Card.CompareTo(_card);
    }

    private void GenerateShirt()
    {
        const int interval = 15;
        const int angle = 90;

        int heightIncrease = (int)Width / 2;

        Color color = (Color)ColorConverter.ConvertFromString("#10133a");//#10133a //#fcdf87 //#fecc50 //#320938
        Brush stroke = new SolidColorBrush(color);

        Effect effect = new DropShadowEffect()
        {
            Color = color,
            Direction = 0,
            ShadowDepth = 0,
            BlurRadius = 10
        };

        CreateLines(interval - angle, 0, 0, Height + heightIncrease);
        CreateLines(interval, angle - interval, Height, -heightIncrease, 1);
        CreateLines(interval, angle - interval, 0, Height + heightIncrease, leftOffset: Width);
        CreateLines(interval - angle, 0, Height, -heightIncrease, 1, Width);


        void CreateLines(int i, int limit, double y1, double y2, double renderPointY = 0, double leftOffset = 0)
        {
            for (int j = i; j < limit; j += interval)
            {
                Line line = new()
                {
                    Y1 = y1,
                    Y2 = y2,
                    Stroke = stroke,
                    RenderTransform = new RotateTransform(j),
                    RenderTransformOrigin = new Point(0, renderPointY),
                    Effect = effect
                };

                if (leftOffset > 0)
                {
                    Canvas.SetLeft(line, leftOffset);
                }

                ShirtCanvas.Children.Add(line);
            }
        }
    }
}