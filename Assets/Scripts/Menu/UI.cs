﻿using UnityEngine;

public class UI : MonoBehaviour{
  public static UI instance{ get; private set; }
  public GameObject playingUI;
  private bool hideUI;
  public Hotbar hotbar;
  public LoadingScreen loadingScreen;
  public GameMenu gameMenu;

  public void Initialize(){
    instance = this;
    hotbar.Initialize();
    loadingScreen.Initialize();
  }

  public void UpdateUI(){
    hotbar.UpdateHotbar();
    if (Input.GetKeyDown(KeyCode.F1)){
      hideUI = !hideUI;
      playingUI.gameObject.SetActive(!hideUI);
    }

    if (Input.GetKeyDown(KeyCode.Escape)){
      gameMenu.OpenMenu();
    }
  }
}