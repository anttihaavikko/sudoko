using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public static class DevKey
    {
        public static bool Down(KeyCode key, bool buildAlso = false)
        {
            return (Application.isEditor || buildAlso) && Input.GetKeyDown(key);
        }
        
        public static bool Held(KeyCode key, bool buildAlso = false)
        {
            return (Application.isEditor || buildAlso) && Input.GetKey(key);
        }
        
        public static bool Up(KeyCode key, bool buildAlso = false)
        {
            return (Application.isEditor || buildAlso) && Input.GetKeyUp(key);
        }
    }
}