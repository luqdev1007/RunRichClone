using System;
using RunRich.Configs;
using RunRich.FX;
using UnityEngine;

namespace RunRich.UI
{
    public sealed class MoneyPopupPresenter : IDisposable
    {
        private const string CurrencySuffix = " $";

        private readonly MoneyPopupAccumulator _accumulator;
        private readonly MoneyPopupConfig _config;
        private readonly MoneyPopupView _view;

        public MoneyPopupPresenter(MoneyPopupAccumulator accumulator, MoneyPopupConfig config, MoneyPopupView view)
        {
            _accumulator = accumulator;
            _config = config;
            _view = view;

            _view.Hide();
            _accumulator.Accumulated += OnAccumulated;
            _accumulator.Ended += OnEnded;
        }

        public void Dispose()
        {
            _accumulator.Accumulated -= OnAccumulated;
            _accumulator.Ended -= OnEnded;
        }

        private void OnAccumulated(int total)
        {
            _view.Show(Format(total), total > 0 ? _config.PositiveColor : _config.NegativeColor);
        }

        private void OnEnded()
        {
            _view.FadeOut();
        }

        private static string Format(int total)
        {
            string sign = total > 0 ? "+" : "-";
            return sign + Mathf.Abs(total) + CurrencySuffix;
        }
    }
}
