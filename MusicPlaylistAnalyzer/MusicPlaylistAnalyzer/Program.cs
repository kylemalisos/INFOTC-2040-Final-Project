using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace song
{
    class Song
    {
        public string name { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string genre { get; set; }
        public int size { get; set; }
        public int time { get; set; }
        public int year { get; set; }
        public int plays { get; set; }

        public Song(string name, string artist, string album, string genre, int size, int time, int year, int plays)
        {
            this.name = name;
            this.artist = artist;
            this.album = album;
            this.genre = genre;
            this.size = size;
            this.time = time;
            this.year = year;
            this.plays = plays;
        }
        override public string ToString()
        {
            return String.Format("Name: {0}, Artist: {1}, Album: {2}, Genre: {3}, Size: {4}, Time: {5}, Year: {6}, Plays: {7}", name, artist, album, genre, size, time, year, plays);
        }
    }
}

namespace MusicPlaylistAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 2)
            {
                Console.WriteLine("MusicPlaylistAnalyzer <music_playlist_file_path> <report_file_path>");
                Console.WriteLine("Supply the path to the input file and output file as command line arguments.");
            }
            else {

                string input_file = args[0];
                string output_file = args[1];

                var songs = new List<song.Song> { };
                string cd = Directory.GetCurrentDirectory();

                StreamReader sr = null;
                StreamWriter sw = null;

                try
                {
                    sr = new StreamReader(input_file);

                    var line = sr.ReadLine();
                    var cols = line.Split('\t');

                    var col_num = cols.Length;
                    int i = 1;

                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        cols = line.Split('\t');

                        if (cols.Length < col_num || cols.Length > col_num)
                        {
                            Console.WriteLine("Row {0} contains {1} values. It should contain {2}.", i + 1, cols.Length, col_num);
                            Environment.Exit(-1);
                        }
                        
                        try
                        {
                            var song = new song.Song(cols[0], cols[1], cols[2], cols[3], Int32.Parse(cols[4]), Int32.Parse(cols[5]), Int32.Parse(cols[6]), Int32.Parse(cols[7]));
                            songs.Add(song);
                            i++;
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Row {0} contains a sting value that should be an integer value. Please check this row and try again.", i + 1);
                            Environment.Exit(-1);
                        }
                    }

                    sw = new StreamWriter(output_file);

                    sw.WriteLine("Music Playlist Report\n");

                    var results = from song in songs where song.plays >= 200 select song;
                    sw.WriteLine("Songs that received 200 or more plays:");
                    foreach (var result in results)
                    {
                        sw.WriteLine(result.ToString());
                    }
                    sw.Write("\n");

                    sw.Write("Number of Alternative songs: ");
                    results = from song in songs where song.genre == "Alternative" select song;
                    var num = results.Count();
                    sw.WriteLine(num);
                    sw.Write("\n");

                    sw.Write("Number of Hip-Hop/Rap songs: ");
                    results = from song in songs where song.genre == "Hip-Hop/Rap" select song;
                    num = results.Count();
                    sw.WriteLine(num);
                    sw.Write("\n");

                    sw.WriteLine("Songs from the album Welcome to the Fishbowl:");
                    results = from song in songs where song.album == "Welcome to the Fishbowl" select song;
                    foreach (var result in results)
                    {
                        sw.WriteLine(result.ToString());
                    }
                    sw.Write("\n");

                    sw.WriteLine("Songs from before 1970:");
                    results = from song in songs where song.year < 1970 select song;
                    foreach (var result in results)
                    {
                        sw.WriteLine(result.ToString());
                    }
                    sw.Write("\n");

                    sw.WriteLine("Song names longer than 85 characters:");
                    results = from song in songs where song.name.Length > 85 select song;
                    foreach (var result in results)
                    {
                        sw.WriteLine(result.ToString());
                    }
                    sw.Write("\n");

                    sw.Write("Longest song: ");
                    results = from song in songs orderby song.time descending select song;
                    var ans = results.First();
                    sw.WriteLine(ans.ToString());
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("\nOne of the files could not be found when opening please check the name of the file path and try again.\n");
                }
                catch (IOException e)
                {
                    Console.WriteLine("\nThere was an issue reading or writing lines to the files. Please check the file names and try again.\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error that occured during the execution of the program. Please check your file paths and try again.");
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                    }
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }
            }
        }
    }
}
