

namespace RazerBase
{

    #region enum ScreenBaseTypeEnum : int
    /// <summary>
    /// This enum contains the choices for the type of control a ScreenBase object is.
    /// </summary>
    public enum ScreenBaseTypeEnum : int
    {
        Unknown = 0,
        Folder = 1,
        Tab = 2,
        Lookup = 3
    } 
    #endregion

    public enum AccessLevel : int
    {
        NoAccess = 0, // No Access to the object
        ViewOnly = 1, // View Only Access
        ViewUpdate = 2, // View, Update, No Delete
        ViewUpdateDelete = 3 // View, Update, Delete
    }

}
