using Coffee.UIExtensions;
using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Spawn_UIParticle : Screen_Event
    {
        [SerializeField, Tooltip("Local space. Should be same values as actor positions, if that helps.")] private Vector3 spawnPosition = Vector3.zero;
        [SerializeField] private UIParticle fellaToSpawnPrefab;
        [SerializeField, Tooltip("name of GameObject spawned by Object_Factory")] private string particlesName = "particles";
        [SerializeField, Tooltip("You should probably leave this on. If your particles loop and this is false, you should use Remove_NamedObject")] private bool deleteOnScreenProgression = true;
        [Header("Override Particles")]
        [SerializeField] private bool overrideParticles = false;
        [SerializeField] private ParticleSystem overridingParticles;

        private Transform placeToSpawn;

        public override void DoEvent()
        {
            if (!Object_Factory.TryGetObject("Overlay", out GameObject go)) { Debug.LogError("can't find overlay object in scene. no spawn particles!"); return; }
            
            placeToSpawn = go.transform;
            GameObject guh = Object_Factory.SpawnObject(fellaToSpawnPrefab.gameObject, particlesName, placeToSpawn);
            guh.transform.localPosition = spawnPosition;

            GameObject particlechild = guh.transform.GetChild(0).gameObject;
            var particles = particlechild.GetComponent<ParticleSystem>();

            if (overrideParticles)
            {
                particles.CopyParticleSystem(overridingParticles);
                guh.GetComponent<UIParticle>().RefreshParticles();
            }

            if (!particles.main.loop) Script_Manager.SM.StartCoroutine(Object_Factory.RemoveObjectOverTime(particlesName, particles.main.duration));

            particles.Play(true);

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
