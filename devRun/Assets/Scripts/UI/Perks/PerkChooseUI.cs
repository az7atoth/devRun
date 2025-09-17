using Az7.UI;
using Az7.UserInput;
using Az7.Utils.Disposables;
using UnityEngine;
using UniRx;
using Az7.Extensions;
using UnityEngine.UI;
using TMPro;

namespace Branches
{
    public class PerkChooseUI : UI_ViewBase
    {
        public override UI_ViewKey ViewKey => UI_ViewKey.PerkChoose;

        [SerializeField] private Image _leftIcon;
        [SerializeField] private Image _rightIcon;
        [SerializeField] private TMP_Text _leftDescription;
        [SerializeField] private TMP_Text _rightDescription;
        [SerializeField] private PerkConfig[] _perkConfigs;

        public bool PerkSelected { get; private set; }

        private PerkType _left;
        private PerkType _right;

        private DisposableTracker _disposableTracker = new DisposableTracker();

        public void Setup()
        {
            PerkSelected = false;
            DefineRandomPerks();

            var leftConfig = GetConfig(_left);
            var rightConfig = GetConfig(_right);

            if (leftConfig == null || rightConfig == null)
            {
                PerkSelected = true;
                return;
            }

            _leftIcon.sprite = leftConfig.Icon;
            _rightIcon.sprite = rightConfig.Icon;

            _leftDescription.text = leftConfig.Description;
            _rightDescription.text = rightConfig.Description;
        }

        public void Activate()
        {
            InputProviderSingleMono.Instance.OnMoveLeft.Subscribe(pressed =>
            {
                if (!pressed) { return; }
                SelectPerk(true);
            }).AddTo(_disposableTracker);


            InputProviderSingleMono.Instance.OnMoveRight.Subscribe(pressed =>
            {
                if (!pressed) { return; }
                SelectPerk(false);
            }).AddTo(_disposableTracker);
        }

        public void Deactivate()
        {
            _disposableTracker.Dispose();
        }

        private void SelectPerk(bool isLeft)
        {
            PerkController.Instance.AddPerk(isLeft ? _left : _right);
            PerkSelected = true;
            Deactivate();
        }

        private void DefineRandomPerks()
        {
            var rndIndex = Random.Range(0, PerkController.Instance.AvailablePerks.Count);
            _left = PerkController.Instance.AvailablePerks[rndIndex];
            _right = _left;

            var i = 0;

            do
            {
                rndIndex = Random.Range(0, PerkController.Instance.AvailablePerks.Count);
                _right = PerkController.Instance.AvailablePerks[rndIndex];
                i++;
            } while (_right == _left && i < 1000);
        }

        private PerkConfig GetConfig(PerkType perkType)
        {
            for (int i = 0; i < _perkConfigs.Length; i++)
            {
                if (_perkConfigs[i].PerkType == perkType)
                {
                    return _perkConfigs[i];
                }
            }

            return null;
        }
    }
}
