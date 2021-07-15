using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    [SerializeField] private Transform cameraTran = null;
    [SerializeField] private MeshRenderer _mr = null;
    [SerializeField] private MeshFilter _mf = null;
    [SerializeField] private Transform _tr = null;
    [SerializeField] private bool _validate = false;

    private List<(Vector3? crossA, Vector3? crossB, Vector3? crossC)> list = null;
    private Dictionary<int, Vector3> Adict = null;
    private Dictionary<int, Vector3> Bdict = null;
    private List<Vector3> middlePoints = null;

    public Vector3 startPoint;
    public Vector3 direction;


    private void Update()
    {
        //Debug.Log(direction);
    }

    private void GenerateMeshes()
    {
        var verts = new List<Vector3>();
        _mf.mesh.GetVertices(verts);
        var vertListPartOne = new Dictionary<int, Vector3>();
        var vertListPartTwo = new Dictionary<int, Vector3>();
        Adict = new Dictionary<int, Vector3>();
        Bdict = new Dictionary<int, Vector3>();

        for (int i = 0; i < verts.Count - 1; i++)
        {
            if (GetDistance(direction, startPoint, verts[i]) < 0)
            {

                vertListPartOne.Add(i, verts[i]);
                Adict.Add(i, verts[i]);
            }
            else
            {
                vertListPartTwo.Add(i, verts[i]);
                Bdict.Add(i, verts[i]);
            }
        }

        var triangles = _mf.mesh.triangles;

        var count = 0;
        list = new List<(Vector3? crossA, Vector3? crossB, Vector3? crossC)>();
        middlePoints = new List<Vector3>();
        /*
        var j = 12;
        var crossTuple = GetTrianglesCrossByCutPlane(direction, startPoint, verts[triangles[j]], verts[triangles[j + 1]], verts[triangles[j + 2]]);
        Debug.Log($"crossTuple -> {crossTuple}");
        list.Add(crossTuple);
        */
        for (int i = 0; i < triangles.Length; i = i + 3)
        {
            var crossTuple = GetTrianglesCrossByCutPlane(direction, startPoint, verts[triangles[i]], verts[triangles[i + 1]], verts[triangles[i + 2]]);
            Debug.Log($"crossTuple -> {crossTuple}");
            if (crossTuple.crossA != null || crossTuple.crossB != null)
            {
                list.Add(crossTuple);
                count++;
                if (crossTuple.crossA != null)
                {
                    //Handles.DrawSolidDisc();
                }
            }
                
            //Debug.Log($"{triangles[i]}, {triangles[i + 1]}, {triangles[i + 2]}");
        }
        //Debug.Log($"1234 -> {count}");

        Debug.Log(vertListPartOne.Count);
        Debug.Log(vertListPartTwo.Count);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.DrawLine(Vector3.zero, direction * 1);

        /*
        Debug.Log($"Adict => {Adict.Count}");
        foreach (var q in Adict)
        {
            Handles.DrawWireDisc(q.Value, Vector3.forward, 0.01f);
        }
        */
        /*
        foreach (var q in Bdict)
        {
            Handles.DrawWireDisc(q.Value, Vector3.forward, 0.01f);
        }
        */

        foreach (var item in list)
        {
            if (item.crossA != null)
                Handles.DrawWireDisc((Vector3)item.crossA, Vector3.one, 0.01f);
            if (item.crossB != null)
                Handles.DrawWireDisc((Vector3)item.crossB, Vector3.one, 0.01f);
            if (item.crossC != null)
                Handles.DrawWireDisc((Vector3)item.crossC, Vector3.one, 0.01f);
        }
        foreach (var item in middlePoints)
        {
            Handles.DrawWireDisc((Vector3)item, Vector3.one, 0.025f);
        }

        /*
            if (item.crossA == null)
                Handles.DrawLine((Vector3)item.crossB, (Vector3)item.crossC);
            if (item.crossB == null)
                Handles.DrawAAPolyLine((Vector3)item.crossA, (Vector3)item.crossC);
            if (item.crossC == null)
                Handles.DrawAAPolyLine((Vector3)item.crossA, (Vector3)item.crossB);
        if (item.crossA != null)
            Handles.DrawSolidDisc((Vector3)item.crossA, -cameraTran.forward, 0.01f);
        if (item.crossB != null)
            Handles.DrawSolidDisc((Vector3)item.crossB, -cameraTran.forward, 0.01f);
        if (item.crossC != null)
            Handles.DrawSolidDisc((Vector3)item.crossC, -cameraTran.forward, 0.01f);
        */
    }

    private void OnValidate()
    {
        GenerateMeshes();
    }

    public float GetDistance(Vector3 planeNormal, Vector3 planePos, Vector3 pointPos)
    {
        var c = Mathf.Sqrt(Mathf.Pow(planeNormal.x, 2) + Mathf.Pow(planeNormal.y, 2) + Mathf.Pow(planeNormal.z, 2));
        var b = planeNormal.x * planePos.x + planeNormal.y * planePos.y + planeNormal.z * planePos.z;
        var a = planeNormal.x * pointPos.x + planeNormal.y * pointPos.y + planeNormal.z * pointPos.z;
        var dist = ((a - b) / c) * -1;
        return dist;
    }

    public (Vector3? crossA, Vector3? crossB, Vector3? crossC ) GetTrianglesCrossByCutPlane(
        Vector3 planeNormal, Vector3 planePos, Vector3 pointPosA, Vector3 pointPosB, Vector3 pointPosC
        )
    {
        Vector3? crossA = GetCrossPoint(planeNormal, planePos, pointPosA, pointPosB);
        Vector3? crossB = GetCrossPoint(planeNormal, planePos, pointPosB, pointPosC);
        Vector3? crossC = GetCrossPoint(planeNormal, planePos, pointPosC, pointPosA);

        return (crossA, crossB, crossC);
    }

    public Vector3 CreateVectorByTwoPoints(Vector3 theFirst, Vector3 theSecond)
    {
        return theSecond - theFirst;
        //return theFirst - theSecond;
    }

    public float GetPointsDiffrenceSumm(Vector3 pointOne, Vector3 pointTwo)
    {
        return Mathf.Abs(pointOne.x - pointTwo.x) + Mathf.Abs(pointOne.y - pointTwo.y) + Mathf.Abs(pointOne.z - pointTwo.z);
    }

    public Vector3? GetCrossPoint(Vector3 planeNormal, Vector3 planePos, Vector3 pointPosA, Vector3 pointPosB)
    {
        var ALineNormalizeDirection = CreateVectorByTwoPoints(pointPosA, pointPosB).normalized;
        var scalarA = ALineNormalizeDirection.Scalar(planeNormal);
        // check for paralel
        if (Mathf.Abs(scalarA) < 0.01f)
            return null;
        Debug.Log("---------------------------");

        //Debug.Log("---------------------------");
        //Debug.Log($"planeNormal - {planeNormal}, planePos = {planePos}, pointPosA => {pointPosA}, pointPosB - {pointPosB}");
        //Debug.Log("===========================");
        // firstPointDistance

        var A1Dist = GetDistance(planeNormal, planePos, pointPosA);
        Debug.Log($"A1Dist -> {A1Dist}");
        var A2Dist = GetDistance(planeNormal, planePos, pointPosB);
        Debug.Log($"A2Dist -> {A2Dist}");
        var A1Pos = A1Dist > 0 ? pointPosA + planeNormal * A1Dist : pointPosA + planeNormal * A1Dist;
        var A2Pos = A2Dist > 0 ? pointPosB + planeNormal * A2Dist : pointPosB + planeNormal * A2Dist;
        Debug.Log($"A1Pos - {A1Pos}");
        Debug.Log($"A2Pos - {A2Pos}");
        middlePoints.Add(A1Pos);
        middlePoints.Add(A2Pos);

        // check if points in sameLocation
        // persistance check;
        if (GetPointsDiffrenceSumm(A1Pos, A2Pos) < 0.0001f)
            return A1Pos;
        //var A2Pos = Vector3.zero;
        // proomly infinity
        //var summ = A1Dist / Mathf.Abs(A1Dist) + A2Dist / Mathf.Abs(A2Dist)
        var isCrossLine = (A1Dist > 0 && A2Dist < 0) || (A1Dist < 0 && A2Dist > 0);
        //Debug.Log($"isCrossLine - {isCrossLine}");
        if (!((A1Dist > 0 && A2Dist < 0) || (A1Dist < 0 && A2Dist > 0)))
            return null;
        // get triangles whith equeal engles a1/a2 == b1/b2 == c1/c2
        var ALineDist = Vector3.Distance(A1Pos, A2Pos);
        var AB1 = Vector3.Distance(pointPosA, A1Pos);
        var AB2 = Vector3.Distance(pointPosB, A2Pos);
        Debug.Log($"ALineDist => {ALineDist}");
        Debug.Log($"AB1 => {AB1}");
        Debug.Log($"AB2 => {AB2}");
        // from a point proportion = a/b = c / dist - c
        // a * dist/b - c/b = c
        //var factor = (AB1 + AB2) / 2;
        var factor = (AB1 / AB2) / 2;


        var dist = ALineDist * factor ;
        /*
        var s = AB1 * ALineDist / AB2;
        var oneDivB = 1 / AB2;
        var distFromA = s / (1 + oneDivB);
        */
        Debug.Log($"dist => {dist}");
        var directionFromAPointToB = CreateVectorByTwoPoints(A1Pos, A2Pos).normalized;
        Debug.Log($"directionFromAPointToB => {directionFromAPointToB}");
        var crossPoint = A1Pos + directionFromAPointToB * dist;
        Debug.Log($"crossPoint => {crossPoint}");
        Debug.Log("---------------------------");
        return crossPoint;
            //Debug.Log("---------------------------");
    }

    private Vector3 TransformToWorldCoords(Vector3 pos)
    {
        var objPos = _tr.position;
        return new Vector3(objPos.x + pos.x, objPos.y + pos.y, objPos.z + pos.z);
    }

    private Vector3 TransformToLocalDirection(Vector3 dir)
    {
        //direction
        return Vector3.up;
    }
}

public static class  Vector3Extensions
{
    public static float Scalar(this Vector3 vector, Vector3 otherVector)
    {
        return vector.x * otherVector.x + vector.y * otherVector.y + vector.z * otherVector.z;
    }
}
