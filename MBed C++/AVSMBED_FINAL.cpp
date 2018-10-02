#include <string>
#include <iostream>
#include <vector>
#include <sstream>
#include <cstdlib>
#include <cstdint>
#include "mbed.h"
#include "MCP23017.h"
#include "WattBob_TextLCD.h"
#include "TCS3472_I2C.h"
#include "stdint.h"
#include "VL6180.h"

#define IDENTIFICATIONMODEL_ID 0x0000
#define     BACK_LIGHT_ON(INTERFACE)    INTERFACE->write_bit(1,BL_BIT)
#define     BACK_LIGHT_OFF(INTERFACE)   INTERFACE->write_bit(0,BL_BIT)

//establish serial ports and sensor inputs
Serial pc(USBTX, USBRX);
Serial FPGA(p13,p14);
VL6180  TOF_sensor(p28, p27);
TCS3472_I2C rgb_sensor(p9, p10);

DigitalIn   B0(A0);     //Digital Pins for Card Reader Code
DigitalIn   B1(A1);
DigitalIn   B2(A2);
DigitalIn   B3(A3);

MCP23017            *par_port;
WattBob_TextLCD     *lcd;


using namespace std;
int rgb_readings[4];                              //Capture Raw RGB data

double raw_red;                                 //Doubles for manipulation of RGB data
double raw_green;
double raw_blue;
double raw_clr;
    
//doubles for storing the normalised RGB
double rgb_red;
double rgb_green;
double rgb_blue;
double rgb_clr;   

int Code1;                                  //Card Reader Code memory
int Code2;
int Code3;
int Code4;

std::vector<string> tower1array;           //Define one array for each tower
std::vector<string> tower2array;
std::vector<std::string> dispensed_list;
std::vector<std::string> recycled;
std::string allergy;                        // Comes from Card Reader

bool sys_state = 0;

string ColAvg(){

    rgb_sensor.getAllColors(rgb_readings);     //Read in colour values and convert to RGB 0-255 range
    raw_red = rgb_readings[1];
    raw_green = rgb_readings[2];
    raw_blue = rgb_readings[3];
    raw_clr = rgb_readings[0];
    
    //convert to a 0-255 range
    rgb_red = (raw_red/raw_clr) * 255;
    rgb_green = (raw_green/raw_clr) * 255;
    rgb_blue = (raw_blue/raw_clr) * 255;
    rgb_clr = raw_clr;
    
    //initially set detected to false
    bool detected = false;

//based on RGB ranges, assign colours
 if ((rgb_red > 175) && (rgb_red < 205) && (rgb_green > 35) && (rgb_green < 55) && (rgb_blue > 35) && (rgb_blue < 60)){            //Define a range for each colour to identify blocks
                                     detected = true;
                return "Red";
                                  }
         else if ((rgb_red > 35) && (rgb_red < 75) && (rgb_green > 115) && (rgb_green < 145) && (rgb_blue > 45) && (rgb_blue < 70)){
                                        detected = true;
                return "Green";
                                 }
         else if ((rgb_red > 80) && (rgb_red < 95) && (rgb_green > 110) && (rgb_green < 130) && (rgb_blue > 30) && (rgb_blue < 50)){
                                        detected = true;
                return "Green";
                                 }
         else if ((rgb_red > 115) && (rgb_red < 210) && (rgb_green > 100) && (rgb_green < 190) && (rgb_blue > 35) && (rgb_blue < 70) && (rgb_clr > 50000) && (rgb_clr < 65537)){
                                     detected = true;
                return "Yellow";
                                 }

         else if ((rgb_red > 95) && (rgb_red < 240) && (rgb_green > 85) && (rgb_green < 275) && (rgb_blue > 95) && (rgb_blue < 125) && (rgb_clr > 65000) && (rgb_clr < 65537)){
                                     detected = true;
                return "White";
                                 }
         else if ((rgb_red > 50) && (rgb_red < 115) && (rgb_green > 80) && (rgb_green < 115) && (rgb_blue > 50) && (rgb_blue < 105) && (rgb_clr > 4000) && (rgb_clr < 9000)){
                                     detected = true;
                return "Black";
                                 }
         else if ((rgb_red > 25) && (rgb_red < 55) && (rgb_green > 75) && (rgb_green < 95) && (rgb_blue > 115) && (rgb_blue < 140)){
                                     detected = true;
                return "Blue";
                                 }
         else if ((rgb_red > 150) && (rgb_red < 175) && (rgb_green > 55) && (rgb_green < 75) && (rgb_blue > 30) && (rgb_blue < 45)){
                                    detected = true;
                return "Orange";
                                 }
                                 
        //if still no colour has been detected, the detected will remain false and 
                                 
         if (detected == false){      
        return "Cannot Detect";
        }
        
    } 
    
//function to sort the blocks into the towers

void Store(string col_avg)
{
 
    
    FPGA.putc(0xAA);
    if(tower1array.size() < 12){
            tower1array.push_back(col_avg);         //Stores most recent value of Col_Avg in the array
            //send 0x1F to fpga
            FPGA.putc(0x1F);                        //Moves servo to tip block into tower
            //std::cout << "SORTED TO TOWER 1 "<< endl;
            pc.printf("1\n");
            wait(0.5);
            FPGA.putc(0xAA);  
        } else if(tower2array.size() < 12){
            tower2array.push_back(col_avg); 
            //send 0xF1 to fpga
            FPGA.putc(0xF1);
            //std::cout << "SORTED TO TOWER 2 "<< endl;
            pc.printf("2\n"); 
            wait(0.5);
            FPGA.putc(0xAA);
        } else {
            //send 0xFF to fpga
            FPGA.putc(0xFF);
            //std::cout << "MAXIMUM CAPACITY REACHED "<< endl;
            pc.printf("0\n"); 
        }
}

//Connect code with requested blocks and entered allergy
int requested; // << COMES FROM USER
int dispensed = 0;
int recycle_alternator = 1;

//function that checks if the requested snacks can be managed by the system
bool request_manageable(std::vector<std::string> one, std::vector<std::string> two, int requested, std::string allergy){
    
    
    int allergic_blocks = 0;
    
    //count allergic blocks in tower 1
    for(int i = 0; i<one.size();i++){
        if(one[i] == allergy){
            allergic_blocks = allergic_blocks + 1;
        }
    }
    
    //count allergic blocks in tower 2
    for(int i = 0; i<two.size();i++){
        if(two[i] == allergy){
           allergic_blocks = allergic_blocks + 1;
        }
    }
    
    //calculate number of blocks that teh user is not allergic to in system and check if its greater or equal to requested
    if((one.size()+two.size() - allergic_blocks) >= requested){
        return true;
    } else{
        return false;
    }   
}

//returns the number of possible snacks the system can dispense, while ignoring the blocks the user is allergic to
int manageable_snacks(std::vector<std::string> one, std::vector<std::string> two,std::string allergy){
    
    int m_snacks = 0;
    
    //counts non-allergic blocks in tower 1
    for(int i = 0; i<one.size();i++){
        if(one[i] != allergy){
            m_snacks = m_snacks + 1;
        }
    }
    
      //counts non-allergic blocks in tower 2
    for(int i = 0; i<two.size();i++){
        if(two[i] != allergy){
           m_snacks = m_snacks + 1;
        }
    }
    
    return m_snacks;
}


int main()
{    
FPGA.baud (115200);                         // Define FPGA baud rate
    while(true){
    
    typedef unsigned char byte;
            
    
        char command = pc.getc();
        switch(command){
            
            
            //for testing purposes
            {case '?':
             pc.printf("0\n");   
            break;}
            
            //dispense sequence
            {case 'D':
                
                FPGA.putc(0xbb); //enter dispense mode
                
                //read and convert inputs
                std::string con_in;
                std::cin >> con_in;
                std::string input = con_in;
                std::string svr = input.substr(1,input.size() -1 );

                int num;
                const char* svrnum_conv = (const char*) svr.c_str(); // cast from string to char*
                num = atoi(svrnum_conv); //convert char to integer
                
                requested = num;
            
            dispensed = 0;
            
            //check if requested snacks is possible to be delivered

            if(request_manageable(tower1array,tower2array,requested, allergy)){
    
    
    //this will loop until dispensed snacks equals teh requested number of snacks
        while(dispensed < requested){
            std::cout << "0" << std::endl;
            wait(0.5);
            
            
            //algorithm 1, if both towers full
            if(tower1array.size()>0 && tower2array.size()>0){
            
                //both towers have non-allergic blocks
                if(tower1array[0] != allergy && tower2array[0] != allergy){
                    if(requested - dispensed > 1){
                    dispensed_list.push_back(tower1array[0]);
                    tower1array.erase(tower1array.begin()+0);
                    dispensed_list.push_back(tower2array[0]);
                    tower2array.erase(tower2array.begin()+0);
                    dispensed = dispensed + 2;
                    
                    //Dispense from both towers
                    FPGA.putc(0xFF);
                    wait (0.5);
                    FPGA.putc(0xBB);
                    } else {
                         dispensed_list.push_back(tower1array[0]);
                    tower1array.erase(tower1array.begin()+0);
                    dispensed = dispensed + 1;
                    
                    //Dispense from tower 2
                    FPGA.putc(0x8F);
                    wait (0.5);
                    FPGA.putc(0xBB);
                        
                        }
                    
                //if only tower 1 has non-allergic block
                } else if(tower1array[0] != allergy && tower2array[0] == allergy) {
                    dispensed_list.push_back(tower1array[0]);
                    tower1array.erase(tower1array.begin()+0);
                    dispensed = dispensed + 1;
                    
                    //Dispense from tower 1
                    FPGA.putc(0xF8);
                    wait (0.5);
                    FPGA.putc(0xBB);
                    
                //if only tower 2 has non-allergic blocks
                } else if (tower1array[0] == allergy && tower2array[0] != allergy) {
                    dispensed_list.push_back(tower2array[0]);
                    tower2array.erase(tower2array.begin()+0);
                    dispensed = dispensed + 1;
                    
                    //Dispense from tower 2
                    FPGA.putc(0x8F);
                    wait (0.5);
                    FPGA.putc(0xBB);
                    
                //if both towers have allergic blocks
                } else if (tower1array[0] == allergy && tower2array[0] == allergy) {
                    
                    //alternator switches between 1 and 2, each time this if loop is entered
                    if(recycle_alternator ==1){
                    recycled.push_back(tower1array[0]);
                    tower1array.erase(tower1array.begin()+0);
                        recycle_alternator =2;
                        
                        //recycle tower 1
                        FPGA.putc(0x08);
                        wait (0.5);
                    FPGA.putc(0xBB);
                        
                    } else if(recycle_alternator == 2){
                        recycled.push_back(tower2array[0]);
                        tower2array.erase(tower2array.begin()+0);
                        recycle_alternator =1;
                        
                        //recycle tower 2
                        FPGA.putc(0x80);
                        wait (0.5);
                    FPGA.putc(0xBB);
                    }
                }
                
            //if only tower1 has blocks    
            } else if (tower1array.size() == 0 && tower2array.size()>0){
                
                if(tower2array[0] != allergy){
                    dispensed_list.push_back(tower2array[0]);
                    tower2array.erase(tower2array.begin()+0);
                    dispensed = dispensed + 1;
                    
                    //Dispense from tower 2
                    FPGA.putc(0x8F);
                    wait (0.5);
                    FPGA.putc(0xBB);
                    
                } else {
                    
                    recycled.push_back(tower2array[0]);
                    tower2array.erase(tower2array.begin()+0);
                    
                    //recycle tower 2
                    FPGA.putc(0x80);
                    wait (0.5);
                    FPGA.putc(0xBB);
                    
                }
                
            //if only tower 2 has blocks
            }else if (tower1array.size() > 0 && tower2array.size() == 0){
                
                if(tower1array[0] != allergy){
                    dispensed_list.push_back(tower1array[0]);
                    tower1array.erase(tower1array.begin()+0);
                    dispensed = dispensed + 1;
                    
                    //Dispense from tower 1
                    FPGA.putc(0xF8);
                    wait (0.5);
                    FPGA.putc(0xBB);
                    
                } else {
                    
                    recycled.push_back(tower1array[0]);
                    tower1array.erase(tower1array.begin()+0);
                    
                    //recycle tower 1
                    FPGA.putc(0x08);
                    wait (0.5);
                    FPGA.putc(0xBB);
                    
                }
                
            }
            
        }
        if(dispensed == requested){
            FPGA.putc(0x99);
            }
        
    } else{
        std::cout << "1" << std::endl;

    }
    
dispensed_list.clear();
dispensed = 0;
allergy = "None";
recycled.clear();
FPGA.putc(0x99);

break;}

            {case 'C':                          //Read and send raw colour data for testing
                rgb_sensor.getAllColors(rgb_readings);
                raw_red = rgb_readings[1];
                raw_green = rgb_readings[2];
                raw_blue = rgb_readings[3];
                raw_clr = rgb_readings[0];
                rgb_red = (raw_red/raw_clr) * 255;
                rgb_green = (raw_green/raw_clr) * 255;
                rgb_blue = (raw_blue/raw_clr) * 255;
                rgb_clr = raw_clr;
            pc.printf("%d %d %d %d\r\n", rgb_readings[1], rgb_readings[2],rgb_readings[3], rgb_readings[0]);
            pc.printf("%f %f %f %f\r\n", rgb_red, rgb_green, rgb_blue, rgb_clr);
            break;}
            
            {case 'G' :                        //Sort one block.
            rgb_sensor.getAllColors(rgb_readings);
                string col = ColAvg();
                if( col != "Cannot Detect"){
                Store(col);
                } else{
                pc.printf ("3\n");
                    }
                break;}
                
             {case 'F' :                                       //Test COM port open
                std::cout << "Communication Works\n" << endl;
                break;}
                
            {case 'X' :                                      
                std::cout <<manageable_snacks(tower1array, tower2array,allergy) << endl;
                break;}   
    
            {case 'Z':                                          //Display contents of both towers
                for(int i = 0; i<tower1array.size();i++)
    
                std::cout << "TOWER1 " + tower1array[i] << endl;
                for(int i = 0; i<tower2array.size();i++)

                std::cout << "TOWER2 " + tower2array[i] << endl; 
            default: 
            break;}
            
             {case 'S':
                std::string con_in;
                //std::cout << "Servo Command: ";
                std::cin >> con_in;
                //std::cout << "\n";
    
                std::string svr = con_in.substr(con_in.find(":") +1, sizeof(con_in) + 1);
                std::string svrnum = svr.substr(0, svr.find("&"));
                std::string svrpos = svr.substr(svr.find("&") + 1, sizeof(svr));

                int num;
                const char* svrnum_conv = (const char*) svrnum.c_str(); // cast from string to char*
                num = atoi(svrnum_conv); //convert char to integer
   
                int pos;
                const char* svrpos_conv = (const char*) svrpos.c_str(); // cast from string to char*
                pos = atoi(svrpos_conv); //convert char to integer
      
      
                byte svrout = pos | (num << 4);
                FPGA.putc(svrout);
                break;}
            
            {case 'B':                              
                while(1){ 
                        if(B0 > 0.3f) {             // Await state change on first bit (card inserted)
                            wait(2);                // Wait two seconds for card positioning
                            if(B0 > 0.3f) {         // Set value of Code1 - Code4 according to voltage values at MBED pins 15-18
                                Code1 = 1;
                            } else {
                                Code1 = 0;
                            }
                            if(B1 > 0.3f) {
                                Code2 = 1;
                            } else {
                                Code2 = 0;
                            }
                            if(B2 > 0.3f) {
                                Code3 = 1;
                            } else {
                                Code3 = 0;
                            }
                            if(B3 > 0.3f) {
                                Code4 = 1;
                            } else {
                                Code4 = 0;
                            }
                           pc.printf("%d%d%d%d\n",Code1, Code2, Code3, Code4);              //Send raw sequence for interpretation by Visual C#
                            if (Code1 == 0 && Code2 == 0 && Code3 == 0 && Code4 == 0)       //Set string 'allergy' to correct value
                                    {allergy = "None";}
                            else if (Code1 == 0 && Code2 == 0 && Code3 == 0 && Code4 == 1)
                                    {allergy = "Red";}
                            else if (Code1 == 0 && Code2 == 0 && Code3 == 1 && Code4 == 0)
                                    {allergy = "Green";}
                            else if (Code1 == 0 && Code2 == 1 && Code3 == 0 && Code4 == 0)
                                    {allergy = "Blue";}
                            else if (Code1 == 1 && Code2 == 0 && Code3 == 0 && Code4 == 0)
                                    {allergy = "Yellow";}
                            else if (Code1 == 0 && Code2 == 0 && Code3 == 1 && Code4 == 1)
                                    {allergy = "Orange";}
                            else if (Code1 == 0 && Code2 == 1 && Code3 == 0 && Code4 == 1)
                                    {allergy = "White";}
                            else if (Code1 == 1 && Code2 == 0 && Code3 == 0 && Code4 == 1)
                                    {allergy = "Black";}
                            break;
                            }
                        }
                break;
        }  
    }
}
}
