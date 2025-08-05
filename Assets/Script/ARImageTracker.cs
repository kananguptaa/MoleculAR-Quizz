using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageTracker : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public QuizManager quizManager;

    private bool quizStarted = false;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        if (quizStarted) return;

        foreach (var trackedImage in args.added)
        {
            if (trackedImage.referenceImage.name == "acetic") // Your image name here
            {
                quizManager.StartQuiz(trackedImage.transform);
                quizStarted = true;
            }
        }
    }
}