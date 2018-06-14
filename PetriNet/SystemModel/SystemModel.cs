using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace PetriNet
{
    // todo: add file prompts for load
    // todo: add file prompts for save

    public partial class SystemModel : INotifyPropertyChanged
    {
        [XmlArray("Items")]
        [XmlArrayItem("Node", typeof(Node))]
        [XmlArrayItem("Transition", typeof(Transition))]
        [XmlArrayItem("Arc", typeof(Arc))]

        ObservableCollection<Displayable> items = new ObservableCollection<Displayable>();
        public ObservableCollection<Displayable> Items { get => items; set => UpdateProperty(ref items, value); }
        
        Displayable selectedItem = null;
        public Displayable SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value != selectedItem)
                {
                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = false;
                    }
                    UpdateProperty(ref selectedItem, value);
                    if (selectedItem != null)
                    {
                        selectedItem.IsSelected = true;
                    }
                }
            }
        }
        public ListView TheListView { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty<T>(ref T item, T value, [CallerMemberName] string propertyName = "")
        {
            item = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SystemModel()
        {
            TheRefreshCommand = new RelayCommand<SystemModel>(Refresh);
            TheAddNodeCommand = new RelayCommand<SystemModel>(AddNode);
            TheAddTransitionCommand = new RelayCommand<SystemModel>(AddTransition);
            TheLoadFileCommand = new RelayCommand<SystemModel>(LoadFile);
            TheSaveFileCommand = new RelayCommand<SystemModel>(SaveFile);

            var n1 = new Node() { Ref = "N1", X = 10, Y = 50, TokenCount = 1 };
            var n2 = new Node() { Ref = "N2", X = 150, Y = 50, TokenCount = 2, };
            var t1 = new Transition() { Ref = "T1", X = 100, Y = 10 };
            var t2 = new Transition() { Ref = "T2", X = 100, Y = 100, };
            var a1 = new Arc { Ref = "a1", Node = n1, Trans = t1, FromNode = true };
            var a2 = new Arc { Ref = "a2", Node = n1, Trans = t2, FromNode = false };
            var a3 = new Arc { Ref = "a3", Node = n2, Trans = t1, FromNode = false };
            var a4 = new Arc { Ref = "a4", Node = n2, Trans = t2, FromNode = true };

            Items = new ObservableCollection<Displayable> {
                a1, a2, a3, a4, n1, n2, t1, t2,
            };

            foreach (var arc in Items.OfType<Arc>())
            {
                arc.Trans.Arcs.Add(arc);
            }

            MakeConnections();
            HookUpEvents();

            SelectedItem = n1;

            PropertyChanged += SystemModel_PropertyChanged;

        }

        private void SystemModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "SelectedItem":
                    //SelectedItem.IsSelected = true;
                    break;
            }
        }

        private void HookUpEvents()
        {
            foreach (var displayable in Items)
            {
                switch (displayable)
                {
                    case Transition trans:
                        trans.DoubleClick = dbClick;
                        trans.PropertyChanged += NodeOrTrans_PropertyChanged;
                        trans.Node2Transition += ConnectNode2Trans;
                        break;

                    case Node node:
                        node.PropertyChanged += NodeOrTrans_PropertyChanged;
                        node.Transition2Node += ConnectNode2Trans;
                        break;

                    case Arc arc:
                        arc.PropertyChanged += NodeOrTrans_PropertyChanged;
                        break;
                }
            }
        }


        public void MakeConnections()
        {
            foreach(var trans in Items.OfType<Transition>())
            {
                trans.Arcs.Clear();
                trans.Arcs.AddRange(Items.OfType<Arc>().Where(a => a.Trans == trans));
                trans.ConnectedTransitions.Clear();
            }

            var fromArcs = Items.OfType<Arc>().Where(a => a.FromNode == true);
            var toArcs = Items.OfType<Arc>().Where(a => a.FromNode == false);

            var xxx = from arc in Items.OfType<Arc>().Where(a => a.FromNode == true)
                      join arc2 in Items.OfType<Arc>().Where(a => a.FromNode == false)
                      on arc.Trans.Ref equals arc2.Trans.Ref
                      select new { n1 = arc.Node, t = arc.Trans, n2 = arc2.Node };

            var yyy = from set1 in xxx
                      join set2 in xxx
                      on set1.n2 equals set2.n1
                      where set1.t != set2.t
                      select new { frm = set1.t, to = set2.t };

            foreach (var pair in yyy)
            {
                pair.frm.ConnectedTransitions.Add(pair.to);
            }
        }

        private void ConnectNode2Trans(object sender, ArcAddedEventArgs e)
        {
            var theNextArc = Items.OfType<Arc>().Count() + 1;
            var arc = new Arc { Ref = $"a{theNextArc}", Node = e.ArcNode, Trans = e.ArcTransition, FromNode = e.FromNode };
            arc.PropertyChanged += NodeOrTrans_PropertyChanged;
            Items.Insert(0, arc);
            MakeConnections();
        }

        private void NodeOrTrans_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "X": case "Y": TheRefreshCommand.Execute(this); break;

                case "DeleteTriggered":
                    switch (sender)
                    {   // Delete a node that has been clicked on
                        case Node node:
                            if (System.Windows.MessageBox.Show($"Delete {node.Ref}", "Delete", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                            {
                                Items.Remove(sender as Node);
                                foreach (var removeArc in Items.OfType<Arc>().Where(arc => arc.Node == node).ToList())
                                {
                                    Items.Remove(removeArc);
                                }
                                TheRefreshCommand.Execute(this);
                            }
                            break;
                        // Delete a transition that has been clicked on
                        case Transition trans:
                            if (System.Windows.MessageBox.Show($"Delete {trans.Ref}", "Delete", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                            {
                                // Remove the transition from the list of items
                                Items.Remove(trans);
                                // Remove all the arcs that are connected to the transition
                                foreach (var removeArc in Items.OfType<Arc>().Where(arc => arc.Trans == trans).ToList())
                                {
                                    Items.Remove(removeArc);
                                }
                                // Refresh the screen
                                TheRefreshCommand.Execute(this);
                            }
                            break;
                        case Arc arc:
                            if (System.Windows.MessageBox.Show($"Delete {arc.Ref}", "Delete", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                            {
                                // Remove the transition from the list of items
                                Items.Remove(arc);
                                MakeConnections();
                                TheRefreshCommand.Execute(this);
                            }
                            break;
                    }
                    break;

            }
        }

        private RelayCommand<MouseEventArgs> _mouseUpCommand;
        public RelayCommand<MouseEventArgs> MouseUpCommand
        {
            get
            {
                if (_mouseUpCommand == null)
                    _mouseUpCommand = new RelayCommand<MouseEventArgs>(MouseUp);
                return _mouseUpCommand;
            }
            set => _mouseUpCommand = value;
        }

        private Point lastMousePosition = new Point(0, 0);

        private void MouseUp(MouseEventArgs e)
        {
            if (e.Source is IInputElement theInputElement)
            {
                lastMousePosition = e.GetPosition(theInputElement);
            }
        }

    }

}
