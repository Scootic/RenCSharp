using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Object_Pooling
{
    private static Dictionary<GameObject, Pool> _thePools = new Dictionary<GameObject, Pool>();

    public static GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation)
    {
        if(_thePools == null)
        {
            _thePools = new Dictionary<GameObject, Pool>();
        }

        if(_thePools.ContainsKey(obj) == false)
        {
            _thePools.Add(obj, new Pool(obj));
        }

        return _thePools[obj].Spawn(position, rotation);
    }

    public static IEnumerator DespawnOverTime(GameObject obj, float timeToDespawn)
    {
        float t = 0;
        while (t <= timeToDespawn && obj.activeInHierarchy)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if(obj.activeInHierarchy) Despawn(obj);
    }

    public static void Despawn(GameObject obj)
    {
        if(obj.GetComponent<PoolMember>() == null)
        {
            GameObject.Destroy(obj);
        }
        else
        {
            obj.GetComponent<PoolMember>().MyPool.Despawn(obj);
        }
    }

    private class Pool
    {
        private GameObject _prefab;
        private Stack<GameObject> _inactiveObjects;
        private int _count = 0;

        public Pool(GameObject prefab)
        {
            _prefab = prefab;
            _inactiveObjects = new Stack<GameObject>();
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject obj = null;

            if(_inactiveObjects.Count == 0)
            {
                obj = GameObject.Instantiate(_prefab, position, rotation);
                obj.name = _prefab.name + "_" + _count;
                obj.AddComponent<PoolMember>().MyPool = this;
                _count++;
            }
            else
            {
                obj = _inactiveObjects.Pop();
            }

            if(obj == null)
            {
                return Spawn(position, rotation);
            }

            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            return obj;
        }

        public void Despawn(GameObject obj)
        {
            obj.SetActive(false);
            _inactiveObjects.Push(obj);
        }
    }

    private class PoolMember : MonoBehaviour
    {
        private Pool _myPoo;
        public Pool MyPool { get { return _myPoo; } set { _myPoo = value; } }
    }
}
