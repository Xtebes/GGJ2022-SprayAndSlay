using UnityEngine;
public class YouSpinMeRightRoundBabyRightRound : MonoBehaviour
{
    [SerializeField]
    private int degreesPerSecond;
    void Update()
    {
        transform.Rotate( new Vector3(0, degreesPerSecond * Time.deltaTime, 0));
    }
}
