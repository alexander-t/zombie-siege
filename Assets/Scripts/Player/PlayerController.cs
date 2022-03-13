using Firing;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public GameObject muzzle;
        public CharacterController characterController;
        public PlayerSounds playerSounds;
        public Image delaySpinner;

        [Header("Movement settings")]
        public float horizontalRotationSpeed = 3;
        public float walkingSpeed = 10;
        public float timeToPickup = 0.75f;

        private const float GRAVITY = -9.81f;
        private float velocityY = 0;

        private AudioSource audioSource;
        private GenericObjectPool bulletObjectPool;
        private GenericObjectPool woodObjectPool;
        private PlayerDeath playerDeath;


        private GameObject pickupItem;
        private bool pickingUp = false;
        private float pickUpTime = 0;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            playerDeath = GetComponent<PlayerDeath>();
            bulletObjectPool = GameObject.Find("World").GetComponent<GenericObjectPool>();
            woodObjectPool = GameObject.Find("Resource spawner").GetComponent<GenericObjectPool>();
        }

        void Update()
        {
            if (pickingUp)
            {
                pickUpTime += Time.deltaTime;
                if (pickUpTime < timeToPickup)
                {
                    delaySpinner.fillAmount = pickUpTime / timeToPickup;
                    return;
                }
                pickingUp = false;
                delaySpinner.fillAmount = 0;
            }

            velocityY += GRAVITY;

            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += transform.forward * -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += transform.right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += transform.right * -1;
            }
            if (Input.GetKey(KeyCode.E)) {
                if (pickupItem != null) {
                    woodObjectPool.Reclaim(pickupItem.transform.parent.gameObject);
                    pickingUp = true;
                    pickUpTime = 0;
                    pickupItem = null;
                    delaySpinner.fillAmount = 0;
                    
                    PlayerData.wood++;
                    audioSource.clip = playerSounds.woodCrack;
                    audioSource.Play();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit))
                {
                    GameObject bullet = bulletObjectPool.FindUnusedObject();
                    bullet.SetActive(true);
                    BulletController bulletController = bullet.GetComponent<BulletController>();
                    bulletController.Fire(muzzle.transform.position, raycastHit.point);
                    audioSource.clip = playerSounds.gunshot;
                    audioSource.Play();
                }
            }
            if (Input.GetMouseButton(1))
            {
                transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * horizontalRotationSpeed, Space.Self);
            }

            Vector3 velocity = direction * walkingSpeed;
            velocity.y = velocityY;
            characterController.Move(velocity * Time.deltaTime);

            if (characterController.isGrounded)
            {
                velocityY = 0;
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "DamageZone")
            {
                PlayerData.hp -= 10;
                if (PlayerData.hp <= 0)
                {
                    playerDeath.DieAndRespawn();
                }
            }
            else if (other.tag == "Pickup")
            {
                pickupItem = other.gameObject;
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Pickup")
            {
                pickupItem = null;
            }
        }
    }
}