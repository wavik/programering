/*lcd screen dummy test */
#include <LiquidCrystal.h>

LiquidCrystal lcd(12,11,5,4,3,2);

void setup(){
   lcd.begin(12,2); 
   lcd.print("Starting System");
   
   
}
void loop(){
  int i=0;
  int b=0;
  String a=("");
  //a=("");
   while(i<10 && b==0){
   lcd.setCursor(0,1);
   lcd.print(a);
   delay(500);
   i++;
   a=a+(".");
   if(i=10)
   {
     b=1;
   }
   }
   lcd.clear();
   delay(500);
   lcd.print("All system GO");
   delay(1000);
   lcd.clear();
   testing();
 }
 void testing()
 {
   lcd.print("testing");
 }
   
