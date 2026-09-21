using RunRich.Player;
using UnityEngine;

namespace RunRich.Level
{
    [RequireComponent(typeof(Collider))]
    public abstract class LevelTrigger : MonoBehaviour
    {
        protected abstract void Trigger(PlayerWallet wallet);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerCharacter player) || player.Wallet == null)
                return;

            Trigger(player.Wallet);
        }
    }
}
