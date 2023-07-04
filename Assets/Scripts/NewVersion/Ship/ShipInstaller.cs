using NewVersion.Ship;
using NewVersion.Ship.Enemies;
using NewVersion.Ship.Factory;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipInstaller : MonoBehaviour, IShipInstaller, IEnemiesSpawner
{
    [SerializeField] private ShipToSpawnConfiguration shipPlayerConfiguration;
    [SerializeField] private ShipConfiguration shipConfiguration;
    [SerializeField] private StatesOfGame statesOfGame;
    [SerializeField] private GameObject uiToHowToPlay;
    [SerializeField] private EnemiesSpawner enemiesSpawner;
    private ShipControllerMediator ship;
    private bool isTouching;
    private ShipFactory _shipFactory;
    private bool _isPaused;

    private void Awake()
    {
        _shipFactory = new ShipFactory(Instantiate(shipConfiguration));
        
        statesOfGame.Configure(this);
        
        //Config all states to begging the game
        uiToHowToPlay.SetActive(false);
    }

    public void ShowUiForHowToPlay()
    {
        uiToHowToPlay.SetActive(true);
    }

    public bool PlayerIsTouching()
    {
        return isTouching;
    }

    public bool PlayerIsDead()
    {
        return false;
    }

    public void InstantiatePlayer()
    {
        var shipBuilder = _shipFactory.Create(shipPlayerConfiguration.ShipId.Id);
        shipBuilder.WithShipConfiguration(shipPlayerConfiguration)
            .WithTypeOfInput(TypeOfInput.TouchInput)
            .WithPrefabProjectile(shipPlayerConfiguration.ProjectileId)
            .WithMediator(this);

        ship = shipBuilder.Build();
    }

    public void StartSpawningEnemies()
    {
        
        enemiesSpawner.StartSpawning();
        
    }

    public void PauseAll()
    {
        _isPaused = true;
        ship.Pause();
        enemiesSpawner.Pause();
    }

    public void PlayAll()
    {
        _isPaused = false;
        ship.Play();
        enemiesSpawner.Play();
    }

    public void HideInstructions()
    {
        uiToHowToPlay.SetActive(false);
    }

    public void OnTouch(InputAction.CallbackContext context)
    {
        isTouching = context.phase switch
        {
            InputActionPhase.Started => true,
            InputActionPhase.Canceled => false,
            _ => isTouching
        };
    }

    public bool IsPause()
    {
        return _isPaused;
    }
}