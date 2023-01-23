using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SelectionBlock : Block
{
    public void SetBlockPos(int3 blockPos)
    {
        blockPos.z = 1;
        this.SetLightPos(blockPos);
        this.blockPos = blockPos;
        this.transform.position = new Vector3(transform.position.x, transform.position.y, -100);
    }
}
