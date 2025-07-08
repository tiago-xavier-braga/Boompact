using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XaviGames.Ui
{
    public class FpsDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _fpsText;

        private float elapsed;
        private int frames;

        private void Update()
        {
            elapsed += Time.unscaledDeltaTime;
            frames++;

            if (elapsed >= 0.5f)
            {
                float fps = frames / elapsed;
                _fpsText.text = "FPS: " + Mathf.RoundToInt(fps);
                elapsed = 0f;
                frames = 0;
            }
        }
    }
}
