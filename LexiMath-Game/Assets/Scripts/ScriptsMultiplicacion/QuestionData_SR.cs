using System.Collections.Generic;

[System.Serializable]
public class QuestionData
{
    public string questionText;
    public int correctAnswer;
    public List<int> options;
}