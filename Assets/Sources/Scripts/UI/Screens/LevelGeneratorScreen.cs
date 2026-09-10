using System;
using TMPro;
using UnityEngine;
using Zenject;

public class LevelGeneratorScreen : UIScreen
{
    [SerializeField] private TMP_InputField _triplets;
    [SerializeField] private TMP_InputField _uniqueCards;
    [SerializeField] private TMP_InputField _bankSize;
    [SerializeField] private TMP_InputField _width;
    [SerializeField] private TMP_InputField _height;
    [SerializeField] private TMP_InputField _maxLayers;

    [Inject] private LevelGenerator _generator;
    [Inject] private Field _field;

    public void GenerateLevel()
    {
        _generator.SetGeneratorParams(
            int.Parse(_triplets.text),
            int.Parse(_uniqueCards.text),
            int.Parse(_bankSize.text),
            int.Parse(_width.text),
            int.Parse(_height.text),
            int.Parse(_maxLayers.text));

        _field.Initialize(_generator.Generate());
    }
}
