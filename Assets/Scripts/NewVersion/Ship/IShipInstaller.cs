using System;

public interface IShipInstaller
{
    void ShowUiForHowToPlay();
    bool PlayerIsTouching();
    bool PlayerIsDead();
    void InstantiatePlayer();
    void StartSpawningEnemies();
    void PauseAll();
    void PlayAll();
    void HideInstructions();
}