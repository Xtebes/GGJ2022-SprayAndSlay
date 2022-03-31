using UnityEngine;
public class SpriteUIOnState : MonoBehaviour
{
    [SerializeField]
    private GameObject saneSprite, insaneSprite;
    void Start()
    {
        Game.Instance.ui.onFadeMid += state =>
        {
            if (state == Game.mState.sane)
            {
                saneSprite.gameObject.SetActive(true);
                insaneSprite.SetActive(false);
                return;
            }
            saneSprite.gameObject.SetActive(false);
            insaneSprite.SetActive(true);
        };
    }
}
