using System.Collections;
using System.Collections.Generic;
using Assets.PlayerScripts;
using UnityEngine;

public class QWOPCharacter : MonoBehaviour
{
    public InputReader inputReader;

    private Rigidbody rigidBody;
    // Start is called before the first frame update
    void Awake()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        if (rigidBody is null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
