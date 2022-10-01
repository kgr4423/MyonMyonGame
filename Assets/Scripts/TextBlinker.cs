using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinker : MonoBehaviour
{
    private float speed = 1.0f;
    private Text text;
    private float time;
    private float temp;

    void Start()
    {
        text = this.gameObject.GetComponent<Text>();
    }

    void Update()
    {
        text.color = GetAlphaColor(text.color);
    }

    Color GetAlphaColor(Color color){

        if(Mathf.Sin(time) > 0.5f){
            time += Time.deltaTime * 0.5f * speed;
            color.a = 1;
        }else{
            time += Time.deltaTime * 3.0f * speed;
            color.a = 0.5f + Mathf.Sin(time) * 0.5f;
        }

        return color;
    }
}
