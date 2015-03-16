using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfTemplate
{
    class Program
    {
        class FormatLine
        {
            public int LineNo { get; set; }
            public string Pattern { get; set; }
        }

        static void Main(string[] args)
        {
            List<FormatLine> formatLines = readTemplate("C:\\Users\\Josh\\Desktop\\pdfs\\headliner\\date\\time\\template.txt");
            processPDF("C:\\Users\\Josh\\Desktop\\pdfs\\headliner\\date\\time\\pdf.pdf", formatLines);
        }

        static List<FormatLine> readTemplate (string path)
        {
            List<FormatLine> formatLines = new List<FormatLine>();
            string[] lines = File.ReadAllLines(path);

            foreach (string l in lines)
            {
                if (l.Length == 0 || l[0] == '#')
                {
                    continue;
                }

                string[] tokens = l.Split();
                int line_no;

                // This is a line definition.
                if (int.TryParse(tokens[0], out line_no))
                {
                    if (tokens.Length-1 > 0)
                    {
                        string pattern = string.Join(" ", tokens, 1, tokens.Length-1);
                        formatLines.Add(new FormatLine { LineNo = line_no, Pattern = pattern });
                    }
                }
            }

            return formatLines;
        }

        static void processPDF(string pdfPath, List<FormatLine> formatLines)
        {
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    Console.WriteLine("------------------------------");
                    Console.WriteLine("Page {0}", i);

                    string text = PdfTextExtractor.GetTextFromPage(reader, i);
                    string[] lines = text.Split('\n');
                    Dictionary<string, string> values = new Dictionary<string,string>();

                    foreach (FormatLine line in formatLines)
                    {
                        Regex regex = new Regex(line.Pattern);
                        GroupCollection groups = regex.Match(lines[line.LineNo]).Groups;
                        foreach (string groupname in regex.GetGroupNames())
                        {
                            switch(groupname)
                            {
                                case "section":
                                case "row" :
                                case "seat" :
                                case "barcode" :
                                    values.Add(groupname, groups[groupname].Value);
                                    break;
                            }
                        }            
                    }

                    foreach(string key in values.Keys)
                    {
                        Console.WriteLine("{0} = {1}", key, values[key]);
                    }
                }
            }
        }
    }
}
