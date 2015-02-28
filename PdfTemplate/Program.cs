using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfTemplate
{
    class Program
    {
        class FormatLine
        {
            public int LineNo { get; set; }
            public string Format { get; set; }
        }

        static void Main(string[] args)
        {
            List<FormatLine> formatLines = readTemplate("C:\\Users\\josh\\Desktop\\template.txt");
            processPDF("C:\\Users\\josh\\Desktop\\pdfs\\George Evenue.pdf", formatLines);
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
                        string format = string.Join(" ", tokens, 1, tokens.Length-1);
                        formatLines.Add(new FormatLine { LineNo = line_no, Format = format });
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
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);
                    string[] lines = text.Split('\n');

                    foreach (FormatLine line in formatLines)
                    {
                        Scanner scanner = new Scanner();
                        Dictionary<string,string> values = scanner.Scan(line.Format, lines[line.LineNo]);

                        foreach (string key in values.Keys)
                        {
                            Console.WriteLine(string.Format("{0} = {1}", key, values[key]));
                        }
                    }                    
                }
            }
        }
    }
}
