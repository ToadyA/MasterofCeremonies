using System;
using System.IO;
using NAudio.Wave;
using System.Text.RegularExpressions;
using NAudio.CoreAudioApi;
using System.Collections;

namespace MasterofCeremonies 
{
    class MC
    {
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        private static bool paused = false;
        private static long bookmark;

        private static int turt = 0; //counter for audio clips to be inserted into the clips Queue
        private static int les = 0; //counter for how many audio clips remain, so that we can determine when to play the closer clip
        private static Queue<String> clips = new Queue<String>();
        private static List<String> meatClips = new List<String>();
        private static List<String> potatoClips = new List<String>();
        private static bool playingTurtles = false;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            meatClips.Add("NeverLetUsDown"); //fullbody clips in threes
            meatClips.Add("WeShreddedShredder");
            meatClips.Add("WatchOutForShredder");
            meatClips.Add("Clank");
            meatClips.Add("NoClank");

            potatoClips.Add("TeenageBrothers"); //intermediary clips
            potatoClips.Add("TeenageBrothers1234");
            potatoClips.Add("ShellOfATime");

            Console.WriteLine("Give me the audio.");
            string inputAudio = Console.ReadLine();
            string nAudio = Janitor(inputAudio);
            Console.WriteLine("Cut your hair, flat back and up front!");
            Console.WriteLine(nAudio);

            if (File.Exists(nAudio))
            {
                PlayAudio(nAudio);
                AudioControl();
            }
            else
            {
                Console.WriteLine("OopsOops");
            }
        }

        static void AudioControl()
        {
            Console.WriteLine("");
            string input = "";
            input = Console.ReadLine()?.ToUpper(); ;
            Letters(input);
        }

        static void Letters(string input)
        {
            switch (input)
            {
                case "P":
                    PauseAudio();
                    break;
                case "R":
                    if (!paused)
                    {
                        Console.WriteLine("\n\nokay.\n");
                    }
                    ResumeAudio();
                    break;
                case "S":
                    StopAudio();
                    break;
                case "E":
                    outputDevice?.Dispose();
                    audioFile?.Dispose();
                    Environment.Exit(0);
                    break;
                case "T":
                    turt ++;//stack Turtles uses to build a Turtles combo, every time you enter T
                    Turtles();
                    break;
                default:
                    Console.Write("Try entering P or S to Pause or Stop respectively, or R to resume. Enter E to end.");
                    break;
            }
        }

        static void PlayAudio(string nAudio)
        {
            audioFile = new AudioFileReader(nAudio);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            outputDevice.Play();
            paused = false;
            Console.Write("Playing; enter S to Stop, P to Pause, R to Resume. Be sure to hit Enter.");

            AudioControl();
        }

        static void PauseAudio()
        {
            if(outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                paused = true;
                bookmark = audioFile.Position;
                outputDevice.Pause();
                Console.Write("Audio is paused. Enter 'R' to resume.");
            }

            AudioControl();
        }

        static void ResumeAudio()
        {
            if(outputDevice != null && paused == true)
            {
                audioFile.Position = bookmark;
                outputDevice.Play();
                paused = false;
                Console.WriteLine("It starts.");
            }

            AudioControl();
        }

        static void StopAudio()
        {
            if(outputDevice != null)
            {
                outputDevice.Stop();
                paused = false;
                bookmark = 0;
                Console.WriteLine("Give you a perm now, and we're done!.");
            }

            AudioControl();
        }

        static void Turtles() //buffer class to manage Turtles counter, turt
        {
            if (!playingTurtles)
            {
                Shredder(); //if it isn't already playing, play the Turtles audio
                playingTurtles = true;
            }

            clips.Enqueue(meatClips[random.Next(5)]);
            if (clips.Count % 3 == 0)
            {
                clips.Enqueue(potatoClips[random.Next(3)]);//every fourth clip results in an intermediary clip, on the house
            }

            turt -= 1;
        }

        //idea: have a regular check every few seconds for detecting new clips to add, so as to tack it on at the end of a clip.
        //idea: two arrays, refresh one array when it is exhausted, and fill it up with the contents of a filler third array, tacking the second array on using FollowedBy().
        static void Shredder() //plays Turtles until quota is met
        {
            //exhaust the queue, assigning a random timestamp for each entry.
            
            //if the queue is empty, play a finisher.
            //do not relinquish user control during this process (assume it's fine. If it isn't, do something about it).
        }

        static string Janitor(string path)
        {
            if (path.StartsWith("\"") && path.EndsWith("\"")) //using the Copy as Path option parethesizes your path with double-quotes, which cannot be read inherently
            {
                path = path.Substring(1, path.Length - 2);
            }
            path = Regex.Replace(path, @"[/\\]+", @"\");//replaces one or more forward slashes, and one or more back slashes, with single back slashes

            return path;
        }
    }
}
