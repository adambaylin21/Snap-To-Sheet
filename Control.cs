using Godot;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

public partial class Control : Godot.Control
{
	LineEdit pathFolder; Button chooseFolder; FileDialog FileDialog; Label statusNumber;
	LineEdit apiLink; SpinBox SpinBox; Label status; TextEdit logOutput;

	string configure = @"module/configure.json";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFolder = GetNode<LineEdit>("./pathFolder");
		apiLink = GetNode<LineEdit>("./apiLink");
		chooseFolder = GetNode<Button>("./chooseFolder");
		FileDialog = GetNode<FileDialog>("./FileDialog");
		statusNumber = GetNode<Label>("./statusNumber");
		status = GetNode<Label>("./status");
		SpinBox = GetNode<SpinBox>("./SpinBox");
		logOutput = GetNode<TextEdit>("./logOutput");

		ConfigureFile content = JsonHelper.ReadJson<ConfigureFile>(configure);
		if (content.pathFolder != null)
		{
			pathFolder.Text = ShortenPath(content.pathFolder);
			string[] files = GetFilesInFolder(content.pathFolder);
			int fileCount = files.Length;
			statusNumber.Text = fileCount.ToString();
		}
		if (content.apiLink != null)
		{
			apiLink.Text = content.apiLink;
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

	private void _on_api_link_text_changed(string newLink)
	{
		ConfigureFile content = JsonHelper.ReadJson<ConfigureFile>(configure);
		content.apiLink = newLink;
		JsonHelper.WriteJson(configure, content);
	}

	private async void _on_start_snap_pressed()
	{
		try
		{
			ConfigureFile content = JsonHelper.ReadJson<ConfigureFile>(configure);
			if (content.pathFolder == "" || content.apiLink == "")
			{
				status.Text = "Please select folder and API link";
				// status.Modulate = new Color (0.9f,0.2f,0,1);
				return;
			}
			status.Text = "";
			string[] files = GetFilesInFolder(content.pathFolder);
			
			int delay = (int)SpinBox.Value;
			int delayBetweenUploads = delay * 1000;
			string url = content.apiLink;
		
			List<string> results = await SnapFile(files, delayBetweenUploads, url);

			foreach (var result in results)
			{
				logOutput.Text += result + "\n";
			}

		}
		catch (Exception ex)
		{
			status.Text = $"Lỗi: {ex.Message}";
		}
	}

	private void _on_folder_selected(string path)
	{
		ConfigureFile content = JsonHelper.ReadJson<ConfigureFile>(configure);
		content.pathFolder = path;
		JsonHelper.WriteJson(configure, content);
		pathFolder.Text = ShortenPath(path);
		string[] files = GetFilesInFolder(path);
		int fileCount = files.Length;
		statusNumber.Text = fileCount.ToString();

	}

	private string ShortenPath(string path, int maxLength = 45)
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

	static string[] GetFilesInFolder(string folderPath)
	{
		if (!Directory.Exists(folderPath))
		{
			throw new DirectoryNotFoundException("The specified folder does not exist.");
		}

		// Định nghĩa các phần mở rộng file hợp lệ
		string[] validExtensions = { ".jpg", ".jpeg", ".png" };

		// Lấy tất cả file trong thư mục và lọc theo phần mở rộng
		string[] files = Directory.GetFiles(folderPath)
								.Where(file => validExtensions.Contains(Path.GetExtension(file).ToLower()))
								.ToArray();

		// Trả về danh sách file
		return files;
	}

	static async Task<List<string>> SnapFile(string[] files, int delayBetweenUploads, string url)
	{
		var results = new List<string>();

		foreach (var file in files)
		{
			try
			{
				// Thực hiện upload ảnh
				string result = await UploadImageAsync(file, url);

				// Lấy 5 ký tự cuối cùng của tên file
				string namefile = file.Length >= 15 ? file.Substring(file.Length - 15) : file;

				// Thêm kết quả vào danh sách
				results.Add($"{namefile} | {result}");

				// Console.WriteLine($"Upload {file}: {result}");
			}
			catch (Exception ex)
			{
				results.Add($"Error {file}: {ex.Message}");
				Console.WriteLine($"Error {file}: {ex.Message}");
			}

			// Nghỉ một khoảng thời gian trước khi upload file tiếp theo
			await Task.Delay(delayBetweenUploads);
		}

		return results;
	}

	static async Task<string> UploadImageAsync(string filePath, string url)
	{
		using (var httpClient = new System.Net.Http.HttpClient())
		using (var content = new MultipartFormDataContent())
		{
			try
			{
				using (var fileStream = File.OpenRead(filePath))
				{
					string fileExtension = Path.GetExtension(filePath).ToLower();
					string mediaType = fileExtension switch
					{
						".jpg" or ".jpeg" => "image/jpeg",
						".png" => "image/png",
						".gif" => "image/gif",
						_ => throw new NotSupportedException("Unsupported format")
					};

					var streamContent = new StreamContent(fileStream);
					streamContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
					content.Add(streamContent, "file", Path.GetFileName(filePath));

					var response = await httpClient.PostAsync(url, content);
					if (response.IsSuccessStatusCode)
					{
						return await response.Content.ReadAsStringAsync();
					}
					else
					{
						return $"Error: {response.StatusCode}";
					}
				}
			}
			catch (Exception ex)
			{
				return $"Upload error: {ex.Message}";
			}
		}
	}



}
