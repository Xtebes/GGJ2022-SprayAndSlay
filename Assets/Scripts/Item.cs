using UnityEngine;
using UnityEngine.Events;
public class Item : MonoBehaviour
{
    public UnityEvent<Item, PlayerController> onItemPicked;
    private void Start()
    {
        onItemPicked.AddListener((item, player) => Destroy(gameObject));
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player;
        if (!other.TryGetComponent(out player)) return;
        onItemPicked.Invoke(this, player);
    }
}
