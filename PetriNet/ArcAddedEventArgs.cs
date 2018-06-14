using System;

namespace PetriNet
{
    public class ArcAddedEventArgs : EventArgs
    {
        public ArcAddedEventArgs(Node node, Transition trans, bool fromNode) : base()
        {
            ArcNode = node;
            ArcTransition = trans;
            FromNode = fromNode;
        }

        public virtual Node ArcNode { get; }
        public virtual Transition ArcTransition { get; }
        public virtual bool FromNode { get; }
    }

}
