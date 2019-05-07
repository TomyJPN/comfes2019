using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PokerSystem;
using System.Text;

public class PokerTest : MonoBehaviour {
  InputField inputField;

  [SerializeField]
  Text resultText;

  // Start is called before the first frame update
  void Start() {
    inputField = GetComponent<InputField>();
  }

  // Update is called once per frame
  void Update() {

  }

  public void InputLogger() {
    string inputValue = inputField.text;

    List<int> cards = new List<int>();
    foreach (string text in inputValue.Split(' ')) {
      try { cards.Add(int.Parse(text, System.Globalization.NumberStyles.AllowHexSpecifier)); }
      catch { }
    }
    //役判定
    PorkerSystem.PokerHand hand = PorkerSystem.Judge(cards);
    // 結果出力
    StringBuilder sb = new StringBuilder();
    string[] suit = new string[] { "♠", "♥", "♦", "♣" };
    string[] rank = new string[] { "Ａ", "２", "３", "４", "５", "６", "７", "８", "９", "10", "Ｊ", "Ｑ", "Ｋ" };

    foreach (int c in cards) {
      try {
        sb.Append(
            suit[((c & 0xF0) >> 4) - 1] +
            rank[(c & 0x0F) - 1]);
      }
      catch { }
    }

    sb.Append(
        System.Environment.NewLine +
        hand.ToString());

    Debug.Log(sb.ToString());
    resultText.text = sb.ToString();

    InitInputField();
  }
  void InitInputField() {

    // 値をリセット
    inputField.text = "";

    // フォーカス
    inputField.ActivateInputField();
  }

}