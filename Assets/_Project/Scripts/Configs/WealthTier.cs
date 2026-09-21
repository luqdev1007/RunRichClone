using System;
using UnityEngine;

namespace RunRich.Configs
{
    [Serializable]
    public sealed class WealthTier
    {
        [SerializeField] private string _title;
        [SerializeField] private int _moneyThreshold;
        [SerializeField] private Color _gaugeColor = Color.white;
        [SerializeField] private Mesh _outfitMesh;
        [SerializeField] private int _gaitId;

        public string Title => _title;
        public int MoneyThreshold => _moneyThreshold;
        public Color GaugeColor => _gaugeColor;
        public Mesh OutfitMesh => _outfitMesh;
        public int GaitId => _gaitId;
    }
}
