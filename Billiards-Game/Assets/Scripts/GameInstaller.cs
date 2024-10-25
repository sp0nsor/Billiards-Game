using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGameView>().To<GameView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IMatchView>().To<MatchView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IBallsContainer>().To<BallsContainer>().FromComponentInHierarchy().AsSingle();


    }
}
