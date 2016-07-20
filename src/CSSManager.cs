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
            // will needs to change this at some point
            // a Selector can have multiple tag names, ids
            public string tagName;
            public string id;
            public string[] classes;
            public byte[] specificity;

            public SimpleSelector(string tag, string id, string[] c)
            {
                this.tagName = tag;
                this.id = id;
                this.classes = c;

                this.specificity = new byte[3]{0,0,0};
                if (this.id.Length != 0)
                {
                    this.specificity[0] = 1;
                }
                this.specificity[1] = (byte)classes.Length;
                if(this.tagName.Length != 0)
                {
                    this.specificity[2] = 1;
                }
            }

            public override string ToString()
            {
                StringBuilder strBuilder = new StringBuilder();
                if(id.Length != 0)
                {
                    strBuilder.Append('#');
                    strBuilder.Append(id);
                }
                strBuilder.Append(' ');
                strBuilder.Append(tagName);
                if(classes.Length != 0)
                {
                    foreach (string c in classes)
                    {
                        strBuilder.Append('.');
                        strBuilder.Append(c);
                    }
                }

                return strBuilder.ToString();
            }
        }

        /// <summary>
        /// Keeps track of a selectors declarations 
        /// </summary>
        public struct Declaration
        {
            public string name;
            public Value value;

            public override string ToString()
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append(name);
                strBuilder.Append(':');
                strBuilder.Append(value);
                return strBuilder.ToString();
            }
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
                        string str = BitConverter.ToString(data);
                        string[] subStrings = str.Split('-');
                        foreach(string s in subStrings)
                        {
                            strBuilder.Append(s);
                        }
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

        /// <summary>
        /// Sorts an array of selectors by specificity.
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns>Returns a sorted array</returns>
        public static SimpleSelector[] SortSelectorArray(SimpleSelector[] selectors)
        {
            for(int i=0;i<selectors.Length;i++)
            {
                bubbleUp(selectors, i);
            }
            return selectors;
        }

        private static void bubbleUp(SimpleSelector[] array, int exclude)
        {
            SimpleSelector s = array[0];
            for(int i=0;i<array.Length - exclude;i++)
            {
                if(compare(s, array[i]) == 1 || compare(s, array[i]) == 0)
                {
                    s = array[i];
                }
                // swapping
                else
                {
                    array[i - 1] = array[i];
                    array[i] = s;
                }
            }
        }

        /// <summary>
        /// Compares s1 to s2
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns>-1 if s1 is less than, 0 if they're equal and 1 if s1 is bigger</returns>
        private static int compare(SimpleSelector s1, SimpleSelector s2)
        {
            for(int i = 0; i<s1.specificity.Length; i++)
            {
                if (s1.specificity[i] > s2.specificity[i])
                    return 1;
                else if (s1.specificity[i] < s2.specificity[i])
                    return -1;
            }

            // neither was bigger
            return 0;
        }

        public CSSManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}