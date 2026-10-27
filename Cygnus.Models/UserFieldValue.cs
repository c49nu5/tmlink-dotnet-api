namespace Cygnus.Models
{
    public class UserFieldValue
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
        /// User field value (16 chars max).
        /// </summary>
        public string? Value { get; set; }
    }
}