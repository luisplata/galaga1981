public class LogicLifeOfPlayer
{
    private readonly ILifeOfPlayerControllerView lifeOfPlayerControllerView;
    public bool EstaVivo { get; set; }

    public LogicLifeOfPlayer(ILifeOfPlayerControllerView lifeOfPlayerControllerView)
    {
        this.lifeOfPlayerControllerView = lifeOfPlayerControllerView;
        EstaVivo = true;
    }

    public void Murio(bool IsEnemy, bool isBulletEnemy)
    {
        if (IsEnemy || isBulletEnemy)
        {
           lifeOfPlayerControllerView.PlayerDied();
        }
    }
}