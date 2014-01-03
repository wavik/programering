/*
    Copyright 2008, 2009 Theo Öjerteg

    This file is part of Sonic Reader.

    Sonic Reader is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
  
    Sonic Reader is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Sonic Reader.  If not, see <http://www.gnu.org/licenses/>.

*/
import java.io.*;
import java.util.*;
import gnu.io.*;

/**
 * Class declaration
 *
 *
 * @author
 * @version 1.8, 08/03/00
 */
public class SimpleRead implements Runnable, SerialPortEventListener {
    static CommPortIdentifier portId;
    static Enumeration	      portList;
    InputStream		      inputStream;
    SerialPort		      serialPort;
    Thread		      readThread;
    OutputStream       outputStream;


	int[] cmdGetData = { 0x00, 0xaa, 0x55, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
	int[] cmdOk = { 0x55 };
	int[] cmdRecvData = { 0xaa, 0x55, 0x03, 0x00, 0x01, 0x00, 0x28, 0x00, 0x00, 0x2b };

	int[] cmdWriteData = { 0x00, 0xaa, 0x55, 0x05, 0x00, 0x01, 0x00, 0x0e, 0x00, 0x00, 0x13 };


    /**
     * Method declaration
     *
     *
     * @param args
     *
     * @see
     */
    public static void main(String[] args) {
    boolean		      portFound = false;
    String		      defaultPort = "/dev/ttyUSB0";

 	if (args.length > 0) {
	    defaultPort = args[0];
	} 
   
	portList = CommPortIdentifier.getPortIdentifiers();

	while (portList.hasMoreElements()) {
	    portId = (CommPortIdentifier) portList.nextElement();
	    if (portId.getPortType() == CommPortIdentifier.PORT_SERIAL) {
		if (portId.getName().equals(defaultPort)) {
		    System.out.println("Found port: "+defaultPort);
		    portFound = true;

		    SimpleRead reader = new SimpleRead();

		} 
	    } 
	} 
	if (!portFound) {
	    System.out.println("port " + defaultPort + " not found.");
	} 
 	
    } 

    /**
     * Constructor declaration
     *
     *
     * @see
     */
    public SimpleRead() {
	try {
	    serialPort = (SerialPort) portId.open("SimpleReadApp", 2000);
	} catch (PortInUseException e) {}

	try {
	    inputStream = serialPort.getInputStream();
	    outputStream = serialPort.getOutputStream();
	} catch (IOException e) {
	    System.err.println("Kunde inte öppna strömmar.");
	    System.exit(-1);
	}
	
	try {
	    serialPort.addEventListener(this);
	} catch (TooManyListenersException e) {
	    System.err.println("Kunde inte lägga till lyssnare");
	    System.exit(-1);
	}

	serialPort.notifyOnDataAvailable(true);


	try {
	    serialPort.notifyOnOutputEmpty(true);
	} catch (Exception e) {
	    System.out.println("Error setting event notification");
	    System.out.println(e.toString());
	    System.exit(-1);
	}

	// Ful fix från webben....
	// http://forum.java.sun.com/thread.jspa?threadID=673793&start=15&tstart=0
	boolean resetSpeed = false;
	while (!resetSpeed)
	    try {
		System.err.println("Trying to set serial params...");
		serialPort.setSerialPortParams(9600, SerialPort.DATABITS_8, 
					       SerialPort.STOPBITS_1, 
					       SerialPort.PARITY_NONE);
		resetSpeed = true;
				System.err.println("Serial parameters set!");
			} catch (Exception e) {
				System.err.println("SetSerialPortParams failed!...");
			}


// 	try {
// 	    serialPort.setSerialPortParams(9600, SerialPort.DATABITS_8, 
// 					   SerialPort.STOPBITS_1, 
// 					   SerialPort.PARITY_NONE);
// 	} catch (UnsupportedCommOperationException e) {
// 	    System.err.println("Kunde inte konfa port");
// 	    System.exit(-1);
// 	}





	readThread = new Thread(this);

	readThread.start();
	writeData(cmdGetData);

    }

    /**
     * Method declaration
     *
     *
     * @see
     */
    public void run() {
	try {
	    Thread.sleep(2000);
	} catch (InterruptedException e) {}
    } 

    /**
     * Method declaration
     *
     *
     * @param event
     *
     * @see
     */
    public void serialEvent(SerialPortEvent event) {

	System.err.println("Receiving...");
	switch (event.getEventType()) {

	case SerialPortEvent.BI:

	case SerialPortEvent.OE:

	case SerialPortEvent.FE:

	case SerialPortEvent.PE:

	case SerialPortEvent.CD:

	case SerialPortEvent.CTS:

	case SerialPortEvent.DSR:

	case SerialPortEvent.RI:

	case SerialPortEvent.OUTPUT_BUFFER_EMPTY:
	    break;

	case SerialPortEvent.DATA_AVAILABLE:
	    byte[] readBuffer = new byte[20];
	    int numBytes = 0;
	    try {
		while (inputStream.available() > 0) {
		    numBytes = inputStream.read(readBuffer);
		} 

		for(int i = 0; i<numBytes;i++)
		    System.out.println(readBuffer[i]);
		System.out.print(new String(readBuffer)+"\nOk!\n");

	    } catch (IOException e) {}

	    break;
	}
    } 




    /**
     * Method declaration
     *
     *
     * @param args
     *
     * @see
     */
    public  void writeData(int[] cmd) {


// 	try {
// 	    outputStream = serialPort.getOutputStream();
// 	} catch (IOException e) {}
	
// 	try {
// 	    serialPort.setSerialPortParams(9600, 
// 					   SerialPort.DATABITS_8, 
// 					   SerialPort.STOPBITS_1, 
// 					   SerialPort.PARITY_NONE);
// 	} catch (UnsupportedCommOperationException e) {
// 	    System.err.println("fel: ");
// 	    System.exit(-1);
// 	}
	

// 	try {
// 	    serialPort.notifyOnOutputEmpty(true);
// 	} catch (Exception e) {
// 	    System.out.println("Error setting event notification");
// 	    System.out.println(e.toString());
// 	    System.exit(-1);
// 	}
	
		    
	System.out.println(
			   "Writing to "
			   +serialPort.getName());
	
	try {
	    for(int d: cmd)
	    outputStream.write(d);
	} catch (IOException e) {
	    System.out.println("Error writing to "
				+serialPort.getName());
	    System.out.println(e.toString());
	    System.exit(-1);
	}
 	try {
 	    Thread.sleep(2000);  // Be sure data is xferred before closing
 	} catch (Exception e) {}

 	try {
	    for(int d: cmdOk)
	        outputStream.write(d);
 	} catch (IOException e) {}
	
	serialPort.close();
    } 

	

}