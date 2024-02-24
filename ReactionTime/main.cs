using Godot;
using System;
using System.IO.Ports;
using System.Diagnostics;
using Godot.NativeInterop;
public partial class main : Node2D
{
	
	const double TIME_BETWEEN_CHANGE = 3.5;
	Color COLOUR_RED = Color.Color8(255, 0, 0, 255);
	Color COLOUR_GREEN = Color.Color8(0, 255, 0, 255);
	Color COLOUR_BLUE = Color.Color8(0, 0, 255, 255);
	Color COLOUR_WHITE = Color.Color8(255, 255, 255, 255);
	// Called when the node enters the scene tree for the first time.
	SerialPort serialPort;
	ColorRect background;
	Timer timer, countdown_timer;
	RichTextLabel countdown_text, info_text, port_info_text;
	Color[] colour_list = new Color[3];
	Random rand = new Random();
	Color current_colour;
	bool countdown_timer_started, countdown_began, countdown, already = false;
	bool using_arduino = true;
	double time_passed_from_change = 0;
	Button light_button, sound_button, movement_button, end_button, set_arduino_port_input;
	AudioStreamPlayer sound_player;
	LineEdit line_edit;
	int gamemode = -1;
	public override void _Ready()
	{	
		// stopwatch = new Stopwatch();
		light_button = GetNode<Button>("LightButton");
		sound_button = GetNode<Button>("SoundButton");
		movement_button = GetNode<Button>("MovementButton");
		end_button = GetNode<Button>("EndButton");
		set_arduino_port_input = GetNode<Button>("SetArduinoPort");

		line_edit = GetNode<LineEdit>("LineEdit");


		sound_player = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		background = GetNode<ColorRect>("Background");
		colour_list[0] = COLOUR_RED;
		colour_list[1] = COLOUR_GREEN;
		colour_list[2] = COLOUR_BLUE;
		timer = GetNode<Timer>("Timer");
		countdown_timer = GetNode<Timer>("CountdownTimer");
		countdown_text = GetNode<RichTextLabel>("CountdownText");
		port_info_text = GetNode<RichTextLabel>("PortInfo");
		countdown_text.Text = "";
		info_text = GetNode<RichTextLabel>("InfoText");
		info_text.Text = "";
		end_button.Visible = false;

		serialPort = new SerialPort();
		// serialPort.PortName = "COM4";
		serialPort.BaudRate = 9600;
		// try {
		// 	serialPort.Open();
		// 	sound_button.Text = "Buzzer sound test";
		// } catch {
		using_arduino = false;
		movement_button.Disabled = true;
		sound_button.Text = "WAV sound test";
		// }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (countdown) {
			countdown_text.Text = $"[center]{Math.Round(timer.TimeLeft)}[/center]";
		} else {
			countdown_text.Text = "";
		}
		
		if (Input.IsKeyPressed(Key.Space) && !already && !countdown) {
			// stopwatch.Start();
			// stopwatch_time = stopwatch.ElapsedMilliseconds;
			// stopwatch.Reset();
			if (gamemode == 0) {
				gamemode0_end();
			} else if (gamemode == 1 || gamemode == 2) {
				gamemode1_2_end();
			}
		}

		if (gamemode == 0) {
			if (current_colour == COLOUR_RED) {
				time_passed_from_change += delta;
			}
		} else if (gamemode == 1 || gamemode == 2) {
			if (countdown_began == true) {
				time_passed_from_change += delta;
			}
		}
		
	}

	public void _on_timer_timeout() {
		if (end_button.Visible == false) {// ONLY WHEN NOT DONE
			countdown = false;
			if (gamemode == 0) {
				current_colour = colour_list[rand.Next(0, 3)];
				background.Color = current_colour;
				if (current_colour != COLOUR_RED) {
					timer.Start(TIME_BETWEEN_CHANGE);
				} else {
					// stopwatch = Stopwatch.StartNew();
					timer.Stop();
				}
				
			} else if (gamemode == 1 || gamemode == 2) {
				if (!countdown_timer_started) {
					countdown_timer.Start(rand.Next(5, 15));
					countdown_timer_started = true;	
				}
			}
		} else {
			timer.Stop();
		}
	}

	public void _on_countdown_timer_timeout() {
		if (gamemode == 1) {
			if (serialPort.IsOpen) {
				serialPort.Write("2");
			} else {
				sound_player.Play();
			}

			
			// stopwatch = Stopwatch.StartNew();
			countdown_timer.Stop();
			countdown_began = true;
		} else if (gamemode == 2) {
			if (serialPort.IsOpen) {
				serialPort.Write("1");
				// stopwatch = Stopwatch.StartNew();
				countdown_timer.Stop();
				countdown_began = true;
			}
		}
	}

	public void _on_end_button_pressed() {
		enable_buttons();

		already = false;
		countdown_timer_started = false;
		end_button.Visible = false;
		info_text.Text = "";
		gamemode = -1;

		if (using_arduino) {
			serialPort.Write("0");
		}
	}

	public void _on_light_button_pressed() {
		disable_buttons();
		gamemode = 0;

		countdown = true;
		timer.Start(TIME_BETWEEN_CHANGE);
	}

	public void _on_sound_button_pressed() {
		disable_buttons();
		gamemode = 1;

		countdown = true;
		timer.Start(TIME_BETWEEN_CHANGE);
	}

	public void _on_movement_button_pressed() {
		disable_buttons();
		gamemode = 2;

		countdown = true;
		timer.Start(TIME_BETWEEN_CHANGE);
	}

	public void disable_buttons() {
		light_button.Disabled = true;
		sound_button.Disabled = true;
		movement_button.Disabled = true;
	}

	public void enable_buttons() {
		light_button.Disabled = false;
		sound_button.Disabled = false;
		if (using_arduino) {
			movement_button.Disabled = false;
		}
	}

	public void gamemode0_end() {
		if (current_colour == COLOUR_RED) {
			// stopwatch.Stop();
			// stopwatch_time = stopwatch.ElapsedMilliseconds;
			// stopwatch.Reset();
			GD.Print("win");
			GD.Print(time_passed_from_change);
			// GD.Print($"TO remove: {stopwatch_time}");
			info_text.Text = $"[center]Reaction time: {Math.Round((time_passed_from_change + Double.Epsilon) * 1000)}ms[/center]";
		} else {
			GD.Print("lose");
			info_text.Text = "[center]This ain't red?[/center]";
			
		}
		// stopwatch.Reset();
		reset_game();
	}

	public void gamemode1_2_end() {
		if (countdown_began == true) {
			// stopwatch.Stop();
			// stopwatch_time = stopwatch.ElapsedMilliseconds;
			// stopwatch.Reset();
			GD.Print("win:");
			GD.Print(time_passed_from_change);
			// GD.Print($"TO remove: {stopwatch_time}");
			info_text.Text = $"[center]Reaction time: {Math.Round((time_passed_from_change + Double.Epsilon) * 1000)}ms[/center]";
		} else {
			GD.Print("lose");
			info_text.Text = "[center]Nope![/center]";
			
		}
		if (using_arduino) {
			serialPort.Write("0");
		}
		// stopwatch.Reset();
		reset_game();
	}

	public void reset_game() {
		already = true;
		countdown_began = false;
		end_button.Visible = true;
		background.Color = COLOUR_WHITE;
		current_colour = COLOUR_WHITE;
		time_passed_from_change = 0;
	}

	public void set_com_port() {
		if (!serialPort.IsOpen) {
			// serialPort = new SerialPort();
			if (line_edit.Text.Replace(" ", "").ToUpper() == "") {
				line_edit.QueueFree();
				set_arduino_port_input.QueueFree();
				port_info_text.QueueFree();
				using_arduino = false;
				movement_button.Disabled = true;
				sound_button.Text = "WAV sound test";
				return;
			} else {
				serialPort.PortName = line_edit.Text.Replace(" ", "").ToUpper();
				
			}
			GD.Print($"Serial: {line_edit.Text}");
			// serialPort.BaudRate = 9600;
			try {
				serialPort.Open();
				sound_button.Text = "Buzzer sound test";
				line_edit.QueueFree();
				set_arduino_port_input.QueueFree();
				port_info_text.QueueFree();
				using_arduino = true;
				movement_button.Disabled = false;
				sound_button.Text = "Buzzer sound test";
			} catch {
				using_arduino = false;
				movement_button.Disabled = true;
				sound_button.Text = "WAV sound test";
				line_edit.PlaceholderText = "Port does not exist!";
				line_edit.Text = "";
				line_edit.AddThemeColorOverride("font_placeholder_color", COLOUR_RED);
			}
		}
	}

	public void _on_line_edit_text_submitted(String _) {
		set_com_port();
	}

	public void _on_set_arduino_port_pressed() {
		set_com_port();
	}
}
