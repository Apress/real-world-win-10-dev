using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BigMountainX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmployeeSignIn : Page
    {
        List<dynamic> passphrases = new List<dynamic>
    {
            new {Phrase= "the quick brown fox jumped over the lazy dogs",File="phrase1.xml" },
            new { Phrase="it's not you it's me",File="phrase2.xml" },
            new  { Phrase="be the spoon",File="phrase3.xml" },
            new  { Phrase="don't worry be happy",File="phrase4.xml" }
    };

        string _passphrase;
        Type _target_type;
        public EmployeeSignIn()
        {
            this.InitializeComponent();
        }

        async private void OnSignInRequested(object sender, RoutedEventArgs e)
        {
            bool failed_signin = true;
            if (!string.IsNullOrWhiteSpace(_passphrase)
                && !string.IsNullOrWhiteSpace(txt_passphrase.Text))
            {
                if (txt_employeecode.Password == "employee"
                    && txt_passphrase.Text.ToLower() == _passphrase.ToLower())
                {
                    failed_signin = false;
                    Frame.Navigate(_target_type);
                }
            }

            if (failed_signin)
            {
                await new MessageDialog("Login Failed").ShowAsync();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _target_type = e.Parameter as Type;
            base.OnNavigatedTo(e);
        }

        async private void OnPlayPassphrase(object sender, RoutedEventArgs e)
        {
            //select a random item
            Random random = new Random();
            int seed_value = random.Next(0, 1000);
            int selection = seed_value % 4;
            var tuple = passphrases[selection];

            _passphrase = tuple.Phrase;

            var ssml_path = $"ms-appx:///media/audio/{tuple.File}";
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

        async private void OnSayPrompt(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                button.Content = "...";
                bool failed_signin = true;
                Random random = new Random();
                int seed_value = random.Next(0, 1000);
                int selection = seed_value % 4;
                var tuple = passphrases[selection];

                var prompt = (string)tuple.Phrase;
                //general dictation
                var recognizer = new SpeechRecognizer();
                var phrases = passphrases.Select(i => (string)i.Phrase).ToList();

                var list_constraint = new SpeechRecognitionListConstraint(phrases);
                recognizer.Constraints.Add(list_constraint);

                recognizer.StateChanged += Recognizer_StateChanged;
                recognizer.UIOptions.ExampleText = $"Repeat the phrase '{prompt}'";
                await recognizer.CompileConstraintsAsync();
                var result = await recognizer.RecognizeWithUIAsync();
                if (result.Status == SpeechRecognitionResultStatus.Success)
                {
                    if (result.Text.ToLower() == prompt.ToLower())
                    {
                        if (txt_employeecode2.Password == "employee")
                        {
                            failed_signin = false;
                            Frame.Navigate(_target_type);
                        }
                    }

                }

                if (failed_signin)
                {
                    button.Content = "Failed connection";
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void Recognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {

        }
    }
}
