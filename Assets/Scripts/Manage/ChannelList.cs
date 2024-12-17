using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
    public class ChannelList
    {
        private string data;
        private int pointer = 0;
        public List<Channel> channels;
        public ChannelList(string rawJSON)
        {
            data = rawJSON;
            channels = new List<Channel>();
            Parse();
        }

        public string GetGameIDForChannelName(string name)
        {
            if (channels.Count == 0)
            {
                return "";
            }
            return channels.First(x => x.ChannelName.ToLower() == name.ToLower()).Data.gameId;
        }
        private bool Parse()
        {
            EatWhitespace();
            ConsumeSymbol('{');
            while (pointer < data.Length)
            {
                int sp = pointer;
                ConsumeChannel();
                EatWhitespace();
 
                //end of valid input
                if (Peek('}'))
                {
                    ConsumeSymbol('}');
                }

                if (pointer == data.Length)
                {
                    return true;
                }
                if (sp == pointer)
                {
                    //nothing consumed during loop!
                    return false;
                }
            }
            return pointer == data.Length;
        }

        private void ConsumeChannel()
        {
            var title = ConsumeString();
            ConsumeSymbol(':');
            var blob = ConsumeBlock();
            var c = new Channel(title,blob);
            channels.Add(c);
            //trailing comma
            if (Peek(','))
            {
                ConsumeSymbol(',');
            }

        }

        private bool Peek(char match)
        {
            if (pointer < data.Length)
            {
                return data[pointer] == match;
            }

            return false;
        }

        private string ConsumeBlock()
        {
            int depth = 0;
            int start = pointer;
            ConsumeSymbol('{');
            depth++;
            bool inString = false;
            while (true)
            {
                if (pointer >= data.Length)
                {
                    throw new FormatException();
                }

                var c = data[pointer];
                if (inString)
                {
                    if (c == '"')
                    {
                        inString = false;
                    }

                    pointer++;
                    continue;
                }
                if (c == '\\')
                {
                    //skip next.
                    pointer++;
                    continue;
                }else if (c == '{'){
                    depth++;
                }else if (c == '}')
                {
                    depth--;
                }else if (c == '"')
                {
                    inString = true;
                }
                pointer++;
                if (depth == 0)
                {
                    break;
                }
            }
            return data.Substring(start, pointer - start);
        }

        private string ConsumeString()
        {
            ConsumeSymbol('"');
            var end = data.IndexOf('"', pointer);
            var s = data.Substring(pointer, end - pointer);
            pointer = end + 1;
            return s;
        }

        private void EatWhitespace()
        {
            while(pointer < data.Length && char.IsWhiteSpace(data[pointer]))
            {
                pointer++;
            }
        }

        private void ConsumeSymbol(char symbol)
        {
            if (data[pointer] == symbol)
            {
                pointer++;
                return;
            }
            else
            {
                throw new Exception($"Can't parse {symbol} at position {pointer}");
            }
        }
    }