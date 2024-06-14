using System.Collections;
using System.Collections.Generic;
using SideFX.Anchors;
using UnityEngine;

public class ScrollingBlock : MonoBehaviour
{
    public float scrollingBlockStartingZ;
    /// <summary>
    /// Returns true if this block should go the back
    /// </summary>
    /// <returns></returns>
    public bool BlockSwapCheck() => transform.position.z <= -5f; // must be equal to 0 - block length
}
