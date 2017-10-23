
// TODO: Make state generic
public abstract class State
{
    protected AI _ai;
    public abstract void Tick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public State(AI ai)
    {
        _ai = ai;
    }
}
