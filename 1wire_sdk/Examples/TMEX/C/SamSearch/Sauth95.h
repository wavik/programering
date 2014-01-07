// sauth95.h
#ifndef _SAUTH95_H_
#define _SAUTH95_H_
/////////////////////////////////// defines ////////////////////////////////////
#include <windows.h>
#include "dow95.h"


typedef struct _MYLARGE_INTEGER {
           DWORD LowPart;
           DWORD HighPart;
       } MYLARGE_INTEGER;
typedef MYLARGE_INTEGER *PMYLARGE_INTEGER;


#define DOWRESET        1
#define DOWBIT          2
#define DOWBYTE         3
#define DOWTOGGLEOD     4
#define DOWCHECKBSY     5
#define DOWTOGGLEPASS   6
#define CHECK_OVERDRIVE 0x22
////////////////////////////////////////////////////////////////////////////////
#endif // _SAUTH95_H_

