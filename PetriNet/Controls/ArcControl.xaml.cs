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

namespace PetriNet
{
    /// <summary>
    /// Interaction logic for ArcControl.xaml
    /// </summary>
    public partial class ArcControl : UserControl
    {
        const int NODE_WIDTH = 60;
        const int TRANS_WIDTH = 10;
        const int TRANS_HEIGHT = 30;
        private Arc theArc = null;

        public ArcControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (theCanvas.DataContext is Arc anArc)
            {
                theArc = anArc;
                theArc.PropertyChanged += TheArc_PropertyChanged;

                Line1.X1 = theArc.Node.X + NODE_WIDTH / 2;
                Line1.Y1 = theArc.Node.Y + NODE_WIDTH / 2;

                Line1.X2 = theArc.Trans.X + TRANS_WIDTH / 2;
                Line1.Y2 = theArc.Trans.Y + TRANS_HEIGHT / 2;

                var midX = (Line1.X2 + Line1.X1) / 2; ;
                var midY = (Line1.Y2 + Line1.Y1) / 2; ;

                Canvas.SetLeft(TextBlock1, midX);
                Canvas.SetTop(TextBlock1, midY);

                var deltaX = Line1.X2 - Line1.X1;
                var deltaY = Line1.Y2 - Line1.Y1;

                var angle =  Math.Atan2(deltaY, deltaX) * (180.0 / Math.PI);

                System.Console.WriteLine($"{theArc.Ref}: {angle} ({midX}, {midY}) ({Line1.X1},{Line1.Y1}) ({Line1.X2},{Line1.Y2})");

                // middle point

                var trans = new TransformGroup();
                trans.Children.Add(new RotateTransform(angle, 0, 0));
                trans.Children.Add(new TranslateTransform(midX, midY));

                if (theArrow.RenderTransform is TransformGroup tg)
                {
                    if (tg.Children[1] is TranslateTransform tx)
                    {
                        tx.X = midX;
                        tx.Y = midY;
                    }
                    if (tg.Children[0] is RotateTransform rt)
                    {
                        rt.Angle = angle + (theArc.FromNode ? 180 : 0);
                    }
                }
                Line1.Stroke = theArc.IsSelected ? Brushes.Red : Brushes.Black;

            }
        }

        private void TheArc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Line1.Stroke = theArc.IsSelected ? Brushes.Red : Brushes.Black;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dummy = false;
            (DataContext as Arc).UpdateProperty(ref dummy, true, "DeleteTriggered");
        }
    }
}
