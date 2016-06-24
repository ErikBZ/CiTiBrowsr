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
            {
                strBuilder.Append(nextChar());
            }
            return strBuilder.ToString();
        }

        private Dom.Node parseNode()
        {
            Dom.Node thing = new Dom.Node();
            if (nextChar() == '<')
            {

            }

            return thing;
        }

        private Dom.Node parseElement()
        {
            return new Dom.Node();
        }

        private Dom.Node parseText()
        {
            return new Dom.Node();
        }
    }
}