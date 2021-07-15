using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutterPlane : MonoBehaviour
{
    [SerializeField] private Transform _tran = null;
    [SerializeField] private MeshCutter meshCutter = null;

    private void Update()
    {
        meshCutter.startPoint = _tran.position;
        meshCutter.direction = _tran.up.normalized;
    }

}
