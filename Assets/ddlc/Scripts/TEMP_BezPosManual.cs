using EXPERIMENTAL;
using UnityEditor.SearchService;
using UnityEngine;

public class TEMP_BezPosManual : MonoBehaviour
{
    [SerializeField] protected float timeToElapse;
    [SerializeField] protected Vector3[] boundingPoints;
    protected Vector3[] realBounders;
    protected float t, eval;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void OnEnable()
    {
        t = 0;
        realBounders = new Vector3[boundingPoints.Length];
        for (int i = 0; i < boundingPoints.Length; i++) 
        {
            realBounders[i] = transform.position + boundingPoints[i]; 
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        t += Time.deltaTime;
        eval = t / timeToElapse;

        transform.position = TrigHelper.BezPos(realBounders, eval);

        if (t >= timeToElapse) t = 0;
    }
}
