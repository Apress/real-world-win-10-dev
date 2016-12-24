using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace General.UWP.Library
{
	public class StateAwareObject<T> where T : StateAwareObject<T>, new()
	{
		StorageFolder _folder;
		StorageFile _file;
		string _file_name;

		public string AsSerializedString()
		{
			DataContractSerializer dcs = new DataContractSerializer(typeof(T));
			MemoryStream ms = new MemoryStream();
			dcs.WriteObject(ms, this);
			ms.Seek(0, SeekOrigin.Begin);
			byte[] buffer = new byte[ms.Length];
			ms.Read(buffer, 0, buffer.Length);
			string serialized_object = Encoding.ASCII.GetString(buffer);
			return serialized_object;
		}



		async public Task SaveAsync()
		{
			StorageFile file = _file;
			if (file == null)
			{
				file = await _folder.CreateFileAsync(_file_name, CreationCollisionOption.ReplaceExisting);
			}

			var serialized = this.AsSerializedString();
			await file.WriteTextAsync(serialized);
		}

		async public Task DeleteAsync()
		{
			StorageFile file = _file;
			if (file != null)
			{
				await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
			}
		}

		public static T FromSerializedString(string resume_string)
		{
			try
			{
				var buffer = Encoding.ASCII.GetBytes(resume_string);
				MemoryStream ms = new MemoryStream(buffer);
				DataContractSerializer dcs = new DataContractSerializer(typeof(T));
				var retval = dcs.ReadObject(ms);
				var app_state = retval as T;
				return app_state;
			}
			catch
			{
				return null;
			}
		}

		async public static Task<T> FromStorageFileAsync(StorageFile file)
		{
			var app_state = FromSerializedString(await file.ReadTextAsync());
			if (app_state == null)
				app_state = new T();
			app_state._folder = await file.GetParentAsync();
			app_state._file = file;
			app_state._file_name = file.Name;
			return app_state;
		}

		async public static Task<T> FromStorageFileAsync(StorageFolder folder, string file_name)
		{
			var file = (await folder.TryGetItemAsync(file_name)) as StorageFile;
			if (file == null)
			{
				var retval = new T();
				retval._folder = folder;
				retval._file_name = file_name;
				return retval;
			}
			else
				return await FromStorageFileAsync(file as StorageFile);
		}
	}
}
