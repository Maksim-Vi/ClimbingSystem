namespace Climb
{
    public class PlayerStateMachine : StateManager<PlayerStateMachine.PlayerState>
    {
        public enum PlayerState
        {
            Idle,
            Walk,
            Run
        }

        private void Awake() 
        {
            CurrentState = States[PlayerState.Idle];
        }
    }
}