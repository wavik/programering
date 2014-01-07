/*****************************************************************************
 *
 *    Adapter.h                                      { Ver 0.01 10/25/96 }
 *
 ****************************************************************************/

#ifndef SERIAL_DS2480
#define SERIAL_DS2480      0
#endif 

#ifndef PARALLEL_DS1481
#define PARALLEL_DS1481    1
#endif

#ifndef SERIAL_DS1413
#define SERIAL_DS1413      2
#endif

#ifndef USB_DS2490
#define USB_DS2490         4
#endif

#ifndef AUTO_RESOURCE_RELEASE
#define AUTO_RESOURCE_RELEASE    0x80
#endif

#define PARMSET_infinite   0x0E

#define MAX_SERIAL_PORTS   4 
#define MAX_PARALLEL_PORTS 3
#ifdef WIN32
#define MAX_USB_PORTS      15 
#else
#define MAX_USB_PORTS      0
#endif// WIN32
#define MAX_ADAPTER_NUM    MAX_SERIAL_PORTS +  MAX_PARALLEL_PORTS + MAX_USB_PORTS 

#define MAX_DEV_NAME_LEN   30

typedef enum
{
   SearialAdapter,
   ParallelAdapter,
   USBAdapter,
   LastAdapter
}
ADAPTERLIST;

typedef struct _ADAPTERINFO
{
   BYTE Type;
   BYTE Num;
   char DevString[MAX_DEV_NAME_LEN];
}
ADAPTERINFO, *PADAPTERINFO, FAR *LPADAPTERINFO, NEAR *NPADAPTERINFO;

typedef struct _IBHOST
{
   BYTE          AdapterCount;
   LPADAPTERINFO lpAdapter[MAX_ADAPTER_NUM];
}
IBHOST, *PIBHOST, FAR *LPIBHOST, NEAR *NPIBHOST;

typedef struct _IBADDR
{
   BYTE        ROMData[8];
   ADAPTERINFO iBPort;
}
IBADDR, *PIBADDR, FAR *LPIBADDR, NEAR *NPIBADDR;

