using UnityEngine;

namespace Work.CSH.Code.Managers
{
    public class MonoSingleton<T> : MonoBehaviour where T : new()
    {
        public static T Instance;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = new T();
            }
        }

        private void OnDestroy()
        {

        }
    }
}