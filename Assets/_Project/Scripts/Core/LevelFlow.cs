using ButchersGames;
using UnityEngine.SceneManagement;

namespace RunRich.Core
{
    public sealed class LevelFlow
    {
        private readonly LevelManager _manager;

        public LevelFlow(LevelManager manager)
        {
            _manager = manager;
        }

        public void Next()
        {
            _manager.NextLevel();
            Reload();
        }

        public void Restart()
        {
            _manager.RestartLevel();
            Reload();
        }

        private static void Reload()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (buildIndex < 0)
                return;

            SceneManager.LoadScene(buildIndex);
        }
    }
}
