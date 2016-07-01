using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Dom data struct for html. I'm trying to write my own simple web browser
/// we'll see how that goes
/// </summary>

namespace ToyBrowser.src
{
    class Dom
    {
        public enum NodeType
        {
            Text, Element
        };

        internal class Node
        {
            protected NodeType nodeType;

            protected Node[] children;                        // will be null whenever node type is Text 
            protected string text;                            // if nodeType is text then this will be non-null
            protected string tag;                             // non empty when nodeType is Element
            protected Dictionary<string, string> attributes;  // also non null whenever nodeType is Element

            public Node()
            {
                throw new Exception("Don't use this constructor");
            }

            public Node(NodeType t, string text, string tag, Dictionary<string, string> attr, Node[] children)
            {
                this.nodeType = t;
                if (t == NodeType.Text)
                {
                    this.text = text;
                    this.tag = null;
                    this.attributes = null;
                    this.children = null;
                }
                else
                {
                    this.text = null;
                    this.tag = tag;
                    this.attributes = attr;
                    this.children = children;
                }
            }

            public override string ToString()
            {
                StringBuilder strBuilder = new StringBuilder();
                int tabber = 0;

                this.addToStringBuilder(strBuilder, tabber);

                return strBuilder.ToString();
            }

            public void addToStringBuilder(StringBuilder strBuilder, int tabber)
            {
                // could also try using switch but there's only two nodetype right so whatever
                if (nodeType == NodeType.Element)
                {
                    StringBuilder closer = new StringBuilder();
                    for (int i = 0; i < tabber; i++)
                    {
                        strBuilder.Append("\t");
                    }

                    strBuilder.Append("<");
                    strBuilder.Append(tag);
                    // add in attributes here later
                    foreach(KeyValuePair<string, string> kvp in attributes)
                    {
                        strBuilder.Append(' ');
                        strBuilder.Append(kvp.Key);
                        strBuilder.Append('=');
                        strBuilder.Append('"');
                        strBuilder.Append(kvp.Value);
                        strBuilder.Append('"');
                    }
                    strBuilder.Append(">");

                    closer.Append("\n");
                    // adding this later
                    for (int i = 0; i < tabber; i++)
                    {
                        closer.Append("\t");
                    }
                    closer.Append("<\\");
                    closer.Append(tag);
                    closer.Append(">");

                    foreach (Node c in children)
                    {
                        strBuilder.Append("\n");
                        c.addToStringBuilder(strBuilder, tabber + 1);
                    }

                    strBuilder.Append(closer.ToString());
                }
                else if (nodeType == NodeType.Text)         // redundant but makes it easier for reading i guess
                {
                    for (int i = 0; i < tabber; i++)
                    {
                        strBuilder.Append("\t");
                    }
                    strBuilder.Append(text);
                }
                else
                {
                    // do nothing!
                }
            }
        }

        // static since I'll be manipulating Node and not class Dom
        public static Node createText(string text)
        {
            return new Node(NodeType.Text, text, null, null, null);
        }

        public static Node createElement(string tag, Dictionary<string, string> attr, Node[] children)
        {
            return new Node(NodeType.Element, null, tag, attr, children);
        }
    }
}