using System;

/// <summary>
/// Manages all the CSS rules and is used by Parser to parse CSS
/// </summary>
namespace ToyBrowser.src
{
    public class CSSManager
    {

        public struct Systelsheet
        {
            Rule[] rules;
        }

        public struct Rule
        {
            SimpleSelector[] selectors;
            Declaration[] declarations;
        }

        public struct SimpleSelector
        {
            public string tagName;
            public string id;
            public string[] classes;
        }

        /// <summary>
        /// Keeps track of a selectors declarations 
        /// </summary>
        public struct Declaration
        {
            public string name;
            public Value value;
        }

        /// <summary>
        /// Tracks a declaration's value
        /// Some more functionality may be added later on, probably not though.
        /// </summary>
        public struct Value
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
        public enum ValueType
        {
            Keyword,
            Length,
            Color,
        }

        /// <summary>
        /// Enum for the type of unit that a value might be like px or p
        /// </summary>
        public enum Unit
        {
            Px,
            // add more units later 
        }

        /// <summary>
        /// Struct that keep track of a Values color
        /// uses 4 unisgned bytes for Red, Green, Blue and Alpha
        /// </summary>
        public struct Color
        {
            byte r, g, b, a;
        }

        public struct Specificity
        {
            public uint most;
            public uint med;
            public uint least;

            public Specificity(SimpleSelector x)
            {
                this.least = (uint)x.tagName.Length;
                this.med = (uint)x.classes.Length;
                this.most = (uint)x.id.Length;
            }
        }

        public CSSManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}