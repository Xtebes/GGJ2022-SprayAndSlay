using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    private float _catchMeterCurrentValue;
    public float catchMeterCurrentValue
    {
        get
        {
            return _catchMeterCurrentValue;
        }
        set
        {
            float clampedValue = Mathf.Max(value, 0);
            _catchMeterCurrentValue = clampedValue;
            Game.Instance.ui.catchSlider.value = clampedValue;
        }
    }
    private System.Action onMouseDown;
    private System.Action[] onMouseDowns = new System.Action[2];
    [SerializeField]
    private new Rigidbody rigidbody;
    [SerializeField]
    private Transform cameraHolder;
    [SerializeField]
    private Health health;
    [SerializeField]
    private float speed, jumpForce, weaponFireRate, feetDistance, timeBetweenSteps, destructionStateTimer, reductionPerSecond, catchMeterMaxValue;
    private float iWeaponFireRate, iTimeBetweenSteps = 0;
    [SerializeField]
    private Vector3 feetArea;
    private Vector3 positionToBe, movementDirection;
    private Vector3 half = new Vector3(0.5f, 0.5f);
    [SerializeField]
    private LayerMask fireLayerMask, jumpDetectionLayerMask;
    private bool isJumping;
    private bool isOnFloor
    {
        get
        {
            return Physics.BoxCast(transform.position, feetArea / 2, -transform.up, transform.rotation, feetDistance, jumpDetectionLayerMask);
        }
    }
    public Image weaponImage;
    [SerializeField]
    private Sprite weaponSprite;
    [SerializeField]
    private Sprite glassCleanerSprite;
    [SerializeField]
    private Sprite[] weaponFireSprite;
    [HideInInspector]
    public float stateCooldownTimer;
    [SerializeField]
    private AudioSource shotgunSoundSource, stepSoundSource;
    [SerializeField]
    private AudioClip[] stepWalkSoundClips, stepRunSoundClips;
    private Coroutine fireAnime;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Game.Instance.ui.onFadeMid += OnFadeMid;
        onMouseDown = null;
        onMouseDowns[0] = null;
        onMouseDowns[1] = OnMouseDownDestruction;
        Game.Instance.ui.DoFadeInOut(Color.black, Color.clear, 1, 1);
        Game.Instance.ui.healthSlider.maxValue = health.maxHealth;
        Game.Instance.ui.healthSlider.value = health.health;
        Game.Instance.ui.catchSlider.maxValue = catchMeterMaxValue;
        catchMeterCurrentValue = 0;
        health.onDamageReceived.AddListener((damageReceived, health) => Game.Instance.ui.healthSlider.value = health);
        health.onHealthDepleted.AddListener((health)=> Game.Instance.RestartGame());
        StartCoroutine(JumpCycle());
    }
    void Update()
    {
        if (catchMeterCurrentValue > catchMeterMaxValue) Game.Instance.RestartGame();
        iWeaponFireRate -= Time.deltaTime;
        stateCooldownTimer -= Time.deltaTime;
        iTimeBetweenSteps -= Time.deltaTime;
        catchMeterCurrentValue -= Time.deltaTime * reductionPerSecond;
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDirection = (Camera.main.transform.right * inputVector.x + Camera.main.transform.forward * inputVector.y).normalized;
        isJumping = Input.GetKey(KeyCode.Space) && Game.Instance.state == Game.mState.sane;
        Game.Instance.ui.cooldownTimer.text = ((int)Mathf.Max(stateCooldownTimer, 0)).ToString();
        if (Input.GetKeyDown(KeyCode.Mouse1) && stateCooldownTimer <= 0)
        {
            Game.Instance.state = (Game.mState)(((int)Game.Instance.state + 1) % 2);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && iWeaponFireRate <= 0)
        {
            iWeaponFireRate = weaponFireRate;
            onMouseDown?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Game.Instance.QuitToMainMenu();
        }
        if (inputVector.magnitude > 0.2 && iTimeBetweenSteps < 0 && isOnFloor)
        {
            var stepSoundClips = Game.Instance.state == Game.mState.sane ? stepRunSoundClips : stepWalkSoundClips;
            iTimeBetweenSteps = timeBetweenSteps;
            stepSoundSource.clip = stepSoundClips[Random.Range(0, stepSoundClips.Length)];
            stepSoundSource.Play();
        }
    }
    private void FixedUpdate()
    {
        positionToBe = rigidbody.position + new Vector3(movementDirection.x, 0, movementDirection.z) * Time.fixedDeltaTime * speed;
        rigidbody.MovePosition(positionToBe);
    }
    private IEnumerator JumpCycle()
    {
        IEnumerator JumpOnCooldown(float cooldown)
        {
            rigidbody.AddForce(transform.up.normalized * jumpForce, ForceMode.Impulse);
            yield return new WaitForSeconds(cooldown);
        }
        while (true)
        {
            if (isJumping && isOnFloor)
            {
                yield return StartCoroutine(JumpOnCooldown(0.4f));
            }
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator FireAnim()
    {
        shotgunSoundSource.time = 0;
        shotgunSoundSource.Play();
        foreach (var sprite in weaponFireSprite)
        {
            weaponImage.sprite = sprite;
            yield return new WaitForSeconds(0.1f);
        }
        weaponImage.sprite = weaponSprite;
    }
    private void OnFadeMid(Game.mState state)
    {
        switch (state)
        {
            case Game.mState.sane:
                stateCooldownTimer = 5;
                if (fireAnime != null)
                {
                    StopCoroutine(fireAnime);
                }
                weaponImage.sprite = glassCleanerSprite;
                Game.Instance.ui.healthSlider.transform.parent.gameObject.SetActive(false);
                Game.Instance.ui.catchSlider.transform.parent.gameObject.SetActive(true);
                break;
            case Game.mState.insane:
                stateCooldownTimer = destructionStateTimer;
                weaponImage.sprite = weaponSprite;
                Game.Instance.ui.healthSlider.transform.parent.gameObject.SetActive(true);
                Game.Instance.ui.catchSlider.transform.parent.gameObject.SetActive(false);
                break;
        }
        onMouseDown = onMouseDowns[(int)state];
    }
    private void OnMouseDownDestruction()
    {
        if (fireAnime != null)
        {
            StopCoroutine(fireAnime);
        }
        fireAnime = StartCoroutine(FireAnim());
        Ray ray = Camera.main.ViewportPointToRay(half);
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo, 10, fireLayerMask);
        Health health;
        if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out health))
        {
            health.TryTakeDamage(1, new string[] { "NPC", "BOX" });
        }
    }
}
