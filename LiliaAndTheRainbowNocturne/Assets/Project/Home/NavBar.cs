using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavBar : SingletonMonoBehaviour<NavBar> {
  public void Awake() {
    if (this != Instance) {
      Destroy(this);
      return;
    }
    DontDestroyOnLoad(this.gameObject);
  }
  
  void Start() {

  }

  public void onHomeBtn() {
    if (SceneManager.GetActiveScene().name == "Home") {
      Debug.Log(SceneManager.GetActiveScene().name);
      return;
    }
    SceneManager.LoadScene("Home");
  }
  public void onCharaBtn() {
    if (SceneManager.GetActiveScene().name == "Chara") {
      Debug.Log(SceneManager.GetActiveScene().name);
      return;
    }
    SceneManager.LoadScene("Chara");
  }
  public void onStoryBtn() {
    if (SceneManager.GetActiveScene().name == "StoryMenu") {
      Debug.Log(SceneManager.GetActiveScene().name);
      return;
    }
    SceneManager.LoadScene("StoryMenu");
  }
  public void onQuestBtn() {
    if (SceneManager.GetActiveScene().name == "Quest") {
      Debug.Log(SceneManager.GetActiveScene().name);
      return;
    }
    SceneManager.LoadScene("Quest");
  }
  public void onGatyaBtn() {
    if (SceneManager.GetActiveScene().name == "Gatya") {
      Debug.Log(SceneManager.GetActiveScene().name);
      return;
    }
    SceneManager.LoadScene("Gatya");
  }
  public void onMenuBtn() {
    if (SceneManager.GetActiveScene().name == "Menu") {
      Debug.Log(SceneManager.GetActiveScene().name);
      return;
    }
    SceneManager.LoadScene("Menu");
  }
}
