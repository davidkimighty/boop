namespace boop
{
    public interface IStateTransition
    {
        void Transition(Interactable.State state, bool selected, bool instant = false);
    }
}