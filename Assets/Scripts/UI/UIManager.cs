using System;
using UnityEngine;

namespace UI
{
  public class UIManager : MonoBehaviour
  {
    public static UIManager Instance;

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
      if (Instance == null)
      {
        Instance = this;
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