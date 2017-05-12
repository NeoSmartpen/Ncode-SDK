#pragma once

#include <iostream>
#include <string>

struct RequestInfo
{
	int section;
	int owner;
	int book;
	int page;
	int width;
	int height;
	int dpi;
	bool isBold;
};

struct ResponseData
{
	int errorCode;				// errorCode
	std::string returnString;   // return value or error message
	std::string versionAPI;     // Lambda API version
	unsigned char *data;        // NCode appkeyResponseData
	int dataLength;
};

struct TicketInfo
{
	std::string account;
	int section;
	int owner;
	int bookSize;
	int bookStart;
	int pageStart;
	int pageSize;
	int period;

	std::string extraInfo;
	int state;
	int type;
	int pageLeft;
};


class CNCodeSDK
{
public:
	CNCodeSDK();
	~CNCodeSDK();


public:
	void Init(const std::string &key);
	TicketInfo* GetTickets();
	int GetTicketCount();
	bool SetPageInfo(int ticketIndex, int bookOffset, int pageOffset);
	bool SetPageInfo(int section, int owner, int book, int page);
	bool SetPageSize(int width, int height, int dpi);
	bool SetPageSizeFromPaperName(std::string paper, int dpi, bool isLandscape = false);
	void SetBold(bool isBold);
	bool GenerateNCode(ResponseData *ncodeData);

	RequestInfo* GetRequestInfo();
	std::string GetVersion()	{ return version; }
	std::string GetLastError()	{ return lastErrorMsg;	}
	int GetErrorCode()			{ return lastErrorCode; }

private:
	bool CheckCodeInfoRange(int s, int o, int b, int p);
	bool CheckImageSize(int s, int w, int h, int dpi);
	bool GetReturnValue(ResponseData *appkeyResponse, std::string outputStr);
	std::string RequestLambda(std::string appkeyResponseData, std::string url);
	unsigned char* GenNCodeBitmapData(unsigned char *data);
	void DrawNCodeAtByteArray(unsigned char *buf, int x, int y, int value);

private:
	std::string appKey;
	TicketInfo *tickets;
	int ticketCount;

	bool isKeySet;
	bool isTicketLoaded;
	bool isSetSection;
	bool isReady;

	RequestInfo requestInfo;

	std::string version;
	std::string lastErrorMsg;
	int lastErrorCode;

};

