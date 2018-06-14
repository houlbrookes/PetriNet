using System;
using System.Windows;
using System.Windows.Input;

namespace PetriNet
{

    public partial class SystemModel
    {
        private ICommand _dblClick = new RelayCommand<Transition>(Transition_dblClick);
        public ICommand dbClick { get => _dblClick; }

        private static void Transition_dblClick(Transition trans)
        {
            if (trans != null)
            {
                if (trans.On)
                {
                    trans.Fire();
                    foreach (var t in trans.ConnectedTransitions)
                    {
                        var temp = true;
                        t.UpdateProperty(ref temp, false, "On");
                    }
                }
                else
                {
                    MessageBox.Show("Transition is not ON");
                }
            }
            else
            {
                MessageBox.Show("Clicked (not trans)");
            }
        }

    }

}
