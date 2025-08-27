using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public BlockCharacter TargetCharacter;
    public Rigidbody RB;
    public Animator Anim;
    void Awake()
    {
        // RB = GetComponent<Rigidbody>();
        // RB.isKinematic = true;
    }
    void Start()
    {
        Anim = GetComponent<Animator>();
   //     RB.isKinematic = false;
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
