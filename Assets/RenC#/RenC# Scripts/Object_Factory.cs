using Coffee.UIExtensions;
using EXPERIMENTAL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace RenCSharp
{
    /// <summary>
    /// class for spawning and handling gameobjects. mostly to use gameobject.find() less.
    /// </summary>
    public static class Object_Factory 
    {
        private static Dictionary<string, GameObject> activeGameObjects = new Dictionary<string, GameObject>();

        public static bool TryGetObject(string name, out GameObject GO)
        {
            GO = null;
            if (!activeGameObjects.ContainsKey(name)) return false; 
            else
            {
                GO = activeGameObjects[name];
                return true;
            }
        }

        public static GameObject SpawnObject(GameObject prefab, string name, Transform parent = null)
        {
            GameObject t = GameObject.Instantiate(prefab, parent);
            t.name = name;
            if (activeGameObjects.ContainsKey(name))
            {
                RemoveObject(name);
            }

            activeGameObjects.Add(name, t);
            
            return t;
        }
        /// <summary>
        /// Specific method for spawning particle objects, because they have more derangement going on.
        /// </summary>
        /// <param name="overrideParticles">Decides whether or not to bother with the subParticleAsset</param>
        /// <param name="name">The name the new obj will have</param>
        /// <param name="parent">Transform parent.</param>
        /// <param name="UIPartiGUID">Asset GUID for the UIParticle Obj</param>
        /// <param name="subPartiGUID">Asset GUID for the ParticleSystem Obj that will override existing one.</param>
        /// <returns></returns>
        public static async Awaitable<GameObject> SpawnParticleObject(bool overrideParticles, string name, Transform parent, Vector3 localPosition, string UIPartiGUID, string subPartiGUID)
        {
            await Awaitable.MainThreadAsync();
            AsyncOperationHandle UIParticleObjHandle = Addressables.LoadAssetAsync<GameObject>(UIPartiGUID);
            await UIParticleObjHandle.Task;

            if(UIParticleObjHandle.Status == AsyncOperationStatus.Failed) 
            {
                Debug.LogError("Problem loading UIParticleObject: " + UIPartiGUID);
                UIParticleObjHandle.Release();
                return null;
            }

            GameObject uiParticleObj = GameObject.Instantiate(UIParticleObjHandle.Result as GameObject, parent);
            UIParticleObjHandle.Release();
            uiParticleObj.name = name;

            GameObject particlechild = uiParticleObj.transform.GetChild(0).gameObject;
            ParticleSystem ogParticleSystem = particlechild.GetComponent<ParticleSystem>();

            if (overrideParticles)
            {
                AsyncOperationHandle subParticleObjHandle = Addressables.LoadAssetAsync<GameObject>(subPartiGUID);
                await subParticleObjHandle.Task;
                if(subParticleObjHandle.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogError("Problem loading subparticles: " + subPartiGUID + ", still spawning in the UIParticleObj");
                }
                else
                {
                    GameObject subPartyFab = subParticleObjHandle.Result as GameObject;
                    ParticleSystem ps = subPartyFab.GetComponent<ParticleSystem>();
                    ogParticleSystem.CopyParticleSystem(ps);
                    ParticleSystemRenderer psRender = ogParticleSystem.GetComponent<ParticleSystemRenderer>();
                    psRender.CopyValuesThroughReflection(ps.GetComponent<ParticleSystemRenderer>());
                    UIParticle uip = uiParticleObj.GetComponent<UIParticle>();
                    uip.RefreshParticles();
                }
                subParticleObjHandle.Release();
            }

            if (!ogParticleSystem.main.loop) RemoveObjectOverTimeAsync(name, ogParticleSystem.main.duration); //don't await this, just start ts!
            ogParticleSystem.Play();

            uiParticleObj.transform.localPosition = localPosition;

            ParticleToken pt = new(localPosition, UIPartiGUID, subPartiGUID, name, parent.name);
            uiParticleObj.GetComponent<UIParticle_Helper>().SetMyParticleToken = pt;
            Event_Bus.TryFireSingleObjEvent("AddParticleToList", pt);

            activeGameObjects.Add(name, uiParticleObj);
            return uiParticleObj;
        }

        public static async Awaitable<GameObject> SpawnObjectAsync(GameObject prefab, string name, Transform parent = null)
        {
            await Awaitable.MainThreadAsync();
            if (activeGameObjects.ContainsKey(name)) return null;
            GameObject t = GameObject.Instantiate(prefab, parent);
            t.name = name;
            activeGameObjects.Add(name, t);
            return t;
        }

        public static void RemoveObject(string name)
        {
            if (activeGameObjects.ContainsKey(name))
            {
                GameObject t = activeGameObjects[name];
                if (t.TryGetComponent(out IRemovableObject iroh))
                {
                    iroh.OnRemove();
                }
                activeGameObjects.Remove(name);
                GameObject.Destroy(t);
                Debug.Log("Removed object of name: " + name);
            } else Debug.LogWarning("No active gameobject of name: " + name);
        }

        public static IEnumerator RemoveObjectOverTime(string name, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            RemoveObject(name);
        }

        public static async Awaitable RemoveObjectOverTimeAsync(string name, float seconds)
        {
            await Awaitable.WaitForSecondsAsync(seconds);
            RemoveObject(name);
        }

        /// <summary>
        /// Very dangerous. Only should be used OnDisable for SM or sum thang;
        /// </summary>
        public static void ScrubDictionary()
        {
            foreach(KeyValuePair<string, GameObject> kvp in activeGameObjects)
            {
                GameObject.Destroy(kvp.Value);
            }

            activeGameObjects = new Dictionary<string, GameObject>();
        }
    }
}
