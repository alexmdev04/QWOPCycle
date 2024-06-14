using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SideFX.Anchors;
using SideFX.Events;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] ScrollingBlock scrollingBlockPrefab;
    [SerializeField] int numOfScrollingBlocks = 3;
    [SerializeField] float scrollingBlockLength = 5f;
    ScrollingBlock[] scrollingBlocks;
    [SerializeField] float moveBlockDistance = 0.1f;
    float lastBlockStartingZ;
    bool scrollingBlocksReady;

    EventBinding<SceneReady> _sceneReadyBinding;

    void OnEnable() {
        _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
        EventBus<SceneReady>.Register(_sceneReadyBinding);
    }
    void OnDisable() {
        EventBus<SceneReady>.Deregister(_sceneReadyBinding);
    }
    void OnSceneReady(SceneReady e)
    {
        for (int i = 0; i < numOfScrollingBlocks; i++)
        {
            ScrollingBlock scrollingBlockNew = Instantiate(scrollingBlockPrefab);
            scrollingBlockNew.name = "ScrollingBlock" + (i + 1);
            float scrollingBlockNewStartingZ = scrollingBlockLength * i;
            scrollingBlockNew.transform.position = new Vector3(
                scrollingBlockNew.transform.position.x,
                scrollingBlockNew.transform.position.y,
                scrollingBlockNewStartingZ);
            scrollingBlockNew.scrollingBlockStartingZ = scrollingBlockNewStartingZ;
            scrollingBlocks[i] = scrollingBlockNew;
        }
        lastBlockStartingZ = scrollingBlocks[^1].scrollingBlockStartingZ;
        scrollingBlocksReady = true;
    }
    void Start()
    {
        scrollingBlocks = new ScrollingBlock[numOfScrollingBlocks];
    }
    void FixedUpdate()
    {
        if (!scrollingBlocksReady) { return; }
        BlockSwap();
        MoveBlocks();
    }
    void MoveBlocks()
    {
        if (Keyboard.current.downArrowKey.isPressed)
        {
            foreach (ScrollingBlock scrollingBlock in scrollingBlocks)
            {
                scrollingBlock.transform.position = new Vector3(
                    scrollingBlock.transform.position.x,
                    scrollingBlock.transform.position.y,
                    scrollingBlock.transform.position.z - moveBlockDistance);
            }
        }
    }
    void BlockSwap()
    {
        foreach (ScrollingBlock scrollingBlock in scrollingBlocks)
        {
            if (scrollingBlock.BlockSwapCheck())
            {
                scrollingBlock.transform.position = new Vector3(
                    scrollingBlock.transform.position.x,
                    scrollingBlock.transform.position.y,
                    lastBlockStartingZ);
            }
        }
    }
}
