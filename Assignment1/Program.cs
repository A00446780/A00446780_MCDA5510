using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Assignment1
{
    public class DirWalker
    {
        int countGood = 0;
        int countBad = 0;

        public void walk(String path)
        {
            string[] list = Directory.GetDirectories(path);
            if (list.Length != 0)
            {
                foreach (string dirpath in list)
                {
                    if (Directory.Exists(dirpath))
                    {
                        walk(dirpath);
                        //Console.WriteLine("Dir: " + dirpath);
                    }
                }
            }
            string[] fileList = Directory.GetFiles(path);
            foreach (string filepath in fileList)
            {
                    //Console.WriteLine(filepath);
                    this.parse(filepath);
            }
        }

        public void parse(String fileName)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(fileName))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    Boolean badRow = false;
                    int rowNum = 0;
                    while (!parser.EndOfData)
                    {
                        try
                        {
                            //Process row
                            if (rowNum != 0)
                            {
                                string[] fields = parser.ReadFields();
                                foreach (string f1 in fields)
                                {
                                    //if (f1.Length == 0 || f1 == "\"\"")
                                    if (f1.Length == 0)
                                    {
                                        badRow = true;
                                        break;
                                    }
                                }
                                if (!badRow)
                                {
                                    int fieldCount = 0;
                                    foreach (string f2 in fields)
                                    {
                                        fieldCount++;
                                        if (fieldCount < fields.Length) op.Write(f2 + ",");
                                        else op.Write(f2+","+year+"/"+mm+"/"+dd);
                                    }
                                    countGood++;
                                    op.WriteLine();
                                }
                                else
                                {
                                    countBad++;
                                }
                                badRow = false;
                            }
                            else
                            {
                                parser.ReadFields();
                                rowNum++;
                            }
                        }
                        catch (Exception e)
                        {
                            el.WriteLine(e.Message);
                        }
                    }
                }
            }
            catch (IOException ioe)
            {
                el.WriteLine(ioe.StackTrace);
            }
        }


        static StreamWriter OpenStream(string path)
        {
            if (path is null)
            {
                Console.WriteLine("You did not supply a file path.");
                return null;
            }

            try
            {
                var fs = new FileStream(path, FileMode.CreateNew);
                return new StreamWriter(fs);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file or directory cannot be found.");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("The file or directory cannot be found.");
            }
            catch (DriveNotFoundException)
            {
                Console.WriteLine("The drive specified in 'path' is invalid.");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("'path' exceeds the maxium supported path length.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("You do not have permission to create this file.");
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
            {
                Console.WriteLine("There is a sharing violation.");
            }
            catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
            {
                Console.WriteLine("The file already exists.");
            }
            catch (IOException e)
            {
                Console.WriteLine($"An exception occurred:\nError code: " +
                                  $"{e.HResult & 0x0000FFFF}\nMessage: {e.Message}");
            }
            return null;
        }

        StreamWriter el;
        StreamWriter op;
        StreamWriter ol;
        string year;
        string mm;
        string dd;

        public static void Main(String[] args)
        {
            DirWalker fw = new DirWalker();
            fw.el = OpenStream(@".\errorLog.txt");
            fw.op = OpenStream(@".\output.csv");
            fw.ol = OpenStream(@".\outputLog.txt");
            if (fw.el != null && fw.op != null && fw.ol != null)
            {
                DateTime start = DateTime.Now;
                fw.year = start.Year.ToString();
                fw.mm = start.Month.ToString();
                fw.dd = start.Day.ToString();
                fw.op.WriteLine("First Name,Last Name,Street Number,Street,City,Province,Postal Code,Country,Phone Number,email Address,Date");
                fw.walk(@"C:\Users\s6860862\Documents\MCDA 5510 Software Dev\Sample Data Large");
                DateTime end = DateTime.Now;
                TimeSpan ts = (end - start);
                fw.ol.WriteLine("Total Execution Time = {0:00}h {1:00}m {2:00}s {3}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                fw.ol.WriteLine("Total Valid Rows = " + fw.countGood + "\nTotal Skipped Rows = " + fw.countBad);
            }
            fw.el.Close();
            fw.op.Close();
            fw.ol.Close();
        }
    }
}