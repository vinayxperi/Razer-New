

#region using statements

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RazerInterface;

#endregion

namespace RazerBase
{

    #region class ucFieldPlaceHolder
    /// <summary>
    /// Interaction logic for ucFieldPlaceHolder.xaml
    /// This class doesn't actually do anything, but it has all the properties of a FieldValueControl
    /// just to be consistent in case there is some value of the interface to affect this control;
    /// </summary>
    public partial class ucFieldPlaceHolder : UserControl, IFieldValueControl
    {
        
        #region Private Variables
        private GridLength labelWidth;
        private string labelText;
        private string bindPath;
        private double DefaultLabelWidth = 84;
        private string text;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Create a new instance of a 'ucFieldPlaceHolder' object.
        /// </summary>
        public ucFieldPlaceHolder()
        {
            // Create controls
            InitializeComponent();

            // Perform initializations for this object.
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
               // Set the label width to the default label width
               this.LabelWidth = new GridLength(DefaultLabelWidth);
            }
            #endregion
                                    
        #endregion

        #region Events
   
        #endregion

        #region Properties
    
            #region BindPath
            /// <summary>
            /// This property gets or sets the value for 'BindPath'.
            /// </summary>
            public string BindPath
            {
                get { return bindPath; }
                set { bindPath = value; }
            }
            #endregion

            #region LabelText
            /// <summary>
            /// This property gets or sets the value for 'LabelText'.
            /// </summary>
            public string LabelText
            {
                get { return labelText; }
                set { labelText = value; }
            }
            #endregion
            
            #region LabelWidth
            /// <summary>
            /// This property gets or sets the value for 'LabelWidth'.
            /// </summary>
            public GridLength LabelWidth
            {
                get { return labelWidth; }
                set {  labelWidth = value; }
            }
            #endregion
          
            #region Text
            /// <summary>
            /// This property gets or sets the value for 'Text'.
            /// </summary>
            public string Text
            {
                get { return text; }
                set { text = value; }
            }
            #endregion
            
        #endregion
        
    }
    #endregion

}
