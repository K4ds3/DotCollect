using UnityEngine;

namespace SeizedPixels.DotCollect.Utils
{
    public class MobileUtils
    {
        public static void ShareText(string text)
        {
            #if UNITY_ANDROID
                AndroidUtils.ShareText(text);
            #else
                Debug.Log("Tried to share text: " + text);
            #endif
        }
    }
}