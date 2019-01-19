﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private BallBody body;

    private Vector3 move;
    // the world-relative desired move direction, calculated from the camForward and user input.

    private Transform cam; // A reference to the main camera in the scenes transform
    private Vector3 camForward; // The current forward direction of the camera
    private int jump; // whether the jump button is currently pressed
    private int action;

    public bool can_input = true;

    private void Awake()
    {
        // Set up the reference.
        body = GetComponent<BallBody>();

        // get the transform of the main camera
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Ball needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            // we use world-relative controls in this case, which may not be what the user wants, but hey, we warned them!
        }
    }

    private void Update()
    {
        // Get the axis and jump input.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        GetButton(ref jump, "Jump");
        GetButton(ref action, "Action");

        // calculate move direction
        if (cam != null)
        {
            // calculate camera relative direction to move:
            camForward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up); //Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            move = (v * camForward + h * cam.right).normalized;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            move = (v * Vector3.forward + h * Vector3.right).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (can_input)
            body.Input(move.magnitude > 0f, move, jump, action);
    }

    private void GetButton(ref int button, string axisName)
    {
        button = Input.GetAxis(axisName) > 0f ? 1 : 0;

        if (button == 0)
        {
            button = Input.GetButtonUp(axisName) ? -1 : 0;
        }
        else if (button == 1)
        {
            button = Input.GetButtonDown(axisName) ? 2 : 1;
        }
    }
}