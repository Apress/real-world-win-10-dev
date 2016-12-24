using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.ComponentModel;
using Windows.UI.ViewManagement;
using Windows.Storage.Pickers;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.Threading.Tasks;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ResumeManager
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LandingPage : Page, INotifyPropertyChanged
	{
		StorageFolder _library_folder;
		int _count = 0;
		List<Resume> Resumes { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public LandingPage()
		{
			this.InitializeComponent();
			this.Loaded += LandingPage_Loaded;
		}

		async private void LandingPage_Loaded(object sender, RoutedEventArgs e)
		{
			_count = (await AppStorageManager.ListResumes()).Count;
			await RefreshUIAsync();
		}

		async Task RefreshUIAsync(ResumeOperationResult? result = null)
		{
			var app_view = ApplicationView.GetForCurrentView();
			var resume_list = await AppStorageManager.ListResumes();
			app_view.Title = $"{resume_list.Count} resumes, {(await AppStorageManager.GetTotalStorage())} KB, QUOTA={ApplicationData.Current.RoamingStorageQuota} KB";
			Resumes = resume_list;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Resumes"));

			if (result != null)
			{
				if (result == ResumeOperationResult.SuccessOverLimit)
				{
					app_view.TitleBar.BackgroundColor = Windows.UI.Colors.Red;
				}
				else
					app_view.TitleBar.BackgroundColor = null;
			}
		}

		async private void AddResume(object sender, RoutedEventArgs e)
		{
			var result = await AppStorageManager.
				SaveResumeAsync(new Resume() { Name = $"Resume {_count}" });
			_count++;

			await RefreshUIAsync(result);
		}


		async private void RemoveResume(object sender, RoutedEventArgs e)
		{
			var resume_list = await AppStorageManager.ListResumes();
			var oldest_resume = resume_list.LastOrDefault();

			var result = await AppStorageManager.
				DeleteResume(oldest_resume);
			_count--;
			await RefreshUIAsync(result);
		}



		async private void ExportResume(object sender, RoutedEventArgs e)
		{
			var app_view = ApplicationView.GetForCurrentView();
			var selected_resume = list_resumes.SelectedItem as Resume;
			if (selected_resume != null)
			{
				var resume = selected_resume;

				FileSavePicker saver = new FileSavePicker();
				saver.DefaultFileExtension = ".resume";
				saver.SuggestedFileName = "my file";
				saver.FileTypeChoices.Add("ResumeManager File", new List<string>() { ".resume" });

				var new_file = await saver.PickSaveFileAsync();
				if (new_file != null)
				{
					var new_resume_id = Guid.NewGuid().ToString();
					resume.Name = $"resume import [{new_resume_id}]";
					resume.ResumeID = new_resume_id;
					await new_file.WriteTextAsync(resume.AsSerializedString());
					await RefreshUIAsync();
				}
			}
			else
			{
				await new MessageDialog("Select a resume first").ShowAsync();
			}
		}

		async private void ImportResume(object sender, RoutedEventArgs e)
		{
			var app_view = ApplicationView.GetForCurrentView();

			FileOpenPicker opener = new FileOpenPicker();
			opener.ViewMode = PickerViewMode.Thumbnail;
			opener.SuggestedStartLocation = PickerLocationId.Desktop;
			opener.CommitButtonText = "Import the resume";
			opener.FileTypeFilter.Add(".resume");
			var selected_file = await opener.PickSingleFileAsync();
			if (selected_file != null)
			{
				//read and deserialize resume
				var resume = await Resume.FromStorageFileAsync(selected_file);

				if (resume == null)
				{
					MessageDialog md = new MessageDialog("Cannot read resume data.");
					await md.ShowAsync();
					return;
				}

				//name resume
				var new_resume_id = Guid.NewGuid().ToString();
				int current_storage_count = (await AppStorageManager.ListResumes()).Count;
				resume.Name = $"resume import [{new_resume_id}]";
				resume.ResumeID = new_resume_id;


				//now store the resume in AppData
				var result = await AppStorageManager.
				SaveResumeAsync(resume, CreationCollisionOption.GenerateUniqueName);
				await RefreshUIAsync(result);
			}
		}

		async private void SelectLibrary(object sender, RoutedEventArgs e)
		{
			FolderPicker picker = new FolderPicker();
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = PickerLocationId.Desktop;
			picker.CommitButtonText = "Select this location";
			picker.FileTypeFilter.Add(".resume");
			_library_folder = await picker.PickSingleFolderAsync();
			if (_library_folder != null)
			{
				Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(_library_folder);
			}
		}

		async private void ClearLibrary(object sender, RoutedEventArgs e)
		{
			await AppStorageManager.ClearStorageAsync();
			await RefreshUIAsync();
		}
	}
}