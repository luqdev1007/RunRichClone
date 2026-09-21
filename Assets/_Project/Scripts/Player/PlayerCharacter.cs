using UnityEngine;

namespace RunRich.Player
{
    public sealed class PlayerCharacter : MonoBehaviour
    {
        public PlayerWallet Wallet { get; private set; }

        public void Construct(PlayerWallet wallet)
        {
            Wallet = wallet;
        }
    }
}
