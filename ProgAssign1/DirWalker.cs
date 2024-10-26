using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgAssign1
{
    internal static class DirWalker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DirWalker));
        public static void walk(String path, ref long CountValidRows, ref long CountSkippedRows, string current_folder = "")
        {
            try
            {
                string[] list = Directory.GetDirectories(path);


                if (list == null) return;

                foreach (string dirpath in list)
                {
                    string temp_date = current_folder;
                    if (Directory.Exists(dirpath))
                    { 
                        string this_dir = dirpath.Substring(dirpath.LastIndexOf('\\') + 1);
                        if (this_dir.ToCharArray().Length == 1)
                        {
                            this_dir = this_dir.PadLeft(2,'0');
                        }
                        temp_date = string.IsNullOrEmpty(temp_date) ? this_dir : current_folder + "/" + this_dir;
                        walk(dirpath, ref CountValidRows, ref CountSkippedRows, temp_date);
                        //log.Info("Dir:" + dirpath);
                    }
                }
                string[] fileList = Directory.GetFiles(path);
                if (current_folder.Split('/').Length == 3 && !DateTime.TryParseExact(current_folder, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    log.Info("Invalid Date due to directory naming! Skiping the directoty: " + current_folder);
                    return;

                }
                foreach (string filepath in fileList)
                {
                    ReadFile.ReadCsvFile(filepath, current_folder, ref CountValidRows, ref CountSkippedRows);
                    //log.Info("File:" + filepath);
                }
            }
            catch (FileNotFoundException)
            {
                log.Error("The file or directory cannot be found.");
            }
            catch (DirectoryNotFoundException)
            {
                log.Error("The file or directory cannot be found.");
            }
            catch (DriveNotFoundException)
            {
                log.Error("The drive specified in 'path' is invalid.");
            }
            catch (PathTooLongException)
            {
                log.Error("'path' exceeds the maxium supported path length.");
            }
            catch (UnauthorizedAccessException)
            {
                log.Error("You do not have permission to create this file.");
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
            {
                log.Error("There is a sharing violation.");
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
            {
                log.Error("The file already exists.");
            }
            catch (IOException e)
            {
                log.Error($"An exception occurred:\nError code: " +
                                  $"{e.HResult & 0x0000FFFF}\nMessage: {e.Message}");
            }
        }
    }
}
