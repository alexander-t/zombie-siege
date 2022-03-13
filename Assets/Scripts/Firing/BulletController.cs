using Npc;
using UnityEngine;

namespace Firing
{
    public class BulletController : MonoBehaviour
    {
        public const float MaxRange = 200;

        private float speed = 2000;

        private Rigidbody bulletBody;
        private GenericObjectPool bulletPool;
        private Vector3 startPosition;

        public void Awake()
        {
            bulletBody = GetComponent<Rigidbody>();
            bulletPool = GameObject.Find("World").GetComponent<GenericObjectPool>();
        }

        public void Fire(Vector3 muzzlePosition, Vector3 target)
        {
            startPosition = transform.position = muzzlePosition;
            transform.LookAt(target);
            Vector3 direction = target - muzzlePosition;
            bulletBody.velocity = direction.normalized * Time.fixedDeltaTime * speed;
        }

        public void Update()
        {
            if ((transform.position - startPosition).magnitude > MaxRange) {
                bulletPool.Reclaim(gameObject);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                NpcActions damageAbsorber = other.GetComponentInParent<NpcActions>();
                damageAbsorber.TakeDamage();
                bulletPool.Reclaim(gameObject);
            }
            else if (other.tag == "Terrain")
            {
                bulletPool.Reclaim(gameObject);
            }
        }
    }
}