using Godot;
using System;
using System.IO;
using System.Linq;

public partial class Control : Godot.Control
{
	LineEdit pathFolder; Button chooseFolder; FileDialog FileDialog; Label statusNumber;

	string configure = @"module/configure.json";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFolder = GetNode<LineEdit>("./pathFolder");
		chooseFolder = GetNode<Button>("./chooseFolder");
		FileDialog = GetNode<FileDialog>("./FileDialog");
		statusNumber = GetNode<Label>("./statusNumber");

		ConfigureFile content = JsonHelper.ReadJson<ConfigureFile>(configure);
		if (content.pathFolder != null)
		{
			pathFolder.Text = ShortenPath(content.pathFolder);
			int fileCount = CountFilesInFolder(content.pathFolder);
			statusNumber.Text = fileCount.ToString();
		}
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
		ConfigureFile content = JsonHelper.ReadJson<ConfigureFile>(configure);
		content.pathFolder = path;
		JsonHelper.WriteJson(configure, content);
		pathFolder.Text = ShortenPath(path);
		int fileCount = CountFilesInFolder(path);
		statusNumber.Text = fileCount.ToString();

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

	static int CountFilesInFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException("The specified folder does not exist.");
        }
        // Define valid image extensions
        string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };

        // Get all files in the folder and filter by image extensions
        string[] imageFiles = Directory.GetFiles(folderPath)
                                        .Where(file => validExtensions.Contains(Path.GetExtension(file).ToLower()))
                                        .ToArray();

        // Return the count of image files
        return imageFiles.Length;
    }

}



