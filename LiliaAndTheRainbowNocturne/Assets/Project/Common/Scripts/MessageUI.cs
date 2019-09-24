using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour {
  private Text text;
  
  // Start is called before the first frame update
  void Awake() {
    text = transform.Find("img/Text").GetComponent<Text>();
    Invoke("delete",2f);
    DontDestroyOnLoad(this);
  }

  public void SetMessageText(string str) {
    Debug.Log(str);
    text.text = str;
  }

  void delete() {
    Destroy(gameObject);
  }
}
