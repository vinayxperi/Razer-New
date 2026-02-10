using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cTransaction
{
    /// <summary>
    /// Class cSpParm used in cTransaction to store paramter information.
    /// </summary>
    public class cSpParm : ICloneable
    {
        private string isParameterName = null;
        private string isType = null;
        private int iiLength;
        private int iiOrder;
        private bool bIsOutPutParm = false;
        private object ioParm = null;

        /// <summary>
        /// Read Only Property: Parameter
        /// </summary>
        /// <value>String</value>
        /// <returns>String</returns>
        /// <remarks>The SP parameter name</remarks>
        public string Parameter 
        { 
            get { return isParameterName; } 
        }

        /// <summary>
        /// Read Only Property: Type
        /// </summary>
        /// <value>String</value>
        /// <returns>String</returns>
        /// <remarks>The SP parameter type</remarks>
        public string Type
        {
            get { return isType; }
        }

        /// <summary>
        /// Read Only Property: Length
        /// </summary>
        /// <value>Integer</value>
        /// <returns>Integer</returns>
        /// <remarks>The SP parameter length; if applicable.</remarks>
        public int Length
        {
            get { return iiLength; }
        }

        /// <summary>
        /// Read Only Property: Order
        /// </summary>
        /// <value>Integer</value>
        /// <returns>Integer</returns>
        /// <remarks>The SP parameter sequence</remarks>
        public int Order
        {
            get { return iiOrder; }
        }

        public bool IsOutPutParm
        {
            get { return bIsOutPutParm; }
        }

        /// <summary>
        /// Read Only Property: Parm
        /// </summary>
        /// <value>Object</value>
        /// <returns>Object</returns>
        /// <remarks>The SP parm</remarks>
        public object Parm
        {
            get { return ioParm; }
        }

        /// <summary>
        /// Constructor for class cSpParms
        /// </summary>
        /// <param name="parameter">String</param>
        /// <param name="parm">Object</param>
        /// <remarks>Used for passing in SP parms to cTransaction objects</remarks>
        public cSpParm(string parameter, object parm, bool IsOutPutParm)
        {
            isParameterName = parameter;
            ioParm = parm;
            this.bIsOutPutParm = IsOutPutParm;
        }

        /// <summary>
        /// Constructor for class cSpParms
        /// </summary>
        /// <param name="parameter">String</param>
        /// <param name="type">String</param>
        /// <param name="length">Integer</param>
        /// <param name="order">Integer</param>
        /// <remarks>Used for internal database lookups in cTransaction</remarks>
        public cSpParm(string parameter, string type, int length, int order, bool IsOutPutParm)
        {
            isParameterName = parameter;
            isType = type;
            iiLength = length;
            iiOrder = order;
            this.bIsOutPutParm = IsOutPutParm;
        }

        /// <summary>
        /// Adds a value to the parms that are passed to cTransaction objects.
        /// </summary>
        /// <param name="parameter">String</param>
        /// <param name="parm">Object</param>
        /// <remarks></remarks>
        public void AddParmValue(string parameter, object parm)
        {
            isParameterName = parameter;
            ioParm = parm;
        }

        /// <summary>
        /// Clone returns a shallow copy of the object
        /// </summary>
        /// <returns>Object</returns>
        /// <remarks></remarks>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
