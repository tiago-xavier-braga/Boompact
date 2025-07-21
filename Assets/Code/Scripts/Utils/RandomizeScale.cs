using UnityEngine;
using XaviEssencials.Shared;

namespace XaviGames.Utils
{
    [ExecuteInEditMode]
    public class RandomizeScale : MonoBehaviour
    {
        [SerializeField]
        private bool _randomizeX = true;
        
        [SerializeField]
        private bool _randomizeY = true;
        
        [SerializeField]
        private bool _randomizeZ = true;

        [SerializeField]
        private float _minScale = 0.5f;

        [SerializeField]
        private float _maxScale = 2f;

        [Button("Randomize")]
        public void Randomize()
        {
            Vector3 newScale = transform.localScale;

            if (_randomizeX)
            {
                newScale.x = Random.Range(_minScale, _maxScale);
            }
            if (_randomizeY)
            {
                newScale.y = Random.Range(_minScale, _maxScale);
            }
            if (_randomizeZ)
            {
                newScale.z = Random.Range(_minScale, _maxScale);
            }

            transform.localScale = newScale;
        }
    }
}