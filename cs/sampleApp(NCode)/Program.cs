using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NeoLABNcodeSDK;
using System.IO;

namespace sampleApp_Ncode_
{
    class Program
    {
        static void Main(string[] args)
        {
            Generate_Ncode_With_Predeterminded_PageInfo();
        }

        static void Generate_Ncode_With_Predeterminded_PageInfo()
        {
            CNcodeSDK sdk = new CNcodeSDK();


            Console.WriteLine("1) Initializing with app key");
            Console.WriteLine();
            
            // this is sample app key for testing
            sdk.Init("855422da920c239d1349beb455d46364f5cb485cda83a9f61938fbedb75a075e");



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
            string paperSizeName = "A6";
            int dpi = 600;
            Size imgSize = sdk.GetImageSizeFromPaperSize(paperSizeName, dpi, true);
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

            codeData.image.Save(outputFilename);



            Console.WriteLine("6) Complete");
            Console.WriteLine("   press a key");
            Console.ReadLine();
        }
    }
}
