using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PetriNet
{
    public partial class SystemModel
    {
        public ICommand TheAddNodeCommand { get; }

        private void AddNode(SystemModel sysModel)
        {
            var nextNode = sysModel.Items.OfType<Node>().Count()+1;
            var node = new Node { Ref = $"N{nextNode}", X = lastMousePosition.X, Y = lastMousePosition.Y };
            sysModel.Items.Add(node);
            node.PropertyChanged += NodeOrTrans_PropertyChanged;
            node.Transition2Node += ConnectNode2Trans;
        }
    }
}
