using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
public class UI : MonoBehaviour
{
    [SerializeField]
    private Image fade;
    Tween fadeTween;
    public Slider healthSlider;
    public Slider catchSlider;
    public GameObject winScreen;
    public Image crossHair;
    public Sprite[] crossHairSprite;
    public TextMeshProUGUI cooldownTimer;
    public System.Action<Game.mState> onFadeMid;
    public AudioSource yaySource;
    public Tween DoFadeInOut(Color startColor, Color endColor, float duration, int loopAmount, bool stateOnFinish = false, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.InCubic)
    {
        fade.gameObject.SetActive(true);
        fadeTween.Kill();
        fade.color = startColor;
        crossHair.sprite = crossHairSprite[(int)Game.Instance.state];
        fadeTween = fade.DOColor(endColor, duration / loopAmount).SetLoops(loopAmount, loopType).SetEase(ease);
        fadeTween.onComplete = () => fade.gameObject.SetActive(stateOnFinish);
        StartCoroutine(WaitForBlank(Game.Instance.state));
        return fadeTween;
    }
    private IEnumerator WaitForBlank(Game.mState state)
    {
        yield return new WaitForSeconds(0.5f);
        onFadeMid.Invoke(state);
    }
}
