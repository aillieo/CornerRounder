using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AillieoUtils;

public class LineDrawer : MonoBehaviour
{
    [SerializeField]
    private Vector3[] points1;

    [SerializeField]
    private CornerRounderConfig config;

    public Vector3[] Points1 { get => points1; }

    private List<Vector3> points2 = new List<Vector3>();

    public List<Vector3> Points2 { get => points2; }

    public CornerRounderConfig Config { get => config; }
}
