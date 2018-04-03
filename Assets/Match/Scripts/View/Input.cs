using UnityEngine;
using System.Collections;

public class Input : MonoBehaviour
{
    public int moveThreshold = 30;
    public float doubleClickThreshold = 0.5f;

    private BoxCollider2D mCollider;
    private Vector2 mTouchBeginPos;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}
}
