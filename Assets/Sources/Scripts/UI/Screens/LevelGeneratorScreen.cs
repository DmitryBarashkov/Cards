using Cards.Databases;
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
            LevelConfig config = ScriptableObject.CreateInstance<LevelConfig>();

            config.TotalTriplets = int.Parse(_triplets.text);
            config.UniqueTypesCount = int.Parse(_uniqueCards.text);
            config.BankSize = int.Parse(_bankSize.text);
            config.GridWidth = int.Parse(_width.text);
            config.GridHeight = int.Parse(_height.text);
            config.MaxLayers = int.Parse(_maxLayers.text);
            config.Shape = (LevelShape)_shape.value;

            _generator.SetGeneratorParams(config);
            _field.Initialize(_generator.Generate());
        }
    }
}