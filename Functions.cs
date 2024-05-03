using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace GibsonServerListPlus
{
    //Ici on stock les fonctions, dans des class pour la lisibilité du code dans Plugin.cs 

    //Cette class regroupe un ensemble de fonction plus ou moins utile
    public class Utility
    {
        //Cette Fonction permet d'écrire une ligne dans un fichier txt
        public static void Log(string path, string line)
        {
            // Utiliser StreamWriter pour ouvrir le fichier et écrire à la fin
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(line); // Écrire la nouvelle ligne
            }
        }

        //Cette fonction créer un dossier si il n'existe pas déjà
        public static void CreateFolder(string path, string logPath)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Log(logPath, "Erreur [CreateFolder] : " + ex.Message);
            }
        }
        //Cette fonction créer un fichier si il n'existe pas déjà
        public static void CreateFile(string path, string logPath)
        {
            try
            {
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(logPath, "Erreur [CreateFile] : " + ex.Message);
            }
        }

        //Cette fonction réinitialise un fichier
        public static void ResetFile(string path, string logPath)
        {
            try
            {
                // Vérifier si le fichier existe
                if (File.Exists(path))
                {
                    using (StreamWriter sw = new StreamWriter(path, false))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Log(logPath, "Erreur [ResetFile] : " + ex.Message);
            }
        }
        public static List<GameObject> GetAllServer()
        {
            // Trouver tous les GameObjects
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            // Filtrer ceux qui ont le nom "IcePieceSolid(Clone)" ou "PiceSolid(Clone)"
            var filteredServer = allObjects.Where(obj => obj.name == "ServerUIPrefab(Clone)").ToList();

            return filteredServer;
        }

        public static void WriteServerData(string filePath, string newLine)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(newLine))
                {
                    return;
                }

                List<string> lines = File.ReadAllLines(filePath).ToList();

                string[] newData = newLine.Split(",");
                string newId = newData[0];
                string newStatus = newData[1];
                bool idExists = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    string[] existingData = lines[i].Split(",");
                    string existingId = existingData[0];
                    string existingStatus = existingData[1];

                    if (existingId == newId && existingStatus == newStatus)
                    {
                        idExists = true;
                        break;
                    }
                    else if (existingId == newId)
                    {
                        lines[i] = newLine;
                        idExists = true;
                        break;
                    }
                }

                if (!idExists)
                {
                    lines.Add(newLine);
                }

                File.WriteAllLines(filePath, lines);
            }
            catch
            {
            }
        }
        public static void ReadServerData(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                List<string> remainingLines = new List<string>();

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    string[] data = line.Split(",");
                    if (long.TryParse(data[2], out long unixTimestamp))
                    {
                        // Convertir le timestamp Unix en objet DateTime
                        DateTime time = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;

                        // Vérifier si la date et l'heure sont dans les 24 dernières heures
                        if ((DateTime.Now - time).TotalHours <= 24)
                        {
                            remainingLines.Add(line);
                        }

                        if (data[1] == "Kicked")
                        {
                            if (!Variables.kickedServers.Contains(ulong.Parse(data[0])) && !Variables.bannedServers.Contains(ulong.Parse(data[0])))
                                Variables.kickedServers.Add(ulong.Parse(data[0]));
                        }
                        if (data[1] == "Banned")
                        {
                            if (Variables.kickedServers.Contains(ulong.Parse(data[0])))
                                Variables.kickedServers.Remove(ulong.Parse(data[0]));
                            if (!Variables.bannedServers.Contains(ulong.Parse(data[0])))
                                Variables.bannedServers.Add(ulong.Parse(data[0]));

                        }
                    }
                }

                File.WriteAllLines(filePath, remainingLines);
            }
            catch
            {
            }
        }
    }
}
