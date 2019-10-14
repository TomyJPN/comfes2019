using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
  public void onStartBtn() {
    FadeManager.Instance.LoadScene("Home", 0.5f);
  }
}
