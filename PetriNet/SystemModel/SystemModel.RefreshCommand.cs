using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PetriNet
{
    public partial class SystemModel
    {
        public ICommand TheRefreshCommand { get; }
        double _width = 500D;
        public double Width { get => _width; private set => UpdateProperty(ref _width, value); }

        public void Refresh(SystemModel sysModel)
        {
                ObservableCollection<Displayable> saveState = null;

                saveState = sysModel.Items;
                sysModel.Items = new ObservableCollection<Displayable>();
                sysModel.Items = saveState;
                saveState = null;

                sysModel.Width = sysModel.Items.Max(disp => disp.X) + 60;
        }
    }
}
