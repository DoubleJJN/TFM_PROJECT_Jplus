using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectarNodosGrafos : MonoBehaviour
{
    private Transform[] points;
    private LineRenderer lr;
    // Start is called before the first frame update
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < points.Length; i++){
            lr.SetPosition(i, points[i].position);
        }
    }



    public void SetupLine(Transform[] points){
        lr.positionCount = points.Length;
        this.points = points;
    }
}
