using System.Collections.Generic;

namespace GibsonServerListPlus
{
    //Ici on stock les variables "globale" pour la lisibilité du code dans Plugin.cs 
    internal class Variables
    {
        //folder
        public static string assemblyFolderPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string defaultFolderPath = assemblyFolderPath + "\\";
        public static string mainFolderPath = defaultFolderPath + @"GibsonServerListPlus\";

        //file
        public static string logFilePath = mainFolderPath + "log.txt";
        public static string serversDataFilePath = mainFolderPath + "serverData.txt";


        //List
        public static List<Il2CppSystem.Collections.Generic.Dictionary<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique>.Entry> playersList;
        public static List<ulong> bannedServers = new List<ulong>();
        public static List<ulong> kickedServers = new List<ulong>();

        //float
        public static float elapsed;

        //ulong
        public static ulong lastServerOn;
    }
}
