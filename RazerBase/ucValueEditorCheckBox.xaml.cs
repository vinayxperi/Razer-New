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
using Infragistics.Windows.Editors;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucValueEditorCheckBox.xaml
    /// </summary>
    public partial class ucValueEditorCheckBox : ValueEditorCheckBox
    {
        public ucValueEditorCheckBox()
        {
            InitializeComponent();
            this.Checked += new RoutedEventHandler(ucValueEditorCheckBox_Checked);
        }

        void ucValueEditorCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("HI");
        }
    }
}
