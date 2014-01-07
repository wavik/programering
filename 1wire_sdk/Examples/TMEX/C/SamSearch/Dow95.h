/* dow95.h */

#ifndef _DOW95_H_
#define _DOW95_H_

#ifndef SERIAL_DS2480
#define SERIAL_DS2480      0
#endif

#ifndef PARALLEL_DS1481         
#define PARALLEL_DS1481    1
#endif

// DS1413
#ifndef SERIAL_DS1413
#define SERIAL_DS1413      2
#endif

//MGP BEGIN
#ifndef USB_DS2490
#define USB_DS2490         4
#endif
//MGP END

typedef unsigned char  uchar;
typedef unsigned short ushort;
typedef unsigned int   uint;
typedef unsigned long  ulong;

#ifndef TRUE
#define TRUE 1
#define FALSE 0
#endif

#ifdef __cplusplus
extern "C" {
#endif

void  iBOverdriveOff(void);
void  FastSleep(DWORD);
uchar ToggleOverdrive(void);
uchar garbagebag(uchar *);// returns lastone for SACWD300.dll
uchar gndtest(void);
uchar iBOverdriveOn(void);
uchar iBDOWReset(void);
uchar iBDOWCheck(void);                 
uchar iBKeyOpen(void);
uchar iBKeyClose(void);
uchar iBSetup(uchar);
uchar iBNext(void);
uchar iBFirst(void);
uchar iBAccess(void);
uchar iBFastAccess(void);
uchar iBDataByte(uchar);
uchar iBDataBit(uchar);
uchar iBDataBlock(uchar *, int);
uchar *iBROMData(void);
uchar SetAdapterSpeed(ulong);
uchar SetAdapterType(uchar, char *);
uchar SetAdapter5VTime(uchar);
uchar Adapter5VPrime(void);
uchar Adapter5VCancel(void);
uchar iBStreamRS(uchar *,int,int);
uchar iBStream(uchar *,int);
uchar iBStrongAccess(void);
ulong GetAccessAPIVersion(void);
int Adapter5VActivate(ulong ustimeout);
HANDLE GetDriverHandle(void);

#ifdef __cplusplus

}
#endif

#endif /* _DOW95_H_ */

/* dow95.h */
