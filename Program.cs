using System;
using System.IO;
using NAudio.Wave;
using System.Text.RegularExpressions;
using NAudio.CoreAudioApi;

namespace MasterofCeremonies 
{
    class MC
    {
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        private static bool paused = false;
        private static long bookmark;

        static void Main(string[] args)
        {
            Console.WriteLine("Gimve the audio.");
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
                        Console.WriteLine("\n \n okay. \n");
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
