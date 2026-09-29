using Cards.Gameplay;
using TMPro;
using UnityEngine;
using Zenject;

namespace Cards.UI
{
    public class LevelGeneratorScreen : UIScreen
    {
        [SerializeField] private TMP_InputField _triplets;
        [SerializeField] private TMP_InputField _uniqueCards;
        [SerializeField] private TMP_InputField _bankSize;
        [SerializeField] private TMP_InputField _width;
        [SerializeField] private TMP_InputField _height;
        [SerializeField] private TMP_InputField _maxLayers;
        [SerializeField] private TMP_Dropdown _shape;

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
                int.Parse(_maxLayers.text),
                (LevelShape)_shape.value);

            _field.Initialize(_generator.Generate());
        }
    }
}