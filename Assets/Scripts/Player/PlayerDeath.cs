using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerDeath : MonoBehaviour
    {
        [SerializeField] ParticleSystem respawnParticleSystem;
        [SerializeField] GameObject playerModel;

        private float respawnDuration;
        private float respawnTime;
        private Vector3 originalScale;

        public void Start()
        {
            respawnTime = respawnDuration = respawnParticleSystem.main.duration;
            originalScale = playerModel.transform.localScale;
        }

        public void DieAndRespawn()
        {
            PlayerData.money = 0;
            PlayerData.hp = PlayerData.MaxHp;
            transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.identity;


            respawnTime = 0;
            respawnParticleSystem.Play();
        }

        public void Update()
        {
            if (respawnTime < respawnDuration)
            {
                respawnTime += Time.deltaTime;
                playerModel.transform.localScale = originalScale * respawnTime / respawnDuration;
            }
            else
            {
                playerModel.transform.localScale = originalScale;
                respawnTime = respawnDuration = respawnParticleSystem.main.duration;
            }
        }
    }
}
