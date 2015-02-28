using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTemplate
{
    class Scanner
    {
        class Tag
        {
            public string Key { get { return key.ToString(); } }
            public int Len { get; set; }

            StringBuilder key;

            public Tag()
            {
                key = new StringBuilder();
                Len = 0;
            }

            public void Eat(char c)
            {
                key.Append(c);
            }
        }

        public Dictionary<string, string> Scan(string format, string line)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            string[] line_tokens = line.Split();
            string[] format_tokens = format.Split();
            int ldx = 0;
            int fdx = 0;

            while (fdx < format_tokens.Length && ldx < line_tokens.Length)
            {
                if (format_tokens[fdx][0] == '{')
                {
                    // Found a tag.

                    Tag tag = parseTag(format_tokens[fdx]);
                    if (tag != null)
                    {
                        string value = parseValue(line_tokens[ldx], tag);
                        values.Add(tag.Key, value);
                    }

                    ++fdx;
                    ++ldx;
                }
                else if (format_tokens[fdx][0] == '*')
                {
                    // Look ahead at the next format token and if the input format token matches the current line
                    // token move to the next format token otherwise to the next line token.

                    if (fdx + 1 == format_tokens.Length)
                    {
                        break;
                    }
                    else if (format_tokens[fdx + 1] == line_tokens[ldx])
                    {
                        ++fdx;
                    }
                    else
                    {
                        ++ldx;
                    }
                }
                else if (format_tokens[fdx] != line_tokens[ldx])
                {
                    // Stop the scan when the format token doesn't match the line token.
                    break;
                }
                else
                {
                    // Move forward in
                    ++fdx;
                    ++ldx;
                }
            }

            return values;
        }

        Tag parseTag(string format)
        {
            Tag tag = new Tag();
            int pos = 1;    // skip the opening bracket

            bool in_length = false;
            StringBuilder len_chars = new StringBuilder();

            while (true)
            {
                if (pos == format.Length)
                {
                    // End of the format string but no end of tag.
                    break;
                }
                else if (format[pos] == '}')
                {
                    // End of the tag.
                    if (in_length)
                    {
                        tag.Len = Convert.ToInt32(len_chars.ToString());
                    }

                    return tag;
                }
                else if (format[pos] == ':')
                {
                    in_length = true;
                }
                else
                {
                    if (!in_length)
                    {
                        tag.Eat(format[pos]);
                    }
                    else
                    {
                        len_chars.Append(format[pos]);
                    }
                }

                ++pos;
            }

            return null;
        }

        string parseValue(string line, Tag tag)
        {
            StringBuilder value = new StringBuilder();
            int pos = 0;

            if (tag.Len == 0)
            {
                return line;
            }

            while (true)
            {
                if (pos == line.Length)
                {
                    return value.ToString();
                }
                else if (tag.Len != 0 && tag.Len == value.Length)
                {
                    return value.ToString();
                }
                else
                {
                    value.Append(line[pos]);
                }

                ++pos;
            }

            throw new Exception("wtf");
        }
    }
}
