using System;
using System.Text;
using System.IO;

namespace RhoTools
{
    /// <summary>
    /// Read and write text easily
    /// </summary>
    public class CTxtManager
    {
        /// <summary>
        /// Read file content
        /// </summary>
        /// <param name="fileName">Path to file</param>
        /// <returns>Text contained in file</returns>
        static public string Read(string fileName)
        {
            try
            {
                string tOutput = "";
                string line;
                // Create a new StreamReader, tell it which file to read and what encoding the file
                // was saved as
                StreamReader theReader = new StreamReader(fileName);

                // Immediately clean up the reader after this block of code is done.
                // You generally use the "using" statement for potentially memory-intensive objects
                // instead of relying on garbage collection.
                // (Do not confuse this with the using directive for namespace at the 
                // beginning of a class!)
                using (theReader)
                {
                    // While there's lines left in the text file, do this:
                    do
                    {
                        line = theReader.ReadLine();

                        if (line != null)
                        {
                            // Do whatever you need to do with the text line, it's a string now
                            // In this example, I split it into arguments based on comma
                            // deliniators, then send that array to DoStuff()
                            tOutput += line + "\n";
                        }
                    }
                    while (line != null);

                    // Done reading, close the reader and return true to broadcast success    
                    theReader.Close();
                    return tOutput;
                }
            }
            catch (Exception e)
            {
            // If anything broke in the try block, we throw an exception with information
            // on what didn't work
                UnityEngine.Debug.LogError(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Write string to file
        /// </summary>
        /// <param name="aPath">Path to file</param>
        /// <param name="aContent">String to save</param>
        static public void Write(string aPath, string aContent)
        {
            UTF8Encoding tEncod = new UTF8Encoding();
            byte[] tByteData = tEncod.GetBytes(aContent);

            FileStream tFileStream = null;
            if (File.Exists(aPath))
                tFileStream = new FileStream(aPath, FileMode.Open);
            else
                tFileStream = new FileStream(aPath, FileMode.Create);
            tFileStream.SetLength(0);
            tFileStream.Write(tByteData, 0, tByteData.Length);
            tFileStream.Close();
        }

        /// <summary>
        /// Load json file
        /// </summary>
        /// <param name="aPath">Path to file</param>
        /// <returns>Json node</returns>
        public static SimpleJSON.JSONNode LoadJson(string aPath)
        {
            string tJSON = Read(aPath);
            if (tJSON == null)
                return null;
            return SimpleJSON.JSON.Parse(tJSON);
        }
    }
}
