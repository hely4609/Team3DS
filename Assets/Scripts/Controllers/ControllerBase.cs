using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerBase : MonoBehaviour
{
    protected Player controlledPlayer;
    public Player ControlledPlayer => controlledPlayer;
}
