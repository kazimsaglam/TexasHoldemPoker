using System;
using UnityEngine;

namespace UI
{
  public class WelcomeUIManager : MonoBehaviour
  {
    public static WelcomeUIManager instance;

    [SerializeField]
    private GameObject logInPanel;

    [SerializeField]
    private GameObject signUpPanel;

    private void Awake()
    {
      CreateInstance();
    }

    private void CreateInstance()
    {
      if (instance == null)
      {
        instance = this;
      }
    }

    public void OpenLoginPanel()
    {
      logInPanel.SetActive(true);
      signUpPanel.SetActive(false);
    }

    public void OpenSignUpPanel()
    {
      signUpPanel.SetActive(true);
      logInPanel.SetActive(false);
    }
  }
}