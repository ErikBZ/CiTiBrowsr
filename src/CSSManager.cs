using System;
using System.Text;

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
            public ValueType type;

            // Keywords
            public string keyword;
            
            // Length
            public float len;
            public Unit unit;
            
            // Color
            public Color color;

            public override string ToString()
            {
                StringBuilder strBuilder = new StringBuilder();
                switch(type)
                {
                    case ValueType.Color:
                        strBuilder.Append('#');
                        byte[] data = { color.r, color.g, color.b };
                        strBuilder.Append(BitConverter.ToString(data));
                        break;
                    case ValueType.Keyword:
                        strBuilder.Append(keyword);
                        break;
                    case ValueType.Length:
                        strBuilder.Append(len);
                        switch (unit)
                        {
                            case Unit.P:
                                strBuilder.Append('p');
                                break;
                            case Unit.Px:
                                strBuilder.Append("px");
                                break;
                        }
                        break;
                    default:
                        break;
                }
                return strBuilder.ToString();
            }
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
            P,
            // add more units later 
        }

        /// <summary>
        /// Struct that keep track of a Values color
        /// uses 4 unisgned bytes for Red, Green, Blue and Alpha
        /// </summary>
        public struct Color
        {
            public byte r, g, b, a;
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