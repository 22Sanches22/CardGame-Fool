using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Effects;
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

    private CardSides _side;

    public CardUI()
    {
        InitializeComponent();

        GenerateShirt();
    }

    public CardUI(Card card) : this()
    {
        Rank = card.Rank;
        Suit = card.Suit;
    }

    public Ranks Rank
    {
        get => _rank;
        set
        {
            _rank = value;

            Resources["RankText"] = $"{(_rank < Ranks.Jack ? ((int)_rank) : _rank)}"[0].ToString();
        }
    }

    public Suits Suit
    {
        get => _suit;
        set
        {
            _suit = value;

            Resources["SuitText"] = suitSymbols[_suit];
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
                Shirt.Visibility = Visibility.Hidden;
            }
            else
            {
                Face.Visibility = Visibility.Hidden;
                Shirt.Visibility = Visibility.Visible;
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

    private void GenerateShirt()
    {
        const int interval = 15;
        const int angle = 90;

        int heightAllowance = (int)Width / 2;

        Color color = (Color)ColorConverter.ConvertFromString("#10133a");//#10133a //#fcdf87 //#fecc50 //#320938
        Brush stroke = new SolidColorBrush(color);

        Effect effect = new DropShadowEffect()
        {
            Color = color,
            Direction = 0,
            ShadowDepth = 0,
            BlurRadius = 10
        };

        CreateLines(interval - angle, 0, 0, Height + heightAllowance);
        CreateLines(interval, angle - interval, Height, -heightAllowance, 1);
        CreateLines(interval, angle - interval, 0, Height + heightAllowance, leftOffset: Width);
        CreateLines(interval - angle, 0, Height, -heightAllowance, 1, Width);


        void CreateLines(int i, int maxI, double y1, double y2, double renderPointY = 0, double leftOffset = 0)
        {
            for (int j = i; j < maxI; j += interval)
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

                Shirt.Children.Add(line);
            }
            
        }
    }
}
