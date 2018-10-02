/*-------------------------------------------------
Alpha Vending Solutions V2.0
Verision Date : 17/11/17
Developed By:
	Duncan Fraser

Description:
	Verilog source for use by Alpha Vending Solutions.
	Soruce uses serial commands to drive servos atached to the mechnical design.
	Serial Commands list: pwd/FPGA Serial Commands
	
GPIO Pins;
	Servos 1 - 6 connected to JP2/GPIO_1 pin [0] - [5]
	Serial RX connected to GPIO_0[1]
	Serial RST Connected to GPIO_0[2]
-------------------------------------------------*/

/*-------------------------------------------------
Main Module
-------------------------------------------------*/
module AVS(
	clk,
	rx,
	led_data,
	servo_out,
	hexout0,
	hexout1,
	hexout2,
	hexout3,
	knight
	);
	
	//Inputs
	input clk;
	input rx;
	//input serial_rst;
	
	//Outputs
	output reg 	[7:0] led_data;
	output 		[5:0] servo_out;
	output  	[6:0] hexout0;
	output		[6:0] hexout1;
	output		[6:0] hexout2;
	output		[6:0] hexout3;
	output		[9:0] knight;
	
	//Servo Connections
	wire [5:0] 	servo_out_reg;
	reg	 [7:0]	sv1_pos, sv2_pos, sv3_pos, sv4_pos, sv5_pos, sv6_pos;
	reg  [3:0]	sv_no, sv_pos;
	
	//Servo Parameters
	//Tower Change
	localparam TOW1 = 8'h40 , TOW2 = 8'hC0;
	//Dispence Servos
	localparam RECYC1 = 8'h00, DISP1 = 8'hFF, HOME = 8'h88;
	localparam RECYC2 = 8'hFF, DISP2 = 8'h00;
	
	//UART Connections
	wire [7:0]	rx_data;
	wire		rx_new_data;
	wire		rst;
	reg [7:0]	fsm_mode;
	
	//State Machine States
	reg 	[3:0] state_c;
	wire 	[3:0] state_q;
	
	//Sate Machine State Parameters
	parameter IDLE = 4'b0000, SORT = 4'b0001, RUN = 4'b0010, SVRD = 4'b0011;
	
	//Sort Command Register
	reg [7:0] rx_data_sort;
	
	//Run Command Register
	reg [7:0] rx_data_run;
	
	//seven segment driver output
	wire [6:0] hexdisp;
	
	//Knight Rider Enable
	reg en;
	
	//Instansiate modules
	//Serial receive
	serial_rx 	rx1(.clk(clk), .rst(rst), .rx(rx), .data(rx_data), .new_data(rx_new_data));
	//Servos
	Servo		sv1(.clk(clk), .svr_pos(sv1_pos), .svr_out(servo_out_reg[0]));
	Servo		sv2(.clk(clk), .svr_pos(sv2_pos), .svr_out(servo_out_reg[1]));
	Servo		sv3(.clk(clk), .svr_pos(sv3_pos), .svr_out(servo_out_reg[2]));
	Servo		sv4(.clk(clk), .svr_pos(sv4_pos), .svr_out(servo_out_reg[3]));
	Servo		sv5(.clk(clk), .svr_pos(sv5_pos), .svr_out(servo_out_reg[4]));
	Servo		sv6(.clk(clk), .svr_pos(sv6_pos), .svr_out(servo_out_reg[5]));
	//Seven Segment Driver
	sev_seg		ss1(.data_in(state_q), .sevenseg_out(hexdisp));
	//Knight Rider LEDs
	kitt		kr1(.clk(clk),.en(en), .led_out(knight));
	
	//continual assignments
	assign servo_out  = servo_out_reg;
	assign rst = 0;	
	assign state_q = state_c;
	
	//All seven segs same value
	assign hexout0 = hexdisp;
	assign hexout1 = hexdisp;
	assign hexout2 = hexdisp;
	assign hexout3 = hexdisp;
	
	//State Machine Modes
	always @(posedge clk)
	begin
	
		case(state_c)
			IDLE:	//IDLE, Wait for Sort/Run commands otherwise drive servos directly
			begin
				en <= 0;
				if(rx_new_data)
				begin
					led_data = rx_data;
					fsm_mode = rx_data;
					case(fsm_mode)
					8'hAA:
						begin
							sv2_pos = HOME;
							sv3_pos = HOME;
							
							state_c = SORT;
						end
					8'hBB:
						begin
							sv1_pos = HOME;
							
							state_c = RUN;
						end
					default: 
						begin
							state_c = SVRD;
						end
					endcase
				end
			end	
			//END IDLE 
			
			SORT: //Wait for Commands from Mbed on tower to use
			begin
				en <= 0;
				if(rx_new_data) 
				begin
					sv2_pos = HOME;
					sv3_pos = HOME;
					
					led_data = rx_data;
					rx_data_sort = rx_data;
					
					/* 0xAA Set to HOME
					** 0x1F Tower 1
					** 0xF1 Tower 2
					** 0xFF Sort Complete
					*/
					case(rx_data_sort)
						8'hAA: 
						begin
							sv1_pos = HOME;
							
							sv2_pos = HOME;
							sv3_pos = HOME;
						end
						8'h1F: 
						begin
							sv1_pos = TOW1;
						end
						8'hF1: 
						begin
							sv1_pos = TOW2;
						end
						8'hFF: 
						begin
							sv1_pos = HOME;
							state_c = IDLE;
						end
						8'hBB:
						begin
							state_c = RUN;
						end
						default: 
						begin
							sv1_pos = HOME;							
							state_c = SORT;
						end
					endcase
				end
			end
			//END SORT
			
			RUN: //Wait for Commands from Mbed on Which Tower to Dispence
			begin
				en <= 1;
				if(rx_new_data)
				begin
					led_data = rx_data;
					rx_data_run = rx_data;
					
					sv1_pos = HOME;
					/* sv2 control Tower 1
					** sv3 control Tower 2
					** 0xBB Set to HOME
					** 0xFF Dispence Both Towers
					** 0xF1 Dispence tower 1, Recycle Tower 2
					** 0x1F Recycle Tower 1, Dispence Tower 2
					** 0x00 Recycle Both Towers
					** 0x99 Run Finsihed
					*/
					case(rx_data_run)
						//Home servos
						8'hBB: 
						begin
							sv2_pos = HOME; 
							sv3_pos = HOME;
						end
						//Joint Commands
						8'hFF: 
						begin
							sv2_pos = DISP1; 
							sv3_pos = DISP2;
						end
						8'hF0: 
						begin
							sv2_pos = DISP1; 
							sv3_pos = RECYC2;
						end
						8'h0F: 
						begin							
							sv2_pos = RECYC1; 
							sv3_pos = DISP2;
						end
						8'h00: 
						begin
							sv2_pos = RECYC1; 
							sv3_pos = RECYC2;
						end
						//Individual Commands
						//Tower 1
						8'hF8:
						begin
							sv2_pos = DISP1;
							sv3_pos = HOME;
						end
						8'h08:
						begin
							sv2_pos = RECYC1;
							sv3_pos = HOME;
						end
						//Tower 2
						8'h8F:
						begin
							sv2_pos = HOME;
							sv3_pos = DISP2;
						end
						8'h80:
						begin
							sv2_pos = HOME;
							sv3_pos = RECYC2;
						end
						//End Run
						8'h99: 
						begin
							sv1_pos = HOME;
							
							sv2_pos = HOME;
							sv3_pos = HOME;
							state_c = IDLE;
						end
						
						default:
						begin
							sv2_pos = HOME;
							sv3_pos = HOME;
							
							state_c = RUN;
						end
					endcase
				end
			end
			//END RUN
			
		SVRD:
			begin
				en <= 0;
				sv_no  = rx_data[7:4];
				sv_pos = rx_data[3:0];
				
				case(sv_no)
					4'b0001: 
					begin
						sv1_pos = {sv_pos, 4'b0000}; //Servo1 Data Recieved
						state_c = IDLE;
					end
					4'b0010: 
					begin
						sv2_pos = {sv_pos, 4'b0000}; //Servo2 Data Recieved
						state_c = IDLE;
					end
					4'b0011: 
					begin
						sv3_pos = {sv_pos, 4'b0000}; //Servo3 Data Recieved
						state_c = IDLE;
					end
					4'b0100: 
					begin
						sv4_pos = {sv_pos, 4'b0000}; //Servo4 Data Recieved
						state_c = IDLE;
					end
					4'b0101: 
					begin
						sv5_pos = {sv_pos, 4'b0000}; //Servo5 Data Recieved
						state_c = IDLE;
					end
					4'b0110: 
					begin
						sv6_pos = {sv_pos, 4'b0000}; //Servo6 Data Recieved
						state_c = IDLE;
					end
					4'hE: //All Max
					begin
						sv1_pos <= 8'd255;
						sv2_pos <= 8'd255;
						sv3_pos <= 8'd255;
						sv4_pos <= 8'd255;
						sv5_pos <= 8'd255;
						sv6_pos <= 8'd255;
						state_c = IDLE;
					end
					4'b0000: //All Min
					begin
						sv1_pos <= 8'd0;
						sv2_pos <= 8'd0;
						sv3_pos <= 8'd0;
						sv4_pos <= 8'd0;
						sv5_pos <= 8'd0;
						sv6_pos <= 8'd0;
						state_c = IDLE;
					end
				endcase
			end
			//end SVRD
			
		//main case default
		default: 
			begin
				state_c = IDLE;
			end
		endcase
		
	end
	
endmodule

/*-------------------------------------------------
Support Modules:
	Servo
	UART Receiver
-------------------------------------------------*/

/*------------------------------------------------
Servo Driver
clk; 
	50Mhz Clock input 50% duty Cycle
Servo_Pos;
	8-bit input for position
	0 = lower limit
	255 = upper limit
RCServo_pulse;
	Servo signal output between 1ms and 2ms 
-----------------------------------------------*/
module Servo
	(
		clk,
		svr_pos,
		svr_out
	);
	
	//inputs
	input clk;
	input [7:0] svr_pos;
	
	//Outputs
	output reg svr_out;
	
	//Clock Division
	parameter clkDiv = 390;
	
	//input register
	reg [7:0] 	svr_pos_reg;
	
	//Counters
	reg [11:0]	pulseCount;
	reg [8:0]	clkCount;
	
	//output temp reg
	reg [7:0]	svr_out_reg;
	
	//New Clock
	reg clkTick;
	
	always@(posedge clk)
	begin
		clkTick <= (clkCount == clkDiv -2);
		
		if(clkTick)
		begin
			clkCount <= 0;
		end
		else
		begin
			clkCount <= clkCount +1;
		end
		
		if(clkTick)
		begin
			if(pulseCount != 2564)
			begin
				pulseCount <= pulseCount + 1;
			end
			else 
			begin
				pulseCount <= 0;
			end
		end
		
		if(pulseCount == 0)
		begin
			svr_out_reg <= svr_pos;
		end
		
		svr_out <= (pulseCount < (50 + svr_out_reg));
		
	end
endmodule

/*------------------------------------------------
UART Receiver Modules
	Modules Sourced from
	https://embeddedmicro.com/tutorials/mojo/asynchronous-serial
-----------------------------------------------*/

module serial_rx #(
		parameter CLK_PER_BIT = 434
	)(
		input clk,
		input rst,
		input rx,
		output [7:0] data,
		output new_data
	);
	
	// clog2 is 'ceiling of log base 2' which gives you the number of bits needed to store a value
	parameter CTR_SIZE = $clog2(CLK_PER_BIT);
	
	localparam STATE_SIZE = 2;
	localparam IDLE = 2'd0,
		WAIT_HALF = 2'd1,
		WAIT_FULL = 2'd2,
		WAIT_HIGH = 2'd3;
	
	reg [CTR_SIZE-1:0] ctr_d, ctr_q;
	reg [2:0] bit_ctr_d, bit_ctr_q;
	reg [7:0] data_d, data_q;
	reg new_data_d, new_data_q;
	reg [STATE_SIZE-1:0] state_d, state_q = IDLE;
	reg rx_d, rx_q;
	
	assign new_data = new_data_q;
	assign data = data_q;
	
	always @(*) begin
		rx_d = rx;
		state_d = state_q;
		ctr_d = ctr_q;
		bit_ctr_d = bit_ctr_q;
		data_d = data_q;
		new_data_d = 1'b0;
		
		case (state_q)
		IDLE: begin
			bit_ctr_d = 3'b0;
			ctr_d = 1'b0;
			if (rx_q == 1'b0) begin
			state_d = WAIT_HALF;
			end
		end
		WAIT_HALF: begin
			ctr_d = ctr_q + 1'b1;
			if (ctr_q == (CLK_PER_BIT >> 1)) begin
			ctr_d = 1'b0;
			state_d = WAIT_FULL;
			end
		end
		WAIT_FULL: begin
			ctr_d = ctr_q + 1'b1;
			if (ctr_q == CLK_PER_BIT - 1) begin
			data_d = {rx_q, data_q[7:1]};
			bit_ctr_d = bit_ctr_q + 1'b1;
			ctr_d = 1'b0;
			if (bit_ctr_q == 3'd7) begin
				state_d = WAIT_HIGH;
				new_data_d = 1'b1;
			end
			end
		end
		WAIT_HIGH: begin
			if (rx_q == 1'b1) begin
			state_d = IDLE;
			end
		end
		default: begin
			state_d = IDLE;
		end
		endcase
		
	end
	
	always @(posedge clk) begin
		if (rst) begin
		ctr_q <= 1'b0;
		bit_ctr_q <= 3'b0;
		new_data_q <= 1'b0;
		state_q <= IDLE;
		end else begin
		ctr_q <= ctr_d;
		bit_ctr_q <= bit_ctr_d;
		new_data_q <= new_data_d;
		state_q <= state_d;
		end
		
		rx_q <= rx_d;
		data_q <= data_d;
	end
   
endmodule

/*------------------------------------------------
Seven Segment Driver
Ports;
	data_in;
		4-bit data in from state machine
	sevenseg_out;
		7-bits controling bits of seven segment display
-----------------------------------------------*/
module sev_seg
(
	data_in,
	sevenseg_out
);

	input 		[3:0] data_in;
	output 	reg [6:0] sevenseg_out;
	
	parameter IDLE = 4'b0000, SORT = 4'b0001, RUN = 4'b0010, SVRD = 4'b0011;
	//Display active low
	always @(*)
	begin
		case(data_in)//Change dependent on current state of fsm 
			IDLE: sevenseg_out = 7'b1001111;
			RUN	: sevenseg_out = 7'b0010010;
			SORT: sevenseg_out = 7'b0000110;
			SVRD: sevenseg_out = 7'b1001100;
			default: sevenseg_out = 7'b1111111;
		endcase
	end
endmodule

/*------------------------------------------------
Knight Rider LED Annimation
Ports;
	clk;
		50Mhz Clock pulse
	led_out;
		LED Output 10-bits
-----------------------------------------------*/
module kitt
(
	clk,
	en,
	led_out
);
	parameter clkcount = 5000000;
	
	input 	clk;
	input 	en;
	output	[9:0] led_out;
	
	reg [23:0] count;
	reg 	   direction = DIR_init;
	
	parameter LEDS_init = 10'b1100000000;
	parameter DIR_init = 1;
	
	reg [9:0] leds = LEDS_init;
	reg [3:0] position = DIR_init*8;
	
	assign led_out = leds;
	always@(posedge clk)
	begin
		if(en == 1)
		begin
			if(count == clkcount)
			begin
				if(direction == 0)
				begin
					leds <= leds << 1;
				end
				else
				begin
					leds <= leds >> 1;
				end
				position <= position +1;
				count = 0;
			end
			else
			begin
				count = count + 1;
			end
		end
	end
	
	
	always@(*)
	begin
		if(position < 8)
		begin
			direction = 0;
		end
		else 
		begin
			direction = 1;
		end
	end
endmodule