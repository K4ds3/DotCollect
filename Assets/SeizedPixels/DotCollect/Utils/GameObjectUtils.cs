using UnityEngine;

namespace SeizedPixels.DotCollect.Utils
{
    public static class GameObjectUtils
    {
        public static void SetGameObjects(bool active, params GameObject[] gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.SetActive(active);
            }
        }
        
        public static void DisableGameObjects(params MonoBehaviour[] gameObjects)
        {
            foreach (var monoBehaviour in gameObjects)
            {
                SetGameObjects(false, monoBehaviour.gameObject);
            }
        }
        
        public static void EnableGameObjects(params MonoBehaviour[] gameObjects)
        {
            foreach (var monoBehaviour in gameObjects)
            {
                SetGameObjects(true, monoBehaviour.gameObject);
            }
        }
    }
}