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
    /// Interaction logic for TransControl.xaml
    /// </summary>
    public partial class TransControl : UserControl
    {
        Transition TheTransition = null;

        public TransControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Transition trans)
            {
                TheTransition = trans;
                TheTransition.PropertyChanged += TheTransition_PropertyChanged;
                if (TheTransition.On && TheTransition.IsSelected)
                {
                    theRectangle.Fill = Brushes.Red;
                }
                else if (TheTransition.On && !TheTransition.IsSelected)
                {
                    theRectangle.Fill = Brushes.Pink;
                }
                else if (!TheTransition.On && TheTransition.IsSelected)
                {
                    theRectangle.Fill = Brushes.Green;
                }
                else if (!TheTransition.On && !TheTransition.IsSelected)
                {
                    theRectangle.Fill = Brushes.LightGreen;
                }
            }
        }

        private void TheTransition_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (TheTransition.On && TheTransition.IsSelected)
            {
                theRectangle.Fill = Brushes.Red;
            }
            else if (TheTransition.On && !TheTransition.IsSelected)
            {
                theRectangle.Fill = Brushes.Pink;
            }
            else if (!TheTransition.On && TheTransition.IsSelected)
            {
                theRectangle.Fill = Brushes.Green;
            }
            else if (!TheTransition.On && !TheTransition.IsSelected)
            {
                theRectangle.Fill = Brushes.LightGreen;
            }
        }

        protected bool isDragging;
        private Point clickPosition;

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            if (e.Source is UserControl draggableControl)
            {
                clickPosition = e.GetPosition(draggableControl);
                draggableControl.CaptureMouse();
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (e.Source is UserControl draggable)
            {
                draggable.ReleaseMouseCapture();
                if (this.RenderTransform is TranslateTransform tt)
                {
                    TheTransition.X = tt.X;
                    TheTransition.Y = tt.Y;
                }
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Source is UserControl draggableControl && isDragging)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        // Package the data.
                        DataObject data = new DataObject();
                        data.SetData(DataFormats.StringFormat, (DataContext as Transition).Ref);
                        data.SetData("Object", (DataContext as Transition));

                        // Inititate the drag-and-drop operation.
                        DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
                    }
                    else
                    {

                        Point currentPosition = e.GetPosition(draggableControl);
                        if (draggableControl.RenderTransform is TranslateTransform transform)
                        {
                            var delta = currentPosition - clickPosition;
                            transform.X += delta.X;
                            transform.Y += delta.Y;
                        }
                        else
                        {
                            transform = new TranslateTransform();
                            draggableControl.RenderTransform = transform;
                            var delta = currentPosition - clickPosition;
                            transform.X += delta.X;
                            transform.Y += delta.Y;
                        }
                    }
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Generate a Property Changed event with DeleteTriggered
            var dummy = false;
            TheTransition.UpdateProperty(ref dummy, true, "DeleteTriggered");
        }

        private void UserControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            // These Effects values are set in the drop target's
            // DragOver event handler.
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }

        bool dropEventMask = false;
        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (dropEventMask) return;
            dropEventMask = true;
            base.OnDrop(e);
            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);
                object dataObject = e.Data.GetData("Object");

                // If the string can be converted into a Brush, 
                // convert it and apply it to the ellipse.
                BrushConverter converter = new BrushConverter();

                if (e.Data.GetData("Object") is Node node)
                {
                    //todo: remove the next line
                    System.Console.WriteLine($"UserControl_Drop called");
                    TheTransition.Connect2Transition(node);

                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }

            }
            e.Handled = true;
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            base.OnDragOver(e);
            dropEventMask = false;
            e.Effects = DragDropEffects.None;

            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                // If the string can be converted into a Brush, allow copying or moving.
                //VALIDATION CODE
                var valid = true;
                if (valid)
                {
                    // Set Effects to notify the drag source what effect
                    // the drag-and-drop operation will have. These values are 
                    // used by the drag source's GiveFeedback event handler.
                    // (Copy if CTRL is pressed; otherwise, move.)
                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }
            e.Handled = true;
        }

    }
}
