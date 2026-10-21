namespace Cygnus.Models
{
    public enum UserFieldType
    {
        /// <summary>
        /// The UserField is not used
        /// </summary>
        NotUsed = 0,
        /// <summary>
        /// Text or String
        /// </summary>
        Text = 1,
        /// <summary>
        /// Number
        /// </summary>
        Number = 2,
        /// <summary>
        /// Date in DD-MM-YYYY format
        /// </summary>
        DateDMY = 3,
        /// <summary>
        /// Date in MM-DD-YYYY format
        /// </summary>
        DateMDY = 4,
        /// <summary>
        /// A List of Items
        /// </summary>
        List = 5,
        /// <summary>
        /// Bool or Yes/No type
        /// </summary>
        YesNo = 6,
    }
}