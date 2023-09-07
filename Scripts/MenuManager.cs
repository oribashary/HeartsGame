using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public enum MenuPage
    {
        MainMenu,
        Options,
        SinglePlayer,
        Multiplayer,
        StudentInfo
    }

    public GameObject mainMenuPage;
    public GameObject optionsPage;
    public GameObject singlePlayerPage;
    public GameObject multiplayerPage;
    public GameObject studentInfoPage;

    private Stack<MenuPage> pageHistory = new Stack<MenuPage>();
    private MenuPage savedPage;

    private void Start()
    {
        ShowPage(MenuPage.MainMenu);
    }

    public void ShowPage(MenuPage pageToShow)
    {
        if (pageHistory.Count > 0)
        {
            MenuPage previousPage = pageHistory.Peek();
            HidePage(previousPage);
        }

        switch (pageToShow)
        {
            case MenuPage.MainMenu:
                mainMenuPage.SetActive(true);
                ResetScale(mainMenuPage);
                break;
            case MenuPage.Options:
                optionsPage.SetActive(true);
                ResetScale(optionsPage);
                break;
            case MenuPage.SinglePlayer:
                singlePlayerPage.SetActive(true);
                ResetScale(singlePlayerPage);
                break;
            case MenuPage.Multiplayer:
                multiplayerPage.SetActive(true);
                ResetScale(multiplayerPage);
                break;
            case MenuPage.StudentInfo:
                studentInfoPage.SetActive(true);
                ResetScale(studentInfoPage);
                break;
        }

        pageHistory.Push(pageToShow);
    }

    public void SaveCurrentPage(string pageName)
    {
        if (Enum.TryParse(pageName, out MenuPage pageToSave))
            savedPage = pageToSave;
    }

    public void GoBackToSavedPage()
    {
        HidePage(MenuPage.Options);
        ShowPage(savedPage);
    }

    public void GoBack()
    {
        if (pageHistory.Count > 1)
        {
            MenuPage currentPage = pageHistory.Pop();
            HidePage(currentPage);

            MenuPage previousPage = pageHistory.Peek();
            Debug.Log("Going back from " + currentPage + " to " + previousPage);
            ShowPage(previousPage);
        }
    }

    private void HidePage(MenuPage pageToHide)
    {
        switch (pageToHide)
        {
            case MenuPage.MainMenu:
                mainMenuPage.SetActive(false);
                break;
            case MenuPage.Options:
                optionsPage.SetActive(false);
                break;
            case MenuPage.SinglePlayer:
                singlePlayerPage.SetActive(false);
                break;
            case MenuPage.Multiplayer:
                multiplayerPage.SetActive(false);
                break;
            case MenuPage.StudentInfo:
                studentInfoPage.SetActive(false);
                break;
        }
    }

    private void ResetScale(GameObject page)
    {
        page.transform.localScale = Vector3.one;
    }
}