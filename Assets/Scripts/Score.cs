using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField]
    GameObject UITextField;

    public int TotalScore { get; private set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (UITextField != null) {
            UITextField.GetComponent<Text>().text = TotalScore.ToString();
        }
    }

    public void Reset()
    {
        TotalScore = 0;
    }

    public void AddPoints(int points)
    {
        TotalScore += points;
    }
}
