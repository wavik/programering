// TODO: AdapterDetected

/*---------------------------------------------------------------------------
 * Copyright (C) 2004 Dallas Semiconductor Corporation, All Rights Reserved.
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
 * IN NO EVENT SHALL DALLAS SEMICONDUCTOR BE LIABLE FOR ANY CLAIM, DAMAGES
 * OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * Except as contained in this notice, the name of Dallas Semiconductor
 * shall not be used except as stated in the Dallas Semiconductor
 * Branding Policy.
 *---------------------------------------------------------------------------
 */

using System;
using System.IO;
using System.IO.Ports;
using DalSemi;
using DalSemi.Utils;

namespace DalSemi.OneWire.Adapter
{
    internal class SerialAdapter : PortAdapter 
    {

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
        /*
        public override Boolean AdapterDetected
        {
            get
            {
                return true; // TODO
            }
        }
        */

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
        /*
        public boolean adapterDetected ()
           throws OneWireIOException, OneWireException
        {
           boolean rt;

           try
           {

              // acquire exclusive use of the port
              beginLocalExclusive();
              uAdapterPresent();

              rt = uVerify();
           }
           catch (OneWireException e)
           {
              rt = false;
           }
           finally
           {

              // release local exclusive use of port
              endLocalExclusive();
           }

           return rt;
        }
        */




        #region private fields
        //private bool doDebugMessages = false;

        private uint maxBaud = 115200;

        private SerialPort serial;

        private bool adapterPresent;

        /// <summary>Flag to indicate more than expected byte received in a transaction </summary>
        private bool extraBytesReceived;

        /// <summary>U Adapter packet builder</summary>
        private UPacketBuilder uBuild;

        /// <summary>State of the OneWire</summary>
        private OneWireState owState;

        /// <summary>U Adapter state</summary>
        private UAdapterState uState;

        /// <summary>Input buffer to hold received data</summary>
        private System.Collections.ArrayList inBuffer;
        #endregion

        #region Constructors and Destructors

        public SerialAdapter()
        {
            owState = new OneWireState();
            uState = new UAdapterState(owState);
            uBuild = new UPacketBuilder(uState);
            inBuffer = new System.Collections.ArrayList(10);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (serial != null)
                {
                    serial.Dispose();
                    serial = null;
                }
            }
        }

        public override System.Collections.IList PortNames
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }
        public override string AdapterName
        {
            get
            {
                return "DS9097U";
            }
        }
        public override string PortName
        {
            get
            {
                return serial.PortName;
            }
        }
        public override string PortTypeDescription
        {
            get
            {
                return serial.PortName + " (.NET)";
            }
        }

        public override bool OpenPort(String PortName)
        {
            if (serial == null)
            {
                bool isPresent = false;
                Debug.WriteLine("DEBUG: Opening Port: " + PortName);
                serial = new SerialPort(PortName, 9600, Parity.None, 8, StopBits.One);
                serial.ReadBufferSize = 4096;
                serial.WriteBufferSize = 16;
                serial.ReadTimeout = 500;
                serial.WriteTimeout = -1;
                serial.DiscardNull = false;
                try
                {
                    serial.Open();
                    serial.DtrEnable = true;
                    serial.RtsEnable = true;
                    sleep(20);
                    Debug.WriteLine("DEBUG: Open Succeeded");

                    isPresent = uAdapterPresent();
                    if(isPresent)
                        return true;
                    else
                        Debug.WriteLine("DEBUG: Adapter not present");
                }
                catch (Exception exc)
                {
                    Debug.WriteLine("DEBUG: Open failed: " + exc.ToString());
                    return false;
                }
                finally
                {
                    if (!isPresent && serial.IsOpen)
                        serial.Close();
                }
            }
            return false;
        }
        #endregion

        #region Data I/O
        public override OWResetResult Reset()
        {
            try
            {
                // acquire exclusive use of the port
                BeginExclusive(true);

                // make sure adapter is present
                if (uAdapterPresent())
                {
                    // check for pending power conditions
                    if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                        SetPowerNormal();

                    // flush out the com buffer
                    //serial.Flush();

                    // build a message to read the baud rate from the U brick
                    uBuild.Restart();

                    int reset_offset = uBuild.OneWireReset();

                    // send and receive
                    byte[] result_array = uTransaction(uBuild);

                    // check the result
                    if (result_array.Length == (reset_offset + 1))
               {
                  OWResetResult rslt = uBuild.InterpretOneWireReset(
                     result_array[reset_offset]);
                  onReset(rslt);
                  return rslt;
               }
                    else
                        throw new AdapterException("USerialAdapter-reset: no return byte from 1-Wire reset");
                }
                else
                    throw new AdapterException("Error communicating with adapter");
            }
            catch (System.IO.IOException ioe)
            {
                throw new AdapterException(ioe);
            }
            finally
            {
                // release local exclusive use of port
                EndExclusive();
            }
        }


        /// <summary> Sends a bit to the 1-Wire Network.
        /// 
        /// </summary>
        /// <param name="bitValue"> the bit value to send to the 1-Wire Network.
        /// </param>
        public override void PutBit(bool bitValue)
        {
            try
            {
                // acquire exclusive use of the port
                BeginExclusive(true);

                // make sure adapter is present
                if (uAdapterPresent())
                {
                    // check for pending power conditions
                    if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                        SetPowerNormal();

                    // flush out the com buffer
                    serial.DiscardInBuffer();

                    // build a message to send bit to the U brick
                    uBuild.Restart();

                    int bit_offset = uBuild.DataBit(bitValue, owState.levelChangeOnNextBit);

                    // check if just started power delivery
                    if (owState.levelChangeOnNextBit)
                    {
                        // clear the primed condition
                        owState.levelChangeOnNextBit = false;

                        // set new level state
                        owState.oneWireLevel = OWLevel.LEVEL_POWER_DELIVERY;
                    }

                    // send and receive
                    byte[] result_array = uTransaction(uBuild);

                    // check for echo
                    if (bitValue != uBuild.InterpretOneWireBit((byte)result_array[bit_offset]))
                        throw new AdapterException("1-Wire communication error, echo was incorrect");
                }
                else
                    throw new AdapterException("Error communicating with adapter");
            }
            catch (System.IO.IOException ioe)
            {
                throw new AdapterException(ioe);
            }
            finally
            {

                // release local exclusive use of port
                EndExclusive();
            }
        }

        /// <summary> Sends a byte to the 1-Wire Network.
        /// 
        /// </summary>
        /// <param name="byteValue"> the byte value to send to the 1-Wire Network.
        /// </param>
        public override void PutByte(int byteValue)
        {
            byte[] temp_block = new byte[1];

            temp_block[0] = (byte)byteValue;

            DataBlock(temp_block, 0, 1);

            // check to make sure echo was what was sent
            if (temp_block[0] != (byte)byteValue)
                throw new AdapterException("Error short on 1-Wire during putByte");
        }

        /// <summary> Gets a bit from the 1-Wire Network.
        /// 
        /// </summary>
        /// <returns>  the bit value recieved from the the 1-Wire Network.
        /// </returns>
        public override bool GetBit()
        {
            try
            {

                // acquire exclusive use of the port
                BeginExclusive(true);

                // make sure adapter is present
                if (uAdapterPresent())
                {

                    // check for pending power conditions
                    if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                        SetPowerNormal();

                    // flush out the com buffer
                    serial.DiscardInBuffer();

                    // build a message to send bit to the U brick
                    uBuild.Restart();

                    int bit_offset = uBuild.DataBit(true, owState.levelChangeOnNextBit);

                    // check if just started power delivery
                    if (owState.levelChangeOnNextBit)
                    {

                        // clear the primed condition
                        owState.levelChangeOnNextBit = false;

                        // set new level state
                        owState.oneWireLevel = OWLevel.LEVEL_POWER_DELIVERY;
                    }

                    // send and receive
                    byte[] result_array = uTransaction(uBuild);

                    // check the result
                    if (result_array.Length == (bit_offset + 1))
                        return uBuild.InterpretOneWireBit((byte)result_array[bit_offset]);
                    else
                        return false;
                }
                else
                    throw new AdapterException("Error communicating with adapter");
            }
            catch (System.IO.IOException ioe)
            {
                throw new AdapterException(ioe);
            }
            finally
            {
                // release local exclusive use of port
                EndExclusive();
            }
        }

        /// <summary> Gets a byte from the 1-Wire Network.
        /// 
        /// </summary>
        /// <returns>  the byte value received from the the 1-Wire Network.
        /// </returns>
        public override int GetByte()
        {
            byte[] temp_block = GetBlock(1);
         return (temp_block[0] & 0x0FF);
        }

        /// <summary> Get a block of data from the 1-Wire Network.
        /// 
        /// </summary>
        /// <param name="len"> length of data bytes to receive
        /// </param>
        /// <returns>  the data received from the 1-Wire Network.
        /// </returns>
        public override byte[] GetBlock(int len)
        {
            byte[] temp_block = new byte[len];

            GetBlock(temp_block, 0, len);

            return temp_block;
        }

        /// <summary> Get a block of data from the 1-Wire Network and write it into
        /// the provided array.
        /// 
        /// </summary>
        /// <param name="arr">    array in which to write the received bytes
        /// </param>
        /// <param name="len">    length of data bytes to receive
        /// </param>
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
        /// </param>
        public override void GetBlock(byte[] arr, int off, int len)
        {
            // set block to read 0xFF
            for (int i = off; i < len; i++)
                arr[i] = 0xFF;

            DataBlock(arr, off, len);
        }


        /// <summary> Sends a block of data and returns the data received in the same array.
        /// This method is used when sending a block that contains reads and writes.
        /// The 'read' portions of the data block need to be pre-loaded with 0xFF's.
        /// It starts sending data from the index at offset 'off' for length 'len'.
        /// 
        /// </summary>
        /// <param name="buffer"> array of data to transfer to and from the 1-Wire Network.
        /// </param>
        /// <param name="off">       offset into the array of data to start
        /// </param>
        /// <param name="len">       length of data to send / receive starting at 'off'
        /// </param>
        public override void DataBlock(byte[] buffer, int off, int len)
        {
            int data_offset;
            byte[] ret_data;

         try
         {
            
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                // make sure adapter is present
                if (uAdapterPresent())
                {

                    // check for pending power conditions
                    if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                        SetPowerNormal();

                    // set the correct baud rate to stream this operation
                    SetStreamingSpeed(UPacketBuilder.OPERATION_BYTE);

                    // flush out the com buffer
                    serial.DiscardInBuffer();

                    // build a message to write/read data bytes to the U brick
                    uBuild.Restart();

                    // check for primed byte
                    if ((len == 1) && owState.levelChangeOnNextByte)
                    {
                        data_offset = uBuild.PrimedDataByte(buffer[off]);
                        owState.levelChangeOnNextByte = false;

                        // send and receive
                        ret_data = uTransaction(uBuild);

                        // set new level state
                        owState.oneWireLevel = OWLevel.LEVEL_POWER_DELIVERY;

                        // extract the result byte
                        buffer[off] = uBuild.InterpretPrimedByte(ret_data, data_offset);
                    }
                    else
                    {
                        data_offset = uBuild.DataBytes(buffer, off, len);

                        // send and receive
                        ret_data = uTransaction(uBuild);

                        // extract the result byte(s)
                        uBuild.InterpretDataBytes(ret_data, data_offset, buffer, off, len);
                    }
                }
                else
                    throw new AdapterException("Error communicating with adapter");
            }
            catch (System.IO.IOException ioe)
            {
                throw new AdapterException(ioe);
            }
            finally
            {

                // release local exclusive use of port
                EndExclusive();
            }
        }
        #endregion

        #region Communication Speed
        /// <summary>OWSpeed representing current speed of communication on 1-Wire network
        /// </summary>
        public override OWSpeed Speed
        {
            set
            {
               try
               {
                  // acquire exclusive use of the port
                  BeginExclusive(true);
               
                  OWSpeed old = owState.oneWireSpeed;

                  // change 1-Wire speed
                  owState.oneWireSpeed = value;

                  // fire speed change event
                  if(old!=value)
                     onSpeedChange(old, value);
               
                  // set adapter to communicate at this new speed (regular == flex for now)
                  if (value == OWSpeed.SPEED_OVERDRIVE)
                      uState.uSpeedMode = UAdapterState.USPEED_OVERDRIVE;
                  else
                      uState.uSpeedMode = UAdapterState.USPEED_FLEX;
               }
               finally
               {
                  // release local exclusive use of port
                  EndExclusive();
               }
            }
            get
            {
               return owState.oneWireSpeed;
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
        public override void SetPowerDuration(OWPowerTime powerDur)
        {
            if (powerDur != OWPowerTime.DELIVERY_INFINITE)
                throw new AdapterException("USerialAdapter-setPowerDuration, does not support this duration, infinite only");
            else
                owState.levelTimeFactor = OWPowerTime.DELIVERY_INFINITE;
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
        /// @throws AdapterException on a 1-Wire communication error
        /// @throws AdapterException on a setup error with the 1-Wire adapter
        /// </returns>
        public override bool StartPowerDelivery(OWPowerStart changeCondition)
        {
         try
         {
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                if (changeCondition == OWPowerStart.CONDITION_AFTER_BIT)
                {
                    owState.levelChangeOnNextBit = true;
                    owState.primedLevelValue = OWLevel.LEVEL_POWER_DELIVERY;
                }
                else if (changeCondition == OWPowerStart.CONDITION_AFTER_BYTE)
                {
                    owState.levelChangeOnNextByte = true;
                    owState.primedLevelValue = OWLevel.LEVEL_POWER_DELIVERY;
                }
                else if (changeCondition == OWPowerStart.CONDITION_NOW)
                {

                    // make sure adapter is present
                    if (uAdapterPresent())
                    {

                        // check for pending power conditions
                        if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                            SetPowerNormal();

                        // flush out the com buffer
                        serial.DiscardInBuffer();

                        // build a message to read the baud rate from the U brick
                        uBuild.Restart();

                        // set the SPUD time value
                        int set_SPUD_offset = uBuild.SetParameter(
                           ProgramPulseTime5.TIME5V_infinite);

                        // add the command to begin the pulse
                        uBuild.SendCommand(UPacketBuilder.FUNCTION_5VPULSE_NOW, false);

                        // send and receive
                        byte[] result_array = uTransaction(uBuild);

                        // check the result
                        if (result_array.Length == (set_SPUD_offset + 1))
                        {
                            owState.oneWireLevel = OWLevel.LEVEL_POWER_DELIVERY;

                            return true;
                        }
                    }
                    else
                        throw new AdapterException("Error communicating with adapter");
                }
                else
                    throw new AdapterException("Invalid power delivery condition");

                return false;
            }
            catch (System.IO.IOException ioe)
            {
                throw new AdapterException(ioe);
            }
            finally
            {
                // release local exclusive use of port
                EndExclusive();
            }
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
        public override void SetProgramPulseDuration(OWPowerTime pulseDur)
        {
            if (pulseDur != OWPowerTime.DELIVERY_EPROM)
                throw new AdapterException("Only support EPROM length program pulse duration");
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
        /// 
        /// </param>
        /// <returns> <code>true</code> if the voltage change was successful,
        /// <code>false</code> otherwise.
        /// 
        /// @throws AdapterException on a 1-Wire communication error
        /// @throws AdapterException on a setup error with the 1-Wire adapter
        /// or the adapter does not support this operation
        /// </returns>
        public override bool StartProgramPulse(OWPowerStart changeCondition)
        {
            // check if adapter supports program
            if (!uState.programVoltageAvailable)
                throw new AdapterException(
                   "SerialAdapter: startProgramPulse, program voltage not available");

            // check if correct change condition
            if (changeCondition != OWPowerStart.CONDITION_NOW)
                throw new AdapterException(
                   "SerialAdapter: startProgramPulse, CONDITION_NOW only currently supported");

         try
         {
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                // build a message to read the baud rate from the U brick
                uBuild.Restart();

                //int set_SPUD_offset =
                uBuild.SetParameter(ProgramPulseTime12.TIME12V_512us);

                // add the command to begin the pulse
                //int pulse_offset =
                uBuild.SendCommand(UPacketBuilder.FUNCTION_12VPULSE_NOW, true);

                // send the command
                //char[] result_array =
                uTransaction(uBuild);

                // check the result ??
                return true;
            }
            finally
            {

                // release local exclusive use of port
                EndExclusive();
            }
        }

        /// <summary> Sets the 1-Wire Network voltage to 0 volts.  This method is used
        /// rob all 1-Wire Network devices of parasite power delivery to force
        /// them into a hard reset.
        /// </summary>
        public override void StartBreak()
        {
         try
         {
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                // power down the 2480 (dropping the 1-Wire)
                serial.DtrEnable = false;
                serial.RtsEnable = false;
                serial.BreakState = true;

                // wait for power to drop
                sleep(200);

                // set the level state
                owState.oneWireLevel = OWLevel.LEVEL_BREAK;
            }
            finally
            {
                // release local exclusive use of port
                EndExclusive();
            }
        }

        /// <summary> Sets the 1-Wire Network voltage to normal level.  This method is used
        /// to disable 1-Wire conditions created by startPowerDelivery and
        /// startProgramPulse.  This method will automatically be called if
        /// a communication method is called while an outstanding power
        /// command is taking place.
        /// 
        /// @throws AdapterException on a 1-Wire communication error
        /// @throws AdapterException on a setup error with the 1-Wire adapter
        /// or the adapter does not support this operation
        /// </summary>
        public override void SetPowerNormal()
        {
         try
         {
            
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                if (owState.oneWireLevel == OWLevel.LEVEL_POWER_DELIVERY)
                {

                    // make sure adapter is present
                    if (uAdapterPresent())
                    {

                        // flush out the com buffer
                        //serial.Flush();

                        // build a message to read the baud rate from the U brick
                        uBuild.Restart();

                        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//
                        // shughes - 8-28-2003
                        // Fixed the Set Power Level Normal problem where adapter
                        // is left in a bad state.  Removed bad fix: extra getBit()
                        // SEE BELOW!
                        // stop pulse command
                        uBuild.SendCommand(UPacketBuilder.FUNCTION_STOP_PULSE, true);

                        // start pulse with no prime
                        uBuild.SendCommand(UPacketBuilder.FUNCTION_5VPULSE_NOW, false);
                        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//

                        // add the command to stop the pulse
                        int pulse_response_offset = uBuild.SendCommand(
                           UPacketBuilder.FUNCTION_STOP_PULSE, true);

                        // send and receive
                        byte[] result_array = uTransaction(uBuild);

                        // check the result
                        if (result_array.Length == (pulse_response_offset + 1))
                        {
                            owState.oneWireLevel = OWLevel.LEVEL_NORMAL;

                            //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//
                            // shughes - 8-28-2003
                            // This is a bad "fix", it was needed when we were causing
                            // a bad condition.  Instead of fixing it here, we should
                            // fix it where we were causing it..  Which we did!
                            // SEE ABOVE!
                            //getBit();
                            //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//\\//
                        }
                        else
                            throw new AdapterException(
                               "Did not get a response back from stop power delivery");
                    }
                }
                else if (owState.oneWireLevel == OWLevel.LEVEL_BREAK)
                {

                    // restore power
                    serial.DtrEnable = true;
                    serial.RtsEnable = true;
                    serial.BreakState = false;

                    // wait for power to come up
                    sleep(300);

                    // set the level state
                    owState.oneWireLevel = OWLevel.LEVEL_NORMAL;

                    // set the DS2480 to the correct mode and verify
                    adapterPresent = false;

                    if (!uAdapterPresent())
                        throw new AdapterException(
                           "Did not get a response back from adapter after break");
                }
            }
            catch (System.IO.IOException ioe)
            {
                throw new AdapterException(ioe);
            }
            finally
            {

                // release local exclusive use of port
                EndExclusive();
            }
        }
        #endregion

        #region Adapter Features
        /// <summary> Returns whether adapter can physically support overdrive mode.
        /// 
        /// </summary>
        /// <returns>  <code>true</code> if this port adapter can do OverDrive,
        /// <code>false</code> otherwise.
        /// 
        /// @throws AdapterException on a 1-Wire communication error with the adapter
        /// @throws AdapterException on a setup error with the 1-Wire
        /// adapter
        /// </returns>
        public override bool CanOverdrive
        {
            get
            {
                return true;
            }
        }

        /// <summary> Returns whether the adapter can physically support hyperdrive mode.
        /// 
        /// </summary>
        /// <returns>  <code>true</code> if this port adapter can do HyperDrive,
        /// <code>false</code> otherwise.
        /// 
        /// @throws AdapterException on a 1-Wire communication error with the adapter
        /// @throws AdapterException on a setup error with the 1-Wire
        /// adapter
        /// </returns>
        public override bool CanHyperdrive
        {
            get
            {
                return false;
            }
        }

        /// <summary> Returns whether the adapter can physically support flex speed mode.
        /// 
        /// </summary>
        /// <returns>  <code>true</code> if this port adapter can do flex speed,
        /// <code>false</code> otherwise.
        /// 
        /// @throws AdapterException on a 1-Wire communication error with the adapter
        /// @throws AdapterException on a setup error with the 1-Wire
        /// adapter
        /// </returns>
        public override bool CanFlex
        {
            get
            {
                return true;
            }
        }

        /// <summary> Returns whether adapter can physically support 12 volt power mode.
        /// 
        /// </summary>
        /// <returns>  <code>true</code> if this port adapter can do Program voltage,
        /// <code>false</code> otherwise.
        /// 
        /// @throws AdapterException on a 1-Wire communication error with the adapter
        /// @throws AdapterException on a setup error with the 1-Wire
        /// adapter
        /// </returns>
        public override bool CanProgram
        {
            get
            {
            try
            {
               
                // acquire exclusive use of the port
                BeginExclusive(true);
               
                    // only check if the port is aquired
                    if (uAdapterPresent())
                    {

                        // perform a reset to read the program available flag
                        if (uState.revision == 0)
                            Reset();

                        // return the flag
                        return uState.programVoltageAvailable;
                    }
                    else
                        throw new AdapterException("SerialAdapter-canProgram, adapter not present");
                }
                finally
                {

                    // release local exclusive use of port
                    EndExclusive();
                }
            }
        }

        /// <summary> Returns whether the adapter can physically support strong 5 volt power
        /// mode.
        /// 
        /// </summary>
        /// <returns>  <code>true</code> if this port adapter can do strong 5 volt
        /// mode, <code>false</code> otherwise.
        /// 
        /// @throws AdapterException on a 1-Wire communication error with the adapter
        /// @throws AdapterException on a setup error with the 1-Wire
        /// adapter
        /// </returns>
        public override bool CanDeliverPower
        {
            get
            {
                return true;
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
                return false;
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
                return true;
            }
        }
        #endregion

        #region Searching
        /// <summary> Returns <code>true</code> if the first iButton or 1-Wire device
        /// is found on the 1-Wire Network.
        /// If no devices are found, then <code>false</code> will be returned.
        /// </summary>
        /// <returns>  <code>true</code> if an iButton or 1-Wire device is found.
        /// </returns>
        public override bool GetFirstDevice(byte[] address, int offset)
        {
         // reset the current search
            owState.searchLastDiscrepancy = 0;
            owState.searchFamilyLastDiscrepancy = 0;
            owState.searchLastDevice = false;

         // search for the first device using next
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
            bool search_result;

         try
         {
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                // check for previous last device
                if (owState.searchLastDevice)
                {
                    owState.searchLastDiscrepancy = 0;
                    owState.searchFamilyLastDiscrepancy = 0;
                    owState.searchLastDevice = false;

                    return false;
                }

                // check for 'first' and only 1 target
                if ((owState.searchLastDiscrepancy == 0)
                   && (owState.searchLastDevice == false)
                   && (owState.searchIncludeFamilies.Length == 1))
                {

               // set the search to find the 1 target first
                    owState.searchLastDiscrepancy = 64;

                    // create an id to set
                    byte[] new_id = new byte[8];

                    // set the family code
                    new_id[0] = (byte)owState.searchIncludeFamilies[0];

                    // clear the rest
                    for (int i = 1; i < 8; i++)
                        new_id[i] = 0;

                    // set this new ID
                    Array.Copy(new_id, 0, owState.ID, 0, 8);
                }

                // loop until the correct type is found or no more devices
                do
                {

                    // perform a search and keep the result
                    search_result = Search(owState);

                    if (search_result)
                    {
                        for (int i = 0; i < 8; i++)
                            address[i + offset] = (byte)owState.ID[i];

                        // check if not in exclude list
                        bool is_excluded = false;

                        for (int i = 0; i < owState.searchExcludeFamilies.Length; i++)
                        {
                            if (owState.ID[0] == owState.searchExcludeFamilies[i])
                            {
                                is_excluded = true;

                                break;
                            }
                        }

                        // if not in exclude list then check for include list
                        if (!is_excluded)
                        {

                            // loop through the include list
                            bool is_included = false;

                            for (int i = 0; i < owState.searchIncludeFamilies.Length; i++)
                            {
                                if (owState.ID[0] == owState.searchIncludeFamilies[i])
                                {
                                    is_included = true;

                                    break;
                                }
                            }

                            // check if include list or there is no include list
                            if (is_included || (owState.searchIncludeFamilies.Length == 0))
                                return true;
                        }
                    }

                    // skip the current type if not last device
                    if (!owState.searchLastDevice && (owState.searchFamilyLastDiscrepancy != 0))
                    {
                        owState.searchLastDiscrepancy = owState.searchFamilyLastDiscrepancy;
                        owState.searchFamilyLastDiscrepancy = 0;
                        owState.searchLastDevice = false;
                    }
                  // end of search so reset and return
                    else
                    {
                        owState.searchLastDiscrepancy = 0;
                        owState.searchFamilyLastDiscrepancy = 0;
                        owState.searchLastDevice = false;
                        search_result = false;
                    }
                }
                while (search_result);

                // device not found
                return false;
            }
            finally
            {

                // release local exclusive use of port
                EndExclusive();
            }
        }
        /// <summary> Verifies that the iButton or 1-Wire device specified is present on
        /// the 1-Wire Network. This does not affect the 'current' device
        /// state information used in searches (findNextDevice...).
        /// 
        /// </summary>
        /// <param name="address"> device address to verify is present
        /// 
        /// </param>
        /// <returns>  <code>true</code> if device is present else
        /// <code>false</code>.
        /// 
        /// @throws AdapterException on a 1-Wire communication error
        /// @throws AdapterException on a setup error with the 1-Wire adapter
        /// 
        /// </returns>
        /// <seealso cref="Address">
        /// </seealso>
        public override bool IsPresent(byte[] address, int offset)
        {
         try
         {
            
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                // make sure adapter is present
                if (uAdapterPresent())
                {

                    // check for pending power conditions
                    if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                        SetPowerNormal();

                    // if in overdrive, then use the block method in super
                    if (owState.oneWireSpeed == OWSpeed.SPEED_OVERDRIVE)
                        return BlockIsPresent(address, offset, false);

                    // create a private OneWireState
                    OneWireState onewire_state = new OneWireState();

                    // set the ID to the ID of the iButton passes to this method
                    Array.Copy(address, offset, onewire_state.ID, 0, 8);

                    // set the state to find the specified device
                    onewire_state.searchLastDiscrepancy = 64;
                    onewire_state.searchFamilyLastDiscrepancy = 0;
                    onewire_state.searchLastDevice = false;
                    onewire_state.searchOnlyAlarmingButtons = false;

                    // perform a search
                    if (Search(onewire_state))
                    {

                        // compare the found device with the desired device
                        for (int i = 0; i < 8; i++)
                            if (address[i + offset] != onewire_state.ID[i])
                                return false;

                        // must be the correct device
                        return true;
                    }

                    // failed to find device
                    return false;
                }
                else
                    throw new AdapterException("Error communicating with adapter");
            }
            finally
            {

                // release local exclusive use of port
                EndExclusive();
            }
        }

        /// <summary> Verifies that the iButton or 1-Wire device specified is present
        /// on the 1-Wire Network and in an alarm state. This does not
        /// affect the 'current' device state information used in searches
        /// (findNextDevice...).
        /// 
        /// </summary>
        /// <param name="address"> device address to verify is present and alarming
        /// 
        /// </param>
        /// <returns>  <code>true</code> if device is present and alarming else
        /// <code>false</code>.
        /// 
        /// @throws AdapterException on a 1-Wire communication error
        /// @throws AdapterException on a setup error with the 1-Wire adapter
        /// 
        /// </returns>
        /// <seealso cref="Address">
        /// </seealso>
        public override bool IsAlarming(byte[] address, int offset)
        {
         try
         {
            
            // acquire exclusive use of the port
            BeginExclusive(true);
            
                // make sure adapter is present
                if (uAdapterPresent())
                {

                    // check for pending power conditions
                    if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                        SetPowerNormal();

                    // if in overdrive, then use the block method in super
                    if (owState.oneWireSpeed == OWSpeed.SPEED_OVERDRIVE)
                        return BlockIsPresent(address, offset, true);

                    // create a private OneWireState
                    OneWireState onewire_state = new OneWireState();

                    // set the ID to the ID of the iButton passes to this method
                    Array.Copy(address, offset, onewire_state.ID, 0, 8);

                    // set the state to find the specified device (alarming)
                    onewire_state.searchLastDiscrepancy = 64;
                    onewire_state.searchFamilyLastDiscrepancy = 0;
                    onewire_state.searchLastDevice = false;
                    onewire_state.searchOnlyAlarmingButtons = true;

                    // perform a search
                    if (Search(onewire_state))
                    {

                        // compare the found device with the desired device
                        for (int i = 0; i < 8; i++)
                            if (address[i + offset] != onewire_state.ID[i])
                                return false;

                        // must be the correct device
                        return true;
                    }

                    // failed to find any alarming device
                    return false;
                }
                else
                    throw new AdapterException("Error communicating with adapter");
            }
            finally
            {

                // release local exclusive use of port
                EndExclusive();
            }
        }

      /// <summary> Set the 1-Wire Network search to find only iButtons and 1-Wire
        /// devices that are in an 'Alarm' state that signals a need for
        /// attention.  Not all iButton types
        /// have this feature.  Some that do: DS1994, DS1920, DS2407.
        /// This selective searching can be canceled with the
        /// 'setSearchAllDevices()' method.
        /// </summary>
        public override void SetSearchOnlyAlarmingDevices()
        {
            owState.searchOnlyAlarmingButtons = true;
        }

      /// <summary> Set the 1-Wire Network search to not perform a 1-Wire
      /// reset before a search.  This feature is chiefly used with
        /// the DS2409 1-Wire coupler.
      /// The normal reset before each search can be restored with the
        /// 'setSearchAllDevices()' method.
        /// </summary>
        public override void SetNoResetSearch()
        {
            owState.skipResetOnSearch = true;
        }

      /// <summary> Set the 1-Wire Network search to find all iButtons and 1-Wire
        /// devices whether they are in an 'Alarm' state or not and
        /// restores the default setting of providing a 1-Wire reset
      /// command before each search. (see setNoResetSearch() method).
        /// </summary>
        public override void SetSearchAllDevices()
        {
            owState.searchOnlyAlarmingButtons = false;
            owState.skipResetOnSearch = false;
        }

      /// <summary> Removes any selectivity during a search for iButtons or 1-Wire devices
        /// by family type.  The unique address for each iButton and 1-Wire device
        /// contains a family descriptor that indicates the capabilities of the
        /// device.
        /// </summary>
        public override void TargetAllFamilies()
        {

         // clear the include and exclude family search lists
            owState.searchIncludeFamilies = new byte[0];
            owState.searchExcludeFamilies = new byte[0];
        }

      /// <summary> Takes an integer to selectively search for this desired family type.
        /// If this method is used, then no devices of other families will be
        /// found by getFirstButton() & getNextButton().
        /// </summary>
        /// <param name="family">  the code of the family type to target for searches
        /// </param>
        public override void TargetFamily(int familyID)
        {
            // replace include family array with 1 element array
            owState.searchIncludeFamilies = new byte[1];
            owState.searchIncludeFamilies[0] = (byte)familyID;
        }

        /// <summary> Takes an array of bytes to use for selectively searching for acceptable
        /// family codes.  If used, only devices with family codes in this array
      /// will be found by any of the search methods.
        /// </summary>
        /// <param name="family"> array of the family types to target for searches
        /// </param>
        public override void TargetFamily(byte[] familyID)
        {
            // replace include family array with new array
            owState.searchIncludeFamilies = new byte[familyID.Length];

            Array.Copy(familyID, 0, owState.searchIncludeFamilies, 0, familyID.Length);
        }

        /// <summary> Takes an integer family code to avoid when searching for iButtons.
        /// or 1-Wire devices.
        /// If this method is used, then no devices of this family will be
      /// found by any of the search methods.
        /// </summary>
        /// <param name="family">  the code of the family type NOT to target in searches
        /// </param>
        public override void ExcludeFamily(int familyID)
        {
            // replace exclude family array with 1 element array
            owState.searchExcludeFamilies = new byte[1];
            owState.searchExcludeFamilies[0] = (byte)familyID;
        }

        /// <summary> Takes an array of bytes containing family codes to avoid when finding
        /// iButtons or 1-Wire devices.  If used, then no devices with family
      /// codes in this array will be found by any of the search methods.
        /// </summary>
        /// <param name="family"> array of family cods NOT to target for searches
        /// </param>
        public override void ExcludeFamily(byte[] familyID)
        {
            // replace exclude family array with new array
            owState.searchExcludeFamilies = new byte[familyID.Length];

            Array.Copy(familyID, 0, owState.searchExcludeFamilies, 0, familyID.Length);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>Normal Search, all devices participate </summary>
        private const byte NORMAL_SEARCH_CMD = (byte)(0xF0);

        /// <summary>Conditional Search, only 'alarming' devices participate </summary>
        private const byte ALARM_SEARCH_CMD = (byte)(0xEC);

        /// <summary> Perform a 'strongAccess' with the provided 1-Wire address.
      /// 1-Wire Network has already been reset and the 'search'
        /// command sent before this is called.
        /// 
        /// </summary>
        /// <param name="address"> device address to do strongAccess on
        /// </param>
        /// <param name="alarmOnly"> verify device is present and alarming if true
        /// 
        /// </param>
        /// <returns>  true if device participated and was present
      /// in the strongAccess search
        /// </returns>
        private bool BlockIsPresent(byte[] address, int offset, bool alarmOnly)
        {
            byte[] send_packet = new byte[25];
            int i;

            // reset the 1-Wire
            Reset();

         // send search command
            if (alarmOnly)
                send_packet[0] = ALARM_SEARCH_CMD;
            //PutByte(ALARM_SEARCH_CMD);
            else
                send_packet[0] = NORMAL_SEARCH_CMD;
            //PutByte(NORMAL_SEARCH_CMD);

            // set all bits at first
            for (i = 1; i < 25; i++)
                send_packet[i] = (byte)0xFF;

         // now set or clear apropriate bits for search
            // TODO
            for (i = 0; i < 64; i++)
                ArrayWriteBit(
                   ArrayReadBit(i, address, offset), (i + 1) * 3 - 1, send_packet, 1);

            // send to 1-Wire Net
            DataBlock(send_packet, 0, 25);

            // check the results of last 8 triplets (should be no conflicts)
            int cnt = 56, goodbits = 0, tst, s;

            for (i = 168; i < 192; i += 3)
            {
                tst = (ArrayReadBit(i, send_packet, 1) << 1)
                   | ArrayReadBit(i + 1, send_packet, 1);
                s = ArrayReadBit(cnt++, address, offset);

                if (tst == 0x03)
                // no device on line
                {
                    goodbits = 0; // number of good bits set to zero

                    break; // quit
                }

                if (((s == 0x01) && (tst == 0x02)) || ((s == 0x00) && (tst == 0x01)))
                    // correct bit
                    goodbits++; // count as a good bit
            }

            // check too see if there were enough good bits to be successful
            return (goodbits >= 8);
        }

        /// <summary> Writes the bit state in a byte array.
        /// </summary>
        /// <param name="state">new state of the bit 1, 0
        /// </param>
        /// <param name="index">bit index into byte array
        /// </param>
        /// <param name="buf">byte array to manipulate
        /// </param>
        /// <param name="offset">offset into byte array to read from
        /// </param>
        private void ArrayWriteBit(int state, int index,
           byte[] buf, int offset)
        {
            int nbyt = index >> 3;
            int nbit = index - (nbyt << 3);

            if (state == 1)
                buf[nbyt] |= (byte)(0x01 << nbit);
            else
                buf[nbyt] &= (byte)~(0x01 << nbit);
        }

        /// <summary> Reads a bit state in a byte array.
        /// </summary>
        /// <param name="index">bit index into byte array
        /// </param>
        /// <param name="buf">byte array to read from
        /// </param>
        /// <param name="offset">offset into byte array to read from
        /// </param>
        /// <returns> bit state 1 or 0
        /// </returns>
        private int ArrayReadBit(int index,
           byte[] buf, int offset)
        {
            int nbyt = index >> 3;
            int nbit = index - (nbyt << 3);

            return ((buf[nbyt] >> nbit) & 0x01);
        }

      /// <summary>Peform a search using the oneWire state provided
        /// </summary>
      /// <param name="mState"> current OneWire state used to do the search
        /// </param>
        private bool Search(OneWireState mState)
        {
            int reset_offset = 0;

            // make sure adapter is present
            if (uAdapterPresent())
            {

                // check for pending power conditions
                if (owState.oneWireLevel != OWLevel.LEVEL_NORMAL)
                    SetPowerNormal();

                // set the correct baud rate to stream this operation
                SetStreamingSpeed(UPacketBuilder.OPERATION_SEARCH);

                // reset the packet
                uBuild.Restart();

            // add a reset/ search command
                if (!mState.skipResetOnSearch)
                    reset_offset = uBuild.OneWireReset();

                if (mState.searchOnlyAlarmingButtons)
                    uBuild.DataByte((byte)ALARM_SEARCH_CMD);
                else
                    uBuild.DataByte((byte)NORMAL_SEARCH_CMD);

            // add search sequence based on mState
                int search_offset = uBuild.Search(mState);

            // send/receive the search
                byte[] result_array = uTransaction(uBuild);

            // interpret search result and return
                if (!mState.skipResetOnSearch)
                {
               OWResetResult rslt = uBuild.InterpretOneWireReset(
                  result_array[reset_offset]);
               onReset(rslt);
               if(OWResetResult.RESET_NOPRESENCE == rslt)
                    {
                        return false;
                    }
                }

                return uBuild.InterpretSearch(mState, result_array, search_offset);
            }
            else
                throw new AdapterException("Error communicating with adapter");
        }


        /// <summary> set the correct baud rate to stream this operation</summary>
        private void SetStreamingSpeed(int desiredOperation)
        {
            // get the desired baud rate for this operation
            uint baud = UPacketBuilder.GetDesiredBaud(
               desiredOperation, owState.oneWireSpeed, maxBaud);

            // check if already at the correct speed
            if (baud == serial.BaudRate)
                return;

            //         if (doDebugMessages)
            //            System.Console.Out.WriteLine("Changing baud rate from " + serial.BaudRate + " to " + baud);

            // convert this baud to 'u' baud
            AdapterBaud ubaud;

            switch (baud)
            {
                case 115200:
                    ubaud = AdapterBaud.BAUD_115200;
                    break;

                case 57600:
                    ubaud = AdapterBaud.BAUD_57600;
                    break;

                case 19200:
                    ubaud = AdapterBaud.BAUD_19200;
                    break;

                case 9600:
                default:
                    ubaud = AdapterBaud.BAUD_9600;
                    break;
            }

            // see if this is a new baud
            if (ubaud == uState.ubaud)
                return;

            // default, loose communication with adapter
            adapterPresent = false;

            // build a message to read the baud rate from the U brick
            uBuild.Restart();

            int baud_offset = uBuild.SetParameter(ubaud);

            try
            {
                // send command, no response at this baud rate
                System.Collections.IEnumerator pkts = uBuild.Packets;
                pkts.MoveNext();
                RawSendPacket pkt = (RawSendPacket)pkts.Current;
                byte[] temp_buf = new byte[pkt.dataList.Count];
                pkt.dataList.CopyTo(temp_buf);

                serial.DiscardInBuffer();
                serial.Write(temp_buf, 0, pkt.dataList.Count);

                // delay to let things settle
                do
                {
                    sleep(5);
                }
                while (serial.BytesToWrite > 0);
                sleep(5);
                serial.DiscardInBuffer();
                

                // set the baud rate
                sleep(5); 
                serial.BaudRate = (int)baud;
            }
            catch (System.IO.IOException ioe)
            {
                throw new AdapterException(ioe);
            }

            uState.ubaud = ubaud;

            // delay to let things settle
            sleep(5);

            // verify adapter is at new baud rate
            uBuild.Restart();

            baud_offset = uBuild.GetParameter(Parameter.PARAMETER_BAUDRATE);

            // set the DS2480 communication speed for subsequent blocks
            uBuild.SetSpeed();

            try
            {

                // send and receive
                //serial.Flush();

                byte[] result_array = uTransaction(uBuild);

                // check the result
                if (result_array.Length == 1)
                {
                    if (((result_array[baud_offset] & 0xF1) == 0) && ((result_array[baud_offset] & 0x0E) == (byte)uState.ubaud))
                    {
                        //                  if (doDebugMessages)
                        //                     System.Console.Out.WriteLine("Success, baud changed and DS2480 is there");

                        // adapter still with us
                        adapterPresent = true;

                        // flush any garbage characters
                        sleep(150);
                        serial.DiscardInBuffer();

                        return;
                    }
                }
            }
         catch (System.IO.IOException ioe)
            {
            Debug.WriteLine("SerialAdapter-setStreamingSpeed: " + ioe);
            }
         catch (AdapterException ae)
            {
            Debug.WriteLine("SerialAdapter-setStreamingSpeed: " + ae);
            }

            //         if (doDebugMessages)
            //            System.Console.Out.WriteLine("Failed to change baud of DS2480");
        }


        /// <summary> Verify that the DS2480 based adapter is present on the open port.
        /// 
        /// </summary>
        /// <returns> 'true' if adapter present
        /// 
        /// @throws AdapterException - if port not selected
        /// </returns>
        private bool uAdapterPresent()
        {
            bool rt = true;

            // check if adapter has already be verified to be present
            if (!adapterPresent)
            {

                // do a master reset
                uMasterReset();

                // attempt to verify
                if (!uVerify())
                {

                    // do a master reset and try again
                    uMasterReset();

                    if (!uVerify())
                    {

                        // do a power reset and try again
                        uPowerReset();

                        if (!uVerify())
                            rt = false;
                    }
                }
            }

            adapterPresent = rt;

            //         if (doDebugMessages)
            //            System.Console.Error.WriteLine("DEBUG: AdapterPresent result: " + rt);

            return rt;
        }

        /// <summary> Do a master reset on the DS2480.  This reduces the baud rate to
        /// 9600 and peforms a break.  A single timing byte is then sent.
        /// </summary>
        private void uMasterReset()
        {
            //         if (doDebugMessages)
            //            System.Console.Error.WriteLine("DEBUG: uMasterReset");

            // try to aquire the port
            try
            {

                // set the baud rate
                serial.BaudRate = 9600;

                uState.ubaud = AdapterBaud.BAUD_9600;

                // put back to standard speed
                owState.oneWireSpeed = OWSpeed.SPEED_REGULAR;
                uState.uSpeedMode = UAdapterState.USPEED_FLEX;
                uState.ubaud = AdapterBaud.BAUD_9600;

                // send a break to reset DS2480
                serial.BreakState = true;
                sleep(20);
                serial.BreakState = false;
                sleep(5);

                // send the timing byte
                serial.Write(new byte[] { UPacketBuilder.FUNCTION_RESET, UPacketBuilder.FUNCTION_RESET }, 0, 2);
                do
                {
                   sleep(5);
                }
                while (serial.BytesToWrite > 0);
                sleep(20);
                serial.DiscardInBuffer();
            }
         catch (System.IO.IOException e)
            {
            Debug.WriteLine("USerialAdapter-uMasterReset: " + e);
            }
        }

        /// <summary>  Do a power reset on the DS2480.  This reduces the baud rate to
        /// 9600 and powers down the DS2480.  A single timing byte is then sent.
        /// </summary>
        private void uPowerReset()
        {
            //         if (doDebugMessages)
            //            System.Console.Error.WriteLine("DEBUG: uPowerReset");

            // try to aquire the port
            try
            {

                // set the baud rate
                serial.BaudRate = 9600;

                uState.ubaud = AdapterBaud.BAUD_9600;

                // put back to standard speed
                owState.oneWireSpeed = OWSpeed.SPEED_REGULAR;
                uState.uSpeedMode = UAdapterState.USPEED_FLEX;
                uState.ubaud = AdapterBaud.BAUD_9600;

                // power down DS2480
                serial.DtrEnable = false;
                serial.RtsEnable = false;
                sleep(300);
                serial.DtrEnable = true;
                serial.RtsEnable = true;
                sleep(1);

                // send the timing byte
                serial.DiscardInBuffer();
                serial.Write(new byte[] { UPacketBuilder.FUNCTION_RESET, UPacketBuilder.FUNCTION_RESET }, 0, 2);
                do
                {
                    sleep(5);
                }
                while(serial.BytesToWrite>0);
                sleep(20);
                serial.DiscardInBuffer();
            }
         catch (System.IO.IOException e)
            {
            Debug.WriteLine("USerialAdapter-uPowerReset: " + e);

            }
        }

        /// <summary>  Read and verify the baud rate with the DS2480 chip and perform a
        /// single bit MicroLAN operation.  This is used as a DS2480 detect.
        /// 
        /// </summary>
        /// <returns>  'true' if the correct baud rate and bit operation
        /// was read from the DS2480
        /// 
        /// @throws AdapterException on a 1-Wire communication error
        /// </returns>
        private bool uVerify()
        {
            try
            {
                //serial.Flush();

                // build a message to read the baud rate from the U brick
                uBuild.Restart();

                uBuild.SetParameter(
                   uState.uParameters[(int)owState.oneWireSpeed].pullDownSlewRate);
                uBuild.SetParameter(
                   uState.uParameters[(int)owState.oneWireSpeed].write1LowTime);
                uBuild.SetParameter(
                   uState.uParameters[(int)owState.oneWireSpeed].sampleOffsetTime);
                uBuild.SetParameter(
                   ProgramPulseTime5.TIME5V_infinite);
                int baud_offset = uBuild.GetParameter(Parameter.PARAMETER_BAUDRATE);
                int bit_offset = uBuild.DataBit(true, false);

                // send and receive
                byte[] result_array = uTransaction(uBuild);

                // check the result
                if (result_array.Length == (bit_offset + 1))
                {
                    if (((result_array[baud_offset] & 0xF1) == 0) && ((result_array[baud_offset] & 0x0E) == (byte)uState.ubaud) && ((result_array[bit_offset] & 0xF0) == 0x90) && ((result_array[bit_offset] & 0x0C) == uState.uSpeedMode))
                        return true;
                }
            }
         catch (System.IO.IOException ioe)
            {
            Debug.WriteLine("USerialAdapter-uVerify: " + ioe);
            }
         catch (AdapterException e)
            {
            Debug.WriteLine("USerialAdapter-uVerify: " + e);
            }

            return false;
        }

        /// <summary> Write the raw U packet and then read the result.
        /// 
        /// </summary>
        /// <param name="tempBuild"> the U Packet Build where the packet to send
        /// resides
        /// 
        /// </param>
        /// <returns>  the result array
        /// 
        /// @throws AdapterException on a 1-Wire communication error
        /// </returns>
        private byte[] uTransaction(UPacketBuilder tempBuild)
        {
            int offset;

            try
            {
                // clear the buffers
                //serial.Flush();
                inBuffer.Clear();

                // loop to send all of the packets
                for (System.Collections.IEnumerator packet_enum = tempBuild.Packets; packet_enum.MoveNext(); )
                {

                    // get the next packet
                    RawSendPacket pkt = (RawSendPacket)packet_enum.Current;

                    // bogus packet to indicate need to wait for long DS2480 alarm reset
                    if ((pkt.dataList.Count == 0) && (pkt.returnLength == 0))
                    {
                        sleep(10);
                        continue;
                    }

                    // get the data
                    byte[] temp_buf = new byte[pkt.dataList.Count];
                    pkt.dataList.CopyTo(temp_buf);

                    // remember number of bytes in input
                    offset = inBuffer.Count;

                    // send the packet
                    //sleep(2);
                    serial.DiscardInBuffer();
                    int num = pkt.dataList.Count;
                    serial.Write(temp_buf, 0, num);
                    while(num>0)
                    {
                        num = serial.BytesToWrite;
                    }

                    // wait on returnLength bytes in inBound
                    byte[] returnBytes = new byte[pkt.returnLength];
                    int timeout = (2 * pkt.returnLength) + 100;
                    while (serial.BytesToRead < returnBytes.Length && ((timeout--) > 0))
                        sleep(1);
                    int readLen = serial.Read(returnBytes, 0, returnBytes.Length);
                    inBuffer.AddRange(returnBytes);
                }

                // read the return packet
                byte[] ret_buffer = new byte[inBuffer.Count];
                inBuffer.CopyTo(ret_buffer);

                // check for extra bytes in inBuffer
                extraBytesReceived = (inBuffer.Count > tempBuild.totalReturnLength);

                // clear the inbuffer
                inBuffer.Clear();

                return ret_buffer;
            }
            catch (System.IO.IOException e)
            {

                // need to check on adapter
                adapterPresent = false;

                // pass it on
                throw new AdapterException(e.Message);
            }
        }


        /// <summary> Sleep for the specified number of milliseconds</summary>
        private void sleep(int msTime)
        {

            // provided debug on standard out
            //         if (doDebugMessages)
            //            System.Console.Error.WriteLine("DEBUG: sleep(" + msTime + ")");
            System.Threading.Thread.Sleep(msTime);
        }
        #endregion
    }
}
