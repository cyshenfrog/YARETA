using System.IO;
using UnityEditor;
using UnityEngine;

namespace HinputClasses.Internal {
    public class SceneInstance : MonoBehaviour {
        // --------------------
        // SINGLETON PATTERN
        // --------------------
    
        //The instance of SceneInstance. Assigned when first called.
        private static SceneInstance _instance;
        public static SceneInstance instance { 
            get {
                CheckInstance();
                return _instance;
            } 
        }

        public static void CheckInstance() {
            if (_instance != null) return;
            
            GameObject go = new GameObject {name = "Hinput Scene Instance"};
            go.transform.SetParent(Settings.instance.transform);
            _instance = go.AddComponent<SceneInstance>();
        }
    
        #pragma warning disable
        private void Awake () {
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(this);

            Instantiate(Resources.Load("Hinput Player Manager"), Settings.instance.transform);
            FirstTimeSetup();
        }
        #pragma warning restore
    
    
        // --------------------
        // FIRST TIME SETUP
        // --------------------

        private static void FirstTimeSetup() {
            #if UNITY_EDITOR
            if (File.Exists("Assets/Hinput/Settings/version") == false) {
                ErrorMessages.FailedSetupError();
                return;
            }
	        
            if (File.ReadAllText("Assets/Hinput/Settings/version") == Application.unityVersion) return;

            try {
                AssetDatabase.ImportAsset("Assets/Hinput/Prefabs/Hinput Player.prefab");
                AssetDatabase.ImportAsset("Assets/Hinput/Resources/Hinput Player Manager.prefab");
                AssetDatabase.ImportAsset("Assets/Hinput/Hinput Settings.prefab");
            } catch {
                ErrorMessages.FailedSetupError();
                return;
            }
	        
            File.WriteAllText("Assets/Hinput/Settings/version", Application.unityVersion);
            #endif
        }
    
    
        // --------------------
        // ON APPLICATION QUIT
        // --------------------
    
        public void OnApplicationQuit() => Hinput.anyGamepad.StopVibration();
    }
}