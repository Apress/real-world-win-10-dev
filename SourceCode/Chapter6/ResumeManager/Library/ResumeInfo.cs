using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ResumeManager
{
	public class WorkExperience
	{
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Company { get; set; }
		public string Title { get; set; }
		public List<Technology> TechnologiesUsed { get; set; }
	}

	public class Technology
	{
		public string Name { get; set; }
	}

	public class Certification
	{
		public string CertificationName { get; set; }

		public DateTime AcquisitionDate { get; set; }

		public string TechSponsor { get; set; }
	}

	public class Education
	{
		public string SchoolName { get; set; }
		public string SchoolAddress { get; set; }
		public string Degree { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string EducationLevel { get; set; }
	}

	public class Resume
	{
		public string ResumeID { get; set; } = Guid.NewGuid().ToString();
		public string Name { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime LastUpdateDate { get; set; }

		public string EmailAddress { get; set; }
		public string PhoneNumber { get; set; }
		public string ProfileSummary { get; set; }

		StorageFile File { get; set; }

		public List<Certification> Certifications { get; set; }

		public List<Education> EductionHistory { get; set; }
		public List<WorkExperience> WorkHistory { get; set; }

		public StorageFile GetFile()
		{
			return File;
		}

		async public Task DeleteAsync()
		{
			await File.DeleteAsync(StorageDeleteOption.PermanentDelete);
		}

		public string AsSerializedString()
		{
			DataContractSerializer dcs = new DataContractSerializer(typeof(Resume));
			MemoryStream ms = new MemoryStream();
			dcs.WriteObject(ms, this);
			ms.Seek(0, SeekOrigin.Begin);
			byte[] buffer = new byte[ms.Length];
			ms.Read(buffer, 0, buffer.Length);
			string serialized_object = Encoding.ASCII.GetString(buffer);
			return serialized_object;
		}

		public static Resume FromSerializedString(string resume_string)
		{
			try
			{
				var buffer = Encoding.ASCII.GetBytes(resume_string);
				MemoryStream ms = new MemoryStream(buffer);
				DataContractSerializer dcs = new DataContractSerializer(typeof(Resume));
				var retval = dcs.ReadObject(ms);
				var resume = retval as Resume;
				return resume;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		async public static Task<Resume> FromStorageFileAsync(StorageFile file)
		{
			var resume = FromSerializedString(await file.ReadTextAsync());
			resume.File = file;
			return resume;
		}
	}


}
