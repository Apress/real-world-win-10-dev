using BigMountainX;
using General.UWP.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using static Windows.Storage.NameCollisionOption;

namespace BMX.BackgroundTasks
{
    sealed public class CortanaAppService : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        AppState State { get; set; }

        static int count = 0;

        async public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var local = ApplicationData.Current.LocalFolder;
            State = await AppState.FromStorageFileAsync(local, "state.xml");

            var app_service = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            var connection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(app_service);

            //get the voice command
            var command = await connection.GetVoiceCommandAsync();
            var rule_path = command.SpeechRecognitionResult.RulePath[0];

            VoiceCommandUserMessage user_message = new VoiceCommandUserMessage();
            if (rule_path == "change-desktop-wallpaper")
            {
                await HandleChangeWallpaper(connection, user_message);
            }
            else if (rule_path == "make-suggestions")
            {
                await HandleMakeSuggestions(connection, command, user_message);
            }
            else if (rule_path == "buy-tickets")
            {
                await HandleBuyTickets(connection, command, user_message);

            }
            else if (rule_path == "count")
            {
                count++;
                user_message.SpokenMessage = $"the current count is {count}";
                user_message.DisplayMessage = user_message.SpokenMessage;
                var response = VoiceCommandResponse.CreateResponse(user_message);
                await connection.ReportSuccessAsync(response);
            }
            else if (rule_path == "joke-knock-knock")
            {
                await HandleTellJoke(connection, user_message);
            }
            else if (rule_path == "joke-whos-there")
            {
                await HandleWhosThere(connection, user_message);
            }
            else if (rule_path == "joke-punchline")
            {
                await HandlePunchline(connection, command, user_message);
            }

            _deferral.Complete();
        }

        async Task HandlePunchline(VoiceCommandServiceConnection connection, VoiceCommand command, VoiceCommandUserMessage user_message)
        {
            var local_folder = ApplicationData.Current.LocalFolder;
            var file = await local_folder.CreateFileAsync("joke_state.txt", CreationCollisionOption.OpenIfExists);
            var text = await file.ReadTextAsync();
            if (text != "joke")
            {
                user_message.SpokenMessage = "first ask me to tell you a joke";
                var response = VoiceCommandResponse.CreateResponse(user_message);
                await connection.ReportSuccessAsync(response);
                return;
            }
            var name = command.SpeechRecognitionResult.SemanticInterpretation.Properties["joke_name"][0].ToLower();
            switch (name)
            {
                case "amos":
                    {
                        user_message.SpokenMessage = "a mosquito";
                        user_message.DisplayMessage = user_message.SpokenMessage;
                        var response = VoiceCommandResponse.CreateResponse(user_message);
                        await connection.ReportSuccessAsync(response);
                    }
                    break;
                case "who":
                    {
                        user_message.SpokenMessage = "that's what an owl says";
                        user_message.DisplayMessage = user_message.SpokenMessage;
                        var response = VoiceCommandResponse.CreateResponse(user_message);
                        await connection.ReportSuccessAsync(response);
                    }
                    break;
                case "honey bee":
                    {
                        user_message.SpokenMessage = "honey bee a dear and get me some juice";
                        user_message.DisplayMessage = user_message.SpokenMessage;
                        var response = VoiceCommandResponse.CreateResponse(user_message);
                        await connection.ReportSuccessAsync(response);
                    }
                    break;
                case "lettuce":
                    {
                        user_message.SpokenMessage = "lettuce in it's cold out here";
                        user_message.DisplayMessage = user_message.SpokenMessage;
                        var response = VoiceCommandResponse.CreateResponse(user_message);
                        await connection.ReportSuccessAsync(response);
                    }
                    break;
                case "double":
                    {
                        user_message.SpokenMessage = "double u";
                        user_message.DisplayMessage = user_message.SpokenMessage;
                        var response = VoiceCommandResponse.CreateResponse(user_message);
                        await connection.ReportSuccessAsync(response);
                    }
                    break;
            }

            //the joke has now been told, reset
            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        private static async Task HandleWhosThere(VoiceCommandServiceConnection connection, VoiceCommandUserMessage user_message)
        {
            var local_folder = ApplicationData.Current.LocalFolder;
            var file = await local_folder.CreateFileAsync("joke_state.txt", CreationCollisionOption.OpenIfExists);
            var text = await file.ReadTextAsync();
            if (text == "knock knock")
            {
                await file.WriteTextAsync("joke");
                Random random = new Random();
                var next = random.Next(100);
                var index = next % 5;

                var jokes = new string[] {
                "amos",
                "who",
                "honey bee",
                "lettuce",
                "double"
            };

                var joke = jokes[index];

                user_message.SpokenMessage = joke;
                user_message.DisplayMessage = user_message.SpokenMessage;
                var response = VoiceCommandResponse.CreateResponse(user_message);
                await connection.ReportSuccessAsync(response);
            }
            else
            {
                user_message.SpokenMessage = "first ask me to tell you a joke";
                var response = VoiceCommandResponse.CreateResponse(user_message);
                await connection.ReportSuccessAsync(response);
            }
        }

        private static async Task HandleTellJoke(VoiceCommandServiceConnection connection, VoiceCommandUserMessage user_message)
        {
            var local_folder = ApplicationData.Current.LocalFolder;
            var file = await local_folder.CreateFileAsync("joke_state.txt", CreationCollisionOption.ReplaceExisting);
            await file.WriteTextAsync("knock knock");

            user_message.SpokenMessage = "knock knock";
            user_message.DisplayMessage = "knock knock!";
            var response = VoiceCommandResponse.CreateResponse(user_message);
            await connection.ReportSuccessAsync(response);
        }

        private async Task HandleChangeWallpaper(VoiceCommandServiceConnection connection, VoiceCommandUserMessage user_message)
        {
            //copy images to appdata
            var local_folder = ApplicationData.Current.LocalFolder;
            var install_path = Package.Current.InstalledLocation;
            var media_path = await install_path.GetFolderAsync("media\\images");
            var images = await media_path.GetFilesAsync();
            foreach (var image in images)
            {
                try
                {
                    await local_folder.GetFileAsync(image.Name);
                    continue;
                }
                catch { }
                await image.CopyAsync(local_folder, image.Name, ReplaceExisting);
            }

            //change wallpaper and prepare response back to user

            var result = await UserSettings.ChangeWallpaperAsync();
            user_message.SpokenMessage = "Your wallpaper was modified, do you want me to change the lock screen as well?";

            var backup_message = new VoiceCommandUserMessage
            {
                SpokenMessage = "Change your lock screen",
            };

            var response = VoiceCommandResponse.CreateResponseForPrompt(user_message, backup_message);
            var confirm_result = await connection.RequestConfirmationAsync(response);

            if (confirm_result.Confirmed)
            {
                await UserSettings.ChangeLockScreenAsync();
                user_message.SpokenMessage = "Your lock screen was also modified.";
                response = VoiceCommandResponse.CreateResponse(user_message);
                await connection.ReportSuccessAsync(response);
            }
            else
            {
                user_message.SpokenMessage = "okay, you're all set then";
                response = VoiceCommandResponse.CreateResponse(user_message);
                await connection.ReportSuccessAsync(response);
            }
        }

private async Task HandleBuyTickets(VoiceCommandServiceConnection connection, VoiceCommand command, VoiceCommandUserMessage user_message)
{
    var location = command.SpeechRecognitionResult.SemanticInterpretation.Properties["location"][0].ToLower();
    var event_type = command.SpeechRecognitionResult.SemanticInterpretation.Properties["event_type"][0].ToLower();

    var events = State.Events;
    var target_event = (from evt in events
                        where evt.Address.ToLower().Contains(location)
                        select evt).FirstOrDefault();
    user_message.SpokenMessage = $"I got your tickets to {target_event.EventTitle}";
    var response = VoiceCommandResponse.CreateResponse(user_message);
    await connection.ReportSuccessAsync(response);
}

        private async Task HandleMakeSuggestions(VoiceCommandServiceConnection connection, VoiceCommand command, VoiceCommandUserMessage user_message)
        {
            var tiles = new List<VoiceCommandContentTile>();
            var location = command.SpeechRecognitionResult.SemanticInterpretation.Properties["location"][0].ToLower();

            //find events
            var events = State.Events;
            events = (from evt in events
                      where evt.Address.ToLower().Contains(location)
                      select evt).ToList();

            //create tiles
            foreach (var evt in events)
            {
                tiles.Add(new VoiceCommandContentTile
                {
                    ContentTileType = VoiceCommandContentTileType.TitleWithText,
                    AppLaunchArgument = $"event,event_id={evt.EventID}",
                    Title = evt.EventTitle,
                    TextLine1 = evt.Description,
                });
            }

            //respond
            var response = VoiceCommandResponse.CreateResponse(user_message, tiles);

            response.AppLaunchArgument = "all_events";

            await connection.ReportSuccessAsync(response);
        }
    }
}
