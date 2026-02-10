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
    /// Interaction logic for ucXamCheckBox.xaml
    /// </summary>
    public partial class ucXamCheckBox : XamCheckEditor
    {
        public ucXamCheckBox()
        {
            InitializeComponent();
            this.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(ucXamCheckBox_LostKeyboardFocus);
        
        }

        void ucXamCheckBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.IsChecked.HasValue)
            {
                if (this.IsChecked.Value)
                {
                    this.Value = 1;

                }
                else
                {
                    this.Value = 0;

                }
            }
            //TraversalRequest tr = new TraversalRequest(FocusNavigationDirection.Right);
            //this.MoveFocus(tr);
        }
    }
    

}
