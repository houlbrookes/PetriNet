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
        public ICommand TheAddTransitionCommand { get; }

        private void AddTransition(SystemModel sysModel)
        {
            var nextTrans = sysModel.Items.OfType<Transition>().Count() + 1;
            var transition = new Transition { Ref = $"T{nextTrans}", X = lastMousePosition.X, Y = lastMousePosition.Y };
            sysModel.Items.Add(transition);
            transition.DoubleClick = dbClick;
            transition.PropertyChanged += NodeOrTrans_PropertyChanged;
            transition.Node2Transition += ConnectNode2Trans;
        }
    }
}
