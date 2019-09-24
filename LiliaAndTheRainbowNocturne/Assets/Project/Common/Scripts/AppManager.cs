using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

class RequestError {
  public int code { get; }
  public string Message { get; }
}

/// <summary>
/// アプリ起動中常に動くシングルトンマネージャークラス
/// </summary>
public class AppManager : SingletonMonoBehaviour<AppManager> {
  [SerializeField]
  GameObject messageBoxPrefab;

  public GameObject loadingUiPrefab;
  public string baseUrl = "http://ik1-323-21780.vs.sakura.ne.jp:3001";

  //通信時一時保存
  public bool isRequestFinished;
  public string httpResponseData;
  public int httpResponseCode;

  public void Awake() {
    if (this != Instance) {
      Destroy(this);
      return;
    }

    DontDestroyOnLoad(this.gameObject);
  }

  void Start() {

  }

  /// <summary>
  /// 画面上に指定したメッセージボックスを生成する（時間経過で消える）
  /// </summary>
  public void viewMessage(string str) {
    GameObject obj = Instantiate(messageBoxPrefab, transform.position, transform.rotation) as GameObject;
    MessageUI m = obj.GetComponent<MessageUI>();
    m.SetMessageText(str);
  }

  public string checkHttpResponseCode() {
    try {
      if (httpResponseCode < 400) return "[" + httpResponseCode + "]成功";
      else if (httpResponseCode == 404) return "[" + httpResponseCode + "]該当なし";
      else if (httpResponseCode == 500) return "[" + httpResponseCode + "]サーバーサイドエラー";
      else if (httpResponseCode == 400) return JsonConvert.DeserializeObject<RequestError>(AppManager.Instance.httpResponseData).Message;

      else return "エラーコード：" + httpResponseCode;

    }
    catch {
      return "その他のエラー";
    }
  }
}
