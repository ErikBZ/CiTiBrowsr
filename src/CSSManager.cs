using System;

/// <summary>
/// Manages all the CSS rules and is used by Parser to parse CSS
/// </summary>
namespace ToyBrowser.src
{
    public class CSSManager
    {

        public struct SimpleSelector
        {
            string tagName;
            string id;
            string[] classes;
        }
        struct Declaration
        {
            string name;
            Value value;
        }

        /// <summary>
        /// Tracks a declaration's value
        /// Some more functionality may be added later on, probably not though.
        /// </summary>
        struct Value
        {
            ValueType type;

            // Keywords
            string keyword;
            
            // Length
            float len;
            Unit unit;
            
            // Color
            Color color;
        }

        /// <summary>
        /// Will be used for error checking to make sure Value has a certain
        /// variable tracked
        /// </summary>
        enum ValueType
        {
            Keyword,
            Length,
            Color,
        }

        /// <summary>
        /// Enum for the type of unit that a value might be like px or p
        /// </summary>
        enum Unit
        {
            Px,
            // add more units later 
        }

        /// <summary>
        /// Struct that keep track of a Values color
        /// uses 4 unisgned bytes for Red, Green, Blue and Alpha
        /// </summary>
        struct Color
        {
            byte r, g, b, a;
        }

        public CSSManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}