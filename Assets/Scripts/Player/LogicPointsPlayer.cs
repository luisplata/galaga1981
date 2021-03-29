public class LogicPointsPlayer
{
    private readonly IControllerPointsView view;
    private readonly IPlayerPrefsAdapter playerPrefsAdapter;

    public LogicPointsPlayer(IControllerPointsView view,IPlayerPrefsAdapter playerPrefsAdapter)
    {
        this.view = view;
        this.playerPrefsAdapter = playerPrefsAdapter;
        CheckPoints();
    }

    private void CheckPoints()
    {
        if (!playerPrefsAdapter.HasKey("score"))
        {
            playerPrefsAdapter.SetInt("score", 0);
        }
        view.ShowPuntuaction(GetPoints().ToString());
    }

    public void PointsUp(int pointsNews)
    {
        //ademas de aumentar la puntuacion actualizamos la puntuacion de la UI
        var points = GetPoints() + pointsNews;
        playerPrefsAdapter.SetInt("score", points);
        view.ShowPuntuaction(points.ToString());
    }

    public int GetPoints()
    {
        return playerPrefsAdapter.GetInt("score");
    }
}