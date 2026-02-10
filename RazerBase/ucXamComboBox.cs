using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.Windows.Editors;

namespace RazerBase
{
    public class ucXamComboBox : ComboBoxItemsProvider
    {
        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }
    }
}
