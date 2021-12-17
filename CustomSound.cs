using System;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using HarmonyLib;
using Util.Audio;

namespace Custom_Sound
{
    public class CustomSound : MelonMod
    {
        public static string root = "/CustomSounds/";
        public static string hitFilePath = "hit.wav";
        public static string railFilePath = "rail.wav";
        public static string missFilePath = "miss.wav";
        public static string specialFilePath = "special.wav";
        public static string specialpassFilePath = "specialpass.wav";
        public static string specialfailFilePath = "specialfail.wav";
        public static string maxcomboFilePath = "maxcombo.wav";
        public static string wallFilePath = "wall.wav";
        public static string buttonclickFilePath = "buttonclick.wav";
        public static string buttonhoverFilePath = "buttonhover.wav";
        public static string gameoverFilePath = "gameover.wav";
        public static string resultFilePath = "resultbgm.wav";

        public static CustomSound cs_instance;
        /*
         * missing sounds:
            * song pass
            * power up
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
            resultbgm
        }
        

        public IEnumerator GetAudioClip(string path, SfxSelect sfxSelect)
        {
            Type xsfx = null;
            ExtraSFXAudioController xsfxInstance = null;
            //Type util_hitsfxsource = typeof(Util_HitSFXSource);
            //FieldInfo HitSFX_instance = util_hitsfxsource.GetField("RightSFXSource.clip", BindingFlags.NonPublic | BindingFlags.Instance);
            using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + root + path, AudioType.WAV))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    MelonLogger.Msg("GetAudioClip Error");
                    //Game_InfoProvider.WriteToLogFile("GetAudioClip Error " + webRequest.error, false);
                }
                else
                { 
                    if (sfxSelect >= SfxSelect.buttonclick & sfxSelect <= SfxSelect.buttonhover)
                    {
                        xsfx = typeof(ExtraSFXAudioController);
                        FieldInfo xsfxInstanceField = xsfx.GetField("s_instance", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        xsfxInstance = (ExtraSFXAudioController)xsfxInstanceField.GetValue(null);
                    }

                    AudioClip hitAudio = DownloadHandlerAudioClip.GetContent(webRequest);
                    
                    // position in array: 0 = Impact, 1 = Laser
                    switch (sfxSelect)
                    {
                        // from Util_HitSFXSource
                        case SfxSelect.hit: // note hit
                            Util_HitSFXSource.s_instance.m_hitBadClip[1] = hitAudio;
                            Util_HitSFXSource.s_instance.m_hitClip[1] = hitAudio;
                            Util_HitSFXSource.s_instance.m_hitPerfectClip[1] = hitAudio;
                            MelonLogger.Msg("Hit set");
                            break;
                        case SfxSelect.miss: // note miss
                            Util_HitSFXSource.s_instance.m_failClip = hitAudio;
                            MelonLogger.Msg("Miss set");
                            break;
                        case SfxSelect.rail:
                            Util_HitSFXSource.s_instance.m_lineStartClip[1] = hitAudio;
                            //Util_HitSFXSource.s_instance.m_lineClip[1] = hitAudio; is private for some reason
                            Util_HitSFXSource.s_instance.m_lineEndClip[1] = hitAudio; // rail end hit
                            MelonLogger.Msg("Rail set");
                            break;
                        case SfxSelect.special: // special start
                            Util_HitSFXSource.s_instance.m_comboClip = hitAudio;
                            MelonLogger.Msg("Special start set");
                            break;
                        case SfxSelect.specialpass: // special complete
                            Util_HitSFXSource.s_instance.m_comboEndClip = hitAudio;
                            MelonLogger.Msg("Special complete set");
                            break;
                        case SfxSelect.specialfail: // special fail
                            Util_HitSFXSource.s_instance.m_comboFailClip = hitAudio;
                            MelonLogger.Msg("Special fail set");
                            break;
                        case SfxSelect.maxcombo: // max combo
                            Util_HitSFXSource.s_instance.m_rewardClip = hitAudio;
                            MelonLogger.Msg("Max combo set");
                            break;
                        case SfxSelect.wall: // wall hit
                            Util_HitSFXSource.s_instance.m_failClipWall = hitAudio;
                            MelonLogger.Msg("Wall hit set");
                            break;
                        /* from ExtraSFXAudioCOntroller (all audioclips are private)
                        backButtonClipDefault
                        homeMenuBTNHover
                        homeMenuBTNStay
                        timeCounterClip
                        multiplayerRiderJoinAnnouncer
                        multiplayerCounterReadyAnnouncer
                        multiplayerPositionsAnnouncer
                        homeMenuElectricity
                         */
                        case SfxSelect.buttonclick: // menu click, click seems to be detected instead of hovering sometimes
                            FieldInfo clickClip = xsfx.GetField("buttonClickClip", BindingFlags.NonPublic | BindingFlags.Instance);
                            clickClip.SetValue(xsfxInstance, hitAudio);
                            MelonLogger.Msg("Menu click set");
                            break;
                        case SfxSelect.buttonhover: // menu hover
                            
                            FieldInfo hoverClip = xsfx.GetField("buttonHoverClip", BindingFlags.NonPublic | BindingFlags.Instance);
                            hoverClip.SetValue(xsfxInstance, hitAudio);
                            MelonLogger.Msg("Menu hover set");
                            break;
                        case SfxSelect.gameover: // game over
                            Type gcm = typeof(GameControlManager);
                            FieldInfo gameoverClip = gcm.GetField("m_GameOverClip", BindingFlags.NonPublic | BindingFlags.Instance);
                            gameoverClip.SetValue(GameControlManager.s_instance, hitAudio);
                            MelonLogger.Msg("Game over set");
                            break;
                        case SfxSelect.resultbgm: // constant background sounds
                            Type gssc = typeof(Game_ScoreSceneController);
                            FieldInfo resultClip = gssc.GetField("m_NormalClip", BindingFlags.NonPublic | BindingFlags.Instance);
                            resultClip.SetValue(Game_ScoreSceneController.s_instance, hitAudio);
                            MelonLogger.Msg("Result bgm set");
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Util_HitSFXSource), "Awake")]
        public static class OverwriteHitSFX
        {
            private static void Postfix()
            {
                // Setting is SFXEnabled in settings.bin
                // SynthFinder
                // add new sound files to cache 
                // scheint aber noch falscher cache zu sein 
                // müsste eher sfxcache sein falls es das gibt
                // List m_sfxAudioClips (CustomizeGraphy)
                MelonLogger.Msg("Custom sound HitSFX");
                var cs_instance = new CustomSound();
                MelonCoroutines.Start(cs_instance.GetAudioClip(hitFilePath, SfxSelect.hit));
                MelonCoroutines.Start(cs_instance.GetAudioClip(missFilePath, SfxSelect.miss));
                //MelonCoroutines.Start(instance.GetAudioClip(railFilePath, SfxSelect.rail));
                MelonCoroutines.Start(cs_instance.GetAudioClip(specialFilePath, SfxSelect.special));
                MelonCoroutines.Start(cs_instance.GetAudioClip(specialpassFilePath, SfxSelect.specialpass));
                MelonCoroutines.Start(cs_instance.GetAudioClip(specialfailFilePath, SfxSelect.specialfail));
                MelonCoroutines.Start(cs_instance.GetAudioClip(maxcomboFilePath, SfxSelect.maxcombo));
                MelonCoroutines.Start(cs_instance.GetAudioClip(wallFilePath, SfxSelect.wall));
            }
        }
        [HarmonyPatch(typeof(ExtraSFXAudioController), "Awake")]
        public static class OverwriteXSFX
        {
            private static void Postfix()
            {
                MelonLogger.Msg("Custom sound XSFX");
                var cs_instance = new CustomSound();
                MelonCoroutines.Start(cs_instance.GetAudioClip(buttonclickFilePath, SfxSelect.buttonclick));
                MelonCoroutines.Start(cs_instance.GetAudioClip(buttonhoverFilePath, SfxSelect.buttonhover));
            }
        }
        [HarmonyPatch(typeof(GameControlManager), "Awake")]
        public static class OverwriteGCM
        {
            private static void Postfix()
            {
                /* from GameCOntrolManager (all audioclips are private)
                        m_GameOverClip
                        m_SpecialStartClip
                        m_SpecialEndClip
                        m_SpecialFailCip
                        m_CounterClip
                         */
                var cs_instance = new CustomSound();
                MelonLogger.Msg("Custom sound GCM");
                MelonCoroutines.Start(cs_instance.GetAudioClip(gameoverFilePath, SfxSelect.gameover));
            }
        }
        [HarmonyPatch(typeof(Game_ScoreSceneController), "Awake")]
        public static class OverwriteGSSC
        {
            private static void Postfix()
            {
                // from Game_ScoreSceneController (all audioclips are private)
                var cs_instance = new CustomSound();
                MelonLogger.Msg("Custom sound GSSC");
                MelonCoroutines.Start(cs_instance.GetAudioClip(resultFilePath, SfxSelect.resultbgm));
            }
        }
    }
}
