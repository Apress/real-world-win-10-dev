using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;
using static Windows.Storage.CreationCollisionOption;
using profile = Windows.System.UserProfile.UserProfilePersonalizationSettings;

namespace General.UWP.Library
{
    public static class UserSettings
    {
        async public static Task<uint> ChangeWallpaperAsync()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                //open file
                var local_folder = ApplicationData.Current.LocalFolder;
                var file = await local_folder.CreateFileAsync("marker.txt", OpenIfExists);
                string value = value = await file.ReadTextAsync();
                uint current_index = 1;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    current_index = uint.Parse(value);
                }
                
                //read and convert value of file
               
                

                //get file for index
                var image_path = $"ms-appdata:///local/club{current_index}.jpg";
                var image_uri = new Uri(image_path);
                var image_file = await StorageFile.GetFileFromApplicationUriAsync(image_uri);

                bool was_set = false;
                try
                {
                    //display image file
                    was_set = await profile.Current.TrySetWallpaperImageAsync(image_file);
                }
                catch (Exception ex)
                {
                    was_set = false;
                }

                if (was_set)
                {
                    //increment index and store in file
                    current_index++;
                    if (current_index > 7)
                        current_index = 1;

                    await file.WriteTextAsync(current_index.ToString());
                }
                return current_index;
            }
            return 0;
        }

        async public static Task<uint> ChangeLockScreenAsync()
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                //open file
                var local_folder = ApplicationData.Current.LocalFolder;
                var file = await local_folder.CreateFileAsync("marker_lock.txt", OpenIfExists);
                //read and convert value of file
                var value = await file.ReadTextAsync();
                uint current_index = 1;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    current_index = uint.Parse(value);
                }

                //get file for index
                var image_path = $"ms-appdata:///local/club{current_index}.jpg";
                var image_uri = new Uri(image_path);
                var image_file = await StorageFile.GetFileFromApplicationUriAsync(image_uri);

                bool was_set = false;
                try
                {
                    //display image file
                    was_set = await profile.Current.TrySetLockScreenImageAsync(image_file);
                }
                catch (Exception ex)
                {
                    was_set = false;
                }

                if (was_set)
                {
                    //increment index and store in file
                    current_index++;
                    if (current_index > 7)
                        current_index = 1;

                    await file.WriteTextAsync(current_index.ToString());
                }
                return current_index;
            }
            return 0;
        }
    }
}
