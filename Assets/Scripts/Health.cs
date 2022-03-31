using UnityEngine;
using UnityEngine.Events;
public class Health : MonoBehaviour
{
    public int maxHealth, health;
    [SerializeField]
    private string[] tags;
    public UnityEvent<int, int> onDamageReceived, onHealReceived;
    public UnityEvent<Health> onHealthDepleted;
    public void TryTakeDamage(int damage, string[] tags)
    {
        bool containsTags = false;
        foreach (var comparedTag in tags)
        {
            foreach (var tag in this.tags)
            {
                if (comparedTag != tag) continue; 
                containsTags = true; 
                break;
            }
            if (containsTags) break;
        }
        if (!containsTags) return;
        health -= damage;
        if (health < 1) onHealthDepleted.Invoke(this);
        onDamageReceived?.Invoke(damage, health);
    }
    public void StartHealth(UnityAction<int, int> onDamageReceived, UnityAction<int, int> onHealReceived, UnityAction<Health> onHealthDepleted, int maxHealth)
    {
        this.onDamageReceived.AddListener(onDamageReceived);
        this.onHealReceived.AddListener(onHealReceived);
        this.onHealthDepleted.AddListener(onHealthDepleted);
        this.maxHealth = maxHealth;
        health = maxHealth;
    }
}
