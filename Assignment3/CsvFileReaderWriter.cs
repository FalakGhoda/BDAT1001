using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assignment3.Models
{
    public class CsvFileReaderWriter
    {
        public CsvFileReaderWriter()
        {
        }

        public static char Delimiter { get; set; } = ','; //Comma by default
        public static char TextQualifier { get; set; } = '"'; //Quote by default

        /// <summary>
        /// Reads the contents of a file into a string
        /// </summary>
        /// <param name="filePath">The full path to a file</param>
        /// <returns>Contents of a file</returns>
        public string Read(string filePath)
        {
            using (StreamReader stream = new StreamReader(filePath))
            {
                return stream.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the contents of a file into a List of strings
        /// Each item in the list is a line from the file
        /// </summary>
        /// <param name="filePath">The full path to a file</param>
        /// <returns>Each line of a file in the form of a List of strings</returns>
        public List<string> ParseFile(string filePath)
        {
            string fileContent = Read(filePath);

            return ParseString(fileContent);
        }

        /// <summary>
        /// Reads the contents of a file into a List of strings
        /// Each item in the list is a line from the delimted text separated by Carriage Return and Linefeed combination
        /// </summary>
        /// <param name="filePath">The full path to a file</param>
        /// <returns>Each line of a file in the form of a List of strings</returns>
        public List<string> ParseString(string delimitedText)
        {
            return delimitedText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Reads the contents of a file into a List of tokenized string arrays
        /// Each item in the list is a line from the file split by " and ,
        /// </summary>
        /// <param name="filePath">The full path to a file</param>
        /// <returns>Each line of a file in the form of a List of strings</returns>
        public List<string[]> GetEntities(string filePath)
        {
            List<string> entries = ParseFile(filePath);

            List<string[]> entities = new List<string[]>();

            foreach (var entry in entries)
            {
                entities.Add(CSVEntityParser(entry));
            }

            return entities;
        }

        /// <summary>
        /// Reads the contents of a file into a List of tokenized string arrays
        /// Each item in the list is a line from the file split by " and ,
        /// </summary>
        /// <returns>Each entry of a List to a tokenized List of string array</returns>
        public List<string[]> GetEntities(List<string> entries)
        {
            List<string[]> entities = new List<string[]>();

            foreach (var entry in entries)
            {
                entities.Add(CSVEntityParser(entry));
            }

            return entities;
        }

        public string[] CSVEntityParser(string delimitedText)
        {
            List<string> tokens = new List<string>();

            bool isInText = false;
            int lastChar = -1;
            int currentChar = 0;

            while (currentChar < delimitedText.Length)
            {
                if (delimitedText[currentChar] == TextQualifier)
                {
                    isInText = !isInText;
                }
                else if (delimitedText[currentChar] == Delimiter)
                {
                    if (!isInText)
                    {
                        tokens.Add(delimitedText.Substring(lastChar + 1, (currentChar - lastChar)).Trim(' ', Delimiter));
                        lastChar = currentChar;
                    }
                }
                currentChar++;
            }

            if (lastChar != delimitedText.Length - 1)
            {
                tokens.Add(delimitedText.Substring(lastChar + 1).Trim());
            }

            return tokens.ToArray();
        }

        /// <summary>
        /// Writes a List of string array to a file delimited by a delimiter
        /// </summary>
        /// <param name="filePath">The full path to a file</param>
        /// <param name="entities">A List of tokenized string array</param>
        public void WriteFile(string filePath, List<string[]> entities)
        {
            StreamWriter sw;

            //Decide which stream to write to Console or File
            if (string.IsNullOrEmpty(filePath))
            {
                //Write to the console if the file path is not specified
                sw = new StreamWriter(Console.OpenStandardOutput());
                sw.AutoFlush = true;
                Console.SetOut(sw);
            }
            else
            {
                //Write to a file if the file path is valid
                sw = new StreamWriter(filePath);
            }

            //Iterate over entries and join the array by the Delimiter
            foreach (var entity in entities)
            {
                sw.WriteLine(string.Join(Delimiter, entity));
            }

            //Close the stream
            sw.Close();
        }
    }
}