﻿using System.Collections.Generic;
using HarmonyLib;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace PeasAPI.Managers
{
    public class WatermarkManager
    {
        public class Watermark
        {
            /// <summary>
            /// Text that gets added to the version text
            /// </summary>
            public string VersionText { get; set; }

            /// <summary>
            /// How much the version text should be lowered
            /// </summary>
            public Vector3 VersionTextOffset { get; set; }

            /// <summary>
            /// Text that gets added to the ping text
            /// </summary>
            public string PingText { get; set; }

            /// <summary>
            /// How much the ping text should be lowered
            /// </summary>
            public Vector3 PingTextOffset { get; set; }

            public Watermark(string versionText, string pingText,
                Vector3 versionTextOffset, Vector3 pingTextOffset)
            {
                VersionText = versionText;
                PingText = pingText;
                VersionTextOffset = versionTextOffset;
                PingTextOffset = pingTextOffset;
            }
        }

        private static List<Watermark> Watermarks = new List<Watermark>();

        public static readonly Vector2 defaultVersionTextOffset = new (0f, -0.2f);
        public static readonly Vector2 defaultPingTextOffset = new Vector2(0f, 0f);

        /// <summary>
        /// Whether the reactor version text should be destroyed or not
        /// </summary>
        public static bool UseReactorVersion { get; set; } = false;

        public static Watermark PeasApiWatermark = new Watermark($"\n<color=#ff0000ff>PeasAPI {PeasAPI.Version} <color=#ffffffff> by <color=#ff0000ff>Peasplayer\n<color=#ffffffff>Reactor-Framework", 
            "\n<color=#ff0000ff>PeasAPI", new Vector2(), new Vector2());

        public static void AddWatermark(string versionText, string pingText,
            Vector2 versionTextOffset = new Vector2(), Vector2 pingTextOffset = new Vector2())
        {
            var watermark = new Watermark(versionText, pingText, versionTextOffset, pingTextOffset);
            Watermarks.Add(watermark);
        }

        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionShowerStartPatch
        {
            static void Postfix(VersionShower __instance)
            {
                foreach (var watermark in Watermarks)
                {
                    __instance.transform.position += watermark.VersionTextOffset;

                    if (UseReactorVersion)
                    {
                        if (watermark.VersionText != null)
                            Reactor.Patches.ReactorVersionShower.TextUpdated += text => text.text = watermark.VersionText;
                    }
                    else
                    {
                        if (watermark.VersionText != null)
                            __instance.text.text += watermark.VersionText;
                    
                        foreach (var gameObject in Object.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()))
                            if (gameObject.name.Contains("ReactorVersion"))
                                Object.Destroy(gameObject);
                    }
                }

                if (PeasAPI.ShamelessPlug)
                {
                    __instance.transform.position += PeasApiWatermark.VersionTextOffset;

                    if (UseReactorVersion)
                    {
                        if (PeasApiWatermark.VersionText != null)
                            Reactor.Patches.ReactorVersionShower.TextUpdated += text => text.text = PeasApiWatermark.VersionText;
                    }
                    else
                    {
                        if (PeasApiWatermark.VersionText != null)
                            __instance.text.text += PeasApiWatermark.VersionText;
                    
                        foreach (var gameObject in Object.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>()))
                            if (gameObject.name.Contains("ReactorVersion"))
                                Object.Destroy(gameObject);
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingTrackerUpdatePatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.transform.localPosition = new Vector3(2.5833f, 2.9f, 0f);
                foreach (var watermark in Watermarks)
                {
                    __instance.transform.localPosition += watermark.PingTextOffset;
                
                    if (watermark.PingText != null)
                        __instance.text.text += watermark.PingText;
                }

                if (PeasAPI.ShamelessPlug)
                {
                    __instance.transform.localPosition += PeasApiWatermark.PingTextOffset;
                
                    if (PeasApiWatermark.PingText != null)
                        __instance.text.text += PeasApiWatermark.PingText;
                }
            }
        }
    }
}
