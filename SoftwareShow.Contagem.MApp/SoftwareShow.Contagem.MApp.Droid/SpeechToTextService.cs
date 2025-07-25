using Android.App;
using Android.Content;
using Android.Speech;
using SoftwareShow.Contagem.MApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp
{
    public class SpeechToTextService : ISpeechToTextService
    {
        public static AutoResetEvent autoEvent = new AutoResetEvent(false);
        public static string SpeechText;
        const int VOICE = 10;
        public static Activity Activity { get; set; }

        public SpeechToTextService()
        {
        }

        public async Task<string> GetSpeechToTextAsync()
        {
            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Fale o nome ou o código do produto");
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

            SpeechText = "";
            autoEvent.Reset();
            Activity.StartActivityForResult(voiceIntent, VOICE);
            await Task.Run(() => { autoEvent.WaitOne(new TimeSpan(0, 2, 0)); });
            return SpeechText;
        }
    }
}
