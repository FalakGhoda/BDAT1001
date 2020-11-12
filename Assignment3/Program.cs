using Assignment3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace Assignment3
{
    public class Program
    {
        static string url = "ftp://waws-prod-dm1-127.ftp.azurewebsites.windows.net/BDAT1001-10983";
        static string username = @"bdat100119f\bdat1001";
        static string password = "bdat1001";
       

       

        public static object Newtonsoft { get; private set; }
        //Newtonsoft.Json.JsonConvert.SerializeObject(student);

        static void Main(string[] args)
        {

            CsvFileReaderWriter reader = new CsvFileReaderWriter();
            Console.WriteLine(GetDirectory(url));

            string directories = GetDirectory(url);
            List<string> Studentdirectories = directories.Split("\r\n", StringSplitOptions.None).ToList();
            List<student> students = new List<student>();

            foreach (var Studentdir in Studentdirectories)
            {
                string[] Student_Props = Studentdir.Split(" ", StringSplitOptions.None);

                if (Student_Props.Length >= 1 && !String.IsNullOrEmpty(Student_Props[0]))
                {

                    student student = new student();
                    student.StudentId = Student_Props[0];
                    student.FirstName = Student_Props[1];
                    student.LastName = Student_Props[2];

                    string remoteDownloadFilePath = $"/{Studentdir}/info.csv";
                    string remoteDownloadFilePath2 = $"/{Studentdir}/myimage.jpg";
                    //Path to a valid folder and the new file to be saved
                    string localDownloadFileDestination = $@"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Data\{student.FirstName}.csv";
                    string localDownloadFileDestination2 = $@"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Images\{student.FirstName}.jpg";
                    var downcsv = DownloadFile(FTP.BaseUrl + remoteDownloadFilePath, localDownloadFileDestination);
                    var downimg = DownloadFile(FTP.BaseUrl + remoteDownloadFilePath2, localDownloadFileDestination2);
                    
                    var fields = reader.GetEntities(reader.ParseString(localDownloadFileDestination));

                    List<string[]> entities = reader.GetEntities(localDownloadFileDestination);


                    students.Add(student);
                    Console.WriteLine($"{student.LastName} {student.FirstName} {student.LastName} {student.ImageData} {student.MyRecord}");
                }
            }

            Console.WriteLine($"The list contain {students.Count()} students");

            var Student2004 = students.Where(x => x.StudentId.StartsWith("2004"));
            Console.WriteLine($"there are {Student2004.Count()} that start with 2004");

            int Highestage = students.Max(temp => temp.Age);
            Console.WriteLine($"The highest Age in the list is {Highestage}");

            int Lowestage = students.Min(temp => temp.Age);
            Console.WriteLine($"The lowest Age in the list is {Lowestage}");

            double Avg_age = students.Average(temp => temp.Age);
            Console.WriteLine($"The average age is => {Avg_age.ToString("0")}");

            student me = students.Find(x => x.StudentId == "200452357");
            me.MyRecord = true;

            
            List<string> studentsCSV = new List<string>();

            foreach (var student in students)
            {
                Console.WriteLine($"{student.LastName} {student.FirstName} {student.LastName} {student.ImageData} {student.MyRecord}");
                Console.WriteLine(student);
                Console.WriteLine(student.ToCSV());

                studentsCSV.Add(student.ToCSV());
            }

            using (StreamWriter sw = new StreamWriter(@"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Data\students.csv"))
            {
                sw.WriteLine("StudentId, FirstName, LastName");
                foreach (var studentCSV in studentsCSV)
                {
                    sw.WriteLine(studentCSV);
                }
            }
           
            string json = JsonConvert.SerializeObject(students);
            
            using (StreamWriter sw = new StreamWriter(@"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Data\students.json"))
            {
                sw.WriteLine(json);
            }
            

            XmlSerializer serializer = new XmlSerializer(typeof(List<student>));

            using (Stream fs = new FileStream(@"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Data\students.xml", FileMode.Create))
            {
                XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
                serializer.Serialize(writer, students);
            }
            string uploadcsv = @"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Data\students.csv";
     
            string csvpath = "/200452357%20Falak%20Ghoda/students.csv";
            string uploadjson = @"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Data\students.json";
            string jsonpath = "//200452357%20Falak%20Ghoda//students.json";
            string uploadxml = @"C:\Users\falak\Desktop\Assignment3 (1)\Assignment3\Content\Data\students.xml";
            string xmlpath = "/200452357%20Falak%20Ghoda//students.xml";

            Console.WriteLine(UploadFile(uploadcsv, url + csvpath));
            Console.WriteLine(UploadFile(uploadjson, url + jsonpath));
            Console.WriteLine(UploadFile(uploadxml, url + xmlpath));

        }
            
            static string UploadFile(string uploadfile, string url)
            {
            string output = "";

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);

            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(uploadfile))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            //Get the length or size of the file
            request.ContentLength = fileContents.Length;

            //Write the file to the stream on the server
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            //Send the request
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                output = $"Upload File Complete, status {response.StatusDescription}";
            }

            return (output);
            }
            

        public static string DownloadFile(string sourceFileUrl, string destinationFilePath, string username = FTP.UserName, string password = FTP.Password)
        {
            string output;

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(sourceFileUrl);

            //Specify the method of transaction
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            //Indicate Binary so that any file type can be downloaded
            request.UseBinary = true;
            request.KeepAlive = false;

            try
            {
                //Create an instance of a Response object
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    //Request a Response from the server
                    using (Stream stream = response.GetResponseStream())
                    {
                        //Build a variable to hold the data using a size of 1Mb or 1024 bytes
                        byte[] buffer = new byte[1024]; //1 Mb chucks

                        //Establish a file stream to collect data from the response
                        using (FileStream fs = new FileStream(destinationFilePath, FileMode.Create))
                        {
                            //Read data from the stream at the rate of the size of the buffer
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);

                            //Loop until the stream data is complete
                            while (bytesRead > 0)
                            {
                                //Write the data to the file
                                fs.Write(buffer, 0, bytesRead);

                                //Read data from the stream at the rate of the size of the buffer
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                            }

                            //Close the StreamReader
                            fs.Close();
                        }

                        //Close the Stream
                        stream.Close();
                    }

                    //Close the FtpWebResponse
                    response.Close();

                    //Output the results to the return string
                    output = $"Download Complete, status {response.StatusDescription}";
                }

            }
            catch (WebException e)
            {
                FtpWebResponse response = (FtpWebResponse)e.Response;
                //Something went wrong with the Web Request
                output = e.Message + $"\n Exited with status code {response.StatusCode}";
            }
            catch (Exception e)
            {
                //Something went wrong
                output = e.Message;
            }

            //Return the output of the Responce
            return (output);
        }

        static string GetDirectory(string url)
        {
            string output;

            //string directories = GetDirectory(url);

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            //request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(username, password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    output = reader.ReadToEnd();
                }
            }

            Console.WriteLine($"Directory List Complete with status code: {response.StatusDescription}");

            return (output);
        }

        


    }
}
