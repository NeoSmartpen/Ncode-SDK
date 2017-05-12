#include "NCodeSDK.h"
#include "simplePng.h"
#include <iostream>
#include <string>


#if _WIN64 
	#if _DEBUG
	#pragma comment(lib, "NeoLABNCodeSDK_cpp_x64d.lib")
	#else
	#pragma comment(lib, "NeoLABNCodeSDK_cpp_x64.lib")
	#endif
#else
	#if _DEBUG
	#pragma comment(lib, "NeoLABNCodeSDK_cpp_x86d.lib")
	#else
	#pragma comment(lib, "NeoLABNCodeSDK_cpp_x86.lib")
	#endif
#endif


using namespace std;


void main()
{
	CNCodeSDK sdk;
	
	cout << "NCode SDK_cpp version : " << sdk.GetVersion();
	cout << "\n\n";


	cout << "1) Initializing with app key\n";


	// this is sample app key for testing
	sdk.Init("855422da920c239d1349beb455d46364f5cb485cda83a9f61938fbedb75a075e");


	cout << "\n";
	cout << "2) Getting tickets list\n";

	TicketInfo *tickets = sdk.GetTickets();

	if (sdk.GetErrorCode() != 0)
	{
		// error code
		// -100 : no appkey
		// -101 : request app key API error
		// -102 : request ticket API error
		// -103 : no tickets
		cout << "   Ticket error : " + to_string(sdk.GetErrorCode()) + "\n";
		cout << "   Error message : " + sdk.GetLastError() + "\n";
		getchar();
		return;
	}
	int ticketCount = sdk.GetTicketCount();

	cout << "   Found " + to_string(ticketCount) + " ticket(s)\n";

	for (int i = 0; i < ticketCount; ++i)
	{
		cout << "   Ticket[" + to_string(i) + "]\n";
		cout << "   Section : " + to_string(tickets[i].section) + "\n";
		cout << "   Owner   : " + to_string(tickets[i].owner) + "\n";
		cout << "   Book    : " + to_string(tickets[i].bookStart) + "~" + to_string(tickets[i].bookStart + tickets[i].bookSize - 1) + "\n";
		cout << "   Page    : " + to_string(tickets[i].pageStart) + "~" + to_string(tickets[i].pageStart + tickets[i].pageSize - 1) + "\n";
	}

	cout << "\n";
	cout << "3) Choose ticket and set page for generating\n";


	int ticketIndex = 0;
	int bookOffset = 2;
	int pageOffset = 15;

	if (!sdk.SetPageInfo(ticketIndex, bookOffset, pageOffset))
	{
		cout << "   Ticket range error\n";
		cout << "   Error message : " + sdk.GetLastError() + "\n";
		getchar();
		return;
	}
	cout << "   Selected ticket index : " + to_string(ticketIndex) + "\n";
	cout << "   Book offset : " + to_string(bookOffset) + "\n";
	cout << "   Page offset : " + to_string(pageOffset) + "\n";


	cout << "\n";
	cout << "3-1) Get size from paper size name\n";
	string paperSizeName = "A10";
	int dpi = 600;
	sdk.SetPageSizeFromPaperName(paperSizeName, dpi, true);
	cout << "   Paper Size (" + paperSizeName + ")\n";


	cout << "\n";
	cout << "4) Generating NCode data\n";
	ResponseData codeData;
	RequestInfo *requestInfo = sdk.GetRequestInfo();


	if (!sdk.GenerateNCode(&codeData))
	{
		cout << "   Generate NCode error : " + to_string(codeData.errorCode) + "\n";
		cout << "   Error message : " + sdk.GetLastError() + "\n";
		getchar();
		return;
	}
	else
	{
		cout << "   Generating NCode complete\n";
		cout << "   Section : " + to_string(requestInfo->section) + "\n";
		cout << "   Owner : " + to_string(requestInfo->owner) + "\n";
		cout << "   Book : " + to_string(requestInfo->book) + "\n";
		cout << "   Page : " + to_string(requestInfo->page) + "\n";
		cout << "   Image size : (" + to_string(requestInfo->width) + "," + to_string(requestInfo->height) + ")\n";
	}


	cout << "\n";
	cout << "5) Saving NCode image file\n";
	string outputFilename =
		to_string(requestInfo->section) + "_" +
		to_string(requestInfo->owner) + "_" +
		to_string(requestInfo->book) + "_" +
		to_string(requestInfo->page) + ".png";


	MakeSimplePng(outputFilename, requestInfo->width, requestInfo->height, codeData.data);

	cout << "\n";
	cout << "6) Complete\n";
	cout << "   press a key\n";
	getchar();

	return;
}