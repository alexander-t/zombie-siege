using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Firing
{
    public class GenericObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int objectsToPreallocate = 64;

        private List<GameObject> objects;

        void Awake()
        {
            // Make sure that the objects aren't parented to some strange transform
            transform.position = Vector3.zero;

            objects = new List<GameObject>(objectsToPreallocate);

            GameObject go;
            for (int i = 0; i < objectsToPreallocate; i++)
            {
                go = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
                go.SetActive(false);
                objects.Add(go);
            }
        }

        public GameObject FindUnusedObject()
        {
            for (int i = 0; i < objectsToPreallocate; i++)
            {
                if (!objects[i].activeInHierarchy)
                {
                    return objects[i];
                }
            }
            return null;
        }

        public void Reclaim(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }
    }
}
