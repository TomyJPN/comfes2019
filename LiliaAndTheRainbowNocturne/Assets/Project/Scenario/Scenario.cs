using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioData {
  public string[] text { get; set; }
  public string[] name { get; set; }
}

public class Scenario : MonoBehaviour {
  ScenarioData scenarioData = new ScenarioData();

  [SerializeField]
  Text scenarioText;
  [SerializeField]
  Text scenarioName;
  int count;

  void Start() {
    scenarioData.text = new string[] { "こんにちは！", "リリアです", "よろしく","abcdefghijklmnopqrstuvwxyz","ABCDEFGHIJKLMNOPQRSTUVWXYZ","あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよわをん", "亜哀挨愛曖悪握圧扱宛嵐安案暗永英栄衛映樋口楓解会界回改" };
    scenarioData.name = new string[] { "リリア","リリア","主人公","test","TEST","てすと","漢字"};
    Textupdate();
  }

  void Update() {

  }

  public void Textupdate() {
    Debug.Log("count:" + count.ToString());
    if (count >= scenarioData.text.Length) {
      End();
      return;
    }
    else {
      scenarioName.text = scenarioData.name[count];
      scenarioText.text = scenarioData.text[count];
      count++;
    }
  }

  void End() {
    Destroy(gameObject);
    Debug.Log("End");
  }

}
