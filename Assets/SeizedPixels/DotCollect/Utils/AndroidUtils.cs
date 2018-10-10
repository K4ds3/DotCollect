using UnityEngine;

namespace SeizedPixels.DotCollect.Utils
{
    public static class AndroidUtils
    {
        public static void ShareText(string message, string title)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("putExtra",
                intentClass.GetStatic<AndroidJavaObject>("EXTRA_SUBJECT"), title);
            intentObject.Call<AndroidJavaObject>("putExtra",
                intentClass.GetStatic<AndroidJavaObject>("EXTRA_TITLE"), title);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<AndroidJavaObject>("EXTRA_TEXT"),
                message);
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);
        }

        public static void ShareText(string message)
        {
            ShareText(message, message.Substring(0, 20) + "...");
        }
    }
}