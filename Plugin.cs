//Using (ici on importe des bibliothèques utiles)
global using BepInEx;
global using BepInEx.IL2CPP;
global using HarmonyLib;
global using UnityEngine;
global using System;
global using System.IO;
global using UnhollowerRuntimeLib;
using System.Linq;
using System.Timers;
using TMPro;
using System.Collections.Generic;

namespace GibsonServerListPlus
{
    [BepInPlugin("PlaceHereGUID", "GibsonServerListPlus", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            //Ici on créer un fichier log.txt situé dans le dossier GibsonTemplateMod
            Utility.CreateFolder(Variables.mainFolderPath, Variables.logFilePath);

            Utility.CreateFile(Variables.logFilePath, Variables.logFilePath);
            Utility.CreateFile(Variables.serversDataFilePath, Variables.logFilePath);

            Utility.ResetFile(Variables.logFilePath, Variables.logFilePath);
        }


        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Update))]
        [HarmonyPostfix]
        public static void OnSteamManagerUpdate(SteamManager __instance)
        {
            Variables.elapsed += Time.deltaTime;

            if (Variables.elapsed > 0.1f)
            {
                CheckPromptBox();
                CheckServerList();

                Variables.elapsed = 0f;
            }
            void CheckPromptBox()
            {
                GameObject promptBox = GameObject.Find("EssentialUI/Prompt(Clone)/Tab0/Container/Content/Text (TMP)");

                if (promptBox != null && promptBox.activeSelf)
                {
                    string promptText = promptBox.GetComponent<TextMeshProUGUI>().text;
                    DateTime now = DateTime.Now;
                    long unixTimestamp = ((DateTimeOffset)now).ToUnixTimeSeconds();

                    if (promptText.Contains("(Banned)"))
                    {
                        Utility.WriteServerData(Variables.serversDataFilePath, $"{Variables.lastServerOn},Banned,{unixTimestamp}");
                    }
                    if (promptText.Contains("(Kicked)"))
                    {
                        Utility.WriteServerData(Variables.serversDataFilePath, $"{Variables.lastServerOn},Kicked,{unixTimestamp}");
                    }
                }
                else
                {
                    Variables.lastServerOn = (ulong)LobbyManager.Instance.field_Private_CSteamID_0;
                }
            }

            void CheckServerList()
            {
                GameObject serverList = GameObject.Find("UI/ServerList/ServerList");

                if (serverList != null && serverList.activeSelf)
                {
                    Utility.ReadServerData(Variables.serversDataFilePath);
                    var allServers = Utility.GetAllServer();

                    foreach (var server in allServers)
                    {
                        ulong serverId = (ulong)server.GetComponent<MonoBehaviourPublicRaprTetiplTelovemiTeUnique>().field_Private_CSteamID_0;

                        if (Variables.bannedServers.Contains(serverId))
                        {
                            HandleBannedServer(server);
                        }

                        if (Variables.kickedServers.Contains(serverId))
                        {
                            HandleKickedServer(server);
                        }
                    }
                }
            }
            void HandleBannedServer(GameObject server)
            {
                var serverText = server.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                serverText.color = Color.red;
                serverText.text = serverText.text.Replace(" (Banned)", "") + " (Banned)";
            }

            void HandleKickedServer(GameObject server)
            {
                var serverText = server.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                serverText.color = Color.yellow;
                serverText.text = serverText.text.Replace(" (Kicked)", "") + " (Kicked)";
            }
        }


        //Anticheat Bypass 
        [HarmonyPatch(typeof(EffectManager), "Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0")]
        [HarmonyPatch(typeof(LobbyManager), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(MonoBehaviourPublicVesnUnique), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(LobbySettings), "Method_Public_Void_PDM_2")]
        [HarmonyPatch(typeof(MonoBehaviourPublicTeplUnique), "Method_Private_Void_PDM_32")]
        [HarmonyPrefix]
        public static bool Prefix(System.Reflection.MethodBase __originalMethod)
        {
            return false;
        }
    }
}