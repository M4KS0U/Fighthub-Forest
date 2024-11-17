using System;

public class Decision : DecisionNode
{
    public DecisionNode trueNode;
    public DecisionNode falseNode;
    public Func<bool> decisionCondition;

    public override bool Evaluate()
    {
        if (decisionCondition.Invoke())
        {
            trueNode.Evaluate();
            return true;
        }
        else
        {
            falseNode.Evaluate();
            return false;
        }
    }
}
