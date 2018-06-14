using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace PetriNet
{
    [DataContract(IsReference = true)]
    public class Transition : Displayable
    {
        [DataMember]
        public List<Arc> Arcs { get; set; }

        [DataMember]
        public List<Transition> ConnectedTransitions { get; set; }

        public bool TestValue { get; set; }
        public bool On
        {
            get
            {
                // a transition is ON if all the inputs have enough tokens
                // and all outputs have sufficient space
                bool result = Arcs.TrueForAll(arc => (arc.FromNode && arc.Node.TokenCount >= arc.Qty)
                                           || (!arc.FromNode && arc.Node.MaxToken <= arc.Qty));
                return result;
            }
        }

        [XmlIgnore]
        public ICommand DoubleClick { get; set; }

        public Transition()
        {
            Arcs = new List<Arc>();
            ConnectedTransitions = new List<Transition>();
            DoubleClick = null;
        }

        /// <summary>
        /// Transfer over the tokens from source to destination
        /// </summary>
        public void Fire()
        {
            foreach (var arc in Arcs)
            {
                if (arc.FromNode)
                {
                    arc.Node.TokenCount -= arc.Qty;
                }
                else
                {
                    arc.Node.TokenCount += arc.Qty;
                }
            }

            var dummy = false;
            UpdateProperty(ref dummy, true, "On");
        }

        public event ArcAddedEventHandler Node2Transition;
    
        public void Connect2Transition(Node node)
        {
            Node2Transition?.Invoke(this, new ArcAddedEventArgs(node, this, true));
        }

        public delegate void ArcAddedEventHandler(object sender, ArcAddedEventArgs e);

    }

}
