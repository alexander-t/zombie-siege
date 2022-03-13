using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] Image eliteImage;
    [SerializeField] Canvas canvas;

    private GameObject playerCamera;
    private bool elite;

    void Awake() {
        playerCamera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        Vector3 facingDirection = playerCamera.transform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, facingDirection, 30, 30);
        transform.rotation = Quaternion.LookRotation(newDirection);
        eliteImage.enabled = Elite;
    }

    public float HealthBarPercentage 
    {
        set => healthBarImage.fillAmount = (value > 0 && value <= 0.05) ? 0.05f : value;
    }

    public bool Elite {
        get => elite;
        set => elite = value;
    }

    public void Show() {
        canvas.enabled = true;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
