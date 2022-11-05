using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRotation : MonoBehaviour
{
    [SerializeField][Range(0, 360)] private float _rotateValue;

    [SerializeField] private GameObject _building;
    [SerializeField] private GameObject _point;

    private Quaternion rot;



    
    public void Rotate()
    {
        Vector3 dir = _building.transform.position - _point.transform.position;
        dir.y = 0f;
        rot = Quaternion.LookRotation(dir.normalized);

        _point.transform.rotation = rot;
        transform.RotateAround(
            _point.transform.position, 
            Vector3.up, 
            -1 * _point.transform.rotation.eulerAngles.y
        );
    }

    
}
