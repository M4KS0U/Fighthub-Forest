using System;

public class ActionNode : DecisionNode
{
    private Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }

    public override bool Evaluate()
    {
        action.Invoke();
        return true;
    }
}
