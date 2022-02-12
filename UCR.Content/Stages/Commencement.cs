using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltimateCustomRun.Stages
{
    public static class Commencement
    {
        public static void Changes()
        {
            var stupid = SceneManager.GetActiveScene().name;
            if (stupid == "moon2")
            {
                var a = GameObject.Find("HOLDER: Final Arena");
                a.transform.localScale = new Vector3(1f, 1.3f, 1f);
                a.transform.GetChild(1).gameObject.SetActive(false);
                // disable ramps

                a.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                a.transform.GetChild(2).gameObject.transform.GetChild(2).gameObject.SetActive(true);
                a.transform.GetChild(2).gameObject.transform.GetChild(3).gameObject.SetActive(true);
                a.transform.GetChild(2).gameObject.transform.GetChild(4).gameObject.SetActive(true);
                // enable cool unused props

                a.transform.GetChild(8).gameObject.SetActive(false);
                // disable these dumb hexagons visual clutter :eyebrow:

                a.transform.GetChild(6).gameObject.transform.GetChild(2).gameObject.transform.position = new Vector3(0f, 0.12f, 0f);
                // put the disc a bit higher

                // i dont think any of these work :Happiesttroll:
            }
        }
    }
}
