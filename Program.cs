using System.Diagnostics;
using System.Net;
using VideoLibrary;

namespace YouTubeDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("write url video : ");
                string url = Console.ReadLine();
                string information = "";
                var videos = YouTube.Default.GetAllVideos(url);
                int hightaudio = 1;
                int hightvideo = 1;
                Console.WriteLine("\nlist all format \n");
                foreach (var item in videos)//write all file on this url
                {
                    Console.WriteLine(item.Resolution + "," + item.Format + "," + item.AudioFormat + "," + item.AudioBitrate + "," + item.ContentLength + "," + item.AdaptiveKind);
                    if (item.AdaptiveKind.ToString() == "Audio" && item.AudioBitrate > hightaudio)
                    {
                        hightaudio = item.AudioBitrate;
                        information = item.AudioFormat + "," + item.AudioBitrate + "," + item.ContentLength;
                    }
                    if (item.Resolution > hightvideo)
                    {
                        hightvideo = item.Resolution;
                    }
                }
                Console.WriteLine("\ndownload high video resolotion {0} and high audio bitrate {1}", hightvideo, hightaudio);
                string[] split = information.Split(',');
                foreach (var item in videos)//download audio
                {
                    if (split[0] == item.AudioFormat.ToString() && split[1] == item.AudioBitrate.ToString() && split[2] == item.ContentLength.ToString())
                    {
                        Console.WriteLine("\ndownload audio with bitrate {0} and size {1}MB", item.AudioBitrate, Math.Round((double)item.ContentLength / 1000000, 2));
                        downloadbest(item, Directory.GetCurrentDirectory() + "\\file123456798.mp3");
                        Console.Write("end\n");
                    }
                }
                foreach (var item in videos)//download video
                {
                    if (item.Resolution == hightvideo)
                    {
                        Console.WriteLine("\ndownload video with Resolution {0} and size {1}MB", item.Resolution, Math.Round((double)item.ContentLength / 1000000, 2));
                        downloadbest(item, Directory.GetCurrentDirectory() + "\\file123456798.mp4");
                        Console.Write("end\n");
                        break;
                    }
                }
                Console.WriteLine("wait for marge");
                combine();
                File.Delete(Directory.GetCurrentDirectory() + "\\file123456798.mp3");
                File.Delete(Directory.GetCurrentDirectory() + "\\file123456798.mp4");
                Console.WriteLine("press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n\n\n" + ex);
                Console.ReadKey();
            }
            Process.Start(Directory.GetCurrentDirectory());
        }
        static void combine()
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg.exe";
            p.StartInfo.Arguments = "-i \"" + Directory.GetCurrentDirectory() + "\\file123456798.mp4\" -i \"" + Directory.GetCurrentDirectory() + "\\file123456798.mp3\" -preset veryfast  \"" + Directory.GetCurrentDirectory() + "\\final.mp4\"";
            p.Start();
            p.WaitForExit();
        }
        static void downloadbest(YouTubeVideo y, string patch)
        {
            int total = 0;
            FileStream fs = null;
            Stream streamweb = null;
            WebResponse w_response = null;
            try
            {
                WebRequest w_request = WebRequest.Create(y.Uri);
                if (w_request != null)
                {
                    w_response = w_request.GetResponse();
                    if (w_response != null)
                    {
                        fs = new FileStream(patch, FileMode.Create);
                        byte[] buffer = new byte[128 * 1024];
                        int bytesRead = 0;
                        streamweb = w_response.GetResponseStream();
                        Console.WriteLine("Download Started");
                        do
                        {
                            bytesRead = streamweb.Read(buffer, 0, buffer.Length);
                            fs.Write(buffer, 0, bytesRead);
                            total += bytesRead;
                            Console.Write($"\rDownloading ({Math.Round(((double)total / (int)y.ContentLength) * 100, 2)}%) {total}/{y.ContentLength}     ");
                        } while (bytesRead > 0);
                        Console.WriteLine("\nDownload Complete");
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n\n\n" + ex);
                Console.ReadKey();
                Process.Start(Directory.GetCurrentDirectory());
            }
            finally
            {
                if (w_response != null) w_response.Close();
                if (fs != null) fs.Close();
                if (streamweb != null) streamweb.Close();
            }
        }
    }


}