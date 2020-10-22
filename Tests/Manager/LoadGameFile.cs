using System;
using System.Collections.Generic;
using System.IO;

namespace Zero_Web_GetGameContent.Manager
{
    internal class LoadGameFile
    {

        public List<string> StoreItemURLs = new List<string>();
        private string filePath = Environment.CurrentDirectory + "NewGamesToLoad.txt";

        public void LoadGamesFromFile()
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                StoreItemURLs.Add(line);
            }
            //File.Delete(filePath);
            //File.Create(filePath);
        }

    }
}
