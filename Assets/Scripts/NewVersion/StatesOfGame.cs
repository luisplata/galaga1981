using UnityEngine;

public class StatesOfGame : MonoBehaviour
{
    private TeaTime _waitForTouch, _game, _pause, _endGame;

    private IShipInstaller _shipInstaller;
    public void Configure(IShipInstaller shipInstaller)
    {
        CreateTeaTimes();
        _shipInstaller = shipInstaller;
        _waitForTouch.Play();
    }

    private void CreateTeaTimes()
    {
        _waitForTouch = this.tt().Pause().Add(() =>
        {
            _shipInstaller.ShowUiForHowToPlay();
        }).Wait(()=>_shipInstaller.PlayerIsTouching(),0.1f).Add(() =>
        {
        }).Add(()=>
        {
            _shipInstaller.InstantiatePlayer();
            _shipInstaller.StartSpawningEnemies();
            _shipInstaller.HideInstructions();
            _game.Play();
        });
        
        _game = this.tt().Pause().Add(() =>
        {
            _shipInstaller.PlayAll();
            Debug.Log("Game");
        }).Wait(()=>!_shipInstaller.PlayerIsTouching(),0.1f).Add(() =>
        {
            if(_shipInstaller.PlayerIsDead())
                _endGame.Play();
            else
                _pause.Play();
        });
        _pause = this.tt().Pause().Add(() =>
        {
            _shipInstaller.PauseAll();
            Debug.Log("Pause");
        }).Wait(()=>_shipInstaller.PlayerIsTouching(),0.1f).Add(() =>
        {
            _game.Play();
        });
    }
}
