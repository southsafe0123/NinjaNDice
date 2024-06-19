

public class Question
{
    //int id
    //string text question
    //string[] text answer
    //int correct answer

    public int id { get; set; }
    public string text { get; set; }
    public string[] answers { get; set; }
    public int correctAnswer { get; set; }
    public Question(int id, string text, string[] answers, int correctAnswer)
    {
        this.id = id;
        this.text = text;
        this.answers = answers;
        this.correctAnswer = correctAnswer;
    }

    public Question()
    {

    }



}



