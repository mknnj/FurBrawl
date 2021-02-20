using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Initial
{
    public class TimerScene : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float time = 0;

        private void Start()
        {
            //StartCoroutine(TimerCoroutine());
        }


        private void Update()
        {
            if (time < 2)
            {
                Color color = image.color;
                color.a = 1 - (time / 2);
                image.color = color;
                time += Time.deltaTime;
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        IEnumerator TimerCoroutine()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
