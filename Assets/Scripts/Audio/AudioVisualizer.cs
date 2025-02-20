using UnityEngine;

namespace HA
{
    public class AudioVisualizer : MonoBehaviour
    {
        private GameObject[] _visualizers;

        [SerializeField] private SpriteRenderer backgroundSprite;
        [SerializeField] private GameObject imagePrefab;
        public float maxScale;

        // Start is called before the first frame update
        private void Start()
        {
            _visualizers = new GameObject[AudioMaster.Samples.Length];
            for (int i = 0; i < _visualizers.Length; i++)
            {
                GameObject objToSpawn = (GameObject)Instantiate(imagePrefab);
                objToSpawn.transform.position = this.transform.position;
                objToSpawn.transform.parent = this.transform;
                objToSpawn.name = "BandVisualizer " + i;
                objToSpawn.transform.position = new Vector3(i, 0, 0);
                _visualizers[i] = objToSpawn;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            Vector3 currentColor = new Vector4(backgroundSprite.color.r, backgroundSprite.color.g, backgroundSprite.color.b, 255);
            Color randomColor = Random.ColorHSV(1f, 1f, 1f, 1f);
            Vector4 nextColor = new Vector4(randomColor.r, randomColor.g, randomColor.b, 255);
            for (int i = 0; i < _visualizers.Length; i++)
            {
                _visualizers[i].transform.localScale =
                Vector3.Lerp(_visualizers[i].transform.localScale,
                new Vector3(1, (AudioMaster.Samples[i] * maxScale) + 2, 1), AudioMaster.LerpTime * Time.deltaTime);

                backgroundSprite.color = Vector4.Lerp(currentColor, nextColor, AudioMaster.Samples[i] * maxScale + 2 * Time.deltaTime);
            }
        }
    }
}
