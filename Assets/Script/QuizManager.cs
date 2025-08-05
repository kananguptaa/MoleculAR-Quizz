using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public GameObject moleculePrefab;
        public string correctAnswer;
        public string[] options;
        public string hintText;
    }

    public List<QuizQuestion> questions;
    public Button[] optionButtons;
    public Button nextButton;
    public Button hintButton;
    public GameObject hintTextBox;
    public TMP_Text hintTextContent;

    private GameObject currentMolecule;
    private QuizQuestion currentQuestion;
    private int currentIndex = -1;

    private Transform currentTrackedImageTransform;
    private bool isHintVisible = false;

    // ✅ NEW: Track question sequence instead of using random
    private int sequenceIndex = 0;
    private List<int> questionSequence;

    void Start()
    {
        foreach (var btn in optionButtons)
            btn.gameObject.SetActive(false);

        nextButton.gameObject.SetActive(false);
        hintButton.gameObject.SetActive(false);
        hintTextBox.SetActive(false);
    }

    public void StartQuiz(Transform trackedImageTransform)
    {
        currentTrackedImageTransform = trackedImageTransform;

        foreach (var btn in optionButtons)
            btn.gameObject.SetActive(true);

        nextButton.gameObject.SetActive(true);
        hintButton.gameObject.SetActive(true);

        // ✅ NEW: Initialize question order (no repeats)
        questionSequence = new List<int>();
        for (int i = 0; i < questions.Count; i++)
            questionSequence.Add(i);

        ShowNextQuestion();

        hintButton.onClick.RemoveAllListeners();
        hintButton.onClick.AddListener(ToggleHint);
    }

    public void ShowNextQuestion()
    {
        if (currentMolecule != null)
            Destroy(currentMolecule);

        // ✅ NEW: Show questions in fixed sequence order
        currentIndex = questionSequence[sequenceIndex];
        sequenceIndex = (sequenceIndex + 1) % questionSequence.Count;

        currentQuestion = questions[currentIndex];

        currentMolecule = Instantiate(
            currentQuestion.moleculePrefab,
            currentTrackedImageTransform.position,
            currentTrackedImageTransform.rotation,
            currentTrackedImageTransform
        );

        List<string> shuffledOptions = new List<string>(currentQuestion.options);
        ShuffleList(shuffledOptions);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            TMP_Text tmpText = optionButtons[i].GetComponentInChildren<TMP_Text>();
            tmpText.text = shuffledOptions[i];

            SetButtonColor(optionButtons[i], Color.white);

            int index = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => CheckAnswer(shuffledOptions[index]));
        }

        // Reset hint
        isHintVisible = false;
        hintTextBox.SetActive(false);
    }

    public void CheckAnswer(string selectedAnswer)
    {
        foreach (var btn in optionButtons)
        {
            TMP_Text tmpText = btn.GetComponentInChildren<TMP_Text>();

            if (tmpText.text == currentQuestion.correctAnswer)
                SetButtonColor(btn, Color.green);
            else
                SetButtonColor(btn, Color.red);

            btn.interactable = false;
        }
    }

    private void SetButtonColor(Button button, Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        cb.highlightedColor = color;
        button.colors = cb;

        Image img = button.GetComponent<Image>();
        if (img != null)
            img.color = color;
    }

    private void ShuffleList(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void OnNextButtonPressed()
    {
        foreach (var btn in optionButtons)
        {
            btn.interactable = true;
            SetButtonColor(btn, Color.white);
        }

        ShowNextQuestion();
    }

    private void ToggleHint()
    {
        isHintVisible = !isHintVisible;

        if (isHintVisible)
        {
            hintTextContent.text = currentQuestion.hintText;
            hintTextBox.SetActive(true);
        }
        else
        {
            hintTextBox.SetActive(false);
        }
    }
}