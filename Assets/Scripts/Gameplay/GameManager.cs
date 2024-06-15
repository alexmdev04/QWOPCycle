using SideFX.Events;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject blockPrefab;
    [SerializeField] [Tooltip("This value is only used in Start()")] int numOfBlocksToCreate = 7;
    [SerializeField] [Tooltip("In meters")] float blockLength = 5f;
    [SerializeField] [Tooltip("In meters per second")] float blockMoveDistance = 10f;

    GameObject[] blocks;
    int
        blockMovedIndex,
        blocksTotal;
    float
        blockMoveToFrontThreshhold,
        blockMoveDistanceOld;
    bool
        blocksReady;

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
        BlocksInitialize();
    }
    void Start()
    {
        blocks = new GameObject[numOfBlocksToCreate];
        blockMovedIndex = numOfBlocksToCreate - 1;
    }
    void Update()
    {
        if (!blocksReady) { return; }
        if (blockMoveDistanceOld != blockMoveDistance)
        { // this prevents misalignment from editing moveBlockDistance at runtime
            blockMoveDistanceOld = blockMoveDistance;
            BlocksResetPositions();
        }
        BlocksMoveToFrontCheck();
        BlocksScroll();
    }
    /// <summary>
    /// Instantiates all blocks and sets values
    /// </summary>
    void BlocksInitialize()
    {
        for (int i = 0; i < numOfBlocksToCreate; i++)
        {
            GameObject blockNew = Instantiate(blockPrefab);
            blockNew.name = "block" + (i + 1);
            blockNew.transform.position = blockNew.transform.position.With(z: blockLength * i);
            blocks[i] = blockNew;
        }
        blocksTotal = numOfBlocksToCreate;
        blockMoveToFrontThreshhold = 0f - blockLength;
        blocksReady = true;
    }
    /// <summary>
    /// Moves all blocks towards the player by ScrollBlockDistance(). if the value alters during runtime, reset all block positions
    /// </summary>
    void BlocksScroll()
    {
        foreach (GameObject scrollingBlock in blocks)
        {
            scrollingBlock.transform.position = scrollingBlock.transform.position.With(z: scrollingBlock.transform.position.z - (blockMoveDistance * Time.deltaTime));
        }
    }
    /// <summary>
    /// If the block's Z is beyond or equal to the threshold, move it in front of the most recently moved block
    /// </summary>
    void BlocksMoveToFrontCheck()
    {
        foreach (GameObject scrollingBlock in blocks)
        {
            if (scrollingBlock.transform.position.z <= blockMoveToFrontThreshhold)
            {
                scrollingBlock.transform.position = scrollingBlock.transform.position.With(z: blocks[blockMovedIndex].transform.position.z + blockLength);
                blockMovedIndex++;
                if (blockMovedIndex >= blocksTotal) { blockMovedIndex = 0; }
            }
        }
    }
    /// <summary>
    /// Resets all blocks to starting positions
    /// </summary>
    void BlocksResetPositions()
    {
        blockMovedIndex = blocksTotal - 1;
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].transform.position = blocks[i].transform.position.With(z: blockLength * i);
        }
    }
}
