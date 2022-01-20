using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text;

public class MainController : MonoBehaviour
{
    [SerializeField]
    private InputField number1Inp;

    [SerializeField]
    private InputField number2Inp;

    [SerializeField]
    private Text resultTxt;

    [SerializeField]
    private string endpointUrl = "http://fallingduckbackend.somee.com/api/ExtraCoins/Calculation";

    private TwoNumbers twoNumbers = new TwoNumbers();

    public void Send() {
        twoNumbers.time = number1Inp.text;
        twoNumbers.coins = number2Inp.text;

        string bodyJson = JsonConvert.SerializeObject(twoNumbers);
        StartCoroutine(CallEndpoint(endpointUrl, bodyJson, OnNumberReceived));
    }

    private void OnNumberReceived(string obj) {
        resultTxt.text = obj;
    }

    private IEnumerator CallEndpoint(string finalUrl, string bodyJson, Action<string> callback = null) {
        Debug.Log($"Calling: {finalUrl}, with body: {bodyJson}");

        UnityWebRequest webRequest = new UnityWebRequest(finalUrl, "POST");

        //Mucho crap para que funcione el envío de json
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        string data = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
        Debug.Log($"Result: {webRequest.result}");
        Debug.Log($"Data returned: {data}");
        if (webRequest.isDone) {
            callback?.Invoke(data);
            webRequest.Dispose();
        }
    }
}
