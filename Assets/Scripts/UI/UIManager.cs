using System;
using UnityEngine;

namespace UI
{
  public class UIManager : MonoBehaviour
  {
    public static UIManager instance;

    [SerializeField]
    private GameObject logInPanel;

    [SerializeField]
    private GameObject signUpPanel;

    [SerializeField]
    private GameObject dataUpdatePanel;

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

    public void OpenUpdatePanel()
    {
      logInPanel.SetActive(false);
      signUpPanel.SetActive(false);
      dataUpdatePanel.SetActive(true);
    }


    public void OpenSignUpPanel()
    {
      signUpPanel.SetActive(true);
      logInPanel.SetActive(false);
    }
  }
}