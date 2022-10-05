using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData
{
    public static Vector3[] voxelVerts = new Vector3[24]
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0),
        new Vector3(0,1,0),
        new Vector3(1,0,0),
        new Vector3(1,0,1),
        new Vector3(1,1,1),
        new Vector3(1,1,0),
        new Vector3(1,0,1),
        new Vector3(0,0,1),
        new Vector3(0,1,1),
        new Vector3(1,1,1),
        new Vector3(0,0,1),
        new Vector3(0,0,0),
        new Vector3(0,1,0),
        new Vector3(0,1,1),
        new Vector3(0,1,0),
        new Vector3(1,1,0),
        new Vector3(1,1,1),
        new Vector3(0,1,1),
        new Vector3(1,0,0),
        new Vector3(0,0,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
    };

    public static int[,] triangles = new int[6, 6]
    {
        {0,3,1,1,3,2}, //front
        {4,7,5,5,7,6}, //right
        {8,11,9,9,11,10}, //back
        {12,15,13,13,15,14}, //left
        {16,19,17,17,19,18}, //top
        {20,23,21,21,23,22}, //bottom
    };
}
