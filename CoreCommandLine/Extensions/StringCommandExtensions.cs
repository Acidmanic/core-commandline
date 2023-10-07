using System;
using System.Collections.Generic;

namespace CoreCommandLine.Extensions
{
    internal static class StringCommandExtensions
    {



        public static string[] SplitToArgs(this string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return new string[] { };
            }

            
            const int stateText=0;
            const int stateQuote=1;
            const int stateWhiteSpace=2;
            
            var state = stateWhiteSpace;
            
            var qtChar = '"';
            var segments = new List<string>();
            var buffer = "";
            
            var chars = line.ToCharArray();

            Action deliver = () =>
            {
                if (!string.IsNullOrEmpty(buffer))
                {
                    segments.Add(buffer);
                    buffer = "";
                }   
            };

            foreach (var c in chars)
            {

                if (state==stateText)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        deliver();
                        state = stateWhiteSpace;
                    }
                    else
                    {
                        buffer += c;
                    }
                }else if (state == stateQuote)
                {
                    if (c == qtChar)
                    {
                        deliver();
                        state = stateWhiteSpace;
                    }
                    else
                    {
                        buffer += c;
                    }
                }else if (state == stateWhiteSpace)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        if (c == '"' || c == '\'')
                        {
                            qtChar = c;

                            state = stateQuote;
                        }
                        else
                        {
                            state = stateText;
                            buffer += c;    
                        }
                    }
                }
            }

            return segments.ToArray();
        }
    }
}