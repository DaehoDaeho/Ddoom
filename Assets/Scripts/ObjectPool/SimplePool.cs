using UnityEngine;
using System.Collections.Generic;

public class SimplePool : MonoBehaviour
{
    [SerializeField] private PooledObject prefab;
    [SerializeField] private int warmCount = 10;

    private Queue<PooledObject> items = new Queue<PooledObject>();

    private void Awake()
    {
        for(int i=0; i<warmCount; ++i)
        {
            CreateOne();
        }
    }

    public void Return(PooledObject obj)
    {
        if (obj != null)
        {
            obj.gameObject.SetActive(false);
            items.Enqueue(obj);
        }    
    }

    public PooledObject Rent(Vector3 position, Quaternion rotation, float lifetimeSeconds)
    {
        if(items.Count > 0)
        {
            PooledObject obj = items.Dequeue();
            if (obj != null)
            {
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.Setup(this, lifetimeSeconds);
                return obj;
            }
        }

        PooledObject obj2 = CreateOne();
        if(obj2 != null)
        {
            obj2.transform.SetPositionAndRotation(position, rotation);
            obj2.Setup(this, lifetimeSeconds);
        }

        return obj2;
    }

    PooledObject CreateOne()
    {
        PooledObject obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        items.Enqueue(obj);
        return obj;
    }
}
