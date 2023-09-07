using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    #region Private Variables

    private const float DefaultFallTime = 5f;
    private const float FallSpeed = 2.0f;
    private const float InitialPercentage = 0.1f;
    private const float FallHeight = -5f;
    private const float TargetPositionZ = -1;

    private float fallTime = DefaultFallTime;
    private Vector3 initialPosition;
    private bool isFalling = false;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if (!isFalling)
            return;
            
        StartCoroutine(Fall());
    }

    #endregion

    #region Public Methods

    public void StartFalling()
    {
        initialPosition = transform.position;
        StartCoroutine(Fall());
    }

    #endregion

    #region Coroutines

    private IEnumerator Fall()
    {
        isFalling = false;
        fallTime = Random.Range(3, 10);
        float startTime = Time.time;
        Vector3 targetPosition = new Vector3(transform.position.x, FallHeight, TargetPositionZ);

        float percentage = InitialPercentage;

        while (percentage <= 1.0f)
        {
            float elapsedTime = Time.time - startTime;
            percentage = elapsedTime / fallTime;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, percentage);
            yield return new WaitForEndOfFrame();
        }

        isFalling = true;
    }

    #endregion
}