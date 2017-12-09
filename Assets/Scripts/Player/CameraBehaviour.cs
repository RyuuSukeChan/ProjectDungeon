﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float freq;

    private Vector2 cameraPos;

    void Start ()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.GetComponent<Transform>();

        cameraTransform = this.GetComponent<Transform>();
	}

	void Update ()
    {
        cameraPos = Vector2.Lerp(cameraTransform.position, playerTransform.position, freq);

        cameraTransform.position = new Vector3(cameraPos.x, cameraPos.y, -10);
	}
}