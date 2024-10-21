using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private float aimDistance = 10f;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private bool isAimingAtNPC;

    private void Awake() {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;

            if (Physics.Raycast(ray, out RaycastHit hit, aimDistance, npcLayer))
            {
                if (hit.collider.CompareTag("NPC"))
                {
                    if (!isAimingAtNPC)
                    {
                        isAimingAtNPC = true;
                        hit.collider.GetComponent<AI_Agent>().OnPlayerAiming(true);
                    }
                }
            }

            else if (isAimingAtNPC)
            {
                isAimingAtNPC = false;
                hit.collider.GetComponent<AI_Agent>().OnPlayerAiming(false);
            }
        }

        if (starterAssetsInputs.aim) {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        } else {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 13f));
        }

        if (starterAssetsInputs.shoot && !starterAssetsInputs.aim) {
            starterAssetsInputs.shoot = false;
        }

        if (starterAssetsInputs.shoot && starterAssetsInputs.aim) {
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            starterAssetsInputs.shoot = false;
        }

    }
}
