#include <OneWire.h>
#include <LiquidCrystal.h>
// LCD=======================================================
//initialize the library with the numbers of the interface pins
LiquidCrystal lcd(12, 11, 5, 4, 3, 2);
#define LCD_WIDTH 16
#define LCD_HEIGHT 2

/* DS18S20 Temperature chip i/o */

OneWire  ds(7);  // on pin 9
#define MAX_DS18B20_SENSORS 2
byte addr[MAX_DS18B20_SENSORS][8];
float temp;
void setup(void) 
{
  Serial.begin(57600);
  lcd.begin(LCD_WIDTH, LCD_HEIGHT,1);
  lcd.setCursor(0,0);
  lcd.print("DS1820 Test");
  if (!ds.search(addr[0])) 
  {
    lcd.setCursor(0,0);
    lcd.print("No more addresses.");
    ds.reset_search();
    delay(250);
    return;
  }
  if ( !ds.search(addr[1])) 
  {
    lcd.setCursor(0,0);
    lcd.print("No more addresses.");
    ds.reset_search();
    delay(250);
    return;
  }
}
int HighByte, LowByte, TReading, SignBit, Tc_100, Whole, Fract;
char buf[16];
char buf2[16];

void loop(void) 
{
  byte i, sensor;
  byte present = 0;
  byte data[12];

  for (sensor=0;sensor<MAX_DS18B20_SENSORS;sensor++)
  {
    if ( OneWire::crc8( addr[sensor], 7) != addr[sensor][7]) 
    {
      lcd.setCursor(0,0);
      lcd.print("CRC is not valid");
      return;
    }

    if ( addr[sensor][0] != 0x28) 
    {
      lcd.setCursor(0,0);
      lcd.print("Device is not a DS18B20 family device.");
      return;
    }

    ds.reset();
    ds.select(addr[sensor]);
    ds.write(0x44,1);         // start conversion, with parasite power on at the end

    delay(1000);     // maybe 750ms is enough, maybe not
    // we might do a ds.depower() here, but the reset will take care of it.

    present = ds.reset();
    ds.select(addr[sensor]);    
    ds.write(0xBE);         // Read Scratchpad

    for ( i = 0; i < 9; i++) 
    {           // we need 9 bytes
      data[i] = ds.read();
    }

    LowByte = data[0];
    HighByte = data[1];
    TReading = (HighByte << 8) + LowByte;
    SignBit = TReading & 0x8000;  // test most sig bit
    if (SignBit) // negative
    {
      TReading = (TReading ^ 0xffff) + 1; // 2's comp
    }
     Tc_100 = (6 * TReading) + TReading / 4;
    //Tc_100 = (TReading*100/2);    

    Whole = Tc_100 / 100;  // separate off the whole and fractional portions
    Fract = Tc_100 % 100;

    sprintf(buf, "%d:%c%d.%d\337C     ",sensor,SignBit ? '-' : '+', Whole, Fract < 10 ? 0 : Fract);

    lcd.setCursor(0,0);
    lcd.print(buf);
   // lcd.setCursor(0,1);
   // if (Tc_100<25000){
   //       lcd.print("< 25000");
   // }else{
   //   lcd.print("> 25000");
   // }
//////////// MCP 9700
// float temperature = 0.0;   // stores the calculated temperature
//    int sample;                // counts through ADC samples
//    float ten_samples = 0.0;   // stores sum of 10 samples
//  
//    // take 10 samples from the MCP9700
//    for (sample = 0; sample < 10; sample++) {
//        // convert A0 value to temperature
//        temperature = ((float)analogRead(0) * 5.0 / 1024.0) - 0.5;
//        temperature = temperature / 0.01;
//        // sample every 0.1 seconds
//        delay(100);
//        // sum of all samples
//        ten_samples = ten_samples + temperature;
//    }
//    // get the average value of 10 temperatures
//    temperature = ten_samples / 10.0;
//    // send temperature out of serial port
//    Serial.print(temperature);
//    Serial.println(" deg. C");
//    //sprintf(buf2, "%a \337C", temperature);
//    //lcd.setCursor(0,1);
//    //lcd.print(buf2);
//    ten_samples = 0.0;
 temp = analogRead(0)*5/1024.0;
  temp = temp - 0.5;
  temp = temp / 0.01;
  Serial.println(temp);
  sprintf(buf2, "%a \337C", temp);
    lcd.setCursor(0,1);
    lcd.print(buf2);
  delay(500);
}
}
