using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using MelonLoader;
using UnityEngine;
using VRC.UI.Core;

namespace ReModCE.Loader
{
    public static class BuildInfo
    {
        public const string Name = "ReModLJ";
        public const string Author = "Requi, FenrixTheFox, Xaiver, Potato, Psychloor, LJ";
        public const string Company = null;
        public const string Version = "1.0.0.2";
        public const string DownloadLink = "";
    }

    public class ReLoader : MelonMod
    {
        private Action _onApplicationStart;
        private Action _onUiManagerInit;
        private Action _onUiManagerInitEarly;
        private Action _onFixedUpdate;
        private Action _onUpdate;
        private Action _onGUI;
        private Action _onApplicationQuit;
        private Action _onLateUpdate;
        private Action _onPreferencesLoaded;
        private Action _onPreferencesSaved;

        private Action<int, string> _onSceneWasLoaded;
        private Action<int, string> _onSceneWasInitialized;
        public override void OnApplicationStart()
        {
            var category = MelonPreferences.CreateCategory("ReModCE");

            byte[] bytes = null;

            try
            {
                bytes = File.ReadAllBytes("ReModCE.dll");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to read ReModCE.dll: " + ex);
                return;
            }

            if (bytes == null)
            {
                MelonLogger.Error($"Failed to read file for ReModLJ, unable to load!");
                return;
            }

            Assembly assembly;
            try
            {
                assembly = Assembly.Load(bytes);
            }
            catch (BadImageFormatException e)
            {
                MelonLogger.Error($"Couldn't load specified image: {e}");
                return;
            }

            if (assembly == null)
            {
                MelonLogger.Error("Assembly is null");
                return;
            }

            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }

            var remodClass = types.FirstOrDefault(type => type.Name == "ReModCE");
            if (remodClass == null)
            {
                MelonLogger.Error($"Couldn't find ReModCE class in assembly. ReModCE won't load.");
                return;
            }

            var methods = remodClass.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var m in methods)
            {
                var parameters = m.GetParameters();
                switch (m.Name)
                {
                    case nameof(OnApplicationStart) when parameters.Length == 0:
                        _onApplicationStart = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnApplicationQuit) when parameters.Length == 0:
                        _onApplicationQuit = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnSceneWasLoaded) when parameters.Length == 2 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == typeof(string):
                        _onSceneWasLoaded = (Action<int, string>)Delegate.CreateDelegate(typeof(Action<int, string>), m);
                        break;
                    case nameof(OnSceneWasInitialized) when parameters.Length == 2 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == typeof(string):
                        _onSceneWasInitialized = (Action<int, string>)Delegate.CreateDelegate(typeof(Action<int, string>), m);
                        break;
                    case nameof(OnUpdate) when parameters.Length == 0:
                        _onUpdate = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnUiManagerInit) when parameters.Length == 0:
                        _onUiManagerInit = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnUiManagerInitEarly) when parameters.Length == 0:
                        _onUiManagerInitEarly = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnGUI) when parameters.Length == 0:
                        _onGUI = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnLateUpdate) when parameters.Length == 0:
                        _onLateUpdate = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnFixedUpdate) when parameters.Length == 0:
                        _onFixedUpdate = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnPreferencesLoaded) when parameters.Length == 0:
                        _onPreferencesLoaded = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                    case nameof(OnPreferencesSaved) when parameters.Length == 0:
                        _onPreferencesSaved = (Action)Delegate.CreateDelegate(typeof(Action), m);
                        break;
                }
            }

            MelonCoroutines.Start(WaitForUiManager());
            _onApplicationStart();
        }

        public void OnUiManagerInit()
        {
            _onUiManagerInit();
        }

        public void OnUiManagerInitEarly()
        {
            _onUiManagerInitEarly();
        }

        public override void OnFixedUpdate()
        {
            _onFixedUpdate();
        }

        public override void OnUpdate()
        {
            _onUpdate();
        }

        public override void OnLateUpdate()
        {
            _onLateUpdate();
        }

        public override void OnGUI()
        {
            _onGUI();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            _onSceneWasLoaded(buildIndex, sceneName);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            _onSceneWasInitialized(buildIndex, sceneName);
        }

        public override void OnApplicationQuit()
        {
            _onApplicationQuit();
        }

        public override void OnPreferencesLoaded()
        {
            _onPreferencesLoaded();
        }

        public override void OnPreferencesSaved()
        {
            _onPreferencesSaved();
        }

        private IEnumerator WaitForUiManager()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null) yield return null;
            OnUiManagerInitEarly();

            while (UIManager.field_Private_Static_UIManager_0 == null) yield return null;
            while (GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true) == null) yield return null;

            OnUiManagerInit();
        }

        private static string ComputeHash(HashAlgorithm sha256, byte[] data)
        {
            var bytes = sha256.ComputeHash(data);
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
