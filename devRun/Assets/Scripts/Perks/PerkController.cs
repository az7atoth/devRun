using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace DevRun
{
    public class PerkController : MonoBehaviour
    {
        public ReactiveCommand OnPerksChanged { get; private set; } = new();

        public static PerkController Instance { get; private set; }

        /// <summary>
        /// Perk type and current level
        /// </summary>
        public Dictionary<PerkType, int> ActivePerks { get; private set; } = new(6);

        /// <summary>
        /// Perks for choose
        /// </summary>
        public List<PerkType> AvailablePerks { get; private set; } = new(6);

        [SerializeField] private PerkConfig[] _perkConfigs;

        public void AddPerk(PerkType perkType)
        {
            var config = GetPerkConfig(perkType);
            if (config == null) return;

            if (ActivePerks.ContainsKey(perkType))
            {
                ActivePerks[perkType] += 1;
            }
            else
            {
                ActivePerks.Add(perkType, 1);
            }

            ApplyPerkEffect(config);

            UpdateAvailablePerks();
            OnPerksChanged?.Execute();
        }

        public void Clear()
        {
            ActivePerks.Clear();
            UpdateAvailablePerks();
            OnPerksChanged?.Execute();
        }

        private void UpdateAvailablePerks()
        {
            AvailablePerks.Clear();

            for (int i = 0; i < _perkConfigs.Length; i++)
            {
                var config = _perkConfigs[i];

                if (ActivePerks.TryGetValue(config.PerkType, out var perkLevel))
                {
                    if (perkLevel < config.MaxLevel)
                    {
                        AvailablePerks.Add(config.PerkType);
                    }
                }
                else
                {
                    AvailablePerks.Add(config.PerkType);
                }
            }
        }

        private void ApplyPerkEffect(PerkConfig config)
        {
            switch (config.PerkType)
            {
                case PerkType.MergeTimeBonus:
                    Blackboard.MergeEventTimeModifier.Value = 1f + (ActivePerks[config.PerkType] * config.EffectByLevel);
                    break;
                case PerkType.LessCodeLoss:
                    Blackboard.CodeLossModifier.Value = .5f - (ActivePerks[config.PerkType] * config.EffectByLevel);
                    break;
                case PerkType.BugLimit:
                    Blackboard.BugLimit.Value = 3 + (ActivePerks[config.PerkType] * Mathf.RoundToInt(config.EffectByLevel));
                    break;
                case PerkType.TransitionSpeed:
                    Blackboard.PerkTransitionSpeedModifier.Value = 1f + (ActivePerks[config.PerkType] * config.EffectByLevel);
                    break;
                case PerkType.DebuffTimeReduction:
                    Blackboard.DebuffTimeModifier.Value = 1f - (ActivePerks[config.PerkType] * config.EffectByLevel);
                    break;
                case PerkType.BuffTimeBonus:
                    Blackboard.BuffTimeModifier.Value = 1f + (ActivePerks[config.PerkType] * config.EffectByLevel);
                    break;
            }
        }

        private PerkConfig GetPerkConfig(PerkType perkType)
        {
            for (int i = 0; i < _perkConfigs.Length; i++)
            {
                if (_perkConfigs[i].PerkType == perkType) return _perkConfigs[i];
            }

            return null;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
