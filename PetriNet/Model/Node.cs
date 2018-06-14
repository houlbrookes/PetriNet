using System.Runtime.Serialization;

namespace PetriNet
{
    [DataContract(IsReference = true)]
    public class Node : Displayable
    {
        int tokenCount = 0;
        [DataMember]
        public int TokenCount
        {
            get => tokenCount;
            set
            {
                UpdateProperty(ref tokenCount, value);
            }
        }
        [DataMember]
        public int MaxToken { get; set; }

        public event ArcAddedEventHandler Transition2Node;

        public void Connect2Transition(Transition trans)
        {
            Transition2Node?.Invoke(this, new ArcAddedEventArgs(this, trans, false));
        }

    }

    public delegate void ArcAddedEventHandler(object sender, ArcAddedEventArgs e);

}
