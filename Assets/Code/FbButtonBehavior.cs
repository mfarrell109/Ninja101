using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FbButtonBehavior : MonoBehaviour {

    private Image image;
    private Button button;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        image.enabled = false;
        button.enabled = false;

        StartCoroutine(Show());
	}

    IEnumerator Show()
    {
        yield return new WaitForSeconds(5f);

        image.enabled = true;
        button.enabled = true;

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        if (image.enabled)
        {
            yield return new WaitForSeconds(1f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        image.enabled = !image.enabled;

        StartCoroutine(Blink());
    }
}
