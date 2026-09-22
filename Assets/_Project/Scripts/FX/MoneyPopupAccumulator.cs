using System;
using DG.Tweening;
using RunRich.Player;

namespace RunRich.FX
{
    public sealed class MoneyPopupAccumulator : IDisposable
    {
        private readonly PlayerWallet _wallet;
        private readonly Tween _idleTimer;

        private int _total;

        public MoneyPopupAccumulator(PlayerWallet wallet, float idlePause)
        {
            _wallet = wallet;
            _idleTimer = DOVirtual.DelayedCall(idlePause, EndStreak)
                .SetAutoKill(false)
                .Pause();

            _wallet.Changed += OnMoneyChanged;
        }

        public event Action<int> Accumulated;
        public event Action Ended;

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
            _idleTimer.Kill();
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            _total += change.Delta;
            _idleTimer.Restart();

            if (_total != 0)
                Accumulated?.Invoke(_total);
        }

        private void EndStreak()
        {
            _idleTimer.Pause();
            _total = 0;
            Ended?.Invoke();
        }
    }
}
