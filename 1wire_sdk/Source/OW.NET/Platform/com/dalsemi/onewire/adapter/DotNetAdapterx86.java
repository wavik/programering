/*---------------------------------------------------------------------------
   * Copyright (C) 1999 - 2007 Maxim Integrated Products, All Rights Reserved.
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
package com.dalsemi.onewire.adapter;

// imports
import java.util.Enumeration;
import com.dalsemi.onewire.container.OneWireContainer;
import com.dalsemi.onewire.*;
import com.dalsemi.onewire.adapter.*;
import com.dalsemi.onewire.OneWireException;
import java.util.Vector;
import java.lang.ClassNotFoundException;
import java.io.File;
import com.dalsemi.onewire.debug.*;
import System.Runtime.InteropServices.*;

   /**
    * The DSPortAdapterx86 class for all DotNet native adapters (Win32).
    *
    * Instances of valid DSPortAdapter's are retrieved from methods in
    * {@link com.dalsemi.onewire.OneWireAccessProvider OneWireAccessProvider}.
    * 
    * <P>The DotNetAdapterx86 methods can be organized into the following categories: </P>
    * <UL>
    *   <LI> <B> Information </B>
    *     <UL>
    *       <LI> {@link #getAdapterName() getAdapterName}
    *       <LI> {@link #getPortTypeDescription() getPortTypeDescription}
    *       <LI> {@link #getClassVersion() getClassVersion}
    *       <LI> {@link #adapterDetected() adapterDetected}
    *       <LI> {@link #getAdapterVersion() getAdapterVersion}
    *       <LI> {@link #getAdapterAddress() getAdapterAddress}
    *     </UL>
    *   <LI> <B> Port Selection </B>
    *     <UL>
    *       <LI> {@link #getPortNames() getPortNames}
    *       <LI> {@link #selectPort(String) selectPort}
    *       <LI> {@link #getPortName() getPortName}
    *       <LI> {@link #freePort() freePort}
    *     </UL>
    *   <LI> <B> Adapter Capabilities </B>
    *     <UL>
    *       <LI> {@link #canOverdrive() canOverdrive}
    *       <LI> {@link #canHyperdrive() canHyperdrive}
    *       <LI> {@link #canFlex() canFlex}
    *       <LI> {@link #canProgram() canProgram}
    *       <LI> {@link #canDeliverPower() canDeliverPower}
    *       <LI> {@link #canDeliverSmartPower() canDeliverSmartPower}
    *       <LI> {@link #canBreak() canBreak}
    *     </UL>
    *   <LI> <B> 1-Wire Network Semaphore </B>
    *     <UL>
    *       <LI> {@link #beginExclusive(boolean) beginExclusive}
    *       <LI> {@link #endExclusive() endExclusive}
    *     </UL>
    *   <LI> <B> 1-Wire Device Discovery </B>
    *     <UL>
    *       <LI> Selective Search Options
    *         <UL>
    *          <LI> {@link #targetAllFamilies() targetAllFamilies}
    *          <LI> {@link #targetFamily(int) targetFamily(int)}
    *          <LI> {@link #targetFamily(byte[]) targetFamily(byte[])}
    *          <LI> {@link #excludeFamily(int) excludeFamily(int)}
    *          <LI> {@link #excludeFamily(byte[]) excludeFamily(byte[])}
    *          <LI> {@link #setSearchOnlyAlarmingDevices() setSearchOnlyAlarmingDevices}
    *          <LI> {@link #setNoResetSearch() setNoResetSearch}
    *          <LI> {@link #setSearchAllDevices() setSearchAllDevices}
    *         </UL>
    *       <LI> Search With Automatic 1-Wire Container creation
    *         <UL>
    *          <LI> {@link #getAllDeviceContainers() getAllDeviceContainers}
    *          <LI> {@link #getFirstDeviceContainer() getFirstDeviceContainer}
    *          <LI> {@link #getNextDeviceContainer() getNextDeviceContainer}
    *         </UL>
    *       <LI> Search With NO 1-Wire Container creation
    *         <UL>
    *          <LI> {@link #findFirstDevice() findFirstDevice}
    *          <LI> {@link #findNextDevice() findNextDevice}
    *          <LI> {@link #getAddress(byte[]) getAddress(byte[])}
    *          <LI> {@link #getAddressAsLong() getAddressAsLong}
    *          <LI> {@link #getAddressAsString() getAddressAsString}
    *         </UL>
    *       <LI> Manual 1-Wire Container creation
    *         <UL>
    *          <LI> {@link #getDeviceContainer(byte[]) getDeviceContainer(byte[])}
    *          <LI> {@link #getDeviceContainer(long) getDeviceContainer(long)}
    *          <LI> {@link #getDeviceContainer(String) getDeviceContainer(String)}
    *          <LI> {@link #getDeviceContainer() getDeviceContainer()}
    *         </UL>
    *     </UL>
    *   <LI> <B> 1-Wire Network low level access (usually not called directly) </B>
    *     <UL>
    *       <LI> Device Selection and Presence Detect
    *         <UL>
    *          <LI> {@link #isPresent(byte[]) isPresent(byte[])}
    *          <LI> {@link #isPresent(long) isPresent(long)}
    *          <LI> {@link #isPresent(String) isPresent(String)}
    *          <LI> {@link #isAlarming(byte[]) isAlarming(byte[])}
    *          <LI> {@link #isAlarming(long) isAlarming(long)}
    *          <LI> {@link #isAlarming(String) isAlarming(String)}
    *          <LI> {@link #select(byte[]) select(byte[])}
    *          <LI> {@link #select(long) select(long)}
    *          <LI> {@link #select(String) select(String)}
    *         </UL>
    *       <LI> Raw 1-Wire IO
    *         <UL>
    *          <LI> {@link #reset() reset}
    *          <LI> {@link #putBit(boolean) putBit}
    *          <LI> {@link #getBit() getBit}
    *          <LI> {@link #putByte(int) putByte}
    *          <LI> {@link #getByte() getByte}
    *          <LI> {@link #getBlock(int) getBlock(int)}
    *          <LI> {@link #getBlock(byte[], int) getBlock(byte[], int)}
    *          <LI> {@link #getBlock(byte[], int, int) getBlock(byte[], int, int)}
    *          <LI> {@link #dataBlock(byte[], int, int) dataBlock(byte[], int, int)}
    *         </UL>
    *       <LI> 1-Wire Speed and Power Selection
    *         <UL>
    *          <LI> {@link #setPowerDuration(int) setPowerDuration}
    *          <LI> {@link #startPowerDelivery(int) startPowerDelivery}
    *          <LI> {@link #setProgramPulseDuration(int) setProgramPulseDuration}
    *          <LI> {@link #startProgramPulse(int) startProgramPulse}
    *          <LI> {@link #startBreak() startBreak}
    *          <LI> {@link #setPowerNormal() setPowerNormal}
    *          <LI> {@link #setSpeed(int) setSpeed}
    *          <LI> {@link #getSpeed() getSpeed}
    *         </UL>
    *     </UL>
    *   <LI> <B> Advanced </B>
    *     <UL>
    *        <LI> {@link #registerOneWireContainerClass(int, Class) registerOneWireContainerClass}
    *     </UL>
    *  </UL>
    *
    * @see com.dalsemi.onewire.OneWireAccessProvider
    * @see com.dalsemi.onewire.container.OneWireContainer
    *
    * @version    0.01, 20 March 2001
    * @author     DS
    * 
    */
public class DotNetAdapterx86
   extends DSPortAdapter
{
   //--------
   //-------- Variables
   //--------

   /** DotNet port type number (0-15) */
   protected int portType;

   /** Current 1-Wire Network Address */
   protected byte[] RomDta = new byte [8];

   /** Flag to indicate next search will look only for alarming devices */
   private boolean doAlarmSearch = false;

   /** Flag to indicate next search will be a 'first' */
   private boolean resetSearch = true;

   /** Flag to indicate next search will not be preceeded by a 1-Wire reset */
   private boolean skipResetOnSearch = false;

   private static final String driver_version = "V0.02";

   /** current adapter communication speed */
   private int speed = 0;

   private int portNum = -1;
   private int sessionHandle = -1;
   private boolean inExclusive = false;;

   private System.Text.StringBuilder mainVersionBuffer 
    = new System.Text.StringBuilder(SIZE_VERSION); 

   private System.Text.StringBuilder typeVersionBuffer 
    = new System.Text.StringBuilder(SIZE_VERSION); 


   private byte[] stateBuffer = new byte[SIZE_STATE];

   private boolean[] adapterSpecFeatures;
   private String adapterSpecDescription;

   /** token indexes into version string */
   private static final int TOKEN_ABRV =           0;
   private static final int TOKEN_DEV =            1;
   private static final int TOKEN_VER =            2;
   private static final int TOKEN_DATE =           3;
   private static final int TOKEN_TAIL =           255;
   /** constant for state buffer size */
   private static final int SIZE_STATE   =         5120;
   /** constant for size of version string */
   private static final int SIZE_VERSION =         256;
   /** constants for uninitialized ports */
   private static final int EMPTY_NEW   =          -2;
   private static final int EMPTY_FREED =          -1;


   //--------
   //-------- Constructors/Destructor
   //--------

   /**
    * Constructs a default adapter
    *
    * @throws ClassNotFoundException
    */
   public DotNetAdapterx86 ()
      throws ClassNotFoundException
   {
      // set default port type
      portType = getDefaultTypeNumber();

      // attempt to set the portType, will throw exception if does not exist
      //if (!setPortType_Native(getInfo(), portType))
      if (!setTMEXPortType(portType))
         throw new ClassNotFoundException("DotNet adapter x86 type does not exist");
   }

   /**
    * Constructs with a specified port type
    *
    *
    * @param newPortType
    * @throws ClassNotFoundException
    */
   public DotNetAdapterx86 (int newPortType)
      throws ClassNotFoundException
   {
      // set default port type
      portType = newPortType;

      // attempt to set the portType, will throw exception if does not exist
      //if (!setPortType_Native(getInfo(), portType))
      if (!setTMEXPortType(portType))
         throw new ClassNotFoundException("DotNet adapter type does not exist");
   }

   /**
    * Finalize to Cleanup native
    */
   protected void finalize ()
   {
      cleanup();
   }

   //--------
   //-------- Methods
   //--------

   /**
    * Retrieve the name of the port adapter as a string.  The 'Adapter'
    * is a device that connects to a 'port' that allows one to
    * communicate with an iButton or other 1-Wire device.  As example
    * of this is 'DS9097U'.
    *
    * @return  <code>String</code> representation of the port adapter.
    */
   public String getAdapterName ()
   {
      // get the adapter name from the version string
      return "{" + getToken(typeVersionBuffer.toString(), TOKEN_DEV) + "}";
   }

   /**
    * Retrieve a description of the port required by this port adapter.
    * An example of a 'Port' would 'serial communication port'.
    *
    * @return  <code>String</code> description of the port type required.
    */
   public String getPortTypeDescription ()
   {
      // get the abreviation from the version string
      String abrv = getToken(typeVersionBuffer.toString(),TOKEN_ABRV);

      // Change COMU to COM
      if (abrv.equals("COMU"))
         abrv = "COM";

      return abrv + " (native)";
   }


   /**
    * Retrieve a version string for this class.
    *
    * @return  version string
    */
   public String getClassVersion ()
   {
      // get the version from the version string
      String version = this.driver_version + ", native: IBFS32.dll("
         + getToken(this.mainVersionBuffer.toString(),TOKEN_VER)
         + ") [type" + this.portType + ":"
         + getToken(this.typeVersionBuffer.toString(),TOKEN_TAIL) + "]("
         + getToken(this.typeVersionBuffer.toString(),TOKEN_VER) + ")";

      return version;
   }

   //--------
   //-------- Port Selection
   //--------

   /**
    * Retrieve a list of the platform appropriate port names for this
    * adapter.  A port must be selected with the method 'selectPort'
    * before any other communication methods can be used.  Using
    * a communcation method before 'selectPort' will result in
    * a <code>OneWireException</code> exception.
    *
    * @return  enumeration of type <code>String</code> that contains the port
    * names
    */
   public Enumeration getPortNames ()
   {
      Vector portVector = new Vector();
      String header = getToken(typeVersionBuffer.toString(),TOKEN_ABRV);

      if(header.equals("COMU"))
         header = "COM";

      for (int i = 0; i < 16; i++)
         portVector.addElement(new String(header + Integer.toString(i)));

      return (portVector.elements());
   }

   /**
    * Specify a platform appropriate port name for this adapter.  Note that
    * even though the port has been selected, it's ownership may be relinquished
    * if it is not currently held in a 'exclusive' block.  This class will then
    * try to re-aquire the port when needed.  If the port cannot be re-aquired
    * ehen the exception <code>PortInUseException</code> will be thrown.
    *
    * @param  portName  name of the target port, retrieved from
    * getPortNames()
    *
    * @return <code>true</code> if the port was aquired, <code>false</code>
    * if the port is not available.
    *
    * @throws OneWireIOException If port does not exist, or unable to communicate with port.
    * @throws OneWireException If port does not exist
    */
   public boolean selectPort (String portName)
      throws OneWireIOException, OneWireException
   {
      int prtnum=0,i;
      boolean rt = false;

      // free the last port
      freePort();

      // get the abreviation from the version string
      String header = getToken(typeVersionBuffer.toString(), TOKEN_ABRV);

      // Change COMU to COM
      if (header.equals("COMU"))
         header = "COM";

      // loop to make sure that the begining of the port name matches the head
      for (i = 0; i < header.length(); i++)
      {
         if (portName.charAt(i) != header.charAt(i))
            return false;
      }

      // i now points to begining of integer (0 TO 15)
      if ((portName.charAt(i) >= '0') && (portName.charAt(i) <= '9'))
      {
         prtnum = portName.charAt(i) - '0';
         if ( ( (i+1) < portName.length()) && 
            (portName.charAt(i+1) >= '0') && (portName.charAt(i+1) <= '9'))
         {  
            prtnum *= 10;
            prtnum += portName.charAt(i+1) - '0';
         }

         if (prtnum > 15)
            return false;
      }

      // now have prtnum
      // get a session handle, 16 sec timeout
      long timeout = System.currentTimeMillis() + 16000;
      do
      {
         sessionHandle = TMEX_LIB_x86.TMExtendedStartSession(prtnum, portType, null);
         // this port type does not exist
         if (sessionHandle == -201)
            break;
            // valid handle
         else if (sessionHandle > 0)
         {
            // do setup
            if (TMEX_LIB_x86.TMSetup(sessionHandle) == 1)
            {
               // read the version again
               TMEX_LIB_x86.TMGetTypeVersion(portType, typeVersionBuffer); 
               byte[] specBuffer = new byte[319];
               // get the adapter spec
               TMEX_LIB_x86.TMGetAdapterSpec(sessionHandle, specBuffer);
               adapterSpecDescription
                  = TMEX_LIB_x86.getDescriptionFromSpecification(specBuffer);
               adapterSpecFeatures
                  = TMEX_LIB_x86.getFeaturesFromSpecification(specBuffer);

               // record the portnum
               this.portNum = (short)prtnum;
               // do a search ??????????????????????
               TMEX_LIB_x86.TMFirst(sessionHandle, stateBuffer);
               // return success
               rt = true;
            }

            break;
         }
      }
      while (timeout > System.currentTimeMillis());

      // close the session
      TMEX_LIB_x86.TMEndSession(sessionHandle);
      sessionHandle = 0;

      // check if session was not available
      if (!rt)
      {
         // free up the port
         freePort();
         // throw exception
         throw new OneWireException("1-Wire Net not available");
      }

      return rt; 
   }



   /**
    * Free ownership of the selected port if it is currently owned back
    * to the system.  This should only be called if the recently
    * selected port does not have an adapter or at the end of
    * your application's use of the port.
    *
    * @throws OneWireException If port does not exist
    */
   public void freePort ()
      throws OneWireException
   {
      // check for opened port
      if ((portNum >= 0) && (portType >= 0))
      {
         // get a session 
         if (get_session())
         {
            // clean up open port and sessions
            TMEX_LIB_x86.TMClose(sessionHandle);

            // release the session (forced, even if in exclusive)
            TMEX_LIB_x86.TMEndSession(sessionHandle);
            sessionHandle = 0;
            inExclusive = false;
         }
      }

      // set flag to indicate this port is now free
      portNum = EMPTY_FREED;
   }

   /**
    * Retrieve the name of the selected port as a <code>String</code>.
    *
    * @return  <code>String</code> of selected port
    *
    * @throws OneWireException if valid port not yet selected
    */
   public String getPortName ()
      throws OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
      {
         throw new OneWireException("Port not selected");
      }

      // get the abreviation from the version string
      String header = getToken(typeVersionBuffer.toString(), TOKEN_ABRV);

      // create the head and port number combo
      // Change COMU to COM
      if (header.equals("COMU"))
         header = "COM";

      return header + portNum;
   }

   //--------
   //-------- Adapter detection 
   //--------

   /**
    * Detect adapter presence on the selected port.
    *
    * @return  <code>true</code> if the adapter is confirmed to be connected to
    * the selected port, <code>false</code> if the adapter is not connected.
    *
    * @throws OneWireIOException
    * @throws OneWireException
    */
   public boolean adapterDetected ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // return success if both port num and type are known
      return ((portNum >= 0) && (portType >= 0));
   }

   /**
    * Retrieve the version of the adapter.
    *
    * @return  <code>String</code> of the adapter version.  It will return
    * "<na>" if the adapter version is not or cannot be known.
    *
    * @throws OneWireIOException on a 1-Wire communication error such as 
    *         no device present.  This could be
    *         caused by a physical interruption in the 1-Wire Network due to 
    *         static native shorts or a newly arriving 1-Wire device issuing a 'presence pulse'.
    * @throws OneWireException on a communication or setup error with the 1-Wire 
    *         adapter
    */
   public String getAdapterVersion ()
      throws OneWireIOException, OneWireException
   {
      if(portNum<0 || portType<0)
         return "<na>";
      else if (portType == 5)
      {  
         // only know this with DS9097U type 
         // get the adapter name from the version string
         String ver = getToken(typeVersionBuffer.toString(),TOKEN_TAIL);

         char rev = '0';
         int ch = ver.indexOf("Rev");
         if(ch>0)
            rev = ver.charAt(ch + 3);

         return "DS2480x revision " + rev + ", " + adapterSpecDescription;
      }
      else
         return adapterSpecDescription;
   }

   /**
    * Retrieve the address of the adapter if it has one.
    *
    * @return  <code>String</code> of the adapter address.  It will return "<na>" if
    * the adapter does not have an address.  The address is a string representation of an
    * 1-Wire address.
    *
    * @throws OneWireIOException on a 1-Wire communication error such as 
    *         no device present.  This could be
    *         caused by a physical interruption in the 1-Wire Network due to 
    *         static native shorts or a newly arriving 1-Wire device issuing a 'presence pulse'.
    * @throws OneWireException on a communication or setup error with the 1-Wire 
    *         adapter
    * @see    Address
    */
   public String getAdapterAddress ()
      throws OneWireIOException, OneWireException
   {
      return "<na>";   //??? implement later
   }

   //--------
   //-------- Adapter features 
   //--------

   /* The following interogative methods are provided so that client code
    * can react selectively to underlying states without generating an
    * exception.
    */

   /**
    * Returns whether adapter can physically support overdrive mode.
    *
    * @return  <code>true</code> if this port adapter can do OverDrive,
    * <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error with the adapter
    * @throws OneWireException on a setup error with the 1-Wire 
    *         adapter
    */
   public boolean canOverdrive ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      return (adapterSpecFeatures[TMEX_LIB_x86.FEATURE_OVERDRIVE]);
   }

   /**
    * Returns whether the adapter can physically support hyperdrive mode.
    *
    * @return  <code>true</code> if this port adapter can do HyperDrive,
    * <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error with the adapter
    * @throws OneWireException on a setup error with the 1-Wire 
    *         adapter
    */
   public boolean canHyperdrive ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      return false;
   }

   /**
    * Returns whether the adapter can physically support flex speed mode.
    *
    * @return  <code>true</code> if this port adapter can do flex speed,
    * <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error with the adapter
    * @throws OneWireException on a setup error with the 1-Wire 
    *         adapter
    */
   public boolean canFlex ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      return (adapterSpecFeatures[TMEX_LIB_x86.FEATURE_FLEX]);
   }


   /**
    * Returns whether adapter can physically support 12 volt power mode.
    *
    * @return  <code>true</code> if this port adapter can do Program voltage,
    * <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error with the adapter
    * @throws OneWireException on a setup error with the 1-Wire 
    *         adapter
    */
   public boolean canProgram ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      return (adapterSpecFeatures[TMEX_LIB_x86.FEATURE_PROGRAM]);
   }

   /**
    * Returns whether the adapter can physically support strong 5 volt power
    * mode.
    *
    * @return  <code>true</code> if this port adapter can do strong 5 volt
    * mode, <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error with the adapter
    * @throws OneWireException on a setup error with the 1-Wire 
    *         adapter
    */
   public boolean canDeliverPower ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      return (adapterSpecFeatures[TMEX_LIB_x86.FEATURE_POWER]);
   }

   /**
    * Returns whether the adapter can physically support "smart" strong 5
    * volt power mode.  "smart" power delivery is the ability to deliver
    * power until it is no longer needed.  The current drop it detected
    * and power delivery is stopped.
    *
    * @return  <code>true</code> if this port adapter can do "smart" strong
    * 5 volt mode, <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error with the adapter
    * @throws OneWireException on a setup error with the 1-Wire 
    *         adapter
    */
   public boolean canDeliverSmartPower ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      return false; // currently not implemented
   }

   /**
    * Returns whether adapter can physically support 0 volt 'break' mode.
    *
    * @return  <code>true</code> if this port adapter can do break,
    * <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error with the adapter
    * @throws OneWireException on a setup error with the 1-Wire 
    *         adapter
    */
   public boolean canBreak ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      return (adapterSpecFeatures[TMEX_LIB_x86.FEATURE_BREAK]);
   }

   //--------
   //-------- Finding iButtons and 1-Wire devices
   //--------

   /**
    * Returns <code>true</code> if the first iButton or 1-Wire device
    * is found on the 1-Wire Network.
    * If no devices are found, then <code>false</code> will be returned.
    *
    * @return  <code>true</code> if an iButton or 1-Wire device is found.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public boolean findFirstDevice ()
      throws OneWireIOException, OneWireException
   {
      // reset the internal rom buffer
      resetSearch = true;

      return findNextDevice();
   }

   /**
    * Returns <code>true</code> if the next iButton or 1-Wire device
    * is found. The previous 1-Wire device found is used
    * as a starting point in the search.  If no more devices are found
    * then <code>false</code> will be returned.
    *
    * @return  <code>true</code> if an iButton or 1-Wire device is found.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public boolean findNextDevice ()
      throws OneWireIOException, OneWireException
   {
      while (true)
      {
         short[] ROM = new short[8];

         // check if port is selected
         if ((portNum < 0) || (portType < 0))
            throw new OneWireException("Port not selected");

         // get a session 
         if (!get_session())
            throw new OneWireException("Port in use");

         short rt = TMEX_LIB_x86.TMSearch( sessionHandle, stateBuffer, 
            (short)(resetSearch?1:0), (short)(skipResetOnSearch?0:1), 
            (short)(doAlarmSearch ? 0xEC : 0xF0)); 

         // check for microlan exception
         if (rt < 0)
            throw new OneWireIOException("native TMEX_LIB_x86 error " + rt);

         // retrieve the ROM number found
         ROM[0] = 0;
         short romrt = TMEX_LIB_x86.TMRom(sessionHandle, stateBuffer, ROM);
         if (romrt == 1)
         {
            // copy to java array
            for (int i = 0; i < 8; i++)
               this.RomDta[i] = (byte)ROM[i];
         }
         else
            throw new OneWireIOException("native TMEX_LIB_x86 error " + romrt);

         // release the session
         release_session();

         if (rt>0)
         {
            resetSearch = false;

            // check if this is an OK family type
            if (isValidFamily(RomDta))
               return true;

            // Else, loop to the top and do another search.
         }
         else
         {
            resetSearch = true;

            return false;
         }
      }
   }

   /**
    * Copies the 'current' iButton address being used by the adapter into
    * the array.  This address is the last iButton or 1-Wire device found
    * in a search (findNextDevice()...).
    * This method copies into a user generated array to allow the
    * reuse of the buffer.  When searching many iButtons on the one
    * wire network, this will reduce the memory burn rate.
    *
    * @param  address An array to be filled with the current iButton address.
    * @see    Address
    */
   public void getAddress (byte[] address)
   {
      System.arraycopy(RomDta, 0, address, 0, 8);
   }

   /**
    * Verifies that the iButton or 1-Wire device specified is present on
    * the 1-Wire Network. This does not affect the 'current' device
    * state information used in searches (findNextDevice...).
    *
    * @param  address  device address to verify is present
    *
    * @return  <code>true</code> if device is present else
    *         <code>false</code>.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    *
    * @see    Address
    */
   public boolean isPresent (byte[] address)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      short[] ROM = new short[8];
      for(int i=0; i<8; i++)
         ROM[i] = address[i];

      // get the current rom to restore after isPresent() (1.01)
      short[] oldROM = new short[8];
      oldROM[0] = 0;
      TMEX_LIB_x86.TMRom(sessionHandle, stateBuffer, oldROM);
      // set this rom to TMEX_LIB_x86
      TMEX_LIB_x86.TMRom(sessionHandle, stateBuffer, ROM);
      // see if part this present
      int rt = TMEX_LIB_x86.TMStrongAccess(sessionHandle, stateBuffer);
      // restore  
      TMEX_LIB_x86.TMRom(sessionHandle, stateBuffer, oldROM);     
      // release the session
      release_session();

      // check for adapter communcication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);

      return (rt>0);
   }

   /**
    * Verifies that the iButton or 1-Wire device specified is present
    * on the 1-Wire Network and in an alarm state. This does not
    * affect the 'current' device state information used in searches
    * (findNextDevice...).
    *
    * @param  address  device address to verify is present and alarming
    *
    * @return  <code>true</code> if device is present and alarming else
    * <code>false</code>.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    *
    * @see    Address
    */
   public boolean isAlarming (byte[] address)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      short[] ROM = new short[8];
      for(int i=0; i<8; i++)
         ROM[i] = address[i];

      // get the current rom to restore after isPresent() (1.01)
      short[] oldROM = new short[8];
      oldROM[0] = 0;
      TMEX_LIB_x86.TMRom(sessionHandle, stateBuffer, oldROM);
      // set this rom to TMEX_LIB_x86
      TMEX_LIB_x86.TMRom(sessionHandle, stateBuffer, ROM);
      // see if part this present
      int rt = TMEX_LIB_x86.TMStrongAlarmAccess(sessionHandle, stateBuffer);
      // restore  
      TMEX_LIB_x86.TMRom(sessionHandle, stateBuffer, oldROM);     
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);

      return (rt>0);
   }

   /**
    * Selects the specified iButton or 1-Wire device by broadcasting its
    * address.  This operation is refered to a 'MATCH ROM' operation
    * in the iButton and 1-Wire device data sheets.  This does not
    * affect the 'current' device state information used in searches
    * (findNextDevice...).
    *
    * Warning, this does not verify that the device is currently present
    * on the 1-Wire Network (See isPresent).
    *
    * @param  address     iButton to select
    *
    * @return  <code>true</code> if device address was sent,<code>false</code>
    * otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    *
    * @see com.dalsemi.onewire.adapter.DSPortAdapter#isPresent(byte[] address)
    * @see  Address
    */
   public boolean select (byte[] address)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      byte[] send_block = new byte[9];
      send_block[0] = (byte)0x55;  // match command
      for (int i = 0; i < 8; i++)
         send_block[i + 1] = address[i];

      // Change to use a block, not TMRom/TMAccess
      int rt = TMEX_LIB_x86.TMBlockIO(sessionHandle, send_block, (short)9);
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for no device
      else if (rt == -7)
         throw new OneWireException("No device detected");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);

      return (rt>=1);
   }

   //--------
   //-------- Finding iButton/1-Wire device options 
   //--------

   /**
    * Set the 1-Wire Network search to find only iButtons and 1-Wire
    * devices that are in an 'Alarm' state that signals a need for
    * attention.  Not all iButton types
    * have this feature.  Some that do: DS1994, DS1920, DS2407.
    * This selective searching can be canceled with the
    * 'setSearchAllDevices()' method.
    *
    * @see #setNoResetSearch
    */
   public void setSearchOnlyAlarmingDevices ()
   {
      doAlarmSearch = true;
   }

   /**
    * Set the 1-Wire Network search to not perform a 1-Wire
    * reset before a search.  This feature is chiefly used with
    * the DS2409 1-Wire coupler.
    * The normal reset before each search can be restored with the
    * 'setSearchAllDevices()' method.
    */
   public void setNoResetSearch ()
   {
      skipResetOnSearch = true;
   }

   /**
    * Set the 1-Wire Network search to find all iButtons and 1-Wire
    * devices whether they are in an 'Alarm' state or not and
    * restores the default setting of providing a 1-Wire reset
    * command before each search. (see setNoResetSearch() method).
    *
    * @see #setNoResetSearch
    */
   public void setSearchAllDevices ()
   {
      doAlarmSearch     = false;
      skipResetOnSearch = false;
   }

   //--------
   //-------- 1-Wire Network Semaphore methods  
   //--------

   /**
    * Gets exclusive use of the 1-Wire to communicate with an iButton or
    * 1-Wire Device.
    * This method should be used for critical sections of code where a
    * sequence of commands must not be interrupted by communication of
    * threads with other iButtons, and it is permissible to sustain
    * a delay in the special case that another thread has already been
    * granted exclusive access and this access has not yet been
    * relinquished. <p>
    *
    * It can be called through the OneWireContainer
    * class by the end application if they want to ensure exclusive
    * use.  If it is not called around several methods then it
    * will be called inside each method.
    *
    * @param blocking <code>true</code> if want to block waiting
    *                 for an excluse access to the adapter
    * @return <code>true</code> if blocking was false and a
    *         exclusive session with the adapter was aquired
    *
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public boolean beginExclusive (boolean blocking)
      throws OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // check if already in exclusive block
      if (inExclusive)
      {
         // make sure still valid (if not get a new one)
         if (TMEX_LIB_x86.TMValidSession(sessionHandle)>0)
            return true;
      }

      // if in exclusive, no longer valid so clear it
      inExclusive = false;

      // check if blocking 
      if (blocking)
      {
         // loop forever until get a session
         while (!get_session());
         inExclusive = true;
      }
      else
      {
         // try once for handle (actually blocks for up to 2 seconds)
         if (get_session())
            inExclusive = true;
      }
      
      // throw the port in use exception if could not get a TMEX_LIB_x86 session
      if (!inExclusive)
         throw new OneWireException("Port in use");

      return inExclusive;
   }

   /**
    * Relinquishes exclusive control of the 1-Wire Network.
    * This command dynamically marks the end of a critical section and
    * should be used when exclusive control is no longer needed.
    */
   public void endExclusive ()
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         return;

      // clear any current exclusive
      inExclusive = false;

      // release the current handle
      release_session();
   }

   //--------
   //-------- Primitive 1-Wire Network data methods 
   //--------

   /**
    * Sends a bit to the 1-Wire Network.
    *
    * @param  bitValue  the bit value to send to the 1-Wire Network.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public void putBit (boolean bitValue)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      // do 1-Wire bit
      int rt = TMEX_LIB_x86.TMTouchBit(sessionHandle,(short)(bitValue?1:0));
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);

      if (bitValue != (rt>0))
         throw new OneWireIOException("Error during putBit()");
   }

   /**
    * Gets a bit from the 1-Wire Network.
    *
    * @return  the bit value recieved from the the 1-Wire Network.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public boolean getBit ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      // do 1-Wire bit
      int rt = TMEX_LIB_x86.TMTouchBit(sessionHandle, (short)1);
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);

      return (rt>0);
   }

   /**
    * Sends a byte to the 1-Wire Network.
    *
    * @param  byteValue  the byte value to send to the 1-Wire Network.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public void putByte (int byteValue)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      int rt = TMEX_LIB_x86.TMTouchByte(sessionHandle,(short)(0x0FF&byteValue));
      // release the session
      release_session();

      // check for adapter communcication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error " + rt);

      if (rt != ((0x00FF) & byteValue))
         throw new OneWireIOException(
            "Error during putByte(), echo was incorrect ");
   }

   /**
    * Gets a byte from the 1-Wire Network.
    *
    * @return  the byte value received from the the 1-Wire Network.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public int getByte ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      int rt = TMEX_LIB_x86.TMTouchByte(sessionHandle,(short)0x0FF);
      // release the session
      release_session();

      // check for adapter communcication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error " + rt);

      return rt; 
   }

   /**
    * Get a block of data from the 1-Wire Network.
    *
    * @param  len  length of data bytes to receive
    *
    * @return  the data received from the 1-Wire Network.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public byte[] getBlock (int len)
      throws OneWireIOException, OneWireException
   {
      byte[] barr = new byte [len];

      getBlock(barr, 0, len);

      return barr;
   }

   /**
    * Get a block of data from the 1-Wire Network and write it into
    * the provided array.
    *
    * @param  arr     array in which to write the received bytes
    * @param  len     length of data bytes to receive
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public void getBlock (byte[] arr, int len)
      throws OneWireIOException, OneWireException
   {
      getBlock(arr, 0, len);
   }

   /**
    * Get a block of data from the 1-Wire Network and write it into
    * the provided array.
    *
    * @param  arr     array in which to write the received bytes
    * @param  off     offset into the array to start
    * @param  len     length of data bytes to receive
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public void getBlock (byte[] arr, int off, int len)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      int rt;
      if(off==0)
      {
         for(int i=0; i<len; i++)
            arr[i] = (byte)0x0FF;
         rt =  TMEX_LIB_x86.TMBlockStream(sessionHandle, arr, (short)len);
         // release the session
         release_session();
      }
      else
      {
         byte[] dataBlock = new byte[len];
         for(int i=0; i<len; i++)
            dataBlock[i] = (byte)0x0FF;
         rt =  TMEX_LIB_x86.TMBlockStream(sessionHandle, dataBlock, (short)len);
         // release the session
         release_session();
         System.arraycopy(dataBlock, 0, arr, off, len);
      }

      // check for adapter communcication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error " + rt);
   }

   /**
    * Sends a block of data and returns the data received in the same array.
    * This method is used when sending a block that contains reads and writes.
    * The 'read' portions of the data block need to be pre-loaded with 0xFF's.
    * It starts sending data from the index at offset 'off' for length 'len'.
    *
    * @param  dataBlock  array of data to transfer to and from the 1-Wire Network.
    * @param  off        offset into the array of data to start
    * @param  len        length of data to send / receive starting at 'off'
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public void dataBlock (byte dataBlock [], int off, int len)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      int rt = 0;
      if(len>1023)
      {
         byte[] dataBlockBuffer = new byte[1023];
         // Change to only do 1023 bytes at a time
         int numblocks = len / 1023;
         int extra = len % 1023;
         for (int i = 0; i < numblocks; i++)
         {
            System.arraycopy(dataBlock, off + i*1023, dataBlockBuffer, 0, 1023);
            rt = TMEX_LIB_x86.TMBlockStream(sessionHandle, dataBlockBuffer, (short)1023);
            System.arraycopy(dataBlockBuffer, 0, dataBlock, off + i * 1023, 1023);
            if (rt != 1023)
               break;
         }
         if ((rt >= 0) && (extra>0))
         {
            System.arraycopy(dataBlock, off + numblocks*1023, dataBlockBuffer, 0, extra);
            rt =  TMEX_LIB_x86.TMBlockStream(sessionHandle,dataBlockBuffer,(short)extra);
            System.arraycopy(dataBlockBuffer, 0, dataBlock, off + numblocks*1023, extra);
         }
      }
      else if(off>0)
      {
         byte[] dataBlockOffset = new byte[len];
         System.arraycopy(dataBlock, off, dataBlockOffset, 0, len);
         rt =  TMEX_LIB_x86.TMBlockStream(sessionHandle, dataBlockOffset, (short)len);
         System.arraycopy(dataBlockOffset, 0, dataBlock, off, len);
      }
      else
      {
         rt =  TMEX_LIB_x86.TMBlockStream(sessionHandle, dataBlock, (short)len);
      }
      // release the session
      release_session();

      // check for adapter communcication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error " + rt);
   }

   /**
    * Sends a Reset to the 1-Wire Network.
    *
    * @return  the result of the reset. Potential results are:
    * <ul>
    * <li> 0 (RESET_NOPRESENCE) no devices present on the 1-Wire Network.
    * <li> 1 (RESET_PRESENCE) normal presence pulse detected on the 1-Wire
    *        Network indicating there is a device present.
    * <li> 2 (RESET_ALARM) alarming presence pulse detected on the 1-Wire
    *        Network indicating there is a device present and it is in the
    *        alarm condition.  This is only provided by the DS1994/DS2404
    *        devices.
    * <li> 3 (RESET_SHORT) inticates 1-Wire appears shorted.  This can be
    *        transient conditions in a 1-Wire Network.  Not all adapter types
    *        can detect this condition.
    * </ul>
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public int reset ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      // do 1-Wire reset
      int rt = TMEX_LIB_x86.TMTouchReset(sessionHandle);
      // release the session
      release_session();

      // check for adapter communcication problems
      if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error " + rt);
      else if(rt == 3)
         throw new OneWireIOException("1-Wire Net shorted");

      return rt;
   }

   //--------
   //-------- 1-Wire Network power methods  
   //--------

   /**
    * Sets the duration to supply power to the 1-Wire Network.
    * This method takes a time parameter that indicates the program
    * pulse length when the method startPowerDelivery().<p>
    *
    * Note: to avoid getting an exception,
    * use the canDeliverPower() and canDeliverSmartPower()
    * method to check it's availability. <p>
    *
    * @param timeFactor
    * <ul>
    * <li>   0 (DELIVERY_HALF_SECOND) provide power for 1/2 second.
    * <li>   1 (DELIVERY_ONE_SECOND) provide power for 1 second.
    * <li>   2 (DELIVERY_TWO_SECONDS) provide power for 2 seconds.
    * <li>   3 (DELIVERY_FOUR_SECONDS) provide power for 4 seconds.
    * <li>   4 (DELIVERY_SMART_DONE) provide power until the
    *          the device is no longer drawing significant power.
    * <li>   5 (DELIVERY_INFINITE) provide power until the
    *          setPowerNormal() method is called.
    * </ul>
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public void setPowerDuration (int timeFactor)
      throws OneWireIOException, OneWireException
   {
      // Right now we only support infinite pull up.
      if (timeFactor != DELIVERY_INFINITE)
         throw new OneWireException(
            "No support for other than infinite power duration");
   }

   /**
    * Sets the 1-Wire Network voltage to supply power to an iButton device.
    * This method takes a time parameter that indicates whether the
    * power delivery should be done immediately, or after certain
    * conditions have been met. <p>
    *
    * Note: to avoid getting an exception,
    * use the canDeliverPower() and canDeliverSmartPower()
    * method to check it's availability. <p>
    *
    * @param changeCondition
    * <ul>
    * <li>   0 (CONDITION_NOW) operation should occur immediately.
    * <li>   1 (CONDITION_AFTER_BIT) operation should be pending
    *           execution immediately after the next bit is sent.
    * <li>   2 (CONDITION_AFTER_BYTE) operation should be pending
    *           execution immediately after next byte is sent.
    * </ul>
    *
    * @return <code>true</code> if the voltage change was successful,
    * <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public boolean startPowerDelivery (int changeCondition)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      if(!adapterSpecFeatures[TMEX_LIB_x86.FEATURE_POWER])
         throw new OneWireException("Hardware option not available");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      // start 12Volt pulse
      int rt = TMEX_LIB_x86.TMOneWireLevel(sessionHandle,
         TMEX_LIB_x86.LEVEL_SET, TMEX_LIB_x86.LEVEL_STRONG_PULLUP, (short)changeCondition);
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -3)
         throw new OneWireException("Adapter type does not support power delivery");
      else if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);
         // check for could not set
      else if ((rt != TMEX_LIB_x86.LEVEL_STRONG_PULLUP) && (changeCondition == CONDITION_NOW))
         throw new OneWireIOException(
            "native TMEX_LIB_x86 error: could not set adapter to desired level: " + rt);

      return true;
   }

   /**
    * Sets the duration for providing a program pulse on the
    * 1-Wire Network.
    * This method takes a time parameter that indicates the program
    * pulse length when the method startProgramPulse().<p>
    *
    * Note: to avoid getting an exception,
    * use the canDeliverPower() method to check it's
    * availability. <p>
    *
    * @param timeFactor
    * <ul>
    * <li>   6 (DELIVERY_EPROM) provide program pulse for 480 microseconds
    * <li>   5 (DELIVERY_INFINITE) provide power until the
    *          setPowerNormal() method is called.
    * </ul>
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    */
   public void setProgramPulseDuration (int timeFactor)
      throws OneWireIOException, OneWireException
   {
      if (timeFactor != DELIVERY_EPROM)
         throw new OneWireException(
            "Only support EPROM length program pulse duration");
   }

   /**
    * Sets the 1-Wire Network voltage to eprom programming level.
    * This method takes a time parameter that indicates whether the
    * power delivery should be done immediately, or after certain
    * conditions have been met. <p>
    *
    * Note: to avoid getting an exception,
    * use the canProgram() method to check it's
    * availability. <p>
    *
    * @param changeCondition
    * <ul>
    * <li>   0 (CONDITION_NOW) operation should occur immediately.
    * <li>   1 (CONDITION_AFTER_BIT) operation should be pending
    *           execution immediately after the next bit is sent.
    * <li>   2 (CONDITION_AFTER_BYTE) operation should be pending
    *           execution immediately after next byte is sent.
    * </ul>
    *
    * @return <code>true</code> if the voltage change was successful,
    * <code>false</code> otherwise.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    *         or the adapter does not support this operation
    */
   public boolean startProgramPulse (int changeCondition)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      if(!adapterSpecFeatures[TMEX_LIB_x86.FEATURE_PROGRAM])
         throw new OneWireException("Hardware option not available");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      int rt;
      // if pulse is 'now' then use TMProgramPulse
      if (changeCondition == TMEX_LIB_x86.PRIMED_NONE)
      {
         rt = TMEX_LIB_x86.TMProgramPulse(sessionHandle);
         // change rt value to be compatible with TMOneWireLevel
         if (rt !=0 )
            rt = TMEX_LIB_x86.LEVEL_PROGRAM;
      }
      else
      {
         // start 12Volt pulse
         rt = TMEX_LIB_x86.TMOneWireLevel(sessionHandle,
            TMEX_LIB_x86.LEVEL_SET, TMEX_LIB_x86.LEVEL_PROGRAM, (short)changeCondition);
      }
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -3)
         throw new OneWireException("Adapter type does not support EPROM programming");
      else if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);
         // check for could not set
      else if ((rt != TMEX_LIB_x86.LEVEL_PROGRAM) && (changeCondition == CONDITION_NOW))
         throw new OneWireIOException(
            "native TMEX_LIB_x86 error: could not set adapter to desired level: " + rt);

      return true;
   }

   /**
    * Sets the 1-Wire Network voltage to 0 volts.  This method is used
    * rob all 1-Wire Network devices of parasite power delivery to force
    * them into a hard reset.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    *         or the adapter does not support this operation
    */
   public void startBreak ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      // start break
      int rt = TMEX_LIB_x86.TMOneWireLevel(sessionHandle,
         TMEX_LIB_x86.LEVEL_SET, TMEX_LIB_x86.LEVEL_BREAK, TMEX_LIB_x86.PRIMED_NONE);
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -3)
         throw new OneWireException("Adapter type does not support break");
      else if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);
         // check for could not set
      else if (rt != TMEX_LIB_x86.LEVEL_BREAK)
         throw new OneWireIOException(
            "native TMEX_LIB_x86 error: could not set adapter to break: " + rt);
   }

   /**
    * Sets the 1-Wire Network voltage to normal level.  This method is used
    * to disable 1-Wire conditions created by startPowerDelivery and
    * startProgramPulse.  This method will automatically be called if
    * a communication method is called while an outstanding power
    * command is taking place.
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    *         or the adapter does not support this operation
    */
   public void setPowerNormal ()
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      // set back to normal
      int rt = TMEX_LIB_x86.TMOneWireLevel(sessionHandle,
         TMEX_LIB_x86.LEVEL_SET, TMEX_LIB_x86.LEVEL_NORMAL, TMEX_LIB_x86.PRIMED_NONE);
      // release the session
      release_session();

      if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);
   }

   //--------
   //-------- 1-Wire Network speed methods 
   //--------

   /**
    * This method takes an int representing the new speed of data
    * transfer on the 1-Wire Network. <p>
    *
    * @param speed
    * <ul>
    * <li>     0 (SPEED_REGULAR) set to normal communciation speed
    * <li>     1 (SPEED_FLEX) set to flexible communciation speed used
    *            for long lines
    * <li>     2 (SPEED_OVERDRIVE) set to normal communciation speed to
    *            overdrive
    * <li>     3 (SPEED_HYPERDRIVE) set to normal communciation speed to
    *            hyperdrive
    * <li>    >3 future speeds
    * </ul>
    *
    * @param desiredSpeed
    *
    * @throws OneWireIOException on a 1-Wire communication error
    * @throws OneWireException on a setup error with the 1-Wire adapter
    *         or the adapter does not support this operation
    */
   public void setSpeed (int desiredSpeed)
      throws OneWireIOException, OneWireException
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         throw new OneWireException("Port not selected");

      // get a session 
      if (!get_session())
         throw new OneWireException("Port in use");

      short tmspeed = 0;
      // translate to TMEX_LIB_x86 speed
      switch (desiredSpeed)
      {
         case SPEED_FLEX:
            tmspeed = TMEX_LIB_x86.TIME_RELAXED;
            break;
         case SPEED_OVERDRIVE:
            tmspeed = TMEX_LIB_x86.TIME_OVERDRV;
            break;
         default:
         case SPEED_REGULAR:
            tmspeed = TMEX_LIB_x86.TIME_NORMAL; 
            break;
      };
      // change speed
      int rt = TMEX_LIB_x86.TMOneWireCom(sessionHandle, TMEX_LIB_x86.LEVEL_SET, tmspeed);
      // (1.01) if in overdrive then force an exclusive
      if (desiredSpeed == SPEED_OVERDRIVE)
         inExclusive = true;
      // release the session
      release_session();

      // check for adapter communication problems
      if (rt == -3)
         throw new OneWireException("Adapter type does not support selected speed");
      else if (rt == -12)
         throw new OneWireException("1-Wire Adapter communication exception");
         // check for microlan exception
      else if (rt < 0)
         throw new OneWireIOException("native TMEX_LIB_x86 error" + rt);
         // check for could not set
      else if (rt != tmspeed)
         throw new OneWireIOException(
            "native TMEX_LIB_x86 error: could not set adapter to desired speed: " + rt);

      this.speed = desiredSpeed;
   }


   /**
    * This method returns the current data transfer speed through a
    * port to a 1-Wire Network. <p>
    *
    * @return
    * <ul>
    * <li>     0 (SPEED_REGULAR) set to normal communication speed
    * <li>     1 (SPEED_FLEX) set to flexible communication speed used
    *            for long lines
    * <li>     2 (SPEED_OVERDRIVE) set to normal communication speed to
    *            overdrive
    * <li>     3 (SPEED_HYPERDRIVE) set to normal communication speed to
    *            hyperdrive
    * <li>    >3 future speeds
    * </ul>
    */
   public int getSpeed ()
   {
      // check if port is selected
      if ((portNum < 0) || (portType < 0))
         return SPEED_REGULAR;

      // get a session 
      if (!get_session())
         return SPEED_REGULAR;

      // change speed
      int rt = TMEX_LIB_x86.TMOneWireCom(sessionHandle, 
         (short)TMEX_LIB_x86.LEVEL_READ, (short)0);

      // release the session
      release_session();

      // translate to OWAPI speed
      switch (rt)
      {
         case TMEX_LIB_x86.TIME_RELAXED:
            speed = SPEED_FLEX; 
            break;
         case TMEX_LIB_x86.TIME_OVERDRV: 
            speed = SPEED_OVERDRIVE; 
            break;
         default:
            // note that this function will return SPEED_REGULAR if
            // an adapter communication exception occurred when
            // TMOneWireCom is called
         case TMEX_LIB_x86.TIME_NORMAL:
            speed = SPEED_REGULAR; 
            break;
      }
   
      return speed;
   }

   //--------
   //-------- Misc 
   //--------

   /**
    * Select the DotNet specified port type (0 to 15)  Use this
    * method if the constructor with the PortType cannot be used.
    *
    *
    * @param newPortType
    * @return  true if port type valid.  Instance is only usable
    *          if this returns false.
    */
   public boolean setTMEXPortType (int newPortType)
   {
      // check if already have a session handle open on old port
      if (this.sessionHandle>0)
         TMEX_LIB_x86.TMEndSession(sessionHandle);

      this.sessionHandle = 0;
      this.inExclusive = false;

      // read the version strings
      TMEX_LIB_x86.Get_Version(this.mainVersionBuffer); 
	  // will fail if not valid port type
	  if (TMEX_LIB_x86.TMGetTypeVersion(newPortType, this.typeVersionBuffer) > 0) 
      {
		 // set default port type
         portType = newPortType;
         return true;
      }
      return false;
   }

   //--------
   //-------- Additional Native Methods 
   //--------

   /**
    * CleanUp the native state for classes owned by the provided
    * thread.
    */
   public static void CleanUpByThread (Thread thread)
   {
      //cleanup();
   }

   /**
    * Get the default Adapter Name.
    *
    * @return  String containing the name of the default adapter
    */
   public static String getDefaultAdapterName ()
   {
      short portNumRef[] = new short[1];
      short portTypeRef[] = new short[1];
	   System.Text.StringBuilder version = new System.Text.StringBuilder(255);
      String adapterName = "<na>";

      // get the default num/type
      if (TMEX_LIB_x86.TMReadDefaultPort(portNumRef, portTypeRef) == 1) 
      {
         // read the version again
         if (TMEX_LIB_x86.TMGetTypeVersion(portTypeRef[0],version) == 1)
         {
            adapterName = getToken(version.toString(),TOKEN_DEV);
         }
      }

      return "{" + adapterName + "}";
   }

   /**
    * Get the default Adapter Port name.
    *
    * @return  String containing the name of the default adapter port
    */
   public static String getDefaultPortName ()
   {
      short portNumRef[] = new short[1];
      short portTypeRef[] = new short[1];
	   System.Text.StringBuilder version = new System.Text.StringBuilder(255);
      String portName = "<na>";

      // get the default num/type
      if (TMEX_LIB_x86.TMReadDefaultPort(portNumRef, portTypeRef) == 1) 
      {
         // read the version again
         if (TMEX_LIB_x86.TMGetTypeVersion(portTypeRef[0],version) == 1)
         {
            // get the abreviation from the version string
            String header = getToken(version.toString(),TOKEN_ABRV);

            // create the head and port number combo
            // Change COMU to COM
            if (header.equals("COMU"))
               portName = "COM" + portNumRef[0];
            else
               portName = header + portNumRef[0];
         }
      }

      return portName;
   }

   /**
    * Get the default Adapter Type number.
    *
    * @return  int, the default adapter type
    */
   private static int getDefaultTypeNumber ()
   {
      short[] PortNum = new short[1];
      short[] PortType = new short[1];
	   System.Text.StringBuilder zver = new System.Text.StringBuilder(255);

      // get the default num/type
      if (TMEX_LIB_x86.TMReadDefaultPort(PortNum, PortType) == 1) 
      {
         // read the version again
         if (TMEX_LIB_x86.TMGetTypeVersion(PortType[0],zver) == 1)
         {
            return (int)PortType[0];
         }
      }

      return 5;  // fail so at least do DS9097U default type
   }

   /**
    * Cleanup native (called on finalize of this instance)
    */
   private void cleanup ()
   {
      // used to clear 'class state' variables, which aren't around anymore
      try
      {
         freePort();
      }
      catch(Exception e) {;}
   }


   //--------------------------------------------------------------------------
   // Parse the version string for a token
   //
   private static final String getToken(String verStr, int token)
   {
      int currentToken=-1;
      boolean inToken = false;;
      String toReturn = "";

      for (int i = 0; i < verStr.length(); i++)
      {
         if ((verStr.charAt(i) != ' ') && (!inToken)) 
         {
            inToken = true;
            currentToken++;
         }

         if ((verStr.charAt(i) == ' ') && (inToken)) 
            inToken = false;

         if (((inToken) && (currentToken == token)) ||
            ((token == 255) && (currentToken > TOKEN_DATE))) 
            toReturn += verStr.charAt(i);
      }
      return toReturn;
   }

   /**
    * Attempt to get a TMEX_LIB_x86 session.  If already in an 'exclusive' block
    * then just return.  
    */
   private final synchronized boolean get_session()
   {
      int sessionOptions[] = new int[]{ TMEX_LIB_x86.SESSION_INFINITE };

      // check if in exclusive block
      if (inExclusive)
      {
         // make sure still valid (if not get a new one)
         if (TMEX_LIB_x86.TMValidSession(sessionHandle)>0)
            return true;
      }

      
      // attempt to get a session handle (2 sec timeout)
      //long timeout = System.currentTimeMillis() + 2000;
      long timeout = (new java.util.Date()).getTime() + 2000;
      do
      {
         sessionHandle 
            = TMEX_LIB_x86.TMExtendedStartSession(
            portNum, portType, sessionOptions);

         // this port type does not exist
         if (sessionHandle == -201)
            break;
            // valid handle
         else if (sessionHandle > 0)
            // success
            return true;
      }
      while (timeout > System.currentTimeMillis());

      // timeout of invalid porttype
      sessionHandle = 0;

      return false;
   }

   /**
    *  Release a TMEX_LIB_x86 session.  If already in an 'exclusive' block
    *  then just return.  
    */
   private final synchronized boolean release_session()
   {
      // check if in exclusive block
      if (inExclusive)
         return true;

      // close the session
      TMEX_LIB_x86.TMEndSession(sessionHandle);

      // clear out handle (used to indicate not session)
      sessionHandle = 0;

      return true;
   }

}

class TMEX_LIB_x86
{
   /** feature indexes into adapterSpecFeatures array */
   public static final int FEATURE_OVERDRIVE = 0;
   public static final int FEATURE_POWER = 1;
   public static final int FEATURE_PROGRAM = 2;
   public static final int FEATURE_FLEX = 3;
   public static final int FEATURE_BREAK = 4;
   /** Speed settings for TMOneWireCOM */
   public static final short TIME_NORMAL = 0;
   public static final short TIME_OVERDRV = 1;
   public static final short TIME_RELAXED = 2;
   /* for TMOneWireLevel */
   public static final short LEVEL_NORMAL = 0;
   public static final short LEVEL_STRONG_PULLUP = 1;
   public static final short LEVEL_BREAK = 2;
   public static final short LEVEL_PROGRAM = 3;
   public static final short PRIMED_NONE = 0;
   public static final short PRIMED_BIT = 1;
   public static final short PRIMED_BYTE = 2;
   public static final short LEVEL_READ = 1;
   public static final short LEVEL_SET = 0;
   /** session options */
   public static final int SESSION_INFINITE = 1;
   public static final int SESSION_RSRC_RELEASE = 2;

   //---------------------------------------------------------------------------
   // TMEX_LIB_x86 - Session

   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native int TMExtendedStartSession(int portNum, int portType, int[] sessionOptions);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native int TMStartSession(int i1);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMValidSession(int sessionHandle);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMEndSession(int sessionHandle);

   // TMEX_LIB_x86 - Network
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMFirst
      (int sessionHandle, byte[] stateBuffer);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMNext
      (int sessionHandle, byte[] stateBuffer);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMAccess
      (int sessionHandle, byte[] stateBuffer);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMStrongAccess
      (int sessionHandle, byte[] stateBuffer);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMStrongAlarmAccess
      (int sessionHandle, byte[] stateBuffer);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMOverAccess
      (int sessionHandle, byte[] stateBuffer);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMRom(int sessionHandle,
      byte[] stateBuffer, short[] ROM);
   //public static native short TMFirstAlarm(long, void *);
   //public static native short TMNextAlarm(long, void *);
   //public static native short TMFamilySearchSetup(long, void *, short);
   //public static native short TMSkipFamily(long, void *);
   //public static native short TMAutoOverDrive(long, void *, short);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMSearch(int sessionHandle,
      byte[] stateBuffer, short doResetFlag, short skipResetOnSearchFlag,
      short searchCommand);

   // TMEX_LIB_x86 - transport

   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMBlockIO(int sessionHandle, byte[] dataBlock, short len);

   // TMEX_LIB_x86 - Hardware Specific

   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMSetup(int sessionHandle);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMTouchReset(int sessionHandle);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMTouchByte(int sessionHandle, short byteValue);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMTouchBit(int sessionHandle, short bitValue);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMProgramPulse(int sessionHandle);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMClose(int sessionHandle);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMOneWireCom
      (int sessionHandle, short command, short argument);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMOneWireLevel
      (int sessionHandle, short command, short argument, short changeCondition);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMGetTypeVersion
      (int portType, System.Text.StringBuilder sbuff);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short Get_Version(System.Text.StringBuilder sbuff);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMBlockStream
      (int sessionHandle, byte[] dataBlock, short len);
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMGetAdapterSpec
      (int sessionHandle, byte[] adapterSpec);/*byte[319]*/
   /** @attribute DllImport("ibfs32.dll") CharSet=CharSet.Auto */
   public static native short TMReadDefaultPort
      (short[] portTypeRef, short[] portNumRef);
   //---------------------------------------------------------------------------

   public static boolean[] getFeaturesFromSpecification(byte[] adapterSpec)
   {
      boolean[] features = new boolean[32];
      for (int i = 0; i < 32; i++)
         features[i] = (adapterSpec[i * 2] > 0) || (adapterSpec[i * 2 + 1] > 0);
      return features;
   }

   public static String getDescriptionFromSpecification(byte[] adapterSpec)
   {
      int i;
      // find null terminator for string
      for (i = 64; i < 319; i++)
         if (adapterSpec[i] == 0)
            break;
      return new String(adapterSpec, 64, i - 64);
   }
}