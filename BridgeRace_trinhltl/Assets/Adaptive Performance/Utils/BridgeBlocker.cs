using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBlocker : MonoBehaviour
{
    [SerializeField] private float _offset             = 0.5f;
    private                  bool  _isAlreadyActivated = false;
    private void Awake()
    {
        EnableBlocker(false);
        this._isAlreadyActivated = false;

    }

    public void EnableBlocker(bool enable)
    {
        this.gameObject.SetActive(enable);
        if (!enable)
        {
            this._isAlreadyActivated = false;
        }
    }

    public void ActivateBlocker(Vector3 playerPosition, Vector3 playerForward)
    {
        if (!_isAlreadyActivated)
        {
            var blockerPosition = playerPosition + playerForward * _offset;
            this.transform.position = blockerPosition;

            _isAlreadyActivated = true;
        }

        EnableBlocker(true);
    }

}