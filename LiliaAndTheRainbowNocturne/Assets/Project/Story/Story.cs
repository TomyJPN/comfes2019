using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using BestHTTP;
using System;

public class StoryData {
  public int id { get; set; }
  public string name { get; set; }
  public string text { get; set; }
  public string face { get; set; }
  public int chapter { get; set; }
  public int episode { get; set; }
}

public class Story : MonoBehaviour {
  List<StoryData> storyDatas=new List<StoryData>();

  [SerializeField]
  Text scenarioText;
  [SerializeField]
  Text scenarioName;
  int count;

  IEnumerator Start() {
    GetStory(1,1,AppManager.Instance.baseUrl);

    GameObject load = Instantiate(AppManager.Instance.loadingUiPrefab); //ローディングエフェクト
    while (AppManager.Instance.isRequestFinished == false) {
      yield return null;
    }
    if (AppManager.Instance.httpResponseCode != 200) {
      AppManager.Instance.viewMessage(AppManager.Instance.checkHttpResponseCode());
      Destroy(load);  //ローディング終了 
      yield break;
    }
    Destroy(load);  //ローディング終了

    storyDatas = JsonConvert.DeserializeObject<List<StoryData>>(AppManager.Instance.httpResponseData);
    Textupdate();

  }

  void Update() {

  }

  public void Textupdate() {
    if (count >= storyDatas.Count) {
      End();
      return;
    }
    else {
      scenarioName.text = storyDatas[count].name;
      scenarioText.text = storyDatas[count].text;
      count++;
    }
  }

  void End() {
    Destroy(gameObject);
    Debug.Log("End");
  }



  public void GetStory(int chap,int epis, string baseUrl) {
    System.Uri uri = new System.Uri(baseUrl + "/story?chap="+chap+"&epis="+epis);
    HTTPRequest httpRequest = new HTTPRequest(uri, HTTPMethods.Get, CommonRequestFinished);//汎用
    Debug.Log("send");
    httpRequest.Send();
    AppManager.Instance.isRequestFinished = false;
  }

  public static void CommonRequestFinished(HTTPRequest request, HTTPResponse response) {
    AppManager.Instance.isRequestFinished = true;
    AppManager.Instance.httpResponseCode = response.StatusCode;
    // request.Stateを見てリクエストが成功したかなどを判別します  
    switch (request.State) {
      case HTTPRequestStates.Finished:
        // サーバーからレスポンスが返ってきたらHTTPRequestStates.Finishedになります。  
        // response.StatusCodeにレスポンスのステータスコードが入ってるので、値に応じた処理を行います。  
        if (response.StatusCode < 400) {
          AppManager.Instance.httpResponseData = response.DataAsText;
          Debug.Log(response.DataAsText);
          // 成功時の処理  
        }
        else if (response.StatusCode == 404) {
          throw new Exception(response.StatusCode + ":NotFound");
        }
        else if (response.StatusCode == 400) {
          AppManager.Instance.httpResponseData = response.DataAsText;
          var responseBody = JsonConvert.DeserializeObject<RequestError>(response.DataAsText);
          
          throw new Exception(responseBody.Message);
        }
        else {
          // 失敗時の処理  
        }
        break;
      case HTTPRequestStates.Error:
        AppManager.Instance.viewMessage("予期しないエラー");
        throw new Exception("予期しないエラー");
      case HTTPRequestStates.Aborted:
       //リクエストをHTTPRequest.Abort()でAbortさせた場合 
        AppManager.Instance.viewMessage("Abort");
        throw new Exception("Abort");
      case HTTPRequestStates.ConnectionTimedOut:
        //サーバーとのコネクションのタイムアウト  
        AppManager.Instance.viewMessage("コネクションタイムアウト");
        throw new Exception("コネクションタイムアウト");
      case HTTPRequestStates.TimedOut:
        // リクエストのタイムアウト  
        AppManager.Instance.viewMessage("リクエストタイムアウト");
        throw new Exception("リクエストタイムアウト");
      default:
        break;
    }
  }

}
