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
            Int32.Parse(_triplets.text),
            Int32.Parse(_uniqueCards.text),
            Int32.Parse(_bankSize.text),
            Int32.Parse(_width.text),
            Int32.Parse(_height.text),
            Int32.Parse(_maxLayers.text)
        );

        _field.Initialize(_generator.Generate());
    }
}
