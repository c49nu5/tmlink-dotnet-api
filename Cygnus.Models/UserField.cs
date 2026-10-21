namespace Cygnus.Models
{
    public class UserField
    {
        /// <summary>
        /// User field type
        /// </summary>
        public UserFieldType Type { get; set; }
        /// <summary>
        /// User field name (16 chars max).
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Optional List of text items (each 16 chars max)
        /// </summary>
        public List<string>? List { get; set; }
    }
}