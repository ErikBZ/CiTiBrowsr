using System;
using System.Collections.Generic;
using System.IO;
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

        
        static int Main(string[] args)
        {
            string path = @"..\..\examples\test_one.html";
            string toParse = "<html></html>";
            if(File.Exists(path))
            {
                toParse = File.ReadAllText(path);
            }

            Parser parser = new Parser();
            parser.pos = 0;
            parser.input = toParse;
            Dom.Node html = parser.parseElement();
            Console.Write(html);
            Console.Read();

            // program exited properly
            return 0;
        }

        public Parser()
        {
            head = null;
            pos = 0;
            input = "";
        }

        public Parser(uint pos, string input)
        {
            this.pos = pos;
            this.input = input;
        }

        /// <summary>
        /// Parses an entire HTML file
        /// </summary>
        /// <param name="input">Input is the HTML file as a string</param>
        /// <returns>Returns a head for the DOM data struct tree</returns>
        internal Dom.Node parseHTML(string input)
        {
            Parser parser = new Parser(0, input);
            Dom.Node[] nodes = parser.parseChildren();
            if (nodes.Length == 1)
                return nodes[0];
            else
                return Dom.createElement("html", new Dictionary<string, string>(), nodes);
        }

        // returns character at position
        private char nextChar()
        {
            if (endOfFile())
            {
                throw new Exception(String.Format("Position {0} is greater than length {1} of input string", pos, input.Length));
            }
            return input[(int)pos];
        }

        // checks if input string starts with string s
        private bool startsWith(string s)
        {
            string restOfInput = input.Substring((int)pos);
            return restOfInput.StartsWith(s);
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
            while (!endOfFile() && (Char.IsLetterOrDigit(nextChar()) || nextChar() == '.'))
                strBuilder.Append(consumeChar());
            return strBuilder.ToString();
        }

        /// <summary>
        /// chooses which type of Node to parse, Element or Text and calls the correct method
        /// </summary>
        /// <returns>Returns Node of the corerct type</returns>
        private Dom.Node parseNode()
        {
            Dom.Node thing;
            if (nextChar() == '<')
                thing = parseElement();
            else
                thing = parseText();

            return thing;
        }

        /// <summary>
        /// Parses a single element and then its children. This is a recursive method the recursion being
        /// parseElement -> parseChildren -> parseElement et cetera
        /// Also i don't know if i'm handling the exceptions correctly with my ghetto asserts
        /// this is probably a bad way to do it
        /// </summary>
        /// <returns>Returns a sub tree of the DOM struct</returns>
        private Dom.Node parseElement()
        {
            string tag = "";
            Dictionary<string, string> attr = new Dictionary<string, string>();
            // starting the tag handling
            assert(consumeChar(), '<');
            tag = parseTagName();
            attr = parseAttributes();
            assert(consumeChar(), '>');

            /// TODO get the children of the node
            Dom.Node[] children = parseChildren();

            // closing the tag
            assert(consumeChar(), '<');
            assert(consumeChar(), '/');
            assert(parseTagName(), tag);
            assert(consumeChar(), '>');

            return Dom.createElement(tag, attr, children);
        }

        /// <summary>
        /// parses nodes which are in the same level as each other. Uses a queue to add new DomNodes and then 
        /// converts it into an array
        /// </summary>
        /// <returns>Sibling nodes</returns>
        private Dom.Node[] parseChildren()
        {
            Queue<Dom.Node> domNodeQueue = new Queue<Dom.Node>();

            // need to make sure the next char will be non whitespace
            consumeWhitespace();
            while (!endOfFile() && !startsWith("</"))
            {
                domNodeQueue.Enqueue(parseNode());
                consumeWhitespace();
            }
            return domNodeQueue.ToArray();
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
            if (!x.Equals(y))
                throw new Exception(String.Format("Assertion {0} == {1} failed.", x, y));
            return true;
        }
    }
}