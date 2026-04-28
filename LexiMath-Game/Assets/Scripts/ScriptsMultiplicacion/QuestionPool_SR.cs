using System.Collections.Generic;
using UnityEngine;

public static class QuestionPool
{
    public static QuestionData GenerateMultiplicationQuestion(int minFactor = 2, int maxFactor = 9)
    {
        int a = Random.Range(minFactor, maxFactor + 1);
        int b = Random.Range(minFactor, maxFactor + 1);
        int correct = a * b;
        
        QuestionData q = new QuestionData();
        q.questionText = $"{a} × {b} = ?";
        q.correctAnswer = correct;
        q.options = GenerateOptions(correct);
        
        return q;
    }
    
    private static List<int> GenerateOptions(int correct)
    {
        HashSet<int> opts = new HashSet<int> { correct };
        
        while (opts.Count < 4)
        {
            int offset = Random.Range(-10, 11);
            if (offset == 0) continue;
            
            int wrong = correct + offset;
            if (wrong > 0) opts.Add(wrong);
        }
        
        List<int> result = new List<int>(opts);
        for (int i = 0; i < result.Count; i++)
        {
            int rand = Random.Range(i, result.Count);
            (result[i], result[rand]) = (result[rand], result[i]);
        }
        
        return result;
    }
    
    public static QuestionData GenerateBossQuestion()
    {
        return GenerateMultiplicationQuestion(6, 12);
    }
}