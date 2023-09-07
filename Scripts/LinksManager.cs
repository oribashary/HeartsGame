using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinksManager : MonoBehaviour
{
    private string websiteURL = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

    public void OpenURL()
    {
        Application.OpenURL(websiteURL);
    }
}
