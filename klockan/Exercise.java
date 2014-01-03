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
import java.util.Formatter;

//Borde väl heta Exercise ?
/**
 * @author 
 *
 */
public class Exercise {

	int averageExHeartRate;
	int maxExHeartRate;
	int minExHeartRate;
	int endExHeartRate;

	int calories;
	int fat;

	int weight;
	int height;
	int maxHeartRate;

	int exType;
	// Start klockslag
	int startYear;
	int startMonth;
	int startDay;
	int startHour;
	int startMin;

	// längd på träningspass
	int lenHour;
	int lenMin;
	int lenSec;

	int zone;
	int zoneLower;
	int zoneUpper;
	// tid under zon
	int zoneBelowHour;
	int zoneBelowMin;
	int zoneBelowSec;
	int zoneBelow;

	// tid i zon
	int zoneInHour;
	int zoneInMin;
	int zoneInSec;
	int zoneIn;

	// tid över zon
	int zoneOverHour;
	int zoneOverMin;
	int zoneOverSec;
	int zoneOver;

	// Total längd på passet
	int len;

	private final String separator = ";";

	/**
	 * Tolkar en byte array med träningsdata. Det finns 65 byte.
	 */
	public Exercise(int[] data) {
		startYear = 2000 + bcd2int(data[0]); // XXX oklart om denna är
												// bcdkodad.... Dessutom egen
												// introducerad Y2K-bugg.
		startMonth = bcd2int(data[1]); // XXX Hur är månaden kodad????????
		startDay = bcd2int(data[2]);

		startHour = bcd2int(data[3]); // XXX oklart om denna är bcdkodad....
		startMin = bcd2int(data[4]);

		weight = bcd2int(data[5]) * 100 + bcd2int(data[6]);
		height = bcd2int(data[7]) * 100 + bcd2int(data[8]);
		maxHeartRate = data[9];

		calories = bcd2int(data[12]) * 100 + bcd2int(data[12]);
		fat = bcd2int(data[13]) * 100 + bcd2int(data[14]);

		lenHour = data[15]; // XXX oklart om denna är bcdkodad....
		lenMin = bcd2int(data[16]);
		lenSec = bcd2int(data[17]);

		zone = data[18];
		zoneUpper = data[19];
		zoneLower = data[20];

		zoneBelowHour = bcd2int(data[21]);
		zoneBelowMin = bcd2int(data[22]);
		zoneBelowSec = bcd2int(data[23]);

		zoneOverHour = bcd2int(data[33]);
		zoneOverMin = bcd2int(data[34]);
		zoneOverSec = bcd2int(data[35]);

		averageExHeartRate = data[36];
		minExHeartRate = data[37];
		maxExHeartRate = data[38];
		endExHeartRate = data[39];

		zoneBelow = zoneBelowHour * 3600 + zoneBelowMin * 60 + zoneBelowSec;
		zoneOver = zoneOverHour * 3600 + zoneOverMin * 60 + zoneOverSec;
		len = lenHour * 3600 + lenMin * 60 + lenSec;
		zoneIn = len - zoneOver - zoneBelow;

		zoneInHour = zoneIn / 3600;
		zoneInMin = (zoneIn - 3600 * zoneInHour) / 60;
		zoneInSec = (zoneIn - 3600 * zoneInHour - zoneInMin * 60);
	}

	public int bcd2int(int bcd) {
		return ((bcd & 0xf0) >> 4) * 10 + (bcd & 0xf);

	}

	public String getExerciseType(int exType) {
		String type = "Other";
		switch (exType) {
		case 0:
			type = "Jog";
			break;
		case 1:
			type = "Bike";
			break;
		case 2:
			type = "ExBike";
			break;
		case 3:
			type = "Walk";
			break;
		case 4:
			type = "Aerobic";
			break;
		}
		return type;
	}

	public String toString() {

		StringBuilder sb1 = new StringBuilder();
		Formatter formatter = new Formatter(sb1);
		formatter.format("Exercise type: %s %n", getExcerciseType(exType));
		formatter.format("BMI %-1.0f%n", (float) weight / (height * height)
				* 100 * 100);
		formatter.format("Date: %d-%02d-%02d%n", startYear, startMonth,
				startDay);
		formatter.format("Start: %02d:%02d%n", startHour, startMin);
		formatter.format("Length: %02d:%02d:%02d%n", lenHour, lenMin, lenSec);
		formatter.format("Weight: %d kg%n", weight);
		formatter.format("Height %d cm%n", height);
		formatter.format("Max heartrate %d bpm%n", maxHeartRate);
		formatter.format("Zone %d: %d-%d bpm%n", zone, zoneLower, zoneUpper);
		formatter.format("In zone %02d:%02d:%02d  %-1.0f%%%n", zoneInHour,
				zoneInMin, zoneInSec, (float) zoneIn / len * 100);
		formatter.format("Below zone %02d:%02d:%02d  %-1.0f%%%n",
				zoneBelowHour, zoneBelowMin, zoneBelowSec, (float) zoneBelow
						/ len * 100);
		formatter.format("Over zone %02d:%02d:%02d  %-1.0f%%%n", zoneOverHour,
				zoneOverMin, zoneOverSec, (float) zoneOver / len * 100);

		formatter.format("Max heartrate %d bpm %-1.0f%% Max %n",
				maxExHeartRate, (float) maxExHeartRate / maxHeartRate * 100);
		formatter.format("Avg heartrate %d bpm %-1.0f%% Max %n",
				averageExHeartRate, (float) averageExHeartRate / maxHeartRate
						* 100);
		formatter.format("End heartrate %d bpm %-1.0f%% Max %n",
				endExHeartRate, (float) endExHeartRate / maxHeartRate * 100);
		formatter.format("Min heartrate %d bpm %-1.0f%% Max %n",
				minExHeartRate, (float) minExHeartRate / maxHeartRate * 100);
		formatter.format("Calories burnt %d%n", calories);
		formatter.format("Fat burnt %d g%n", fat);

		return sb1.toString();
	}

	public String toCSV() {
		StringBuilder sb1 = new StringBuilder();
		Formatter formatter = new Formatter(sb1);
		formatter.format("Exercise type: %s %n", getExcerciseType(exType));

		return sb1.toString();
	}

	public void appendToFile(String data) {

	}

	public static void main(String[] tst) {
		int[] ex01 = { 0x08, 0x23, 0x13, 0x18, 0x24, 0x00, 0x77, 0x01, 0x83,
				0xBE, 0x60, 0x00, 0x27, 0x00, 0x00, 0x00, 0x03, 0x52, 0x01,
				0x99, 0x83, 0x00, 0x02, 0x12, 0x00, 0x00, 0x30, 0x00, 0x00,
				0x36, 0x00, 0x00, 0x24, 0x00, 0x00, 0x10, 0x76, 0x37, 0x9F,
				0x96, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0xBC };

		int[] ex02 = { 0x08, 0x24, 0x14, 0x22, 0x35, 0x00, 0x77, 0x01, 0x83,
				0xBE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x01,
				0x99, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x84 };

		int[] ex03 = { 0x08, 0x25, 0x15, 0x18, 0x04, 0x00, 0x77, 0x01, 0x83,
				0xBE, 0x10, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x02, 0x40, 0x02,
				0x42, 0x3B, 0x00, 0x01, 0x16, 0x00, 0x00, 0x02, 0x00, 0x00,
				0x06, 0x00, 0x00, 0x02, 0x00, 0x01, 0x14, 0x42, 0x30, 0x69,
				0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x3B };

		int[] ex04 = { 0x08, 0x25, 0x15, 0x18, 0x08, 0x00, 0x77, 0x01, 0x83,
				0xBE, 0x10, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x07, 0x02,
				0x42, 0x3B, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x2C, 0x2C,
				0x2C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x65 };

		int[] ex05 = { 0x08, 0x25, 0x15, 0x18, 0x13, 0x00, 0x77, 0x01, 0x83,
				0xBE, 0x10, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x01, 0x09, 0x03,
				0x63, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x40, 0x00, 0x00, 0x18, 0x00, 0x00, 0x10, 0x5B, 0x4C, 0x76,
				0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0xE1 };

		int[] ex06 = { 0x08, 0x25, 0x15, 0x18, 0x15, 0x00, 0x77, 0x01, 0x83,
				0xBE, 0x10, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x59, 0x03,
				0x63, 0x42, 0x00, 0x00, 0x26, 0x00, 0x00, 0x08, 0x00, 0x00,
				0x16, 0x00, 0x00, 0x06, 0x00, 0x00, 0x02, 0x47, 0x37, 0x64,
				0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0xC5 };

		int[] ex07 = { 0x08, 0x25, 0x15, 0x18, 0x16, 0x00, 0x77, 0x01, 0x83,
				0xBE, 0x10, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x02, 0x04, 0x03,
				0x63, 0x42, 0x00, 0x00, 0x20, 0x00, 0x00, 0x20, 0x00, 0x01,
				0x20, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x4B, 0x3A, 0x5B,
				0x3A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x64 };

		System.err.println((new Exercise(ex01)).toString());
		System.err.println((new Exercise(ex02)).toString());
		System.err.println((new Exercise(ex03)).toString());
		System.err.println((new Exercise(ex04)).toString());
		System.err.println((new Exercise(ex05)).toString());
		System.err.println((new Exercise(ex06)).toString());
		System.err.println((new Exercise(ex07)).toString());

		// try {
		// FileOutputStream fos = new FileOutputStream("hello.txt", true);
		// //DataOutputStream dos = new DataOutputStream (fos);
		// fos.write(p.toString().getBytes());
		// fos.flush();
		// fos.close();
		// } catch (IOException e) {
		// e.printStackTrace();
		// }
	}

}