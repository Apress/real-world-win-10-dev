using General.UWP.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using MediaButton = Windows.Media.SystemMediaTransportControlsButton;
using ButtonArgs = Windows.Media.SystemMediaTransportControlsButtonPressedEventArgs;
using ReceivedArgs = Windows.Media.Playback.MediaPlayerDataReceivedEventArgs;

namespace BMX.BackgroundTasks
{
    sealed public class AudioPlayback : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral; // Used to keep task alive
        MediaPlayer _player;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            //declare controls for background task handling
            _player = BackgroundMediaPlayer.Current;
            _player.SystemMediaTransportControls.IsPreviousEnabled = true;
            _player.SystemMediaTransportControls.IsNextEnabled = true;

            //handle events
            BackgroundMediaPlayer.MessageReceivedFromForeground += ForegroundMessageReceived;
            _player.SystemMediaTransportControls.ButtonPressed += MediaButtonPressed;

            taskInstance.Task.Completed += (s, args) => _deferral.Complete();
        }

        async private void ForegroundMessageReceived(object sender, ReceivedArgs e)
        {
            if (e.Data.ContainsKey("action"))
            {
                var button_string = e.Data["action"] as string;
                var button_type = typeof(MediaButton);
                var button = (MediaButton)Enum.Parse(button_type, button_string);
                await Play(button);
            }
        }

        async private void MediaButtonPressed(SystemMediaTransportControls sender, ButtonArgs args)
        {
            await Play(args.Button);
        }

        async Task Play(MediaButton button)
        {
            switch (button)
            {
                case MediaButton.Play:
                    _player.Play();
                    break;
                case MediaButton.Pause:
                    _player.Pause();
                    break;
                case MediaButton.Previous:
                    {
                        SendMessage("action", "stop");
                        var file_path = "ms-appx:///media/video/zaharat.mp4";
                        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(file_path));
                        _player.SetFileSource(file);
                    }
                    break;
                case MediaButton.Next:
                    {
                        SendMessage("action", "stop");
                        var file_path = "ms-appx:///media/video/black.mp4";
                        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(file_path));
                        _player.SetFileSource(file);
                    }
                    break;
            }
        }

        void SendMessage(string key, string value)
        {
            BackgroundMediaPlayer.SendMessageToForeground(new ValueSet
            {
                { key,value}
            });
        }
    }
}
