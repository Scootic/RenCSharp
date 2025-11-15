using UnityEngine;
using RenCSharp.Actors;
using UnityEngine.UI;
namespace RenCSharp.Sequences
{
    [CreateAssetMenu(menuName = "ScreenAction/SpawnActor")]
    public class Spawn_Actor : Screen_Event
    {
        [SerializeField] private Actor bitchToSpawn;
        [SerializeField] private int transformIndex, poseIndex, expressionIndex;
        public override void DoShit()
        {
            GameObject spawnt = Instantiate(Script_Manager.SM.ActorPrefab, Script_Manager.SM.ActorPositions[transformIndex].position, Quaternion.identity);
            spawnt.name = bitchToSpawn.ActorName;
            Image pose = spawnt.transform.GetChild(0).GetComponent<Image>();
            Image expression = pose.transform.GetChild(0).GetComponent<Image>();
            pose.sprite = bitchToSpawn.Poses[poseIndex];
            expression.sprite = bitchToSpawn.Expressions[expressionIndex];
        }
    }
}
