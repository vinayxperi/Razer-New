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
using RazerBase.Interfaces;

namespace Security
{
    /// <summary>
    /// Interaction logic for SecurityRoleTab.xaml
    /// </summary>
    public partial class SecurityRole : ScreenBase, IScreen
    {
        public SecurityRole()
        {
            InitializeComponent();
        }

        #region IScreen Implementation
        public void Init(cBaseBusObject businessObject)
        {

        }

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }
        #endregion
    }
}
