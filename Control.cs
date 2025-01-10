using Godot;
using System;

public partial class Control : Godot.Control
{
	LineEdit inputPatch; Button folderBrowser;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		inputPatch = GetNode<LineEdit>("./inputPatch");
		folderBrowser = GetNode<Button>("./folderBrowser");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_button_pressed()
	{
		Console.WriteLine("Oke");
	}
}
