using UniRx;
using UnityEngine;

namespace DevRun
{
    public static class Blackboard
    {
        //core
        public static ReactiveProperty<GameState> GameState { get; private set; } = new();
        public static ReactiveProperty<float> Difficulty { get; private set; } = new();
        public static ReactiveProperty<int> Level { get; private set; } = new();
        public static ReactiveProperty<int> MaxLanesCount { get; private set; } = new(3);

        //speed
        public static ReactiveProperty<float> GameSpeedRatio { get; private set; } = new();
        public static ReactiveProperty<float> GameSpeedRatioModifier { get; private set; } = new(1f);
        public static ReactiveProperty<float> MovementSpeed { get; private set; } = new();

        //collectables
        public static ReactiveProperty<int> BugsCollected { get; private set; } = new();

        //player stats
        public static ReactiveProperty<float> TransitionSpeed { get; private set; } = new();
        public static ReactiveProperty<float> TransitionSpeedModifier { get; private set; } = new();
        public static ReactiveProperty<float> PerkTransitionSpeedModifier { get; private set; } = new();
        public static ReactiveProperty<float> MergeEventTimeModifier { get; private set; } = new();
        public static ReactiveProperty<float> BuffTimeModifier { get; private set; } = new();
        public static ReactiveProperty<float> DebuffTimeModifier { get; private set; } = new();
        public static ReactiveProperty<int> MergeEventIterations { get; private set; } = new();
        public static ReactiveProperty<int> BugLimit { get; private set; } = new();
        public static ReactiveProperty<float> CodeLossModifier { get; private set; } = new();

        //events
        public static ReactiveCommand OnBadPickUp { get; private set; } = new();
        public static ReactiveCommand OnGoodPickUp { get; private set; } = new();
        public static ReactiveCommand OnBuffPickUp { get; private set; } = new();
        public static ReactiveCommand OnVersionUpgrade { get; private set; } = new();
        public static ReactiveCommand OnMergeCorrect { get; private set; } = new();

        //DEBUG
        public static int ActionsCounter { get; set; }
    }
}
