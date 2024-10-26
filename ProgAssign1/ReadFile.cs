using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;
using log4net;
using System.Reflection.PortableExecutable;

namespace ProgAssign1
{
    internal static class ReadFile
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReadFile));

        public static List<Dictionary<string, string>>? ReadCsvFile(string filePath, string directory_date, ref long CountValidRows, ref long CountSkippedRows)
        {
            string[] reqestedHeaderList = new string[] {"First Name", "Last Name", "Street Number", "Street", "City",
"Province", "Country", "Postal Code", "Phone Number", "email Address" };
            var records = new List<Dictionary<string, string>>();
            string outputFilePath = @"C:\Users\amit.dey\source\repos\ProgAssign1\ProgAssign1\Output\output.csv";
            bool isRowIngnored = false;
            try
            {

                //log.Info("------------------------ Reading file from: "+ filePath + "-------------------------");
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                }))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = new Dictionary<string, string>();
                        var headers = csv.HeaderRecord ?? Array.Empty<string>();
                        bool containsAll = reqestedHeaderList.All(rh => headers.Contains(rh));
                        if (containsAll)
                        {
                                                     

                            foreach (var header in reqestedHeaderList)
                            {
                                record[header] = csv.GetField<string>(header) ?? string.Empty;
                                if (string.IsNullOrEmpty(record[header]))
                                {
                                    isRowIngnored = true;
                                    CountSkippedRows++;
                                    log.Info("Incomplete record! Missing field: " + header);
                                    break;
                                }
                                else
                                {
                                    record[header] = record[header].Replace("\"", "\"\"");
                                }
                            }
                            record["date"] = directory_date;
                            if (!isRowIngnored)
                            {
                                
                                string? directoryPath = Path.GetDirectoryName(outputFilePath);

                                // Check if the directory exists, if not, create it
                                if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }
                                AppendDictionaryToCsv(record, outputFilePath);
                                CountValidRows++;
                            }
                            else
                            {
                                CountSkippedRows++;
                                isRowIngnored = false;

                            }
                        }
                        else 
                        {
                            CountSkippedRows++;
                        }
                    }
                }
                //CountSkippedRows += skippedRowCount;
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
            catch (Exception ex)
            {
                log.Error($"An error occurred while reading the CSV file: {ex.Message}");
                return null;
            }

            return records;
        }

        private static void AppendDictionaryToCsv(Dictionary<string, string> dictionary, string filePath)
        {
            bool fileExists = File.Exists(filePath);

            using (var writer = new StreamWriter(filePath, true)) // 'true' for append mode
            {
                // Write CSV headers if file is new
                if (!fileExists)
                {
                    writer.WriteLine(string.Join(",", dictionary.Keys));
                }
                writer.WriteLine(string.Join(",", dictionary.Values));
                // Write each dictionary entry as a CSV row
                /*foreach (var entry in dictionary)
                {
                    writer.WriteLine($"{entry.Key},{entry.Value}");

                }*/
            }
        }



    }

}
