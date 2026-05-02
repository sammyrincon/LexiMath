using System.Collections.Generic;
using UnityEngine;

public static class QuestionPool
{
    public static QuestionData_SR GenerateMultiplicationQuestion(int minFactor = 2, int maxFactor = 9)
    {
        int a = Random.Range(minFactor, maxFactor + 1);
        int b = Random.Range(minFactor, maxFactor + 1);
        int correct = a * b;

        QuestionData_SR q = new QuestionData_SR();
        q.questionText = $"¿Cuánto es {a} × {b}?";
        q.correctAnswer = correct;
        q.options = GenerateOptions(correct, a, b);

        return q;
    }

    // Distractors use adjacent table entries — children commonly confuse
    // 4×7 with 4×6 or 3×7, so these options test real table mastery.
    private static List<int> GenerateOptions(int correct, int a, int b)
    {
        HashSet<int> opts = new HashSet<int> { correct };

        int[] candidates = {
            a * (b + 1), a * (b - 1),
            (a + 1) * b, (a - 1) * b,
            a * (b + 2), (a + 2) * b,
        };

        for (int i = candidates.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        foreach (int c in candidates)
        {
            if (c > 0 && c != correct) opts.Add(c);
            if (opts.Count >= 4) break;
        }

        while (opts.Count < 4)
        {
            int offset = Random.Range(1, 5) * (Random.value > 0.5f ? 1 : -1);
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

    public static QuestionData_SR GenerateBossQuestion()
    {
        return GenerateMultiplicationQuestion(6, 12);
    }
}