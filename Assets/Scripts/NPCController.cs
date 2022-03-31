using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
public class NPCController : MonoBehaviour
{
    public float catchMeterIncreasePerSecond, catchRadius;
    public Health health;
    [SerializeField]
    private AIPath path;
    [SerializeField]
    private float 
        maxRangeFromSpawn, 
        movementSpeed, minTimePerMovement, maxTimePerMovement,
        minTimeForFire,maxTimeForFire, 
        projectileSpeed, projectileRadius, projectileLifeTime;
    [SerializeField]
    private int damage;
    private Vector3 spawnPosition;
    [SerializeField]
    private GameObject shotPrefab, gunTip;
    [SerializeField]
    private Image npcImage;
    private Color color;
    private Tween colorTween;
    [SerializeField]
    private Sprite killedSane, standingSane, killedDoom, standingDoom;
    private Coroutine fireCycle, movementCycle, catchCycle;
    [SerializeField]
    private LayerMask projectileMask;
    private Vector3 GetRandomDirection2D()
    {
        return new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;
    }
    private void TakeDamage(int damage, int health)
    {
        npcImage.color = Color.red;
        colorTween.Kill();
        colorTween = npcImage.DOColor(color, 0.3f);
    }
    private void Kill()
    {
        var lookAtCamera = GetComponentInChildren<LookAtCamera>();
        Vector3 eulerAngles = lookAtCamera.transform.localEulerAngles;
        lookAtCamera.transform.eulerAngles = Vector3.zero;
        transform.eulerAngles = eulerAngles;
        Destroy(lookAtCamera);
        Destroy(health);
        Destroy(path);
        StopAllCoroutines();
        npcImage.sprite = killedDoom;
        transform.DORotate(new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z), 1);
    }
    private void Start()
    {
        color = npcImage.color;
        spawnPosition = transform.position;
        path.maxSpeed = movementSpeed;
        health.onDamageReceived.AddListener(TakeDamage);
        Game.Instance.ui.onFadeMid += OnFadeMidAlive;
        health.onHealthDepleted.AddListener(health =>
        {
            Kill();
            Game.Instance.ui.onFadeMid += OnFadeMidDead;
            Game.Instance.ui.onFadeMid -= OnFadeMidAlive;
        });
    }
    private void OnFadeMidAlive(Game.mState state)
    {
        if (state == Game.mState.sane)
        {
            npcImage.sprite = standingSane;
            path.destination = spawnPosition;
            transform.position = spawnPosition;
            path.canMove = false;
            if (movementCycle != null)
            {
                StopCoroutine(movementCycle);
                movementCycle = null;
            }
            if (fireCycle != null)
            {
                StopCoroutine(fireCycle);
                fireCycle = null;
            }
            if (catchCycle == null)
            {
                catchCycle = StartCoroutine(CatchCycle());
            }
        }
        else
        {
            path.canMove = true;
            if (movementCycle == null)
            {
                movementCycle = StartCoroutine(Movement());
            }
            if (fireCycle == null)
            {
                fireCycle = StartCoroutine(FireCycle());
            }        
            if (catchCycle != null)
            {
                StopCoroutine(catchCycle);
                catchCycle = null;
            }            
            npcImage.sprite = standingDoom;
        }
    }
    private void OnFadeMidDead(Game.mState state)
    {
        if (state == Game.mState.sane)
        {
            npcImage.sprite = killedSane;
        }
        else
        {
            npcImage.sprite = killedDoom;
        }
    }
    private IEnumerator CatchCycle()
    {
        while (true)
        {
            if (Vector3.Distance(Game.Instance.player.transform.position, transform.position) < catchRadius)
            {
                Game.Instance.player.catchMeterCurrentValue += catchMeterIncreasePerSecond * Time.deltaTime;
            }
            yield return null;
        }
    }
    private void OnProjectileHit(Health[] colliders)
    {
        foreach (var health in colliders)
        {
            health.TryTakeDamage(damage, new string[] {"PLAYER", "BOX" });
        }
    }
    private void Fire()
    {
        var projectile = Instantiate(shotPrefab, gunTip.transform.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.LaunchProjectile
            ((Game.Instance.player.transform.position - gunTip.transform.position).normalized, 
            projectileSpeed, projectileRadius, projectileLifeTime, projectileMask, OnProjectileHit);
    }
    public IEnumerator FireCycle()
    {
        while (true)
        {
            float timeForFire = Random.Range(minTimeForFire, maxTimeForFire);
            yield return new WaitForSeconds(timeForFire);
            Fire();
        }
    }
    public IEnumerator Movement()
    {
        while (true)
        {
            float timeForMovement = Random.Range(minTimePerMovement, maxTimePerMovement);
            Vector3 destination = spawnPosition + GetRandomDirection2D() * Random.Range(1, maxRangeFromSpawn);
            //charAnimation.currentPlayingAnimation = animation.walkingAnimationFrames[(int)Game.Instance.state];
            yield return GoToDestination(destination);
            //charAnimation.currentPlayingAnimation = animation.standingAnimationFrames[(int)Game.Instance.state];
            yield return new WaitForSeconds(timeForMovement);
        }
    }
    public IEnumerator GoToDestination(Vector3 destination)
    {
        yield return new WaitForFixedUpdate();
        path.destination = destination;
        path.SearchPath();
        while (!path.reachedDestination)
        {
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
}