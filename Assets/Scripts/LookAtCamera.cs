using UnityEngine;
public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.Rotate(new Vector3(0, 180, 0));
    }
}
