using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SubtitleFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory;
            string ext = "avi";
            if (args.Length > 0)
            {
                if (Directory.Exists(args[0]))
	            {
		            directory = args[0];
                }
                else
                {
                    Console.WriteLine("Directory does not exist.");
                    return;
                }
            }
            else
            {
                directory = Directory.GetCurrentDirectory();
            } 

            //get all video files
            string[] videos = Directory.EnumerateFiles(directory, "*." + ext, SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName).ToArray();

            //if no avi filew were found, look for MKVs
            if (videos.Length == 0)
            {
                videos = Directory.EnumerateFiles(directory, "*.MKV", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName).ToArray();
                ext = "mkv";
            }
          
            //get all srt files
            string[] subtitles = Directory.EnumerateFiles(directory, "*.SRT", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName).ToArray();

            if (videos.Length == 0)
            {
                Console.WriteLine("No videos found.");
                return;
            }
            if (subtitles.Length == 0)
            {
                Console.WriteLine("No subtitles found.");
                return;
            }
            
            //for each video file find corresponding srt file and rename it
            foreach (string video in videos)
            {
                //get series and episode 
                var regex = new Regex(@"(s\d\d?e\d\d?)",RegexOptions.IgnoreCase);

                //as the episode can be either S01E01 or s01e01, we'll work with lowercase variant from now on
                string result = regex.Match(video).ToString().ToLower();
                Console.WriteLine("Going to work with: " + result);

                //prepare new name for subtitle
                string newSubtitleFileName = video.Replace("." + ext, ".srt");
                Console.WriteLine("New filename: " + newSubtitleFileName);

                //get subtitle to rename
                string oldFileName = Array.Find(subtitles, s => s.ToLower().Contains(result.ToString()));
                if (oldFileName != null)
                {
                    File.Move(directory + "\\" + oldFileName, directory + "\\" + newSubtitleFileName);
                }
                else
                {
                    Console.WriteLine("No subtitle file found, skipping...");
                }    
            }
        }
    }
}
