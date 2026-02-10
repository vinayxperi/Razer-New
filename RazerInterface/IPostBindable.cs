using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazerInterface
{
    public interface IPostBindable
    {
        #region Methods
        /// <summary>
        /// This interface is used by any method that needs to call PostBind()
        /// </summary>
        void PostBind();
        #endregion

    }
}
