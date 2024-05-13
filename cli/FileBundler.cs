using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace cli
{
    public static class FileBundler
    {
      public static void creatFileByLanguage(string language, string output, bool note, bool sort, bool removeEmptyLines, string author)
        {
           
            string[] langArr = {"c#","java","python","javascript","react"};
            string[] endArr = { "cs", "java", "py", "js", "jsx" };
            string ending = "";
            string directoryPath = Directory.GetCurrentDirectory();
            string filnalPath=Directory.GetCurrentDirectory();
            string[] files=null;
            
            if (string.IsNullOrEmpty(output))                                                                                 //בדיקה שאכן הכניס ערך לoutput
            {
                throw new InvalidOperationException("The --output option is required. Please provide a valid output file name.");
            }

            if (language.ToLower() == "all")                                                                                        //האם הכניס all
            {
                if (sort)                                                                                               //האם לשנות את אופן המיון
                {
                    files = Directory.GetFiles(directoryPath).OrderBy(file => Path.GetExtension(file), StringComparer.OrdinalIgnoreCase).ToArray();
                }
                else
                {
                    files = Directory.GetFiles(directoryPath).OrderBy(file => file).ToArray();
                }
                ending = "txt";
            }
            else                                                                                                                 //הכניס שפה מסויימת
            {
                for (int i = 0; i < langArr.Length; i++)                                                                        //מציאת סיומת מתאימה
                {
                    if (langArr[i].ToLower() == language.ToLower())
                    {
                        ending = endArr[i];
                        break;
                    }
                }
                files = Directory.GetFiles(directoryPath, $"*.{ending}");                                               //שמירת הקבצים התואמים לסיומת
            }

            if (ending == "")                                                                                              // בדיק תקינות שאכן יש שפה
            {
                throw new InvalidOperationException("Invalid ending. The ending cannot be empty.");
            }

            if (IsFolderPathFormat(output))                                                                          //אם זה ניתוב צור בו את התיקייה
            {
                try
                {
                    using (File.Create(output+".txt")) { } ;                                                                        //אם הצליח
                    filnalPath = output + ".txt";
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    Console.WriteLine("file directory is invalid");                                                                     //אם לא הצליח
                }
            }
            else                                                                                                        //צור קובץ חדש בתיקייה הנוכחית
            {
                var directory = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(directory, output);
                try
                {
                    using (File.Create(output + ".txt")) { };
                    filnalPath = output + ".txt";
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    Console.WriteLine("file directory is invalid");
                }

            }
            
            copyContant(files,filnalPath,ending,note, removeEmptyLines, author);                                //שלחה להעתקת תוכן הקבצים לקובץ החדש
      }

        public static bool IsFolderPathFormat(string path)                                                  //בודקת האם המחרוזת היא בפורמט של ניתוב
        {
            string pattern = @"^[A-Za-z]:\\.+";
            return Regex.IsMatch(path, pattern);
        }
        public static void copyContant(string[] files, string destinationFilePath, string ending,bool note, bool removeEmptyLines, string author)//מעתיקה את תוכן הקבצים לקובץ היעד
        {
            
            if (!string.IsNullOrEmpty(author)) { 
                File.AppendAllText(destinationFilePath,  author);                                                             //כתיבת הסופר
                File.AppendAllText(destinationFilePath, Environment.NewLine);
            }
            foreach (string filePath in files)
            {
               
                if (note)
                {
                    string Note = Path.GetFullPath(filePath);
                    File.AppendAllText(destinationFilePath,Note);
                    File.AppendAllText(destinationFilePath, Environment.NewLine);
                }
                string fileContents = File.ReadAllText(filePath);
                if (removeEmptyLines)
                {
                    fileContents = RemoveEmptyLines(fileContents);
                }
                File.AppendAllText(destinationFilePath, fileContents);
                File.AppendAllText(destinationFilePath, Environment.NewLine);
            }
        } 
        private static string RemoveEmptyLines(string text)
        {
            string[] lines = text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.None);
            lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            return string.Join(Environment.NewLine, lines);
        }
    }
}
