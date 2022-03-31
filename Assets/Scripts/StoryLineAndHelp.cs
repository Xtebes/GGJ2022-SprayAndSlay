using UnityEngine;
using TMPro;
public class StoryLineAndHelp : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI controlsText, loreText, helpText;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            controlsText.gameObject.SetActive(!controlsText.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            loreText.gameObject.SetActive(!loreText.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            helpText.gameObject.SetActive(!helpText.gameObject.activeSelf);
        }
    }
}
