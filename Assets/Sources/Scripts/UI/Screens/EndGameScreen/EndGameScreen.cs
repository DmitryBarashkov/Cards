using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class EndGameScreen : UIScreen, IPointerClickHandler
{
    [SerializeField] ParticleSystem _effect;

    private float _duration = 1.5f;

    public override void Setup()
    {
        _gameObject.SetActive(true);
        _canvasGroup.interactable = false;
        Canvas.ForceUpdateCanvases();

        if (_effect != null)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1;
            _effect.Play();
        }
        else
            FadeIn(_duration);
    }

    public void Close() => _gameObject.SetActive(false);

    public void OnPointerClick(PointerEventData eventData) => _canvasGroup.DOComplete();

    private void FadeIn(float duration)
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, duration).SetUpdate(true)
            .OnComplete(() => _canvasGroup.interactable = true);
    }
}
