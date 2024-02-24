#define MOTOR_PIN 8
#define SOUND_PIN 7
String readString;
// the setup function runs once when you press reset or power the board
void setup() {
    // initialize digital pin LED_BUILTIN as an output.
    
    pinMode(MOTOR_PIN, OUTPUT);
    pinMode(SOUND_PIN, OUTPUT);
    Serial.begin(9600);
    // Serial.write("hello");
    // digitalWrite(SOUND_PIN, HIGH);
}

// the loop function runs over and over again forever
void loop() {
    readString = ""; //https://forum.arduino.cc/t/read-line-from-serial/98251/5
    while (Serial.available()) {
        delay(3);  //delay to allow buffer to fill 
        if (Serial.available() >0) {
            char c = Serial.read();  //gets one byte from serial buffer
            readString += c; //makes the string readString
        } 
    }

    if (readString.length() > 0) {
        Serial.print("beg: ");
        Serial.print(readString);
        Serial.println(" :end");
    }
    // Serial.println(readString);
    if (readString == "0") {
        digitalWrite(MOTOR_PIN, LOW);
        digitalWrite(SOUND_PIN, LOW);
    } else if (readString == "1") {
        digitalWrite(MOTOR_PIN, HIGH);
    } else if (readString == "2") {
        digitalWrite(SOUND_PIN, HIGH);
    }
    // write_start();                 
}

// void write_stop() {
//     Serial.write("a");
// }

// void write_start() {
//     Serial.write("b");
// }