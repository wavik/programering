/*---------------------------------------------------------------------------
 * Copyright (C) 2004 Maxim Integrated Products, All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY,  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL MAXIM INTEGRATED PRODUCTS BE LIABLE FOR ANY CLAIM, DAMAGES
 * OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * Except as contained in this notice, the name of Maxim Integrated Products
 * shall not be used except as stated in the Maxim Integrated Products
 * Branding Policy.
 *---------------------------------------------------------------------------
 */
using System;
using System.Runtime.InteropServices;

namespace DalSemi.OneWire.Adapter
{
   internal enum TMEXPortType:int
   {
      PassiveSerialPort = 1,
      ParallelPort = 2,
      SerialPort = 5,
      USBPort = 6
   }

   internal class TMEXLibAdapter:PortAdapter
   {
      #region Fields
      private TMEX TMEXLibrary;
      private System.Text.StringBuilder mainVersionBuffer
         = new System.Text.StringBuilder(SIZE_VERSION);

      private System.Text.StringBuilder typeVersionBuffer
         = new System.Text.StringBuilder(SIZE_VERSION);

      private bool[] adapterSpecFeatures;
      private System.String adapterSpecDescription;

      private byte[] stateBuffer = new byte[SIZE_STATE];

      private TMEXPortType portType = TMEXPortType.SerialPort;

      private int portNum = - 1;
      private int sessionHandle = - 1;
      private bool inExclusive = false;
      private OWSpeed speed = OWSpeed.SPEED_REGULAR;
      #endregion

      #region Constants
      /// <summary>token indexes into version string </summary>
      private const int TOKEN_ABRV = 0;
      private const int TOKEN_DEV = 1;
      private const int TOKEN_VER = 2;
      private const int TOKEN_DATE = 3;
      private const int TOKEN_TAIL = 255;
      /// <summary>constant for state buffer size </summary>
      private const int SIZE_STATE = 5120;
      /// <summary>constant for size of version string </summary>
      private const int SIZE_VERSION = 256;
      /// <summary>constants for uninitialized ports </summary>
      private const int EMPTY_NEW = - 2;
      private const int EMPTY_FREED = - 1;
      #endregion

      #region Constructors and Destructors
      /// <summary> Constructs a default adapter
      ///
      /// </summary>
      /// <throws>  ClassNotFoundException </throws>
      public TMEXLibAdapter()
      {
         TMEXLibrary = new TMEX();
         // attempt to set the portType, will throw exception if does not exist
         //if (!setPortType_Native(getInfo(), portType))
         if (!setTMEXPortType(portType))
         {
            throw new AdapterException("TMEX adapter type does not exist");
         }
      }

      /// <summary> Constructs with a specified port type
      ///
      ///
      /// </summary>
      /// <param name="">newPortType
      /// </param>
      /// <throws>  ClassNotFoundException </throws>
      public TMEXLibAdapter(TMEXPortType newPortType)
      {
         TMEXLibrary = new TMEX();
         // attempt to set the portType, will throw exception if does not exist
         //if (!setPortType_Native(getInfo(), portType))
         if (!setTMEXPortType(newPortType))
         {
            throw new AdapterException("TMEX adapter type does not exist");
         }
      }

      /// <summary> Finalize to Cleanup native</summary>
      ~TMEXLibAdapter()
      {
         Dispose(false);
      }

      public override void Dispose()
      {
         Dispose(true);
      }

      protected override void Dispose(bool Disposing)
      {
         // check for opened port
         if ((portNum >= 0) && (portType >= 0))
         {
            // get a session
            if (get_session())
            {
               // clean up open port and sessions
               TMEXLibrary.TMClose(sessionHandle);

               // release the session (forced, even if in exclusive)
               TMEXLibrary.TMEndSession(sessionHandle);
               sessionHandle = 0;
               inExclusive = false;
            }
         }

         // set flag to indicate this port is now free
         portNum = EMPTY_FREED;
      }

      public override string AdapterName
      {
         get
         {
            // get the adapter name from the version string
            return "{" + getToken(typeVersionBuffer.ToString(), TOKEN_DEV) + "}";
         }
      }

      public override string PortName
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            // get the abreviation from the version string
            string header = getToken(typeVersionBuffer.ToString(), TOKEN_ABRV);

            // create the head and port number combo
            // Change COMU to COM
            if (header.Equals("COMU"))
               header = "COM";

            return header + portNum;
         }
      }

      public override System.Collections.IList PortNames
      {
         get
         {
            System.Collections.ArrayList portVector = new System.Collections.ArrayList(16);
            String header = getToken(typeVersionBuffer.ToString(),TOKEN_ABRV);

            if(header.Equals("COMU"))
               header = "COM";

            for (int i = 0; i < 16; i++)
               portVector.Add(header + i);

            return (portVector);
         }
      }

      public override System.String PortTypeDescription
      {
         get
         {
            // get the abreviation from the version string
            string abrv = getToken(typeVersionBuffer.ToString(), TOKEN_ABRV);

            // Change COMU to COM
            if (abrv.Equals("COMU"))
               abrv = "COM";

            return abrv + " (native)";
         }
      }

      /// <summary> Select the DotNet specified port type (0 to 15)  Use this
      /// method if the constructor with the PortType cannot be used.
      ///
      ///
      /// </summary>
      /// <param name="">newPortType
      /// </param>
      /// <returns>  true if port type valid.  Instance is only usable
      /// if this returns false.
      /// </returns>
      public bool setTMEXPortType(TMEXPortType newPortType)
      {
         // check if already have a session handle open on old port
         if (this.sessionHandle > 0)
            TMEXLibrary.TMEndSession(sessionHandle);

         this.sessionHandle = 0;
         this.inExclusive = false;

         // read the version strings
         TMEXLibrary.Get_Version(this.mainVersionBuffer);

         // will fail if not valid port type
         if (TMEXLibrary.TMGetTypeVersion((int)newPortType, this.typeVersionBuffer) > 0)
         {
            // set default port type
            portType = newPortType;
            return true;
         }
         return false;
      }

      /// <summary> Specify a platform appropriate port name for this adapter.  Note that
      /// even though the port has been selected, it's ownership may be relinquished
      /// if it is not currently held in a 'exclusive' block.  This class will then
      /// try to re-aquire the port when needed.  If the port cannot be re-aquired
      /// ehen the exception <code>PortInUseException</code> will be thrown.
      ///
      /// </summary>
      /// <param name="portName"> name of the target port, retrieved from
      /// getPortNames()
      ///
      /// </param>
      /// <returns> <code>true</code> if the port was aquired, <code>false</code>
      /// if the port is not available.
      ///
      /// </returns>
      /// <throws>  AdapterException If port does not exist, or unable to communicate with port. </throws>
      public override bool OpenPort(System.String portName)
      {
         int prtnum = 0, i;
         int EnhancedOptions = TMEX_Constants.SESSION_RSRC_RELEASE;
         bool rt = false; ;

         // free the last port
         Dispose();

         // get the abreviation from the version string
         System.String header = getToken(typeVersionBuffer.ToString(), TOKEN_ABRV);

         // Change COMU to COM
         if (header.Equals("COMU"))
            header = "COM";

         // loop to make sure that the begining of the port name matches the head
         for (i = 0; i < header.Length; i++)
         {
            if (portName[i] != header[i])
               return false;
         }

         // i now points to begining of integer (0 TO 15)
         if ((portName[i] >= '0') && (portName[i] <= '9'))
         {
            prtnum = portName[i] - '0';
            if (((i + 1) < portName.Length) && (portName[i + 1] >= '0') && (portName[i + 1] <= '9'))
            {
               prtnum *= 10;
               prtnum += portName[i + 1] - '0';
            }

            if (prtnum > 15)
               return false;
         }

         // now have prtnum
         // get a session handle, 16 sec timeout
         long timeout = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 + 16000;
         do
         {

            this.sessionHandle =
               TMEXLibrary.TMExtendedStartSession((short)prtnum, (short)portType, ref EnhancedOptions);
            // this port type does not exist
            if (sessionHandle == - 201)
               break;
               // valid handle
            else if (sessionHandle > 0)
            {
               // do setup
               if (TMEXLibrary.TMSetup(sessionHandle) == 1)
               {
                  // read the version again
                  TMEXLibrary.TMGetTypeVersion((int)portType, typeVersionBuffer);
                  byte[] specBuffer = new byte[319];
                  // get the adapter spec
                  TMEXLibrary.TMGetAdapterSpec(sessionHandle, specBuffer);
                  adapterSpecDescription = TMEXLibrary.getDescriptionFromSpecification(specBuffer);
                  adapterSpecFeatures = TMEXLibrary.getFeaturesFromSpecification(specBuffer);

                  // record the portnum
                  this.portNum = (short) prtnum;
                  // do a search ??????????????????????
                  TMEXLibrary.TMFirst(sessionHandle, stateBuffer);
                  // return success
                  rt = true;
               }

               break;
            }
         }
         while (timeout > (System.DateTime.Now.Ticks - 621355968000000000) / 10000);

         // close the session
         TMEXLibrary.TMEndSession(sessionHandle);
         sessionHandle = 0;

         // check if session was not available
         if (!rt)
         {
            // free up the port
            Dispose();
            // throw exception
            throw new AdapterException("1-Wire Net not available");
         }

         return rt;
      }


      #endregion

      #region Data I/O
      /// <summary> Sends a Reset to the 1-Wire Network.
      ///
      /// </summary>
      /// <returns>  the result of the reset. Potential results are:
      /// <ul>
      /// <li> 0 (RESET_NOPRESENCE) no devices present on the 1-Wire Network.
      /// <li> 1 (RESET_PRESENCE) normal presence pulse detected on the 1-Wire
      /// Network indicating there is a device present.
      /// <li> 2 (RESET_ALARM) alarming presence pulse detected on the 1-Wire
      /// Network indicating there is a device present and it is in the
      /// alarm condition.  This is only provided by the DS1994/DS2404
      /// devices.
      /// <li> 3 (RESET_SHORT) inticates 1-Wire appears shorted.  This can be
      /// transient conditions in a 1-Wire Network.  Not all adapter types
      /// can detect this condition.
      /// </ul>
      ///
      /// </returns>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override OWResetResult Reset()
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         // do 1-Wire reset
         int rt = TMEXLibrary.TMTouchReset(sessionHandle);
         // release the session
         release_session();

         // check for adapter communcication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0 || rt > 3)
            throw new AdapterException("native TMEX error " + rt);
         else if (rt == 3)
            throw new AdapterException("1-Wire Net shorted");

         OWResetResult rslt = (OWResetResult)rt;
         onReset(rslt);
         return rslt;
      }

      /// <summary> Sends a bit to the 1-Wire Network.
      ///
      /// </summary>
      /// <param name="bitValue"> the bit value to send to the 1-Wire Network.
      ///
      /// </param>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override void PutBit(bool bitValue)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         // do 1-Wire bit
         int rt = TMEXLibrary.TMTouchBit(sessionHandle, (short) (bitValue?1:0));
         // release the session
         release_session();

         // check for adapter communication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);

         if (bitValue != (rt > 0))
            throw new AdapterException("Error during putBit()");
      }

      /// <summary> Sends a byte to the 1-Wire Network.
      ///
      /// </summary>
      /// <param name="byteValue"> the byte value to send to the 1-Wire Network.
      ///
      /// </param>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override void PutByte(int byteValue)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         int rt = TMEXLibrary.TMTouchByte(sessionHandle, (short) byteValue);
         // release the session
         release_session();

         // check for adapter communcication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error " + rt);

         if (rt != ((0x00FF) & byteValue))
            throw new AdapterException("Error during putByte(), echo was incorrect ");
      }

      /// <summary> Gets a bit from the 1-Wire Network.
      ///
      /// </summary>
      /// <returns>  the bit value recieved from the the 1-Wire Network.
      /// </returns>
      public override bool GetBit()
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         // do 1-Wire bit
         int rt = TMEXLibrary.TMTouchBit(sessionHandle, (short) 1);
         // release the session
         release_session();

         // check for adapter communication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);

         return (rt > 0);
      }


      /// <summary> Gets a byte from the 1-Wire Network.
      ///
      /// </summary>
      /// <returns>  the byte value received from the the 1-Wire Network.
      /// </returns>
      public override int GetByte()
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         int rt = TMEXLibrary.TMTouchByte(sessionHandle, (short) 0x0FF);

         // release the session
         release_session();

         // check for adapter communcication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error " + rt);

         return rt;
      }

      /// <summary> Gets a byte from the 1-Wire Network.
      ///
      /// </summary>
      /// <returns>  the byte value received from the the 1-Wire Network.
      ///
      /// </returns>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>

      /// <summary> Get a block of data from the 1-Wire Network.
      ///
      /// </summary>
      /// <param name="len"> length of data bytes to receive
      ///
      /// </param>
      /// <returns>  the data received from the 1-Wire Network.
      ///
      /// </returns>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override byte[] GetBlock(int len)
      {
         byte[] barr = new byte[len];

         GetBlock(barr, 0, len);

         return barr;
      }

      /// <summary> Get a block of data from the 1-Wire Network and write it into
      /// the provided array.
      ///
      /// </summary>
      /// <param name="arr">    array in which to write the received bytes
      /// </param>
      /// <param name="len">    length of data bytes to receive
      ///
      /// </param>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override void GetBlock(byte[] arr, int len)
      {
         GetBlock(arr, 0, len);
      }

      /// <summary> Get a block of data from the 1-Wire Network and write it into
      /// the provided array.
      ///
      /// </summary>
      /// <param name="arr">    array in which to write the received bytes
      /// </param>
      /// <param name="off">    offset into the array to start
      /// </param>
      /// <param name="len">    length of data bytes to receive
      ///
      /// </param>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override void GetBlock(byte[] arr, int off, int len)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         int rt;
         if (off == 0)
         {
            for (int i = 0; i < len; i++)
               arr[i] = (byte) 0x0FF;
            rt = TMEXLibrary.TMBlockStream(sessionHandle, arr, (short) len);
            // release the session
            release_session();
         }
         else
         {
            byte[] dataBlock = new byte[len];
            for (int i = 0; i < len; i++)
               dataBlock[i] = (byte) 0x0FF;
            rt = TMEXLibrary.TMBlockStream(sessionHandle, dataBlock, (short) len);
            // release the session
            release_session();
            Array.Copy(dataBlock, 0, arr, off, len);
         }

         // check for adapter communcication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error " + rt);
      }

      /// <summary> Sends a block of data and returns the data received in the same array.
      /// This method is used when sending a block that contains reads and writes.
      /// The 'read' portions of the data block need to be pre-loaded with 0xFF's.
      /// It starts sending data from the index at offset 'off' for length 'len'.
      ///
      /// </summary>
      /// <param name="dataBlock"> array of data to transfer to and from the 1-Wire Network.
      /// </param>
      /// <param name="off">       offset into the array of data to start
      /// </param>
      /// <param name="len">       length of data to send / receive starting at 'off'
      ///
      /// </param>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override void DataBlock(byte[] dataBlock, int off, int len)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         int rt = 0;
         if (len > 1023)
         {
            byte[] dataBlockBuffer = new byte[1023];
            // Change to only do 1023 bytes at a time
            int numblocks = len / 1023;
            int extra = len % 1023;
            for (int i = 0; i < numblocks; i++)
            {
               Array.Copy(dataBlock, off + i * 1023, dataBlockBuffer, 0, 1023);
               rt = TMEXLibrary.TMBlockStream(sessionHandle, dataBlockBuffer, (short) 1023);
               Array.Copy(dataBlockBuffer, 0, dataBlock, off + i * 1023, 1023);
               if (rt != 1023)
                  break;
            }
            if ((rt >= 0) && (extra > 0))
            {
               Array.Copy(dataBlock, off + numblocks * 1023, dataBlockBuffer, 0, extra);
               rt = TMEXLibrary.TMBlockStream(sessionHandle, dataBlockBuffer, (short) extra);
               Array.Copy(dataBlockBuffer, 0, dataBlock, off + numblocks * 1023, extra);
            }
         }
         else if (off > 0)
         {
            byte[] dataBlockOffset = new byte[len];
            Array.Copy(dataBlock, off, dataBlockOffset, 0, len);
            rt = TMEXLibrary.TMBlockStream(sessionHandle, dataBlockOffset, (short) len);
            Array.Copy(dataBlockOffset, 0, dataBlock, off, len);
         }
         else
         {
            rt = TMEXLibrary.TMBlockStream(sessionHandle, dataBlock, (short) len);
         }
         // release the session
         release_session();

         // check for adapter communcication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error " + rt);
      }
      #endregion

      #region Communication Speed
      /// <summary>OWSpeed representing current speed of communication on 1-Wire network
      /// </summary>
      public override OWSpeed Speed
      {
         set
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            // get a session
            if (!get_session())
               throw new AdapterException("Port in use");

            // change speed
            int rt = TMEXLibrary.TMOneWireCom(
               sessionHandle, TMEX_Constants.LEVEL_SET, (short)value);

            // (1.01) if in overdrive then force an exclusive
            if (value == OWSpeed.SPEED_OVERDRIVE)
               inExclusive = true;
            // release the session
            release_session();

            // check for adapter communication problems
            if (rt == - 3)
               throw new AdapterException(
                  "Adapter type does not support selected speed");
            else if (rt == - 12)
               throw new AdapterException(
                  "1-Wire Adapter communication exception");
               // check for microlan exception
            else if (rt < 0)
               throw new AdapterException(
                  "native TMEX error" + rt);
               // check for could not set
            else if (rt != (short)value)
               throw new AdapterException(
                  "native TMEX error: could not set adapter to desired speed: " + rt);

            OWSpeed old = this.speed;
            this.speed = value;
            // fire speed change event
            onSpeedChange(old, value);
         }
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               return OWSpeed.SPEED_REGULAR;

            // get a session
            if (!get_session())
               return OWSpeed.SPEED_REGULAR;

            // change speed
            int rt = TMEXLibrary.TMOneWireCom(sessionHandle,
               (short) TMEX_Constants.LEVEL_READ, (short) 0);

            // release the session
            release_session();

            if (rt < 0 || rt >3)
               return OWSpeed.SPEED_REGULAR;
            else
               return (OWSpeed)rt;
         }
      }
      #endregion

      #region Power Delivery

      /// <summary> Sets the duration to supply power to the 1-Wire Network.
      /// This method takes a time parameter that indicates the program
      /// pulse length when the method startPowerDelivery().<p>
      ///
      /// Note: to avoid getting an exception,
      /// use the canDeliverPower() and canDeliverSmartPower()
      /// method to check it's availability. <p>
      ///
      /// </summary>
      /// <param name="">timeFactor
      /// <ul>
      /// <li>   0 (DELIVERY_HALF_SECOND) provide power for 1/2 second.
      /// <li>   1 (DELIVERY_ONE_SECOND) provide power for 1 second.
      /// <li>   2 (DELIVERY_TWO_SECONDS) provide power for 2 seconds.
      /// <li>   3 (DELIVERY_FOUR_SECONDS) provide power for 4 seconds.
      /// <li>   4 (DELIVERY_SMART_DONE) provide power until the
      /// the device is no longer drawing significant power.
      /// <li>   5 (DELIVERY_INFINITE) provide power until the
      /// setBusNormal() method is called.
      /// </ul>
      /// </param>
      public override void SetPowerDuration (OWPowerTime timeFactor)
      {
         // Right now we only support infinite pull up.
         if (timeFactor != OWPowerTime.DELIVERY_INFINITE)
            throw new AdapterException(
               "No support for other than infinite power duration");
      }

      /// <summary> Sets the 1-Wire Network voltage to supply power to an iButton device.
      /// This method takes a time parameter that indicates whether the
      /// power delivery should be done immediately, or after certain
      /// conditions have been met. <p>
      ///
      /// Note: to avoid getting an exception,
      /// use the canDeliverPower() and canDeliverSmartPower()
      /// method to check it's availability. <p>
      ///
      /// </summary>
      /// <param name="">changeCondition
      /// <ul>
      /// <li>   0 (CONDITION_NOW) operation should occur immediately.
      /// <li>   1 (CONDITION_AFTER_BIT) operation should be pending
      /// execution immediately after the next bit is sent.
      /// <li>   2 (CONDITION_AFTER_BYTE) operation should be pending
      /// execution immediately after next byte is sent.
      /// </ul>
      ///
      /// </param>
      /// <returns> <code>true</code> if the voltage change was successful,
      /// <code>false</code> otherwise.
      ///
      /// </returns>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      public override bool StartPowerDelivery(OWPowerStart changeCondition)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         if (!adapterSpecFeatures[TMEX_Constants.FEATURE_POWER])
            throw new AdapterException("Hardware option not available");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         // start 12Volt pulse
         int rt = TMEXLibrary.TMOneWireLevel(sessionHandle,
            TMEX_Constants.LEVEL_SET, TMEX_Constants.LEVEL_STRONG_PULLUP, (short)changeCondition);
         // release the session
         release_session();

         // check for adapter communication problems
         if (rt == - 3)
            throw new AdapterException("Adapter type does not support power delivery");
         else if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);
            // check for could not set
         else if ((rt != TMEX_Constants.LEVEL_STRONG_PULLUP)
            && (changeCondition == OWPowerStart.CONDITION_NOW))
            throw new AdapterException("native TMEX error: could not set adapter to desired level: " + rt);

         return true;
      }

      /// <summary> Sets the duration for providing a program pulse on the
      /// 1-Wire Network.
      /// This method takes a time parameter that indicates the program
      /// pulse length when the method startProgramPulse().<p>
      ///
      /// Note: to avoid getting an exception,
      /// use the canDeliverPower() method to check it's
      /// availability. <p>
      ///
      /// </summary>
      /// <param name="">timeFactor
      /// <ul>
      /// <li>   6 (DELIVERY_EPROM) provide program pulse for 480 microseconds
      /// <li>   5 (DELIVERY_INFINITE) provide power until the
      /// setBusNormal() method is called.
      /// </ul>
      /// </param>
      public override void SetProgramPulseDuration (OWPowerTime timeFactor)
      {
         if (timeFactor != OWPowerTime.DELIVERY_EPROM)
            throw new AdapterException(
               "Only support EPROM length program pulse duration");
      }

      /// <summary> Sets the 1-Wire Network voltage to eprom programming level.
      /// This method takes a time parameter that indicates whether the
      /// power delivery should be done immediately, or after certain
      /// conditions have been met. <p>
      ///
      /// Note: to avoid getting an exception,
      /// use the canProgram() method to check it's
      /// availability. <p>
      ///
      /// </summary>
      /// <param name="">changeCondition
      /// <ul>
      /// <li>   0 (CONDITION_NOW) operation should occur immediately.
      /// <li>   1 (CONDITION_AFTER_BIT) operation should be pending
      /// execution immediately after the next bit is sent.
      /// <li>   2 (CONDITION_AFTER_BYTE) operation should be pending
      /// execution immediately after next byte is sent.
      /// </ul>
      /// </param>
      /// <returns> <code>true</code> if the voltage change was successful,
      /// <code>false</code> otherwise.
      /// </returns>
      public override bool StartProgramPulse(OWPowerStart changeCondition)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         if (!adapterSpecFeatures[TMEX_Constants.FEATURE_PROGRAM])
            throw new AdapterException("Hardware option not available");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         int rt;
         // if pulse is 'now' then use TMProgramPulse
         if (changeCondition == OWPowerStart.CONDITION_NOW)
         {
            rt = TMEXLibrary.TMProgramPulse(sessionHandle);
            // change rt value to be compatible with TMOneWireLevel
            if (rt != 0)
               rt = TMEX_Constants.LEVEL_PROGRAM;
         }
         else
         {
            // start 12Volt pulse
            rt = TMEXLibrary.TMOneWireLevel(sessionHandle,
               TMEX_Constants.LEVEL_SET, TMEX_Constants.LEVEL_PROGRAM, (short)changeCondition);
         }
         // release the session
         release_session();

         // check for adapter communication problems
         if (rt == - 3)
            throw new AdapterException("Adapter type does not support EPROM programming");
         else if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);
            // check for could not set
         else if ((rt != TMEX_Constants.LEVEL_PROGRAM)
            && (changeCondition == OWPowerStart.CONDITION_NOW))
            throw new AdapterException(
               "native TMEX error: could not set adapter to desired level: " + rt);

         return true;
      }

      /// <summary> Sets the 1-Wire Network voltage to 0 volts.  This method is used
      /// rob all 1-Wire Network devices of parasite power delivery to force
      /// them into a hard reset.
      /// </summary>
      public override void StartBreak()
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         // start break
         int rt = TMEXLibrary.TMOneWireLevel(sessionHandle,
            TMEX_Constants.LEVEL_SET, TMEX_Constants.LEVEL_BREAK, TMEX_Constants.PRIMED_NONE);
         // release the session
         release_session();

         // check for adapter communication problems
         if (rt == - 3)
            throw new AdapterException("Adapter type does not support break");
         else if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);
            // check for could not set
         else if (rt != TMEX_Constants.LEVEL_BREAK)
            throw new AdapterException(
               "native TMEX error: could not set adapter to break: " + rt);
      }

      /// <summary> Sets the 1-Wire Network voltage to normal level.  This method is used
      /// to disable 1-Wire conditions created by startPowerDelivery and
      /// startProgramPulse.  This method will automatically be called if
      /// a communication method is called while an outstanding power
      /// command is taking place.
      ///
      /// </summary>
      /// <throws>  OneWireIOException on a 1-Wire communication error </throws>
      /// <throws>  AdapterException on a setup error with the 1-Wire adapter </throws>
      /// <summary>         or the adapter does not support this operation
      /// </summary>
      public override void SetPowerNormal()
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         // set back to normal
         int rt = TMEXLibrary.TMOneWireLevel(sessionHandle,
            TMEX_Constants.LEVEL_SET,
            (short)OWLevel.LEVEL_NORMAL, (short)OWPowerStart.CONDITION_NOW);
         // release the session
         release_session();

         if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);
      }

      #endregion

      #region Adapter Features
      /// <summary> Returns whether adapter can physically support overdrive mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do OverDrive,
      /// <code>false</code> otherwise.
      /// </returns>
      public override bool CanOverdrive
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            return (adapterSpecFeatures[TMEX_Constants.FEATURE_OVERDRIVE]);
         }
      }

      /// <summary> Returns whether the adapter can physically support hyperdrive mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do HyperDrive,
      /// <code>false</code> otherwise.
      /// </returns>
      public override bool CanHyperdrive
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            return false;
         }
      }

      /// <summary> Returns whether the adapter can physically support flex speed mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do flex speed,
      /// <code>false</code> otherwise.
      /// </returns>
      public override bool CanFlex
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            return (adapterSpecFeatures[TMEX_Constants.FEATURE_FLEX]);
         }
      }


      /// <summary> Returns whether adapter can physically support 12 volt power mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do Program voltage,
      /// <code>false</code> otherwise.
      /// </returns>
      public override bool CanProgram
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            return (adapterSpecFeatures[TMEX_Constants.FEATURE_PROGRAM]);
         }
      }

      /// <summary> Returns whether the adapter can physically support strong 5 volt power
      /// mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do strong 5 volt
      /// mode, <code>false</code> otherwise.
      /// </returns>
      public override bool CanDeliverPower
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            return (adapterSpecFeatures[TMEX_Constants.FEATURE_POWER]);
         }
      }

      /// <summary> Returns whether the adapter can physically support "smart" strong 5
      /// volt power mode.  "smart" power delivery is the ability to deliver
      /// power until it is no longer needed.  The current drop it detected
      /// and power delivery is stopped.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do "smart" strong
      /// 5 volt mode, <code>false</code> otherwise.
      /// </returns>
      public override bool CanDeliverSmartPower
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            return false; // currently not implemented
         }
      }

      /// <summary> Returns whether adapter can physically support 0 volt 'break' mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do break,
      /// <code>false</code> otherwise.
      /// </returns>
      public override bool CanBreak
      {
         get
         {
            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            return (adapterSpecFeatures[TMEX_Constants.FEATURE_BREAK]);
         }
      }
      #endregion

      #region Searching
      private bool resetSearch = true;
      private bool doAlarmSearch = false;
      private bool skipResetOnSearch = false;

      /// <summary> Returns <code>true</code> if the first iButton or 1-Wire device
      /// is found on the 1-Wire Network.
      /// If no devices are found, then <code>false</code> will be returned.
      /// </summary>
      /// <returns>  <code>true</code> if an iButton or 1-Wire device is found.
      /// </returns>
      public override bool GetFirstDevice(byte[] address, int offset)
      {
         // reset the internal rom buffer
         resetSearch = true;

         return GetNextDevice(address, offset);
      }

      /// <summary> Returns <code>true</code> if the next iButton or 1-Wire device
      /// is found. The previous 1-Wire device found is used
      /// as a starting point in the search.  If no more devices are found
      /// then <code>false</code> will be returned.
      /// </summary>
      /// <returns>  <code>true</code> if an iButton or 1-Wire device is found.
      /// </returns>
      public override bool GetNextDevice(byte[] address, int offset)
      {
         while (true)
         {
            Debug.WriteLine("DEBUG: TMEXLibAdapter.GetNextDevice(byte[],int) called");
            Debug.WriteLine("DEBUG: TMEXLibAdapter.GetNextDevice, resetSearch="+resetSearch);
            Debug.WriteLine("DEBUG: TMEXLibAdapter.GetNextDevice, skipResetOnSearch="+skipResetOnSearch);
            Debug.WriteLine("DEBUG: TMEXLibAdapter.GetNextDevice, doAlarmSearch="+doAlarmSearch);
            short[] ROM = new short[8];

            // check if port is selected
            if ((portNum < 0) || (portType < 0))
               throw new AdapterException("Port not selected");

            bool gotSession = false;
            short rt;
            try
            {
               // get a session
               gotSession = get_session();
               if (!gotSession)
                  throw new AdapterException("Port in use");

               rt = TMEXLibrary.TMSearch(sessionHandle, stateBuffer,
                  (short) (resetSearch?1:0), (short) (skipResetOnSearch?0:1),
                  (short) (doAlarmSearch?0xEC:0xF0));

               Debug.WriteLine("DEBUG: TMEXLibary.TMSearch, rt=" + rt);

               // check for microlan exception
               if (rt < 0)
               {
                  if(!skipResetOnSearch)
                     // we'll just use Short for the general error condition
                     onReset(OWResetResult.RESET_SHORT);
                  throw new AdapterException("native TMEX error " + rt);
               }

               // retrieve the ROM number found
               ROM[0] = 0;
               short romrt = TMEXLibrary.TMRom(sessionHandle, stateBuffer, ROM);
               if (romrt == 1)
               {
                  // copy to java array
                  for (int i = 0; i < 8; i++)
                     address[i+offset] = (byte) ROM[i];
               }
               else
               {
                  if(!skipResetOnSearch)
                     // we'll just use Short for the general error condition
                     onReset(OWResetResult.RESET_SHORT);
                  throw new AdapterException("native TMEX error " + romrt);
               }
            }
            finally
            {
               // release the session
               release_session();
            }

            if (rt > 0)
            {
               resetSearch = false;

               // check if this is an OK family type
               if (isValidFamily(address[offset]))
               {
                  if(!skipResetOnSearch)
                     // we'll assume this is a presence pulse
                     onReset(OWResetResult.RESET_PRESENCE);
                  return true;
               }
               else
               {
                  if(!skipResetOnSearch)
                     // we'll assume this is no presence pulse
                     onReset(OWResetResult.RESET_NOPRESENCE);
               }

               // Else, loop to the top and do another search.
            }
            else
            {
               resetSearch = true;

               return false;
            }
         }
      }

      /// <summary> Verifies that the iButton or 1-Wire device specified is present on
      /// the 1-Wire Network. This does not affect the 'current' device
      /// state information used in searches (findNextDevice...).
      /// </summary>
      /// <param name="address"> device address to verify is present
      /// </param>
      /// <returns>  <code>true</code> if device is present else
      /// <code>false</code>.
      /// </returns>
      /// <seealso cref="Address">
      /// </seealso>
      public override bool IsPresent(byte[] address, int offset)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         short[] ROM = new short[8];
         for (int i = 0; i < 8; i++)
            ROM[i] = address[i+offset];

         // get the current rom to restore after isPresent() (1.01)
         short[] oldROM = new short[8];
         oldROM[0] = 0;
         TMEXLibrary.TMRom(sessionHandle, stateBuffer, oldROM);
         // set this rom to TMEX
         TMEXLibrary.TMRom(sessionHandle, stateBuffer, ROM);
         // see if part this present
         int rt = TMEXLibrary.TMStrongAccess(sessionHandle, stateBuffer);
         // restore
         TMEXLibrary.TMRom(sessionHandle, stateBuffer, oldROM);
         // release the session
         release_session();

         // check for adapter communcication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);

         return (rt > 0);
      }

      /// <summary> Verifies that the iButton or 1-Wire device specified is present
      /// on the 1-Wire Network and in an alarm state. This does not
      /// affect the 'current' device state information used in searches
      /// (findNextDevice...).
      /// </summary>
      /// <param name="address"> device address to verify is present and alarming
      /// </param>
      /// <returns>  <code>true</code> if device is present and alarming else
      /// <code>false</code>.
      /// </returns>
      /// <seealso cref="Address">
      /// </seealso>
      public override bool IsAlarming(byte[] address, int offset)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         short[] ROM = new short[8];
         for (int i = 0; i < 8; i++)
            ROM[i] = address[i+offset];

         // get the current rom to restore after isPresent() (1.01)
         short[] oldROM = new short[8];
         oldROM[0] = 0;
         TMEXLibrary.TMRom(sessionHandle, stateBuffer, oldROM);
         // set this rom to TMEX
         TMEXLibrary.TMRom(sessionHandle, stateBuffer, ROM);
         // see if part this present
         int rt = TMEXLibrary.TMStrongAlarmAccess(sessionHandle, stateBuffer);
         // restore
         TMEXLibrary.TMRom(sessionHandle, stateBuffer, oldROM);
         // release the session
         release_session();

         // check for adapter communication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);

         return (rt > 0);
      }

      /// <summary> Selects the specified iButton or 1-Wire device by broadcasting its
      /// address.  This operation is refered to a 'MATCH ROM' operation
      /// in the iButton and 1-Wire device data sheets.  This does not
      /// affect the 'current' device state information used in searches
      /// (findNextDevice...).
      ///
      /// Warning, this does not verify that the device is currently present
      /// on the 1-Wire Network (See isPresent).
      /// </summary>
      /// <param name="address">    iButton to select
      /// </param>
      /// <returns>  <code>true</code> if device address was sent,<code>false</code>
      /// otherwise.
      /// </returns>
      public override bool SelectDevice(byte[] address, int off)
      {
         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new AdapterException("Port not selected");

         // get a session
         if (!get_session())
            throw new AdapterException("Port in use");

         byte[] send_block = new byte[9];
         send_block[0] = (byte) 0x55; // match command
         for (int i = 0; i < 8; i++)
            send_block[i + 1] = address[i+off];

         // Change to use a block, not TMRom/TMAccess
         int rt = TMEXLibrary.TMBlockIO(sessionHandle, send_block, (short) 9);
         // release the session
         release_session();

         // check for adapter communication problems
         if (rt == - 12)
            throw new AdapterException("1-Wire Adapter communication exception");
            // check for no device
         else if (rt == - 7)
            throw new AdapterException("No device detected");
            // check for microlan exception
         else if (rt < 0)
            throw new AdapterException("native TMEX error" + rt);

         return (rt >= 1);
      }

      /// <summary> Set the 1-Wire Network search to find only iButtons and 1-Wire
      /// devices that are in an 'Alarm' state that signals a need for
      /// attention.  Not all iButton types
      /// have this feature.  Some that do: DS1994, DS1920, DS2407.
      /// This selective searching can be canceled with the
      /// 'setSearchAllDevices()' method.
      /// </summary>
      /// <seealso cref="#setNoResetSearch">
      /// </seealso>
      public override void SetSearchOnlyAlarmingDevices()
      {
         doAlarmSearch = true;
      }

      /// <summary> Set the 1-Wire Network search to not perform a 1-Wire
      /// reset before a search.  This feature is chiefly used with
      /// the DS2409 1-Wire coupler.
      /// The normal reset before each search can be restored with the
      /// 'setSearchAllDevices()' method.
      /// </summary>
      public override void SetNoResetSearch()
      {
         skipResetOnSearch = true;
      }

      /// <summary> Set the 1-Wire Network search to find all iButtons and 1-Wire
      /// devices whether they are in an 'Alarm' state or not and
      /// restores the default setting of providing a 1-Wire reset
      /// command before each search. (see setNoResetSearch() method).
      /// </summary>
      /// <seealso cref="#setNoResetSearch">
      /// </seealso>
      public override void SetSearchAllDevices()
      {
         doAlarmSearch = false;
         skipResetOnSearch = false;
      }
      #endregion

      #region Private Helper Methods
      //--------------------------------------------------------------------------
      // Parse the version string for a token
      //
      private static System.String getToken(System.String verStr, int token)
      {
         int currentToken = - 1;
         bool inToken = false; ;
         System.String toReturn = "";

         for (int i = 0; i < verStr.Length; i++)
         {
            if ((verStr[i] != ' ') && (!inToken))
            {
               inToken = true;
               currentToken++;
            }

            if ((verStr[i] == ' ') && (inToken))
               inToken = false;

            if (((inToken) && (currentToken == token)) ||
               ((token == 255) && (currentToken > TOKEN_DATE)))
               toReturn += verStr[i];
         }
         return toReturn;
      }

      /// <summary> Attempt to get a TMEX session.  If already in an 'exclusive' block
      /// then just return.
      /// </summary>
      private bool get_session()
      {
         lock (this)
         {
            int sessionOptions = TMEX_Constants.SESSION_INFINITE;

            // check if in exclusive block
            if (inExclusive)
            {
               // make sure still valid (if not get a new one)
               if (TMEXLibrary.TMValidSession(sessionHandle) > 0)
                  return true;
            }

            // attempt to get a session handle (2 sec timeout)
            //long timeout = System.currentTimeMillis() + 2000;
            //UPGRADE_TODO: Method 'java.util.Date.getTime' was converted to 'System.DateTime.Ticks' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilDategetTime_3"'
            long timeout = (System.DateTime.Now).Ticks + 2000;
            do
            {
               sessionHandle = TMEXLibrary.TMExtendedStartSession((short)portNum, (short)portType, ref sessionOptions);

               // this port type does not exist
               if (sessionHandle == - 201)
                  break;
                  // valid handle
               else if (sessionHandle > 0)
                  // success
                  return true;
            }
            while (timeout > (System.DateTime.Now.Ticks - 621355968000000000) / 10000);

            // timeout of invalid porttype
            sessionHandle = 0;

            return false;
         }
      }

      /// <summary>  Release a TMEX session.  If already in an 'exclusive' block
      /// then just return.
      /// </summary>
      private bool release_session()
      {
         lock (this)
         {
            // check if in exclusive block
            if (inExclusive)
               return true;

            // close the session
            TMEXLibrary.TMEndSession(sessionHandle);

            // clear out handle (used to indicate not session)
            sessionHandle = 0;

            return true;
         }
      }
      #endregion
   }
}
