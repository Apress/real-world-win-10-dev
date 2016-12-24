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
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.ApplicationModel.VoiceCommands;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CompetingBMXApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async private void OnTextToSpeech(object sender, RoutedEventArgs e)
        {
            //initalize the synthesizer
            var speech = new SpeechSynthesizer();

            //create an audio stream of the text
            var speech_stream = await speech.SynthesizeTextToStreamAsync("The quick brown fox jumped over the lazy dog");

            //set the stream
            speech_player.SetSource(speech_stream, speech_stream.ContentType);
        }



        async private void OnSSMLSpeech(object sender, RoutedEventArgs e)
        {
            var ssml_path = "ms-appx:///speech.xml";
            var ssml_uri = new Uri(ssml_path);
            var ssml_file = await StorageFile.GetFileFromApplicationUriAsync(ssml_uri);
            var ssml_string = await ssml_file.ReadTextAsync();

            //initalize the synthesizer
            var speech = new SpeechSynthesizer();

            //create an audio stream of the text
            var speech_stream = await speech.SynthesizeSsmlToStreamAsync(ssml_string);

            //set the stream
            speech_player.SetSource(speech_stream, speech_stream.ContentType);
        }

        async private void OnRecognize(object sender, RoutedEventArgs e)
        {
            //general dictation
            var recognizer = new SpeechRecognizer();
            recognizer.UIOptions.ExampleText = "Say something, I'm giving up on you.";
            await recognizer.CompileConstraintsAsync();
            var result = await recognizer.RecognizeWithUIAsync();
            txt_dictation.Text = result.Text;
        }

        async private void OnRecognizeFromList(object sender, RoutedEventArgs e)
        {
            //if you are listening for one word this is a good thing to use
            var recognizer = new SpeechRecognizer();
            recognizer.UIOptions.ExampleText = "To test this say one, two, or three";
            var list = new SpeechRecognitionListConstraint("To test this say one, two, or three".Split(','));
            recognizer.Constraints.Add(list);
            await recognizer.CompileConstraintsAsync();
            var result = await recognizer.RecognizeWithUIAsync();
            txt_dictation.Text = result.Text;
        }

        async private void OnRecognizeFromWeb(object sender, RoutedEventArgs e)
        {
            var recognizer = new SpeechRecognizer();
            recognizer.UIOptions.ExampleText = "You can say 'Something'";
            recognizer.UIOptions.AudiblePrompt = "Say something I'm giving up on you.";
            recognizer.UIOptions.IsReadBackEnabled = false;
            recognizer.UIOptions.ShowConfirmation = false;
            var topic = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch");
            recognizer.Constraints.Add(topic);
            await recognizer.CompileConstraintsAsync();
            var result = await recognizer.RecognizeWithUIAsync();
            txt_dictation.Text = result.Text;
        }

        async private void OnRecognizeNoUI(object sender, RoutedEventArgs e)
        {
            var recognizer = new SpeechRecognizer();
            
            var topic = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch");
            recognizer.Constraints.Add(topic);
            await recognizer.CompileConstraintsAsync();
            var result = await recognizer.RecognizeAsync();
            txt_dictation.Text = result.Text;
        }

        private void OnRecognizeVoiceCommand(object sender, RoutedEventArgs e)
        {
            var recognizer = new SpeechRecognizer();
            recognizer.UIOptions.ExampleText = "You can say 'Something'";
            recognizer.UIOptions.AudiblePrompt = "Say something I'm giving up on you.";
            recognizer.UIOptions.IsReadBackEnabled = true;
            recognizer.UIOptions.ShowConfirmation = true;
        }

        async private void OnContinuousRecognize(object sender, RoutedEventArgs e)
        {
            var recognizer = new SpeechRecognizer();
            await recognizer.CompileConstraintsAsync();
            recognizer.ContinuousRecognitionSession.ResultGenerated += async (s, args) =>
            {
                await txt_dictation.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        txt_dictation.Text += args.Result.Text;
                    });
            };
            await recognizer.ContinuousRecognitionSession.StartAsync(SpeechContinuousRecognitionMode.Default);
        }

        async private void OnVoiceCommand(object sender, RoutedEventArgs e)
        {
            var vcd_path = "ms-appx:///voicecommands.xml";
            var vcd_uri = new Uri(vcd_path);
            var vcd_file = await StorageFile.GetFileFromApplicationUriAsync(vcd_uri);
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcd_file);
            int x = 0;
            x++;

            ////modifying phrase list programmatically (for more dynamic interactions)
            //VoiceCommandDefinition command_set;
            //if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue("en-us-CommandSet", out command_set))
            //{

            //    await command_set.SetPhraseListAsync("location", new string[] {"London", "Dallas", "New York", "Phoenix"});
            //}

        }

        async private void OnServiceThroughCortana(object sender, RoutedEventArgs e)
        {
            var vcd_path = "ms-appx:///voicecommands_advanced.xml";
            var vcd_uri = new Uri(vcd_path);
            var vcd_file = await StorageFile.GetFileFromApplicationUriAsync(vcd_uri);
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcd_file);
        }
    }
}
