using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private AudioService _audioServicePrefab;
    [SerializeField] private CardsDatabase _cardsDatabase;
    [SerializeField] private InAppsDatabase _inAppDatabase;

    [Header("Languages flags")]
    [SerializeField] private List<LanguageSpritePair> _languageFlags;

    public override void InstallBindings()
    {
        Container.Bind<CardsDatabase>().FromInstance(_cardsDatabase).AsSingle().NonLazy();
        Container.Bind<InAppsDatabase>().FromInstance(_inAppDatabase).AsSingle().NonLazy();
        Container.Bind<InputService>().AsSingle().NonLazy();        
        
        Container.BindInterfacesAndSelfTo<AudioService>()
            .FromComponentInNewPrefab(_audioServicePrefab)
            .UnderTransformGroup("GlobalServices")
            .AsSingle()
            .NonLazy();

        BindLanguages();
    }

    private void BindLanguages()
    {
        var flagsDictionary = new Dictionary<string, Sprite>();

        foreach (var pair in _languageFlags)
        {
            if (pair.Sprite == null) 
                continue;

            string key = pair.LanguageCode.ToLower().Trim();

            if (!flagsDictionary.ContainsKey(key))
                flagsDictionary.Add(key, pair.Sprite);
            else
                Debug.LogWarning($"[GlobalInstaller] Дубликат ключа языка: {key}");            
        }

        Container.Bind<Dictionary<string, Sprite>>().WithId("Languages").FromInstance(flagsDictionary).AsSingle();
    }

    [Serializable]
    public struct LanguageSpritePair
    {
        [Tooltip("Код языка, например: ru, en, de")]
        public string LanguageCode;

        [Tooltip("Спрайт флага для этого языка")]
        public Sprite Sprite;
    }
}
