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
using DalSemi;
using DalSemi.Utils;
using DalSemi.Serial;

namespace DalSemi.OneWire.Adapter
{

   /// <summary>UPacketBuilder contains the methods to build a communication packet
   /// to the DS2480 based serial adapter.
   /// </summary>
   /// <version>0.00</version>
   /// <author>DS, SH</author>
   internal class UPacketBuilder
   {
      /// <summary> Retrieve enumeration of raw send packets
      /// 
      /// </summary>
      /// <returns>  the enumeration of packets
      /// </returns>
      public System.Collections.IEnumerator Packets
      {
         get
         {

            // put the last packet into the vector if it is non zero
            if (packet.dataList.Count > 0)
               NewPacket();

            return packetsVector.GetEnumerator();
         }

      }

      //--------
      //-------- Finals
      //--------
      //-------- Misc

      /// <summary>Byte operation                                     </summary>
      public const int OPERATION_BYTE = 0;

      /// <summary>Byte operation                                     </summary>
      public const int OPERATION_SEARCH = 1;

      /// <summary>Max bytes to stream at once  </summary>
      public const byte MAX_BYTES_STREAMED = (byte)(64);

      //-------- DS9097U function commands

      /// <summary>DS9097U funciton command, single bit               </summary>
      public const byte FUNCTION_BIT = (byte)(0x81);

      /// <summary>DS9097U funciton command, turn Search mode on      </summary>
      public const byte FUNCTION_SEARCHON = (byte)(0xB1);

      /// <summary>DS9097U funciton command, turn Search mode off     </summary>
      public const byte FUNCTION_SEARCHOFF = (byte)(0xA1);

      /// <summary>DS9097U funciton command, OneWire reset            </summary>
      public const byte FUNCTION_RESET = (byte)(0xC1);

      /// <summary>DS9097U funciton command, 5V pulse imediate        </summary>
      public const byte FUNCTION_5VPULSE_NOW = (byte)(0xED);

      /// <summary>DS9097U funciton command, 12V pulse imediate        </summary>
      public const byte FUNCTION_12VPULSE_NOW = (byte)(0xFD);

      /// <summary>DS9097U funciton command, 5V pulse after next byte </summary>
      public const byte FUNCTION_5VPULSE_ARM = (byte)(0xEF);

      /// <summary>DS9097U funciton command to stop an ongoing pulse  </summary>
      public const byte FUNCTION_STOP_PULSE = (byte)(0xF1);

      //-------- DS9097U bit polarity settings for doing bit operations

      /// <summary>DS9097U bit polarity one for function FUNCTION_BIT   </summary>
      public const byte BIT_ONE = (byte)(0x10);

      /// <summary>DS9097U bit polarity zero  for function FUNCTION_BIT </summary>
      public const byte BIT_ZERO = (byte)(0x00);

      //-------- DS9097U 5V priming values 

      /// <summary>DS9097U 5V prime on for function FUNCTION_BIT    </summary>
      public const byte PRIME5V_TRUE = (byte)(0x02);

      /// <summary>DS9097U 5V prime off for function FUNCTION_BIT   </summary>
      public const byte PRIME5V_FALSE = (byte)(0x00);

      //-------- DS9097U command masks 

      /// <summary>DS9097U mask to read or write a configuration parameter   </summary>
      public const byte CONFIG_MASK = (byte)(0x01);

      /// <summary>DS9097U mask to read the OneWire reset response byte </summary>
      public const byte RESPONSE_RESET_MASK = (byte)(0x03);

      //-------- DS9097U reset results 

      /// <summary>DS9097U  OneWire reset result = shorted </summary>
      public const byte RESPONSE_RESET_SHORT = (byte)(0x00);

      /// <summary>DS9097U  OneWire reset result = presence </summary>
      public const byte RESPONSE_RESET_PRESENCE = (byte)(0x01);

      /// <summary>DS9097U  OneWire reset result = alarm </summary>
      public const byte RESPONSE_RESET_ALARM = (byte)(0x02);

      /// <summary>DS9097U  OneWire reset result = no presence </summary>
      public const byte RESPONSE_RESET_NOPRESENCE = (byte)(0x03);

      //-------- DS9097U bit interpretation 

      /// <summary>DS9097U mask to read bit operation result   </summary>
      public const byte RESPONSE_BIT_MASK = (byte)(0x03);

      /// <summary>DS9097U read bit operation 1 </summary>
      public const byte RESPONSE_BIT_ONE = (byte)(0x03);

      /// <summary>DS9097U read bit operation 0 </summary>
      public const byte RESPONSE_BIT_ZERO = (byte)(0x00);

      /// <summary>Enable/disable debug messages                   </summary>
      public static bool doDebugMessages = false;

      //--------
      //-------- Variables
      //--------

      /// <summary> The current state of the U brick, passed into constructor.</summary>
      private UAdapterState uState;

      /// <summary> The current current count for the number of return bytes from
      /// the packet being created.
      /// </summary>
      protected internal int totalReturnLength;

      /// <summary> Current raw send packet before it is added to the packetsVector</summary>
      protected internal RawSendPacket packet;

      /// <summary> Vector of raw send packets</summary>
      protected internal System.Collections.ArrayList packetsVector;

      /// <summary> Flag to send only 'bit' commands to the DS2480</summary>
      protected internal bool bitsOnly;

      //--------
      //-------- Constructors
      //--------

      /// <summary> Constructs a new u packet builder.
      /// 
      /// </summary>
      /// <param name="startUState">  the object that contains the U brick state
      /// which is reference when creating packets
      /// </param>
      public UPacketBuilder(UAdapterState startUState)
      {

         // get a reference to the U state
         uState = startUState;

         // create the buffer for the data
         packet = new RawSendPacket();

         // create the vector
         packetsVector = new System.Collections.ArrayList(10);

         // Restart the packet to initialize
         Restart();

         // Default on SunOS to bit-banging
         //UPGRADE_ISSUE: Method 'java.lang.System.getProperty' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000_javalangSystemgetProperty_javalangString"'
         //bitsOnly = (System_Renamed.getProperty("os.name").IndexOf("SunOS") != - 1);

         // check for a bits only property
         // TODO: set bit-banging property
         System.String bits = null;//OneWireAccessProvider.getProperty("onewire.serial.forcebitsonly");
         if ((System.Object)bits != null)
         {
            if (bits.IndexOf("true") != -1)
               bitsOnly = true;
            else if (bits.IndexOf("false") != -1)
               bitsOnly = false;
         }
      }

      //--------
      //-------- Packet handling Methods 
      //--------

      /// <summary> Reset the packet builder to start a new one.</summary>
      public virtual void Restart()
      {
         // clear the vector list of packets
         System.Collections.ArrayList temp_arraylist;
         temp_arraylist = packetsVector;
         temp_arraylist.RemoveRange(0, temp_arraylist.Count);

         // truncate the packet to 0 length
         packet.dataList.Clear();

         packet.returnLength = 0;

         // reset the return cound
         totalReturnLength = 0;
      }

      /// <summary> Take the current packet and place it into the vector.  This
      /// indicates a place where we need to wait for the results from
      /// DS9097U adapter.
      /// </summary>
      public virtual void NewPacket()
      {

         // add the packet
         packetsVector.Add(packet);

         // get a new packet
         packet = new RawSendPacket();
      }

      //--------
      //-------- 1-Wire Network operation append methods 
      //--------

      /// <summary>Add the command to reset the OneWire at the current speed.
      /// 
      /// </summary>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int OneWireReset()
      {
         // set to command mode
         SetToCommandMode();

         // append the reset command at the current speed
         packet.dataList.Add((byte)(FUNCTION_RESET | uState.uSpeedMode));

         // count this as a return 
         totalReturnLength++;
         packet.returnLength++;

         // check if not streaming resets
         if (!uState.streamResets)
            NewPacket();

         // check for 2480 wait on extra bytes packet 
         if (uState.longAlarmCheck && ((uState.uSpeedMode == UAdapterState.USPEED_REGULAR) || (uState.uSpeedMode == UAdapterState.USPEED_FLEX)))
            NewPacket();

         return totalReturnLength - 1;
      }

      /// <summary> Append data bytes (read/write) to the packet.
      /// 
      /// </summary>
      /// <param name="dataBytesValue"> character array of data bytes
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int DataBytes(byte[] dataBytesValue)
      {
         byte byte_value;
         int i, j;

         // set to data mode
         if (!bitsOnly)
            SetToDataMode();

         //         // provide debug output
         //         if (doDebugMessages)
         //            System.Console.Error.WriteLine("DEBUG: UPacketbuilder-DataBytes[] length " + dataBytesValue.Length);
         //       
         // record the current count location 
         int ret_value = totalReturnLength;

         // check each byte to see if some need duplication
         for (i = 0; i < dataBytesValue.Length; i++)
         {
            // convert the rest to AdapterExceptions
            if (bitsOnly)
            {
               // change byte to bits
               byte_value = dataBytesValue[i];
               for (j = 0; j < 8; j++)
               {
                  DataBit(((byte_value & 0x01) == 0x01), false);
                  byte_value = (byte)(byte_value >> 1);
               }
            }
            else
            {
               // append the data
               packet.dataList.Add(dataBytesValue[i]);

               // provide debug output
               //               if (doDebugMessages)
               //                  System.Console.Error.WriteLine("DEBUG: UPacketbuilder-DataBytes[] byte[" + System.Convert.ToString((int) dataBytesValue[i] & 0x00FF, 16) + "]");

               // check for duplicates needed for special characters  
               if (((byte)(dataBytesValue[i] & 0x00FF) == UAdapterState.MODE_COMMAND) || (((byte)(dataBytesValue[i] & 0x00FF) == UAdapterState.MODE_SPECIAL) && (uState.revision == UAdapterState.CHIP_VERSION1)))
               {
                  // duplicate this data byte
                  packet.dataList.Add(dataBytesValue[i]);
               }

               // add to the return number of bytes
               totalReturnLength++;
               packet.returnLength++;

               // provide debug output
               //               if (doDebugMessages)
               //                  System.Console.Error.WriteLine("DEBUG: UPacketbuilder-DataBytes[] returnlength " + packet.returnLength + " bufferLength " + packet.dataList.Count);

               // check for packet too large or not streaming bytes
               if ((packet.dataList.Count > MAX_BYTES_STREAMED) || !uState.streamBytes)
                  NewPacket();
            }
         }

         return ret_value;
      }

      /// <summary> Append data bytes (read/write) to the packet.
      /// 
      /// </summary>
      /// <param name="dataBytesValue"> byte array of data bytes
      /// </param>
      /// <param name="off">  offset into the array of data to start
      /// </param>
      /// <param name="len">  length of data to send / receive starting at 'off'
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int DataBytes(byte[] dataBytesValue, int off, int len)
      {
         byte[] tmpBuf = new byte[len];
         Array.Copy(dataBytesValue, off, tmpBuf, 0, len);

         return DataBytes(tmpBuf);
      }

      /// <summary> Append a data byte (read/write) to the packet.
      /// 
      /// </summary>
      /// <param name="dataByteValue"> data byte to append
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int DataByte(byte dataByteValue)
      {

         // contruct a temporary array of characters of lenght 1
         // to use the DataBytes method
         byte[] tmpBuff = new byte[1];

         tmpBuff[0] = dataByteValue;

         // provide debug output
         //         if (doDebugMessages)
         //            System.Console.Error.WriteLine("DEBUG: UPacketbuilder-DataBytes [" + System.Convert.ToString((int) dataByteValue & 0x00FF, 16) + "]");

         return DataBytes(tmpBuff);
      }

      /// <summary> Append a data byte (read/write) to the packet.  Do a strong pullup
      /// when the byte is complete
      /// 
      /// </summary>
      /// <param name="dataByteValue"> data byte to append
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int PrimedDataByte(byte dataByteValue)
      {
         int offset, start_offset = 0;

         // create a primed data byte by using bits with last one primed
         for (int i = 0; i < 8; i++)
         {
            offset = DataBit(((dataByteValue & 0x01) == 0x01), (i == 7));
            dataByteValue = (byte)(dataByteValue >> 1);

            // record the starting offset
            if (i == 0)
               start_offset = offset;
         }

         return start_offset;
      }

      /// <summary> Append a data bit (read/write) to the packet.
      /// 
      /// </summary>
      /// <param name="DataBit">  bit to append
      /// </param>
      /// <param name="strong5V"> true if want strong5V after bit
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int DataBit(bool dataBit, bool strong5V)
      {

         // set to command mode
         SetToCommandMode();

         // append the bit with polarity and strong5V options
         packet.dataList.Add((byte)(FUNCTION_BIT | uState.uSpeedMode | ((dataBit) ? BIT_ONE : BIT_ZERO) | ((strong5V) ? PRIME5V_TRUE : PRIME5V_FALSE)));

         // add to the return number of bytes
         totalReturnLength++;
         packet.returnLength++;

         // check for packet too large or not streaming bits
         if ((packet.dataList.Count > MAX_BYTES_STREAMED) || !uState.streamBits)
            NewPacket();

         return (totalReturnLength - 1);
      }

      /// <summary> Append a Search to the packet.  Assume that any reset and Search
      /// command have already been appended.  This will add only the Search
      /// itself.
      /// 
      /// </summary>
      /// <param name="mState">OneWire state
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int Search(OneWireState mState)
      {

         // set to command mode
         SetToCommandMode();

         // Search mode on
         packet.dataList.Add((byte)(FUNCTION_SEARCHON | uState.uSpeedMode));

         // set to data mode
         SetToDataMode();

         // create the Search sequence character array
         byte[] search_sequence = new byte[16];

         // get a copy of the current ID
         byte[] id = new byte[8];

         for (int i = 0; i < 8; i++)
            id[i] = (byte)(mState.ID[i] & 0xFF);

         // clear the string
         for (int i = 0; i < 16; i++)
            search_sequence[i] = (byte)(0);

         // provide debug output
         //         if (doDebugMessages)
         //            System.Console.Error.WriteLine("DEBUG: UPacketbuilder-Search [" + System.Convert.ToString((int) id.Length, 16) + "]");

         // only modify bits if not the first Search
         if (mState.searchLastDiscrepancy != 0xFF)
         {

            // set the bits in the added buffer
            for (int i = 0; i < 64; i++)
            {

               // before last discrepancy (go direction based on ID)
               if (i < (mState.searchLastDiscrepancy - 1))
                  BitWrite(search_sequence, (i * 2 + 1), BitRead(id, i));
               // at last discrepancy (go 1's direction)
               else if (i == (mState.searchLastDiscrepancy - 1))
                  BitWrite(search_sequence, (i * 2 + 1), true);

               // after last discrepancy so leave zeros
            }
         }

         // remember this position
         int return_position = totalReturnLength;

         // add this sequence
         packet.dataList.AddRange(search_sequence);

         // set to command mode
         SetToCommandMode();

         // Search mode off
         packet.dataList.Add((byte)(FUNCTION_SEARCHOFF | uState.uSpeedMode));

         // add to the return number of bytes
         totalReturnLength += 16;
         packet.returnLength += 16;

         return return_position;
      }

      /// <summary> Append a Search off to set the current speed.</summary>
      public virtual void SetSpeed()
      {

         // set to command mode
         SetToCommandMode();

         // Search mode off and change speed
         packet.dataList.Add((byte)(FUNCTION_SEARCHOFF | uState.uSpeedMode));

         // no return byte
      }

      //--------
      //-------- U mode commands 
      //--------

      /// <summary> Set the U state to command mode.</summary>
      public virtual void SetToCommandMode()
      {
         if (!uState.inCommandMode)
         {

            // append the command to switch
            packet.dataList.Add((byte)UAdapterState.MODE_COMMAND);

            // switch the state
            uState.inCommandMode = true;
         }
      }

      /// <summary> Set the U state to data mode.</summary>
      public virtual void SetToDataMode()
      {
         if (uState.inCommandMode)
         {

            // append the command to switch
            packet.dataList.Add((byte)UAdapterState.MODE_DATA);

            // switch the state
            uState.inCommandMode = false;
         }
      }

      /// <summary> Append a get parameter to the packet.
      /// 
      /// </summary>
      /// <param name="parameter"> parameter to get
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int GetParameter(Parameter parameter)
      {

         // set to command mode
         SetToCommandMode();

         // append paramter get
         packet.dataList.Add((byte)(CONFIG_MASK | ((byte)parameter) >> 3));

         // add to the return number of bytes
         totalReturnLength++;
         packet.returnLength++;

         // check for packet too large
         if (packet.dataList.Count > MAX_BYTES_STREAMED)
            NewPacket();

         return (totalReturnLength - 1);
      }

      /// <summary> Append a set parameter to the packet.</summary>
      /// <param name="parameterValue"> parameter value
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int SetParameter(SlewRate parameterValue)
      {
         return SetParameter(
            Parameter.PARAMETER_SLEW, (byte)parameterValue);
      }
      /// <summary> Append a set parameter to the packet.</summary>
      /// <param name="parameterValue"> parameter value
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int SetParameter(ProgramPulseTime12 parameterValue)
      {
         return SetParameter(
            Parameter.PARAMETER_12VPULSE, (byte)parameterValue);
      }
      /// <summary> Append a set parameter to the packet.</summary>
      /// <param name="parameterValue"> parameter value
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int SetParameter(ProgramPulseTime5 parameterValue)
      {
         return SetParameter(
            Parameter.PARAMETER_5VPULSE, (byte)parameterValue);
      }
      /// <summary> Append a set parameter to the packet.</summary>
      /// <param name="parameterValue"> parameter value
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int SetParameter(WriteOneLowTime parameterValue)
      {
         return SetParameter(
            Parameter.PARAMETER_WRITE1LOW, (byte)parameterValue);
      }
      /// <summary> Append a set parameter to the packet.</summary>
      /// <param name="parameterValue"> parameter value
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int SetParameter(SampleOffsetTime parameterValue)
      {
         return SetParameter(
            Parameter.PARAMETER_SAMPLEOFFSET, (byte)parameterValue);
      }
      /// <summary> Append a set parameter to the packet.</summary>
      /// <param name="parameterValue"> parameter value
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      public virtual int SetParameter(AdapterBaud parameterValue)
      {
         return SetParameter(
            Parameter.PARAMETER_BAUDRATE, (byte)parameterValue);
      }
      /// <summary> Append a set parameter to the packet.</summary>
      /// <param name="parameter">      parameter to set
      /// </param>
      /// <param name="parameterValue"> parameter value
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation
      /// </returns>
      int SetParameter(Parameter parameter, byte parameterValue)
      {

         // set to command mode
         SetToCommandMode();

         // append the paramter set with value
         packet.dataList.Add((byte)((CONFIG_MASK | (byte)parameter) | parameterValue));

         // add to the return number of bytes
         totalReturnLength++;
         packet.returnLength++;

         // check for packet too large
         if (packet.dataList.Count > MAX_BYTES_STREAMED)
            NewPacket();

         return (totalReturnLength - 1);
      }

      /// <summary> Append a send command to the packet.  This command does not
      /// elicit a response byte.
      /// 
      /// </summary>
      /// <param name="command">      command to send
      /// </param>
      /// <param name="">expectResponse
      /// 
      /// </param>
      /// <returns> the number offset in the return packet to get the
      /// result of this operation (if there is one)
      /// </returns>
      public virtual int SendCommand(byte command, bool expectResponse)
      {

         // set to command mode
         SetToCommandMode();

         // append the paramter set with value
         packet.dataList.Add((byte)command);

         // check for response
         if (expectResponse)
         {

            // add to the return number of bytes
            totalReturnLength++;
            packet.returnLength++;
         }

         // check for packet too large
         if (packet.dataList.Count > MAX_BYTES_STREAMED)
            NewPacket();

         return (totalReturnLength - 1);
      }

      //--------
      //-------- 1-Wire Network result interpretation methods 
      //--------

      /// <summary> Interpret the block of bytes 
      /// 
      /// </summary>
      /// <param name="dataByteResponse"> 
      /// </param>
      /// <param name="">responseOffset
      /// </param>
      /// <param name="">result
      /// </param>
      /// <param name="">offset
      /// </param>
      /// <param name="">len
      /// </param>
      public virtual void InterpretDataBytes(byte[] dataByteResponse, int responseOffset, byte[] result, int offset, int len)
      {
         byte result_byte;
         int temp_offset, i, j;

         for (i = 0; i < len; i++)
         {
            // convert the rest to AdapterExceptions
            if (bitsOnly)
            {
               temp_offset = responseOffset + 8 * i;

               // provide debug output
               //               if (doDebugMessages)
               //                  System.Console.Error.WriteLine("DEBUG: UPacketbuilder-InterpretDataBytes[] responseOffset " + responseOffset + " offset " + offset + " lenbuf " + dataByteResponse.Length);

               // loop through and interpret each bit
               result_byte = 0;
               for (j = 0; j < 8; j++)
               {
                  result_byte = (byte)(result_byte >> 1);

                  if (InterpretOneWireBit(dataByteResponse[temp_offset + j]))
                     result_byte |= 0x80;
               }

               result[offset + i] = (byte)(result_byte & 0xFF);
            }
            else
               result[offset + i] = (byte)dataByteResponse[responseOffset + i];
         }
      }

      /// <summary> Interpret the reset response byte from a U adapter
      /// 
      /// </summary>
      /// <param name="resetResponse"> reset response byte from U
      /// 
      /// </param>
      /// <returns> the number representing the result of a 1-Wire reset
      /// </returns>
      public virtual OWResetResult InterpretOneWireReset(byte resetResponse)
      {

         if ((resetResponse & 0xC0) == 0xC0)
         {

            // retrieve the chip version and program voltage state
            uState.revision = (byte)(UAdapterState.CHIP_VERSION_MASK & resetResponse);
            uState.programVoltageAvailable = ((UAdapterState.PROGRAM_VOLTAGE_MASK & resetResponse) != 0);

            // provide debug output
            //            if (doDebugMessages)
            //               System.Console.Error.WriteLine("DEBUG: UPacketbuilder-reset response " + System.Convert.ToString((int) resetResponse & 0x00FF, 16));

            // convert the response byte to the OneWire reset result
            switch (resetResponse & RESPONSE_RESET_MASK)
            {
               case RESPONSE_RESET_SHORT:
                  return OWResetResult.RESET_SHORT;

               case RESPONSE_RESET_PRESENCE:
                  if (uState.longAlarmCheck)
                  {

                     // check if can give up checking
                     if (uState.lastAlarmCount++ > UAdapterState.MAX_ALARM_COUNT)
                        uState.longAlarmCheck = false;
                  }

                  return OWResetResult.RESET_PRESENCE;

               case RESPONSE_RESET_ALARM:
                  uState.longAlarmCheck = true;
                  uState.lastAlarmCount = 0;

                  return OWResetResult.RESET_ALARM;

               default:
                  return OWResetResult.RESET_NOPRESENCE;
               case RESPONSE_RESET_NOPRESENCE:
                  return OWResetResult.RESET_NOPRESENCE;
            }
         }
         else
            return OWResetResult.RESET_NOPRESENCE;
      }

      /// <summary> Interpret the bit response byte from a U adapter
      /// 
      /// </summary>
      /// <param name="bitResponse"> bit response byte from U
      /// 
      /// </param>
      /// <returns> boolean representing the result of a 1-Wire bit operation
      /// </returns>
      public virtual bool InterpretOneWireBit(byte bitResponse)
      {

         // interpret the bit
         if ((bitResponse & RESPONSE_BIT_MASK) == RESPONSE_BIT_ONE)
            return true;
         else
            return false;
      }

      /// <summary> Interpret the Search response and set the 1-Wire state accordingly.
      /// 
      /// </summary>
      /// <param name="bitResponse"> bit response byte from U
      /// 
      /// </param>
      /// <param name="">mState
      /// </param>
      /// <param name="">searchResponse
      /// </param>
      /// <param name="">responseOffset
      /// 
      /// </param>
      /// <returns> boolean return is true if a valid ID was found when
      /// interpreting the Search results
      /// </returns>
      public virtual bool InterpretSearch(OneWireState mState, byte[] searchResponse, int responseOffset)
      {
         byte[] temp_id = new byte[8];

         // change byte offset to bit offset
         int bit_offset = responseOffset * 8;

         // set the temp Last Descrep to none
         int temp_last_descrepancy = 0xFF;
         int temp_last_family_descrepancy = 0;

         // interpret the Search response sequence
         for (int i = 0; i < 64; i++)
         {

            // get the SerialNum bit
            BitWrite(temp_id, i, BitRead(searchResponse, (i * 2) + 1 + bit_offset));

            // check LastDiscrepancy
            if (BitRead(searchResponse, i * 2 + bit_offset) && !BitRead(searchResponse, i * 2 + 1 + bit_offset))
            {
               temp_last_descrepancy = i + 1;

               // check LastFamilyDiscrepancy
               if (i < 8)
                  temp_last_family_descrepancy = i + 1;
            }
         }

         // check
         byte[] id = new byte[8];

         for (int i = 0; i < 8; i++)
            id[i] = (byte)temp_id[i];

         // check results
         if ((!IsValidRomID(id, 0)) || (temp_last_descrepancy == 63))
            return false;

         // check for lastone
         if ((temp_last_descrepancy == mState.searchLastDiscrepancy) || (temp_last_descrepancy == 0xFF))
            mState.searchLastDevice = true;

         // copy the ID number to the buffer
         for (int i = 0; i < 8; i++)
            mState.ID[i] = (byte)temp_id[i];

         // set the count
         mState.searchLastDiscrepancy = temp_last_descrepancy;
         mState.searchFamilyLastDiscrepancy = temp_last_family_descrepancy;

         return true;
      }

      private bool IsValidRomID(byte[] address, int offset)
      {
         return ((address[offset] != 0)
            && (CRC8.Compute(address, offset, 8, 0) == 0));
      }

      /// <summary> Interpret the data response byte from a primed byte operation
      /// 
      /// </summary>
      /// <param name="">primedDataResponse
      /// </param>
      /// <param name="">responseOffset
      /// 
      /// </param>
      /// <returns> the byte representing the result of a 1-Wire data byte
      /// </returns>
      public virtual byte InterpretPrimedByte(byte[] primedDataResponse, int responseOffset)
      {
         byte result_byte = 0;

         // loop through and interpret each bit
         for (int i = 0; i < 8; i++)
         {
            result_byte = (byte)(result_byte >> 1);

            if (InterpretOneWireBit(primedDataResponse[responseOffset + i]))
               result_byte |= 0x80;
         }

         return (byte)(result_byte & 0xFF);
      }

      //--------
      //-------- Misc Utility methods 
      //--------

      /// <summary> Request the maximum rate to do an operation</summary>
      public static uint GetDesiredBaud(int operation, OWSpeed owSpeed, uint maxBaud)
      {
         uint baud = 9600;

         switch (operation)
         {
            case OPERATION_BYTE:
               if (owSpeed == OWSpeed.SPEED_OVERDRIVE)
                  baud = 115200;
               else
                  baud = 9600;
               break;

            case OPERATION_SEARCH:
               if (owSpeed == OWSpeed.SPEED_OVERDRIVE)
                  baud = 57600;
               else
                  baud = 9600;
               break;
         }

         if ((uint)baud > maxBaud)
            baud = maxBaud;

         return (uint)baud;
      }

      /// <summary> Bit utility to read a bit in the provided array of chars.
      /// 
      /// </summary>
      /// <param name="bitBuffer">array of chars where the bit to read is located
      /// </param>
      /// <param name="address">  bit location to read (LSBit of first Byte in bitBuffer
      /// is postion 0)
      /// 
      /// </param>
      /// <returns> the boolean value of the bit position
      /// </returns>
      public virtual bool BitRead(byte[] bitBuffer, int address)
      {
         int byte_number, bit_number;

         byte_number = (address / 8);
         bit_number = address - (byte_number * 8);

         return (((byte)((bitBuffer[byte_number] >> bit_number) & 0x01)) == 0x01);
      }

      /// <summary> Bit utility to write a bit in the provided array of chars.
      /// 
      /// </summary>
      /// <param name="bitBuffer">array of chars where the bit to write is located
      /// </param>
      /// <param name="address">  bit location to write (LSBit of first Byte in bitBuffer
      /// is postion 0)
      /// </param>
      /// <param name="newBitState">new bit state
      /// </param>
      public virtual void BitWrite(byte[] bitBuffer, int address, bool newBitState)
      {
         int byte_number, bit_number;

         byte_number = (address / 8);
         bit_number = address - (byte_number * 8);

         if (newBitState)
            bitBuffer[byte_number] |= (byte)(0x01 << bit_number);
         else
            bitBuffer[byte_number] &= (byte)(~(0x01 << bit_number));
      }
   }


   /// <summary>UParameterSettings contains the parameter settings state for one
   /// speed on the DS2480 based iButton COM port adapter.
   /// </summary>
   /// <version>0.00</version>
   /// <author>DS, SH</author>
   internal struct UParameterSettings
   {
      /// <summary> The pull down slew rate for this mode.</summary>
      public SlewRate pullDownSlewRate;

      /// <summary> 12 Volt programming pulse time expressed in micro-seconds.</summary>
      public ProgramPulseTime12 pulse12VoltTime;

      /// <summary> 5 Volt programming pulse time expressed in milli-seconds.</summary>
      public ProgramPulseTime5 pulse5VoltTime;

      /// <summary> Write 1 low time expressed in micro-seconds.</summary>
      public WriteOneLowTime write1LowTime;

      /// <summary> Data sample offset and write 0 recovery time expressed in 
      /// micro-seconds.</summary>
      public SampleOffsetTime sampleOffsetTime;

      /// <summary>Parameter Settings constructor.</summary>
      public UParameterSettings(SlewRate sr, ProgramPulseTime12 ppt12,
         ProgramPulseTime5 ppt5, WriteOneLowTime wolt, SampleOffsetTime sot)
      {
         pullDownSlewRate = sr;
         pulse12VoltTime = ppt12;
         pulse5VoltTime = ppt5;
         write1LowTime = wolt;
         sampleOffsetTime = sot;
      }
   }


   /// <summary>Parameter selection</summary>
   internal enum Parameter : byte
   {
      /// <summary>Parameter selection, pull-down slew rate            </summary>
      PARAMETER_SLEW = (byte)(0x10),
      /// <summary>Parameter selection, 12 volt pulse time             </summary>
      PARAMETER_12VPULSE = (byte)(0x20),
      /// <summary>Parameter selection, 5 volt pulse time              </summary>
      PARAMETER_5VPULSE = (byte)(0x30),
      /// <summary>Parameter selection, write 1 low time               </summary>
      PARAMETER_WRITE1LOW = (byte)(0x40),
      /// <summary>Parameter selection, sample offset                  </summary>
      PARAMETER_SAMPLEOFFSET = (byte)(0x50),
      /// <summary>Parameter selection, baud rate                      </summary>
      PARAMETER_BAUDRATE = (byte)(0x70)
   }


   /// <summary>Pull down slew rate times</summary>
   internal enum SlewRate : byte
   {
      /// <summary>Pull down slew rate, 15V/us                    </summary>
      SLEWRATE_15Vus = (byte)(0x00),
      /// <summary>Pull down slew rate, 2.2V/us                   </summary>
      SLEWRATE_2p2Vus = (byte)(0x02),
      /// <summary>Pull down slew rate, 1.65V/us                  </summary>
      SLEWRATE_1p65Vus = (byte)(0x04),
      /// <summary>Pull down slew rate, 1.37V/us                  </summary>
      SLEWRATE_1p37Vus = (byte)(0x06),
      /// <summary>Pull down slew rate, 1.1V/us                   </summary>
      SLEWRATE_1p1Vus = (byte)(0x08),
      /// <summary>Pull down slew rate, 0.83V/us                  </summary>
      SLEWRATE_0p83Vus = (byte)(0x0A),
      /// <summary>Pull down slew rate, 0.7V/us                   </summary>
      SLEWRATE_0p7Vus = (byte)(0x0C),
      /// <summary>Pull down slew rate, 0.55V/us                  </summary>
      SLEWRATE_0p55Vus = (byte)(0x0E),
   }


   /// <summary>12 Volt programming pulse times</summary> 
   internal enum ProgramPulseTime12 : byte
   {
      /// <summary>12 Volt programming pulse, time 32us           </summary>
      TIME12V_32us = (byte)(0x00),
      /// <summary>12 Volt programming pulse, time 64us           </summary>
      TIME12V_64us = (byte)(0x02),
      /// <summary>12 Volt programming pulse, time 128us          </summary>
      TIME12V_128us = (byte)(0x04),
      /// <summary>12 Volt programming pulse, time 256us          </summary>
      TIME12V_256us = (byte)(0x06),
      /// <summary>12 Volt programming pulse, time 512us          </summary>
      TIME12V_512us = (byte)(0x08),
      /// <summary>12 Volt programming pulse, time 1024us         </summary>
      TIME12V_1024us = (byte)(0x0A),
      /// <summary>12 Volt programming pulse, time 2048us         </summary>
      TIME12V_2048us = (byte)(0x0C),
      /// <summary>12 Volt programming pulse, time (infinite)     </summary>
      TIME12V_infinite = (byte)(0x0E)
   }


   /// <summary>5 Volt programming pulse times </summary>
   internal enum ProgramPulseTime5 : byte
   {
      /// <summary>5 Volt programming pulse, time 16.4ms        </summary>
      TIME5V_16p4ms = (byte)(0x00),
      /// <summary>5 Volt programming pulse, time 65.5ms        </summary>
      TIME5V_65p5ms = (byte)(0x02),
      /// <summary>5 Volt programming pulse, time 131ms         </summary>
      TIME5V_131ms = (byte)(0x04),
      /// <summary>5 Volt programming pulse, time 262ms         </summary>
      TIME5V_262ms = (byte)(0x06),
      /// <summary>5 Volt programming pulse, time 524ms         </summary>
      TIME5V_524ms = (byte)(0x08),
      /// <summary>5 Volt programming pulse, time 1.05s         </summary>
      TIME5V_1p05s = (byte)(0x0A),
      /// <summary>5 Volt programming pulse, time 2.10sms       </summary>
      TIME5V_2p10s = (byte)(0x0C),
      /// <summary>5 Volt programming pulse, dynamic current detect       </summary>
      TIME5V_dynamic = (byte)(0x0C),
      /// <summary>5 Volt programming pulse, time (infinite)    </summary>
      TIME5V_infinite = (byte)(0x0E)
   }


   /// <summary>Write 1 low time </summary>
   internal enum WriteOneLowTime : byte
   {
      /// <summary>Write 1 low time, 8us                        </summary>
      WRITE1TIME_8us = (byte)(0x00),
      /// <summary>Write 1 low time, 9us                        </summary>
      WRITE1TIME_9us = (byte)(0x02),
      /// <summary>Write 1 low time, 10us                       </summary>
      WRITE1TIME_10us = (byte)(0x04),
      /// <summary>Write 1 low time, 11us                       </summary>
      WRITE1TIME_11us = (byte)(0x06),
      /// <summary>Write 1 low time, 12us                       </summary>
      WRITE1TIME_12us = (byte)(0x08),
      /// <summary>Write 1 low time, 13us                       </summary>
      WRITE1TIME_13us = (byte)(0x0A),
      /// <summary>Write 1 low time, 14us                       </summary>
      WRITE1TIME_14us = (byte)(0x0C),
      /// <summary>Write 1 low time, 15us                       </summary>
      WRITE1TIME_15us = (byte)(0x0E)
   }


   /// <summary>Data sample offset and write 0 recovery times </summary>
   internal enum SampleOffsetTime : byte
   {
      /// <summary>Data sample offset and Write 0 recovery time, 4us   </summary>
      SAMPLEOFFSET_TIME_4us = (byte)(0x00),
      /// <summary>Data sample offset and Write 0 recovery time, 5us   </summary>
      SAMPLEOFFSET_TIME_5us = (byte)(0x02),
      /// <summary>Data sample offset and Write 0 recovery time, 6us   </summary>
      SAMPLEOFFSET_TIME_6us = (byte)(0x04),
      /// <summary>Data sample offset and Write 0 recovery time, 7us   </summary>
      SAMPLEOFFSET_TIME_7us = (byte)(0x06),
      /// <summary>Data sample offset and Write 0 recovery time, 8us   </summary>
      SAMPLEOFFSET_TIME_8us = (byte)(0x08),
      /// <summary>Data sample offset and Write 0 recovery time, 9us   </summary>
      SAMPLEOFFSET_TIME_9us = (byte)(0x0A),
      /// <summary>Data sample offset and Write 0 recovery time, 10us  </summary>
      SAMPLEOFFSET_TIME_10us = (byte)(0x0C),
      /// <summary>Data sample offset and Write 0 recovery time, 11us  </summary>
      SAMPLEOFFSET_TIME_11us = (byte)(0x0E)
   }


   /// <summary>DS9097U brick baud rates expressed for the DS2480 ichip</summary>  
   internal enum AdapterBaud : byte
   {
      /// <summary>DS9097U brick baud rate expressed for the DS2480 ichip, 9600 baud   </summary>
      BAUD_9600 = (byte)(0x00),
      /// <summary>DS9097U brick baud rate expressed for the DS2480 ichip, 19200 baud  </summary>
      BAUD_19200 = (byte)(0x02),
      /// <summary>DS9097U brick baud rate expressed for the DS2480 ichip, 57600 baud  </summary>
      BAUD_57600 = (byte)(0x04),
      /// <summary>DS9097U brick baud rate expressed for the DS2480 ichip, 115200 baud </summary>
      BAUD_115200 = (byte)(0x06)
   }


   /// <summary>UAdapterState contains the communication state of the DS2480
   /// based COM port adapter.
   /// //\\//\\ This class is very preliminary and not all
   /// functionality is complete or debugged.  This
   /// class is subject to change.                  //\\//\\
   /// </summary>
   /// <version>0.00</version>
   /// <author>DS, SH</author>
   internal class UAdapterState
   {

      //--------
      //-------- Finals
      //--------

      //------- DS9097U speed modes

      /// <summary>DS9097U speed mode, regular speed                         </summary>
      public const byte USPEED_REGULAR = (byte)(0x00);

      /// <summary>DS9097U speed mode, flexible speed for long lines         </summary>
      public const byte USPEED_FLEX = (byte)(0x04);

      /// <summary>DS9097U speed mode, overdrive speed                       </summary>
      public const byte USPEED_OVERDRIVE = (byte)(0x08);

      /// <summary>DS9097U speed mode, pulse, for program and power delivery </summary>
      public const byte USPEED_PULSE = (byte)(0x0C);

      //------- DS9097U modes

      /// <summary>DS9097U data mode                                  </summary>
      public const byte MODE_DATA = (byte)(0x00E1);

      /// <summary>DS9097U command mode                               </summary>
      public const byte MODE_COMMAND = (byte)(0x00E3);

      /// <summary>DS9097U pulse mode                                 </summary>
      public const byte MODE_STOP_PULSE = (byte)(0x00F1);

      /// <summary>DS9097U special mode (in revision 1 silicon only)  </summary>
      public const byte MODE_SPECIAL = (byte)(0x00F3);

      //------- DS9097U chip revisions and state

      /// <summary>DS9097U chip revision 1  </summary>
      public const byte CHIP_VERSION1 = (byte)(0x04);

      /// <summary>DS9097U chip revision mask  </summary>
      public const byte CHIP_VERSION_MASK = (byte)(0x1C);

      /// <summary>DS9097U program voltage available mask  </summary>
      public const byte PROGRAM_VOLTAGE_MASK = (byte)(0x20);

      /// <summary>Maximum number of alarms</summary>
      public const int MAX_ALARM_COUNT = 3000;

      //--------
      //-------- Variables
      //--------

      /// <summary> Parameter settings for the three logical modes</summary>
      public UParameterSettings[] uParameters;

      /// <summary> The OneWire State object reference</summary>
      public OneWireState oneWireState;

      /// <summary> Flag true if can stream bits</summary>
      public bool streamBits;

      /// <summary> Flag true if can stream bytes</summary>
      public bool streamBytes;

      /// <summary> Flag true if can stream Search</summary>
      public bool streamSearches;

      /// <summary> Flag true if can stream resets</summary>
      public bool streamResets;

      /// <summary> Current baud rate that we are communicating with the DS9097U
      /// expressed for the DS2480 ichip. <p>
      /// Valid values can be:
      /// <ul>
      /// <li> BAUD_9600
      /// <li> BAUD_19200
      /// <li> BAUD_57600
      /// <li> BAUD_115200
      /// </ul>
      /// </summary>
      public AdapterBaud ubaud;

      /// <summary> This is the current 'real' speed that the OneWire is operating at.
      /// This is used to represent the actual mode that the DS2480 is operting
      /// in.  For example the logical speed might be USPEED_REGULAR but for
      /// RF emission reasons we may put the actual DS2480 in OWSpeed.SPEED_FLEX. <p>
      /// The valid values for this are:
      /// <ul>
      /// <li> USPEED_REGULAR
      /// <li> USPEED_FLEX
      /// <li> USPEED_OVERDRIVE
      /// <li> USPEED_PULSE
      /// </ul>
      /// </summary>
      public byte uSpeedMode;

      /// <summary> This is the current state of the DS2480 adapter on program
      /// voltage availablity.  'true' if available.
      /// </summary>
      public bool programVoltageAvailable;

      /// <summary> True when DS2480 is currently in command mode.  False when it is in
      /// data mode.
      /// </summary>
      public bool inCommandMode;

      /// <summary> The DS2480 revision number.  The current value values are 1 and 2.</summary>
      public byte revision;

      /// <summary> Flag to indicate need to Search for long alarm check</summary>
      protected internal bool longAlarmCheck;

      /// <summary> Count of how many resets have been seen without Alarms</summary>
      protected internal int lastAlarmCount;

      //--------
      //-------- Constructors
      //--------

      /// <summary> Construct the state of the U brick with the defaults</summary>
      public UAdapterState(OneWireState newOneWireState)
      {

         // get a pointer to the OneWire state object
         oneWireState = newOneWireState;

         // set the defaults
         ubaud = AdapterBaud.BAUD_9600;
         uSpeedMode = USPEED_FLEX;
         revision = 0;
         inCommandMode = true;
         streamBits = true;
         streamBytes = true;
         streamSearches = true;
         streamResets = false;
         programVoltageAvailable = false;
         longAlarmCheck = false;
         lastAlarmCount = 0;

         // create the three speed logical parameter settings
         uParameters = new UParameterSettings[4];
         uParameters[0] = new UParameterSettings(
            SlewRate.SLEWRATE_1p37Vus, ProgramPulseTime12.TIME12V_infinite,
            ProgramPulseTime5.TIME5V_infinite, WriteOneLowTime.WRITE1TIME_10us,
            SampleOffsetTime.SAMPLEOFFSET_TIME_8us);
         uParameters[1] = new UParameterSettings(
            SlewRate.SLEWRATE_1p37Vus, ProgramPulseTime12.TIME12V_infinite,
            ProgramPulseTime5.TIME5V_infinite, WriteOneLowTime.WRITE1TIME_10us,
            SampleOffsetTime.SAMPLEOFFSET_TIME_8us);
         uParameters[2] = new UParameterSettings(
            SlewRate.SLEWRATE_1p37Vus, ProgramPulseTime12.TIME12V_infinite,
            ProgramPulseTime5.TIME5V_infinite, WriteOneLowTime.WRITE1TIME_10us,
            SampleOffsetTime.SAMPLEOFFSET_TIME_8us);
         uParameters[3] = new UParameterSettings(
            SlewRate.SLEWRATE_1p37Vus, ProgramPulseTime12.TIME12V_infinite,
            ProgramPulseTime5.TIME5V_infinite, WriteOneLowTime.WRITE1TIME_10us,
            SampleOffsetTime.SAMPLEOFFSET_TIME_8us);

         // adjust flex time 
         uParameters[(int)OWSpeed.SPEED_FLEX].pullDownSlewRate = SlewRate.SLEWRATE_0p83Vus;
         uParameters[(int)OWSpeed.SPEED_FLEX].write1LowTime = WriteOneLowTime.WRITE1TIME_12us;
         uParameters[(int)OWSpeed.SPEED_FLEX].sampleOffsetTime = SampleOffsetTime.SAMPLEOFFSET_TIME_10us;
      }
   }


   /// <summary>Raw Send Packet that contains a StingBuffer of bytes to send and
   /// an expected return length.
   /// </summary>
   /// <version>0.00</version>
   /// <author>DS, SH</author>
   internal class RawSendPacket
   {

      //--------
      //-------- Variables
      //--------

      /// <summary> StringBuffer of bytes to send</summary>
      //public System.Text.StringBuilder buffer;
      public System.Collections.ArrayList dataList;


      /// <summary> Expected length of return packet</summary>
      public int returnLength;

      //--------
      //-------- Constructors
      //--------

      /// <summary> Construct and initiailize the raw send packet</summary>
      public RawSendPacket()
      {
         dataList = new System.Collections.ArrayList();
         returnLength = 0;
      }
   }


   /// <summary> 1-Wire Network State contains the current 1-Wire Network state information
   /// </summary>
   /// <version>0.00</version>
   /// <author>DS, SH</author>
   internal class OneWireState
   {

      //--------
      //-------- Variables
      //--------

      /// <summary> This is the current logical speed that the 1-Wire Network is operating at. <p>
      /// </summary>
      public OWSpeed oneWireSpeed;

      /// <summary> This is the current logical 1-Wire Network pullup level.<p>
      /// </summary>
      public OWLevel oneWireLevel;

      /// <summary> True if programming voltage is available</summary>
      public bool canProgram;

      /// <summary> True if a level change is primed to occur on the next bit
      /// of communication.
      /// </summary>
      public bool levelChangeOnNextBit;

      /// <summary> True if a level change is primed to occur on the next byte
      /// of communication.
      /// </summary>
      public bool levelChangeOnNextByte;

      /// <summary> The new level value that is primed to change on the next bit
      /// or byte depending on the flags, levelChangeOnNextBit and
      /// levelChangeOnNextByte. <p>
      /// </summary>
      public OWLevel primedLevelValue;

      /// <summary> The amount of time that the 'level' value will be on for. <p>
      /// </summary>
      public OWPowerTime levelTimeFactor;

      /// <summary> Value of the last discrepancy during the last Search for an iButton.</summary>
      public int searchLastDiscrepancy;

      /// <summary> Value of the last discrepancy in the family code during the last
      /// Search for an iButton.
      /// </summary>
      public int searchFamilyLastDiscrepancy;

      /// <summary> Flag to indicate that the last device found is the last device in a
      /// Search sequence on the 1-Wire Network.
      /// </summary>
      public bool searchLastDevice;

      /// <summary> ID number of the current iButton found.</summary>
      public byte[] ID;

      /// <summary> Array of iButton families to include in any Search.</summary>
      public byte[] searchIncludeFamilies;

      /// <summary> Array of iButton families to exclude in any Search.</summary>
      public byte[] searchExcludeFamilies;

      /// <summary> Flag to indicate the conditional Search is to be performed so that
      /// only iButtons in an alarm state will be found.
      /// </summary>
      public bool searchOnlyAlarmingButtons;

      /// <summary> Flag to indicate next Search will not be preceeded by a 1-Wire reset</summary>
      public bool skipResetOnSearch;

      //--------
      //-------- Constructors
      //--------

      /// <summary> Construct the initial state of the 1-Wire Network.</summary>
      public OneWireState()
      {

         // speed, level
         oneWireSpeed = OWSpeed.SPEED_REGULAR;
         oneWireLevel = OWLevel.LEVEL_NORMAL;

         // level primed
         levelChangeOnNextBit = false;
         levelChangeOnNextByte = false;
         primedLevelValue = OWLevel.LEVEL_NORMAL;
         levelTimeFactor = OWPowerTime.DELIVERY_INFINITE;

         // adapter abilities
         canProgram = false;

         // Search options 
         searchIncludeFamilies = new byte[0];
         searchExcludeFamilies = new byte[0];
         searchOnlyAlarmingButtons = false;
         skipResetOnSearch = false;

         // new iButton object
         ID = new byte[8];

         // Search state
         searchLastDiscrepancy = 0;
         searchFamilyLastDiscrepancy = 0;
         searchLastDevice = false;
      }
   }
}
