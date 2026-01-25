using System.Collections.Generic;

public enum QuestionType
{
    Single = 0,
    Multiple = 1,
    Text = 2
}

public class Test
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int TimeMinutes { get; set; }

    public List<Question> Questions { get; set; }

    public Test()
    {
        Questions = new List<Question>();
    }
}

public class Question
{
    public string Id { get; set; }
    public string Text { get; set; }
    public QuestionType Type { get; set; }
    public List<string> Options { get; set; }
    public string AnswerHash { get; set; }
    public string Salt { get; set; }

    public Question()
    {
        Options = new List<string>();
    }
}
