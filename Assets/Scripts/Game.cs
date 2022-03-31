using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Game : Singleton<Game>
{
    public enum mState
    {
        sane,
        insane,
    }
    [SerializeField]
    private mState _state;
    public mState state
    {
        get { return _state; }
        set
        {
            if (_state == value) return;
            _state = value;
            onStateChanged.Invoke(value);
            ui.DoFadeInOut(Color.clear, Color.white, 1, 2);
        }
    }
    public Action<mState> onStateChanged;
    public UI ui;
    [SerializeField]
    private GameObject AccessCard1, AccessCard2, accessCard2Pos, executeOfficeGate;
    [SerializeField]
    private Teleport teleport1, teleport2;
    [HideInInspector]
    public PlayerController player;
    public void ChangeSpriteAccordingToMentalState(Image spriteSource, Sprite[] spriteArray)
    {
        spriteSource.sprite = spriteArray[(int)state];
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ShowWinScreen()
    {
        ui.winScreen.SetActive(true);
        ui.yaySource.Play();
        state = mState.sane;
        Invoke("QuitToMainMenu", 5);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    private void Start()
    {
        state = mState.sane;
        player = FindObjectOfType<PlayerController>();
        var item = Instantiate(AccessCard2, accessCard2Pos.transform.position, Quaternion.identity).GetComponent<Item>();
        item.onItemPicked.AddListener((item, player) =>
        {
            teleport2.gameObject.SetActive(true);
            executeOfficeGate.gameObject.SetActive(false);
            StartCoroutine(teleport2.ShowTeleport());
        });

    }
    public void SpawnAccessCard1OnPosition(Transform transform)
    {
        Item item = Instantiate(AccessCard1, transform.position, Quaternion.identity).GetComponent<Item>();
        item.onItemPicked.AddListener((item, player) =>
        {
            teleport1.gameObject.SetActive(true);
            StartCoroutine(teleport1.ShowTeleport());
        });
    }
}
