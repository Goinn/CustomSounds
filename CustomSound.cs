using System;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using HarmonyLib;
using Util.Audio;
using System.IO;

namespace Custom_Sound
{
    public class CustomSound : MelonMod
    {
        public static string root = "/CustomSounds/";
        public static string full_path = Application.dataPath + "/.." + root;
        public static string hitFilePath = "hit.wav";
        public static string railFilePath = "rail.wav";
        public static string missFilePath = "miss.wav";
        public static string specialFilePath = "special.wav";
        public static string specialpassFilePath = "specialpass.wav";
        public static string specialfailFilePath = "specialfail.wav";
        public static string maxmultilierFilePath = "maxmultiplier.wav";
        public static string wallFilePath = "wall.wav";
        public static string buttonclickFilePath = "buttonclick.wav";
        public static string buttonhoverFilePath = "buttonhover.wav";
        public static string gameoverFilePath = "gameover.wav";
        public static string resultFilePath = "resultbgm.wav";
        public static string ambientFilePath = "ambient.wav";
        public static string applauseFilePath = "applause.wav";
        public static CustomSound cs_instance;
        /*
         * missing sounds:
            * song pass
            * power up
            * Game_PauseMenu.OnSFXChange()
         */

        public enum SfxSelect
        {
            hit,
            miss,
            rail,
            special,
            specialpass,
            specialfail,
            maxcombo,
            wall,
            buttonclick,
            buttonhover,
            gameover,
            resultbgm,
            ambient,
            applause
        }

        public IEnumerator GetAudioClip(string path, SfxSelect sfxSelect)
        {
            AudioClip hitAudio = null;
            if (!File.Exists(full_path + path)) yield break;
            
            using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + root + path, AudioType.WAV))
            {

                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError | webRequest.isHttpError)
                {
                    MelonLogger.Msg("GetAudioClip Error");
                }
                else
                {
                    if (new FileInfo(full_path + path).Length != 0)
                    {
                        hitAudio = DownloadHandlerAudioClip.GetContent(webRequest);
                    }
                    
                    // position in array: 0 = Impact, 1 = Laser
                    switch (sfxSelect)
                    {
                        // from Util_HitSFXSource
                        case SfxSelect.hit: // note hit
                            Util_HitSFXSource.s_instance.m_hitBadClip[1] = hitAudio;
                            Util_HitSFXSource.s_instance.m_hitClip[1] = hitAudio;
                            Util_HitSFXSource.s_instance.m_hitPerfectClip[1] = hitAudio;
                            break;
                        case SfxSelect.miss: // note miss
                            Util_HitSFXSource.s_instance.m_failClip = hitAudio;
                            break;
                        case SfxSelect.rail:
                            Util_HitSFXSource.s_instance.m_lineStartClip[1] = hitAudio;
                            //Util_HitSFXSource.s_instance.m_lineClip[1] = hitAudio; is private for some reason
                            Util_HitSFXSource.s_instance.m_lineEndClip[1] = hitAudio; // rail end hit
                            break;
                        case SfxSelect.special: // special start
                            Util_HitSFXSource.s_instance.m_comboClip = hitAudio;
                            break;
                        case SfxSelect.specialpass: // special complete
                            Util_HitSFXSource.s_instance.m_comboEndClip = hitAudio;
                            break;
                        case SfxSelect.specialfail: // special fail
                            Util_HitSFXSource.s_instance.m_comboFailClip = hitAudio;
                            break;
                        case SfxSelect.maxcombo: // max combo
                            Util_HitSFXSource.s_instance.m_rewardClip = hitAudio;
                            break;
                        case SfxSelect.wall: // wall hit
                            Util_HitSFXSource.s_instance.m_failClipWall = hitAudio;
                            break;
                    }
                }
            }
        }
        public IEnumerator GetAudioClip_HitSFX(string path, String fieldName)
        {
            AudioClip hitAudio = null;
            if (!File.Exists(full_path + path)) yield break;
            using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + root + path, AudioType.WAV))
            {
                FieldInfo instanceField = null;
                FieldInfo audioField = null;
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError | webRequest.isHttpError)
                {
                    MelonLogger.Msg("GetAudioClip Error");
                }
                else
                {
                    if (new FileInfo(full_path + path).Length != 0)
                    {
                        hitAudio = DownloadHandlerAudioClip.GetContent(webRequest);
                    }
                    Type objectType = typeof(Util_HitSFXSource);
                    var instance = Util_HitSFXSource.s_instance;
                    audioField = objectType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    audioField.SetValue(instance, hitAudio);
                }
            }
        }

        public IEnumerator GetAudioClip(string path, Type objectType, String fieldName, bool isPublicInstance = false)
        {
            AudioClip hitAudio = null;
            if (!File.Exists(full_path + path)) yield break;
            using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + root + path, AudioType.WAV))
            {
                FieldInfo instanceField = null;
                FieldInfo audioField = null;
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError | webRequest.isHttpError)
                {
                    MelonLogger.Msg("GetAudioClip Error");
                }
                else
                {
                    if (new FileInfo(full_path + path).Length != 0)
                    {
                        hitAudio = DownloadHandlerAudioClip.GetContent(webRequest);
                    }
                    if (isPublicInstance)
                    {
                        instanceField = objectType.GetField("s_instance", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    }
                    else
                    {
                        instanceField = objectType.GetField("s_instance", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    }
                    var instance = instanceField.GetValue(null);
                    audioField = objectType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    audioField.SetValue(instance, hitAudio);
                }
            }
        }

        public IEnumerator GetAudioClip(string path, AudioSource source)
        {
            AudioClip hitAudio = null;
            if (!File.Exists(full_path + path)) yield break;
            using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + root + path, AudioType.WAV))
            {

                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError | webRequest.isHttpError)
                {
                    MelonLogger.Msg("GetAudioClip Error");
                    //Game_InfoProvider.WriteToLogFile("GetAudioClip Error " + webRequest.error, false);
                }
                else
                {
                    if (new FileInfo(full_path + path).Length != 0)
                    {
                        hitAudio = DownloadHandlerAudioClip.GetContent(webRequest);
                    }
                    source.clip = hitAudio;
                    source.Play();
                }
            }
        }

        [HarmonyPatch(typeof(Util_HitSFXSource), "Awake")]
        public static class OverwriteHitSFX
        {
            private static void Postfix()
            {
                MelonLogger.Msg("Setting Hit SFX");
                var cs_instance = new CustomSound();
                Type classType = typeof(Util_HitSFXSource);
                Util_HitSFXSource instance = Util_HitSFXSource.s_instance;
                FieldInfo pathField = null;

                MelonCoroutines.Start(cs_instance.GetAudioClip(hitFilePath, SfxSelect.hit));
                MelonCoroutines.Start(cs_instance.GetAudioClip(missFilePath, SfxSelect.miss));
                //MelonCoroutines.Start(instance.GetAudioClip(railFilePath, SfxSelect.rail));
                MelonCoroutines.Start(cs_instance.GetAudioClip(specialFilePath, SfxSelect.special));
                MelonCoroutines.Start(cs_instance.GetAudioClip(specialpassFilePath, SfxSelect.specialpass));
                MelonCoroutines.Start(cs_instance.GetAudioClip(specialfailFilePath, SfxSelect.specialfail));
                MelonCoroutines.Start(cs_instance.GetAudioClip(maxmultilierFilePath, SfxSelect.maxcombo));
                MelonCoroutines.Start(cs_instance.GetAudioClip(wallFilePath, SfxSelect.wall));
            }
        }
        [HarmonyPatch(typeof(ExtraSFXAudioController), "Awake")]
        public static class OverwriteXSFX
        {
            private static void Postfix()
            {

                /* from ExtraSFXAudioController (all audioclips are private)
                backButtonClipDefault
                homeMenuBTNHover
                homeMenuBTNStay -> AudioSource StaySound
                timeCounterClip
                multiplayerRiderJoinAnnouncer
                multiplayerCounterReadyAnnouncer
                multiplayerPositionsAnnouncer
                homeMenuElectricity
                 */
                MelonLogger.Msg("Setting Menu SFX");
                var cs_instance = new CustomSound();
                Type classType = typeof(ExtraSFXAudioController);
                MelonCoroutines.Start(cs_instance.GetAudioClip(buttonclickFilePath, classType, "buttonClickClip"));
                MelonCoroutines.Start(cs_instance.GetAudioClip(buttonhoverFilePath, classType, "buttonHoverClip"));
            }
        }
        [HarmonyPatch(typeof(GameControlManager), "Awake")]
        public static class OverwriteGCM
        {
            private static void Postfix()
            {
                /* from GameControlManager (all audioclips are private)
                        m_GameOverClip
                        m_SpecialStartClip -> unused?
                        m_SpecialEndClip -> unused?
                        m_SpecialFailCip -> unused?
                        m_CounterClip
                         */
                var cs_instance = new CustomSound();
                MelonLogger.Msg("Setting Game Over SFX");
                Type classType = typeof(GameControlManager);
                MelonCoroutines.Start(cs_instance.GetAudioClip(gameoverFilePath, classType, "m_GameOverClip", true));
            }
        }

        public static void OverwriteResultScreen()
        {
            CustomSound cs_instance = new CustomSound();
            GameObject backgroundAudio = GameObject.Find("[Background Audio]");
            Transform music = backgroundAudio.transform.Find("[Music]");
            AudioSource musicSource = (AudioSource)music.GetComponent(typeof(AudioSource));
            Component[] bgAudioSources = backgroundAudio.GetComponents(typeof(AudioSource));

            MelonLogger.Msg("Setting Result Scene SFX");
            foreach (AudioSource source in bgAudioSources)
            {
                switch (source.clip.name)
                {
                    case "ScoreEnd1":
                        MelonCoroutines.Start(cs_instance.GetAudioClip(applauseFilePath, source));
                        break;
                    case "Ambient":
                        MelonCoroutines.Start(cs_instance.GetAudioClip(ambientFilePath, source));
                        break;
                    case "Carro5":
                        //cs_instance.GetAudioClip(droneSoundPath, source.clip);
                        source.enabled = false;
                        break;
                }
            }

            MelonCoroutines.Start(cs_instance.GetAudioClip(resultFilePath, musicSource));

        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            /*var mainMenuScenes = new List<string>()
            {
                "01.The Room",
                "02.The Void",
                "03.Roof Top",
                "04.The Planet",
                "SongSelection"
            };*/
            base.OnSceneWasInitialized(buildIndex, sceneName);
            if (sceneName == "3.GameEnd") OverwriteResultScreen();
        }

        public override void OnApplicationLateStart()
        {
            base.OnApplicationLateStart();
            string[] audioFiles = new string[] { hitFilePath, railFilePath, missFilePath, specialFilePath, specialpassFilePath,
                specialfailFilePath, maxmultilierFilePath, wallFilePath, buttonclickFilePath, buttonhoverFilePath, gameoverFilePath,
                resultFilePath, applauseFilePath};

            LoggerInstance.Msg("Init");
            if (!Directory.Exists(Application.dataPath + "/../" + root))
            {
                Directory.CreateDirectory(Application.dataPath + "/../" + root);
            }
            /*foreach (string file in audioFiles)
            {
                if (!File.Exists(Application.dataPath + "/../" + root + file))
                {
                    File.Create(Application.dataPath + "/../" + root + file);
                }
            }*/
        }
    }
}
