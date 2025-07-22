using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace XaviGames.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [field: SerializeField]
        [field: Range(0f, 1f)]
        public float MasterVolume { get; private set; } = 1f;

        public UnityAction<float> OnMasterVolumeChanged;

        public static AudioManager Instance { get; private set; }

        public void OnValidate()
        {
            MasterVolume = Mathf.Clamp(MasterVolume, 0f, 1f);
            OnMasterVolumeChanged?.Invoke(MasterVolume);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
