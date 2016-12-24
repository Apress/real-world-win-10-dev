using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ResumeManager
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ResumeFilePicker : Page, INotifyPropertyChanged
	{
		FileOpenPickerUI _open_basket;
		FileSavePickerUI _save_basket;
		bool _saving;
		string _previous_id = null;
		public event PropertyChangedEventHandler PropertyChanged;

		List<Resume> Resumes { get; set; }

		public ResumeFilePicker()
		{
			this.InitializeComponent();
			this.Loaded += ResumeFilePicker_Loaded;

		}

		async private void ResumeFilePicker_Loaded(object sender, RoutedEventArgs e)
		{
			Resumes = await AppStorageManager.ListResumes();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Resumes"));
		}

		public void Activate(FileOpenPickerActivatedEventArgs args)
		{
			_saving = false;
			_open_basket = args.FileOpenPickerUI;
			Window.Current.Content = this;
			Window.Current.Activate();
		}


		public void Activate(FileSavePickerActivatedEventArgs args)
		{
			_saving = true;
			_save_basket = args.FileSavePickerUI;
			_save_basket.TargetFileRequested += _save_basket_TargetFileRequested;
			Window.Current.Content = this;
			Window.Current.Activate();
		}

		async void _save_basket_TargetFileRequested(FileSavePickerUI sender, TargetFileRequestedEventArgs args)
		{
			var deferral = args.Request.GetDeferral();
			args.Request.TargetFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync($"resumes\\{_save_basket.FileName}", CreationCollisionOption.GenerateUniqueName);
			deferral.Complete();
		}


		async private void ResumeListSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_saving)
			{
				var grid_view = sender as ListView;
				var resume = grid_view.SelectedItem as Resume;
				var resume_file = resume?.GetFile();
				if (resume_file != null)
				{
					if (_previous_id != null)
						_open_basket.RemoveFile(_previous_id);

					_open_basket.AddFile(resume.ResumeID, resume_file);
					_open_basket.Title = $"{resume.Name} selected";
					_previous_id = resume.ResumeID;

				}
				else
					await new MessageDialog("Resume is null").ShowAsync();
			}

		}

	}
}
