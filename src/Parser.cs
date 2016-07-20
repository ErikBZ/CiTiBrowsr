using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

/// <summary>
/// Parses HTML into a DOM object
/// </summary>

namespace ToyBrowser.src
{
    public class Parser
    {
        uint pos;
        string input;


        static int Main(string[] args)
        {
            //string path = @"..\..\examples\test_one.html";
            //string toParse = "<html></html>";
            //if (File.Exists(path))
            //{
            //    toParse = File.ReadAllText(path);
            //}

            CSSManager.SimpleSelector s1 = new CSSManager.SimpleSelector();
            CSSManager.SimpleSelector s2 = new CSSManager.SimpleSelector();
            CSSManager.SimpleSelector s3 = new CSSManager.SimpleSelector();
            CSSManager.SimpleSelector s4 = new CSSManager.SimpleSelector();

            s1.specificity = new byte[3] { 0, 0, 0 };
            s2.specificity = new byte[3] { 2, 0, 0 };
            s3.specificity = new byte[3] { 0, 0, 4 };
            s4.specificity = new byte[3] { 3, 0, 4 };

            CSSManager.SimpleSelector[] specs = new CSSManager.SimpleSelector[4] { s1, s2, s3, s4 };
            CSSManager.SortSelectorArray(specs);

            foreach(CSSManager.SimpleSelector s in specs)
            {
                Console.Write(string.Format("{0} {1} {2}\n", s.specificity[0], s.specificity[1], s.specificity[2]));
            }
            Console.Read();

            // program exited properly
            return 0;
        }

        public Parser()
        {
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

        // consumes an x amount of characters
        private void consumeSomeChars(int x)
        {
            for(int i=0;i< x;i++)
            {
                consumeChar();
            }
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
        /// called after consuming whitespace after parsing a tag or a compelete attribute dictionary entry
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

        // Evertying below this is for parsing CSS


        /// <summary>
        /// Parses a single line as a declaration in a CSS 
        /// </summary>
        /// <returns></returns>
        private CSSManager.Declaration parseDeclaration()
        {
            StringBuilder strBuilder = new StringBuilder();
            CSSManager.Declaration decl = new CSSManager.Declaration();

            // parseing it here
            // declaration grammar is such that [a-zA-Z-_]*
            // basically an identifier
            consumeWhitespace();
            decl.name = parseIdentifier();
            assert(consumeChar(), ':');
            decl.value = parseValue();

            return decl;
        }

        private CSSManager.Value parseValue()
        {
            CSSManager.Value val = new CSSManager.Value();

            if (Char.IsDigit(nextChar()))
            {
                val.type = CSSManager.ValueType.Length;
                val.len = parseStringToFloat();
                consumeWhitespace();
                val.unit = parseUnit();
            }
            else if(Char.IsLetter(nextChar()))
            {
                val.type = CSSManager.ValueType.Keyword;
                val.keyword = parseIdentifier();
            }
            else if(nextChar() == '#')
            {
                val.color = parseColor();
                val.type = CSSManager.ValueType.Color;
            }
            else
            {
                throw new Exception(String.Format("Value {0} cannot be the beginning of a value.\n", nextChar()));
            }
            return val;
        }

        /// <summary>
        /// parses a part of string that has a grammer consisitng of 
        /// [0-9]*.?[0-9]* and returns it as a float
        /// </summary>
        /// <returns>A float parsed from a string</returns>
        private float parseStringToFloat()
        {
            StringBuilder strBuilder = new StringBuilder();
            float flo = 0;
            strBuilder.Append(parseNumbers());
            if (!endOfFile() && nextChar() == '.')
            {
                strBuilder.Append(consumeChar());
                strBuilder.Append(parseNumbers());
            }
            flo = (float)Double.Parse(strBuilder.ToString());
            return flo;
        }

        private CSSManager.Unit parseUnit()
        {
            CSSManager.Unit u = CSSManager.Unit.Px;
            if (startsWith("px") || startsWith("Px") || startsWith("PX") || startsWith("pX"))
            {
                consumeSomeChars(2);
                u = CSSManager.Unit.Px;
            }
            else if(startsWith("p"))
            {
                consumeSomeChars(1);
                u = CSSManager.Unit.P;
            }
            return u;
        }

        // returns a section of string that is only numbers
        private string parseNumbers()
        {
            StringBuilder strBuilder = new StringBuilder();

            while(!endOfFile() && Char.IsDigit(nextChar()))
            {
                strBuilder.Append(consumeChar());
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Parses a colors for a css value
        /// </summary>
        /// <returns></returns>
        private CSSManager.Color parseColor()
        {
            assert(consumeChar(), '#');
            CSSManager.Color col = new CSSManager.Color();
            string hexColor = parseIdentifier();
            Console.Write(hexColor + "\n");

            byte[] rgb = stringToByteArray(hexColor);
            col.r = rgb[0];     // first byte is red
            col.g = rgb[1];     // second byte is green
            col.b = rgb[2];     // thrid byte is blue
            col.a = 255;        // aplha

            return col;
        }
      
        /// <summary>
        /// using linq to get the bytes out of the hex color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>An array of bytes converted from hex</returns>
        private byte[] stringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }
        

        /// <summary>
        /// Parses the selector section of a CSS file also does so
        /// very terribly.
        /// </summary>
        /// <TODO>
        /// Add error checking so that garabage selectors get thrown out or are
        /// properly communitcated to the user
        /// </TODO>
        /// <returns>A single simple selector with all its values parsed from a CSS file</returns>
        private CSSManager.SimpleSelector parseSimpleSelector()
        {
            CSSManager.SimpleSelector selector = new CSSManager.SimpleSelector();
            // set selector.classes to this after everything is done
            Queue<string> classQue = new Queue<string>();
            bool keepEating = true;                         // gets set to false when there is nothing left to parse for the selector

            while(!endOfFile() && keepEating)
            {
                if (nextChar() == '#')                      // parse ID
                {
                    consumeChar();
                    selector.id = parseIdentifier();   
                }
                else if (nextChar() == '.')                 // parse a class and enqueue to classQue
                {
                    consumeChar();
                    classQue.Enqueue(parseIdentifier());
                }
                else if (nextChar() == '*')                 // don't do anything and move on
                {
                    consumeChar();

                }
                else if(validIdentifierChar(nextChar()))    // parse the Tag Name
                {
                    selector.tagName = parseIdentifier();
                }
                else
                {
                    keepEating = false;
                }
            }
            selector.classes = classQue.ToArray();
            return selector;
        }

        /// <summary> 
        /// parsing an identifier
        /// identifiers can start with '-' or '_' and have them in the name as well
        /// the grammar as follows -?[_a-zA-Z]+[_a-zA-Z0-9-]*
        /// if it starts with - then the second char MUST be an alpha.
        /// </summary>
        /// <returns>Returns a valid selector name</returns>
        private string parseIdentifier()
        {
            StringBuilder strBuilder = new StringBuilder();
            while (!endOfFile() && (Char.IsLetterOrDigit(nextChar()) || nextChar() == '-' || nextChar() == '_'))
            {
                strBuilder.Append(consumeChar());
            }
            return strBuilder.ToString();
        }

        private bool validIdentifierChar(char c)
        {
            bool valid = false;
            if (Char.IsLetter(c))
                valid = true;
            return valid;
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