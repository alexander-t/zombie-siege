using UnityEngine;
using Player;
using System;

namespace Npc
{
    [RequireComponent(typeof(NpcData))]
    public class NpcActions : MonoBehaviour
    {
        [Header("Healthbar & level")]
        [SerializeField] private GameObject outputCanvas;


        [Header("Death")]
        [Tooltip("Make sure that this references a particle system that's in the prefab; not just another prefab")]
        [SerializeField] private ParticleSystem explosionParticleSystem;
        [SerializeField] private GameObject model;

        private Overlay overlay;
        private NpcData npcData;
        private Animator animator;

        private bool isAlive = true;
        private float hp;
        private float healthbarDisplayCooldown = 5;
        private float healthbarDisplayTime = 0;

        private bool attacking = false;
        private float attackTime;

        void Awake()
        {
            overlay = outputCanvas.GetComponentInChildren<Overlay>();
            npcData = GetComponent<NpcData>();
            animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            hp = npcData.Npc.MaxHp;
            overlay.Elite = npcData.Npc.Level > 0;
            overlay.Hide();
        }

        void Update()
        {
            if (attacking)
            {
                attackTime += Time.deltaTime;
                if (attackTime > npcData.Npc.AttackCooldown)
                {
                    attacking = false;
                }
            }

            if (overlay != null)
            {
                healthbarDisplayTime += Time.deltaTime;
                if (healthbarDisplayTime >= healthbarDisplayCooldown)
                {
                    overlay.Hide();
                }
            }
        }

        public void Attack()
        {
            if (!attacking)
            {
                attacking = true;
                attackTime = 0;
                animator.Play("zombie attack");
            }
        }

        public void TakeDamage(float damage = 10.0f)
        {
            hp -= damage;
            if (overlay != null)
            {
                overlay.Show();
                overlay.HealthBarPercentage = hp / npcData.Npc.MaxHp;
                healthbarDisplayTime = 0;
            }

            if (isAlive && hp <= 0)
            {
                PlayerData.money += npcData.Npc.KillReward;
                Die();
            }
        }

        public void Die()
        {
            isAlive = false;

            // Don't animate dying characters
            if (animator != null)
            {
                animator.StopPlayback();
                animator.enabled = false;
            }

            model.SetActive(false);
            outputCanvas.SetActive(false);

            explosionParticleSystem.Play();

            Destroy(transform.root.gameObject, 2);
        }
    }
}
