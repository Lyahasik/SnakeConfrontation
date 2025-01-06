using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/// <summary>
/// Its important to identify the platform the game is running on. 
/// When game is running on PC, Mac or on a browser (WebGL), we need to enable mouse + keyboard controls, 
/// but if the game is running on an iPhone or an Android phone, we can only enable virtual joystick.
/// 
/// There are two ways to detect the platfrom. Builtin method and external JSLIB method.
/// Both ways are covered below.
/// </summary>

namespace SnakeWarzIO
{
    public class PlatformDetector : MonoBehaviour
    {
        public static PlatformDetector instance;
        public Text debugText;

        #if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern bool IsMobile();
        #endif

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            print("<b>" + "#1 ==> isRunningOnMobilePlatform: " + IsMobilePlatform() + "</b>");
            debugText.text += "" + "#1 ==> [BuiltIn] isRunningOnMobilePlatform: " + IsMobilePlatform() + "\r\n";

            print("<b>" + "#2 ==> isRunningOnMobileExternal: " + IsMobilePlatformExternal() + "</b>");
            debugText.text += "" + "#2 ==> [JsLib] isRunningOnMobileExternal: " + IsMobilePlatformExternal() + "\r\n";
        }

        public bool IsMobilePlatform()
        {
            return Application.isMobilePlatform;
        }

        public bool IsMobilePlatformExternal()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
                        return IsMobile();
            #endif

            return false;
        }
    }
}