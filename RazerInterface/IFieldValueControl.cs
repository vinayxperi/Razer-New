

#region using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

#endregion

namespace RazerInterface
{

    #region interface IFieldValueControl
    /// <summary>
    /// This interface is used by controls such as the LabelTextBoxControl, LabelCheckBoxControl
    /// and the soon to be created LabelCalendarControl;
    /// </summary>
    public interface IFieldValueControl
    {

        #region Properties

            #region HorizontalAlignment
            /// <summary>
            /// This property gets or sets the value for 'HorizontalAlignment'.
            /// </summary>
            HorizontalAlignment HorizontalAlignment
            {
                get;
                set;
            }
            #endregion
            
            #region LabelWidth
            /// <summary>
            /// This property gets or sets the value for 'LabelWidth'.
            /// </summary>
            GridLength LabelWidth
            {
                get;
                set;
            }
            #endregion

        #endregion

    } 
    #endregion

}
