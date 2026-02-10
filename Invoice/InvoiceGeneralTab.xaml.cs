

#region using statements

using RazerBase;
using RazerInterface;
using System;
using System.Data;
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


#endregion

namespace Invoice
{

    #region class InvoiceGeneralTab
    /// <summary>
    /// This class represents a 'ucTab1' object.
    /// </summary>
    public partial class InvoiceGeneralTab : ScreenBase
    {

        #region Private Variables
        #endregion

        #region Constructor
        
        public InvoiceGeneralTab()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Perform initializations for this object
            Init();
        }
        #endregion

        #region Methods

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Tab;

            // Change this setting to the name of the DataTable that will be used for Binding.
            MainTableName = "general";


        }
        #endregion
        private void ScreenBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        #endregion

       
     
    }
    #endregion
}