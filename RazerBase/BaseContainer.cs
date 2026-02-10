

#region using statements

using System.Windows;
using System.Windows.Controls;
using RazerInterface;

#endregion

namespace RazerBase
{

    #region class BaseContainer
    /// <summary>
    /// This control inherits from StakPanel
    /// </summary>
    public class BaseContainer : StackPanel
    {
        
        #region Private Variables
        private int leftMargin;
        private int labelWidth;
        #endregion
        
        #region Constructor
        /// <summary>
        /// This constructor [enter description here].
        /// </summary>
        static BaseContainer()
        {
            // Set the Style (is this needed ?)
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseContainer), new FrameworkPropertyMetadata(typeof(BaseContainer)));
        }
        #endregion

        #region Events

        #endregion

        #region Methods

            #region NotifyChildren(string propertyName, int doubleValue)
            /// <summary>
            /// This method [enter description here].
            /// </summary>
            public void NotifyChildren(string propertyName, double doubleValue)
            {
                // iterate the children
                foreach (UIElement element in this.Children)
                {
                    // cast the element as a ucLabelTextBox
                    IFieldValueControl control = element as IFieldValueControl;

                     // if the textBox exists
                    if (control != null)
                    {
                        switch(propertyName)
                        {
                            case "LabelWidth":

                                // set the doubleValue
                                control.LabelWidth = new GridLength(doubleValue);

                                // required
                                break;

                            case "HorizontalAlignment":

                                // set the horizontal align
                                control.HorizontalAlignment = HorizontalAlignment;

                                // required
                                break;
                        }
                    }
                }
            }
            #endregion
            
        #endregion

        #region Properties

            #region LabelWidth
            /// <summary>
            /// This property gets or sets the value for 'LabelWidth'.
            /// </summary>
            public int LabelWidth
            {
                get { return labelWidth; }
                set 
                { 
                    // set the label width
                    labelWidth = value;

                    // Notify the children of this object
                    NotifyChildren("LabelWidth", labelWidth);
                }
            }
            #endregion
            
            #region LeftMargin
            /// <summary>
            /// This property gets or sets the value for 'LeftMargin'.
            /// </summary>
            public int LeftMargin
            {
                get { return leftMargin; }
                set { leftMargin = value; }
            }
            #endregion
            
        #endregion
        
    }
    #endregion

}
