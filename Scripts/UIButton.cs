using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private GameObject targetObject;
    [SerializeField] private string targetMessage;
    [SerializeField] private Color highlightColor = Color.cyan;
    #endregion

    #region Private Fields
    private Vector3 defaultScale;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Initialization
    private void Start()
    {
        defaultScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    #endregion

    #region Mouse Events
    public void OnMouseEnter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    public void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void OnMouseDown()
    {
        AdjustScale(0.1f);
    }

    public void OnMouseUp()
    {
        ResetScale();
        SendTargetMessage();
    }
    #endregion

    #region Private Methods
    private void AdjustScale(float amount)
    {
        transform.localScale = defaultScale + new Vector3(amount, amount, amount);
    }

    private void ResetScale()
    {
        transform.localScale = defaultScale;
    }

    private void SendTargetMessage()
    {
        if (targetObject != null)
        {
            targetObject.SendMessage(targetMessage);
        }
    }
    #endregion
}