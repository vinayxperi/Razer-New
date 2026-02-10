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
using RazerBase;
using RazerInterface;

namespace Units
{
    /// <summary>
    /// Interaction logic for UnitsGhostScreen.xaml
    /// </summary>
    public partial class UnitsGhostScreen : ScreenBase
    {
        public UnitsGhostScreen()
        {
            InitializeComponent();
        }



        public void CloseScreen()
        {
            System.Windows.Window AdjParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            //this.CurrentBusObj.ObjectData.AcceptChanges();
            //StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                AdjParent.Close();
            }

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Visibility = System.Windows.Visibility.Visible;
            CloseScreen();
        }
    }
}
