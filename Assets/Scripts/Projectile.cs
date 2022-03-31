using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class Projectile : MonoBehaviour
{
    private Vector3 oldPosition, direction;
    private float lifeTime, speed, radius, maxLifeTime;
    [SerializeField]
    private LayerMask layerMask;
    private UnityAction<Health[]> onCollidersHit;
    public void LaunchProjectile(Vector3 direction, float speed, float radius, float maxLifeTime, LayerMask layerMask,UnityAction<Health[]> onCollidersHit)
    {
        this.speed = speed;
        this.radius = radius;
        this.maxLifeTime = maxLifeTime;
        this.direction = direction;
        this.onCollidersHit = onCollidersHit;
        this.layerMask = layerMask;
    }
    private void OnEnable()
    {
        lifeTime = 0;
        oldPosition = transform.position;
        Game.Instance.onStateChanged += onStateChanged;
    }

    private void OnDisable()
    {
        if (Game.Instance == null) return;
        Game.Instance.onStateChanged -= onStateChanged;
    }
    private void onStateChanged(Game.mState state)
    {
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        lifeTime += Time.deltaTime;
        transform.position = transform.position + direction.normalized * speed * Time.fixedDeltaTime;
        RaycastHit hit;
        if (Physics.Raycast(new Ray(oldPosition, transform.position - oldPosition), out hit, Vector3.Distance(transform.position, oldPosition), layerMask))
        {
            Collider[] colliders = Physics.OverlapSphere(hit.point, radius);
            var healthsOnRadius = new List<Health>();
            foreach (var collider in colliders)
            {
                Health health;
                if (!collider.TryGetComponent(out health)) continue;
                healthsOnRadius.Add(health);
            }
            onCollidersHit.Invoke(healthsOnRadius.ToArray());
            Destroy(gameObject);
        }
        if (lifeTime >= maxLifeTime)
        {
            Destroy(gameObject);
        }
        oldPosition = transform.position;
    }
}
