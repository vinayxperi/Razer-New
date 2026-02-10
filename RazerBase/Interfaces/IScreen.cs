using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazerBase.Interfaces
{
    public interface IScreen
    {
        void Init(cBaseBusObject businessObject);
        string WindowCaption { get; }
    }
}
