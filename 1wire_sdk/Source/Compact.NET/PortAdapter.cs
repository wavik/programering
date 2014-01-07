/*---------------------------------------------------------------------------
* Copyright (C) 2006-2008 Maxim Integrated Products, All Rights Reserved.
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
//using System.Runtime.Serialization;
using DalSemi.Serial;

namespace DalSemi.OneWire.Adapter
{
   #region enums
   /// <summary>
   /// Indicates the communication speed of the 1-Wire line
   /// </summary>
   public enum OWSpeed:int
   {
      /// <summary>Speed modes for 1-Wire Network, regular                    </summary>
      SPEED_REGULAR = 0,
      /// <summary>Speed modes for 1-Wire Network, overdrive                  </summary>
      SPEED_OVERDRIVE = 1,
      /// <summary>Speed modes for 1-Wire Network, flexible for long lines    </summary>
      SPEED_FLEX = 2,
      /// <summary>Speed modes for 1-Wire Network, hyperdrive                 </summary>
      SPEED_HYPERDRIVE = 3
   }

   /// <summary>
   /// Indicates the power level of the 1-Wire line
   /// </summary>
   public enum OWLevel:int
   {
      /// <summary>1-Wire Network level, normal (weak 5Volt pullup)                            </summary>
      LEVEL_NORMAL = 0,
      /// <summary>1-Wire Network level, (strong 5Volt pullup, used for power delivery) </summary>
      LEVEL_POWER_DELIVERY = 1,
      /// <summary>1-Wire Network level, (strong pulldown to 0Volts, reset 1-Wire)      </summary>
      LEVEL_BREAK = 2,
      /// <summary>1-Wire Network level, (strong 12Volt pullup, used to program eprom ) </summary>
      LEVEL_PROGRAM = 3
   }
	
   /// <summary>
   /// Indicates result of 1-Wire line reset
   /// </summary>
   public enum OWResetResult:int
   {
      /// <summary>1-Wire Network reset result = no presence </summary>
      RESET_NOPRESENCE = 0x00,
      /// <summary>1-Wire Network reset result = presence    </summary>
      RESET_PRESENCE = 0x01,
      /// <summary>1-Wire Network reset result = alarm       </summary>
      RESET_ALARM = 0x02,
      /// <summary>1-Wire Network reset result = shorted     </summary>
      RESET_SHORT = 0x03
   }
		
   /// <summary>
   /// Indicates the change condition to begin power delivery
   /// </summary>
   public enum OWPowerStart:int
   {
      /// <summary>Condition for power state change, immediate                      </summary>
      CONDITION_NOW = 0,
      /// <summary>Condition for power state change, after next bit communication   </summary>
      CONDITION_AFTER_BIT = 1,
      /// <summary>Condition for power state change, after next byte communication  </summary>
      CONDITION_AFTER_BYTE = 2
   }

   /// <summary>
   /// Indicates the amount of time to deliver power
   /// </summary>
   public enum OWPowerTime:int
   {
      /// <summary>Duration used in delivering power to the 1-Wire, 1/2 second         </summary>
      DELIVERY_HALF_SECOND = 0,
      /// <summary>Duration used in delivering power to the 1-Wire, 1 second           </summary>
      DELIVERY_ONE_SECOND = 1,
      /// <summary>Duration used in delivering power to the 1-Wire, 2 seconds          </summary>
      DELIVERY_TWO_SECONDS = 2,
      /// <summary>Duration used in delivering power to the 1-Wire, 4 second           </summary>
      DELIVERY_FOUR_SECONDS = 3,
      /// <summary>Duration used in delivering power to the 1-Wire, smart complete     </summary>
      DELIVERY_SMART_DONE = 4,
      /// <summary>Duration used in delivering power to the 1-Wire, infinite           </summary>
      DELIVERY_INFINITE = 5,
      /// <summary>Duration used in delivering power to the 1-Wire, current detect     </summary>
      DELIVERY_CURRENT_DETECT = 6,
      /// <summary>Duration used in delivering power to the 1-Wire, 480 us             </summary>
      DELIVERY_EPROM = 7
   }   
   #endregion

   #region exceptions
   /// <summary>
   /// Exception object thrown by all PortAdapters, to represent adapter communication
   /// exceptions
   /// </summary>
   public class AdapterException:Exception
   {
      /// <summary>
      /// constructs exception with the given message
      /// </summary>
      /// <param name="msg"></param>
      public AdapterException(String msg):
         base(msg)
      {}

      /// <summary>
      /// constructs exception with the given inner exception
      /// </summary>
      /// <param name="ex"></param>
      public AdapterException(Exception ex):
         base("AdapterException: " + ex.Message, ex)
      {}

      /// <summary>
      /// Constructs exception with the given message, and given internal exception
      /// </summary>
      /// <param name="msg"></param>
      /// <param name="ex"></param>
      public AdapterException(String msg, Exception ex):
         base(msg, ex)
      {}
   }
   #endregion

   #region events

   /// <summary>
   /// Argument for event handler for 1-Wire reset events, holds result of reset.
   /// </summary>
   public class ResetEventInfo:EventArgs
   {
      internal OWResetResult result;

      internal ResetEventInfo(OWResetResult resetResult)
      {
         this.result = resetResult;
      }

      /// <summary>
      /// Result of 1-Wire Reset.
      /// </summary>
      public OWResetResult Result
      {
         get{ return result; }
      }
   }

   /// <summary>
   /// Argument for event handler for 1-Wire speed change events, holds old and new speed.
   /// </summary>
   public class SpeedChangeEventInfo:EventArgs
   {
      internal OWSpeed oldSpeed;
      internal OWSpeed newSpeed;

      internal SpeedChangeEventInfo(OWSpeed OldSpeed, OWSpeed NewSpeed)
      {
         this.oldSpeed = OldSpeed;
         this.newSpeed = NewSpeed;
      }

      /// <summary>
      /// Speed of 1-Wire before change
      /// </summary>
      public OWSpeed OldSpeed
      {
         get{ return oldSpeed; }
      }
      /// <summary>
      /// Speed of 1-Wire after change
      /// </summary>
      public OWSpeed NewSpeed
      {
         get{ return newSpeed; }
      }
   }

   /// <summary>
   /// Argument for event handler for 1-Wire data IO events
   /// </summary>
   public class DataIOEventInfo:EventArgs
   {
      internal bool transmit = false;
      internal byte[] data = null;

      internal DataIOEventInfo(bool tmit, byte[] d)
      {
         transmit = tmit;
         data = (byte[])d.Clone();
      }

      /// <summary>
      /// If true, this is a transmit event, other it is a receive event (or echo event)
      /// </summary>
      public bool IsTransmit
      {
         get { return transmit; }
      }

      /// <summary>
      /// Data either transmitted or received
      /// </summary>
      public byte[] Data
      {
         get { return data; }
      }
   }

   /// <summary>
   /// Event handler for 1-Wire Reset events
   /// </summary>
   public delegate void ResetEventHandler(object src, ResetEventInfo args);

   /// <summary>
   /// Event handler for 1-Wire Speed change events
   /// </summary>
   public delegate void SpeedChangeEventHandler(object src, SpeedChangeEventInfo args);

   /// <summary>
   /// Event handler for 1-Wire IOEvents events
   /// </summary>
   public delegate void DataIOEventHandler(object src, DataIOEventInfo args);

   #endregion


   /// <summary>
   /// Abstract base class for all 1-Wire Adapter objects.
   /// </summary>
   public abstract class PortAdapter : IDisposable
   {

      #region open, close, and free

      /// <summary>
      /// Gets or sets the ISite associated with this component
      /// </summary>
      //public abstract System.ComponentModel.ISite Site {get; set;}

      /// <summary>
      /// Represents the method that handles the Disposed event of a component.
      /// </summary>
      //public event EventHandler Disposed;

      /// <summary>
      /// Dispose's all resources for this object
      /// </summary>
      public abstract void Dispose(); // !!!
      /// <summary>
      /// Dispose's all resources for this object
      /// </summary>
      protected abstract void Dispose(bool disposing); // !!!

      /// <summary>
      /// Opens the specified port and verifies existance of the adapter
      /// </summary>
      /// <returns>true if adapter found on specified port</returns>
      public abstract bool OpenPort(String PortName);
      /// <summary>
      /// Closes the port and frees all resources from use
      /// </summary>
      public void FreePort()
      {
         Dispose();
      }
      #endregion

      #region event handlers and fire methods
      
      /// <summary>
      /// An event for indicating when a 1-Wire Reset occurs and what the result is.
      /// </summary>
      public event ResetEventHandler ResetEvent;

      /// <summary>
      /// An event for indicating when a 1-Wire Reset occurs and what the result is.
      /// </summary>
      public event SpeedChangeEventHandler SpeedChangeEvent;

      /// <summary>
      /// event for indicating when data is transmitted
      /// </summary>
      public event DataIOEventHandler DataIOEvent;


      /// <summary>
      /// onReset event, fires event for all listeners
      /// </summary>
      /// <param name="rslt">the result of the 1-Wire reset</param>
      protected virtual void onReset(OWResetResult rslt)
      {
         if(ResetEvent!=null)
            ResetEvent(this, new ResetEventInfo(rslt));
      }

      /// <summary>
      /// onSpeedChange event, fires event for all listeners
      /// </summary>
      /// <param name="OldSpeed">the 1-Wire speed before change</param>
      /// <param name="NewSpeed">the 1-Wire speed after change</param>
      protected virtual void onSpeedChange(OWSpeed OldSpeed, OWSpeed NewSpeed)
      {
         if(SpeedChangeEvent!=null)
            SpeedChangeEvent(this, new SpeedChangeEventInfo(OldSpeed, NewSpeed));
      }

      /// <summary>
      /// onDataIO event, fires for all data IO
      /// </summary>
      /// <param name="isTransmit">if true, this is a transmit event, otherwise it is echo</param>
      /// <param name="data">The data transmitted or received</param>
      protected virtual void onDataIO(bool isTransmit, byte[] data)
      {
         if(DataIOEvent!=null)
            DataIOEvent(this, new DataIOEventInfo(isTransmit, data));
      }
      
      #endregion
      
      #region semaphores
      private System.Threading.Mutex exclusiveMutex = new System.Threading.Mutex(false);
      private int exclusiveCount = 0;
      private Object BeginLock = new Object();

      /// <summary>
      /// Obtain exclusive control of this adapter object
      /// </summary>
      /// <param name="blocking">if true, blocks until available</param>
      /// <returns>if true, exclusive control has been granted</returns>
      public bool BeginExclusive(bool blocking)
      {
         /* not CF friendly
         if(!blocking)
            return lockObject.WaitOne(1000, true);
         else
            return lockObject.WaitOne();
         */
         if(blocking || exclusiveCount==0)
         {
            lock(BeginLock)
            {
               if(blocking || exclusiveCount==0)
               {
                  if(exclusiveMutex.WaitOne())
                  {
                     exclusiveCount++;
                     return true;
                  }
               }
            }
         }
         return false;
      }

      /// <summary>
      /// Release exclusive control of this adapter object
      /// </summary>
      public void EndExclusive()
      {
         if(exclusiveCount>0)
         {
            try
            {
               // throws exception if not owned
               exclusiveMutex.ReleaseMutex();
               // won't execute if above failes
               exclusiveCount--;
            }
            catch
            {}
         }
      }
      #endregion

      #region 1-Wire I/O

      /// <summary> Sends a Reset to the 1-Wire Network.
      /// </summary>
      /// <returns>  the result of the reset.
      /// </returns>
      public abstract OWResetResult Reset();

      /// <summary> Sends a bit to the 1-Wire Network.
      /// 
      /// </summary>
      /// <param name="bitValue"> the bit value to send to the 1-Wire Network.
      /// </param>
      public abstract void PutBit(bool bitValue);

      /// <summary> Sends a byte to the 1-Wire Network.
      /// 
      /// </summary>
      /// <param name="byteValue"> the byte value to send to the 1-Wire Network.
      /// </param>
      public abstract void PutByte(int byteValue);

      /*
      /// <summary> Sends a block of data to the 1-Wire Network.
      /// 
      /// </summary>
      /// <param name="arr">array which contains the bytes to write
      /// </param>
      public abstract void PutBlock(byte[] bytes);

      /// <summary> Sends a block of data to the 1-Wire Network.
      /// 
      /// </summary>
      /// <param name="arr">array which contains the bytes to write
      /// </param>
      /// <param name="len">length of data bytes to write
      /// </param>
      public abstract void PutBlock(byte[] bytes, int len);

      /// <summary> Sends a block of data to the 1-Wire Network.
      /// 
      /// </summary>
      /// <param name="arr">array which contains the bytes to write
      /// </param>
      /// <param name="off">offset into the array to start
      /// </param>
      /// <param name="len">length of data bytes to write
      /// </param>
      public abstract void PutBlock(byte[] bytes, int off, int len);
      */

      /// <summary> Gets a bit from the 1-Wire Network.
      /// 
      /// </summary>
      /// <returns>  the bit value recieved from the the 1-Wire Network.
      /// </returns>
      public abstract bool GetBit();
      
      /// <summary> Gets a byte from the 1-Wire Network.
      /// 
      /// </summary>
      /// <returns>  the byte value received from the the 1-Wire Network.
      /// </returns>
      public abstract int GetByte();
      
      /// <summary> Get a block of data from the 1-Wire Network.
      /// 
      /// </summary>
      /// <param name="len"> length of data bytes to receive
      /// </param>
      /// <returns>  the data received from the 1-Wire Network.
      /// </returns>
      public abstract byte[] GetBlock(int len);
      
      /// <summary> Get a block of data from the 1-Wire Network and write it into
      /// the provided array.
      /// 
      /// </summary>
      /// <param name="arr">    array in which to write the received bytes
      /// </param>
      /// <param name="len">    length of data bytes to receive
      /// </param>
      public abstract void GetBlock(byte[] arr, int len);

      /// <summary> Get a block of data from the 1-Wire Network and write it into
      /// the provided array.
      /// 
      /// </summary>
      /// <param name="arr">    array in which to write the received bytes
      /// </param>
      /// <param name="off">    offset into the array to start
      /// </param>
      /// <param name="len">    length of data bytes to receive
      /// </param>
      public abstract void GetBlock(byte[] arr, int off, int len);

      /// <summary> Sends a block of data and returns the data received in the same array.
      /// This method is used when sending a block that contains reads and writes.
      /// The 'read' portions of the data block need to be pre-loaded with 0xFF's.
      /// It starts sending data from the index at offset 'off' for length 'len'.
      /// 
      /// </summary>
      /// <param name="data">array of data to transfer to and from the 1-Wire Network.
      /// </param>
      /// <param name="offset">offset into the array of data to start
      /// </param>
      /// <param name="length">length of data to send / receive starting at 'off'
      /// </param>
      public abstract void DataBlock(byte[] data, int offset, int length);

      #endregion

      #region Communication Speed
      /// <summary>OWSpeed representing current speed of communication on 1-Wire network
      /// </summary>
      public abstract OWSpeed Speed
      { set; get; }
      #endregion

      #region Power Delivery

      /// <summary> Sets the duration to supply power to the 1-Wire Network.
      /// This method takes a time parameter that indicates the program
      /// pulse length when the method startPowerDelivery().
      /// 
      /// Note: to avoid getting an exception,
      /// use the canDeliverPower() and canDeliverSmartPower()
      /// method to check it's availability.
      /// 
      /// </summary>
      /// <param name="powerDur">time factor
      /// </param>
      public abstract void SetPowerDuration(OWPowerTime powerDur);
      
      /// <summary> Sets the 1-Wire Network voltage to supply power to an iButton device.
      /// This method takes a time parameter that indicates whether the
      /// power delivery should be done immediately, or after certain
      /// conditions have been met.
      /// 
      /// Note: to avoid getting an exception,
      /// use the canDeliverPower() and canDeliverSmartPower()
      /// method to check it's availability.
      /// 
      /// </summary>
      /// <param name="changeCondition">change condition
      /// </param>
      /// <returns> <code>true</code> if the voltage change was successful,
      /// <code>false</code> otherwise.
      /// </returns>
      public abstract bool StartPowerDelivery(OWPowerStart changeCondition);
		
      /// <summary> Sets the duration for providing a program pulse on the
      /// 1-Wire Network.
      /// This method takes a time parameter that indicates the program
      /// pulse length when the method startProgramPulse().
      /// 
      /// Note: to avoid getting an exception,
      /// use the canDeliverPower() method to check it's
      /// availability.
      /// 
      /// </summary>
      /// <param name="pulseDur">time factor
      /// </param>
      public abstract void SetProgramPulseDuration(OWPowerTime pulseDur);
      
      /// <summary> Sets the 1-Wire Network voltage to eprom programming level.
      /// This method takes a time parameter that indicates whether the
      /// power delivery should be done immediately, or after certain
      /// conditions have been met.
      /// 
      /// Note: to avoid getting an exception,
      /// use the canProgram() method to check it's
      /// availability.
      /// 
      /// </summary>
      /// <param name="changeCondition">change condition
      /// </param>
      /// <returns> <code>true</code> if the voltage change was successful,
      /// <code>false</code> otherwise.
      /// 
      /// @throws OneWireIOException on a 1-Wire communication error
      /// @throws OneWireException on a setup error with the 1-Wire adapter
      /// or the adapter does not support this operation
      /// </returns>
      public abstract bool StartProgramPulse(OWPowerStart changeCondition);
		

      /// <summary> Sets the 1-Wire Network voltage to 0 volts.  This method is used
      /// rob all 1-Wire Network devices of parasite power delivery to force
      /// them into a hard reset.
      /// </summary>
      public abstract void StartBreak();


      /// <summary> Sets the 1-Wire Network voltage to normal level.  This method is used
      /// to disable 1-Wire conditions created by startPowerDelivery and
      /// startProgramPulse.  This method will automatically be called if
      /// a communication method is called while an outstanding power
      /// command is taking place.
      /// 
      /// @throws OneWireIOException on a 1-Wire communication error
      /// @throws OneWireException on a setup error with the 1-Wire adapter
      /// or the adapter does not support this operation
      /// </summary>
      public abstract void SetPowerNormal();

      #endregion

      #region Adapter Features

      /// <summary> Returns whether adapter can physically support overdrive mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do OverDrive,
      /// <code>false</code> otherwise.
      /// </returns>
      public abstract bool CanOverdrive{ get; }
		
      /// <summary> Returns whether the adapter can physically support hyperdrive mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do HyperDrive,
      /// <code>false</code> otherwise.
      /// </returns>
      public abstract bool CanHyperdrive{ get; }
		
      /// <summary> Returns whether the adapter can physically support flex speed mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do flex speed,
      /// <code>false</code> otherwise.
      /// </returns>
      public abstract bool CanFlex{ get; }
		
		
      /// <summary> Returns whether adapter can physically support 12 volt power mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do Program voltage,
      /// <code>false</code> otherwise.
      /// </returns>
      public abstract bool CanProgram{ get; }
		
      /// <summary> Returns whether the adapter can physically support strong 5 volt power
      /// mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do strong 5 volt
      /// mode, <code>false</code> otherwise.
      /// </returns>
      public abstract bool CanDeliverPower{ get; }
		
      /// <summary> Returns whether the adapter can physically support "smart" strong 5
      /// volt power mode.  "smart" power delivery is the ability to deliver
      /// power until it is no longer needed.  The current drop it detected
      /// and power delivery is stopped.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do "smart" strong
      /// 5 volt mode, <code>false</code> otherwise.
      /// </returns>
      public abstract bool CanDeliverSmartPower{ get; }
		
      /// <summary> Returns whether adapter can physically support 0 volt 'break' mode.
      /// </summary>
      /// <returns>  <code>true</code> if this port adapter can do break,
      /// <code>false</code> otherwise.
      /// </returns>
      public abstract bool CanBreak{ get; }
      #endregion

      #region Searching

      /// <summary>
      /// exclude family codes
      /// </summary>
      protected internal byte[] exclude = null;
      /// <summary>
      /// include family codes
      /// </summary>
      protected internal byte[] include = null;

      /// <summary> Returns <code>true</code> if the first iButton or 1-Wire device
      /// is found on the 1-Wire Network.
      /// If no devices are found, then <code>false</code> will be returned.
      /// </summary>
      /// <param name="address"> device address found
      /// </param>
      /// <param name="offset">offset into array where address begins
      /// </param>
      /// <returns>  <code>true</code> if an iButton or 1-Wire device is found.
      /// </returns>
      public abstract bool GetFirstDevice(byte[] address, int offset);

      /// <summary> Returns <code>true</code> if the next iButton or 1-Wire device
      /// is found. The previous 1-Wire device found is used
      /// as a starting point in the search.  If no more devices are found
      /// then <code>false</code> will be returned.
      /// </summary>
      /// <param name="address"> device address found
      /// </param>
      /// <param name="offset">offset into array where address begins
      /// </param>
      /// <returns>  <code>true</code> if an iButton or 1-Wire device is found.
      /// </returns>
      public abstract bool GetNextDevice(byte[] address, int offset);

      /// <summary> Verifies that the iButton or 1-Wire device specified is present on
      /// the 1-Wire Network. This does not affect the 'current' device
      /// state information used in searches (findNextDevice...).
      /// </summary>
      /// <param name="address"> device address to verify is present
      /// </param>
      /// <param name="offset">offset into array where address begins
      /// </param>
      /// <returns>  <code>true</code> if device is present else
      /// <code>false</code>.
      /// </returns>
      public abstract bool IsPresent(byte[] address, int offset);

      /// <summary> Verifies that the iButton or 1-Wire device specified is present
      /// on the 1-Wire Network and in an alarm state. This does not
      /// affect the 'current' device state information used in searches
      /// (findNextDevice...).
      /// </summary>
      /// <param name="address"> device address to verify is present and alarming
      /// </param>
      /// <param name="offset">offset into array where address begins
      /// </param>
      /// <returns>  <code>true</code> if device is present and alarming else
      /// <code>false</code>.
      /// </returns>
      public abstract bool IsAlarming(byte[] address, int offset);

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
      /// <param name="offset">offset into array where address begins
      /// </param>
      /// <returns>  <code>true</code> if device address was sent,<code>false</code>
      /// otherwise.
      /// </returns>
      public virtual bool SelectDevice(byte[] address, int offset)
      {
         // send 1-Wire Reset
         OWResetResult rslt = Reset();
			
         // broadcast the MATCH ROM command and address
         byte[] send_packet = new byte[9];
			
         send_packet[0] = (byte)(0x55); // MATCH ROM command
			
         Array.Copy(address, offset, send_packet, 1, 8);
         DataBlock(send_packet, 0, 9);
			
         // success if any device present on 1-Wire Network
         return ((rslt == OWResetResult.RESET_PRESENCE) 
            || (rslt == OWResetResult.RESET_ALARM));
      }

      /// <summary> Set the 1-Wire Network search to find only iButtons and 1-Wire
      /// devices that are in an 'Alarm' state that signals a need for
      /// attention.  Not all iButton types
      /// have this feature.  Some that do: DS1994, DS1920, DS2407.
      /// This selective searching can be canceled with the
      /// 'setSearchAllDevices()' method.
      /// </summary>
      public abstract void SetSearchOnlyAlarmingDevices();

      /// <summary> Set the 1-Wire Network search to not perform a 1-Wire
      /// reset before a search.  This feature is chiefly used with
      /// the DS2409 1-Wire coupler.
      /// The normal reset before each search can be restored with the
      /// 'setSearchAllDevices()' method.
      /// </summary>
      public abstract void SetNoResetSearch();

      /// <summary> Set the 1-Wire Network search to find all iButtons and 1-Wire
      /// devices whether they are in an 'Alarm' state or not and
      /// restores the default setting of providing a 1-Wire reset
      /// command before each search. (see setNoResetSearch() method).
      /// </summary>
      public abstract void SetSearchAllDevices();

      /// <summary> Removes any selectivity during a search for iButtons or 1-Wire devices
      /// by family type.  The unique address for each iButton and 1-Wire device
      /// contains a family descriptor that indicates the capabilities of the
      /// device.
      /// </summary>
      public virtual void  TargetAllFamilies()
      {
         include = null;
         exclude = null;
      }
		
      /// <summary> Takes an integer to selectively search for this desired family type.
      /// If this method is used, then no devices of other families will be
      /// found by any of the search methods.
      /// </summary>
      /// <param name="family">  the code of the family type to target for searches
      /// </param>
      public virtual void TargetFamily(int family)
      {
         if ((include == null) || (include.Length != 1))
            include = new byte[1];
			
         include[0] = (byte) family;
      }
		
      /// <summary> Takes an array of bytes to use for selectively searching for acceptable
      /// family codes.  If used, only devices with family codes in this array
      /// will be found by any of the search methods.
      /// </summary>
      /// <param name="family"> array of the family types to target for searches
      /// </param>
      public virtual void TargetFamily(byte[] family)
      {
         if ((include == null) || (include.Length != family.Length))
            include = new byte[family.Length];
			
         Array.Copy(family, 0, include, 0, family.Length);
      }
		
      /// <summary> Takes an integer family code to avoid when searching for iButtons.
      /// or 1-Wire devices.
      /// If this method is used, then no devices of this family will be
      /// found by any of the search methods.
      /// </summary>
      /// <param name="family">  the code of the family type NOT to target in searches
      /// </param>
      public virtual void ExcludeFamily(int family)
      {
         if ((exclude == null) || (exclude.Length != 1))
            exclude = new byte[1];
			
         exclude[0] = (byte) family;
      }
		
      /// <summary> Takes an array of bytes containing family codes to avoid when finding
      /// iButtons or 1-Wire devices.  If used, then no devices with family
      /// codes in this array will be found by any of the search methods.
      /// 
      /// </summary>
      /// <param name="family"> array of family cods NOT to target for searches
      /// </param>
      public virtual void ExcludeFamily(byte[] family)
      {
         if ((exclude == null) || (exclude.Length != family.Length))
            exclude = new byte[family.Length];
			
         Array.Copy(family, 0, exclude, 0, family.Length);
      }

      /// <summary> Checks to see if the family found is in the desired
      /// include group.
      /// 
      /// </summary>
      /// <returns>  <code>true</code> if in include group
      /// </returns>
      protected internal virtual bool isValidFamily(byte familyCode)
      {
         if (exclude != null)
         {
            for (int i = 0; i < exclude.Length; i++)
            {
               if (familyCode == exclude[i])
               {
                  return false;
               }
            }
         }
			
         if (include != null)
         {
            for (int i = 0; i < include.Length; i++)
            {
               if (familyCode == include[i])
               {
                  return true;
               }
            }
				
            return false;
         }
			
         return true;
      }
      #endregion

      #region adapter and port identity
      /// <summary>
      /// The name of this adapter type
      /// </summary>
      public abstract string AdapterName{ get; }
      /// <summary>
      /// The port name for this adapter type (i.e. COM1, LPT1, etc)
      /// </summary>
      public abstract string PortName{ get; }
      /// <summary>
      /// Detailed description for this port type
      /// </summary>
      public abstract string PortTypeDescription{ get; }
      /// <summary>
      /// Collection of valid port names for this port type
      /// </summary>
      public abstract System.Collections.IList PortNames { get; }
      #endregion

      #region Equals, HashCode, ToString
      /// <summary>
      /// Get Hash Code (proxies to .ToString().GetHashCode())
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode()
      {
         return ToString().GetHashCode();
      }
      /// <summary>
      /// Tells if objects are equal (proxies to .ToString().Equals())
      /// </summary>
      /// <param name="o"></param>
      /// <returns></returns>
      public override bool Equals(System.Object o)
      {
         if (o != null && o is PortAdapter)
         {
            if (o == this || o.ToString().Equals(this.ToString()))
            {
               return true;
            }
         }
         return false;
      }
      /// <summary>
      /// Returns a string representation of this adapter (i.e. "DS9097U COM1")
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         return AdapterName + " " + PortName;
      }
      #endregion
   }
}
