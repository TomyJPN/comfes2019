using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavBar : MonoBehaviour {
  float fadeSpeed=0.25f;

  void Start() {

  }

  public void onHomeBtn() {
    if (SceneManager.GetActiveScene().name == "Home") {
      return;
    }
    FadeManager.Instance.LoadScene("Home", fadeSpeed);
  }
  public void onCharaBtn() {
    if (SceneManager.GetActiveScene().name == "Chara") {
      return;
    }
    FadeManager.Instance.LoadScene("Chara", fadeSpeed);
  }
  public void onStoryBtn() {
    if (SceneManager.GetActiveScene().name == "StoryMenu") {
      return;
    }
    FadeManager.Instance.LoadScene("StoryMenu", fadeSpeed);
  }
  public void onQuestBtn() {
    if (SceneManager.GetActiveScene().name == "Quest") {
      return;
    }
    FadeManager.Instance.LoadScene("Quest", fadeSpeed);
  }
  public void onGatyaBtn() {
    if (SceneManager.GetActiveScene().name == "Gatya") {
      return;
    }
    FadeManager.Instance.LoadScene("Gatya", fadeSpeed);
  }
  public void onMenuBtn() {
    if (SceneManager.GetActiveScene().name == "Menu") {
      return;
    }
    FadeManager.Instance.LoadScene("Menu", fadeSpeed);
  }
}
