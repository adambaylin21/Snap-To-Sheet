using Godot;
using System;

public partial class Control : Godot.Control
{
	LineEdit pathFolder; Button chooseFolder; FileDialog FileDialog;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFolder = GetNode<LineEdit>("./pathFolder");
		chooseFolder = GetNode<Button>("./chooseFolder");
		FileDialog = GetNode<FileDialog>("./FileDialog");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_button_pressed()
	{
		FileDialog.Popup();
	}
	private void _on_file_dialog_dir_selected(string path)
	{
		pathFolder.Text = ShortenPath(path);
	}
	private string ShortenPath(string path, int maxLength = 30)
	{
		if (path.Length <= maxLength)
		{
			return path;
		}

		int charsToShow = maxLength / 2;
		string start = path.Substring(0, charsToShow);
		string end = path.Substring(path.Length - charsToShow);

		return $"{start}...{end}";
	}

}



