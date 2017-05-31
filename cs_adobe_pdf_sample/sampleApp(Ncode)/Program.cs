using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NeoLABNcodeSDK;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using PDFLibAgent;      // wrapper class of Datalogics' Adobe PDF library


/*
 * Generating Ncode image & Ncoded PDF sample code 
 * This is first version of PDF sample.
 * This code can be updated from user's request.
 */

namespace sampleApp_Ncode_cs_adobe_
{
    class Program
    {
        CNcodeSDK sdk = null;
        PDFControl lib = null;
        static string ncodeImageFilename = "";



        public Program()
        {
            Console.WriteLine("///////////////////");
            Console.WriteLine("  Creating Ncode");
            Console.WriteLine("///////////////////");
            Console.WriteLine();
            Console.WriteLine("1) Initializing with app key");
            Console.WriteLine();

            // This is sample app key for testing.
            // If you have your app key, enter here.
            string appKey = "855422da920c239d1349beb455d46364f5cb485cda83a9f61938fbedb75a075e";

            sdk = new CNcodeSDK();
            if (sdk.Init(appKey))
            {
                Request_and_GenerateNcode();
            }



            Console.WriteLine("////////////////////////");
            Console.WriteLine("  Creating Ncoded PDF");
            Console.WriteLine("////////////////////////");
            Console.WriteLine();
            Console.WriteLine("1) Initializing with lib key");
            Console.WriteLine();

            // Input your Adobe pdf library key.
            // If you don't have it, you should perchase it.
            string workingDir = Directory.GetCurrentDirectory();
            string libKey = "Input your library key!";
                                                                            
            lib = new PDFControl();
            if (lib.init(workingDir, libKey))
            {
                ncodeImageFilename = workingDir + @"\" + ncodeImageFilename;
                RemoveK_and_AddNcode(workingDir + @"\input.pdf", workingDir + @"\output.pdf", new string[] { ncodeImageFilename });
            }
            lib.libraryCleanUp();



            Console.WriteLine();
            Console.WriteLine("-- Complete --");
            Console.ReadLine();
        }


        static void Main(string[] args)
        {
            new Program();
        }


        /// <summary>
        /// Request Ncode data and generate Ncode image.
        /// </summary>
        void Request_and_GenerateNcode()
        {
            Console.WriteLine("2) Getting tickets list");
            Console.WriteLine();
            CNcodeSDK.TicketInfo[] tickets;
            int getTicketRet = sdk.GetTickets(out tickets);
            if (getTicketRet >= 100)
            {
                // error code
                // 100 : no appkey
                // 101 : request app key API error
                // 102 : request ticket API error
                // 103 : no tickets
                Console.WriteLine("   Ticket error : " + getTicketRet.ToString());
                Console.WriteLine("   Error message : " + sdk.GetLastError());
                Console.ReadLine();
                return;
            }
            Console.WriteLine("   Found " + getTicketRet.ToString() + " ticket(s)");
            Console.WriteLine();

            for (int i = 0; i < tickets.Length; ++i)
            {
                Console.WriteLine("   Ticket[" + i.ToString() + "]");
                Console.WriteLine("   Section : " + tickets[i].section.ToString());
                Console.WriteLine("   Owner   : " + tickets[i].owner.ToString());
                Console.WriteLine("   Book    : " + tickets[i].bookStart.ToString() + "~" + (tickets[i].bookStart + tickets[i].bookSize - 1).ToString());
                Console.WriteLine("   Page    : " + tickets[i].pageStart.ToString() + "~" + (tickets[i].pageStart + tickets[i].pageSize - 1).ToString());
                if (tickets[i].type == 1)
                    Console.WriteLine("   Reusable : True");
                else
                    Console.WriteLine("   Reusable : False");
                Console.WriteLine();
            }



            Console.WriteLine("3) Choose ticket and set page for generating");
            Console.WriteLine();

            int ticketIndex = 0;
            int bookOffset = 2;
            int pageOffset = 15;
            CNcodeSDK.TicketInfo pageInfo = sdk.SetStartPageByOffset(tickets[ticketIndex], bookOffset, pageOffset);

            if (pageInfo == null)
            {
                Console.WriteLine("   Ticket range error");
                Console.WriteLine("   Error message : " + sdk.GetLastError());
                Console.ReadLine();
                return;
            }
            Console.WriteLine("   Selected ticket index : " + ticketIndex.ToString());
            Console.WriteLine("   Book offset : " + bookOffset.ToString());
            Console.WriteLine("   Page offset : " + pageOffset.ToString());
            Console.WriteLine();



            Console.WriteLine("3-1) Get size from paper size name");
            Console.WriteLine();
            string paperSizeName = "A4";
            int dpi = 600;
            Size imgSize = sdk.GetImageSizeFromPaperSize(paperSizeName, dpi, false);
            Console.WriteLine("   Paper Size (" + paperSizeName + ") : " + "(" + imgSize.Width.ToString() + ", " + imgSize.Height.ToString() + ")");
            Console.WriteLine();



            Console.WriteLine("4) Generating Ncode data");
            Console.WriteLine();
            CNcodeSDK.NcodeData codeData = sdk.GenerateNcode(
                pageInfo,               // page information from tickets
                imgSize.Width,          // width (pixel)
                imgSize.Height,         // height (pixel)
                dpi,                    // dpi (600 or 1200)
                false);                 // is bold

            if (codeData.errorCode != 0)
            {
                Console.WriteLine("   Generate Ncode error : " + codeData.errorCode.ToString());
                Console.WriteLine("   Error message : " + sdk.GetLastError());
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine("   Generating Ncode complete");
                Console.WriteLine("   Section : " + codeData.section.ToString());
                Console.WriteLine("   Owner : " + codeData.owner.ToString());
                Console.WriteLine("   Book : " + codeData.book.ToString());
                Console.WriteLine("   Page : " + codeData.page.ToString());
                Console.WriteLine("   Image size : (" + codeData.imgWidth.ToString() + "," + codeData.imgHeight.ToString() + ")");
                Console.WriteLine();
            }



            Console.WriteLine("5) Saving Ncode image file");
            Console.WriteLine();
            string outputFilename =
                pageInfo.section.ToString() + "_" +
                pageInfo.owner.ToString() + "_" +
                pageInfo.bookStart.ToString() + "_" +
                pageInfo.pageStart.ToString() + ".png";


            codeData.image.Save(outputFilename, System.Drawing.Imaging.ImageFormat.Png);
            ncodeImageFilename = outputFilename;

            Console.WriteLine("6) Ncode created");
            Console.WriteLine();
        }


        /// <summary>
        /// 1) Remove carbon black field(K) from CMYK colorspace from PDF file.
        /// 2) Make Ncoded PDF with Ncode image files.
        /// </summary>
        /// <param name="inputPdfFilename"></param>
        /// <param name="outputPdfFilename"></param>
        /// <param name="ncodeIamgeFilenames"></param>
        void RemoveK_and_AddNcode(string inputPdfFilename, string outputPdfFilename, string[] ncodeIamgeFilenames)
        {
            Console.WriteLine("2) Remove carbon black field(K) from CMYK colorspace from PDF file");
            Console.WriteLine();
            IPDFDocument doc = lib.openDocument(inputPdfFilename);
            IPDFDocument newDoc = lib.copyDocumentOnlyPageStructure(doc);

            for (int i = 0; i < newDoc.getPageCount(); i++)
            {
                using (IPDFPage newPage = newDoc.getPageObj(i))
                {
                    newPage.convertColorSpaceToCMY(doc);
                }
            }



            Console.WriteLine("3) Make Ncoded PDF with Ncode image files.");
            Console.WriteLine();
            /////////////////////////////////////////////////////////////////////
            // caution : This code will not work unless Ncode image's bpp is 1.
            /////////////////////////////////////////////////////////////////////
            System.IO.MemoryStream[] ms = new System.IO.MemoryStream[ncodeIamgeFilenames.Length];

            for (int i = 0; i < ncodeIamgeFilenames.Length; ++i)
            {
                ms[i] = new System.IO.MemoryStream();
                System.Drawing.Image ss = System.Drawing.Image.FromFile(ncodeIamgeFilenames[i]);
                ss.Save(ms[i], System.Drawing.Imaging.ImageFormat.Tiff);
            }

            for (int j = 0; j < newDoc.getPageCount(); ++j)
            {
                using (var page = newDoc.getPageObj(j))
                {
                    double x0, y0, x1, y1;
                    x0 = y0 = x1 = y1 = 0;

                    page.getPageMediaBox(ref x0, ref y0, ref x1, ref y1, true);
                    page.addImageContentOver_usingStream(ms[j], true, x0, y0, x1, y1);
                }

                ms[j].Dispose();
            }

            newDoc.saveDocumentAs(outputPdfFilename);
            newDoc.closeDocument();
        }
    }
}
