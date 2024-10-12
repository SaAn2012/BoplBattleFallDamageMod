using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using AGoodNameLib;
using UnityEngine;
using UnityEngine.PlayerLoop;
namespace FallDamage
{


    
    [BepInPlugin("com.SashaAnt.FallDamage", "FallDamage", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ConfigFile config;
        internal static ConfigEntry<int> FallDamage;

        public PlayerPhysics PlayerVelocity;
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            config = Config;
            FallDamage = config.Bind("Settings", "Fall Damage", -30, new ConfigDescription("When the player's y velocity is lower than this number, the player will get damage on impact with an object"));

            //AGoodNameLib.auto_config.NumberBox(ref FallDamage,config,"Fall Damage","Fall damage 1 - easy, 2 - hard, 3 - very hard", 2);

            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(typeof(Patches));
             
        }

        private void Update()
        {
            
        }

    }

    public class Patches
    {
        private static bool toofast;
        private static bool oldtoofast;
        private static bool groundedfirsttime = true;
        private static int FallDamageConfig = Plugin.FallDamage.Value;
        [HarmonyPatch(typeof(PlayerBody), nameof(PlayerBody.UpdateSim))]
        [HarmonyPostfix]
        public static void PlayerBodyUpdateSim(PlayerBody __instance)
        {
            if (__instance.physics.IsGrounded())
            {
                if (groundedfirsttime == true)
                {
                    groundedfirsttime = false;
                }
                if (toofast && !groundedfirsttime)
                {
                    AGoodNameLib.player.do_kill_player(__instance, AGoodNameLib.player.id_from_player_body(__instance), CauseOfDeath.Squished);
                }
            }
            if (__instance.Velocity.y < FallDamageConfig)
            {
                toofast = true;
                Debug.Log(__instance.Velocity.y);
            }
            else
            {
                toofast = false;
            }
            oldtoofast = toofast;

        }
            
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "FallDamage";

        public const string PLUGIN_NAME = "FallDamage";

        public const string PLUGIN_VERSION = "1.0.0";
    }
}
