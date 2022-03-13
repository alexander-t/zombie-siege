using UnityEngine;

public class BorderImageSpawner : MonoBehaviour
{
    [SerializeField] private GameObject borderPrefab;

    void Start()
    {
        for (int i = 0; i < 7; i++)
        {
            Instantiate(borderPrefab, transform.position + transform.rotation * Vector3.right * 40 * i, transform.rotation);
        }
    }

}
