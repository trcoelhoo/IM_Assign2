using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mmisharp;
using Microsoft.Speech.Recognition;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace speechModality
{
    public class SpeechMod
    {
        // changed 16 april 2020
        private static SpeechRecognitionEngine sre= new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-PT"));
        private Grammar gr;


        public event EventHandler<SpeechEventArg> Recognized;
        protected virtual void onRecognized(SpeechEventArg msg)
        {
            EventHandler<SpeechEventArg> handler = Recognized;
            if (handler != null)
            {
                handler(this, msg);
            }
        }

        private LifeCycleEvents lce;
        private MmiCommunication mmic;

        //  NEW 16 april
        private static Tts tts = new Tts(sre);
        private MmiCommunication mmiReceiver;

        public SpeechMod()
        {
            //init LifeCycleEvents..
            lce = new LifeCycleEvents("ASR", "FUSION", "speech-1", "acoustic", "command"); // LifeCycleEvents(string source, string target, string id, string medium, string mode)
            //mmic = new MmiCommunication("localhost",9876,"User1", "ASR");  //PORT TO FUSION - uncomment this line to work with fusion later
            mmic = new MmiCommunication("localhost", 8000, "User1", "ASR"); // MmiCommunication(string IMhost, int portIM, string UserOD, string thisModalityName)

            mmic.Send(lce.NewContextRequest());

            //load pt recognizer
            //sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-PT"));
            gr = new Grammar(Environment.CurrentDirectory + "\\ptG.grxml", "rootRule");
            sre.LoadGrammar(gr);


            sre.SetInputToDefaultAudioDevice();
            sre.RecognizeAsync(RecognizeMode.Multiple);
            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechHypothesized += Sre_SpeechHypothesized;

            // NEW - TTS support 16 April
            tts.Speak("Olá. Pronto para ver Netflix?");


            //  o TTS  no final indica que se recebe mensagens enviadas para TTS
        mmiReceiver = new MmiCommunication("localhost",8000, "User1", "TTS");
        mmiReceiver.Message += MmiReceived_Message;
        mmiReceiver.Start();


        }


    private void Sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            onRecognized(new SpeechEventArg() { Text = e.Result.Text, Confidence = e.Result.Confidence, Final = false });
        }

        //
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            onRecognized(new SpeechEventArg() { Text = e.Result.Text, Confidence = e.Result.Confidence, Final = true });


           

            //SEND
            // IMPORTANT TO KEEP THE FORMAT {"recognized":["SHAPE","COLOR"]}
            string json = "{ \"recognized\": [";
            foreach (var resultSemantic in e.Result.Semantics)
            {
                json += "\"" + resultSemantic.Value.Value + "\", ";
            }
            json = json.Substring(0, json.Length - 2);
            json += "] }";

            var exNot = lce.ExtensionNotification(e.Result.Audio.StartTime + "", e.Result.Audio.StartTime.Add(e.Result.Audio.Duration) + "", e.Result.Confidence, json);
            mmic.Send(exNot);
        }


        //  NEW 16 April 2020   - create receiver, answer to messages received
        //  Adapted from AppGUI code






        //MmiReceived_Message;

        private void MmiReceived_Message(object sender, MmiEventArgs e)
        {

            Random random = new Random();
            Console.WriteLine(e.Message);

            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;

            Console.WriteLine("arr");
            Console.WriteLine(com);



            dynamic json = JsonConvert.DeserializeObject(com);

       
            switch ((string)json.synthesize[0].ToString())
            {
                case "OPEN DONE.":
                    tts.Speak("Bem-vindo. Estes são os perfis disponíveis. Que conta deseja utilizar?");
                    break;
                case "FILM ACTION DONE.":
                    tts.Speak("Estes são os filmes de ação disponíveis.");
                    break;
                case "FILM COMEDY DONE.":
                    tts.Speak("Estes são os filmes de comédia disponíveis.");
                    break;
                case "FILM DRAMA DONE.":
                    tts.Speak("Estes são os filmes de drama disponíveis.");
                    break;
                case "FILM NOVEL DONE.":
                    tts.Speak("Estes são os filmes de romance disponíveis.");
                    break;
                case "FILM TERROR DONE.":
                    tts.Speak("Estes são os filmes de terror disponíveis.");
                    break;
                case "SERIES ACTION DONE.":
                    tts.Speak("Estas são as séries de ação disponíveis.");
                    break;
                case "SERIES COMEDY DONE.":
                    tts.Speak("Estas são as séries de comédia disponíveis.");
                    break;
                case "SERIES DRAMA DONE.":
                    tts.Speak("Estas são as séries de drama disponíveis.");
                    break;
                case "SERIES NOVEL DONE.":
                    tts.Speak("Estas são as séries de romance disponíveis.");
                    break;
                case "SERIES TERROR DONE.":
                    tts.Speak("Estas são as séries de terror disponíveis.");
                    break;
                case "UNMUTE DONE.":
                    String[] responsesUnmute = new[] { 
                        "Som de volta!",
                        "A reproduzir com som.",
                        "Som ligado.",
                        "De novo com som.",
                    };

                    // responde com uma frase aletória
                    tts.Speak(responsesUnmute[random.Next(0, responsesUnmute.Length)]);
                    break;
                case "JUMPINTRO DONE.":
                    String[] responsesJumpIntro = new[] { 
                        "Ninguém gosta da introdução. A começar o episódio!",
                        "Ok. Episódio a começar.",
                        "Saltei a introdução.",
                    };

                    // responde com uma frase aletória
                    tts.Speak(responsesJumpIntro[random.Next(0, responsesJumpIntro.Length)]);
                    break;

            }


        }
    }
}
