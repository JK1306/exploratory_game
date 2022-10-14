using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey_Enemy : MonoBehaviour
{
    public GameObject[] WayPoints;
    public int I_WayPoint=0;
    Transform Target;
    public float F_speed;
    public bool B_CanEat;

    // Start is called before the first frame update
    void Start()
    {
        Target = WayPoints[0].transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Target.position - transform.position;
        transform.Translate(dir.normalized * F_speed*Time.deltaTime);
        if(Vector3.Distance(transform.position,Target.position)<=0.1f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if(I_WayPoint >= WayPoints[0].transform.childCount-1)
        {
            I_WayPoint = 0;
            // Start Again
            return;
        }

        I_WayPoint++;
        Target = WayPoints[0].transform.GetChild(I_WayPoint);
    }
}
