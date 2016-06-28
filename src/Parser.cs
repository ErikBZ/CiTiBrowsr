using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Parses HTML into a DOM object
/// </summary>

namespace ToyBrowser.src
{
    public class Parser
    {
        Dom.Node head;
        uint pos;
        string input;

        public Parser()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        // returns character at position
        private char nextChar()
        {
            if (endOfFile())
            {
                throw new Exception("Position is greater than length of input string");
            }
            return input[(int)pos];
        }

        // checks if input string starts with string s
        private bool startWith(string s)
        {
            return input.StartsWith(s);
        }

        // checks to see if we've reached the end of the input string
        private bool endOfFile()
        {
            return pos >= input.Length;
        }

        // returns characater at position and increments position
        private char consumeChar()
        {
            char next = nextChar();
            pos++;
            return next;
        }

        /// <summary>
        /// increments pos until the current char at pos is not whitespace
        /// </summary>
        private void consumeWhitespace()
        {
            // C# has shortcircuiting yayyy
            while (!endOfFile() && String.IsNullOrWhiteSpace(nextChar().ToString()))
            {
                pos++;
            }
        }

        /// <summary>
        /// parses a tag or attribute name. like html or id="thing"
        /// </summary>
        /// <returns> returns attibute name or tag name</returns>
        private string parseTagName()
        {
            StringBuilder strBuilder = new StringBuilder();
            while (!endOfFile() && (Char.IsLetterOrDigit(nextChar())))
                strBuilder.Append(nextChar());
            return strBuilder.ToString();
        }

        private Dom.Node parseNode()
        {
            Dom.Node thing;
            if (nextChar() == '<')
                thing = parseElement();
            else
                thing = parseText();

            return thing;
        }

        private Dom.Node parseElement()
        {
            // starting the tag handling
            assert(consumeChar(), '<');
            String tag = parseTagName();
            ///TODO parse attirbutes
            Dictionary<string, string> attr = parseAttributes();
            assert(consumeChar(), '>');

            /// TODO get the children of the node
            Dom.Node[] children = null;

            // closing the tag
            assert(consumeChar(), '<');
            assert(consumeChar(), '/');
            assert(parseTagName(), tag);
            assert(consumeChar(), '>');

            return Dom.createElement(tag, attr, children);
        }

        private Dom.Node parseText()
        {
            StringBuilder strBuilder = new StringBuilder();
            while(nextChar() != '<' && !endOfFile())
            {
                strBuilder.Append(consumeChar());
            }
            Dom.Node node = Dom.createText(strBuilder.ToString());
            return node;
        }

        /// <summary>
        /// parseAttributes should be called after parsing the tag name.
        /// </summary>
        /// <returns>A dictionary with all the attibute tags to values dictionary</returns>
        private Dictionary<string, string> parseAttributes()
        {
            // precaution. if the tag has whitespace between the end of the 
            // tag name but no attirbutes with method outside the loop ensures
            // that tags such as <html > or <html    > are okay
            consumeWhitespace();
            Dictionary<string, string> attrMap = new Dictionary<string, string>();
            while(nextChar() != '>')
            {
                consumeWhitespace();
                string key;
                string entry;
                key = parseAttrName();
                entry = parseAttrValue();
                attrMap.Add(key, entry);
            }
            return attrMap;
        }

        /// <summary>
        /// called after consuming whitespace after parsing a tag or a compelte attribute dictionary entry
        /// </summary>
        /// <returns>The name of the attribute</returns>
        private string parseAttrName()
        {
            string name = parseTagName();
            assert(consumeChar(), '=');
            return name;
        }

        /// <summary>
        /// called directly after parsing the tag name
        /// </summary>
        /// <returns>The attirbute value without the the quotes. So just content not "content"</returns>
        private string parseAttrValue()
        {
            assert(consumeChar(), '"');
            string attrValue = parseTagName();
            assert(consumeChar(), '"');
            return attrValue;
        }

        // checks to see if chars are equal
        private bool assert(char x, char y)
        {
            if (x != y)
                throw new Exception(String.Format("Assertion {0} == {1} failed.", x, y));
            return true;
        }

        private bool assert(string x, string y)
        {
            if (x.Equals(y))
                throw new Exception(String.Format("Assertion {0} == {1} failed.", x, y));
            return true;
        }
    }
}