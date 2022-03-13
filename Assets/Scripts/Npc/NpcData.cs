using UnityEngine;

namespace Npc
{
    abstract public class NonPlayerCharacter
    {

        private int maxHp;
        private float movementSpeed;
        private float attackCooldown;
        private int killReward;
        private int level;

        public NonPlayerCharacter(int hitPoints, float movementSpeed, float attackCooldown, int killReward, int level = 0)
        {
            this.maxHp = hitPoints;
            this.movementSpeed = movementSpeed;
            this.attackCooldown = attackCooldown;
            this.killReward = killReward;
            this.level = level;
        }

        public int MaxHp
        {
            get { return maxHp; }
        }

        public float MovementSpeed
        {
            get { return movementSpeed; }
        }

        public float AttackCooldown
        {
            get { return attackCooldown; }
        }

        public int KillReward
        {
            get { return killReward; }
        }

        public int Level
        {
            get { return level; }
        }
    }

    public class StandardZombie : NonPlayerCharacter
    {
        public StandardZombie() : base(100, 1, 3, 10, 0) { }
    }

    public class StandardZombieElite : NonPlayerCharacter
    {
        public StandardZombieElite() : base(150, 3, 2, 25, 1) { }
    }


    public class NpcData : MonoBehaviour
    {
        public static NonPlayerCharacter StandardZombie = new StandardZombie();
        public static NonPlayerCharacter StandardZombieElite = new StandardZombieElite();

        private NonPlayerCharacter npc = StandardZombie;
        public NonPlayerCharacter Npc
        {
            get => npc;
            set => npc = value;
        }

    }
}
