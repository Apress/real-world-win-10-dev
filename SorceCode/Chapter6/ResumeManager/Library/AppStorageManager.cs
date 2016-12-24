using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ResumeManager
{
	public enum ResumeOperationResult
	{
		Success,
		SuccessOverLimit,
		Failure,
	}
	public static class AppStorageManager
	{
		async public static Task<StorageFolder> CreateOrOpenFolder(string folder_name)
		{
			if (!string.IsNullOrWhiteSpace(folder_name))
				return await ApplicationData.Current
							.RoamingFolder
							.CreateFolderAsync(folder_name,
							CreationCollisionOption.OpenIfExists);

			return null;
		}


		async public static Task<ResumeOperationResult> SaveResumeAsync(Resume resume,
			CreationCollisionOption creation_option = CreationCollisionOption.OpenIfExists)
		{
			await CreateOrOpenFolder("resumes");
			ResumeOperationResult result = ResumeOperationResult.Success;


			if (resume.Name != null)
			{
				//write model string to file
				var file = await ApplicationData.Current
							.RoamingFolder
							.CreateFileAsync($"resumes\\{resume.ResumeID}.resume", creation_option);
				await file.WriteTextAsync(resume.AsSerializedString());

				var total_storage = await AppStorageManager.GetTotalStorage();
				if (total_storage > ApplicationData.Current.RoamingStorageQuota)
				{
					result = ResumeOperationResult.SuccessOverLimit;
				}
			}
			else
			{
				throw new Exception("Resume objects require a name.");
			}
			return result;
		}

		async public static Task<Resume> LoadResumeAsync(string resume_id)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(resume_id))
				{
					var file = await ApplicationData.Current
								.RoamingFolder
								.GetFileAsync($"resumes\\{resume_id}.resume");
					var resume_xml = await file.ReadTextAsync();
					var resume = Resume.FromSerializedString(resume_xml);
					return resume;
				}
			}
			catch (Exception ex)
			{

			}
			return null;
		}

		async public static Task<StorageFile> GetResumeFileAsync(string resume_id)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(resume_id))
				{
					var file = await ApplicationData.Current
								.RoamingFolder
								.GetFileAsync($"resumes\\{resume_id}.resume");
					return file;
				}
			}
			catch (Exception ex)
			{

			}
			return null;
		}

		async public static Task<ResumeOperationResult> DeleteResume(Resume resume)
		{
			ResumeOperationResult result = ResumeOperationResult.Success;
			try
			{
				await resume.DeleteAsync();
				var total_storage = await AppStorageManager.GetTotalStorage();
				if (total_storage > ApplicationData.Current.RoamingStorageQuota)
				{
					result = ResumeOperationResult.SuccessOverLimit;
				}
			}
			catch (Exception ex)
			{
				result = ResumeOperationResult.Failure;
			}
			return result;
		}

		async public static Task<List<Resume>> ListResumes()
		{
			var folder = await CreateOrOpenFolder("resumes");
			var files = await folder.GetFilesAsync();

			List<Resume> resumes = new List<Resume>();
			foreach (var file in files)
			{
				try
				{
					var resume = await Resume.FromStorageFileAsync(file);
					resumes.Add(resume);
				}
				catch { }
			}
			return resumes;
		}

		async public static Task ClearStorageAsync()
		{
			var folder = await CreateOrOpenFolder("resumes");
			var files = await folder.GetFilesAsync();
			foreach (var file in files)
			{
				await file.DeleteAsync();
			}
		}

		async public static Task<ulong> GetTotalStorage()
		{
			var folder = await CreateOrOpenFolder("resumes");
			var files = await folder.GetFilesAsync();
			ulong accumulator = 0;
			foreach (var file in files)
			{
				accumulator += (await file.GetBasicPropertiesAsync()).Size;
			}
			return accumulator / 1024;
		}

		async public static Task<ResumeOperationResult> CheckStorage()
		{
			ResumeOperationResult result = ResumeOperationResult.Success;
			var total_storage = await AppStorageManager.GetTotalStorage();
			if (total_storage > ApplicationData.Current.RoamingStorageQuota)
			{
				result = ResumeOperationResult.SuccessOverLimit;
			}
			return result;
		}
	}
}
