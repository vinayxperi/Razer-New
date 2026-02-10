using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazerInterface
{

    #region interface IPreBindable
    /// <summary>
    /// This interface is needed to be implemented by any controls that need 
    /// to have PreBind implemented
    /// </summary>
    public interface IPreBindable
    {

        #region Methods
        /// <summary>
        /// This interface is used by any method that needs to call PreBind()
        /// </summary>
        void PreBind();
        #endregion

    }
    #endregion

}
