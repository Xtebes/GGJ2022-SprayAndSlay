using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class CharAnimation : MonoBehaviour
{
    public Sprite[][] walkingAnimationFrames, standingAnimationFrames;
    public Sprite[] currentPlayingAnimation;
    private Image charImage;
    [SerializeField]
    private int fps, frameCount;
    private int currentFrame;
    void Start()
    {
        currentPlayingAnimation = standingAnimationFrames[(int)Game.Instance.state];
        StartCoroutine(AnimationCycle());
        Game.Instance.onStateChanged += OnStateChanged;
    }
    private void OnStateChanged(Game.mState state)
    {
        currentPlayingAnimation = standingAnimationFrames[(int)state];
    }
    private IEnumerator AnimationCycle()
    {
        while (true)
        {
            float frameTime = 1 / fps;
            charImage.sprite = currentPlayingAnimation[currentFrame];
            currentFrame = currentFrame + 1 % frameCount;
            yield return new WaitForSeconds(frameTime);          
        }
    }
}
