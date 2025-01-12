using Godot;
using System;

public partial class Control : Godot.Control
{
	LineEdit pathFolder; Button chooseFolder;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFolder = GetNode<LineEdit>("./pathFolder");
		chooseFolder = GetNode<Button>("./chooseFolder");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_button_pressed()
	{
		

	}
}
