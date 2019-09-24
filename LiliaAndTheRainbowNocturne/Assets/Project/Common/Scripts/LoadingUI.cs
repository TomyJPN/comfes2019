using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour {
  RectTransform bar;

  void Start() {
    bar = transform.Find("bar").GetComponent<RectTransform>();
  }

  void Update() {
    bar.Rotate(new Vector3(0, 0, 5f));
  }
}
