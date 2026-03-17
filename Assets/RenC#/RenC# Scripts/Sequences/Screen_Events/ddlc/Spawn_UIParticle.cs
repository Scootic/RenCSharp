using EXPERIMENTAL;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenCSharp.Sequences
{
    public class Spawn_UIParticle : Screen_Event
    {
        [SerializeField, Tooltip("Local space. Should be same values as actor positions, if that helps.")] private Vector3 spawnPosition = Vector3.zero;
        [SerializeField] private AssetReference fellaToSpawnPrefab;
        [SerializeField, Tooltip("name of GameObject spawned by Object_Factory")] private string particlesName = "particles";
        [SerializeField, Tooltip("The object that will be parent of particles")] private string placeToSpawnName = "Overlay";
        [SerializeField, Tooltip("You should probably leave this on. If your particles loop and this is false, you should use Remove_NamedObject")] private bool deleteOnScreenProgression = true;
        [Header("Override Particles")]
        [SerializeField] private bool overrideParticles = false;
        [SerializeField] private AssetReference overridingParticles;

        private Transform placeToSpawn;
        private ParticleToken pt;

        public override async void DoEvent()
        {
            if (!Object_Factory.TryGetObject(placeToSpawnName, out GameObject go)) { Debug.LogError("can't find desired object in scene. no spawning particles!"); return; }
            
            placeToSpawn = go.transform;

            GameObject guh = await Object_Factory.SpawnParticleObject(overrideParticles, particlesName, placeToSpawn, fellaToSpawnPrefab.AssetGUID, overridingParticles.AssetGUID);
            guh.transform.localPosition = spawnPosition;

            string[] stoid = new string[1];
            stoid[0] = overridingParticles.AssetGUID;
            pt = new(guh.transform.position, fellaToSpawnPrefab.AssetGUID, stoid, guh.name, placeToSpawnName);
            Event_Bus.TryFireSingleObjEvent("AddParticleToList", (object)pt);

            guh.GetComponent<UIParticle_Helper>().SetMyParticleToken = pt;

            if (deleteOnScreenProgression) Script_Manager.ProgressScreenEvent += PanicStop;
        }

        private void PanicStop()
        {
            Object_Factory.RemoveObject(particlesName);

            Script_Manager.ProgressScreenEvent -= PanicStop;
        }

        public override string ToString()
        {
            return "Spawn Particles Object";
        }
    }
}
