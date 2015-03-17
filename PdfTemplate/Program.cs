using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfTemplateLib;


namespace PdfTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Processor processor = new Processor();
            List<Result> results = processor.ProcessPDF("C:\\Users\\Josh\\Desktop\\pdfs\\headliner\\date\\time\\pdf.pdf", "C:\\Users\\Josh\\Desktop\\pdfs\\headliner\\date\\time\\template.txt");

            foreach (Result result in results)
            {
                Console.WriteLine("Section: {0}, Row: {1}, Seat: {2}, Barcode: {3}", 
                                  result.Section, 
                                  result.Row, 
                                  result.Seat, 
                                  result.Barcode);
            }
             */

            string path = "C:\\Users\\Josh\\Desktop\\pdfs";
            string[] files = Directory.GetFiles(path,"*.pdf",SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string templatePath = Path.GetDirectoryName(file) + "\\template.txt";
                if (File.Exists(templatePath))
                {
                    Processor processor = new Processor();
                    List<Result> results = processor.ProcessPDF(file, templatePath);

                    Console.WriteLine("--------------------");
                    Console.WriteLine(file);
                    foreach (Result result in results)
                    {
                        Console.WriteLine("Section: {0}, Row: {1}, Seat: {2}, Barcode: {3}, Confirmation: {4}",
                                          result.Section,
                                          result.Row,
                                          result.Seat,
                                          result.Barcode,
                                          result.ConfNumber);
                    }                    
                }
            }
        }
    }
}
