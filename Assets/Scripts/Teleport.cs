using UnityEngine;
using System.Collections;
using Cinemachine;
public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Transform destination, camStart,camEnd;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    private float coolDown = 3;

    private void Update()
    {
        coolDown -= Time.deltaTime;
    }
    public IEnumerator ShowTeleport()
    {
        var prevCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
        prevCamera.Priority = 0;
        virtualCamera.Priority = 1;
        float stateTimer = Game.Instance.player.stateCooldownTimer;
        bool wasInsane = Game.Instance.state == Game.mState.insane;
        Game.Instance.player.stateCooldownTimer = 100;
        Game.Instance.player.weaponImage.enabled = false;
        Game.Instance.ui.cooldownTimer.gameObject.SetActive(false);
        Game.Instance.ui.healthSlider.transform.parent.gameObject.SetActive(false);
        Game.Instance.state = Game.mState.sane;
        yield return new WaitForSeconds(0.55f);
        Game.Instance.ui.catchSlider.transform.parent.gameObject.SetActive(false);
        Game.Instance.player.stateCooldownTimer = 100;
        yield return new WaitForSeconds(1.5f);
        virtualCamera.Follow = camEnd;
        yield return new WaitForSeconds(3);
        
        virtualCamera.Priority = 0;
        prevCamera.Priority = 1;
        yield return new WaitForSeconds(1);
        if (wasInsane)
        {
            Game.Instance.state = Game.mState.insane;
        }
        yield return new WaitForSeconds(0.55f);
        Game.Instance.ui.cooldownTimer.gameObject.SetActive(true);
        Game.Instance.player.weaponImage.enabled = true;
        Game.Instance.player.stateCooldownTimer = stateTimer;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player;
        if (!other.TryGetComponent(out player) || coolDown > 0) return;
        StartCoroutine(TeleportSequence(player));
        coolDown = 3;
    }
    private IEnumerator TeleportSequence(PlayerController controller)
    {
        Game.Instance.ui.DoFadeInOut(Color.clear, Color.black, 1, 1, true);
        yield return new WaitForSeconds(1);
        controller.transform.position = destination.position;
        Game.Instance.ui.DoFadeInOut(Color.black, Color.clear, 1, 1, false);
    }
}
