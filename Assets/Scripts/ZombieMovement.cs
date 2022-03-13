using Npc;
using UnityEngine;

[RequireComponent(typeof(NpcData))]
public class ZombieMovement : MonoBehaviour
{
    private CharacterController controller;
    private NpcData npcData;
    private NpcActions npcActions;
    private GameObject player;


    private Vector3 target = Vector3.zero;
    private float fallingVelocity = 0;
    private float turningSpeed = 1;
    private bool halted = false;

    #region Memory management
    private Vector3 direction;
    #endregion

    void Awake()
    {
        player = GameObject.Find("Player");
        controller = GetComponent<CharacterController>();
        npcData = GetComponent<NpcData>();
        npcActions = GetComponent<NpcActions>();
    }

    void Start()
    {
        if (player != null)
        {
            transform.LookAt(player.transform.position);
        }
    }

    void Update()
    {
        if (halted)
        {
            return;
        }

        target = player.transform.position;
        direction = (target - transform.position).normalized;

        fallingVelocity = controller.isGrounded ? 0 : fallingVelocity + Physics.gravity.y * Time.deltaTime * 2;
        direction.y = fallingVelocity;

        controller.Move(direction * Time.deltaTime * npcData.Npc.MovementSpeed);

        // Turn towards the player around the y-axis
        Vector3 facingDirection = Vector3.RotateTowards(transform.forward, direction, turningSpeed * Time.deltaTime, 0.0f);
        facingDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(facingDirection);

        if (Vector3.Distance(transform.position, player.transform.position) < 3)
        {
            npcActions.Attack();
        }
    }

    public void Halt()
    {
        halted = true;
    }
}
