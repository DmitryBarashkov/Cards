using UnityEngine;
using Zenject;

public class UIScreen : MonoBehaviour
{
    protected CanvasGroup _canvasGroup;
    protected GameObject _gameObject;

    private AdService _adService;

    [Inject]
    public virtual void Construct(ShopService service, AdService adService, DiContainer container)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _gameObject = gameObject;
        _adService = adService;
    }

    public virtual void Setup()
    {
        _gameObject.SetActive(true);
        _adService.SetActive(false);
    }

    public void Close()
    {
        _gameObject.SetActive(false);
        _adService.SetActive(true);
    }

    public class Factory : PlaceholderFactory<Transform, GameObject, UIScreen>
    {
    }
}
