using System;
using System.Runtime.Serialization;

namespace PetriNet
{
    [DataContract(IsReference = true)]
    public class Arc : Displayable
    {
        [DataMember]
        public Boolean FromNode { get; set; }
        [DataMember]
        public Transition Trans { get; set; }
        [DataMember]
        public Node Node { get; set; }
        int qty = 1;
        [DataMember]
        public int Qty { get => qty; set => UpdateProperty(ref qty, value); }
        [DataMember]
        public double TransferRate { get; set; }
    }

}
