using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucEditorField.xaml
    /// </summary>
    public partial class ucEditorField : CellValuePresenter
    {
        public ucEditorField()
        {
            InitializeComponent();
            this.GotFocus +=new RoutedEventHandler(ucEditorField_GotFocus);
           
        }

        void ucEditorField_GotFocus(object sender, RoutedEventArgs e)
        {
            ValueEditor x = this.Editor;
            x.StartEditMode();
        }
    }
}
