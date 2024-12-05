using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeDetective;

namespace MimeTypeIdentify
{
    internal class Program
    {
        /// <summary>
        /// This application determines the MIME-type of a supplied file.
        /// It leverages a MIME-type detection NuGet library (MimeDetective), available under 
        /// the MIT license, to read the data from a file and determine what format the file is
        /// based on this data.
        /// 
        /// This is done by reading a file from disk in this example application, but could also
        /// be done entirely by using data from within a memory stream.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MimeTypeIdentify <filepath>");
                Environment.Exit(0);
            }

            string filePath = args[0];
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("Error: file {0} was not found!", filePath);
                Environment.Exit(1);
            }
            
            // Initialize the MimeDetective inspector.
            // The default definitions set is free of use, but more detailed sets are available
            // under a commercial license.  The default set has most common file types though,
            // including PNG, JPG, GIF, PDF, DOC/DOCX, XLS/XLSX, and more.
            ContentInspector mimeTypeInspector = new MimeDetective.ContentInspectorBuilder()
            {
                Definitions = MimeDetective.Definitions.Default.All()
            }.Build();

            // You can pass in the file to be inspected as a file path...
            var filePathResults = mimeTypeInspector.Inspect(filePath);
            if (filePathResults.FirstOrDefault() == null)
            {
                Console.WriteLine("Error: unable to determine file type of {0}!", filePath);
            }
            else
            {
                Console.WriteLine("File at path: {0}", filePathResults.FirstOrDefault().Definition.File.MimeType);
            }

            // ... or you can pass in a byte array...
            byte[] byteArray = File.ReadAllBytes(filePath);
            var byteArrayResults = mimeTypeInspector.Inspect(byteArray);
            if (byteArrayResults.FirstOrDefault() == null)
            {
                Console.WriteLine("Error: unable to determine file type of byte array!");
            }
            else
            {
                Console.WriteLine("File in byte array: {0}", byteArrayResults.FirstOrDefault().Definition.File.MimeType);
            }

            // ... or a MemoryStream.
            using (MemoryStream memStream = new MemoryStream(byteArray, 0, byteArray.Length, false, true))
            {
                var memStreamResults = mimeTypeInspector.Inspect(memStream);
                if (memStreamResults.FirstOrDefault() == null)
                {
                    Console.WriteLine("Error: unable to determine file type of memory stream!");
                }
                else
                {
                    Console.WriteLine("File in memory stream: {0}", memStreamResults.FirstOrDefault().Definition.File.MimeType);
                }
            }

            // You can also compare what the MIME-type returned is, and whether it matches a specific target MIME-type.
            if (filePathResults.FirstOrDefault() != null)
            {
                if (filePathResults.FirstOrDefault().Definition.File.MimeType == MimeDetective.Definitions.Default.FileTypes.Documents.PDF().FirstOrDefault().File.MimeType)
                {
                    Console.WriteLine("File at {0} determined to be a PDF file.", filePath);
                }
                else
                {
                    Console.WriteLine("File at {0} determined to NOT BE a PDF file!", filePath);
                }
            }
        }
    }
}
