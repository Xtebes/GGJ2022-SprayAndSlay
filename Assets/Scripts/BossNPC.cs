using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
public class BossNPC : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Health bossHealth, wallButtonHealth;
    [SerializeField]
    private Image npcImage;
    [SerializeField]
    private float minTimeForFire, maxTimeForFire, projectileSpeed, projectileRadius, projectileLifeTime;
    [SerializeField]
    private int damage;
    private Tween colorTween;
    private Color color;
    [SerializeField]
    private GameObject shotPrefab, gunTip, bossWall, bossWallClosedPosition, bossWallOpenPosition;
    private Coroutine fireCycle;
    [SerializeField]
    private Sprite killedSane, standingSane, killedDoom, standingDoom;
    [SerializeField]
    private LayerMask projectileMask;
    private void OnFadeMidAlive(Game.mState state)
    {
        healthSlider.transform.parent.gameObject.SetActive(!healthSlider.transform.parent.gameObject.activeSelf);
        if (state == Game.mState.sane)
        {
            npcImage.sprite = standingSane;
        }
        else
        {
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
    private void Start()
    {
        bossHealth.onDamageReceived.AddListener(TakeDamage);
        bossHealth.onDamageReceived.AddListener(CheckForWallTime);
        Game.Instance.onStateChanged += OnMentalStateChangedWhileAlive;
        Game.Instance.ui.onFadeMid += OnFadeMidAlive;
        bossHealth.onHealthDepleted.AddListener(health =>
        {
            Kill();
            Game.Instance.ui.onFadeMid += OnFadeMidDead;
            Game.Instance.ui.onFadeMid -= OnFadeMidAlive;
            Game.Instance.onStateChanged -= OnMentalStateChangedWhileAlive;
        });
        color = npcImage.color;
        healthSlider.maxValue = bossHealth.maxHealth;
        healthSlider.value = bossHealth.maxHealth;
    }
    private void Kill()
    {
        var lookAtCamera = GetComponentInChildren<LookAtCamera>();
        Vector3 eulerAngles = lookAtCamera.transform.localEulerAngles;
        lookAtCamera.transform.eulerAngles = Vector3.zero;
        transform.eulerAngles = eulerAngles;
        Destroy(lookAtCamera);
        healthSlider.transform.parent.gameObject.SetActive(false);
        Destroy(bossHealth);
        StopAllCoroutines();
        transform.DORotate(new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z), 1);
    }
    public void CheckForWallTime(int damage, int health)
    {
        if (health >= bossHealth.maxHealth / 2) return;    
        bossHealth.onDamageReceived.RemoveListener(CheckForWallTime);
        bossWall.transform.DOMove(bossWallClosedPosition.transform.position, 2).SetEase(Ease.Linear);
        wallButtonHealth.gameObject.SetActive(true);
        wallButtonHealth.onHealthDepleted.AddListener(health => OpenWall());
    }
    public void OpenWall()
    {
        bossWall.transform.DOMove(bossWallOpenPosition.transform.position, 2);
    }
    private void TakeDamage(int damage, int health)
    {
        npcImage.color = Color.red;
        colorTween.Kill();
        colorTween = npcImage.DOColor(color, 0.3f);
        healthSlider.value = health;
    }
    private void OnMentalStateChangedWhileAlive(Game.mState state)
    {
        switch (state)
        {
            case Game.mState.sane:
                StopCoroutine(fireCycle);
                break;
            case Game.mState.insane:
                fireCycle = StartCoroutine(FireCycle());
                break;
        }
    }
    private void Fire()
    {
        var projectile = Instantiate(shotPrefab, gunTip.transform.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.LaunchProjectile
            ((Game.Instance.player.transform.position - gunTip.transform.position).normalized,
            projectileSpeed, projectileRadius, projectileLifeTime, projectileMask, OnProjectileHit);
    }
    private void OnProjectileHit(Health[] colliders)
    {
        foreach (var health in colliders)
        {
            health.TryTakeDamage(damage, new string[] { "PLAYER", "BOX" });
        }
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
}
