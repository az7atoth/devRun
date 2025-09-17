using Branches;
using UnityEngine;
using UnityEngine.UI;

public class MergeCodeBlock : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _sprites;

    public void Activate(BranchesColorIndex colorIndex)
    {
        SetRandomSprite();
        SetSpriteColor(colorIndex);
    }

    public void Select()
    {
        var color = _image.color;
        color.a = .2f;
        _image.color = color;
    }

    private void SetRandomSprite()
    {
        var rnd = Random.Range(0, _sprites.Length);
        _image.sprite = _sprites[rnd];
    }

    private void SetSpriteColor(BranchesColorIndex index)
    {
        _image.color = ColorProvider.Instance.Get(index);
    }
}
