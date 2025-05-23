﻿using System;
using System.IO;
using NAudio.Wave;
using System.Text.RegularExpressions;
using NAudio.CoreAudioApi;
using System.Collections;
using System.Timers;
using NAudio.Wave.SampleProviders;

namespace MasterofCeremonies 
{
    class MC
    {
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        private static bool paused = false;
        private static long bookmark;
        private static ConcatenatingSampleProvider playlist;

        private static int turt = 0; //counter for audio clips to be inserted into the clips Queue
        private static Queue<String> clips = new Queue<String>(); //clips to be processed
        private static List<String> meatClips = new List<String>(); //pool 1
        private static List<String> potatoClips = new List<String>(); //pool 2
        private static Queue<AudioFileReader> april = new Queue<AudioFileReader>(); //clips processed but not yet reached

        private static bool playingTurtles = false;
        private static Random random = new Random();
        private static System.Timers.Timer clock = new System.Timers.Timer(5000)
        {
            AutoReset = true,
            Enabled = false
        };


        static void Main(string[] args)
        {
            //clips snipped from https:// youtu.be/n-zqjplxN1o?si=y6RuomcoNkrxwiWP

            meatClips.Add("NeverLetUsDown"); //fullbody clips in threes
            meatClips.Add("WeShreddedShredder");
            meatClips.Add("WatchOutForShredder");
            meatClips.Add("Clank");
            meatClips.Add("NoClank");

            potatoClips.Add("TeenageBrothers"); //intermediary clips
            potatoClips.Add("TeenageBrothers1234");
            potatoClips.Add("ShellOfATime");

            clock.Elapsed += Zeroed;

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
                    clock.Enabled = true;
                    turt ++; //stack Turtles uses to build a Turtles combo, every time you enter T
                    Turtles();
                    break;
                default:
                    Console.Write("Try entering P or S or R to Pause or Stop or Resume respectively. Enter E to End, or T to Turtles.");
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
            String snip = meatClips[random.Next(5)];
            Console.Write("Turtles playing " + snip);
            clips.Enqueue(snip);
            snip = potatoClips[random.Next(3)];
            Console.WriteLine(", and Turtles also " + snip + ".");
            clips.Enqueue(snip);

            turt -= 1;

            AudioControl();
        }

        private static void Zeroed(Object source, ElapsedEventArgs e)
        {
            Console.Write("Timer has bounced. ");
            Console.WriteLine("April: " + april.Count + " Clips: " + clips.Count);
            if(april.Count > 0)
            {
                Console.Write(" april dequeue.");
                april.Dequeue();
                if(outputDevice.PlaybackState != PlaybackState.Playing)
                {
                    Console.WriteLine(" let's play.");
                    outputDevice.Play();
                }
            }

            if (clips.Count > 0)
            {
                Console.Write(" incoming clip dequeue.");
                Shredder(clips.Dequeue());
            }
            else
            {
                var endClip = new AudioFileReader("audio/End.mp3");
                april.Enqueue(endClip);
                playlist = new ConcatenatingSampleProvider(new[] { endClip });
                outputDevice = new WaveOutEvent();
                outputDevice.Init(playlist);
                outputDevice.Play();

                playingTurtles = false;
            }
            
            AudioControl();
        }

        //idea: two arrays, refresh one array when it is exhausted, and fill it up with the contents of a filler third array, tacking the second array on using FollowedBy().
        static void Shredder(String clipB) //plays Turtles until quota is met
        {
            var sampleB = new AudioFileReader("audio/" + clipB + ".mp3");
            if (!playingTurtles)
            {
                var sampleA = new AudioFileReader("audio/" + "CountItOff" + ".mp3");
                april.Enqueue(sampleA);
                april.Enqueue(sampleB);
                playlist = new ConcatenatingSampleProvider(new[] { sampleA, sampleB });
                playingTurtles = true;
                Console.Write("Turtles has begun. ");
            }
            else
            {
                april.Enqueue(sampleB);
                playlist = new ConcatenatingSampleProvider(april);
            }

            if (outputDevice == null || outputDevice.PlaybackState != PlaybackState.Playing)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.Init(playlist);
                outputDevice.Play();
                Console.Write("Playing the next clip. ");
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
